using System.Text;
using System.Text.Json;

namespace kinetica.Records;

/// <summary>
/// Immutable collection of metadata about a Kinetica type.
/// Equivalent to Rust's Type struct.
///
/// This class contains all the information needed to create and encode records
/// of this type, including schema information, column metadata, and key indices.
///
/// <example>
/// <code>
/// // From existing table
/// var recordType = await Type.FromTableAsync(kinetica, "my_table");
///
/// // Or create programmatically
/// var recordType = Type.Builder("my_record")
///     .AddIntColumn("id").PrimaryKey().ShardKey()
///     .AddStringColumn("name")
///     .AddDoubleColumn("score").Nullable()
///     .Build();
///
/// // Create records
/// var record = recordType.NewInstance();
/// record.Put(0, 42);
/// record.Put("name", "Alice");
/// </code>
/// </example>
/// </summary>
public sealed class Type
{
    private readonly string _label;
    private readonly IReadOnlyList<Column> _columns;
    private readonly IReadOnlyDictionary<string, int> _columnIndices;
    private readonly IReadOnlyList<int> _shardKeyIndices;
    private readonly IReadOnlyList<int> _primaryKeyIndices;
    private readonly string _schemaString;
    private readonly string? _typeId;

    /// <summary>
    /// Creates a Type from column definitions.
    /// </summary>
    /// <param name="label">The type label/name.</param>
    /// <param name="columns">The column definitions.</param>
    /// <exception cref="ArgumentNullException">If label or columns is null.</exception>
    /// <exception cref="ArgumentException">If columns is empty or contains duplicates.</exception>
    public Type(string label, IEnumerable<Column> columns)
    {
        _label = label ?? throw new ArgumentNullException(nameof(label));
        var columnList = columns?.ToList() ?? throw new ArgumentNullException(nameof(columns));

        if (columnList.Count == 0)
            throw new ArgumentException("At least one column is required", nameof(columns));

        _columns = columnList.AsReadOnly();

        // Build column index map
        var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var shardKeyIndices = new List<int>();
        var primaryKeyIndices = new List<int>();

        for (int i = 0; i < columnList.Count; i++)
        {
            var col = columnList[i];
            if (columnIndices.ContainsKey(col.Name))
                throw new ArgumentException($"Duplicate column name: {col.Name}", nameof(columns));

            columnIndices[col.Name] = i;

            if (col.IsShardKey)
                shardKeyIndices.Add(i);

            if (col.IsPrimaryKey)
                primaryKeyIndices.Add(i);
        }

        _columnIndices = columnIndices;
        _shardKeyIndices = shardKeyIndices.AsReadOnly();
        _primaryKeyIndices = primaryKeyIndices.AsReadOnly();

        // Build Avro schema
        _schemaString = BuildAvroSchema(label, columnList);
        _typeId = null;
    }

