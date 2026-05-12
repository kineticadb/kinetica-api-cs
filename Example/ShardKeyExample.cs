using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;

namespace Example
{
    /// <summary>
    /// Example demonstrating shard key usage with RecordRetriever for multi-head retrieval.
    ///
    /// Shard keys determine how data is distributed across worker nodes. When retrieving
    /// records by shard key using RecordRetriever, requests are routed directly to the
    /// appropriate worker node, enabling efficient multi-head retrieval.
    ///
    /// Allowed shard key types:
    /// - INT (with INT8, INT16 properties)
    /// - LONG (with TIMESTAMP property)
    /// - STRING (with CHAR1-CHAR256, IPV4, UUID, DATE, TIME, DATETIME properties)
    ///
    /// Note: FLOAT, DOUBLE, DECIMAL, and nullable columns cannot be shard keys.
    /// </summary>
    public static class ShardKeyExample
    {
        /// <summary>
        /// Record class with multiple shard key columns demonstrating allowed shard key types.
        /// The combination of all shard key columns determines the worker node for each record.
        ///
        /// Note: For RecordRetriever to work properly, shard keys should be simple types
        /// (INT, LONG, STRING with charN) that can be easily expressed in filter expressions.
        /// </summary>
        public class ShardedRecord : IShardKeyExtractor
        {
            // ========== PRIMARY KEY (also part of shard key) ==========
            /// <summary>Primary key - integer ID (also shard key)</summary>
            public int id { get; set; }

            // ========== SHARD KEY COLUMNS ==========
            // All these columns participate in sharding

            /// <summary>Region code - CHAR4 shard key for geographic distribution</summary>
            public string region_code { get; set; } = string.Empty;

            /// <summary>Category ID - INT16 shard key</summary>
            public int category_id { get; set; }

            // ========== NON-SHARD DATA COLUMNS ==========
            // These columns store data but don't affect sharding

            /// <summary>Event timestamp - stored as LONG (not part of shard key for simpler expression building)</summary>
            public long event_timestamp { get; set; }

            /// <summary>Device UUID - stored for reference</summary>
            public string device_uuid { get; set; } = string.Empty;

            /// <summary>Event name - regular string column</summary>
            public string event_name { get; set; } = string.Empty;

            /// <summary>Event value - double (cannot be shard key)</summary>
            public double event_value { get; set; }

            /// <summary>IP address of the client</summary>
            public string client_ip { get; set; } = string.Empty;

            /// <summary>Additional JSON metadata</summary>
            public string metadata { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"ShardedRecord {{ id={id}, region='{region_code}', category={category_id}, " +
                       $"event='{event_name}', value={event_value:F2} }}";
            }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues(
                    ("id", ShardKeyValue.Int(id)),
                    ("region_code", ShardKeyValue.String(region_code)),
                    ("category_id", ShardKeyValue.Int(category_id))
                );
            }
        }

