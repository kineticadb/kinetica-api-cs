namespace kinetica.SchemaBuilder;

/// <summary>
/// Avro primitive types for schema building.
    /// Equivalent to Rust's AvroType enum.
    /// </summary>
    public enum AvroType
    {
        /// <summary>32-bit signed integer</summary>
        Int,
        /// <summary>64-bit signed integer</summary>
        Long,
        /// <summary>32-bit IEEE 754 float</summary>
        Float,
        /// <summary>64-bit IEEE 754 double</summary>
        Double,
        /// <summary>UTF-8 string</summary>
        String,
        /// <summary>Boolean value</summary>
        Boolean,
        /// <summary>Raw bytes</summary>
        Bytes
    }

    /// <summary>
    /// Extension methods for AvroType.
    /// </summary>
    public static class AvroTypeExtensions
    {
        /// <summary>
        /// Gets the Avro type name string.
        /// </summary>
        public static string GetTypeName(this AvroType type)
        {
            return type switch
            {
                AvroType.Int => "int",
                AvroType.Long => "long",
                AvroType.Float => "float",
                AvroType.Double => "double",
                AvroType.String => "string",
                AvroType.Boolean => "boolean",
                AvroType.Bytes => "bytes",
                _ => "string"
            };
        }
    }
