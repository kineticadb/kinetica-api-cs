using System;
using System.Collections.Generic;

namespace kinetica.Records;

/// <summary>
/// Builder for constructing a Type with a fluent API.
    /// Equivalent to Rust's TypeBuilder.
    ///
    /// <example>
    /// <code>
    /// var recordType = Type.Builder("sensor_reading")
    ///     .AddIntColumn("id").PrimaryKey().ShardKey()
    ///     .AddStringColumn("sensor_name")
    ///     .AddDoubleColumn("value")
    ///     .AddLongColumn("timestamp").Nullable()
    ///     .Build();
    /// </code>
    /// </example>
    /// </summary>
    public sealed class TypeBuilder
    {
        private readonly string _label;
        private readonly List<Column> _columns;
        private (string Name, ColumnType Type, List<string> Properties)? _currentColumn;

        /// <summary>
        /// Creates a new TypeBuilder with the specified label.
        /// </summary>
        /// <param name="label">The type label/name.</param>
        public TypeBuilder(string label)
        {
            _label = label ?? throw new ArgumentNullException(nameof(label));
            _columns = new List<Column>();
            _currentColumn = null;
        }

        private void FinishCurrentColumn()
        {
            if (_currentColumn.HasValue)
            {
                var (name, type, props) = _currentColumn.Value;
                _columns.Add(Column.WithProperties(name, type, props));
                _currentColumn = null;
            }
        }

        #region Add Column Methods

        /// <summary>
        /// Adds a column with the specified type.
        /// </summary>
        public TypeBuilder AddColumn(Column column)
        {
            FinishCurrentColumn();
            _columns.Add(column);
            return this;
        }

        /// <summary>
        /// Adds an integer column.
        /// </summary>
        public TypeBuilder AddIntColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Integer, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a long column.
        /// </summary>
        public TypeBuilder AddLongColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Long, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a float column.
        /// </summary>
        public TypeBuilder AddFloatColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Float, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a double column.
        /// </summary>
        public TypeBuilder AddDoubleColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Double, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a string column.
        /// </summary>
        public TypeBuilder AddStringColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.String, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a bytes column.
        /// </summary>
        public TypeBuilder AddBytesColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Bytes, new List<string>());
            return this;
        }

        /// <summary>
        /// Adds a timestamp column.
        /// </summary>
        public TypeBuilder AddTimestampColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Timestamp, new List<string> { "timestamp" });
            return this;
        }

        /// <summary>
        /// Adds a date column.
        /// </summary>
        public TypeBuilder AddDateColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Date, new List<string> { "date" });
            return this;
        }

        /// <summary>
        /// Adds a datetime column.
        /// </summary>
        public TypeBuilder AddDateTimeColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.DateTime, new List<string> { "datetime" });
            return this;
        }

        /// <summary>
        /// Adds a time column.
        /// </summary>
        public TypeBuilder AddTimeColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Time, new List<string> { "time" });
            return this;
        }

        /// <summary>
        /// Adds a boolean column (stored as int).
        /// </summary>
        public TypeBuilder AddBooleanColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Boolean, new List<string> { "boolean" });
            return this;
        }

        /// <summary>
        /// Adds an int8 column.
        /// </summary>
        public TypeBuilder AddInt8Column(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Int8, new List<string> { "int8" });
            return this;
        }

        /// <summary>
        /// Adds an int16 column.
        /// </summary>
        public TypeBuilder AddInt16Column(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Int16, new List<string> { "int16" });
            return this;
        }

        /// <summary>
        /// Adds a decimal column.
        /// </summary>
        public TypeBuilder AddDecimalColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Decimal, new List<string> { "decimal" });
            return this;
        }

        /// <summary>
        /// Adds an IPv4 column.
        /// </summary>
        public TypeBuilder AddIpv4Column(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Ipv4, new List<string> { "ipv4" });
            return this;
        }

        /// <summary>
        /// Adds a UUID column.
        /// </summary>
        public TypeBuilder AddUuidColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Uuid, new List<string> { "uuid" });
            return this;
        }

        /// <summary>
        /// Adds a char(N) column.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="length">The character length (1, 2, 4, 8, 16, 32, 64, 128, or 256).</param>
        public TypeBuilder AddCharColumn(string name, int length)
        {
            FinishCurrentColumn();
            var (type, prop) = length switch
            {
                1 => (ColumnType.Char1, "char1"),
                2 => (ColumnType.Char2, "char2"),
                4 => (ColumnType.Char4, "char4"),
                8 => (ColumnType.Char8, "char8"),
                16 => (ColumnType.Char16, "char16"),
                32 => (ColumnType.Char32, "char32"),
                64 => (ColumnType.Char64, "char64"),
                128 => (ColumnType.Char128, "char128"),
                256 => (ColumnType.Char256, "char256"),
                _ => throw new ArgumentException($"Invalid char length: {length}. Must be 1, 2, 4, 8, 16, 32, 64, 128, or 256.", nameof(length))
            };
            _currentColumn = (name, type, new List<string> { prop });
            return this;
        }

        /// <summary>
        /// Adds a WKT (Well-Known Text) geometry column.
        /// </summary>
        public TypeBuilder AddWktColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Wkt, new List<string> { "wkt" });
            return this;
        }

        /// <summary>
        /// Adds a JSON column.
        /// </summary>
        public TypeBuilder AddJsonColumn(string name)
        {
            FinishCurrentColumn();
            _currentColumn = (name, ColumnType.Json, new List<string> { "json" });
            return this;
        }

        #endregion

        #region Property Modifiers

        /// <summary>
        /// Marks the current column as primary key.
        /// </summary>
        public TypeBuilder PrimaryKey()
        {
            if (_currentColumn.HasValue)
            {
                var (name, type, props) = _currentColumn.Value;
                if (!props.Contains("primary_key"))
                    props.Add("primary_key");
                _currentColumn = (name, type, props);
            }
            return this;
        }

        /// <summary>
        /// Marks the current column as shard key.
        /// </summary>
        public TypeBuilder ShardKey()
        {
            if (_currentColumn.HasValue)
            {
                var (name, type, props) = _currentColumn.Value;
                if (!props.Contains("shard_key"))
                    props.Add("shard_key");
                _currentColumn = (name, type, props);
            }
            return this;
        }

        /// <summary>
        /// Marks the current column as nullable.
        /// </summary>
        public TypeBuilder Nullable()
        {
            if (_currentColumn.HasValue)
            {
                var (name, type, props) = _currentColumn.Value;
                if (!props.Contains("nullable"))
                    props.Add("nullable");
                _currentColumn = (name, type, props);
            }
            return this;
        }

        /// <summary>
        /// Marks the current column as data column.
        /// </summary>
        public TypeBuilder Data()
        {
            if (_currentColumn.HasValue)
            {
                var (name, type, props) = _currentColumn.Value;
                if (!props.Contains("data"))
                    props.Add("data");
                _currentColumn = (name, type, props);
            }
            return this;
        }

        /// <summary>
        /// Adds a custom property to the current column.
        /// </summary>
        public TypeBuilder WithProperty(string property)
        {
            if (_currentColumn.HasValue && !string.IsNullOrEmpty(property))
            {
                var (name, type, props) = _currentColumn.Value;
                if (!props.Contains(property))
                    props.Add(property);
                _currentColumn = (name, type, props);
            }
            return this;
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Builds the Type.
        /// </summary>
        /// <returns>A new Type instance.</returns>
        public Type Build()
        {
            FinishCurrentColumn();
            return new Type(_label, _columns);
        }

        #endregion
    }
