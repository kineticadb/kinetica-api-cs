namespace kinetica.Records;

/// <summary>
/// Immutable metadata about a column in a Kinetica type.
    /// Equivalent to Rust's Column struct.
    /// </summary>
    public sealed class Column
    {
        private readonly string _name;
        private readonly ColumnType _columnType;
        private readonly IReadOnlyList<string> _properties;
        private readonly bool _isNullable;
        private readonly bool _isPrimaryKey;
        private readonly bool _isShardKey;

        /// <summary>
        /// Creates a new column with the given name and type.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="columnType">The column type.</param>
        public Column(string name, ColumnType columnType)
            : this(name, columnType, Array.Empty<string>())
        {
        }

        /// <summary>
        /// Creates a new column with the given name, type, and properties.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="properties">The column properties (e.g., "primary_key", "shard_key", "nullable").</param>
        public Column(string name, ColumnType columnType, IEnumerable<string> properties)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _columnType = columnType;
            _properties = properties?.ToList().AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>();
            _isNullable = _properties.Any(p => string.Equals(p, "nullable", StringComparison.OrdinalIgnoreCase));
            _isPrimaryKey = _properties.Any(p => string.Equals(p, "primary_key", StringComparison.OrdinalIgnoreCase));
            _isShardKey = _properties.Any(p => string.Equals(p, "shard_key", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates a column with properties from a list.
        /// Equivalent to Rust's Column::with_properties().
        /// </summary>
        public static Column WithProperties(string name, ColumnType columnType, IEnumerable<string> properties)
            => new Column(name, columnType, properties);

        /// <summary>Gets the column name.</summary>
        public string Name => _name;

        /// <summary>Gets the column type.</summary>
        public ColumnType ColumnType => _columnType;

        /// <summary>Gets the base type for this column.</summary>
        public ColumnBaseType BaseType => _columnType.GetBaseType();

        /// <summary>Gets the column properties.</summary>
        public IReadOnlyList<string> Properties => _properties;

        /// <summary>Returns true if this column is nullable.</summary>
        public bool IsNullable => _isNullable;

        /// <summary>Returns true if this column is a primary key.</summary>
        public bool IsPrimaryKey => _isPrimaryKey;

        /// <summary>Returns true if this column is a shard key.</summary>
        public bool IsShardKey => _isShardKey;

        /// <summary>
        /// Returns the Avro type name for this column.
        /// </summary>
        public string AvroTypeName => _columnType.GetAvroTypeName();

        /// <summary>
        /// Creates a copy of this column with the specified properties added.
        /// </summary>
        public Column WithAddedProperties(params string[] additionalProperties)
        {
            var newProps = _properties.Concat(additionalProperties).Distinct().ToList();
            return new Column(_name, _columnType, newProps);
        }

        /// <summary>
        /// Creates a copy of this column marked as nullable.
        /// </summary>
        public Column AsNullable()
        {
            if (_isNullable) return this;
            return WithAddedProperties("nullable");
        }

        /// <summary>
        /// Creates a copy of this column marked as primary key.
        /// </summary>
        public Column AsPrimaryKey()
        {
            if (_isPrimaryKey) return this;
            return WithAddedProperties("primary_key");
        }

        /// <summary>
        /// Creates a copy of this column marked as shard key.
        /// </summary>
        public Column AsShardKey()
        {
            if (_isShardKey) return this;
            return WithAddedProperties("shard_key");
        }

        public override string ToString()
        {
            var props = _properties.Count > 0 ? $" [{string.Join(", ", _properties)}]" : "";
            return $"{_name}: {_columnType}{props}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Column other) return false;
            return _name == other._name &&
                   _columnType == other._columnType &&
                   _properties.SequenceEqual(other._properties);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_name, _columnType);
        }
    }
