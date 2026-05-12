using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kinetica.Records;

/// <summary>
/// A generic record that can hold values for any Kinetica type.
    /// Equivalent to Rust's GenericRecord struct.
    ///
    /// This class stores values in an array indexed by column position, and uses
    /// the associated Type for schema information. It implements both IKineticaRecord
    /// and IShardKeyExtractor for use with BulkInserter.
    ///
    /// <example>
    /// <code>
    /// var recordType = await Type.FromTableAsync(kinetica, "my_table");
    ///
    /// var record = recordType.NewInstance();
    ///
    /// // Set values by index
    /// record.Put(0, 42);
    /// record.Put(1, "Alice");
    ///
    /// // Set values by name
    /// record.Put("score", 95.5);
    ///
    /// // Get values
    /// int? id = record.GetInt(0);
    /// string? name = record.GetString("name");
    /// </code>
    /// </example>
    /// </summary>
    public sealed class GenericRecord : IBulkInsertable
    {
        private readonly Type _type;
        private readonly RecordValue[] _values;

        /// <summary>
        /// Creates a new GenericRecord for the given type.
        /// All values are initialized to Null.
        /// </summary>
        /// <param name="type">The Type definition for this record.</param>
        public GenericRecord(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _values = new RecordValue[type.ColumnCount];

            // Initialize all values to Null
            for (int i = 0; i < _values.Length; i++)
                _values[i] = RecordValue.Null();
        }

        /// <summary>Gets the Type definition for this record.</summary>
        public Type RecordType => _type;

        #region Put Methods (by index)

        /// <summary>Sets an integer value by column index.</summary>
        public bool Put(int index, int value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Int(value);
            return true;
        }

        /// <summary>Sets a long value by column index.</summary>
        public bool Put(int index, long value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Long(value);
            return true;
        }

        /// <summary>Sets a float value by column index.</summary>
        public bool Put(int index, float value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Float(value);
            return true;
        }

        /// <summary>Sets a double value by column index.</summary>
        public bool Put(int index, double value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Double(value);
            return true;
        }

        /// <summary>Sets a string value by column index.</summary>
        public bool Put(int index, string? value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = value != null ? RecordValue.String(value) : RecordValue.Null();
            return true;
        }

        /// <summary>Sets a bytes value by column index.</summary>
        public bool Put(int index, byte[]? value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = value != null ? RecordValue.Bytes(value) : RecordValue.Null();
            return true;
        }

        /// <summary>Sets a boolean value by column index.</summary>
        public bool Put(int index, bool value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Boolean(value);
            return true;
        }

        /// <summary>Sets a null value by column index.</summary>
        public bool PutNull(int index)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = RecordValue.Null();
            return true;
        }

        /// <summary>Sets a RecordValue by column index.</summary>
        public bool Put(int index, RecordValue value)
        {
            if (index < 0 || index >= _values.Length) return false;
            _values[index] = value;
            return true;
        }

        #endregion

        #region Put Methods (by name)

        /// <summary>Sets an integer value by column name.</summary>
        public bool Put(string name, int value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a long value by column name.</summary>
        public bool Put(string name, long value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a float value by column name.</summary>
        public bool Put(string name, float value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a double value by column name.</summary>
        public bool Put(string name, double value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a string value by column name.</summary>
        public bool Put(string name, string? value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a bytes value by column name.</summary>
        public bool Put(string name, byte[]? value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a boolean value by column name.</summary>
        public bool Put(string name, bool value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        /// <summary>Sets a null value by column name.</summary>
        public bool PutNull(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && PutNull(index.Value);
        }

        /// <summary>Sets a RecordValue by column name.</summary>
        public bool Put(string name, RecordValue value)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue && Put(index.Value, value);
        }

        #endregion

        #region Get Methods (by index)

        /// <summary>Gets the raw RecordValue at the specified index.</summary>
        public RecordValue? Get(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index];
        }

        /// <summary>Gets an integer value by column index.</summary>
        public int? GetInt(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsInt();
        }

        /// <summary>Gets a long value by column index.</summary>
        public long? GetLong(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsLong();
        }

        /// <summary>Gets a float value by column index.</summary>
        public float? GetFloat(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsFloat();
        }

        /// <summary>Gets a double value by column index.</summary>
        public double? GetDouble(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsDouble();
        }

        /// <summary>Gets a string value by column index.</summary>
        public string? GetString(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsString();
        }

        /// <summary>Gets a bytes value by column index.</summary>
        public byte[]? GetBytes(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsBytes();
        }

        /// <summary>Gets a boolean value by column index.</summary>
        public bool? GetBool(int index)
        {
            if (index < 0 || index >= _values.Length) return null;
            return _values[index].AsBool();
        }

        #endregion

        #region Get Methods (by name)

        /// <summary>Gets the raw RecordValue by column name.</summary>
        public RecordValue? Get(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? Get(index.Value) : null;
        }

        /// <summary>Gets an integer value by column name.</summary>
        public int? GetInt(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetInt(index.Value) : null;
        }

        /// <summary>Gets a long value by column name.</summary>
        public long? GetLong(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetLong(index.Value) : null;
        }

        /// <summary>Gets a float value by column name.</summary>
        public float? GetFloat(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetFloat(index.Value) : null;
        }

        /// <summary>Gets a double value by column name.</summary>
        public double? GetDouble(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetDouble(index.Value) : null;
        }

        /// <summary>Gets a string value by column name.</summary>
        public string? GetString(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetString(index.Value) : null;
        }

        /// <summary>Gets a bytes value by column name.</summary>
        public byte[]? GetBytes(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetBytes(index.Value) : null;
        }

        /// <summary>Gets a boolean value by column name.</summary>
        public bool? GetBool(string name)
        {
            var index = _type.GetColumnIndex(name);
            return index.HasValue ? GetBool(index.Value) : null;
        }

        #endregion

        #region IKineticaRecord Implementation

        /// <summary>
        /// Returns the Avro schema as a JSON string for this record type.
        /// </summary>
        public string GetAvroSchema() => _type.SchemaString;

        /// <summary>
        /// Returns type properties for this record type.
        /// </summary>
        public static IReadOnlyDictionary<string, IList<string>> GetTypeProperties(Type type)
        {
            var result = new Dictionary<string, IList<string>>();
            foreach (var col in type.Columns)
            {
                if (col.Properties.Count > 0)
                    result[col.Name] = col.Properties.ToList();
            }
            return result;
        }

        #endregion

        #region IShardKeyExtractor Implementation

        /// <summary>
        /// Returns shard key column names and their typed values.
        /// </summary>
        public ShardKeyValues GetShardKeyValues()
        {
            var shardKeyIndices = _type.ShardKeyIndices;
            if (shardKeyIndices.Count == 0)
                return ShardKeyValues.Empty;

            var values = new (string Name, ShardKeyValue Value)[shardKeyIndices.Count];

            for (int i = 0; i < shardKeyIndices.Count; i++)
            {
                var idx = shardKeyIndices[i];
                var col = _type.GetColumn(idx)!;
                var recordValue = _values[idx];

                var shardValue = ConvertToShardKeyValue(recordValue, col.ColumnType);
                values[i] = (col.Name, shardValue);
            }

            return new ShardKeyValues(values);
        }

        private static ShardKeyValue ConvertToShardKeyValue(RecordValue value, ColumnType columnType)
        {
            if (value.IsNull)
                return ShardKeyValue.Null();

            return columnType switch
            {
                ColumnType.Integer or ColumnType.Int8 or ColumnType.Int16 or ColumnType.Boolean
                    => ShardKeyValue.Int(value.AsInt() ?? 0),

                ColumnType.Long
                    => ShardKeyValue.Long(value.AsLong() ?? 0),

                ColumnType.Timestamp
                    => ShardKeyValue.Timestamp(value.AsLong() ?? 0),

                ColumnType.Float
                    => ShardKeyValue.Float(value.AsFloat() ?? 0),

                ColumnType.Double
                    => ShardKeyValue.Double(value.AsDouble() ?? 0),

                ColumnType.Date
                    => ShardKeyValue.Date(value.AsString() ?? ""),

                ColumnType.DateTime
                    => ShardKeyValue.DateTime(value.AsString() ?? ""),

                ColumnType.Time
                    => ShardKeyValue.Time(value.AsString() ?? ""),

                ColumnType.Ipv4
                    => ShardKeyValue.Ipv4(value.AsString() ?? ""),

                ColumnType.Decimal
                    => ShardKeyValue.Decimal(value.AsString() ?? ""),

                ColumnType.Uuid
                    => ShardKeyValue.Uuid(value.AsString() ?? ""),

                _ => ShardKeyValue.String(value.AsString() ?? "")
            };
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Creates a copy of this record.
        /// </summary>
        public GenericRecord Clone()
        {
            var copy = new GenericRecord(_type);
            Array.Copy(_values, copy._values, _values.Length);
            return copy;
        }

        /// <summary>
        /// Resets all values to null.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _values.Length; i++)
                _values[i] = RecordValue.Null();
        }

        /// <summary>
        /// Gets all values as an array (for serialization).
        /// </summary>
        internal RecordValue[] GetValues() => _values;

        /// <summary>
        /// Gets all values as objects (for Avro serialization).
        /// </summary>
        public object?[] ToObjectArray()
        {
            var result = new object?[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                var value = _values[i];
                result[i] = value.Type switch
                {
                    RecordValueType.Null => null,
                    RecordValueType.Int => value.AsInt(),
                    RecordValueType.Long => value.AsLong(),
                    RecordValueType.Float => value.AsFloat(),
                    RecordValueType.Double => value.AsDouble(),
                    RecordValueType.String => value.AsString(),
                    RecordValueType.Bytes => value.AsBytes(),
                    _ => null
                };
            }
            return result;
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(_type.Label);
            sb.Append(" { ");

            for (int i = 0; i < _type.ColumnCount; i++)
            {
                if (i > 0) sb.Append(", ");
                var col = _type.GetColumn(i)!;
                sb.Append(col.Name);
                sb.Append(": ");
                sb.Append(_values[i]);
            }

            sb.Append(" }");
            return sb.ToString();
        }
    }
