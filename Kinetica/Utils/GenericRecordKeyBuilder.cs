using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace kinetica.Utils;

/// <summary>
/// Builds expressions and routing keys for GenericRecord and dictionary-based records.
    /// Used for key lookup in the ADO.NET driver where we don't have strongly-typed records.
    /// </summary>
    internal sealed class GenericRecordKeyBuilder
    {
        /// <summary>
        /// Enumerate all the data types supported by Kinetica.
        /// </summary>
        private enum ColumnType
        {
            CHAR1,
            CHAR2,
            CHAR4,
            CHAR8,
            CHAR16,
            CHAR32,
            CHAR64,
            CHAR128,
            CHAR256,
            DATE,
            DATETIME,
            DECIMAL,
            DECIMAL_BIG,
            DOUBLE,
            FLOAT,
            INT,
            INT8,
            INT16,
            IPV4,
            LONG,
            STRING,
            TIME,
            TIMESTAMP,
            BOOLEAN
        }

        private struct DecimalInfo
        {
            public int Precision;
            public int Scale;
        }

        private static readonly Regex DECIMAL_REGEX = new Regex(@"decimal\s*\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);

        private readonly KineticaType _ktype;
        private readonly IList<int> _routingColumnIndices;
        private readonly IList<ColumnType> _columnTypes;
        private readonly IDictionary<int, DecimalInfo> _decimalInfos;
        private readonly IList<string> _routingColumnNames;
        private int _bufferSize;

        /// <summary>
        /// Creates a GenericRecordKeyBuilder for the given KineticaType.
        /// </summary>
        /// <param name="isPrimaryKey">If true, uses primary key columns; otherwise uses shard key columns.</param>
        /// <param name="ktype">The KineticaType describing the table schema.</param>
        public GenericRecordKeyBuilder(bool isPrimaryKey, KineticaType ktype)
        {
            _ktype = ktype ?? throw new ArgumentNullException(nameof(ktype));
            _bufferSize = 0;
            _routingColumnIndices = new List<int>();
            _columnTypes = new List<ColumnType>();
            _decimalInfos = new Dictionary<int, DecimalInfo>();
            _routingColumnNames = new List<string>();

            // Check for track type table
            bool hasTimestamp = false;
            bool hasX = false;
            bool hasY = false;
            int trackIdColumnIdx = -1;

            IList<KineticaType.Column> columns = ktype.getColumns();
            for (int i = 0; i < columns.Count; ++i)
            {
                KineticaType.Column column = columns[i];

                switch (column.getName())
                {
                    case "TRACKID":
                        trackIdColumnIdx = i;
                        break;
                    case "TIMESTAMP":
                        hasTimestamp = true;
                        break;
                    case "x":
                        hasX = true;
                        break;
                    case "y":
                        hasY = true;
                        break;
                }

                // Check for primary/shard key
                if (isPrimaryKey && column.getProperties().Contains(ColumnProperty.PRIMARY_KEY))
                {
                    _routingColumnIndices.Add(i);
                }
                else if (!isPrimaryKey && column.getProperties().Contains(ColumnProperty.SHARD_KEY))
                {
                    _routingColumnIndices.Add(i);
                }
            }

            // Handle track type table
            if (!isPrimaryKey && hasTimestamp && hasX && hasY && trackIdColumnIdx != -1)
            {
                if (_routingColumnIndices.Count == 0)
                {
                    _routingColumnIndices.Add(trackIdColumnIdx);
                }
                else if (_routingColumnIndices.Count != 1 || _routingColumnIndices[0] != trackIdColumnIdx)
                {
                    throw new KineticaException("Cannot have a shard key other than 'TRACKID' for track tables.");
                }
            }

            // Determine column types and buffer size
            foreach (int i in _routingColumnIndices)
            {
                KineticaType.Column column = columns[i];
                _routingColumnNames.Add(column.getName());

                switch (column.getType())
                {
                    case KineticaType.Column.ColumnType.FLOAT:
                        _columnTypes.Add(ColumnType.FLOAT);
                        _bufferSize += 4;
                        break;

                    case KineticaType.Column.ColumnType.DOUBLE:
                        _columnTypes.Add(ColumnType.DOUBLE);
                        _bufferSize += 8;
                        break;

                    case KineticaType.Column.ColumnType.INT:
                        if (column.getProperties().Contains(ColumnProperty.INT8))
                        {
                            _columnTypes.Add(ColumnType.INT8);
                            _bufferSize += 1;
                        }
                        else if (column.getProperties().Contains(ColumnProperty.INT16))
                        {
                            _columnTypes.Add(ColumnType.INT16);
                            _bufferSize += 2;
                        }
                        else
                        {
                            _columnTypes.Add(ColumnType.INT);
                            _bufferSize += 4;
                        }
                        break;

                    case KineticaType.Column.ColumnType.LONG:
                        if (column.getProperties().Contains(ColumnProperty.TIMESTAMP))
                        {
                            _columnTypes.Add(ColumnType.TIMESTAMP);
                        }
                        else
                        {
                            _columnTypes.Add(ColumnType.LONG);
                        }
                        _bufferSize += 8;
                        break;

                    case KineticaType.Column.ColumnType.STRING:
                        DetermineStringColumnType(column);
                        break;

                    case KineticaType.Column.ColumnType.BYTES:
                        throw new KineticaException("Cannot use bytes column as key.");

                    default:
                        throw new KineticaException($"Unknown column type: {column.getType()}");
                }
            }
        }

        private void DetermineStringColumnType(KineticaType.Column column)
        {
            var properties = column.getProperties();

            if (properties.Contains(ColumnProperty.CHAR1))
            {
                _columnTypes.Add(ColumnType.CHAR1);
                _bufferSize += 1;
            }
            else if (properties.Contains(ColumnProperty.CHAR2))
            {
                _columnTypes.Add(ColumnType.CHAR2);
                _bufferSize += 2;
            }
            else if (properties.Contains(ColumnProperty.CHAR4))
            {
                _columnTypes.Add(ColumnType.CHAR4);
                _bufferSize += 4;
            }
            else if (properties.Contains(ColumnProperty.CHAR8))
            {
                _columnTypes.Add(ColumnType.CHAR8);
                _bufferSize += 8;
            }
            else if (properties.Contains(ColumnProperty.CHAR16))
            {
                _columnTypes.Add(ColumnType.CHAR16);
                _bufferSize += 16;
            }
            else if (properties.Contains(ColumnProperty.CHAR32))
            {
                _columnTypes.Add(ColumnType.CHAR32);
                _bufferSize += 32;
            }
            else if (properties.Contains(ColumnProperty.CHAR64))
            {
                _columnTypes.Add(ColumnType.CHAR64);
                _bufferSize += 64;
            }
            else if (properties.Contains(ColumnProperty.CHAR128))
            {
                _columnTypes.Add(ColumnType.CHAR128);
                _bufferSize += 128;
            }
            else if (properties.Contains(ColumnProperty.CHAR256))
            {
                _columnTypes.Add(ColumnType.CHAR256);
                _bufferSize += 256;
            }
            else if (properties.Contains(ColumnProperty.DATE))
            {
                _columnTypes.Add(ColumnType.DATE);
                _bufferSize += 4;
            }
            else if (properties.Contains(ColumnProperty.DATETIME))
            {
                _columnTypes.Add(ColumnType.DATETIME);
                _bufferSize += 8;
            }
            else if (properties.Contains(ColumnProperty.TIME))
            {
                _columnTypes.Add(ColumnType.TIME);
                _bufferSize += 4;
            }
            else if (properties.Contains(ColumnProperty.IPV4))
            {
                _columnTypes.Add(ColumnType.IPV4);
                _bufferSize += 4;
            }
            else if (properties.Contains(ColumnProperty.DECIMAL))
            {
                // Parse decimal precision/scale from properties
                var decimalMatch = DECIMAL_REGEX.Match(string.Join(",", properties));
                int precision = 18;
                int scale = 4;
                if (decimalMatch.Success)
                {
                    precision = int.Parse(decimalMatch.Groups[1].Value);
                    scale = int.Parse(decimalMatch.Groups[2].Value);
                }

                _decimalInfos[_columnTypes.Count] = new DecimalInfo { Precision = precision, Scale = scale };

                if (precision <= 18)
                {
                    _columnTypes.Add(ColumnType.DECIMAL);
                    _bufferSize += 8;
                }
                else
                {
                    _columnTypes.Add(ColumnType.DECIMAL_BIG);
                    _bufferSize += 12;
                }
            }
            else
            {
                // Regular string - use 8 bytes for hash
                _columnTypes.Add(ColumnType.STRING);
                _bufferSize += 8;
            }
        }

        /// <summary>
        /// Returns whether this builder has any routing key columns.
        /// </summary>
        public bool HasKey()
        {
            return _routingColumnIndices.Count > 0;
        }

        /// <summary>
        /// Gets the names of the routing key columns.
        /// </summary>
        public IList<string> GetRoutingColumnNames()
        {
            return _routingColumnNames;
        }

        /// <summary>
        /// Builds a SQL expression for looking up records by key values.
        /// </summary>
        /// <param name="keyValues">Dictionary of column name -> value for the key columns.</param>
        /// <returns>A SQL expression like "(col1 = 'val1' and col2 = 123)"</returns>
        public string? BuildExpression(IDictionary<string, object?> keyValues)
        {
            if (_bufferSize == 0)
                return null;

            var expression = new StringBuilder("(");

            for (int i = 0; i < _routingColumnIndices.Count; ++i)
            {
                if (i > 0)
                    expression.Append(" and ");

                KineticaType.Column column = _ktype.getColumns()[_routingColumnIndices[i]];
                string columnName = column.getName();

                // Get value from dictionary
                object? value = null;
                if (keyValues.ContainsKey(columnName))
                {
                    value = keyValues[columnName];
                }

                // Handle null values
                if (value == null)
                {
                    expression.Append("is_null(");
                    expression.Append(columnName);
                    expression.Append(")");
                    continue;
                }

                // Add column comparison
                expression.Append("(");
                expression.Append(columnName);
                expression.Append(" = ");

                // Format value based on column type
                switch (_columnTypes[i])
                {
                    case ColumnType.CHAR1:
                    case ColumnType.CHAR2:
                    case ColumnType.CHAR4:
                    case ColumnType.CHAR8:
                    case ColumnType.CHAR16:
                    case ColumnType.CHAR32:
                    case ColumnType.CHAR64:
                    case ColumnType.CHAR128:
                    case ColumnType.CHAR256:
                    case ColumnType.DATE:
                    case ColumnType.DATETIME:
                    case ColumnType.IPV4:
                    case ColumnType.STRING:
                    case ColumnType.TIME:
                        // Quote string values and escape single quotes
                        var strValue = value.ToString()?.Replace("'", "''") ?? "";
                        expression.Append("'");
                        expression.Append(strValue);
                        expression.Append("'");
                        break;

                    case ColumnType.DECIMAL:
                    case ColumnType.DECIMAL_BIG:
                        if (value is decimal d)
                            expression.Append(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        else
                            expression.Append(Convert.ToDecimal(value).ToString(System.Globalization.CultureInfo.InvariantCulture));
                        break;

                    case ColumnType.DOUBLE:
                        if (value is double dbl)
                            expression.Append(dbl.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        else
                            expression.Append(Convert.ToDouble(value).ToString(System.Globalization.CultureInfo.InvariantCulture));
                        break;

                    case ColumnType.FLOAT:
                        if (value is float f)
                            expression.Append(f.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        else
                            expression.Append(Convert.ToSingle(value).ToString(System.Globalization.CultureInfo.InvariantCulture));
                        break;

                    default:
                        // Numeric types (INT, INT8, INT16, LONG, TIMESTAMP)
                        expression.Append(value.ToString());
                        break;
                }

                expression.Append(")");
            }

            expression.Append(")");
            return expression.ToString();
        }

        /// <summary>
        /// Builds a RecordKey for routing to the correct worker.
        /// </summary>
        /// <param name="keyValues">Dictionary of column name -> value for the key columns.</param>
        /// <returns>A RecordKey that can be used to route to the correct worker.</returns>
        internal RecordKey? Build(IDictionary<string, object?> keyValues)
        {
            if (_bufferSize == 0)
                return null;

            RecordKey key = new RecordKey(_bufferSize);

            for (int i = 0; i < _routingColumnIndices.Count; ++i)
            {
                KineticaType.Column column = _ktype.getColumns()[_routingColumnIndices[i]];
                string columnName = column.getName();

                object? value = null;
                if (keyValues.ContainsKey(columnName))
                {
                    value = keyValues[columnName];
                }

                // Add value to key based on type
                switch (_columnTypes[i])
                {
                    case ColumnType.CHAR1:
                        key.addCharN(value?.ToString(), 1);
                        break;
                    case ColumnType.CHAR2:
                        key.addCharN(value?.ToString(), 2);
                        break;
                    case ColumnType.CHAR4:
                        key.addCharN(value?.ToString(), 4);
                        break;
                    case ColumnType.CHAR8:
                        key.addCharN(value?.ToString(), 8);
                        break;
                    case ColumnType.CHAR16:
                        key.addCharN(value?.ToString(), 16);
                        break;
                    case ColumnType.CHAR32:
                        key.addCharN(value?.ToString(), 32);
                        break;
                    case ColumnType.CHAR64:
                        key.addCharN(value?.ToString(), 64);
                        break;
                    case ColumnType.CHAR128:
                        key.addCharN(value?.ToString(), 128);
                        break;
                    case ColumnType.CHAR256:
                        key.addCharN(value?.ToString(), 256);
                        break;
                    case ColumnType.DATE:
                        key.addDate(value?.ToString() ?? "");
                        break;
                    case ColumnType.DATETIME:
                        key.addDateTime(value?.ToString() ?? "");
                        break;
                    case ColumnType.DECIMAL:
                        {
                            var info = _decimalInfos.ContainsKey(i) ? _decimalInfos[i] : new DecimalInfo { Precision = 18, Scale = 4 };
                            if (value == null)
                                key.addDecimal(null, info.Precision, info.Scale);
                            else
                            {
                                // addDecimal expects a string representation
                                var decimalStr = Convert.ToDecimal(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                                key.addDecimal(decimalStr, info.Precision, info.Scale);
                            }
                        }
                        break;
                    case ColumnType.DECIMAL_BIG:
                        {
                            // For big decimals (precision > 18), use the same addDecimal method
                            // which handles both 8-byte and 12-byte decimals based on precision
                            var info = _decimalInfos.ContainsKey(i) ? _decimalInfos[i] : new DecimalInfo { Precision = 28, Scale = 4 };
                            if (value == null)
                                key.addDecimal(null, info.Precision, info.Scale);
                            else
                            {
                                var decimalStr = Convert.ToDecimal(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                                key.addDecimal(decimalStr, info.Precision, info.Scale);
                            }
                        }
                        break;
                    case ColumnType.DOUBLE:
                        if (value == null)
                            key.addDouble(null);
                        else
                            key.addDouble(Convert.ToDouble(value));
                        break;
                    case ColumnType.FLOAT:
                        if (value == null)
                            key.addFloat(null);
                        else
                            key.addFloat(Convert.ToSingle(value));
                        break;
                    case ColumnType.INT:
                        if (value == null)
                            key.addInt(null);
                        else
                            key.addInt(Convert.ToInt32(value));
                        break;
                    case ColumnType.INT8:
                        if (value == null)
                            key.addInt8(null);
                        else
                            key.addInt8(Convert.ToSByte(value));
                        break;
                    case ColumnType.INT16:
                        if (value == null)
                            key.addInt16(null);
                        else
                            key.addInt16(Convert.ToInt16(value));
                        break;
                    case ColumnType.IPV4:
                        key.addIPv4(value?.ToString() ?? "");
                        break;
                    case ColumnType.LONG:
                    case ColumnType.TIMESTAMP:
                        if (value == null)
                            key.addLong(null);
                        else
                            key.addLong(Convert.ToInt64(value));
                        break;
                    case ColumnType.STRING:
                        key.addString(value?.ToString() ?? "");
                        break;
                    case ColumnType.TIME:
                        key.addTime(value?.ToString() ?? "");
                        break;
                }
            }

            key.computeHashes();
            return key;
        }
    }
