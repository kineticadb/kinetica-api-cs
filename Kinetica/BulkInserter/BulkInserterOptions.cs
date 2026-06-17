namespace kinetica;

/// <summary>
/// Configuration options for the <see cref="BulkInserter{T}"/>.
/// </summary>
public sealed class BulkInserterOptions
{
    /// <summary>
    /// Number of records per batch before triggering a flush.
    /// Default: 10,000. Recommended range: 5,000 - 50,000.
    /// </summary>
    public int BatchSize { get; set; } = 10_000;

    /// <summary>
    /// Maximum number of batches that can be in-flight at once.
    /// Used for backpressure control.
    /// Default: 100.
    /// </summary>
    public int MaxInFlightBatches { get; set; } = 100;

    /// <summary>
    /// Number of stripes per worker queue to reduce lock contention.
    /// Default: Environment.ProcessorCount (number of CPU cores).
    /// </summary>
    public int NumStripes { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Maximum number of concurrent flush workers.
    /// Default: Environment.ProcessorCount * 2.
    /// </summary>
    public int MaxFlushWorkers { get; set; } = Math.Max(4, Environment.ProcessorCount * 2);

    /// <summary>
    /// Maximum number of retry attempts for failed inserts.
    /// Default: 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Interval in seconds between automatic flushes.
    /// Set to 0 to disable timed flushing.
    /// Default: 0 (disabled).
    /// </summary>
    public int FlushIntervalSeconds { get; set; } = 0;

    /// <summary>
    /// Options to pass to the insert_records API.
    /// Common options: UPDATE_ON_EXISTING_PK, IGNORE_EXISTING_PK, etc.
    /// </summary>
    public Dictionary<string, string> InsertOptions { get; set; } = new();

    /// <summary>
    /// Optional batch insertion listener for callbacks on batch completion.
    /// </summary>
    public IBatchInsertionListener? BatchListener { get; set; }

    /// <summary>
    /// Optional worker list override. If null, workers are auto-discovered.
    /// </summary>
    public Utils.WorkerList? WorkerList { get; set; }

    /// <summary>
    /// Maximum number of errors to queue before discarding.
    /// Default: 10,000.
    /// </summary>
    public int MaxErrorQueueSize { get; set; } = 10_000;

    /// <summary>
    /// Timeout for close operations in milliseconds.
    /// Default: 60,000 (60 seconds).
    /// </summary>
    public int CloseTimeoutMs { get; set; } = 60_000;

    /// <summary>
    /// Creates options with default values.
    /// </summary>
    public BulkInserterOptions() { }

    /// <summary>
    /// Creates options with specified batch size.
    /// </summary>
    public BulkInserterOptions(int batchSize)
    {
        BatchSize = batchSize;
    }

    /// <summary>
    /// Creates a copy of these options.
    /// </summary>
    public BulkInserterOptions Clone()
    {
        return new BulkInserterOptions
        {
            BatchSize = BatchSize,
            MaxInFlightBatches = MaxInFlightBatches,
            NumStripes = NumStripes,
            MaxFlushWorkers = MaxFlushWorkers,
            MaxRetries = MaxRetries,
            FlushIntervalSeconds = FlushIntervalSeconds,
            InsertOptions = new Dictionary<string, string>(InsertOptions),
            BatchListener = BatchListener,
            WorkerList = WorkerList,
            MaxErrorQueueSize = MaxErrorQueueSize,
            CloseTimeoutMs = CloseTimeoutMs
        };
    }
}