        /// <summary>
        /// Runs the shard key example demonstrating RecordRetriever for multi-head retrieval.
        /// </summary>
        public static async Task RunAsync(string serverUrl, string username, string password)
        {
            Console.WriteLine();
            Console.WriteLine("=========================================================");
            Console.WriteLine("= Shard Key Example - RecordRetriever Multi-Head Demo   =");
            Console.WriteLine("=========================================================");
            Console.WriteLine();

            var options = new Kinetica.Options
            {
                Username = username,
                Password = password
            };
            var kdb = new Kinetica(serverUrl, options);

            const string tableName = "csharp_shard_key_example";

            try
            {
                // Step 1: Create table with shard keys
                Console.WriteLine("Step 1: Creating table with shard key columns...");
                CreateTable(kdb, tableName);
                Console.WriteLine($"  Table '{tableName}' created with composite shard key.");
                Console.WriteLine("  Shard key columns: id, region_code, category_id\n");

                // Step 2: Generate and insert records
                Console.WriteLine("Step 2: Generating test records...");
                const int recordCount = 500;
                var records = GenerateRecords(recordCount);
                Console.WriteLine($"  Generated {recordCount} records across 4 regions and 5 categories.\n");

                Console.WriteLine("Step 3: Inserting records using BulkInserter...");
                var insertStats = await InsertRecordsAsync(kdb, tableName, records);
                Console.WriteLine($"  Inserted: {insertStats.inserted}, Updated: {insertStats.updated}");
                Console.WriteLine($"  Throughput: {insertStats.throughput:N0} records/sec\n");

                // Step 4: Create RecordRetriever for multi-head retrieval
                Console.WriteLine("Step 4: Setting up RecordRetriever for multi-head retrieval...");
                var ktype = KineticaType.fromTable(kdb, tableName);
                var retriever = new RecordRetriever<ShardedRecord>(kdb, tableName, ktype);
                Console.WriteLine("  RecordRetriever initialized with worker routing.\n");

                // Step 5: Demonstrate retrieval by shard key
                Console.WriteLine("Step 5: Retrieving records by shard key (direct worker routing)...");
                Console.WriteLine(new string('-', 80));

                // Pick some sample records to query by their shard keys
                var sampleRecords = new[]
                {
                    records[0],   // First record
                    records[100], // Middle record
                    records[250], // Another middle record
                    records[499]  // Last record
                };

                foreach (var sampleRecord in sampleRecords)
                {
                    Console.WriteLine($"\n  Querying by shard key: id={sampleRecord.id}, region='{sampleRecord.region_code}', " +
                                    $"category={sampleRecord.category_id}");

                    try
                    {
                        // Create a key record with just the shard key values
                        var keyRecord = new ShardedRecord
                        {
                            id = sampleRecord.id,
                            region_code = sampleRecord.region_code,
                            category_id = sampleRecord.category_id
                        };

                        var response = retriever.getRecordsByKey(keyRecord);
                        Console.WriteLine($"  Found {response.data.Count} record(s):");
                        foreach (var record in response.data)
                        {
                            Console.WriteLine($"    -> {record}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Show full exception details for debugging
                        var innerMsg = ex.InnerException?.Message ?? ex.Message;
                        Console.WriteLine($"  Error: {innerMsg}");
                    }
                }

                Console.WriteLine(new string('-', 80));

                // Step 6: Demonstrate limitations of additional expressions
                Console.WriteLine("\nStep 6: Note on RecordRetriever expression limitations...");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("  RecordRetriever.getRecordsByKey() with additional expressions requires:");
                Console.WriteLine("  - Only equality (=) and AND operators");
                Console.WriteLine("  - Referenced columns must have attribute indexes");
                Console.WriteLine("  - No range operators (>, <, >=, <=)");
                Console.WriteLine("");
                Console.WriteLine("  For complex queries, use SQL via executeSql() or filter():");

                // Demo using SQL for complex queries after shard-based lookup
                var sqlResponse = kdb.executeSql(
                    $"SELECT id, region_code, category_id, event_name, event_value FROM {tableName} WHERE region_code = 'US-E' AND event_value > 40 LIMIT 3",
                    0, -9999);
                Console.WriteLine($"\n  SQL query (region='US-E' AND event_value > 40): Found {sqlResponse.total_number_of_records} records");
                foreach (KineticaRecord rec in sqlResponse.data.Take(3))
                {
                    Console.WriteLine($"    -> {rec.ContentsToString()}");
                }
                Console.WriteLine(new string('-', 80));

                // Step 7: Show distribution statistics
                Console.WriteLine("\nStep 7: Showing record distribution by region...");
                ShowDistributionByRegion(kdb, tableName);

                Console.WriteLine("\nShard Key Example completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
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
        /// Creates a table with composite shard key across multiple columns.
        /// </summary>
        private static void CreateTable(Kinetica kdb, string tableName)
        {
            var columnProperties = new Dictionary<string, IList<string>>
            {
                // Primary key and shard key - simple types that work well with RecordRetriever
                ["id"] = new List<string> { ColumnProperty.PRIMARY_KEY, ColumnProperty.SHARD_KEY },
                ["region_code"] = new List<string> { ColumnProperty.CHAR4, ColumnProperty.PRIMARY_KEY, ColumnProperty.SHARD_KEY },
                ["category_id"] = new List<string> { ColumnProperty.INT16, ColumnProperty.PRIMARY_KEY, ColumnProperty.SHARD_KEY },

                // Data columns (not part of shard key)
                ["event_timestamp"] = new List<string> { ColumnProperty.TIMESTAMP },
                ["device_uuid"] = new List<string> { ColumnProperty.UUID },
                ["client_ip"] = new List<string> { ColumnProperty.IPV4 },
                ["metadata"] = new List<string> { ColumnProperty.JSON }
            };

            // Clear existing table
            kdb.clearTable(tableName, "", new Dictionary<string, string>
            {
                [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE
            });

            // Create type and table
            var ktype = KineticaType.fromClass(typeof(ShardedRecord), columnProperties);
            string typeId = ktype.create(kdb);
            kdb.createTable(tableName, typeId);

            // Create attribute indexes on shard key columns for fast lookup
            // This is required for RecordRetriever.getRecordsByKey() to work
            Console.WriteLine("  Creating attribute indexes on shard key columns...");
            foreach (var col in new[] { "id", "region_code", "category_id" })
            {
                try
                {
                    kdb.alterTable(tableName, "create_index", col, new Dictionary<string, string>());
                }
                catch { /* Ignore if index already exists */ }
            }
        }

        /// <summary>
        /// Generates test records with varied shard key values.
        /// </summary>
        private static List<ShardedRecord> GenerateRecords(int count)
        {
            var records = new List<ShardedRecord>(count);
            var random = new Random(42);
            var baseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Define regions and categories for distribution
            var regions = new[] { "US-E", "US-W", "EU-W", "APAC" };
            var eventNames = new[] { "click", "view", "purchase", "signup", "logout" };

            for (int i = 0; i < count; i++)
            {
                var record = new ShardedRecord
                {
                    // Shard key columns
                    id = i,
                    region_code = regions[i % regions.Length],
                    category_id = (i % 5) + 1,  // Categories 1-5
                    event_timestamp = ((DateTimeOffset)baseTime.AddMinutes(i)).ToUnixTimeMilliseconds(),
                    device_uuid = GenerateDeterministicUuid(i),

                    // Data columns
                    event_name = eventNames[random.Next(eventNames.Length)],
                    event_value = random.NextDouble() * 100,
                    client_ip = $"{random.Next(1, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 256)}",
                    metadata = $"{{\"session_id\": \"{Guid.NewGuid()}\", \"page\": \"/page/{i % 20}\"}}"
                };

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Generates a deterministic UUID based on index for reproducibility.
        /// </summary>
        private static string GenerateDeterministicUuid(int index)
        {
            // Create a reproducible UUID based on index
            var bytes = new byte[16];
            BitConverter.GetBytes(index).CopyTo(bytes, 0);
            BitConverter.GetBytes(index * 31).CopyTo(bytes, 4);
            BitConverter.GetBytes(index * 17).CopyTo(bytes, 8);
            BitConverter.GetBytes(index * 13).CopyTo(bytes, 12);
            return new Guid(bytes).ToString();
        }

        /// <summary>
        /// Inserts records using BulkInserter.
        /// </summary>
        private static async Task<(long inserted, long updated, double throughput)> InsertRecordsAsync(
            Kinetica kdb, string tableName, List<ShardedRecord> records)
        {
            var ktype = KineticaType.fromTable(kdb, tableName);

            var options = new BulkInserterOptions
            {
                BatchSize = 200,
                MaxInFlightBatches = 5,
                MaxRetries = 3
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            await using var inserter = new BulkInserter<ShardedRecord>(kdb, tableName, ktype, options);
            inserter.InsertBatch(records);
            await inserter.CloseAsync();

            sw.Stop();

            var throughput = records.Count / (sw.Elapsed.TotalMilliseconds / 1000.0);

            var errors = inserter.DrainErrors();
            if (errors.Count > 0)
            {
                Console.WriteLine($"  Warning: {errors.Count} errors during insertion");
            }

            return (inserter.CountInserted, inserter.CountUpdated, throughput);
        }

        /// <summary>
        /// Shows record distribution across regions using aggregation.
        /// </summary>
        private static void ShowDistributionByRegion(Kinetica kdb, string tableName)
        {
            try
            {
                // Use SQL for cleaner aggregation
                var response = kdb.executeSql(
                    $"SELECT region_code, COUNT(*) as record_count FROM {tableName} GROUP BY region_code ORDER BY region_code",
                    0, -9999);

                Console.WriteLine("  Region distribution:");
                foreach (KineticaRecord record in response.data)
                {
                    Console.WriteLine($"    {record.ContentsToString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Could not get distribution: {ex.Message}");
            }
        }
    }
}
