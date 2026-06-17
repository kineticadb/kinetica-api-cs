namespace kinetica.Utils;

/// <summary>
/// Object that permits efficient retrieval of records from GPUdb using GenericRecord
/// and dictionary-based records, with support for multi-head access and HA failover.
///
/// This is used by the ADO.NET driver for key lookup operations where we don't have
/// strongly-typed record classes. It uses SQL execution with optimized expression
/// building based on shard keys.
///
/// GenericRecordRetriever instances are thread safe and may be used from any number
/// of threads simultaneously.
/// </summary>
public class GenericRecordRetriever
{
    #region Fields

    private readonly Kinetica _kinetica;
    private readonly string _tableName;
    private readonly KineticaType _ktype;
    private readonly GenericRecordKeyBuilder? _shardKeyBuilder;
    private readonly bool _multiHeadEnabled;
    private readonly Random _random;

    // HA Failover fields
    private readonly int _dbHaRingSize;
    private volatile int _numClusterSwitches;
    private volatile Uri? _currentHeadNodeUrl;
    private readonly object _haLock = new object();

    // Mutable state for HA failover
    private volatile IList<int>? _mutableRoutingTable;
    private volatile IList<Uri>? _mutableWorkerUrls;
    private long _shardVersion;
    private long _shardUpdateTime;

    // Immutable initial state
    private readonly IList<int>? _routingTable;
    private readonly IList<Uri> _workerUrls;

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
    /// Gets the KineticaType for the table.
    /// </summary>
    public KineticaType KType => _ktype;

    /// <summary>
    /// Gets the number of cluster switches due to HA failover.
    /// </summary>
    public int NumClusterSwitches => _numClusterSwitches;

    /// <summary>
    /// Gets the HA ring size.
    /// </summary>
    public int HARingSize => _dbHaRingSize;

    /// <summary>
    /// Gets whether multi-head retrieval is enabled.
    /// </summary>
    public bool MultiHeadEnabled => _multiHeadEnabled;

    /// <summary>
    /// Gets the shard key column names, or empty if table is not sharded.
    /// </summary>
    public IList<string> ShardKeyColumnNames => _shardKeyBuilder?.GetRoutingColumnNames() ?? new List<string>();

    #endregion

    #region Constructor

