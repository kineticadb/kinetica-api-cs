using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;

namespace Example
{
    /// <summary>
    /// Comprehensive example demonstrating all Kinetica supported types with:
    /// - Table creation with all column types and properties
    /// - Bulk insertion using the new high-performance BulkInserter
    /// - Record retrieval using getRecords API (general) and RecordRetriever (shard-key based multi-head)
    /// </summary>
    public static class AllTypesExample
    {
        /// <summary>
        /// Record class demonstrating all Kinetica supported data types.
        ///
        /// Kinetica has 6 primitive storage types: INT, LONG, FLOAT, DOUBLE, STRING, BYTES
        /// Complex types (DATE, TIME, DATETIME, DECIMAL, UUID, etc.) are modeled as STRING with properties.
        /// </summary>
        public class AllTypesRecord : IShardKeyExtractor
        {
            // ========== PRIMARY KEY / SHARD KEY ==========
            /// <summary>Primary key - auto-incrementing ID</summary>
            public int id { get; set; }

            // ========== INTEGER TYPES ==========
            /// <summary>Boolean stored as INT with BOOLEAN property (0 or 1)</summary>
            public int bool_col { get; set; }

            /// <summary>8-bit integer stored as INT with INT8 property (-128 to 127)</summary>
            public int int8_col { get; set; }

            /// <summary>16-bit integer stored as INT with INT16 property (-32768 to 32767)</summary>
            public int int16_col { get; set; }

            /// <summary>Standard 32-bit integer</summary>
            public int int_col { get; set; }

            /// <summary>Nullable 32-bit integer</summary>
            public int? nullable_int_col { get; set; }

            // ========== LONG TYPES ==========
            /// <summary>Standard 64-bit integer</summary>
            public long long_col { get; set; }

            /// <summary>Timestamp in milliseconds since Unix epoch (with TIMESTAMP property)</summary>
            public long timestamp_col { get; set; }

            /// <summary>Nullable 64-bit integer</summary>
            public long? nullable_long_col { get; set; }

            // ========== FLOATING POINT TYPES ==========
            /// <summary>32-bit floating point</summary>
            public float float_col { get; set; }

            /// <summary>Nullable 32-bit floating point</summary>
            public float? nullable_float_col { get; set; }

            /// <summary>64-bit floating point</summary>
            public double double_col { get; set; }

            /// <summary>Nullable 64-bit floating point</summary>
            public double? nullable_double_col { get; set; }

            // ========== FIXED-LENGTH STRING TYPES ==========
            /// <summary>1-character fixed string (CHAR1)</summary>
            public string char1_col { get; set; } = string.Empty;

            /// <summary>4-character fixed string (CHAR4)</summary>
            public string char4_col { get; set; } = string.Empty;

            /// <summary>8-character fixed string (CHAR8)</summary>
            public string char8_col { get; set; } = string.Empty;

            /// <summary>16-character fixed string (CHAR16)</summary>
            public string char16_col { get; set; } = string.Empty;

            /// <summary>32-character fixed string (CHAR32)</summary>
            public string char32_col { get; set; } = string.Empty;

            /// <summary>64-character fixed string (CHAR64)</summary>
            public string char64_col { get; set; } = string.Empty;

            /// <summary>128-character fixed string (CHAR128)</summary>
            public string char128_col { get; set; } = string.Empty;

            /// <summary>256-character fixed string (CHAR256)</summary>
            public string char256_col { get; set; } = string.Empty;

            // ========== VARIABLE-LENGTH STRING ==========
            /// <summary>Variable-length string (unlimited)</summary>
            public string string_col { get; set; } = string.Empty;

            /// <summary>Nullable variable-length string</summary>
            public string? nullable_string_col { get; set; }

            // ========== SPECIAL STRING TYPES ==========
            /// <summary>Date in format 'YYYY-MM-DD' (DATE property)</summary>
            public string date_col { get; set; } = string.Empty;

            /// <summary>Time in format 'HH:MM:SS.mmm' (TIME property)</summary>
            public string time_col { get; set; } = string.Empty;

            /// <summary>DateTime in format 'YYYY-MM-DD HH:MM:SS.mmm' (DATETIME property)</summary>
            public string datetime_col { get; set; } = string.Empty;

            /// <summary>Decimal number as string with DECIMAL property (NUMERIC 19,4)</summary>
            public string decimal_col { get; set; } = string.Empty;

            /// <summary>IPv4 address in format 'A.B.C.D' (IPV4 property)</summary>
            public string ipv4_col { get; set; } = string.Empty;

            /// <summary>UUID stored as string (UUID property)</summary>
            public string uuid_col { get; set; } = string.Empty;

