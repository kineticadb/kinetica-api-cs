namespace kinetica;

/// <summary>
/// Result of a batch insertion operation.
    /// </summary>
    public sealed class BatchInsertionResult
    {
        /// <summary>
        /// URL of the worker that processed this batch.
        /// </summary>
        public Uri WorkerUrl { get; init; } = null!;

        /// <summary>
        /// Number of records in the batch.
        /// </summary>
        public int BatchSize { get; init; }

        /// <summary>
        /// Number of records successfully inserted.
        /// </summary>
        public long CountInserted { get; init; }

        /// <summary>
        /// Number of records updated (for upserts).
        /// </summary>
        public long CountUpdated { get; init; }

        /// <summary>
        /// Whether the batch insertion succeeded.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Error message if the batch failed, null otherwise.
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// Time spent encoding records in milliseconds.
        /// </summary>
        public double EncodeTimeMs { get; init; }

        /// <summary>
        /// Time spent on network I/O in milliseconds.
        /// </summary>
        public double NetworkTimeMs { get; init; }

        /// <summary>
        /// Total time for the batch operation in milliseconds.
        /// </summary>
        public double TotalTimeMs { get; init; }

        /// <summary>
        /// Worker index that processed this batch.
        /// </summary>
        public int WorkerIndex { get; init; }

        /// <summary>
        /// Stripe index from which this batch originated.
        /// </summary>
        public int StripeIndex { get; init; }

        /// <summary>
        /// Retry attempt number (0 = first attempt).
        /// </summary>
        public int RetryAttempt { get; init; }

        /// <summary>
        /// Timestamp when the batch was completed.
        /// </summary>
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Listener interface for batch insertion events.
    /// Implementations must be thread-safe as callbacks may occur concurrently.
    /// </summary>
    public interface IBatchInsertionListener
    {
        /// <summary>
        /// Called when a batch insertion completes (success or failure).
        /// This method is called asynchronously and should not block.
        /// </summary>
        /// <param name="result">The result of the batch insertion.</param>
        void OnBatchInserted(BatchInsertionResult result);
    }

    /// <summary>
    /// Error information for failed insertions.
    /// </summary>
    public sealed class InsertError
    {
        /// <summary>
        /// URL of the worker where the error occurred.
        /// </summary>
        public Uri WorkerUrl { get; init; } = null!;

        /// <summary>
        /// Number of records in the failed batch.
        /// </summary>
        public int RecordCount { get; init; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Exception that caused the error, if any.
        /// </summary>
        public Exception? Exception { get; init; }

        /// <summary>
        /// Timestamp when the error occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Retry attempt number when the error occurred.
        /// </summary>
        public int RetryAttempt { get; init; }
    }
