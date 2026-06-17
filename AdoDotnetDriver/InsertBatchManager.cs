using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;

namespace KineticaAdo
{
    /// <summary>
    /// Manages bulk insert operations for the ADO.NET driver.
    /// Uses BulkInserter&lt;GenericRecord&gt; for high-performance batch insertion with:
    /// - Multi-head ingest with shard key routing
    /// - Backpressure control
    /// - Snappy compression (via Kinetica client)
    /// </summary>
    internal class InsertBatchManager : IAsyncDisposable, IDisposable
    {
        private readonly Kinetica _kinetica;
        private readonly ConcurrentDictionary<string, TableBatchContext> _tableContexts;
        private readonly InsertBatchOptions _options;
        private readonly SemaphoreSlim _contextLock;
        private volatile bool _isDisposed;

        // Metrics
        private long _totalRecordsInserted;
        private long _totalBatchesFlushed;
        private long _bufferedRecordCount;

        public InsertBatchManager(Kinetica kinetica, InsertBatchOptions? options = null)
        {
            _kinetica = kinetica ?? throw new ArgumentNullException(nameof(kinetica));
            _options = options ?? new InsertBatchOptions();
            _tableContexts = new ConcurrentDictionary<string, TableBatchContext>(StringComparer.OrdinalIgnoreCase);
            _contextLock = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Gets the total number of records inserted across all tables.
        /// </summary>
        public long TotalRecordsInserted => Interlocked.Read(ref _totalRecordsInserted);

        /// <summary>
        /// Gets the total number of batches flushed across all tables.
        /// </summary>
        public long TotalBatchesFlushed => Interlocked.Read(ref _totalBatchesFlushed);

        /// <summary>
        /// Gets the number of records currently buffered (not yet flushed).
        /// </summary>
        public long BufferedRecordCount => Interlocked.Read(ref _bufferedRecordCount);

        /// <summary>
        /// Inserts a record into the specified table using batch processing.
        /// </summary>
        public async Task<bool> InsertAsync(
            string tableName,
            IList<string> columnNames,
            IList<object?> values,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var context = await GetOrCreateContextAsync(tableName, columnNames, cancellationToken).ConfigureAwait(false);
            await context.InsertAsync(values, cancellationToken).ConfigureAwait(false);
            Interlocked.Increment(ref _bufferedRecordCount);

            return true;
        }

        /// <summary>
        /// Inserts multiple records into the specified table using batch processing.
        /// </summary>
        public async Task<int> InsertBatchAsync(
            string tableName,
            IList<string> columnNames,
            IList<IList<object?>> valuesList,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var context = await GetOrCreateContextAsync(tableName, columnNames, cancellationToken).ConfigureAwait(false);

            foreach (var values in valuesList)
            {
                await context.InsertAsync(values, cancellationToken).ConfigureAwait(false);
                Interlocked.Increment(ref _bufferedRecordCount);
            }

            return valuesList.Count;
        }

        /// <summary>
        /// Flushes all pending records for the specified table.
        /// </summary>
        public async Task<long> FlushTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_tableContexts.TryGetValue(tableName, out var context))
            {
                // Get the current buffered count before flushing
                var flushedCount = Interlocked.Exchange(ref _bufferedRecordCount, 0);
                await context.FlushAsync(cancellationToken).ConfigureAwait(false);
                Interlocked.Add(ref _totalRecordsInserted, flushedCount);
                Interlocked.Increment(ref _totalBatchesFlushed);
                return flushedCount;
            }

            return 0;
        }

        /// <summary>
        /// Flushes all pending records for all tables.
        /// </summary>
        public async Task<long> FlushAllAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            // Get the current buffered count before flushing
            var flushedCount = Interlocked.Exchange(ref _bufferedRecordCount, 0);

            var flushTasks = new List<Task>();

            foreach (var kvp in _tableContexts)
            {
                flushTasks.Add(kvp.Value.FlushAsync(cancellationToken));
            }

            await Task.WhenAll(flushTasks).ConfigureAwait(false);

