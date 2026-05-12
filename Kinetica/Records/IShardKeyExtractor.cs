namespace kinetica.Records;

/// <summary>
/// Interface for extracting shard key values from a record.
    /// Equivalent to Rust's ShardKeyExtractor trait.
    ///
    /// Implementing this interface enables shard-aware routing in the BulkInserter,
    /// allowing records to be sent directly to the correct worker node based on
    /// their shard key values.
    ///
    /// <example>
    /// <code>
    /// public class SensorReading : IShardKeyExtractor
    /// {
    ///     public int Id { get; set; }
    ///     public string Region { get; set; }
    ///     public double Value { get; set; }
    ///
    ///     public ShardKeyValues GetShardKeyValues()
    ///     {
    ///         // Single shard key column
    ///         return ShardKeyValues.Single("id", ShardKeyValue.Int(Id));
    ///
    ///         // Or for composite shard keys:
    ///         // return new ShardKeyValues(
    ///         //     ("id", ShardKeyValue.Int(Id)),
    ///         //     ("region", ShardKeyValue.String(Region))
    ///         // );
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface IShardKeyExtractor
    {
        /// <summary>
        /// Returns shard key column names and their typed values.
        ///
        /// Only columns that are part of the shard key need to be included.
        /// The values are used for computing the routing hash to determine
        /// which worker node should receive the record.
        ///
        /// For best performance, implement this method to avoid allocations
        /// when possible (e.g., use ShardKeyValues.Single() for single-column keys).
        /// </summary>
        /// <returns>A collection of (column name, value) pairs for the shard key.</returns>
        ShardKeyValues GetShardKeyValues();
    }

    /// <summary>
    /// Combined interface for records that support bulk insertion with shard-aware routing.
    /// Equivalent to Rust's T: KineticaRecord + ShardKeyExtractor constraint.
    ///
    /// Types implementing this interface can be used with BulkInserter for
    /// optimal multi-head ingest performance.
    /// </summary>
    public interface IBulkInsertable : IKineticaRecord, IShardKeyExtractor
    {
    }