    /// <summary>
    /// Create a GenericRecordRetriever for the given table.
    /// </summary>
    /// <param name="kdb">The Kinetica connection.</param>
    /// <param name="tableName">The table name to retrieve from.</param>
    /// <param name="ktype">The KineticaType for the table.</param>
    /// <param name="workers">Optional worker list for multi-head retrieval.</param>
    public GenericRecordRetriever(Kinetica kdb, string tableName,
                                   KineticaType ktype,
                                   WorkerList? workers = null)
    {
        _kinetica = kdb ?? throw new ArgumentNullException(nameof(kdb));
        _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        _ktype = ktype ?? throw new ArgumentNullException(nameof(ktype));

        // Initialize HA state
        _dbHaRingSize = kdb.HAManager?.HARingSize ?? 1;
        _numClusterSwitches = kdb.NumClusterSwitches;
        _currentHeadNodeUrl = kdb.URL;

        // Set up the shard key builder
        _shardKeyBuilder = new GenericRecordKeyBuilder(false, ktype);
        if (!_shardKeyBuilder.HasKey())
            _shardKeyBuilder = null;

        // Set up the worker URLs
        _workerUrls = new List<Uri>();
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
                foreach (var workerUrl in workers)
                {
                    if (workerUrl == null) continue;
                    ((List<Uri>)_workerUrls).Add(workerUrl);
                }

                // Get the worker rank information from Kinetica
                _routingTable = kdb.adminShowShards().rank;

                // Check that enough worker URLs are specified
                for (int i = 0; i < _routingTable.Count; ++i)
                {
                    if (_routingTable[i] > _workerUrls.Count)
                        throw new KineticaException("Not enough worker URLs specified.");
                }

                _multiHeadEnabled = true;
            }
            else
            {
                // Single head mode
                ((List<Uri>)_workerUrls).Add(kdb.URL);
                _routingTable = null;
                _multiHeadEnabled = false;
            }
        }
        catch (Exception ex)
        {
            throw new KineticaException("Error initializing GenericRecordRetriever: " + ex.Message, ex);
        }

        _random = new Random((int)DateTime.Now.Ticks);
    }

    #endregion

    #region HA Failover Methods

    private bool ForceFailover(Uri oldUrl, int oldClusterSwitchCount)
    {
        if (_kinetica.HAManager == null || _dbHaRingSize <= 1)
        {
            return false;
        }

        var newUrl = _kinetica.ForceHAFailover(oldUrl, oldClusterSwitchCount);
        if (newUrl != null)
        {
            bool isClusterHealthy = true;

            if (_multiHeadEnabled)
            {
                try
                {
                    var newWorkers = new WorkerList(_kinetica);
                    if (newWorkers.Count == 0)
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

    private bool ReconstructWorkerUrls()
    {
        try
        {
            var newWorkerList = new WorkerList(_kinetica);

            if (newWorkerList.Count == 0)
            {
                return false;
            }

            var newUrls = new List<Uri>();
            foreach (var workerUrl in newWorkerList)
            {
                if (workerUrl == null) continue;
                newUrls.Add(workerUrl);
            }

            lock (_haLock)
            {
                _mutableWorkerUrls = newUrls;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private IList<Uri> GetEffectiveWorkerUrls()
    {
        return _mutableWorkerUrls ?? _workerUrls;
    }

    private IList<int>? GetEffectiveRoutingTable()
    {
        return _mutableRoutingTable ?? _routingTable;
    }

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
    /// Retrieves records for a given shard key using SQL with fast index lookup.
    /// All records matching the key and satisfying the optional additional expression
    /// will be returned, up to the specified limit.
    /// </summary>
    /// <param name="keyValues">Dictionary of column name -> value for the shard key columns.</param>
    /// <param name="expression">An optional additional filter expression.</param>
    /// <param name="columns">Optional list of columns to retrieve. Null means all columns (SELECT *).</param>
    /// <param name="offset">Starting record offset (default 0).</param>
    /// <param name="limit">Maximum number of records to retrieve (default -9999 means no limit).</param>
    /// <returns>The ExecuteSqlResponse containing the results.</returns>
    public ExecuteSqlResponse GetRecordsByKey(
        IDictionary<string, object?> keyValues,
        string? expression = null,
        IList<string>? columns = null,
        long offset = 0,
        long limit = -9999)
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
                // Build the expression from key values
                string? keyExpression = _shardKeyBuilder.BuildExpression(keyValues);
                if (keyExpression == null)
                    throw new KineticaException("No expression could be made from given key values.");

                string fullExpression = keyExpression;
                if (!string.IsNullOrEmpty(expression))
                    fullExpression = keyExpression + " AND (" + expression + ")";

                // Build the SQL query
                string columnList = (columns != null && columns.Count > 0)
                    ? string.Join(", ", columns)
                    : "*";

                string sql = $"SELECT {columnList} FROM {_tableName} WHERE {fullExpression}";

                // Execute the SQL query (fast index lookup is automatic for primary/shard key lookups)
                return _kinetica.executeSql(sql, offset, limit, null, null, null);
            }
            catch (Exception ex) when (Kinetica.IsConnectionError(ex))
            {
                lastException = ex;

                if (HandleConnectionError(ex, retrievalAttemptTimestamp))
                {
                    continue; // Retry after successful failover
                }

                throw new KineticaException("Error in retrieving records by key: " + ex.Message, ex);
            }
            catch (KineticaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new KineticaException("Error in retrieving records by key: " + ex.Message, ex);
            }
        }

        throw new KineticaException("Error in retrieving records by key after " + maxRetries + " retries: " +
                                    (lastException?.Message ?? "Unknown error"), lastException);
    }

    /// <summary>
    /// Retrieves records using a SQL WHERE clause expression with fast index lookup optimization.
    /// </summary>
    /// <param name="whereExpression">SQL WHERE clause expression.</param>
    /// <param name="columns">Optional list of columns to retrieve. Null means all columns.</param>
    /// <param name="offset">Starting record offset (default 0).</param>
    /// <param name="limit">Maximum number of records to retrieve (default -9999 means no limit).</param>
    /// <returns>The ExecuteSqlResponse containing the results.</returns>
    public ExecuteSqlResponse GetRecordsByExpression(
        string whereExpression,
        IList<string>? columns = null,
        long offset = 0,
        long limit = -9999)
    {
        // Build the SQL query
        string columnList = (columns != null && columns.Count > 0)
            ? string.Join(", ", columns)
            : "*";

        string sql = $"SELECT {columnList} FROM {_tableName} WHERE {whereExpression}";

        // Execute the SQL query (fast index lookup is automatic for indexed column lookups)
        return _kinetica.executeSql(sql, offset, limit, null, null, null);
    }

    /// <summary>
    /// Checks if the given key values contain all required shard key columns.
    /// </summary>
    /// <param name="keyValues">Dictionary of column name -> value.</param>
    /// <returns>True if all shard key columns are present.</returns>
    public bool HasAllShardKeyValues(IDictionary<string, object?> keyValues)
    {
        if (_shardKeyBuilder == null)
            return false;

        foreach (var columnName in ShardKeyColumnNames)
        {
            if (!keyValues.ContainsKey(columnName))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Computes the worker index that would handle records with the given key values.
    /// This is useful for understanding data distribution.
    /// </summary>
    /// <param name="keyValues">Dictionary of column name -> value for the shard key columns.</param>
    /// <returns>The worker index (0-based), or -1 if routing is not possible.</returns>
    public int GetWorkerIndexForKey(IDictionary<string, object?> keyValues)
    {
        if (_shardKeyBuilder == null || !_multiHeadEnabled)
            return -1;

        var effectiveRoutingTable = GetEffectiveRoutingTable();
        if (effectiveRoutingTable == null)
            return -1;

        var shardKey = _shardKeyBuilder.Build(keyValues);
        if (shardKey == null)
            return -1;

        return shardKey.route(effectiveRoutingTable);
    }

    #endregion
}
