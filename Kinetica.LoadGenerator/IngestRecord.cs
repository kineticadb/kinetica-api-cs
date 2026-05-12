using System;
using kinetica.Records;

namespace Kinetica.LoadGenerator
{
    /// <summary>
    /// Basic ingest record schema matching the Rust IngestRecord.
    /// Used for simple load generation with timestamp, source, and payload fields.
    /// </summary>
    public class IngestRecord : IShardKeyExtractor
    {
        /// <summary>
        /// Timestamp in milliseconds since Unix epoch.
        /// Property: TIMESTAMP
        /// </summary>
        public long TimestampMillis { get; set; }

        /// <summary>
        /// Source identifier (set to message_size for tracking payload size).
        /// Property: DATA
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// Variable-size payload for measuring throughput with different record sizes.
        /// Uses string type which is well-supported by Kinetica type system.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// Gets the column properties for type creation.
        /// </summary>
        public static Dictionary<string, IList<string>> GetTypeProperties() => new()
        {
            { "TimestampMillis", new List<string> { kinetica.ColumnProperty.TIMESTAMP } },
            { "Source", new List<string> { kinetica.ColumnProperty.DATA } },
            { "Payload", new List<string> { kinetica.ColumnProperty.DATA } }
        };

        public override string ToString() =>
            $"IngestRecord {{ TimestampMillis={TimestampMillis}, Source={Source}, PayloadSize={Payload.Length} }}";

        public ShardKeyValues GetShardKeyValues()
        {
            return new ShardKeyValues();
        }
    }

    /// <summary>
    /// Extended ingest record schema matching the Rust IngestRecord2.
    /// Adds thread tracking and high-precision timing for detailed analysis.
    /// </summary>
    public class IngestRecord2 : IShardKeyExtractor
    {
        /// <summary>
        /// Timestamp in milliseconds since Unix epoch.
        /// Property: TIMESTAMP
        /// </summary>
        public long TimestampMillis { get; set; }

        /// <summary>
        /// Source identifier (set to message_size for tracking payload size).
        /// Property: DATA
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// Thread producer ID for multi-threaded load generation analysis.
        /// Property: DATA
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Thread-local record counter for ordering analysis.
        /// Property: DATA
        /// </summary>
        public long LocalCount { get; set; }

        /// <summary>
        /// High-precision nanosecond timestamp for latency measurements.
        /// Property: DATA
        /// </summary>
        public long Nanosecond { get; set; }

        /// <summary>
        /// Variable-size payload for measuring throughput with different record sizes.
        /// Uses string type which is well-supported by Kinetica type system.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// Gets the column properties for type creation.
        /// </summary>
        public static Dictionary<string, IList<string>> GetTypeProperties() => new()
        {
            { "TimestampMillis", new List<string> { kinetica.ColumnProperty.TIMESTAMP } },
            { "Source", new List<string> { kinetica.ColumnProperty.DATA } },
            { "ThreadId", new List<string> { kinetica.ColumnProperty.DATA } },
            { "LocalCount", new List<string> { kinetica.ColumnProperty.DATA } },
            { "Nanosecond", new List<string> { kinetica.ColumnProperty.DATA } },
            { "Payload", new List<string> { kinetica.ColumnProperty.DATA } }
        };

        public override string ToString() =>
            $"IngestRecord2 {{ TimestampMillis={TimestampMillis}, Source={Source}, ThreadId={ThreadId}, LocalCount={LocalCount}, Nanosecond={Nanosecond}, PayloadSize={Payload.Length} }}";

        public ShardKeyValues GetShardKeyValues()
        {
            return new ShardKeyValues();
        }
    }
}
