using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avro;
using kinetica;
using kinetica.Records;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace KineticaAdo
{
    /// <summary>
    /// Reads CSV/TSV/PSV files and inserts records into Kinetica using bulk insert.
    /// Supports local files and KiFS paths.
    /// </summary>
    internal class CsvFileReader
    {
        private readonly Kinetica _kinetica;
        private readonly InsertFromFileInfo _fileInfo;
        private readonly KineticaType? _tableType;
        private readonly List<string> _columnNames;
        private readonly Dictionary<string, int> _columnIndexMap;

        /// <summary>
        /// Gets the number of records successfully inserted.
        /// </summary>
        public long RecordsInserted { get; private set; }

        /// <summary>
        /// Gets the number of records skipped due to errors.
        /// </summary>
        public long RecordsSkipped { get; private set; }

        /// <summary>
        /// Gets any warnings generated during processing.
        /// </summary>
        public List<string> Warnings { get; } = new();

        /// <summary>
        /// Creates a new CsvFileReader for the specified table and file info.
        /// </summary>
        /// <param name="kinetica">The Kinetica client.</param>
        /// <param name="fileInfo">Information about the file to read.</param>
        /// <param name="tableType">The table type schema (optional, will be discovered if not provided).</param>
        public CsvFileReader(Kinetica kinetica, InsertFromFileInfo fileInfo, KineticaType? tableType = null)
        {
            _kinetica = kinetica ?? throw new ArgumentNullException(nameof(kinetica));
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            _tableType = tableType;
            _columnNames = new List<string>();
            _columnIndexMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reads the file(s) and inserts records into the table.
        /// </summary>
        /// <returns>Number of records inserted.</returns>
        public async Task<long> ReadAndInsertAsync(CancellationToken cancellationToken = default)
        {
            // If it's a KiFS path, use server-side import (supports all formats)
            if (_fileInfo.IsKifsPath)
            {
                return await ImportViaServerApiAsync(cancellationToken).ConfigureAwait(false);
            }

            // For non-delimited formats (Parquet, JSON, Avro, Shapefile), use server-side API
            // These formats require the file to be uploaded to KiFS first or use InsertRecordsFromFiles
            if (_fileInfo.Options.Format != FileFormat.DelimitedText &&
                _fileInfo.Options.Format != FileFormat.Auto)
            {
                return await ImportViaServerApiAsync(cancellationToken).ConfigureAwait(false);
            }

            // Get table type if not provided
            var tableType = _tableType ?? await Task.Run(
                () => KineticaType.fromTable(_kinetica, _fileInfo.TableName),
                cancellationToken).ConfigureAwait(false);

            // Clear table if requested
            if (_fileInfo.Options.InitialClear)
            {
                await ClearTableAsync(cancellationToken).ConfigureAwait(false);
            }

            // Create bulk inserter options
            var bulkOptions = new BulkInserterOptions
            {
                BatchSize = _fileInfo.Options.BatchSize,
                InsertOptions = new Dictionary<string, string>()
            };

            if (_fileInfo.Options.UpdateOnExistingPk)
                bulkOptions.InsertOptions["update_on_existing_pk"] = "true";
            if (_fileInfo.Options.IgnoreExistingPk)
                bulkOptions.InsertOptions["ignore_existing_pk"] = "true";
            if (_fileInfo.Options.TruncateStrings)
                bulkOptions.InsertOptions["truncate_strings"] = "true";

            // Determine columns to use
            var recordSchema = tableType.getSchema() as RecordSchema;
            if (recordSchema == null)
            {
                throw new KineticaException($"Table '{_fileInfo.TableName}' schema is not a record schema");
            }

            var insertColumns = _fileInfo.InsertColumns.Count > 0
                ? _fileInfo.InsertColumns
                : recordSchema.Fields.Select(f => f.Name).ToList();

            // For local delimited text files, process client-side
            // Create Type from KineticaType for GenericRecord
            var recordType = CreateTypeFromKineticaType(tableType);
            using var inserter = new BulkInserter<GenericRecord>(_kinetica, _fileInfo.TableName, tableType, bulkOptions);

            // Resolve file paths (supports glob patterns)
            var files = ResolveLocalFiles(_fileInfo.FilePath!);
            if (files.Count == 0)
            {
                throw new KineticaException($"No files found matching pattern: {_fileInfo.FilePath}");
            }

            foreach (var filePath in files)
            {
                await ProcessFileAsync(filePath, recordSchema, recordType, insertColumns, inserter, cancellationToken).ConfigureAwait(false);
            }

            // Flush remaining records
            await inserter.FlushAsync(cancellationToken).ConfigureAwait(false);

            // Wait for all pending batches
            while (inserter.PendingBatches > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            }

            // Check for errors
            var errors = inserter.DrainErrors();
            if (errors.Count > 0 && _fileInfo.Options.ErrorMode == FileErrorMode.Abort)
            {
                throw new KineticaException($"File insert failed with {errors.Count} error(s): {string.Join("; ", errors.Select(e => e.Message))}");
            }

            RecordsInserted = inserter.CountInserted;
            return RecordsInserted;
        }

        /// <summary>
        /// Synchronous version of ReadAndInsertAsync.
        /// </summary>
        public long ReadAndInsert()
        {
            return Task.Run(() => ReadAndInsertAsync(CancellationToken.None)).GetAwaiter().GetResult();
        }

        private async Task ClearTableAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var options = new Dictionary<string, string>
                {
                    [DeleteRecordsRequest.Options.DELETE_ALL_RECORDS] = DeleteRecordsRequest.Options.TRUE
                };
                _kinetica.deleteRecords(_fileInfo.TableName, new List<string>(), options);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Imports files using Kinetica's server-side InsertRecordsFromFiles API.
        /// This is used for KiFS paths and non-delimited text formats (Parquet, JSON, Avro, etc.).
        /// </summary>
        private async Task<long> ImportViaServerApiAsync(CancellationToken cancellationToken)
        {
            var options = _fileInfo.Options;

            // Build file paths list
            var filePaths = new List<string>();
            if (_fileInfo.IsKifsPath)
            {
                filePaths.Add(_fileInfo.KifsPath!);
            }
            else
            {
                // For local files, they need to be accessible to the server
                // This typically means they should be on a shared filesystem or uploaded to KiFS first
                var files = ResolveLocalFiles(_fileInfo.FilePath!);
                if (files.Count == 0)
                {
                    throw new KineticaException($"No files found matching pattern: {_fileInfo.FilePath}");
                }
                filePaths.AddRange(files);
            }

            // Build options dictionary
            var apiOptions = new Dictionary<string, string>();

            // Set file type
            var fileType = options.Format switch
            {
                FileFormat.Parquet => InsertRecordsFromFilesRequest.Options.PARQUET,
                FileFormat.Json => InsertRecordsFromFilesRequest.Options.JSON,
                FileFormat.Avro => InsertRecordsFromFilesRequest.Options.AVRO,
                FileFormat.Shapefile => InsertRecordsFromFilesRequest.Options.SHAPEFILE,
                FileFormat.DelimitedText => InsertRecordsFromFilesRequest.Options.DELIMITED_TEXT,
                _ => InsertRecordsFromFilesRequest.Options.DELIMITED_TEXT
            };
            apiOptions[InsertRecordsFromFilesRequest.Options.FILE_TYPE] = fileType;

            // Error handling
            var errorHandling = options.ErrorMode switch
            {
                FileErrorMode.Abort => InsertRecordsFromFilesRequest.Options.ABORT,
                FileErrorMode.Skip => InsertRecordsFromFilesRequest.Options.IGNORE_BAD_RECORDS,
                FileErrorMode.Permissive => InsertRecordsFromFilesRequest.Options.PERMISSIVE,
                _ => InsertRecordsFromFilesRequest.Options.ABORT
            };
            apiOptions[InsertRecordsFromFilesRequest.Options.ERROR_HANDLING] = errorHandling;

            // PK handling
            if (options.UpdateOnExistingPk)
                apiOptions[InsertRecordsFromFilesRequest.Options.UPDATE_ON_EXISTING_PK] = InsertRecordsFromFilesRequest.Options.TRUE;
            if (options.IgnoreExistingPk)
                apiOptions[InsertRecordsFromFilesRequest.Options.IGNORE_EXISTING_PK] = InsertRecordsFromFilesRequest.Options.TRUE;

            // Truncate table
            if (options.InitialClear)
                apiOptions[InsertRecordsFromFilesRequest.Options.TRUNCATE_TABLE] = InsertRecordsFromFilesRequest.Options.TRUE;

            // Text-specific options
            if (options.Format == FileFormat.DelimitedText || options.Format == FileFormat.Auto)
            {
                apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_DELIMITER] = options.Delimiter.ToString();
                apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_HAS_HEADER] = options.HasHeader
                    ? InsertRecordsFromFilesRequest.Options.TRUE
                    : InsertRecordsFromFilesRequest.Options.FALSE;
                apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_QUOTE_CHARACTER] = options.QuoteChar.ToString();

                if (options.EscapeChar != '\0')
                    apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_ESCAPE_CHARACTER] = options.EscapeChar.ToString();

                apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_NULL_STRING] = options.NullString;

                if (!string.IsNullOrEmpty(options.CommentPrefix))
                    apiOptions[InsertRecordsFromFilesRequest.Options.TEXT_COMMENT_STRING] = options.CommentPrefix;

                if (options.Skip > 0)
                    apiOptions[InsertRecordsFromFilesRequest.Options.SKIP_LINES] = options.Skip.ToString();
            }

            // Batch size
            apiOptions[InsertRecordsFromFilesRequest.Options.BATCH_SIZE] = options.BatchSize.ToString();

            // Dry run
            if (options.DryRun)
                apiOptions[InsertRecordsFromFilesRequest.Options.INGESTION_MODE] = InsertRecordsFromFilesRequest.Options.DRY_RUN;

            // Columns to load
            if (_fileInfo.InsertColumns.Count > 0)
            {
                apiOptions[InsertRecordsFromFilesRequest.Options.COLUMNS_TO_LOAD] = string.Join(",", _fileInfo.InsertColumns);
            }

            // Create and execute the request
            var request = new InsertRecordsFromFilesRequest(
                _fileInfo.TableName,
                filePaths,
                null, // modify_columns
                null, // create_table_options
                apiOptions);

            var response = await Task.Run(() => _kinetica.insertRecordsFromFiles(request), cancellationToken).ConfigureAwait(false);

            RecordsInserted = response.count_inserted;
            RecordsSkipped = response.count_skipped;

            // Add any info messages as warnings
            if (response.info != null)
            {
                foreach (var kvp in response.info)
                {
                    Warnings.Add($"{kvp.Key}: {kvp.Value}");
                }
            }

            return RecordsInserted;
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

        private async Task ProcessFileAsync(
            string filePath,
            RecordSchema recordSchema,
            kinetica.Records.Type recordType,
            IList<string> insertColumns,
            BulkInserter<GenericRecord> inserter,
            CancellationToken cancellationToken)
        {
            var options = _fileInfo.Options;
            var parser = new CsvParser(options.Delimiter, options.QuoteChar, options.EscapeChar);

            using var reader = new StreamReader(filePath, Encoding.UTF8);

            int lineNumber = 0;
            int linesSkipped = 0;
            var columnMapping = new List<int>(); // Maps file column index to table column index

            // Skip initial lines.  A null result from ReadLineAsync signals
            // end-of-stream (the async-safe alternative to reader.EndOfStream).
            while (linesSkipped < options.Skip)
            {
                if (await reader.ReadLineAsync().ConfigureAwait(false) is null)
                    break;
                lineNumber++;
                linesSkipped++;
            }

            // Process header if present.  Only read a header line when one is
            // expected; a null result means the file had no more lines.
            var headerLine = options.HasHeader
                ? await reader.ReadLineAsync().ConfigureAwait(false)
                : null;

            if (headerLine != null)
            {
                lineNumber++;

                if (!string.IsNullOrEmpty(headerLine))
                {
                    var headers = parser.ParseLine(headerLine);
                    columnMapping = BuildColumnMapping(headers, insertColumns);
                }
            }
            else
            {
                // Use positional mapping
                for (int i = 0; i < insertColumns.Count; i++)
                {
                    columnMapping.Add(i);
                }
            }

            // Process data lines.  Loop until ReadLineAsync returns null
            // (end-of-stream); avoids the synchronous reader.EndOfStream probe.
            long recordCount = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                    break;
                lineNumber++;

                if (line.Length == 0)
                    continue;

                // Skip comment lines
                if (!string.IsNullOrEmpty(options.CommentPrefix) && line.StartsWith(options.CommentPrefix))
                    continue;

                try
                {
                    var values = parser.ParseLine(line);
                    var record = recordType.NewInstance();

                    for (int i = 0; i < insertColumns.Count; i++)
                    {
                        var fileIndex = i < columnMapping.Count ? columnMapping[i] : -1;

                        if (fileIndex >= 0 && fileIndex < values.Length)
                        {
                            var stringValue = values[fileIndex];
                            var columnName = insertColumns[i];
                            var column = recordSchema.Fields
                                .FirstOrDefault(f => f.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                            if (column != null)
                            {
                                var value = ConvertValue(stringValue, column.Schema.Tag.ToString(), options);
                                SetRecordValue(record, columnName, value);
                            }
                        }
                    }

                    await inserter.InsertAsync(record, cancellationToken).ConfigureAwait(false);
                    recordCount++;

                    // Check limit
                    if (options.Limit > 0 && recordCount >= options.Limit)
                        break;
                }
                catch (Exception ex)
                {
                    if (options.ErrorMode == FileErrorMode.Abort)
                    {
                        throw new KineticaException($"Error at line {lineNumber} in file '{filePath}': {ex.Message}", ex);
                    }

                    RecordsSkipped++;
                    if (options.ErrorMode == FileErrorMode.Skip)
                    {
                        Warnings.Add($"Skipped line {lineNumber} in file '{filePath}': {ex.Message}");
                    }
                }
            }
        }

        private List<int> BuildColumnMapping(string[] headers, IList<string> insertColumns)
        {
            var mapping = new List<int>(insertColumns.Count);
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Length; i++)
            {
                headerMap[headers[i].Trim()] = i;
            }

            foreach (var col in insertColumns)
            {
                if (headerMap.TryGetValue(col, out int index))
                {
                    mapping.Add(index);
                }
                else
                {
                    // Column not found in file, will use null/default
                    mapping.Add(-1);
                    Warnings.Add($"Column '{col}' not found in file header");
                }
            }

            return mapping;
        }

        private static void SetRecordValue(GenericRecord record, string columnName, object? value)
        {
            if (value == null)
            {
                record.PutNull(columnName);
                return;
            }

            switch (value)
            {
                case int intVal:
                    record.Put(columnName, intVal);
                    break;
                case long longVal:
                    record.Put(columnName, longVal);
                    break;
                case float floatVal:
                    record.Put(columnName, floatVal);
                    break;
                case double doubleVal:
                    record.Put(columnName, doubleVal);
                    break;
                case string strVal:
                    record.Put(columnName, strVal);
                    break;
                case bool boolVal:
                    record.Put(columnName, boolVal);
                    break;
                case byte[] bytesVal:
                    record.Put(columnName, bytesVal);
                    break;
                default:
                    record.Put(columnName, value.ToString());
                    break;
            }
        }

        private object? ConvertValue(string stringValue, string avroType, FileInsertOptions options)
        {
            // Handle NULL values
            if (string.IsNullOrEmpty(stringValue) || stringValue == options.NullString)
            {
                return null;
            }

            // Convert based on Avro type
            return avroType.ToLowerInvariant() switch
            {
                "string" => stringValue,
                "int" => int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : null,
                "long" => long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var l) ? l : null,
                "float" => float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ? f : null,
                "double" => double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null,
                "boolean" => ParseBoolean(stringValue),
                "bytes" => Convert.FromBase64String(stringValue),
                _ => stringValue
            };
        }

        private static bool? ParseBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Trim().ToUpperInvariant();

            // Common true values
            if (value == "TRUE" || value == "T" || value == "YES" || value == "Y" || value == "1")
                return true;

            // Common false values
            if (value == "FALSE" || value == "F" || value == "NO" || value == "N" || value == "0")
                return false;

            return null;
        }

        private static IList<string> ResolveLocalFiles(string pattern)
        {
            // Check if it's a glob pattern or a direct file path
            if (!ContainsGlobCharacters(pattern))
            {
                if (File.Exists(pattern))
                {
                    return new List<string> { Path.GetFullPath(pattern) };
                }
                return new List<string>();
            }

            // Use glob matching
            var matcher = new Matcher();

            // Get the base directory and the pattern
            var (baseDir, globPattern) = SplitPathAndPattern(pattern);

            matcher.AddInclude(globPattern);

            var directoryInfo = new DirectoryInfo(baseDir);
            if (!directoryInfo.Exists)
            {
                return new List<string>();
            }

            var result = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));
            return result.Files.Select(f => Path.GetFullPath(Path.Combine(baseDir, f.Path))).ToList();
        }

        private static bool ContainsGlobCharacters(string path)
        {
            return path.Contains('*') || path.Contains('?') || path.Contains('[');
        }

        private static (string baseDir, string pattern) SplitPathAndPattern(string path)
        {
            var normalizedPath = path.Replace('\\', '/');
            var lastSeparatorBeforeGlob = -1;

            for (int i = 0; i < normalizedPath.Length; i++)
            {
                char c = normalizedPath[i];
                if (c == '*' || c == '?' || c == '[')
                    break;
                if (c == '/')
                    lastSeparatorBeforeGlob = i;
            }

            if (lastSeparatorBeforeGlob < 0)
            {
                return (Directory.GetCurrentDirectory(), normalizedPath);
            }

            return (normalizedPath.Substring(0, lastSeparatorBeforeGlob), normalizedPath.Substring(lastSeparatorBeforeGlob + 1));
        }
    }

    /// <summary>
    /// Simple CSV parser that handles quoted fields and escapes.
    /// </summary>
    internal class CsvParser
    {
        private readonly char _delimiter;
        private readonly char _quote;
        private readonly char _escape;

        /// <summary>
        /// Creates a new CSV parser with the specified options.
        /// </summary>
        /// <param name="delimiter">Field delimiter character.</param>
        /// <param name="quote">Quote character for string fields.</param>
        /// <param name="escape">Escape character (use '\0' for none).</param>
        public CsvParser(char delimiter = ',', char quote = '"', char escape = '\0')
        {
            _delimiter = delimiter;
            _quote = quote;
            _escape = escape;
        }

        /// <summary>
        /// Parses a single CSV line into fields.
        /// </summary>
        public string[] ParseLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;
            bool escaped = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (escaped)
                {
                    current.Append(c);
                    escaped = false;
                    continue;
                }

                if (_escape != '\0' && c == _escape)
                {
                    escaped = true;
                    continue;
                }

                if (c == _quote)
                {
                    // Check for escaped quote (double quote)
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == _quote)
                    {
                        current.Append(_quote);
                        i++; // Skip next quote
                        continue;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                if (c == _delimiter && !inQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            // Add last field
            fields.Add(current.ToString());

            return fields.ToArray();
        }
    }
}