            /// <summary>Unsigned long as string (ULONG property) - 0 to 18,446,744,073,709,551,615</summary>
            public string ulong_col { get; set; } = string.Empty;

            /// <summary>JSON data stored as string (JSON property)</summary>
            public string json_col { get; set; } = string.Empty;

            /// <summary>Well-Known Text geometry (WKT property)</summary>
            public string wkt_col { get; set; } = string.Empty;

            // Note: BYTES type (for VECTOR data) requires special handling and is not
            // directly supported through reflection-based type creation.
            // Use SQL CREATE TABLE with VECTOR(n) type for vector columns.

            public override string ToString()
            {
                return $"AllTypesRecord {{ id={id}, bool={bool_col}, int={int_col}, long={long_col}, " +
                       $"float={float_col:F2}, double={double_col:F4}, string='{string_col}', " +
                       $"date='{date_col}', time='{time_col}', datetime='{datetime_col}', " +
                       $"decimal='{decimal_col}', ipv4='{ipv4_col}', uuid='{uuid_col}' }}";
            }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues(("id", ShardKeyValue.Int(id)));
            }
        }

        /// <summary>
        /// Runs the comprehensive all-types example.
        /// </summary>
        /// <param name="serverUrl">Kinetica server URL (e.g., http://localhost:9191)</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        public static async Task RunAsync(string serverUrl, string username, string password)
        {
            Console.WriteLine();
            Console.WriteLine("=========================================================");
            Console.WriteLine("= All Types Example - BulkInserter & RecordRetriever    =");
            Console.WriteLine("=========================================================");
            Console.WriteLine();

            // Create Kinetica connection
            var options = new Kinetica.Options
            {
                Username = username,
                Password = password
            };
            var kdb = new Kinetica(serverUrl, options);

            const string tableName = "csharp_all_types_example";

            try
            {
                // Step 1: Create the table with all types
                Console.WriteLine("Step 1: Creating table with all Kinetica types...");
                await CreateTableAsync(kdb, tableName);
                Console.WriteLine($"  Table '{tableName}' created successfully.\n");

                // Step 2: Generate test records
                Console.WriteLine("Step 2: Generating test records...");
                const int recordCount = 1000;
                var records = GenerateRecords(recordCount);
                Console.WriteLine($"  Generated {recordCount} records.\n");

                // Step 3: Insert records using BulkInserter
                Console.WriteLine("Step 3: Inserting records using BulkInserter...");
                var insertStats = await InsertRecordsAsync(kdb, tableName, records);
                Console.WriteLine($"  Inserted: {insertStats.inserted}, Updated: {insertStats.updated}");
                Console.WriteLine($"  Throughput: {insertStats.throughput:N0} records/sec\n");

                // Step 4: Retrieve records using RecordRetriever
                Console.WriteLine("Step 4: Retrieving records using RecordRetriever...");
                var retrievedRecords = await RetrieveRecordsAsync(kdb, tableName);
                Console.WriteLine($"  Retrieved {retrievedRecords.Count} records.\n");

                // Step 5: Display sample records
                Console.WriteLine("Step 5: Sample records (first 5):");
                Console.WriteLine(new string('-', 100));
                foreach (var record in retrievedRecords.Take(5))
                {
                    Console.WriteLine($"  {record}");
                }
                Console.WriteLine(new string('-', 100));
                Console.WriteLine();

                // Step 6: Demonstrate filtered retrieval
                Console.WriteLine("Step 6: Filtered retrieval (id < 10 AND bool_col = 1)...");
                var filteredRecords = await RetrieveFilteredRecordsAsync(kdb, tableName, "id < 10 AND bool_col = 1");
                Console.WriteLine($"  Retrieved {filteredRecords.Count} filtered records.\n");

                // Step 7: Demonstrate sorted retrieval
                Console.WriteLine("Step 7: Sorted retrieval (ORDER BY double_col DESC, LIMIT 5)...");
                var sortedRecords = await RetrieveSortedRecordsAsync(kdb, tableName, "double_col", descending: true, limit: 5);
                Console.WriteLine($"  Top 5 by double_col DESC:");
                foreach (var record in sortedRecords)
                {
                    Console.WriteLine($"    id={record.id}, double_col={record.double_col:F4}");
                }
                Console.WriteLine();

                Console.WriteLine("All Types Example completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                // Cleanup: Drop the table
                Console.WriteLine($"\nCleaning up: Dropping table '{tableName}'...");
                try
                {
                    kdb.clearTable(tableName, "", new Dictionary<string, string>
                    {
                        [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE
                    });
                    Console.WriteLine("  Table dropped.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Warning: Could not drop table: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Creates a table with all supported Kinetica column types and properties.
        /// </summary>
        private static Task CreateTableAsync(Kinetica kdb, string tableName)
        {
            // Define column properties for each column type
            var columnProperties = new Dictionary<string, IList<string>>
            {
                // Primary key
                ["id"] = new List<string> { ColumnProperty.PRIMARY_KEY },

                // Integer type properties
                ["bool_col"] = new List<string> { ColumnProperty.BOOLEAN },
                ["int8_col"] = new List<string> { ColumnProperty.INT8 },
                ["int16_col"] = new List<string> { ColumnProperty.INT16 },
                ["nullable_int_col"] = new List<string> { ColumnProperty.NULLABLE },

                // Long type properties
                ["timestamp_col"] = new List<string> { ColumnProperty.TIMESTAMP },
                ["nullable_long_col"] = new List<string> { ColumnProperty.NULLABLE },

                // Float/Double nullable properties
                ["nullable_float_col"] = new List<string> { ColumnProperty.NULLABLE },
                ["nullable_double_col"] = new List<string> { ColumnProperty.NULLABLE },

                // Fixed-length string properties (CHAR1-CHAR256)
                ["char1_col"] = new List<string> { ColumnProperty.CHAR1 },
                ["char4_col"] = new List<string> { ColumnProperty.CHAR4 },
                ["char8_col"] = new List<string> { ColumnProperty.CHAR8 },
                ["char16_col"] = new List<string> { ColumnProperty.CHAR16 },
                ["char32_col"] = new List<string> { ColumnProperty.CHAR32 },
                ["char64_col"] = new List<string> { ColumnProperty.CHAR64 },
                ["char128_col"] = new List<string> { ColumnProperty.CHAR128 },
                ["char256_col"] = new List<string> { ColumnProperty.CHAR256 },

                // Nullable string
                ["nullable_string_col"] = new List<string> { ColumnProperty.NULLABLE },

                // Special string types
                ["date_col"] = new List<string> { ColumnProperty.DATE },
                ["time_col"] = new List<string> { ColumnProperty.TIME },
                ["datetime_col"] = new List<string> { ColumnProperty.DATETIME },
                ["decimal_col"] = new List<string> { ColumnProperty.DECIMAL },
                ["ipv4_col"] = new List<string> { ColumnProperty.IPV4 },
                ["uuid_col"] = new List<string> { ColumnProperty.UUID },
                ["ulong_col"] = new List<string> { ColumnProperty.ULONG },
                ["json_col"] = new List<string> { ColumnProperty.JSON },
                ["wkt_col"] = new List<string> { ColumnProperty.WKT }
            };

            // Clear any existing table
            kdb.clearTable(tableName, "", new Dictionary<string, string>
            {
                [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE
            });

            // Create the type from our record class with the specified properties
            var ktype = KineticaType.fromClass(typeof(AllTypesRecord), columnProperties);

            // Create the type in the database
            string typeId = ktype.create(kdb);

            // Create the table
            kdb.createTable(tableName, typeId);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Generates test records with all column types populated.
        /// </summary>
        private static List<AllTypesRecord> GenerateRecords(int count)
        {
            var records = new List<AllTypesRecord>(count);
            var random = new Random(42); // Fixed seed for reproducibility
            var baseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            for (int i = 0; i < count; i++)
            {
                var record = new AllTypesRecord
                {
                    // Primary key
                    id = i,

                    // Integer types
                    bool_col = random.Next(0, 2),                    // 0 or 1
                    int8_col = random.Next(-128, 128),               // -128 to 127
                    int16_col = random.Next(-32768, 32768),          // -32768 to 32767
                    int_col = random.Next(),                          // Full int range
                    nullable_int_col = i % 5 == 0 ? null : random.Next(0, 1000),

                    // Long types
                    long_col = random.NextInt64(),
                    timestamp_col = ((DateTimeOffset)baseTime.AddMinutes(i)).ToUnixTimeMilliseconds(),
                    nullable_long_col = i % 7 == 0 ? null : random.NextInt64(0, 1_000_000_000),

                    // Float types
                    float_col = (float)(random.NextDouble() * 1000),
                    nullable_float_col = i % 3 == 0 ? null : (float)(random.NextDouble() * 100),

                    // Double types
                    double_col = random.NextDouble() * 10000,
                    nullable_double_col = i % 4 == 0 ? null : random.NextDouble() * 500,

                    // Fixed-length strings
                    char1_col = ((char)('A' + (i % 26))).ToString(),
                    char4_col = $"C{i % 1000:D3}",
                    char8_col = $"CHAR{i % 10000:D4}",
                    char16_col = $"CHAR16_{i:D8}",
                    char32_col = $"This is a char32 value #{i}",
                    char64_col = $"This is a longer char64 column value for record #{i}",
                    char128_col = $"CHAR128 column with more text capacity. Record #{i}. Random: {random.Next()}",
                    char256_col = $"CHAR256 provides even more space for text data. Record #{i}. UUID: {Guid.NewGuid()}",

                    // Variable-length string
                    string_col = $"Variable length string for record {i} with random data: {Guid.NewGuid()}",
                    nullable_string_col = i % 6 == 0 ? null : $"Nullable string #{i}",

                    // Date/Time types
                    date_col = baseTime.AddDays(i).ToString("yyyy-MM-dd"),
                    time_col = baseTime.AddSeconds(i * 37).ToString("HH:mm:ss.fff"),
                    datetime_col = baseTime.AddMinutes(i).ToString("yyyy-MM-dd HH:mm:ss.fff"),

                    // Special string types
                    decimal_col = (random.NextDouble() * 100000).ToString("F4"),
                    ipv4_col = $"{random.Next(1, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 256)}",
                    uuid_col = Guid.NewGuid().ToString(),
                    ulong_col = ((ulong)random.NextInt64(0, long.MaxValue)).ToString(),
                    json_col = $"{{\"id\": {i}, \"name\": \"record_{i}\", \"active\": {(i % 2 == 0).ToString().ToLower()}}}",
                    wkt_col = $"POINT({-180 + random.NextDouble() * 360:F6} {-90 + random.NextDouble() * 180:F6})"
                };

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Inserts records using the high-performance BulkInserter.
        /// </summary>
        private static async Task<(long inserted, long updated, double throughput)> InsertRecordsAsync(
            Kinetica kdb, string tableName, List<AllTypesRecord> records)
        {
            var ktype = KineticaType.fromTable(kdb, tableName);

            var options = new BulkInserterOptions
            {
                BatchSize = 500,
                MaxInFlightBatches = 10,
                MaxRetries = 3
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            await using var inserter = new BulkInserter<AllTypesRecord>(kdb, tableName, ktype, options);

            // Insert all records in one batch call
            inserter.InsertBatch(records);

            // Close and flush all pending records
            await inserter.CloseAsync();

            sw.Stop();

            var throughput = records.Count / (sw.Elapsed.TotalMilliseconds / 1000.0);

            // Check for errors
            var errors = inserter.DrainErrors();
            if (errors.Count > 0)
            {
                Console.WriteLine($"  Warning: {errors.Count} errors occurred during insertion:");
                foreach (var error in errors.Take(5))
                {
                    Console.WriteLine($"    - {error.Message}");
                }
            }

            return (inserter.CountInserted, inserter.CountUpdated, throughput);
        }

        /// <summary>
        /// Retrieves all records from the table using getRecords API.
        /// </summary>
        private static Task<IList<AllTypesRecord>> RetrieveRecordsAsync(Kinetica kdb, string tableName)
        {
            var response = kdb.getRecords<AllTypesRecord>(tableName, 0, Kinetica.END_OF_SET);
            return Task.FromResult(response.data);
        }

        /// <summary>
        /// Retrieves filtered records using an expression.
        /// </summary>
        private static Task<IList<AllTypesRecord>> RetrieveFilteredRecordsAsync(
            Kinetica kdb, string tableName, string expression)
        {
            var options = new Dictionary<string, string>
            {
                [GetRecordsRequest.Options.EXPRESSION] = expression
            };
            var response = kdb.getRecords<AllTypesRecord>(tableName, 0, Kinetica.END_OF_SET, options);
            return Task.FromResult(response.data);
        }

        /// <summary>
        /// Retrieves sorted records with limit.
        /// </summary>
        private static Task<IList<AllTypesRecord>> RetrieveSortedRecordsAsync(
            Kinetica kdb, string tableName, string sortColumn, bool descending = false, int limit = 10)
        {
            var options = new Dictionary<string, string>
            {
                [GetRecordsRequest.Options.SORT_BY] = sortColumn,
                [GetRecordsRequest.Options.SORT_ORDER] = descending ? "descending" : "ascending"
            };
            var response = kdb.getRecords<AllTypesRecord>(tableName, 0, limit, options);
            return Task.FromResult(response.data);
        }

        /// <summary>
        /// Retrieves records by shard key using RecordRetriever (for multi-head retrieval).
        /// This is used when you have the shard key value and want to route directly to the appropriate worker.
        /// </summary>
        private static Task<IList<AllTypesRecord>> RetrieveByShardKeyAsync(
            Kinetica kdb, string tableName, AllTypesRecord keyRecord, string? additionalExpression = null)
        {
            var ktype = KineticaType.fromTable(kdb, tableName);
            var retriever = new RecordRetriever<AllTypesRecord>(kdb, tableName, ktype);
            var response = retriever.getRecordsByKey(keyRecord, additionalExpression!);
            return Task.FromResult(response.data);
        }
    }
}