            Interlocked.Add(ref _totalRecordsInserted, flushedCount);
            Interlocked.Add(ref _totalBatchesFlushed, flushTasks.Count);

            return flushedCount;
        }

        /// <summary>
        /// Gets the number of pending records for the specified table.
        /// </summary>
        public int GetPendingCount(string tableName)
        {
            if (_tableContexts.TryGetValue(tableName, out var context))
            {
                return (int)context.PendingBatches;
            }
            return 0;
        }

        /// <summary>
        /// Gets the total number of pending records across all tables.
        /// </summary>
        public int GetTotalPendingCount()
        {
            return (int)Interlocked.Read(ref _bufferedRecordCount);
        }

        private async Task<TableBatchContext> GetOrCreateContextAsync(
            string tableName,
            IList<string> columnNames,
            CancellationToken cancellationToken)
        {
            // Fast path: context already exists
            if (_tableContexts.TryGetValue(tableName, out var existingContext))
            {
                return existingContext;
            }

            // Slow path: need to create context with schema discovery
            await _contextLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Double-check after acquiring lock
                if (_tableContexts.TryGetValue(tableName, out existingContext))
                {
                    return existingContext;
                }

                // Discover table schema - run on thread pool to avoid blocking
                var ktype = await Task.Run(() => KineticaType.fromTable(_kinetica, tableName), cancellationToken).ConfigureAwait(false);

                var context = new TableBatchContext(
                    _kinetica,
                    tableName,
                    ktype,
                    columnNames,
                    _options);

                _tableContexts[tableName] = context;
                return context;
            }
            finally
            {
                _contextLock.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(InsertBatchManager));
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            // Flush all pending data synchronously
            try
            {
                Task.Run(async () => await FlushAllAsync().ConfigureAwait(false))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {
                // Ignore errors during disposal
            }

            foreach (var context in _tableContexts.Values)
            {
                context.Dispose();
            }

            _tableContexts.Clear();
            _contextLock.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            // Flush all pending data
            try
            {
                await FlushAllAsync().ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors during disposal
            }

            foreach (var context in _tableContexts.Values)
            {
                await context.DisposeAsync().ConfigureAwait(false);
            }

            _tableContexts.Clear();
            _contextLock.Dispose();
        }
    }

    /// <summary>
    /// Options for batch insert operations.
    /// </summary>
    internal class InsertBatchOptions
    {
        /// <summary>
        /// The number of records to batch before flushing. Default is 10,000.
        /// </summary>
        public int BatchSize { get; set; } = 10000;

        /// <summary>
        /// If true, updates existing records with matching primary keys. Default is false.
        /// </summary>
        public bool UpdateOnExistingPk { get; set; } = false;

        /// <summary>
        /// If true, returns individual errors for each failed record. Default is false.
        /// </summary>
        public bool ReturnIndividualErrors { get; set; } = false;

        /// <summary>
        /// Maximum number of retry attempts for failed batches. Default is 3.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Base delay in milliseconds between retries. Default is 100ms.
        /// </summary>
        public int RetryDelayMs { get; set; } = 100;

        /// <summary>
        /// Maximum in-flight batches for backpressure control. Default is 100.
        /// </summary>
        public int MaxInFlightBatches { get; set; } = 100;

        /// <summary>
        /// Number of stripes per worker queue. Default is processor count.
        /// </summary>
        public int NumStripes { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// Maximum concurrent flush workers. Default is processor count * 2.
        /// </summary>
        public int MaxFlushWorkers { get; set; } = Math.Max(4, Environment.ProcessorCount * 2);

    }

    /// <summary>
    /// Manages batch context for a single table using BulkInserter&lt;GenericRecord&gt;.
    /// </summary>
    internal class TableBatchContext : IAsyncDisposable, IDisposable
    {
        private readonly BulkInserter<GenericRecord> _inserter;
        private readonly kinetica.Records.Type _recordType;
        private readonly int[] _columnMapping; // Maps INSERT column index to schema column index

        public long CountInserted => _inserter.CountInserted;
        public long CountUpdated => _inserter.CountUpdated;
        public long PendingBatches => _inserter.PendingBatches;

        public TableBatchContext(
            Kinetica kinetica,
            string tableName,
            KineticaType ktype,
            IList<string> insertColumnNames,
            InsertBatchOptions options)
        {
            // Create Type from KineticaType
            _recordType = CreateTypeFromKineticaType(ktype);

            // Build column mapping from INSERT order to schema order
            _columnMapping = CreateColumnMapping(ktype, insertColumnNames);

            var bulkOptions = new BulkInserterOptions
            {
                BatchSize = options.BatchSize,
                MaxRetries = options.MaxRetries,
                MaxInFlightBatches = options.MaxInFlightBatches,
                NumStripes = options.NumStripes,
                MaxFlushWorkers = options.MaxFlushWorkers,
                InsertOptions = new Dictionary<string, string>()
            };

            if (options.UpdateOnExistingPk)
            {
                bulkOptions.InsertOptions["update_on_existing_pk"] = "true";
            }
            if (options.ReturnIndividualErrors)
            {
                bulkOptions.InsertOptions["return_individual_errors"] = "true";
            }

            _inserter = new BulkInserter<GenericRecord>(kinetica, tableName, ktype, bulkOptions);
        }

        private static kinetica.Records.Type CreateTypeFromKineticaType(KineticaType ktype)
        {
            var columns = ktype.getColumns();
            var columnDefs = new List<Column>();

            foreach (var col in columns)
            {
                var props = col.getProperties();
                var colType = DetermineColumnType(col);

                columnDefs.Add(new Column(col.getName(), colType, props));
            }

            return new kinetica.Records.Type("dynamic_record", columnDefs);
        }

        private static ColumnType DetermineColumnType(KineticaType.Column col)
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
                KineticaType.Column.ColumnType.INT => ColumnType.Integer,
                KineticaType.Column.ColumnType.LONG => ColumnType.Long,
                KineticaType.Column.ColumnType.FLOAT => ColumnType.Float,
                KineticaType.Column.ColumnType.DOUBLE => ColumnType.Double,
                KineticaType.Column.ColumnType.STRING => ColumnType.String,
                KineticaType.Column.ColumnType.BYTES => ColumnType.Bytes,
                _ => ColumnType.String
            };
        }

        private static int[] CreateColumnMapping(KineticaType ktype, IList<string> insertColumnNames)
        {
            var schemaColumns = ktype.getColumns();
            var mapping = new int[insertColumnNames.Count];

            for (int i = 0; i < insertColumnNames.Count; i++)
            {
                var insertCol = insertColumnNames[i];
                int schemaIndex = -1;

                for (int j = 0; j < schemaColumns.Count; j++)
                {
                    if (string.Equals(schemaColumns[j].getName(), insertCol, StringComparison.OrdinalIgnoreCase))
                    {
                        schemaIndex = j;
                        break;
                    }
                }

                if (schemaIndex == -1)
                {
                    throw new ArgumentException($"Column '{insertCol}' not found in table schema");
                }

                mapping[i] = schemaIndex;
            }

            return mapping;
        }

        public async Task InsertAsync(IList<object?> values, CancellationToken cancellationToken)
        {
            // Create a GenericRecord and populate with mapped values
            var record = _recordType.NewInstance();

            for (int i = 0; i < values.Count && i < _columnMapping.Length; i++)
            {
                var schemaIndex = _columnMapping[i];
                var value = values[i];

                if (value == null || value == DBNull.Value)
                {
                    record.PutNull(schemaIndex);
                }
                else
                {
                    // Get the column's expected type and convert the value accordingly
                    var column = _recordType.GetColumn(schemaIndex);
                    var colType = column?.ColumnType ?? ColumnType.String;

                    PutValueWithTypeConversion(record, schemaIndex, value, colType);
                }
            }

            await _inserter.InsertAsync(record, cancellationToken).ConfigureAwait(false);
        }

        private static void PutValueWithTypeConversion(GenericRecord record, int index, object value, ColumnType targetType)
        {
            switch (targetType)
            {
                case ColumnType.Integer:
                case ColumnType.Int8:
                case ColumnType.Int16:
                case ColumnType.Boolean:
                    // Integer types - convert numeric values to int
                    record.Put(index, Convert.ToInt32(value));
                    break;

                case ColumnType.Long:
                case ColumnType.Timestamp:
                    // Long types - convert numeric values to long
                    if (value is DateTime dt)
                        record.Put(index, new DateTimeOffset(dt).ToUnixTimeMilliseconds());
                    else if (value is DateTimeOffset dto)
                        record.Put(index, dto.ToUnixTimeMilliseconds());
                    else
                        record.Put(index, Convert.ToInt64(value));
                    break;

                case ColumnType.Float:
                    // Float - convert numeric values to float
                    record.Put(index, Convert.ToSingle(value));
                    break;

                case ColumnType.Double:
                    // Double - convert numeric values to double
                    record.Put(index, Convert.ToDouble(value));
                    break;

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
                case ColumnType.Wkt:
                case ColumnType.Json:
                case ColumnType.Ipv4:
                case ColumnType.Uuid:
                case ColumnType.Decimal:
                case ColumnType.Date:
                case ColumnType.DateTime:
                case ColumnType.Time:
                    // String-based types
                    if (value is DateTime dtVal)
                        record.Put(index, dtVal.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    else if (value is DateTimeOffset dtoVal)
                        record.Put(index, dtoVal.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    else if (value is decimal decVal)
                        record.Put(index, decVal.ToString());
                    else if (value is Guid guidVal)
                        record.Put(index, guidVal.ToString());
                    else
                        record.Put(index, value?.ToString() ?? string.Empty);
                    break;

                case ColumnType.Bytes:
                    if (value is byte[] bytesVal)
                        record.Put(index, bytesVal);
                    else
                        record.Put(index, Array.Empty<byte>());
                    break;

                default:
                    // Default: try to use the value directly
                    switch (value)
                    {
                        case int intVal:
                            record.Put(index, intVal);
                            break;
                        case long longVal:
                            record.Put(index, longVal);
                            break;
                        case float floatVal:
                            record.Put(index, floatVal);
                            break;
                        case double doubleVal:
                            record.Put(index, doubleVal);
                            break;
                        case string strVal:
                            record.Put(index, strVal);
                            break;
                        case bool boolVal:
                            record.Put(index, boolVal);
                            break;
                        case byte[] bytes:
                            record.Put(index, bytes);
                            break;
                        default:
                            record.Put(index, value?.ToString() ?? string.Empty);
                            break;
                    }
                    break;
            }
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            await _inserter.FlushAsync(cancellationToken).ConfigureAwait(false);

            // Wait for all pending batches to be processed
            while (_inserter.PendingBatches > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            }

            // Check for errors
            var errors = _inserter.DrainErrors();
            if (errors.Count > 0)
            {
                var errorMessages = string.Join("; ", errors.Select(e => e.Message));
                throw new KineticaException($"Batch insert failed with {errors.Count} error(s): {errorMessages}");
            }
        }

        public List<InsertError> GetErrors() => _inserter.DrainErrors();

        public void Dispose()
        {
            _inserter.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _inserter.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// SQL INSERT statement parser that extracts table name, column names, and values.
    /// </summary>
    internal class InsertStatementParser
    {
        // Pattern 1: INSERT INTO [schema.]table (columns) VALUES (values)
        private static readonly System.Text.RegularExpressions.Regex _insertWithColumnsPattern = new System.Text.RegularExpressions.Regex(
            @"^\s*INSERT\s+INTO\s+(?<table>[\w\.]+)\s*\(\s*(?<columns>[\w\s,]+)\s*\)\s*VALUES\s*\((?<values>.*)\)\s*$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline);

        // Pattern 2: INSERT INTO [schema.]table VALUES (values) - no column names
        private static readonly System.Text.RegularExpressions.Regex _insertWithoutColumnsPattern = new System.Text.RegularExpressions.Regex(
            @"^\s*INSERT\s+INTO\s+(?<table>[\w\.]+)\s+VALUES\s*\((?<values>.*)\)\s*$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline);

        // Pattern for extracting column names
        private static readonly System.Text.RegularExpressions.Regex _columnPattern = new System.Text.RegularExpressions.Regex(
            @"[\w]+",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        public static bool TryParse(string sql, out ParsedInsert? result)
        {
            result = null;

            // Try pattern with columns first
            var match = _insertWithColumnsPattern.Match(sql);
            string columnsStr = "";

            if (!match.Success)
            {
                // Try pattern without columns
                match = _insertWithoutColumnsPattern.Match(sql);
                if (!match.Success)
                {
                    return false;
                }
            }
            else
            {
                columnsStr = match.Groups["columns"].Value.Trim();
            }

            var tableName = match.Groups["table"].Value.Trim();
            var valuesStr = match.Groups["values"].Value.Trim();

            // Extract column names
            var columnNames = new List<string>();
            if (!string.IsNullOrEmpty(columnsStr))
            {
                var columnMatches = _columnPattern.Matches(columnsStr);
                foreach (System.Text.RegularExpressions.Match cm in columnMatches)
                {
                    columnNames.Add(cm.Value);
                }
            }

            // Extract values
            var values = ParseValues(valuesStr);

            result = new ParsedInsert
            {
                TableName = tableName,
                ColumnNames = columnNames,
                Values = values
            };

            return true;
        }

        private static List<object?> ParseValues(string valuesStr)
        {
            var values = new List<object?>();
            var currentValue = new System.Text.StringBuilder();
            int depth = 0;
            bool inString = false;
            char stringChar = '\0';

            for (int i = 0; i < valuesStr.Length; i++)
            {
                char c = valuesStr[i];

                if (inString)
                {
                    if (c == stringChar)
                    {
                        // Check for escaped quote
                        if (i + 1 < valuesStr.Length && valuesStr[i + 1] == stringChar)
                        {
                            currentValue.Append(c);
                            i++; // Skip next char
                        }
                        else
                        {
                            inString = false;
                        }
                    }
                    else
                    {
                        currentValue.Append(c);
                    }
                }
                else if (c == '\'' || c == '"')
                {
                    inString = true;
                    stringChar = c;
                }
                else if (c == '(')
                {
                    depth++;
                    currentValue.Append(c);
                }
                else if (c == ')')
                {
                    depth--;
                    currentValue.Append(c);
                }
                else if (c == ',' && depth == 0)
                {
                    values.Add(ParseSingleValue(currentValue.ToString().Trim()));
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // Add last value
            if (currentValue.Length > 0)
            {
                values.Add(ParseSingleValue(currentValue.ToString().Trim()));
            }

            return values;
        }

        private static object? ParseSingleValue(string valueStr)
        {
            // Check for NULL first (before string handling)
            if (string.Equals(valueStr, "NULL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // String value (quoted) - handle before empty check to preserve empty strings
            if ((valueStr.StartsWith("'") && valueStr.EndsWith("'")) ||
                (valueStr.StartsWith("\"") && valueStr.EndsWith("\"")))
            {
                return valueStr.Substring(1, valueStr.Length - 2)
                    .Replace("''", "'")
                    .Replace("\"\"", "\"");
            }

            // Boolean
            if (string.Equals(valueStr, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (string.Equals(valueStr, "FALSE", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Numeric
            if (long.TryParse(valueStr, out long longValue))
            {
                if (longValue >= int.MinValue && longValue <= int.MaxValue)
                {
                    return (int)longValue;
                }
                return longValue;
            }

            if (double.TryParse(valueStr, out double doubleValue))
            {
                return doubleValue;
            }

            // Return as string
            return valueStr;
        }
    }

    /// <summary>
    /// Parsed INSERT statement result.
    /// </summary>
    internal class ParsedInsert
    {
        public string TableName { get; set; } = string.Empty;
        public List<string> ColumnNames { get; set; } = new();
        public List<object?> Values { get; set; } = new();
    }
}
