using System.Text;

namespace kinetica.SchemaBuilder;

/// <summary>
/// Represents a field definition in an Avro schema.
/// </summary>
internal readonly struct FieldDef
{
    public string Name { get; }
    public AvroType Type { get; }
    public bool IsNullable { get; }
    public string? Doc { get; }

    public FieldDef(string name, AvroType type, bool isNullable, string? doc = null)
    {
        Name = name;
        Type = type;
        IsNullable = isNullable;
        Doc = doc;
    }
}

/// <summary>
/// Builder for constructing Avro schema JSON strings.
/// Equivalent to Rust's AvroSchemaBuilder.
///
/// <example>
/// <code>
/// var schema = new AvroSchemaBuilder("SensorReading")
///     .Namespace("com.example")
///     .IntField("id")
///     .StringField("sensor_name")
///     .NullableDoubleField("value")
///     .LongField("timestamp")
///     .Build();
/// </code>
/// </example>
/// </summary>
public sealed class AvroSchemaBuilder
{
    private readonly string _name;
    private string? _namespace;
    private string? _doc;
    private readonly List<FieldDef> _fields;

    /// <summary>
    /// Creates a new AvroSchemaBuilder with the specified record name.
    /// </summary>
    /// <param name="name">The record type name.</param>
    public AvroSchemaBuilder(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _fields = new List<FieldDef>();
    }

    /// <summary>
    /// Sets the namespace for the schema.
    /// </summary>
    public AvroSchemaBuilder Namespace(string ns)
    {
        _namespace = ns;
        return this;
    }

    /// <summary>
    /// Sets the documentation for the schema.
    /// </summary>
    public AvroSchemaBuilder Doc(string doc)
    {
        _doc = doc;
        return this;
    }

    #region Generic Field Methods

    /// <summary>
    /// Adds a field with the specified name and type.
    /// </summary>
    public AvroSchemaBuilder Field(string name, AvroType type)
    {
        _fields.Add(new FieldDef(name, type, false));
        return this;
    }

    /// <summary>
    /// Adds a field with documentation.
    /// </summary>
    public AvroSchemaBuilder FieldWithDoc(string name, AvroType type, string doc)
    {
        _fields.Add(new FieldDef(name, type, false, doc));
        return this;
    }

    /// <summary>
    /// Adds a nullable field with the specified name and type.
    /// </summary>
    public AvroSchemaBuilder NullableField(string name, AvroType type)
    {
        _fields.Add(new FieldDef(name, type, true));
        return this;
    }

    #endregion

    #region Convenience Type Methods

    /// <summary>Adds an integer field.</summary>
    public AvroSchemaBuilder IntField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Int, false));
        return this;
    }

    /// <summary>Adds a long field.</summary>
    public AvroSchemaBuilder LongField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Long, false));
        return this;
    }

    /// <summary>Adds a float field.</summary>
    public AvroSchemaBuilder FloatField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Float, false));
        return this;
    }

    /// <summary>Adds a double field.</summary>
    public AvroSchemaBuilder DoubleField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Double, false));
        return this;
    }

    /// <summary>Adds a string field.</summary>
    public AvroSchemaBuilder StringField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.String, false));
        return this;
    }

    /// <summary>Adds a boolean field.</summary>
    public AvroSchemaBuilder BoolField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Boolean, false));
        return this;
    }

    /// <summary>Adds a bytes field.</summary>
    public AvroSchemaBuilder BytesField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Bytes, false));
        return this;
    }

    /// <summary>Adds a timestamp field (long).</summary>
    public AvroSchemaBuilder TimestampField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Long, false));
        return this;
    }

    #endregion

    #region Nullable Convenience Methods

    /// <summary>Adds a nullable integer field.</summary>
    public AvroSchemaBuilder NullableIntField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Int, true));
        return this;
    }

    /// <summary>Adds a nullable long field.</summary>
    public AvroSchemaBuilder NullableLongField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Long, true));
        return this;
    }

    /// <summary>Adds a nullable float field.</summary>
    public AvroSchemaBuilder NullableFloatField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Float, true));
        return this;
    }

    /// <summary>Adds a nullable double field.</summary>
    public AvroSchemaBuilder NullableDoubleField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Double, true));
        return this;
    }

    /// <summary>Adds a nullable string field.</summary>
    public AvroSchemaBuilder NullableStringField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.String, true));
        return this;
    }

    /// <summary>Adds a nullable boolean field.</summary>
    public AvroSchemaBuilder NullableBoolField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Boolean, true));
        return this;
    }

    /// <summary>Adds a nullable bytes field.</summary>
    public AvroSchemaBuilder NullableBytesField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Bytes, true));
        return this;
    }

    /// <summary>Adds a nullable timestamp field.</summary>
    public AvroSchemaBuilder NullableTimestampField(string name)
    {
        _fields.Add(new FieldDef(name, AvroType.Long, true));
        return this;
    }

    #endregion

    #region Build Methods

    /// <summary>
    /// Builds the Avro schema JSON string.
    /// </summary>
    public string Build()
    {
        var sb = new StringBuilder();
        sb.Append('{');
        sb.Append("\"type\":\"record\"");
        sb.Append(",\"name\":\"");
        sb.Append(EscapeJsonString(_name));
        sb.Append('"');

        if (!string.IsNullOrEmpty(_namespace))
        {
            sb.Append(",\"namespace\":\"");
            sb.Append(EscapeJsonString(_namespace!));
            sb.Append('"');
        }

        if (!string.IsNullOrEmpty(_doc))
        {
            sb.Append(",\"doc\":\"");
            sb.Append(EscapeJsonString(_doc!));
            sb.Append('"');
        }

        sb.Append(",\"fields\":[");

        for (int i = 0; i < _fields.Count; i++)
        {
            if (i > 0) sb.Append(',');

            var field = _fields[i];
            sb.Append("{\"name\":\"");
            sb.Append(EscapeJsonString(field.Name));
            sb.Append("\",\"type\":");

            var typeName = field.Type.GetTypeName();
            if (field.IsNullable)
            {
                sb.Append("[\"null\",\"");
                sb.Append(typeName);
                sb.Append("\"]");
            }
            else
            {
                sb.Append('"');
                sb.Append(typeName);
                sb.Append('"');
            }

            if (!string.IsNullOrEmpty(field.Doc))
            {
                sb.Append(",\"doc\":\"");
                sb.Append(EscapeJsonString(field.Doc!));
                sb.Append('"');
            }

            sb.Append('}');
        }

        sb.Append("]}");
        return sb.ToString();
    }

    private static string EscapeJsonString(string s)
    {
        if (s.IndexOfAny(new[] { '"', '\\', '\n', '\r', '\t' }) < 0)
            return s;

        var sb = new StringBuilder(s.Length + 10);
        foreach (var c in s)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    #endregion
}
