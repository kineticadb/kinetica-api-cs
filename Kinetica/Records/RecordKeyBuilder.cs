using System.Text.RegularExpressions;

namespace kinetica.Records;

/// <summary>
/// Builds RecordKey objects from ShardKeyValues for routing.
    /// Equivalent to Rust's RecordKeyBuilder.
    ///
    /// This class is used by BulkInserter to compute shard routing for records
    /// that implement IShardKeyExtractor.
    /// </summary>
    public sealed class RecordKeyBuilder
    {
        private readonly IReadOnlyList<int> _shardKeyIndices;
        private readonly IReadOnlyList<ColumnType> _shardKeyTypes;
        private readonly IReadOnlyDictionary<int, (int Precision, int Scale)> _decimalInfos;
        private readonly IReadOnlyDictionary<int, int> _charLengths;
        private readonly int _bufferSize;
        private readonly IReadOnlyDictionary<string, int> _columnNameToShardIndex;

        /// <summary>
        /// Creates a RecordKeyBuilder from a Type definition.
        /// </summary>
        public RecordKeyBuilder(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var shardKeyIndices = new List<int>();
            var shardKeyTypes = new List<ColumnType>();
            var decimalInfos = new Dictionary<int, (int Precision, int Scale)>();
            var charLengths = new Dictionary<int, int>();
            var columnNameToShardIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int bufferSize = 0;

            for (int i = 0; i < type.ShardKeyIndices.Count; i++)
            {
                var colIdx = type.ShardKeyIndices[i];
                var col = type.GetColumn(colIdx)!;

                shardKeyIndices.Add(colIdx);
                shardKeyTypes.Add(col.ColumnType);
                columnNameToShardIndex[col.Name] = i;

                // Calculate buffer size contribution
                bufferSize += GetColumnBufferSize(col.ColumnType, col.Properties, i, decimalInfos, charLengths);
            }

            _shardKeyIndices = shardKeyIndices;
            _shardKeyTypes = shardKeyTypes;
            _decimalInfos = decimalInfos;
            _charLengths = charLengths;
            _bufferSize = bufferSize;
            _columnNameToShardIndex = columnNameToShardIndex;
        }

        /// <summary>
        /// Creates a RecordKeyBuilder from a KineticaType (for backwards compatibility).
        /// </summary>
        public RecordKeyBuilder(kinetica.KineticaType ktype, bool isPrimaryKey = false)
        {
            if (ktype == null)
                throw new ArgumentNullException(nameof(ktype));

            var shardKeyIndices = new List<int>();
            var shardKeyTypes = new List<ColumnType>();
            var decimalInfos = new Dictionary<int, (int Precision, int Scale)>();
            var charLengths = new Dictionary<int, int>();
            var columnNameToShardIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int bufferSize = 0;

            var columns = ktype.getColumns();
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                var props = col.getProperties();

                bool isKey = isPrimaryKey
                    ? props.Contains(kinetica.ColumnProperty.PRIMARY_KEY)
                    : props.Contains(kinetica.ColumnProperty.SHARD_KEY);

                if (isKey)
                {
                    var colType = DetermineColumnType(col);
                    shardKeyIndices.Add(i);
                    shardKeyTypes.Add(colType);
                    columnNameToShardIndex[col.getName()] = shardKeyIndices.Count - 1;

                    bufferSize += GetColumnBufferSizeFromKineticaColumn(col, shardKeyIndices.Count - 1, decimalInfos, charLengths);
                }
            }

            _shardKeyIndices = shardKeyIndices;
            _shardKeyTypes = shardKeyTypes;
            _decimalInfos = decimalInfos;
            _charLengths = charLengths;
            _bufferSize = bufferSize;
            _columnNameToShardIndex = columnNameToShardIndex;
        }

        /// <summary>
        /// Returns true if this builder has shard key columns.
        /// </summary>
        public bool HasKey => _shardKeyIndices.Count > 0;

        /// <summary>
        /// Gets the number of shard key columns.
        /// </summary>
        public int KeyColumnCount => _shardKeyIndices.Count;

        /// <summary>
        /// Builds a RecordKey from the given shard key values.
        /// </summary>
        public RecordKey? Build(ShardKeyValues values)
        {
            if (!HasKey)
                return null;

            if (values.Count != _shardKeyIndices.Count)
                return null;

            var key = new RecordKey(_bufferSize);

            for (int i = 0; i < values.Count; i++)
            {
                var (name, value) = values[i];

                // Find the shard key index for this column name
                if (!_columnNameToShardIndex.TryGetValue(name, out var shardIdx))
                {
                    // Column name doesn't match - try positional
                    shardIdx = i;
                    if (shardIdx >= _shardKeyTypes.Count)
                        return null;
                }

                var colType = _shardKeyTypes[shardIdx];

                if (!AddValueToKey(key, value, colType, shardIdx))
                    return null;
            }

            if (!key.IsValid)
                return null;

            key.ComputeHash();
            return key;
        }

        private bool AddValueToKey(RecordKey key, ShardKeyValue value, ColumnType colType, int shardIdx)
        {
            if (value.IsNull)
                return false; // Null shard key values make the key invalid

            switch (colType)
            {
                case ColumnType.Integer:
                case ColumnType.Int8:
                case ColumnType.Int16:
                case ColumnType.Boolean:
                    if (value.TryGetInt(out var intVal))
                    {
                        key.AddInt(intVal);
                        return true;
                    }
                    return false;

                case ColumnType.Long:
                case ColumnType.Timestamp:
                    if (value.TryGetLong(out var longVal))
                    {
                        key.AddLong(longVal);
                        return true;
                    }
                    return false;

                case ColumnType.Float:
                    key.AddFloat(value.AsFloat());
                    return true;

                case ColumnType.Double:
                    key.AddDouble(value.AsDouble());
                    return true;

                case ColumnType.String:
                case ColumnType.Char1:
                case ColumnType.Char2:
                case ColumnType.Char4:
                case ColumnType.Char8:
                case ColumnType.Char16:
                case ColumnType.Char32:
                case ColumnType.Char64:
                case ColumnType.Char128:
                case ColumnType.Char256:
                    if (value.TryGetString(out var strVal) && strVal != null)
                    {
                        // For char types, get the expected length
                        int charLen = _charLengths.TryGetValue(shardIdx, out var len) ? len : 0;
                        if (charLen > 0)
                            key.AddCharN(strVal, charLen);
                        else
                            key.AddString(strVal);
                        return true;
                    }
                    return false;

                case ColumnType.Date:
                    if (value.TryGetString(out var dateVal) && dateVal != null)
                    {
                        key.AddDate(dateVal);
                        return true;
                    }
                    return false;

                case ColumnType.DateTime:
                    if (value.TryGetString(out var dtVal) && dtVal != null)
                    {
                        key.AddDateTime(dtVal);
                        return true;
                    }
                    return false;

                case ColumnType.Time:
                    if (value.TryGetString(out var timeVal) && timeVal != null)
                    {
                        key.AddTime(timeVal);
                        return true;
                    }
                    return false;

                case ColumnType.Ipv4:
                    if (value.TryGetString(out var ipVal) && ipVal != null)
                    {
                        key.AddIpv4(ipVal);
                        return true;
                    }
                    return false;

                case ColumnType.Decimal:
                    if (value.TryGetString(out var decVal) && decVal != null)
                    {
                        var (precision, scale) = _decimalInfos.TryGetValue(shardIdx, out var info) ? info : (18, 4);
                        key.AddDecimal(decVal, precision, scale);
                        return true;
                    }
                    return false;

                case ColumnType.Uuid:
                    if (value.TryGetString(out var uuidVal) && uuidVal != null)
                    {
                        key.AddUuid(uuidVal);
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }

        #region Buffer Size Calculation

        private static int GetColumnBufferSize(
            ColumnType colType,
            IReadOnlyList<string> properties,
            int shardIdx,
            Dictionary<int, (int Precision, int Scale)> decimalInfos,
            Dictionary<int, int> charLengths)
        {
            switch (colType)
            {
                case ColumnType.Int8:
                    return 1;
                case ColumnType.Int16:
                    return 2;
                case ColumnType.Integer:
                case ColumnType.Boolean:
                case ColumnType.Ipv4:
                case ColumnType.Time:
                case ColumnType.Float:
                    return 4;
                case ColumnType.Long:
                case ColumnType.Timestamp:
                case ColumnType.Date:
                case ColumnType.DateTime:
                case ColumnType.Double:
                case ColumnType.String:
                case ColumnType.Uuid:
                    return 8;
                case ColumnType.Char1:
                    charLengths[shardIdx] = 1;
                    return 1;
                case ColumnType.Char2:
                    charLengths[shardIdx] = 2;
                    return 2;
                case ColumnType.Char4:
                    charLengths[shardIdx] = 4;
                    return 4;
                case ColumnType.Char8:
                    charLengths[shardIdx] = 8;
                    return 8;
                case ColumnType.Char16:
                    charLengths[shardIdx] = 16;
                    return 16;
                case ColumnType.Char32:
                    charLengths[shardIdx] = 32;
                    return 32;
                case ColumnType.Char64:
                    charLengths[shardIdx] = 64;
                    return 64;
                case ColumnType.Char128:
                    charLengths[shardIdx] = 128;
                    return 128;
                case ColumnType.Char256:
                    charLengths[shardIdx] = 256;
                    return 256;
                case ColumnType.Decimal:
                    // Parse decimal precision from properties
                    foreach (var prop in properties)
                    {
                        var match = Regex.Match(prop, @"decimal\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            int precision = int.Parse(match.Groups[1].Value);
                            int scale = int.Parse(match.Groups[2].Value);
                            decimalInfos[shardIdx] = (precision, scale);
                            return precision > 18 ? 12 : 8;
                        }
                    }
                    decimalInfos[shardIdx] = (18, 4);
                    return 8;
                default:
                    return 8; // Default to 8 bytes (Murmur hash)
            }
        }

        private static int GetColumnBufferSizeFromKineticaColumn(
            kinetica.KineticaType.Column col,
            int shardIdx,
            Dictionary<int, (int Precision, int Scale)> decimalInfos,
            Dictionary<int, int> charLengths)
        {
            var props = col.getProperties();

            if (props.Contains("int8")) return 1;
            if (props.Contains("int16")) return 2;
            if (props.Contains("ipv4") || props.Contains("time")) return 4;
            if (props.Contains("char1")) { charLengths[shardIdx] = 1; return 1; }
            if (props.Contains("char2")) { charLengths[shardIdx] = 2; return 2; }
            if (props.Contains("char4")) { charLengths[shardIdx] = 4; return 4; }
            if (props.Contains("char8")) { charLengths[shardIdx] = 8; return 8; }
            if (props.Contains("char16")) { charLengths[shardIdx] = 16; return 16; }
            if (props.Contains("char32")) { charLengths[shardIdx] = 32; return 32; }
            if (props.Contains("char64")) { charLengths[shardIdx] = 64; return 64; }
            if (props.Contains("char128")) { charLengths[shardIdx] = 128; return 128; }
            if (props.Contains("char256")) { charLengths[shardIdx] = 256; return 256; }

            foreach (var prop in props)
            {
                var match = Regex.Match(prop, @"decimal\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int precision = int.Parse(match.Groups[1].Value);
                    int scale = int.Parse(match.Groups[2].Value);
                    decimalInfos[shardIdx] = (precision, scale);
                    return precision > 18 ? 12 : 8;
                }
            }

            // Default sizes based on base type
            return col.getType() switch
            {
                kinetica.KineticaType.Column.ColumnType.INT => 4,
                kinetica.KineticaType.Column.ColumnType.LONG => 8,
                kinetica.KineticaType.Column.ColumnType.FLOAT => 4,
                kinetica.KineticaType.Column.ColumnType.DOUBLE => 8,
                kinetica.KineticaType.Column.ColumnType.STRING => 8,
                kinetica.KineticaType.Column.ColumnType.BYTES => 8,
                _ => 8
            };
        }

        private static ColumnType DetermineColumnType(kinetica.KineticaType.Column col)
        {
            var props = col.getProperties();

            if (props.Contains("boolean")) return ColumnType.Boolean;
            if (props.Contains("int8")) return ColumnType.Int8;
            if (props.Contains("int16")) return ColumnType.Int16;
            if (props.Contains("timestamp")) return ColumnType.Timestamp;
            if (props.Contains("date")) return ColumnType.Date;
            if (props.Contains("datetime")) return ColumnType.DateTime;
            if (props.Contains("time")) return ColumnType.Time;
            if (props.Contains("decimal")) return ColumnType.Decimal;
            if (props.Contains("ipv4")) return ColumnType.Ipv4;
            if (props.Contains("uuid")) return ColumnType.Uuid;
            if (props.Contains("char1")) return ColumnType.Char1;
            if (props.Contains("char2")) return ColumnType.Char2;
            if (props.Contains("char4")) return ColumnType.Char4;
            if (props.Contains("char8")) return ColumnType.Char8;
            if (props.Contains("char16")) return ColumnType.Char16;
            if (props.Contains("char32")) return ColumnType.Char32;
            if (props.Contains("char64")) return ColumnType.Char64;
            if (props.Contains("char128")) return ColumnType.Char128;
            if (props.Contains("char256")) return ColumnType.Char256;

            return col.getType() switch
            {
                kinetica.KineticaType.Column.ColumnType.INT => ColumnType.Integer,
                kinetica.KineticaType.Column.ColumnType.LONG => ColumnType.Long,
                kinetica.KineticaType.Column.ColumnType.FLOAT => ColumnType.Float,
                kinetica.KineticaType.Column.ColumnType.DOUBLE => ColumnType.Double,
                kinetica.KineticaType.Column.ColumnType.STRING => ColumnType.String,
                _ => ColumnType.String
            };
        }

        #endregion
    }
