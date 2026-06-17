namespace kinetica.Records;

/// <summary>
/// Interface for records that can be inserted into Kinetica.
/// Equivalent to Rust's KineticaRecord trait.
///
/// Types implementing this interface can be used with the BulkInserter
/// for high-performance data ingestion.
///
/// <example>
/// <code>
/// public class SensorReading : IKineticaRecord
/// {
///     public int Id { get; set; }
///     public string SensorName { get; set; }
///     public double Value { get; set; }
///
///     private static readonly string _schema = new AvroSchemaBuilder("SensorReading")
///         .IntField("id")
///         .StringField("sensor_name")
///         .DoubleField("value")
///         .Build();
///
///     public string GetAvroSchema() => _schema;
///
///     public static IReadOnlyDictionary&lt;string, IList&lt;string&gt;&gt; GetTypeProperties()
///         => new TypePropertiesBuilder()
///             .PrimaryKey("id")
///             .Build();
/// }
/// </code>
/// </example>
/// </summary>
public interface IKineticaRecord
{
    /// <summary>
    /// Returns the Avro schema as a JSON string for this record type.
    ///
    /// The schema should be cached (e.g., in a static field) to avoid
    /// repeated string allocations.
    /// </summary>
    /// <returns>A JSON string containing the Avro record schema.</returns>
    string GetAvroSchema();
}

/// <summary>
/// Static interface for type-level properties.
/// Implement this alongside IKineticaRecord to provide type metadata.
///
/// Note: C# doesn't support static interface members in all versions.
/// This interface provides a pattern for implementing static GetTypeProperties().
/// </summary>
/// <typeparam name="T">The implementing type.</typeparam>
public interface IKineticaRecordType<T> where T : IKineticaRecord
{
    /// <summary>
    /// Returns type properties including field types and attributes.
    ///
    /// Properties include:
    /// - "primary_key": Column is part of the primary key
    /// - "shard_key": Column is part of the shard key
    /// - "nullable": Column can contain null values
    /// - "timestamp": Column contains timestamp data
    /// - "ipv4": Column contains IPv4 addresses
    /// - etc.
    /// </summary>
    /// <returns>
    /// A dictionary mapping column names to their list of properties.
    /// </returns>
    static abstract IReadOnlyDictionary<string, IList<string>> GetTypeProperties();
}
