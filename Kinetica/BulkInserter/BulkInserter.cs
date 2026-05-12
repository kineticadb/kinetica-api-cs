using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using kinetica.Utils;
using Records = kinetica.Records;

namespace kinetica;

/// <summary>
/// High-performance bulk inserter for Kinetica with support for multi-head ingest,
    /// parallel processing, backpressure control, and async flush workers.
    ///
    /// This implementation follows the Rust BulkInserter architecture with:
    /// - Stripe-based worker queues to reduce lock contention
    /// - Parallel batch processing using TPL
    /// - Async flush workers with bounded channels for backpressure
    /// - Batch insertion callbacks for monitoring
    /// - Automatic retry with exponential backoff
    ///
    /// Types must implement IShardKeyExtractor for shard-aware routing.
    /// </summary>
    /// <typeparam name="T">The record type to insert. Must implement IShardKeyExtractor.</typeparam>
    public sealed class BulkInserter<T> : IAsyncDisposable, IDisposable
        where T : Records.IShardKeyExtractor
    {
        #region Fields

        private readonly Kinetica _kinetica;
        private readonly string _tableName;
        private readonly KineticaType _ktype;
        private readonly BulkInserterOptions _options;

        // Avro encoder - uses DirectAvroEncoder<T> for POCOs, GenericRecordEncoder for GenericRecord
        private readonly DirectAvroEncoder<T>? _directEncoder;
        private readonly GenericRecordEncoder? _genericRecordEncoder;
        private readonly bool _isGenericRecord;

        // New interface-based key builder (replaces reflection-based RecordKeyBuilder<T>)
        private readonly Records.RecordKeyBuilder? _shardKeyBuilder;
        private readonly IList<int>? _routingTable;

        private readonly StripedWorkerQueue<T>[] _workerQueues;
        private readonly int _numWorkers;

        // Flush channel for async batch processing
        private readonly Channel<FlushJob> _flushChannel;
        private readonly Task[] _flushWorkers;
        private readonly CancellationTokenSource _cts;

        // Backpressure control
        private readonly SemaphoreSlim _inFlightSemaphore;

        // Metrics
        private long _countInserted;
        private long _countUpdated;
        private long _pendingBatches;
        private long _totalBatchesSent;
        private long _totalBatchesFailed;

        // Error queue
        private readonly ConcurrentQueue<InsertError> _errorQueue;
        private int _errorCount;

        // Timed flush
        private Timer? _flushTimer;
        private volatile bool _timedFlushRunning;

        // State
        private volatile bool _isDisposed;
        private volatile bool _isClosed;

        // Random for worker selection when no shard key
        private readonly ThreadLocal<Random> _random;

        // HA Failover fields (mutable for cluster switching)
        private readonly int _dbHaRingSize;
        private volatile int _numClusterSwitches;
        private volatile Uri? _currentHeadNodeUrl;
        private readonly object _haLock = new object();

        // Mutable worker state for HA failover
        private volatile StripedWorkerQueue<T>[]? _mutableWorkerQueues;
        private volatile IList<int>? _mutableRoutingTable;
        private long _shardVersion;  // Use Interlocked for thread-safe access
        private long _shardUpdateTime;  // Use Interlocked for thread-safe access
        private readonly bool _multiHeadEnabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new BulkInserter for the specified table.
        /// </summary>
        /// <param name="kinetica">The Kinetica connection.</param>
        /// <param name="tableName">The target table name.</param>
        /// <param name="ktype">The KineticaType for the table.</param>
        /// <param name="options">Optional configuration options.</param>
        public BulkInserter(
            Kinetica kinetica,
            string tableName,
            KineticaType ktype,
            BulkInserterOptions? options = null)
        {
            _kinetica = kinetica ?? throw new ArgumentNullException(nameof(kinetica));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            _ktype = ktype ?? throw new ArgumentNullException(nameof(ktype));
            _options = options?.Clone() ?? new BulkInserterOptions();

            ValidateOptions();

            _cts = new CancellationTokenSource();
            _errorQueue = new ConcurrentQueue<InsertError>();
            _random = new ThreadLocal<Random>(() => new Random(Environment.CurrentManagedThreadId));

            // Initialize shard key builder using the new interface-based approach
            // This replaces the old reflection-based RecordKeyBuilder<T>
            var tempKeyBuilder = new Records.RecordKeyBuilder(_ktype, isPrimaryKey: false);
            _shardKeyBuilder = tempKeyBuilder.HasKey ? tempKeyBuilder : null;

            // Register the type mapping so AvroEncode works (for backwards compatibility)
            _kinetica.SetKineticaSourceClassToTypeMapping(typeof(T), _ktype);

            // Initialize Avro encoder - use GenericRecordEncoder for GenericRecord, DirectAvroEncoder for POCOs
            _isGenericRecord = typeof(T) == typeof(Records.GenericRecord);
            if (_isGenericRecord)
            {
                _genericRecordEncoder = GenericRecordEncoder.GetOrCreate(_ktype);
                _directEncoder = null;
            }
            else
            {
                _directEncoder = DirectAvroEncoder<T>.GetOrCreate(_ktype);
                _genericRecordEncoder = null;
            }

            // Initialize worker queues
            (_workerQueues, _routingTable) = InitializeWorkerQueues();
            _numWorkers = _workerQueues.Length;
            _multiHeadEnabled = _numWorkers > 1;

            // Initialize HA failover state
            _dbHaRingSize = _kinetica.HAManager?.HARingSize ?? 1;
            _numClusterSwitches = _kinetica.NumClusterSwitches;
            _currentHeadNodeUrl = _kinetica.URL;
            _shardVersion = 0;
            _shardUpdateTime = 0;

            // Initialize backpressure semaphore
            _inFlightSemaphore = new SemaphoreSlim(_options.MaxInFlightBatches, _options.MaxInFlightBatches);

            // Initialize flush channel (unbounded for maximum throughput, like Rust's flume)
            // Backpressure is handled by semaphore only when needed
            _flushChannel = Channel.CreateUnbounded<FlushJob>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

            // Start flush workers
            _flushWorkers = new Task[_options.MaxFlushWorkers];
            for (int i = 0; i < _options.MaxFlushWorkers; i++)
            {
                _flushWorkers[i] = Task.Run(() => FlushWorkerLoop(_cts.Token));
            }

            // Start timed flush if configured (matches Rust implementation)
            if (_options.FlushIntervalSeconds > 0)
            {
                _timedFlushRunning = true;
                _flushTimer = new Timer(
                    TimedFlushCallback,
                    null,
                    TimeSpan.FromSeconds(_options.FlushIntervalSeconds),
                    TimeSpan.FromSeconds(_options.FlushIntervalSeconds));
            }
        }

        private void ValidateOptions()
        {
            if (_options.BatchSize < 1)
                throw new ArgumentException("BatchSize must be at least 1", nameof(_options));
            if (_options.MaxInFlightBatches < 1)
                throw new ArgumentException("MaxInFlightBatches must be at least 1", nameof(_options));
            if (_options.NumStripes < 1)
                throw new ArgumentException("NumStripes must be at least 1", nameof(_options));
            if (_options.MaxFlushWorkers < 1)
                throw new ArgumentException("MaxFlushWorkers must be at least 1", nameof(_options));
        }

        private (StripedWorkerQueue<T>[] queues, IList<int>? routingTable) InitializeWorkerQueues()
        {
            var workers = _options.WorkerList;
            IList<int>? routingTable = null;

            // Try to get workers from Kinetica if not provided
            if (workers == null || workers.Count == 0)
            {
                try
                {
                    workers = new WorkerList(_kinetica);
                }
                catch
                {
                    workers = null;
                }
            }

            var queues = new List<StripedWorkerQueue<T>>();

            if (workers != null && workers.Count > 0)
            {
                // Multi-head ingest mode
                int workerIndex = 0;
                foreach (var workerUrl in workers)
                {
                    // Skip removed ranks (null URLs)
                    if (workerUrl == null)
                    {
                        workerIndex++;
                        continue;
                    }

                    var urlStr = workerUrl.ToString().TrimEnd('/');
                    var insertUrl = new Uri($"{urlStr}/insert/records");
                    queues.Add(new StripedWorkerQueue<T>(
                        insertUrl,
                        workerIndex++,
                        _options.NumStripes,
                        _options.BatchSize));
                }

                // Get routing table
                routingTable = _kinetica.adminShowShards().rank;

                // Validate routing table
                foreach (var rank in routingTable)
                {
                    if (rank > queues.Count)
                    {
                        throw new KineticaException("Not enough worker URLs specified.");
                    }
                }
            }
            else
            {
                // Single-head mode
                var urlStr = _kinetica.URL.ToString().TrimEnd('/');
                var insertUrl = new Uri($"{urlStr}/insert/records");
                queues.Add(new StripedWorkerQueue<T>(
                    insertUrl,
                    0,
                    _options.NumStripes,
                    _options.BatchSize));
            }

            return (queues.ToArray(), routingTable);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total count of records inserted.
        /// </summary>
        public long CountInserted => Interlocked.Read(ref _countInserted);

        /// <summary>
        /// Gets the total count of records updated.
        /// </summary>
        public long CountUpdated => Interlocked.Read(ref _countUpdated);

        /// <summary>
        /// Gets the number of batches currently pending (queued or in-flight).
        /// </summary>
        public long PendingBatches => Interlocked.Read(ref _pendingBatches);

        /// <summary>
        /// Gets the total number of batches sent.
        /// </summary>
        public long TotalBatchesSent => Interlocked.Read(ref _totalBatchesSent);

        /// <summary>
        /// Gets the total number of batches that failed.
        /// </summary>
        public long TotalBatchesFailed => Interlocked.Read(ref _totalBatchesFailed);

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName => _tableName;

        /// <summary>
        /// Gets the number of workers.
        /// </summary>
        public int NumWorkers => _numWorkers;

        /// <summary>
        /// Gets the number of errors in the error queue.
        /// </summary>
        public int ErrorCount => _errorCount;

        /// <summary>
        /// Gets whether timed flush is currently running.
        /// </summary>
        public bool IsTimedFlushRunning => _timedFlushRunning;

        /// <summary>
        /// Gets backpressure metrics.
        /// </summary>
        public BackpressureMetrics GetBackpressureMetrics()
        {
            var available = _inFlightSemaphore.CurrentCount;
            var inFlight = _options.MaxInFlightBatches - available;

            return new BackpressureMetrics
            {
                MaxInFlightBatches = _options.MaxInFlightBatches,
                AvailablePermits = available,
                InFlightBatches = inFlight,
                PendingBatches = Interlocked.Read(ref _pendingBatches),
                UtilizationPercent = (double)inFlight / _options.MaxInFlightBatches * 100
            };
        }

        #endregion

        #region Insert Methods

        /// <summary>
        /// Inserts a single record. Non-blocking unless backpressure is applied.
        /// </summary>
        /// <param name="record">The record to insert.</param>
        public void Insert(T record)
        {
            ThrowIfDisposed();

            var (workerIndex, stripeHash) = ComputeRouting(record);
            var queue = _workerQueues[workerIndex];

            var batch = queue.Add(record, stripeHash, out int stripeIndex);

            if (batch != null)
            {
                EnqueueBatch(new FlushJob(queue.Url, workerIndex, stripeIndex, batch));
            }
        }

        /// <summary>
        /// Inserts a single record with async backpressure control.
        /// Waits if the maximum number of in-flight batches is reached.
        /// </summary>
        /// <param name="record">The record to insert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async ValueTask InsertAsync(T record, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var (workerIndex, stripeHash) = ComputeRouting(record);
            var queue = _workerQueues[workerIndex];

            var batch = queue.Add(record, stripeHash, out int stripeIndex);

            if (batch != null)
            {
                await EnqueueBatchWithBackpressureAsync(
                    new FlushJob(queue.Url, workerIndex, stripeIndex, batch, usedBackpressure: true),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Inserts multiple records with parallel routing computation.
        /// This is the most efficient method for bulk inserts.
        /// </summary>
        /// <param name="records">The records to insert.</param>
        public void InsertBatch(IReadOnlyList<T> records)
        {
            ThrowIfDisposed();

            var count = records.Count;
            if (count == 0)
                return;

            // Pre-allocate grouping dictionary with estimated capacity
            // Key: (workerIndex << 16) | stripeIndex to avoid tuple allocation
            var groups = new Dictionary<int, List<T>>(_numWorkers * _options.NumStripes);

            // Compute routing for all records in parallel, storing results in thread-local buffers
            var routingResults = new (int workerIndex, int stripeIndex)[count];

            Parallel.For(0, count, i =>
            {
                var record = records[i];
                var (workerIndex, stripeHash) = ComputeRouting(record);
                var stripeIndex = (int)(stripeHash & (_workerQueues[workerIndex].NumStripes - 1));
                routingResults[i] = (workerIndex, stripeIndex);
            });

            // Group records by (workerIndex, stripeIndex) - sequential but fast
            for (int i = 0; i < count; i++)
            {
                var (workerIndex, stripeIndex) = routingResults[i];
                var key = (workerIndex << 16) | stripeIndex;

                if (!groups.TryGetValue(key, out var list))
                {
                    list = new List<T>(Math.Min(count, _options.BatchSize));
                    groups[key] = list;
                }
                list.Add(records[i]);
            }

            // Add to queues and collect batches - this is lock-heavy but grouped
            var batches = new List<FlushJob>(groups.Count);

            foreach (var kvp in groups)
            {
                var workerIndex = kvp.Key >> 16;
                var stripeIndex = kvp.Key & 0xFFFF;
                var queue = _workerQueues[workerIndex];
                var groupRecords = kvp.Value;

                var completeBatches = queue.AddRangeToStripe(groupRecords, stripeIndex);

                foreach (var batch in completeBatches)
                {
                    batches.Add(new FlushJob(queue.Url, workerIndex, stripeIndex, batch));
                }
            }

            // Enqueue all batches
            foreach (var batch in batches)
            {
                EnqueueBatch(batch);
            }
        }

        /// <summary>
        /// Inserts multiple records with async backpressure control.
        /// </summary>
        /// <param name="records">The records to insert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async ValueTask InsertBatchAsync(IReadOnlyList<T> records, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var count = records.Count;
            if (count == 0)
                return;

            // Pre-allocate grouping structures
            var groups = new Dictionary<int, List<T>>(_numWorkers * _options.NumStripes);
            var routingResults = new (int workerIndex, int stripeIndex)[count];

            // Compute routing in parallel on thread pool
            await Task.Run(() =>
            {
                Parallel.For(0, count, i =>
                {
                    var record = records[i];
                    var (workerIndex, stripeHash) = ComputeRouting(record);
                    var stripeIndex = (int)(stripeHash & (_workerQueues[workerIndex].NumStripes - 1));
                    routingResults[i] = (workerIndex, stripeIndex);
                });
            }, cancellationToken);

            // Group records by (workerIndex, stripeIndex) - sequential but fast
            for (int i = 0; i < count; i++)
            {
                var (workerIndex, stripeIndex) = routingResults[i];
                var key = (workerIndex << 16) | stripeIndex;

                if (!groups.TryGetValue(key, out var list))
                {
                    list = new List<T>(Math.Min(count, _options.BatchSize));
                    groups[key] = list;
                }
                list.Add(records[i]);
            }

            // Collect batches
            var batches = new List<FlushJob>(groups.Count);

            foreach (var kvp in groups)
            {
                var workerIndex = kvp.Key >> 16;
                var stripeIndex = kvp.Key & 0xFFFF;
                var queue = _workerQueues[workerIndex];
                var groupRecords = kvp.Value;

                var completeBatches = queue.AddRangeToStripe(groupRecords, stripeIndex);

                foreach (var batch in completeBatches)
                {
                    batches.Add(new FlushJob(queue.Url, workerIndex, stripeIndex, batch, usedBackpressure: true));
                }
            }

            // Enqueue all batches with backpressure
            foreach (var batch in batches)
            {
                await EnqueueBatchWithBackpressureAsync(batch, cancellationToken);
            }
        }

        #endregion

        #region Routing

        private (int workerIndex, long stripeHash) ComputeRouting(T record)
        {
            int workerIndex;
            long stripeHash;

            if (_routingTable == null)
            {
                // Single worker mode
                workerIndex = 0;
                stripeHash = ComputeStripeHash(record);
            }
            else if (_shardKeyBuilder == null)
            {
                // No shard key, use random worker
                workerIndex = _random.Value!.Next(_numWorkers);
                stripeHash = ComputeStripeHash(record);
            }
            else
            {
                // Route based on shard key using the new IShardKeyExtractor interface
                var shardKeyValues = record.GetShardKeyValues();
                var shardKey = _shardKeyBuilder.Build(shardKeyValues);

                if (shardKey != null && shardKey.IsValid)
                {
                    workerIndex = shardKey.Route(_routingTable);
                    stripeHash = shardKey.HashCode();
                }
                else
                {
                    workerIndex = _random.Value!.Next(_numWorkers);
                    stripeHash = ComputeStripeHash(record);
                }
            }

            return (workerIndex, stripeHash);
        }

        private long ComputeStripeHash(T record)
        {
            // Use shard key values hash or record's hash code for stripe distribution
            var shardKeyValues = record.GetShardKeyValues();
            if (shardKeyValues.Count > 0)
            {
                // Compute hash from shard key values
                int hash = 17;
                foreach (var (name, value) in shardKeyValues)
                {
                    hash = hash * 31 + (name?.GetHashCode() ?? 0);
                    hash = hash * 31 + value.GetHashCode();
                }
                return (uint)hash;
            }
            return (uint)(record?.GetHashCode() ?? 0);
        }

        #endregion

        #region Flush

        /// <summary>
        /// Flushes all queued records to Kinetica.
        /// </summary>
        public void Flush()
        {
            FlushAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Flushes all queued records to Kinetica asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var batches = new List<FlushJob>();

            // Collect all partial batches from all worker queues
            foreach (var queue in _workerQueues)
            {
                var flushedBatches = queue.FlushAll();
                foreach (var (stripeIndex, batch) in flushedBatches)
                {
                    batches.Add(new FlushJob(queue.Url, queue.WorkerIndex, stripeIndex, batch, usedBackpressure: true));
                }
            }

            // Enqueue all batches with backpressure
            foreach (var batch in batches)
            {
                await EnqueueBatchWithBackpressureAsync(batch, cancellationToken);
            }
        }

        private void EnqueueBatch(FlushJob job)
        {
            Interlocked.Increment(ref _pendingBatches);

            // With unbounded channel, TryWrite always succeeds (non-blocking)
            if (!_flushChannel.Writer.TryWrite(job))
            {
                // Should never happen with unbounded channel, but handle gracefully
                Interlocked.Decrement(ref _pendingBatches);
                ProcessBatch(job).GetAwaiter().GetResult();
            }
        }

        private async ValueTask EnqueueBatchWithBackpressureAsync(FlushJob job, CancellationToken cancellationToken)
        {
            // Wait for a permit from the semaphore (backpressure control)
            // This limits the number of in-flight batches to prevent memory overflow
            await _inFlightSemaphore.WaitAsync(cancellationToken);

            Interlocked.Increment(ref _pendingBatches);

            // With unbounded channel, TryWrite always succeeds (non-blocking)
            if (!_flushChannel.Writer.TryWrite(job))
            {
                // Should never happen with unbounded channel, but handle gracefully
                Interlocked.Decrement(ref _pendingBatches);
                _inFlightSemaphore.Release();
                await ProcessBatch(job);
            }
        }

        #endregion

        #region Timed Flush

        /// <summary>
        /// Callback for timed flush. Matches Rust implementation by checking
        /// _timedFlushRunning flag before flushing to avoid flush during shutdown.
        /// </summary>
        private void TimedFlushCallback(object? state)
        {
            // Check if timed flush is still running (matches Rust's double-check pattern)
            if (!_timedFlushRunning || _isClosed || _isDisposed)
                return;

            try
            {
                // Use Task.Run to avoid blocking the timer thread, but properly await
                Task.Run(async () =>
                {
                    // Double-check after potential await (matches Rust pattern)
                    if (!_timedFlushRunning || _isClosed || _isDisposed)
                        return;

                    await FlushAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            catch
            {
                // Suppress exceptions in timer callback to prevent timer from stopping
                // Errors are already captured in the error queue by ProcessBatch
            }
        }

        #endregion

        #region Flush Worker

        private async Task FlushWorkerLoop(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var job in _flushChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        await ProcessBatch(job);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _pendingBatches);
                        // Only release semaphore permit if this batch was enqueued with backpressure
                        // This matches the Rust implementation where insert() is unbounded but
                        // insert_with_backpressure() uses semaphore-based flow control
                        if (job.UsedBackpressure)
                        {
                            _inFlightSemaphore.Release();
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal shutdown
            }
        }

        private async Task ProcessBatch(FlushJob job)
        {
            if (job.Records.Count == 0)
                return;

            var sw = Stopwatch.StartNew();
            double encodeTimeMs = 0;
            double networkTimeMs = 0;
            int retryAttempt = 0;
            Exception? lastException = null;
            bool haFailoverAttempted = false;

            // Capture the insertion attempt timestamp for HA failover logic
            var insertionAttemptTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Get current worker URL (may be updated after failover)
            var currentWorkerUrl = job.WorkerUrl;

            while (retryAttempt <= _options.MaxRetries)
            {
                // Check for updated worker URL after a failover has occurred
                if (haFailoverAttempted && _mutableWorkerQueues != null)
                {
                    var effectiveQueues = GetEffectiveWorkerQueues();
                    if (job.WorkerIndex < effectiveQueues.Length)
                    {
                        currentWorkerUrl = effectiveQueues[job.WorkerIndex].Url;
                    }
                }

                try
                {
                    // Encode records and build request
                    var encodeSw = Stopwatch.StartNew();
                    var encodedRecords = EncodeRecords(job.Records);
                    byte[] requestBytes = DirectRequestEncoder.Encode(_tableName, encodedRecords, _options.InsertOptions);
                    encodeTimeMs = encodeSw.Elapsed.TotalMilliseconds;

                    // Send request using Kinetica's centralized submission method
                    var networkSw = Stopwatch.StartNew();
                    var rawResponse = await Task.Run(() =>
                        _kinetica.SubmitRequestRawBytes(currentWorkerUrl, requestBytes)).ConfigureAwait(false);

                    if (rawResponse.status != "OK")
                    {
                        throw new KineticaException(rawResponse.message ?? "Unknown server error");
                    }

                    InsertRecordsResponse response = _kinetica.AvroDecode<InsertRecordsResponse>(rawResponse.data);
                    networkTimeMs = networkSw.Elapsed.TotalMilliseconds;

                    // Update metrics
                    Interlocked.Add(ref _countInserted, response.count_inserted);
                    Interlocked.Add(ref _countUpdated, response.count_updated);
                    Interlocked.Increment(ref _totalBatchesSent);

                    // Notify listener
                    _options.BatchListener?.OnBatchInserted(new BatchInsertionResult
                    {
                        WorkerUrl = currentWorkerUrl,
                        BatchSize = job.Records.Count,
                        CountInserted = response.count_inserted,
                        CountUpdated = response.count_updated,
                        Success = true,
                        EncodeTimeMs = encodeTimeMs,
                        NetworkTimeMs = networkTimeMs,
                        TotalTimeMs = sw.Elapsed.TotalMilliseconds,
                        WorkerIndex = job.WorkerIndex,
                        StripeIndex = job.StripeIndex,
                        RetryAttempt = retryAttempt
                    });

                    return; // Success
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    // Check if this is a connection error that warrants HA failover
                    // Use Kinetica's centralized connection error detection
                    if (Kinetica.IsConnectionError(ex) && !haFailoverAttempted && _dbHaRingSize > 1)
                    {
                        // Attempt HA failover
                        var shouldRetry = await HandleConnectionErrorAsync(ex, insertionAttemptTimestamp).ConfigureAwait(false);
                        if (shouldRetry)
                        {
                            haFailoverAttempted = true;
                            // Don't increment retry count for failover, retry immediately
                            continue;
                        }
                    }

                    retryAttempt++;

                    if (retryAttempt <= _options.MaxRetries)
                    {
                        // Exponential backoff
                        await Task.Delay(TimeSpan.FromSeconds(retryAttempt)).ConfigureAwait(false);
                    }
                }
            }

            // All retries failed
            Interlocked.Increment(ref _totalBatchesFailed);

            // Queue error
            QueueError(new InsertError
            {
                WorkerUrl = currentWorkerUrl,
                RecordCount = job.Records.Count,
                Message = lastException?.Message ?? "Unknown error",
                Exception = lastException,
                RetryAttempt = retryAttempt - 1
            });

            // Notify listener
            _options.BatchListener?.OnBatchInserted(new BatchInsertionResult
            {
                WorkerUrl = currentWorkerUrl,
                BatchSize = job.Records.Count,
                Success = false,
                ErrorMessage = lastException?.Message,
                EncodeTimeMs = encodeTimeMs,
                NetworkTimeMs = networkTimeMs,
                TotalTimeMs = sw.Elapsed.TotalMilliseconds,
                WorkerIndex = job.WorkerIndex,
                StripeIndex = job.StripeIndex,
                RetryAttempt = retryAttempt - 1
            });
        }

        private List<byte[]> EncodeRecords(IReadOnlyList<T> records)
        {
            // Use the appropriate encoder based on record type
            if (_isGenericRecord)
            {
                // For GenericRecord, use the GenericRecordEncoder
                var genericRecords = (IReadOnlyList<Records.GenericRecord>)(object)records;
                return _genericRecordEncoder!.EncodeManyAsList(genericRecords);
            }
            else
            {
                // For POCOs, use the DirectAvroEncoder with compiled property accessors
                return _directEncoder!.EncodeManyAsList(records);
            }
        }

        #endregion

        #region Error Handling

        private void QueueError(InsertError error)
        {
            if (Interlocked.Increment(ref _errorCount) <= _options.MaxErrorQueueSize)
            {
                _errorQueue.Enqueue(error);
            }
            else
            {
                Interlocked.Decrement(ref _errorCount);
            }
        }

        /// <summary>
        /// Drains and returns all errors from the error queue.
        /// </summary>
        public List<InsertError> DrainErrors()
        {
            var errors = new List<InsertError>();
            while (_errorQueue.TryDequeue(out var error))
            {
                Interlocked.Decrement(ref _errorCount);
                errors.Add(error);
            }
            return errors;
        }

        /// <summary>
        /// Peeks at errors without removing them.
        /// </summary>
        public IReadOnlyList<InsertError> PeekErrors()
        {
            return _errorQueue.ToArray();
        }

        #endregion

        #region HA Failover

        /// <summary>
        /// Gets the number of times the cluster has been switched due to failover.
        /// </summary>
        public int NumClusterSwitches => _numClusterSwitches;

        /// <summary>
        /// Gets the HA ring size (number of clusters).
        /// </summary>
        public int HARingSize => _dbHaRingSize;

        /// <summary>
        /// Attempts to force a failover to another cluster in the HA ring.
        /// Uses the Kinetica class's ForceHAFailover method for consistent failover handling.
        /// </summary>
        /// <param name="oldUrl">The URL that failed.</param>
        /// <param name="oldClusterSwitchCount">The cluster switch count before this failover attempt.</param>
        /// <returns>True if failover succeeded, false otherwise.</returns>
        private Task<bool> ForceFailoverAsync(Uri oldUrl, int oldClusterSwitchCount)
        {
            if (_kinetica.HAManager == null || _dbHaRingSize <= 1)
            {
                return Task.FromResult(false);
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
                        // Test getting workers from the new cluster
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
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Updates worker queues after a cluster switch or shard version change.
        /// </summary>
        /// <param name="countClusterSwitches">The cluster switch count when this update was triggered.</param>
        /// <param name="doReconstructWorkerUrls">Whether to reconstruct worker URLs.</param>
        /// <returns>True if worker queues were updated, false otherwise.</returns>
        private async Task<bool> UpdateWorkerQueuesAsync(int countClusterSwitches, bool doReconstructWorkerUrls)
        {
            var reconstructWorkerUrls = doReconstructWorkerUrls && _multiHeadEnabled;

            try
            {
                // Get the latest shard mapping information
                var shardInfo = _kinetica.adminShowShards();
                var newShardVersion = shardInfo.version;

                // No-op if the shard version hasn't changed
                if (Interlocked.Read(ref _shardVersion) == newShardVersion)
                {
                    var currNumClusterSwitches = _kinetica.NumClusterSwitches;
                    if (countClusterSwitches == currNumClusterSwitches)
                    {
                        if (reconstructWorkerUrls)
                        {
                            return await ReconstructWorkerUrlsAsync().ConfigureAwait(false);
                        }
                        return false;
                    }

                    // Update the HA ring node switch counter
                    lock (_haLock)
                    {
                        _numClusterSwitches = currNumClusterSwitches;
                    }
                }

                // Save the new shard version and update time
                Interlocked.Exchange(ref _shardVersion, newShardVersion);
                Interlocked.Exchange(ref _shardUpdateTime, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                // Update the routing table
                lock (_haLock)
                {
                    _mutableRoutingTable = shardInfo.rank;
                    _currentHeadNodeUrl = _kinetica.URL;
                    _numClusterSwitches = _kinetica.NumClusterSwitches;
                }

                // Reconstruct worker URLs if needed
                if (reconstructWorkerUrls)
                {
                    await ReconstructWorkerUrlsAsync().ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex) when (Kinetica.IsConnectionError(ex))
            {
                // Connection error during shard info retrieval
                return false;
            }
        }

        /// <summary>
        /// Reconstructs worker URLs after a failover.
        /// </summary>
        private Task<bool> ReconstructWorkerUrlsAsync()
        {
            try
            {
                // Fetch new worker list from the cluster
                var newWorkerList = new WorkerList(_kinetica);

                if (newWorkerList.Count == 0)
                {
                    return Task.FromResult(false);
                }

                // Build new worker queues
                var newQueues = new List<StripedWorkerQueue<T>>();
                int workerIndex = 0;
                foreach (var workerUrl in newWorkerList)
                {
                    // Skip removed ranks (null URLs)
                    if (workerUrl == null)
                    {
                        workerIndex++;
                        continue;
                    }

                    var urlStr = workerUrl.ToString().TrimEnd('/');
                    var insertUrl = new Uri($"{urlStr}/insert/records");
                    newQueues.Add(new StripedWorkerQueue<T>(
                        insertUrl,
                        workerIndex++,
                        _options.NumStripes,
                        _options.BatchSize));
                }

                // Update mutable worker queues
                lock (_haLock)
                {
                    _mutableWorkerQueues = newQueues.ToArray();
                }

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Gets the effective worker queues (mutable if available, otherwise original).
        /// </summary>
        private StripedWorkerQueue<T>[] GetEffectiveWorkerQueues()
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
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="insertionAttemptTimestamp">The timestamp when the insertion was first attempted.</param>
        /// <returns>True if retry should happen, false otherwise.</returns>
        private async Task<bool> HandleConnectionErrorAsync(Exception ex, long insertionAttemptTimestamp)
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
                didFailoverSucceed = await ForceFailoverAsync(currUrl, currentCountClusterSwitches).ConfigureAwait(false);
            }

            // Update worker queues
            var updatedWorkerQueues = await UpdateWorkerQueuesAsync(currentCountClusterSwitches, true).ConfigureAwait(false);

            var shardUpdateTime = Interlocked.Read(ref _shardUpdateTime);
            var retry = didFailoverSucceed || updatedWorkerQueues || insertionAttemptTimestamp < shardUpdateTime;

            return retry;
        }

        #endregion

        #region Close

        /// <summary>
        /// Flushes all records and waits for all pending batches to complete.
        /// </summary>
        public void Close()
        {
            CloseAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Flushes all records and waits for all pending batches to complete.
        /// This method will wait indefinitely for all batches to be sent to prevent data loss.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (_isClosed)
                return;

            _isClosed = true;

            // Stop timed flush (matches Rust's stop_timed_flush pattern)
            _timedFlushRunning = false;
            _flushTimer?.Dispose();
            _flushTimer = null;

            // Flush remaining records
            await FlushAsync(cancellationToken);

            // Complete the channel - no more jobs can be enqueued
            _flushChannel.Writer.Complete();

            // Wait for all flush workers to complete processing all remaining batches.
            // This is the definitive wait - workers will exit only after the channel is
            // drained (Complete() was called) and all jobs have been processed.
            // We do NOT use a timeout here to prevent data loss.
            try
            {
                await Task.WhenAll(_flushWorkers).WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // User requested cancellation - cancel workers and exit
                _cts.Cancel();
                // Give workers a moment to respond to cancellation
                try
                {
                    await Task.WhenAll(_flushWorkers).WaitAsync(TimeSpan.FromSeconds(1));
                }
                catch
                {
                    // Ignore - best effort cleanup
                }
                throw; // Re-throw the cancellation
            }

            // Verify all batches were processed (defensive check)
            var remainingBatches = Interlocked.Read(ref _pendingBatches);
            if (remainingBatches != 0)
            {
                // This should never happen, but log/handle if it does
                throw new InvalidOperationException(
                    $"BulkInserter.CloseAsync completed but {remainingBatches} batches are still pending. " +
                    "This indicates a bug in the batch processing logic.");
            }
        }

        #endregion

        #region Disposal

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BulkInserter<T>));
        }

        /// <summary>
        /// Disposes the BulkInserter, closing it if not already closed.
        /// </summary>
        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Disposes the BulkInserter asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            try
            {
                if (!_isClosed)
                {
                    await CloseAsync();
                }
            }
            catch
            {
                // Suppress exceptions during disposal
            }
            finally
            {
                _cts.Cancel();
                _cts.Dispose();
                _inFlightSemaphore.Dispose();
                _flushTimer?.Dispose();
                _random.Dispose();
            }
        }

        #endregion

        #region Nested Types

        private readonly struct FlushJob
        {
            public Uri WorkerUrl { get; }
            public int WorkerIndex { get; }
            public int StripeIndex { get; }
            public IReadOnlyList<T> Records { get; }
            public bool UsedBackpressure { get; }

            public FlushJob(Uri workerUrl, int workerIndex, int stripeIndex, IReadOnlyList<T> records, bool usedBackpressure = false)
            {
                WorkerUrl = workerUrl;
                WorkerIndex = workerIndex;
                StripeIndex = stripeIndex;
                Records = records;
                UsedBackpressure = usedBackpressure;
            }
        }

        #endregion
    }

    /// <summary>
    /// Backpressure metrics for monitoring the BulkInserter.
    /// </summary>
    public sealed class BackpressureMetrics
    {
        /// <summary>
        /// Maximum number of in-flight batches allowed.
        /// </summary>
        public int MaxInFlightBatches { get; init; }

        /// <summary>
        /// Number of available permits in the semaphore.
        /// </summary>
        public int AvailablePermits { get; init; }

        /// <summary>
        /// Number of batches currently in-flight.
        /// </summary>
        public int InFlightBatches { get; init; }

        /// <summary>
        /// Number of batches pending (queued + in-flight).
        /// </summary>
        public long PendingBatches { get; init; }

        /// <summary>
        /// Utilization percentage (in-flight / max * 100).
        /// </summary>
        public double UtilizationPercent { get; init; }
    }
