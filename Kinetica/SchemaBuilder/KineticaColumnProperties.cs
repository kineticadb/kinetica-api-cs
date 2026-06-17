namespace kinetica.SchemaBuilder;

/// <summary>
/// Builder for Kinetica column properties.
/// Equivalent to Rust's KineticaColumnProperties.
///
/// <example>
/// <code>
/// var props = new KineticaColumnProperties()
///     .PrimaryKey()
///     .ShardKey()
///     .Build();
///
/// // Or for specific column types:
/// var timestampProps = new KineticaColumnProperties()
///     .Timestamp()
///     .Nullable()
///     .Build();
/// </code>
/// </example>
/// </summary>
public sealed class KineticaColumnProperties
{
    private readonly List<string> _properties;

    /// <summary>
    /// Creates a new KineticaColumnProperties builder.
    /// </summary>
    public KineticaColumnProperties()
    {
        _properties = new List<string>();
    }

    /// <summary>
    /// Marks the column as a primary key.
    /// </summary>
    public KineticaColumnProperties PrimaryKey()
    {
        AddIfNotExists("primary_key");
        return this;
    }

    /// <summary>
    /// Marks the column as a shard key.
    /// </summary>
    public KineticaColumnProperties ShardKey()
    {
        AddIfNotExists("shard_key");
        return this;
    }

    /// <summary>
    /// Marks the column as nullable.
    /// </summary>
    public KineticaColumnProperties Nullable()
    {
        AddIfNotExists("nullable");
        return this;
    }

    /// <summary>
    /// Marks the column as a timestamp type.
    /// </summary>
    public KineticaColumnProperties Timestamp()
    {
        AddIfNotExists("timestamp");
        return this;
    }

    /// <summary>
    /// Marks the column as a data column (text search enabled).
    /// </summary>
    public KineticaColumnProperties Data()
    {
        AddIfNotExists("data");
        return this;
    }

    /// <summary>
    /// Marks the column as an IPv4 type.
    /// </summary>
    public KineticaColumnProperties Ipv4()
    {
        AddIfNotExists("ipv4");
        return this;
    }

    /// <summary>
    /// Marks the column as a WKT (Well-Known Text) geometry type.
    /// </summary>
    public KineticaColumnProperties Wkt()
    {
        AddIfNotExists("wkt");
        return this;
    }

    /// <summary>
    /// Marks the column as a char(N) type.
    /// </summary>
    /// <param name="n">The character length (1, 2, 4, 8, 16, 32, 64, 128, or 256).</param>
    public KineticaColumnProperties CharN(int n)
    {
        var prop = n switch
        {
            1 => "char1",
            2 => "char2",
            4 => "char4",
            8 => "char8",
            16 => "char16",
            32 => "char32",
            64 => "char64",
            128 => "char128",
            256 => "char256",
            _ => throw new ArgumentException($"Invalid char length: {n}. Must be 1, 2, 4, 8, 16, 32, 64, 128, or 256.", nameof(n))
        };
        AddIfNotExists(prop);
        return this;
    }

    /// <summary>
    /// Marks the column as an int8 type.
    /// </summary>
    public KineticaColumnProperties Int8()
    {
        AddIfNotExists("int8");
        return this;
    }

    /// <summary>
    /// Marks the column as an int16 type.
    /// </summary>
    public KineticaColumnProperties Int16()
    {
        AddIfNotExists("int16");
        return this;
    }

    /// <summary>
    /// Marks the column as a date type.
    /// </summary>
    public KineticaColumnProperties Date()
    {
        AddIfNotExists("date");
        return this;
    }

    /// <summary>
    /// Marks the column as a time type.
    /// </summary>
    public KineticaColumnProperties Time()
    {
        AddIfNotExists("time");
        return this;
    }

    /// <summary>
    /// Marks the column as a datetime type.
    /// </summary>
    public KineticaColumnProperties DateTime()
    {
        AddIfNotExists("datetime");
        return this;
    }

    /// <summary>
    /// Marks the column as a decimal type.
    /// </summary>
    public KineticaColumnProperties Decimal()
    {
        AddIfNotExists("decimal");
        return this;
    }

    /// <summary>
    /// Marks the column as a UUID type.
    /// </summary>
    public KineticaColumnProperties Uuid()
    {
        AddIfNotExists("uuid");
        return this;
    }

    /// <summary>
    /// Marks the column as an unsigned long type.
    /// </summary>
    public KineticaColumnProperties Ulong()
    {
        AddIfNotExists("ulong");
        return this;
    }

    /// <summary>
    /// Marks the column as a boolean type.
    /// </summary>
    public KineticaColumnProperties Boolean()
    {
        AddIfNotExists("boolean");
        return this;
    }

    /// <summary>
    /// Marks the column as a JSON type.
    /// </summary>
    public KineticaColumnProperties Json()
    {
        AddIfNotExists("json");
        return this;
    }

    /// <summary>
    /// Marks the column as an array type.
    /// </summary>
    public KineticaColumnProperties Array()
    {
        AddIfNotExists("array");
        return this;
    }

    /// <summary>
    /// Marks the column as a vector type.
    /// </summary>
    public KineticaColumnProperties Vector()
    {
        AddIfNotExists("vector");
        return this;
    }

    /// <summary>
    /// Adds a custom property.
    /// </summary>
    public KineticaColumnProperties Property(string property)
    {
        if (!string.IsNullOrEmpty(property))
            AddIfNotExists(property);
        return this;
    }

    /// <summary>
    /// Builds the list of properties.
    /// </summary>
    public IList<string> Build()
    {
        return new List<string>(_properties);
    }

    private void AddIfNotExists(string property)
    {
        if (!_properties.Contains(property))
            _properties.Add(property);
    }
}
