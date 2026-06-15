using kinetica.Utils;
using Microsoft.Extensions.Logging;

namespace kinetica;

/// <summary>
/// Object that permits efficient retrieval of records from GPUdb, with support
    /// for multi-head access and HA failover.
    ///
    /// RecordRetriever instances are thread safe and may be used from any number
    /// of threads simultaneously.
    ///
    /// This implementation matches the Rust RecordRetriever with full HA support.
    /// </summary>
    /// <typeparam name="T">The type of object being retrieved.</typeparam>
    public class RecordRetriever<T> where T : new()
    {
        #region Fields

        private readonly Kinetica _kinetica;
        private readonly string _tableName;
        private readonly KineticaType _ktype;
        private readonly RecordKeyBuilder<T>? _shardKeyBuilder;
        private readonly bool _multiHeadEnabled;
        private readonly Random _random;
        private readonly ILogger _logger;

        // HA Failover fields
        private readonly int _dbHaRingSize;
        private volatile int _numClusterSwitches;
        private volatile Uri? _currentHeadNodeUrl;
        private readonly object _haLock = new object();

        // Mutable state for HA failover
        private volatile IList<int>? _mutableRoutingTable;
        private volatile IList<WorkerQueue<T>>? _mutableWorkerQueues;
        private long _shardVersion;
        private long _shardUpdateTime;

        // Immutable initial state
        private readonly IList<int>? _routingTable;
        private readonly IList<WorkerQueue<T>> _workerQueues;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Kinetica connection.
        /// </summary>
        public Kinetica KineticaDB => _kinetica;

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName => _tableName;

        /// <summary>
        /// Gets the number of cluster switches due to HA failover.
        /// </summary>
        public int NumClusterSwitches => _numClusterSwitches;

        /// <summary>
        /// Gets the HA ring size.
        /// </summary>
        public int HARingSize => _dbHaRingSize;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a RecordRetriever object with the given parameters.
        /// </summary>
        /// <param name="kdb">The Kinetica connection.</param>
        /// <param name="table_name">The table name to retrieve from.</param>
        /// <param name="ktype">The KineticaType for the table.</param>
        /// <param name="workers">Optional worker list for multi-head retrieval.</param>
        public RecordRetriever(Kinetica kdb, string table_name,
                               KineticaType ktype,
                               WorkerList? workers = null)
        {
            _kinetica = kdb ?? throw new ArgumentNullException(nameof(kdb));
            _tableName = table_name ?? throw new ArgumentNullException(nameof(table_name));
            _ktype = ktype ?? throw new ArgumentNullException(nameof(ktype));
            _logger = _kinetica.LoggerFactory.CreateLogger("Kinetica.RecordRetriever");

            // Initialize HA state
            _dbHaRingSize = kdb.HAManager?.HARingSize ?? 1;
            _numClusterSwitches = kdb.NumClusterSwitches;
            _currentHeadNodeUrl = kdb.URL;

            // Set up the shard key builder
            _shardKeyBuilder = new RecordKeyBuilder<T>(false, _ktype);
            if (!_shardKeyBuilder.hasKey())
                _shardKeyBuilder = null;

            // Set up the worker queues
            _workerQueues = new List<WorkerQueue<T>>();
            try
            {
                // If no workers are given, try to get them from Kinetica
                if (workers == null || workers.Count == 0)
                {
                    workers = new WorkerList(kdb);
                }

                // If we end up with multiple workers, use those
                if (workers != null && workers.Count > 0)
                {
                    // Add worker queues per worker
                    foreach (var worker_url in workers)
                    {
                        // Skip removed ranks (null URLs)
                        if (worker_url == null) continue;

                        string get_records_worker_url_str = worker_url.ToString() + "get/records";
                        Uri url = new Uri(get_records_worker_url_str);
                        WorkerQueue<T> worker_queue = new WorkerQueue<T>(url);
                        ((List<WorkerQueue<T>>)_workerQueues).Add(worker_queue);
                    }

                    // Get the worker rank information from Kinetica
                    _routingTable = kdb.adminShowShards().rank;

                    // Check that enough worker URLs are specified
                    for (int i = 0; i < _routingTable.Count; ++i)
                    {
                        if (_routingTable[i] > _workerQueues.Count)
                            throw new KineticaException("Not enough worker URLs specified.");
                    }

                    _multiHeadEnabled = true;
                }
                else // multihead is NOT turned on; use the regular Kinetica IP address
                {
                    string get_records_url_str = kdb.URL.ToString() + "get/records";
                    Uri url = new Uri(get_records_url_str);
                    WorkerQueue<T> worker_queue = new WorkerQueue<T>(url);
                    ((List<WorkerQueue<T>>)_workerQueues).Add(worker_queue);
                    _routingTable = null;
                    _multiHeadEnabled = false;
                    _logger.LogInformation(
                        "RecordRetriever for table {Table} is running in degraded mode; " +
                        "requests will be sent to the head node rather than be routed directly to ranks.",
                        _tableName);
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString());
            }

            // Create the random number generator
            _random = new Random((int)DateTime.Now.Ticks);
        }

        #endregion

        #region HA Failover Methods

        /// <summary>
        /// Attempts to force a failover to another cluster in the HA ring.
        /// Uses the Kinetica class's ForceHAFailover method for consistent failover handling.
        /// </summary>
        private bool ForceFailover(Uri oldUrl, int oldClusterSwitchCount)
        {
            if (_kinetica.HAManager == null || _dbHaRingSize <= 1)
            {
                return false;
            }

            // Use Kinetica's centralized failover method
            var newUrl = _kinetica.ForceHAFailover(oldUrl, oldClusterSwitchCount);
            if (newUrl != null)
            {
                // Check worker ranks if multi-head is enabled
                bool isClusterHealthy = true;

                if (_multiHeadEnabled)
                {
                    try
                    {
                        var workers = new WorkerList(_kinetica);
                        if (workers.Count == 0)
                        {
                            isClusterHealthy = false;
                        }
                    }
                    catch
                    {
                        isClusterHealthy = false;
                    }
                }

                if (isClusterHealthy)
                {
                    lock (_haLock)
                    {
                        _currentHeadNodeUrl = newUrl;
                        _numClusterSwitches = _kinetica.NumClusterSwitches;
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates worker queues after a cluster switch or shard version change.
        /// </summary>
        private bool UpdateWorkerQueues(int countClusterSwitches, bool doReconstructWorkerUrls)
        {
            var reconstructWorkerUrls = doReconstructWorkerUrls && _multiHeadEnabled;

            try
            {
                var shardInfo = _kinetica.adminShowShards();
                var newShardVersion = shardInfo.version;

                if (Interlocked.Read(ref _shardVersion) == newShardVersion)
                {
                    var currNumClusterSwitches = _kinetica.NumClusterSwitches;
                    if (countClusterSwitches == currNumClusterSwitches)
                    {
                        if (reconstructWorkerUrls)
                        {
                            return ReconstructWorkerUrls();
                        }
                        return false;
                    }

                    lock (_haLock)
                    {
                        _numClusterSwitches = currNumClusterSwitches;
                    }
                }

                Interlocked.Exchange(ref _shardVersion, newShardVersion);
                Interlocked.Exchange(ref _shardUpdateTime, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                lock (_haLock)
                {
                    _mutableRoutingTable = shardInfo.rank;
                    _currentHeadNodeUrl = _kinetica.URL;
                    _numClusterSwitches = _kinetica.NumClusterSwitches;
                }

                if (reconstructWorkerUrls)
                {
                    ReconstructWorkerUrls();
                }

                return true;
            }
            catch (Exception ex) when (Kinetica.IsConnectionError(ex))
            {
                return false;
            }
        }

        /// <summary>
        /// Reconstructs worker URLs after a failover.
        /// </summary>
        private bool ReconstructWorkerUrls()
        {
            try
            {
                var newWorkerList = new WorkerList(_kinetica);

                if (newWorkerList.Count == 0)
                {
                    return false;
                }

                var newQueues = new List<WorkerQueue<T>>();
                foreach (var workerUrl in newWorkerList)
                {
                    // Skip removed ranks (null URLs)
                    if (workerUrl == null) continue;

                    var urlStr = workerUrl.ToString() + "get/records";
                    var url = new Uri(urlStr);
                    newQueues.Add(new WorkerQueue<T>(url));
                }

                lock (_haLock)
                {
                    _mutableWorkerQueues = newQueues;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the effective worker queues (mutable if available, otherwise original).
        /// </summary>
        private IList<WorkerQueue<T>> GetEffectiveWorkerQueues()
        {
            return _mutableWorkerQueues ?? _workerQueues;
        }

        /// <summary>
        /// Gets the effective routing table (mutable if available, otherwise original).
        /// </summary>
        private IList<int>? GetEffectiveRoutingTable()
        {
            return _mutableRoutingTable ?? _routingTable;
        }

        /// <summary>
        /// Handles connection errors with failover and retry logic.
        /// Uses the Kinetica class's centralized IsConnectionError method for consistent error detection.
        /// </summary>
        private bool HandleConnectionError(Exception ex, long retrievalAttemptTimestamp)
        {
            if (!Kinetica.IsConnectionError(ex))
            {
                return false;
            }

            var currUrl = _currentHeadNodeUrl;
            var currentCountClusterSwitches = _numClusterSwitches;

            bool didFailoverSucceed = false;

            if (currUrl != null && _dbHaRingSize > 1)
            {
                didFailoverSucceed = ForceFailover(currUrl, currentCountClusterSwitches);
            }

            var updatedWorkerQueues = UpdateWorkerQueues(currentCountClusterSwitches, true);

            var shardUpdateTime = Interlocked.Read(ref _shardUpdateTime);
            var retry = didFailoverSucceed || updatedWorkerQueues || retrievalAttemptTimestamp < shardUpdateTime;

            return retry;
        }

        #endregion

        #region Record Retrieval

        /// <summary>
        /// Retrieves records for a given shard key, optionally further limited by an
        /// additional expression. All records matching the key and satisfying the
        /// expression will be returned, up to the system-defined limit. For
        /// multi-head mode the request will be sent directly to the appropriate
        /// worker.
        /// </summary>
        ///
        /// <param name="record">The record based on whose shard column values
        /// records will be fetched from the table.</param>
        /// <param name="expression">An optional expression. Default is null.</param>
        ///
        /// <returns>A GetRecordsResponse object with the decoded retrieved values.</returns>
        public GetRecordsResponse<T> getRecordsByKey(T record, string? expression = null)
        {
            if (_shardKeyBuilder == null)
                throw new KineticaException("Cannot get by key from unsharded table: " + _tableName);

            var retrievalAttemptTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var currentCountClusterSwitches = _numClusterSwitches;

            Exception? lastException = null;
            int maxRetries = 3;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Build the expression
                    string? full_expression = _shardKeyBuilder.buildExpression(record);
                    if (full_expression == null)
                        throw new KineticaException("No expression could be made from given record.");
                    if (expression != null)
                        full_expression = full_expression + " and (" + expression + ")";

                    // Create the options map for the /get/records call
                    IDictionary<string, string> options = new Dictionary<string, string>();
                    options[GetRecordsRequest.Options.EXPRESSION] = full_expression;
                    options[GetRecordsRequest.Options.FAST_INDEX_LOOKUP] = GetRecordsRequest.Options.TRUE;

                    // Create a /get/records request packet
                    GetRecordsRequest request = new GetRecordsRequest(_tableName,
                                                                      0, Kinetica.END_OF_SET,
                                                                      options);

                    // Submit the /get/records request
                    var effectiveRoutingTable = GetEffectiveRoutingTable();
                    if (effectiveRoutingTable == null)
                    {
                        // No routing information is available; talk to rank-0
                        return _kinetica.getRecords<T>(request);
                    }
                    else
                    {
                        // Talk to the appropriate worker rank
                        RawGetRecordsResponse raw_response = new RawGetRecordsResponse();
                        GetRecordsResponse<T> decoded_response = new GetRecordsResponse<T>();

                        // Find the appropriate worker rank
                        RecordKey shard_key = _shardKeyBuilder.build(record);
                        var effectiveWorkerQueues = GetEffectiveWorkerQueues();
                        Uri url = effectiveWorkerQueues[shard_key.route(effectiveRoutingTable)].url;

                        // Make the call using SubmitRequestRaw for direct URL (no HA failover - handled by retriever)
                        raw_response = _kinetica.SubmitRequestRaw<RawGetRecordsResponse>(url, request);

                        // Set up the values of the decoded response properly
                        decoded_response.table_name = raw_response.table_name;
                        decoded_response.type_name = raw_response.type_name;
                        decoded_response.type_schema = raw_response.type_schema;
                        decoded_response.has_more_records = raw_response.has_more_records;
                        decoded_response.total_number_of_records = raw_response.total_number_of_records;

                        // Decode the records
                        _kinetica.DecodeRawBinaryDataUsingRecordType(_ktype,
                                                                     raw_response.records_binary,
                                                                     decoded_response.data);
                        return decoded_response;
                    }
                }
                catch (Exception ex) when (Kinetica.IsConnectionError(ex))
                {
                    lastException = ex;

                    // Attempt HA failover using centralized handling
                    if (HandleConnectionError(ex, retrievalAttemptTimestamp))
                    {
                        // Retry after successful failover
                        continue;
                    }

                    // No successful failover, throw the exception
                    throw new KineticaException("Error in retrieving records by key: " + ex.Message, ex);
                }
                catch (KineticaException ex)
                {
                    throw new KineticaException("Error in retrieving records by key: ", ex);
                }
                catch (Exception ex)
                {
                    throw new KineticaException("Error in retrieving records by key: ", ex);
                }
            }

            // All retries exhausted
            throw new KineticaException("Error in retrieving records by key after " + maxRetries + " retries: " +
                                        (lastException?.Message ?? "Unknown error"), lastException);
        }

        #endregion
    }
