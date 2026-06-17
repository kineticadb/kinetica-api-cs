using Xunit;
using Xunit.Abstractions;
using Kinetica.Tests.Common;
using kinetica;
using kinetica.Records;

namespace Kinetica.Tests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class BulkInserterDebugTest
    {
        private readonly ITestOutputHelper _output;

        public BulkInserterDebugTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public class TestRecord : IShardKeyExtractor
        {
            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public double value { get; set; }
            public long timestamp { get; set; }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues();
            }
        }

        [Fact]
        public async Task DebugBulkInserterErrors()
        {
            using var ctx = new TestContext("bulk_debug");

            // Create table using SQL for simpler setup
            var tableName = ctx.QualifiedTable("debug_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, name VARCHAR(64), value DOUBLE, timestamp LONG, PRIMARY KEY (id))");

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
            _output.WriteLine($"KineticaType schema: {ktype.getSchema()}");

            // Performance test: 50,000 records with batch size 10,000
            const int recordCount = 50_000;
            const int batchSize = 10_000;

            var options = new BulkInserterOptions
            {
                BatchSize = batchSize,
                MaxRetries = 0,
                MaxInFlightBatches = 20
            };

            // Generate records first
            var records = new List<TestRecord>(recordCount);
            var baseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            for (int i = 0; i < recordCount; i++)
            {
                records.Add(new TestRecord
                {
                    id = i,
                    name = $"test_{i}",
                    value = i * 1.5,
                    timestamp = baseTimestamp + i
                });
            }

            // Warm up
            await using (var warmup = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options))
            {
                warmup.InsertBatch(records.Take(100).ToList());
                await warmup.CloseAsync();
            }

            // Clear the table
            ctx.Kinetica.executeSql($"DELETE FROM {tableName}");

            // Time the insertion
            var sw = System.Diagnostics.Stopwatch.StartNew();

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            // Use batch insert for maximum throughput
            inserter.InsertBatch(records);

            await inserter.CloseAsync();
            sw.Stop();

            var elapsedMs = sw.Elapsed.TotalMilliseconds;
            var throughput = recordCount / (elapsedMs / 1000.0);

            _output.WriteLine($"Records: {recordCount}");
            _output.WriteLine($"Batch size: {batchSize}");
            _output.WriteLine($"Elapsed: {elapsedMs:F2} ms");
            _output.WriteLine($"Throughput: {throughput:F0} records/sec");

            // Check errors
            var errors = inserter.DrainErrors();
            _output.WriteLine($"Total errors: {errors.Count}");
            foreach (var error in errors)
            {
                _output.WriteLine($"Error: {error.Message}");
                if (error.Exception != null)
                {
                    _output.WriteLine($"Exception: {error.Exception}");
                }
            }

            _output.WriteLine($"Count inserted: {inserter.CountInserted}");
            _output.WriteLine($"Count updated: {inserter.CountUpdated}");
            _output.WriteLine($"Batches sent: {inserter.TotalBatchesSent}");
            _output.WriteLine($"Batches failed: {inserter.TotalBatchesFailed}");

            // Also try the legacy ingestor to compare
            _output.WriteLine("\n--- Testing Legacy Ingestor ---");
#pragma warning disable CS0618 // intentionally exercising the deprecated KineticaIngestor<T> for comparison
            var legacyIngestor = new KineticaIngestor<TestRecord>(
                ctx.Kinetica,
                tableName,
                10,
                ktype);

            try
            {
                for (int i = 100; i < 115; i++)
                {
                    legacyIngestor.insert(new TestRecord
                    {
                        id = i,
                        name = $"test_{i}",
                        value = i * 1.5,
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    });
                }
                legacyIngestor.flush();
                _output.WriteLine($"Legacy inserted: {legacyIngestor.getCountInserted()}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Legacy error: {ex.Message}");
                _output.WriteLine($"Legacy exception: {ex}");
            }
#pragma warning restore CS0618
        }

        [Fact]
        public async Task CompareHttpImplementations()
        {
            using var ctx = new TestContext("http_compare");

            // Create table using SQL for simpler setup
            var tableName = ctx.QualifiedTable("http_compare_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, name VARCHAR(64), value DOUBLE, timestamp LONG, PRIMARY KEY (id))");

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);

            const int recordCount = 50_000;
            const int batchSize = 10_000;

            // Generate records
            var records = new List<TestRecord>(recordCount);
            var baseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            for (int i = 0; i < recordCount; i++)
            {
                records.Add(new TestRecord
                {
                    id = i,
                    name = $"test_{i}",
                    value = i * 1.5,
                    timestamp = baseTimestamp + i
                });
            }

            _output.WriteLine("=== BulkInserter Performance Test ===");
            _output.WriteLine($"Records: {recordCount}, Batch size: {batchSize}");
            _output.WriteLine("");

            var options = new BulkInserterOptions
            {
                BatchSize = batchSize,
                MaxRetries = 0,
                MaxInFlightBatches = 20
            };

            // Warmup
            await using (var warmup = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options))
            {
                warmup.InsertBatch(records.Take(100).ToList());
                await warmup.CloseAsync();
            }
            ctx.Kinetica.executeSql($"DELETE FROM {tableName}");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            await using (var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options))
            {
                inserter.InsertBatch(records);
                await inserter.CloseAsync();
            }
            sw.Stop();
            var elapsedMs = sw.Elapsed.TotalMilliseconds;
            var throughput = recordCount / (elapsedMs / 1000.0);
            _output.WriteLine($"Elapsed: {elapsedMs:F2} ms");
            _output.WriteLine($"Throughput: {throughput:F0} records/sec");
        }
    }
}
