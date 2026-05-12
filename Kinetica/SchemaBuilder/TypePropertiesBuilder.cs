using System;
using System.Collections.Generic;

namespace kinetica.SchemaBuilder;

/// <summary>
/// Builder for constructing type properties dictionaries.
    /// Equivalent to Rust's TypePropertiesBuilder.
    ///
    /// <example>
    /// <code>
    /// var properties = new TypePropertiesBuilder()
    ///     .Column("id", new KineticaColumnProperties().PrimaryKey().ShardKey())
    ///     .Timestamp("created_at")
    ///     .Data("description")
    ///     .Build();
    /// </code>
    /// </example>
    /// </summary>
    public sealed class TypePropertiesBuilder
    {
        private readonly Dictionary<string, IList<string>> _properties;

        /// <summary>
        /// Creates a new TypePropertiesBuilder.
        /// </summary>
        public TypePropertiesBuilder()
        {
            _properties = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Adds properties for a column.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="props">The column properties builder.</param>
        public TypePropertiesBuilder Column(string name, KineticaColumnProperties props)
        {
            _properties[name] = props.Build();
            return this;
        }

        /// <summary>
        /// Adds a single property to a column.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="property">The property to add.</param>
        public TypePropertiesBuilder ColumnProperty(string name, string property)
        {
            if (!_properties.TryGetValue(name, out var propList))
            {
                propList = new List<string>();
                _properties[name] = propList;
            }

            if (!propList.Contains(property))
                propList.Add(property);

            return this;
        }

        /// <summary>
        /// Marks a column as primary key.
        /// </summary>
        public TypePropertiesBuilder PrimaryKey(string name)
        {
            return ColumnProperty(name, "primary_key");
        }

        /// <summary>
        /// Marks a column as shard key.
        /// </summary>
        public TypePropertiesBuilder ShardKey(string name)
        {
            return ColumnProperty(name, "shard_key");
        }

        /// <summary>
        /// Marks a column as nullable.
        /// </summary>
        public TypePropertiesBuilder Nullable(string name)
        {
            return ColumnProperty(name, "nullable");
        }

        /// <summary>
        /// Marks a column as timestamp type.
        /// </summary>
        public TypePropertiesBuilder Timestamp(string name)
        {
            return ColumnProperty(name, "timestamp");
        }

        /// <summary>
        /// Marks a column as data column (text search enabled).
        /// </summary>
        public TypePropertiesBuilder Data(string name)
        {
            return ColumnProperty(name, "data");
        }

        /// <summary>
        /// Marks a column as IPv4 type.
        /// </summary>
        public TypePropertiesBuilder Ipv4(string name)
        {
            return ColumnProperty(name, "ipv4");
        }

        /// <summary>
        /// Marks a column as date type.
        /// </summary>
        public TypePropertiesBuilder Date(string name)
        {
            return ColumnProperty(name, "date");
        }

        /// <summary>
        /// Marks a column as datetime type.
        /// </summary>
        public TypePropertiesBuilder DateTime(string name)
        {
            return ColumnProperty(name, "datetime");
        }

        /// <summary>
        /// Marks a column as time type.
        /// </summary>
        public TypePropertiesBuilder Time(string name)
        {
            return ColumnProperty(name, "time");
        }

        /// <summary>
        /// Marks a column as decimal type.
        /// </summary>
        public TypePropertiesBuilder Decimal(string name)
        {
            return ColumnProperty(name, "decimal");
        }

        /// <summary>
        /// Marks a column as UUID type.
        /// </summary>
        public TypePropertiesBuilder Uuid(string name)
        {
            return ColumnProperty(name, "uuid");
        }

        /// <summary>
        /// Marks a column as boolean type.
        /// </summary>
        public TypePropertiesBuilder Boolean(string name)
        {
            return ColumnProperty(name, "boolean");
        }

        /// <summary>
        /// Marks a column as int8 type.
        /// </summary>
        public TypePropertiesBuilder Int8(string name)
        {
            return ColumnProperty(name, "int8");
        }

        /// <summary>
        /// Marks a column as int16 type.
        /// </summary>
        public TypePropertiesBuilder Int16(string name)
        {
            return ColumnProperty(name, "int16");
        }

        /// <summary>
        /// Marks a column as WKT (geometry) type.
        /// </summary>
        public TypePropertiesBuilder Wkt(string name)
        {
            return ColumnProperty(name, "wkt");
        }

        /// <summary>
        /// Marks a column as char(N) type.
        /// </summary>
        public TypePropertiesBuilder CharN(string name, int n)
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
                _ => throw new ArgumentException($"Invalid char length: {n}", nameof(n))
            };
            return ColumnProperty(name, prop);
        }

        /// <summary>
        /// Builds the properties dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, IList<string>> Build()
        {
            return new Dictionary<string, IList<string>>(_properties, StringComparer.OrdinalIgnoreCase);
        }
    }