    // Private constructor for FromTableResponse
    private Type(
        string label,
        IReadOnlyList<Column> columns,
        IReadOnlyDictionary<string, int> columnIndices,
        IReadOnlyList<int> shardKeyIndices,
        IReadOnlyList<int> primaryKeyIndices,
        string schemaString,
        string? typeId)
    {
        _label = label;
        _columns = columns;
        _columnIndices = columnIndices;
        _shardKeyIndices = shardKeyIndices;
        _primaryKeyIndices = primaryKeyIndices;
        _schemaString = schemaString;
        _typeId = typeId;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a TypeBuilder for fluent construction.
    /// Equivalent to Rust's Type::builder().
    /// </summary>
    /// <param name="label">The type label/name.</param>
    /// <returns>A new TypeBuilder instance.</returns>
    public static TypeBuilder Builder(string label) => new TypeBuilder(label);

    /// <summary>
    /// Fetch type from an existing Kinetica table.
    /// Equivalent to Rust's Type::from_table().
    /// </summary>
    /// <param name="kinetica">The Kinetica connection.</param>
    /// <param name="tableName">The table name.</param>
    /// <returns>A Type representing the table schema.</returns>
    public static async Task<Type> FromTableAsync(kinetica.Kinetica kinetica, string tableName)
    {
        if (kinetica == null)
            throw new ArgumentNullException(nameof(kinetica));
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentException("Table name is required", nameof(tableName));

        var response = await Task.Run(() => kinetica.showTable(tableName, new Dictionary<string, string>
        {
            ["get_sizes"] = "false"
        }));

        return FromShowTableResponse(tableName, response);
    }

    /// <summary>
    /// Fetch type from an existing Kinetica table (synchronous version).
    /// </summary>
    /// <param name="kinetica">The Kinetica connection.</param>
    /// <param name="tableName">The table name.</param>
    /// <returns>A Type representing the table schema.</returns>
    public static Type FromTable(kinetica.Kinetica kinetica, string tableName)
    {
        if (kinetica == null)
            throw new ArgumentNullException(nameof(kinetica));
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentException("Table name is required", nameof(tableName));

        var response = kinetica.showTable(tableName, new Dictionary<string, string>
        {
            ["get_sizes"] = "false"
        });

        return FromShowTableResponse(tableName, response);
    }

    /// <summary>
    /// Build Type from ShowTableResponse.
    /// Equivalent to Rust's Type::from_table_response().
    /// </summary>
    public static Type FromShowTableResponse(string tableName, kinetica.ShowTableResponse response)
    {
        if (response.type_schemas == null || response.type_schemas.Count == 0)
            throw new InvalidOperationException($"No type schema for table '{tableName}'");

        var schemaStr = response.type_schemas[0];

        // Parse the Avro schema JSON
        using var doc = JsonDocument.Parse(schemaStr);
        var root = doc.RootElement;

        var label = root.TryGetProperty("name", out var nameProp)
            ? nameProp.GetString() ?? tableName
            : tableName;

        if (!root.TryGetProperty("fields", out var fieldsProp) || fieldsProp.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Schema missing fields array");

        // Get properties
        if (response.properties == null || response.properties.Count == 0)
            throw new InvalidOperationException("No properties in response");

        var properties = response.properties[0];

        // Build columns
        var columns = new List<Column>();
        var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var shardKeyIndices = new List<int>();
        var primaryKeyIndices = new List<int>();

        int idx = 0;
        foreach (var field in fieldsProp.EnumerateArray())
        {
            if (!field.TryGetProperty("name", out var fieldNameProp))
                throw new InvalidOperationException("Field missing name");

            var fieldName = fieldNameProp.GetString()
                ?? throw new InvalidOperationException("Field name is null");

            var avroType = ExtractAvroType(field);
            var colProps = properties.TryGetValue(fieldName, out var propList)
                ? propList
                : new List<string>();

            var columnType = ColumnTypeExtensions.FromAvroAndProperties(avroType, colProps);
            var column = Column.WithProperties(fieldName, columnType, colProps);

            columns.Add(column);
            columnIndices[fieldName] = idx;

            if (column.IsShardKey)
                shardKeyIndices.Add(idx);
            if (column.IsPrimaryKey)
                primaryKeyIndices.Add(idx);

            idx++;
        }

        // Get type ID if available
        string? typeId = null;
        if (response.type_ids != null && response.type_ids.Count > 0)
            typeId = response.type_ids[0];

        return new Type(
            label,
            columns.AsReadOnly(),
            columnIndices,
            shardKeyIndices.AsReadOnly(),
            primaryKeyIndices.AsReadOnly(),
            schemaStr, // Use original schema string from Kinetica
            typeId);
    }

    private static string ExtractAvroType(JsonElement field)
    {
        if (!field.TryGetProperty("type", out var typeProp))
            return "string";

        return typeProp.ValueKind switch
        {
            JsonValueKind.String => typeProp.GetString() ?? "string",
            JsonValueKind.Array => ExtractTypeFromUnion(typeProp),
            JsonValueKind.Object => typeProp.TryGetProperty("type", out var innerType)
                ? innerType.GetString() ?? "string"
                : "string",
            _ => "string"
        };
    }

    private static string ExtractTypeFromUnion(JsonElement unionArray)
    {
        // Union type like ["null", "string"] - return the non-null type
        foreach (var element in unionArray.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                var typeName = element.GetString();
                if (typeName != null && typeName != "null")
                    return typeName;
            }
        }
        return "string";
    }

    #endregion

    #region Properties

    /// <summary>Gets the type label/name.</summary>
    public string Label => _label;

    /// <summary>Gets the Avro schema string.</summary>
    public string SchemaString => _schemaString;

    /// <summary>Gets the type ID (if fetched from Kinetica).</summary>
    public string? TypeId => _typeId;

    /// <summary>Gets the number of columns.</summary>
    public int ColumnCount => _columns.Count;

    /// <summary>Gets the shard key column indices.</summary>
    public IReadOnlyList<int> ShardKeyIndices => _shardKeyIndices;

    /// <summary>Gets the primary key column indices.</summary>
    public IReadOnlyList<int> PrimaryKeyIndices => _primaryKeyIndices;

    /// <summary>Returns true if this type has a shard key.</summary>
    public bool HasShardKey => _shardKeyIndices.Count > 0;

    /// <summary>Returns true if this type has a primary key.</summary>
    public bool HasPrimaryKey => _primaryKeyIndices.Count > 0;

    /// <summary>Gets all columns.</summary>
    public IReadOnlyList<Column> Columns => _columns;

    #endregion

    #region Column Access

    /// <summary>
    /// Gets a column by index.
    /// </summary>
    /// <param name="index">The column index.</param>
    /// <returns>The column, or null if index is out of range.</returns>
    public Column? GetColumn(int index)
    {
        if (index < 0 || index >= _columns.Count)
            return null;
        return _columns[index];
    }

    /// <summary>
    /// Gets a column by name.
    /// </summary>
    /// <param name="name">The column name (case-insensitive).</param>
    /// <returns>The column, or null if not found.</returns>
    public Column? GetColumnByName(string name)
    {
        if (_columnIndices.TryGetValue(name, out var index))
            return _columns[index];
        return null;
    }

    /// <summary>
    /// Gets a column index by name.
    /// </summary>
    /// <param name="name">The column name (case-insensitive).</param>
    /// <returns>The column index, or null if not found.</returns>
    public int? GetColumnIndex(string name)
    {
        if (_columnIndices.TryGetValue(name, out var index))
            return index;
        return null;
    }

    /// <summary>
    /// Gets a column index by name, throwing if not found.
    /// </summary>
    /// <param name="name">The column name (case-insensitive).</param>
    /// <returns>The column index.</returns>
    /// <exception cref="KeyNotFoundException">If the column is not found.</exception>
    public int GetColumnIndexOrThrow(string name)
    {
        if (_columnIndices.TryGetValue(name, out var index))
            return index;
        throw new KeyNotFoundException($"Column not found: {name}");
    }

    #endregion

    #region Record Creation

    /// <summary>
    /// Creates a new GenericRecord instance of this type.
    /// Equivalent to Rust's Type::new_instance().
    /// </summary>
    /// <returns>A new GenericRecord with all values initialized to null.</returns>
    public GenericRecord NewInstance() => new GenericRecord(this);

    #endregion

    #region Schema Building

    private static string BuildAvroSchema(string name, IReadOnlyList<Column> columns)
    {
        var sb = new StringBuilder();
        sb.Append("{\"type\":\"record\",\"name\":\"");
        sb.Append(EscapeJsonString(name));
        sb.Append("\",\"fields\":[");

        for (int i = 0; i < columns.Count; i++)
        {
            if (i > 0) sb.Append(',');

            var col = columns[i];
            var avroType = col.AvroTypeName;

            sb.Append("{\"name\":\"");
            sb.Append(EscapeJsonString(col.Name));
            sb.Append("\",\"type\":");

            if (col.IsNullable)
            {
                sb.Append("[\"null\",\"");
                sb.Append(avroType);
                sb.Append("\"]");
            }
            else
            {
                sb.Append('"');
                sb.Append(avroType);
                sb.Append('"');
            }

            sb.Append('}');
        }

        sb.Append("]}");
        return sb.ToString();
    }

    private static string EscapeJsonString(string s)
    {
        // Simple JSON escaping for common cases
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

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Type({_label}, {_columns.Count} columns)";
    }
}
