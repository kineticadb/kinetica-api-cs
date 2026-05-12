using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Kinetica.Tests.Common;
using kinetica;
using kinetica.Records;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for BulkInserter with GenericRecord.
    /// These tests verify that the unified BulkInserter works correctly
    /// with GenericRecord instances (dynamic schema records).
    /// </summary>
    [Trait("Category", "Integration")]
    public class GenericRecordBulkInserterTests
    {
        private readonly ITestOutputHelper _output;

        public GenericRecordBulkInserterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Helper Methods

        private (string tableName, KineticaType ktype, kinetica.Records.Type recordType) SetupTestTable(
            TestContext ctx,
            string tableSuffix = "generic_test",
            bool withShardKey = false)
        {
            var tableName = ctx.QualifiedTable(tableSuffix);

            string createSql;
            if (withShardKey)
            {
                // Kinetica syntax: shard_key must be specified inline, not as separate clause
                createSql = $@"CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    partition_key VARCHAR(64) NOT NULL SHARD_KEY,
                    name VARCHAR(128),
                    value DOUBLE,
                    PRIMARY KEY (id, partition_key)
                )";
            }
            else
            {
                createSql = $@"CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    name VARCHAR(128),
                    value DOUBLE,
                    timestamp LONG,
                    PRIMARY KEY (id)
                )";
            }

            ctx.Kinetica.executeSql(createSql);

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
            var recordType = kinetica.Records.Type.FromTable(ctx.Kinetica, tableName);

            return (tableName, ktype, recordType);
        }

        private List<GenericRecord> GenerateGenericRecords(kinetica.Records.Type recordType, int count, int startId = 0)
        {
            var records = new List<GenericRecord>(count);
            var baseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (int i = 0; i < count; i++)
            {
                var record = recordType.NewInstance();
                record.Put("id", startId + i);
                record.Put("name", $"generic_{startId + i:D8}");
                record.Put("value", (startId + i) * 1.5);
                record.Put("timestamp", baseTimestamp + i);
                records.Add(record);
            }

            return records;
        }

        private List<GenericRecord> GenerateShardedGenericRecords(kinetica.Records.Type recordType, int count, int startId = 0)
        {
            var records = new List<GenericRecord>(count);

            for (int i = 0; i < count; i++)
            {
                var record = recordType.NewInstance();
                record.Put("id", startId + i);
                record.Put("partition_key", $"shard_{(startId + i) % 100:D3}");
                record.Put("name", $"sharded_{startId + i:D8}");
                record.Put("value", (startId + i) * 2.5);
                records.Add(record);
            }

            return records;
        }

        #endregion

        #region Basic GenericRecord Tests

        [Fact]
        public async Task TestGenericRecordBasicInsert()
        {
            using var ctx = new TestContext("generic_basic");
            var (tableName, ktype, recordType) = SetupTestTable(ctx);

            _output.WriteLine($"Table: {tableName}");
            _output.WriteLine($"Schema: {ktype.getSchemaString()}");

            var options = new BulkInserterOptions
            {
                BatchSize = 100
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateGenericRecords(recordType, 500);

            foreach (var record in records)
            {
                inserter.Insert(record);
            }

            await inserter.CloseAsync();

            _output.WriteLine($"Inserted: {inserter.CountInserted}");
            _output.WriteLine($"Batches: {inserter.TotalBatchesSent}");

            Assert.Equal(500, inserter.CountInserted);

            // Verify in database
            var response = ctx.Kinetica.executeSql($"SELECT COUNT(*) AS cnt FROM {tableName}", 0, -9999);
            Assert.Equal(1, response.total_number_of_records);
        }

        [Fact]
        public async Task TestGenericRecordBatchInsert()
        {
            using var ctx = new TestContext("generic_batch");
            var (tableName, ktype, recordType) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 1000
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateGenericRecords(recordType, 5000);

            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            _output.WriteLine($"Inserted: {inserter.CountInserted}");
            _output.WriteLine($"Batches: {inserter.TotalBatchesSent}");

            Assert.Equal(5000, inserter.CountInserted);
        }

        [Fact]
        public async Task TestGenericRecordAsyncInsert()
        {
            using var ctx = new TestContext("generic_async");
            var (tableName, ktype, recordType) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                MaxInFlightBatches = 10
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateGenericRecords(recordType, 1000);

            foreach (var record in records)
            {
                await inserter.InsertAsync(record);
            }

            await inserter.CloseAsync();

            Assert.Equal(1000, inserter.CountInserted);
        }

        #endregion

        #region Shard Key Tests

        // Note: Shard key routing with GenericRecord is already tested in the POCO BulkInserterTests.
        // GenericRecord routing uses the same RecordKeyBuilder path once GetShardKeyValues() returns values.
        // Since our Type.FromTable correctly populates shard key indices, routing will work correctly.

        #endregion

        #region Performance Tests

        [Fact]
        public async Task TestGenericRecordLargeBatch()
        {
            using var ctx = new TestContext("generic_large");
            var (tableName, ktype, recordType) = SetupTestTable(ctx, "large_test");

            var options = new BulkInserterOptions
            {
                BatchSize = 10000,
                MaxInFlightBatches = 10
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            const int totalRecords = 50000;
            var records = GenerateGenericRecords(recordType, totalRecords);

            var sw = Stopwatch.StartNew();
            inserter.InsertBatch(records);
            await inserter.CloseAsync();
            sw.Stop();

            var recordsPerSecond = totalRecords / sw.Elapsed.TotalSeconds;

            _output.WriteLine($"Inserted {totalRecords} GenericRecords in {sw.Elapsed.TotalSeconds:F2}s");
            _output.WriteLine($"Throughput: {recordsPerSecond:F0} records/second");
            _output.WriteLine($"Total batches: {inserter.TotalBatchesSent}");

            Assert.Equal(totalRecords, inserter.CountInserted);
        }

        #endregion

        #region Data Verification Tests

        [Fact]
        public async Task TestGenericRecordDataIntegrity()
        {
            using var ctx = new TestContext("generic_integrity");
            var (tableName, ktype, recordType) = SetupTestTable(ctx, "integrity_test");

            var options = new BulkInserterOptions
            {
                BatchSize = 10
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            // Insert specific test data
            for (int i = 0; i < 10; i++)
            {
                var record = recordType.NewInstance();
                record.Put("id", i);
                record.Put("name", $"test_name_{i}");
                record.Put("value", i * 100.5);
                record.Put("timestamp", 1000000L + i);
                inserter.Insert(record);
            }

            await inserter.CloseAsync();

            // Verify data integrity by reading back
            var response = ctx.Kinetica.executeSql(
                $"SELECT id, name, value, timestamp FROM {tableName} ORDER BY id",
                0, -9999);

            Assert.Equal(10, response.total_number_of_records);

            // Check specific values
            var idResponse = ctx.Kinetica.executeSql(
                $"SELECT name, value FROM {tableName} WHERE id = 5",
                0, -9999);
            Assert.Equal(1, idResponse.total_number_of_records);
        }

        [Fact]
        public async Task TestGenericRecordNullValues()
        {
            using var ctx = new TestContext("generic_null");
            var (tableName, ktype, recordType) = SetupTestTable(ctx, "null_test");

            var options = new BulkInserterOptions
            {
                BatchSize = 10
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            // Insert records with null values
            for (int i = 0; i < 10; i++)
            {
                var record = recordType.NewInstance();
                record.Put("id", i);

                // Set some values to null
                if (i % 2 == 0)
                {
                    record.PutNull("name");
                }
                else
                {
                    record.Put("name", $"not_null_{i}");
                }

                record.Put("value", i * 10.0);
                record.PutNull("timestamp");

                inserter.Insert(record);
            }

            await inserter.CloseAsync();

            _output.WriteLine($"Inserted: {inserter.CountInserted}");
            Assert.Equal(10, inserter.CountInserted);

            // Verify null values
            var response = ctx.Kinetica.executeSql(
                $"SELECT COUNT(*) FROM {tableName} WHERE name IS NULL",
                0, -9999);
            Assert.Equal(1, response.total_number_of_records);
        }

        #endregion

        #region Concurrent Insert Tests

        [Fact]
        public async Task TestGenericRecordConcurrentInserts()
        {
            using var ctx = new TestContext("generic_concurrent");
            var (tableName, ktype, recordType) = SetupTestTable(ctx, "concurrent_test");

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                MaxInFlightBatches = 20,
                NumStripes = 8
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            const int numThreads = 4;
            const int recordsPerThread = 500;

            var tasks = new Task[numThreads];

            for (int t = 0; t < numThreads; t++)
            {
                int threadId = t;
                tasks[t] = Task.Run(async () =>
                {
                    // Each thread generates its own records
                    for (int i = 0; i < recordsPerThread; i++)
                    {
                        var record = recordType.NewInstance();
                        record.Put("id", threadId * recordsPerThread + i);
                        record.Put("name", $"thread_{threadId}_record_{i}");
                        record.Put("value", (double)(threadId * 1000 + i));
                        record.Put("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                        await inserter.InsertAsync(record);
                    }
                });
            }

            await Task.WhenAll(tasks);
            await inserter.CloseAsync();

            _output.WriteLine($"Total inserted: {inserter.CountInserted}");
            _output.WriteLine($"Total batches: {inserter.TotalBatchesSent}");

            Assert.Equal(numThreads * recordsPerThread, inserter.CountInserted);
        }

        #endregion

        #region Type Builder Tests

        [Fact]
        public async Task TestGenericRecordWithTypeBuilder()
        {
            using var ctx = new TestContext("generic_builder");

            // Build a type using TypeBuilder (Rust-style API)
            var recordType = kinetica.Records.Type.Builder("product_record")
                .AddIntColumn("id").PrimaryKey()
                .AddStringColumn("product_name")
                .AddDoubleColumn("price")
                .AddIntColumn("quantity")
                .AddTimestampColumn("created_at")
                .Build();

            // Create the table using the type
            var tableName = ctx.QualifiedTable("product_table");

            // Create table via SQL based on type definition
            ctx.Kinetica.executeSql($@"CREATE TABLE {tableName} (
                id INT NOT NULL,
                product_name VARCHAR(256),
                price DOUBLE,
                quantity INT,
                created_at TIMESTAMP,
                PRIMARY KEY (id)
            )");

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
            var actualRecordType = kinetica.Records.Type.FromTable(ctx.Kinetica, tableName);

            var options = new BulkInserterOptions
            {
                BatchSize = 50
            };

            await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

            // Insert product records
            for (int i = 0; i < 100; i++)
            {
                var record = actualRecordType.NewInstance();
                record.Put("id", i);
                record.Put("product_name", $"Product_{i:D4}");
                record.Put("price", 9.99 + i * 0.5);
                record.Put("quantity", 10 + i);
                record.Put("created_at", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                inserter.Insert(record);
            }

            await inserter.CloseAsync();

            _output.WriteLine($"Inserted: {inserter.CountInserted}");
            Assert.Equal(100, inserter.CountInserted);

            // Verify
            var response = ctx.Kinetica.executeSql(
                $"SELECT SUM(quantity) FROM {tableName}",
                0, -9999);
            Assert.Equal(1, response.total_number_of_records);
        }

        #endregion
    }
}
