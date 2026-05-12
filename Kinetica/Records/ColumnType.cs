namespace kinetica.Records;

/// <summary>
/// Base column types that map to Avro primitive types.
    /// Equivalent to Rust's ColumnBaseType enum.
    /// </summary>
    public enum ColumnBaseType
    {
        /// <summary>32-bit signed integer</summary>
        Integer,
        /// <summary>64-bit signed integer</summary>
        Long,
        /// <summary>32-bit IEEE 754 float</summary>
        Float,
        /// <summary>64-bit IEEE 754 double</summary>
        Double,
        /// <summary>UTF-8 string</summary>
        String,
        /// <summary>Raw bytes</summary>
        Bytes
    }

    /// <summary>
    /// Column types including subtypes determined by properties.
    /// Equivalent to Rust's ColumnType enum.
    /// </summary>
    public enum ColumnType
    {
        // Base types
        /// <summary>32-bit signed integer</summary>
        Integer,
        /// <summary>64-bit signed integer</summary>
        Long,
        /// <summary>32-bit IEEE 754 float</summary>
        Float,
        /// <summary>64-bit IEEE 754 double</summary>
        Double,
        /// <summary>UTF-8 string</summary>
        String,
        /// <summary>Raw bytes</summary>
        Bytes,

        // Integer subtypes
        /// <summary>Boolean stored as integer (0 or 1)</summary>
        Boolean,
        /// <summary>8-bit signed integer</summary>
        Int8,
        /// <summary>16-bit signed integer</summary>
        Int16,

        // Long subtypes
        /// <summary>Timestamp as milliseconds since Unix epoch</summary>
        Timestamp,

        // String subtypes
        /// <summary>Date in YYYY-MM-DD format</summary>
        Date,
        /// <summary>DateTime in YYYY-MM-DD HH:MM:SS.mmm format</summary>
        DateTime,
        /// <summary>Time in HH:MM:SS.mmm format</summary>
        Time,
        /// <summary>Decimal number as string</summary>
        Decimal,
        /// <summary>IPv4 address in dotted-quad format</summary>
        Ipv4,
        /// <summary>UUID in standard format</summary>
        Uuid,
        /// <summary>Fixed-length char(1)</summary>
        Char1,
        /// <summary>Fixed-length char(2)</summary>
        Char2,
        /// <summary>Fixed-length char(4)</summary>
        Char4,
        /// <summary>Fixed-length char(8)</summary>
        Char8,
        /// <summary>Fixed-length char(16)</summary>
        Char16,
        /// <summary>Fixed-length char(32)</summary>
        Char32,
        /// <summary>Fixed-length char(64)</summary>
        Char64,
        /// <summary>Fixed-length char(128)</summary>
        Char128,
        /// <summary>Fixed-length char(256)</summary>
        Char256,

        // Bytes subtypes
        /// <summary>Well-Known Text geometry</summary>
        Wkt,

        // Special types
        /// <summary>Array type</summary>
        Array,
        /// <summary>Vector type</summary>
        Vector,
        /// <summary>JSON type</summary>
        Json
    }

    /// <summary>
    /// Extension methods for ColumnType.
    /// </summary>
    public static class ColumnTypeExtensions
    {
        /// <summary>
        /// Gets the base type for this column type.
        /// Equivalent to Rust's ColumnType::base_type() method.
        /// </summary>
        public static ColumnBaseType GetBaseType(this ColumnType columnType)
        {
            return columnType switch
            {
                ColumnType.Integer or ColumnType.Boolean or ColumnType.Int8 or ColumnType.Int16
                    => ColumnBaseType.Integer,

                ColumnType.Long or ColumnType.Timestamp
                    => ColumnBaseType.Long,

                ColumnType.Float
                    => ColumnBaseType.Float,

                ColumnType.Double
                    => ColumnBaseType.Double,

                ColumnType.String or ColumnType.Date or ColumnType.DateTime or ColumnType.Time or
                ColumnType.Decimal or ColumnType.Ipv4 or ColumnType.Uuid or
                ColumnType.Char1 or ColumnType.Char2 or ColumnType.Char4 or ColumnType.Char8 or
                ColumnType.Char16 or ColumnType.Char32 or ColumnType.Char64 or ColumnType.Char128 or
                ColumnType.Char256 or ColumnType.Array or ColumnType.Vector or ColumnType.Json
                    => ColumnBaseType.String,

                ColumnType.Bytes or ColumnType.Wkt
                    => ColumnBaseType.Bytes,

                _ => ColumnBaseType.String
            };
        }

        /// <summary>
        /// Gets the Avro type name for this base type.
        /// Equivalent to Rust's ColumnBaseType::avro_type() method.
        /// </summary>
        public static string GetAvroTypeName(this ColumnBaseType baseType)
        {
            return baseType switch
            {
                ColumnBaseType.Integer => "int",
                ColumnBaseType.Long => "long",
                ColumnBaseType.Float => "float",
                ColumnBaseType.Double => "double",
                ColumnBaseType.String => "string",
                ColumnBaseType.Bytes => "bytes",
                _ => "string"
            };
        }

        /// <summary>
        /// Gets the Avro type name for this column type.
        /// </summary>
        public static string GetAvroTypeName(this ColumnType columnType)
        {
            return columnType.GetBaseType().GetAvroTypeName();
        }

        /// <summary>
        /// Parse column type from Avro type and properties.
        /// Equivalent to Rust's ColumnType::from_avro_and_properties().
        /// </summary>
        public static ColumnType FromAvroAndProperties(string avroType, IEnumerable<string>? properties)
        {
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    var result = prop.ToLowerInvariant() switch
                    {
                        "boolean" => ColumnType.Boolean,
                        "int8" => ColumnType.Int8,
                        "int16" => ColumnType.Int16,
                        "timestamp" => ColumnType.Timestamp,
                        "date" => ColumnType.Date,
                        "datetime" => ColumnType.DateTime,
                        "time" => ColumnType.Time,
                        "decimal" => ColumnType.Decimal,
                        "ipv4" => ColumnType.Ipv4,
                        "uuid" => ColumnType.Uuid,
                        "char1" => ColumnType.Char1,
                        "char2" => ColumnType.Char2,
                        "char4" => ColumnType.Char4,
                        "char8" => ColumnType.Char8,
                        "char16" => ColumnType.Char16,
                        "char32" => ColumnType.Char32,
                        "char64" => ColumnType.Char64,
                        "char128" => ColumnType.Char128,
                        "char256" => ColumnType.Char256,
                        "wkt" => ColumnType.Wkt,
                        "array" => ColumnType.Array,
                        "vector" => ColumnType.Vector,
                        "json" => ColumnType.Json,
                        _ => (ColumnType?)null
                    };

                    if (result.HasValue)
                        return result.Value;
                }
            }

            // Fall back to base type
            return avroType.ToLowerInvariant() switch
            {
                "int" => ColumnType.Integer,
                "long" => ColumnType.Long,
                "float" => ColumnType.Float,
                "double" => ColumnType.Double,
                "string" => ColumnType.String,
                "bytes" => ColumnType.Bytes,
                _ => ColumnType.String
            };
        }

        /// <summary>
        /// Gets the Kinetica property name for this column type (if it has one).
        /// </summary>
        public static string? GetPropertyName(this ColumnType columnType)
        {
            return columnType switch
            {
                ColumnType.Boolean => "boolean",
                ColumnType.Int8 => "int8",
                ColumnType.Int16 => "int16",
                ColumnType.Timestamp => "timestamp",
                ColumnType.Date => "date",
                ColumnType.DateTime => "datetime",
                ColumnType.Time => "time",
                ColumnType.Decimal => "decimal",
                ColumnType.Ipv4 => "ipv4",
                ColumnType.Uuid => "uuid",
                ColumnType.Char1 => "char1",
                ColumnType.Char2 => "char2",
                ColumnType.Char4 => "char4",
                ColumnType.Char8 => "char8",
                ColumnType.Char16 => "char16",
                ColumnType.Char32 => "char32",
                ColumnType.Char64 => "char64",
                ColumnType.Char128 => "char128",
                ColumnType.Char256 => "char256",
                ColumnType.Wkt => "wkt",
                ColumnType.Array => "array",
                ColumnType.Vector => "vector",
                ColumnType.Json => "json",
                _ => null
            };
        }
    }
