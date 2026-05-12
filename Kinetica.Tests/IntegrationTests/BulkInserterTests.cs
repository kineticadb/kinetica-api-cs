using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Kinetica.Tests.Common;
using kinetica;
using kinetica.Records;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the high-performance BulkInserter.
    /// These tests verify correct functionality with various batch sizes,
    /// concurrency patterns, and error handling scenarios.
    /// </summary>
    [Trait("Category", "Integration")]
    public class BulkInserterTests
    {
        private readonly ITestOutputHelper _output;

        public BulkInserterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Test Record Type

        public class TestRecord : IShardKeyExtractor
        {
            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public double value { get; set; }
            public long timestamp { get; set; }

            public ShardKeyValues GetShardKeyValues()
            {
                // No shard key for this test record, use id as routing hint
                return new ShardKeyValues();
            }
        }

        public class ShardedRecord : IShardKeyExtractor
        {
            public int id { get; set; }
            public string shard_key { get; set; } = string.Empty;
            public double value { get; set; }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues(("shard_key", ShardKeyValue.String(shard_key)));
            }
        }

        #endregion

        #region Helper Methods

        private (string tableName, KineticaType ktype) SetupTestTable(TestContext ctx, bool withShardKey = false)
        {
            string typeDef;
            Dictionary<string, IList<string>> properties;

            if (withShardKey)
            {
                // Shard key columns must be part of the primary key
                typeDef = @"{""type"":""record"",""name"":""sharded_record"",""fields"":[" +
                    @"{""name"":""id"",""type"":""int""}," +
                    @"{""name"":""shard_key"",""type"":""string""}," +
                    @"{""name"":""value"",""type"":[""double"",""null""]}]}";

                properties = new Dictionary<string, IList<string>>
                {
                    { "id", new List<string> { "primary_key" } },
                    { "shard_key", new List<string> { "primary_key", "shard_key" } },
                    { "value", new List<string> { "nullable" } }
                };
            }
            else
            {
                typeDef = @"{""type"":""record"",""name"":""test_record"",""fields"":[" +
                    @"{""name"":""id"",""type"":""int""}," +
                    @"{""name"":""name"",""type"":[""string"",""null""]}," +
                    @"{""name"":""value"",""type"":[""double"",""null""]}," +
                    @"{""name"":""timestamp"",""type"":[""long"",""null""]}]}";

                properties = new Dictionary<string, IList<string>>
                {
                    { "id", new List<string> { "primary_key" } },
                    { "name", new List<string> { "nullable" } },
                    { "value", new List<string> { "nullable" } },
                    { "timestamp", new List<string> { "nullable" } }
                };
            }

            var typeResp = ctx.Kinetica.createType(typeDef, "bulk_test_type", properties, new Dictionary<string, string>());
            var tableName = ctx.QualifiedTable("bulk_test");
            ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
            return (tableName, ktype);
        }

        private List<TestRecord> GenerateTestRecords(int count, int startId = 0)
        {
            var records = new List<TestRecord>(count);
            var baseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (int i = 0; i < count; i++)
            {
                records.Add(new TestRecord
                {
                    id = startId + i,
                    name = $"record_{startId + i:D8}",
                    value = (startId + i) * 1.5,
                    timestamp = baseTimestamp + i
                });
            }

            return records;
        }

        private List<ShardedRecord> GenerateShardedRecords(int count, int startId = 0)
        {
            var records = new List<ShardedRecord>(count);

            for (int i = 0; i < count; i++)
            {
                records.Add(new ShardedRecord
                {
                    id = startId + i,
                    shard_key = $"shard_{(startId + i) % 100:D3}",
                    value = (startId + i) * 2.5
                });
            }

            return records;
        }

        #endregion

        #region Basic Insert Tests

        [Fact]
        public async Task TestBulkInserterBasicInsert()
        {
            using var ctx = new TestContext("bulk_basic");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(500);

            foreach (var record in records)
            {
                inserter.Insert(record);
            }

            await inserter.CloseAsync();

            Assert.Equal(500, inserter.CountInserted);

            // Verify in database - total_number_of_records is the rows returned, not the count value
            var response = ctx.Kinetica.executeSql($"SELECT COUNT(*) AS cnt FROM {tableName}", 0, -9999);
            Assert.Equal(1, response.total_number_of_records);  // One row returned with the count
        }

        [Fact]
        public async Task TestBulkInserterBatchInsert()
        {
            using var ctx = new TestContext("bulk_batch");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 1000
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(5000);

            // Insert all at once using batch method
            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            _output.WriteLine($"Inserted: {inserter.CountInserted}, Updated: {inserter.CountUpdated}");
            _output.WriteLine($"Total batches sent: {inserter.TotalBatchesSent}");

            Assert.Equal(5000, inserter.CountInserted);
        }

        [Fact]
        public async Task TestBulkInserterAsyncInsert()
        {
            using var ctx = new TestContext("bulk_async");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                MaxInFlightBatches = 10
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(1000);

            // Insert with backpressure
            foreach (var record in records)
            {
                await inserter.InsertAsync(record);
            }

            await inserter.CloseAsync();

            Assert.Equal(1000, inserter.CountInserted);
        }

        [Fact]
        public async Task TestBulkInserterAsyncBatchInsert()
        {
            using var ctx = new TestContext("bulk_async_batch");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 500,
                MaxInFlightBatches = 5
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(2500);

            await inserter.InsertBatchAsync(records);

            await inserter.CloseAsync();

            Assert.Equal(2500, inserter.CountInserted);
        }

        #endregion

        #region Shard Key Tests

        [Fact]
        public async Task TestBulkInserterWithShardKey()
        {
            using var ctx = new TestContext("bulk_shard");
            var (tableName, ktype) = SetupTestTable(ctx, withShardKey: true);

            var options = new BulkInserterOptions
            {
                BatchSize = 100
            };

            await using var inserter = new BulkInserter<ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateShardedRecords(1000);

            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            Assert.Equal(1000, inserter.CountInserted);

            // Verify shard distribution (if multi-head is enabled)
            _output.WriteLine($"Workers: {inserter.NumWorkers}");
            _output.WriteLine($"Batches sent: {inserter.TotalBatchesSent}");
        }

        #endregion

        #region Batch Listener Tests

        [Fact]
        public async Task TestBulkInserterWithListener()
        {
            using var ctx = new TestContext("bulk_listener");
            var (tableName, ktype) = SetupTestTable(ctx);

            var listener = new TestBatchListener();

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                BatchListener = listener
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(350);
            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            _output.WriteLine($"Batches completed: {listener.BatchesCompleted}");
            _output.WriteLine($"Total inserted via listener: {listener.TotalInserted}");
            _output.WriteLine($"Avg encode time: {listener.AverageEncodeTimeMs:F2}ms");
            _output.WriteLine($"Avg network time: {listener.AverageNetworkTimeMs:F2}ms");

            Assert.True(listener.BatchesCompleted >= 3, $"Expected at least 3 batches, got {listener.BatchesCompleted}");
            Assert.Equal(350, listener.TotalInserted);
            Assert.True(listener.AllSucceeded);
        }

        private class TestBatchListener : IBatchInsertionListener
        {
            private int _batchesCompleted;
            private long _totalInserted;
            private double _totalEncodeTime;
            private double _totalNetworkTime;
            private bool _allSucceeded = true;

            public int BatchesCompleted => _batchesCompleted;
            public long TotalInserted => _totalInserted;
            public double AverageEncodeTimeMs => _batchesCompleted > 0 ? _totalEncodeTime / _batchesCompleted : 0;
            public double AverageNetworkTimeMs => _batchesCompleted > 0 ? _totalNetworkTime / _batchesCompleted : 0;
            public bool AllSucceeded => _allSucceeded;

            public void OnBatchInserted(BatchInsertionResult result)
            {
                Interlocked.Increment(ref _batchesCompleted);
                Interlocked.Add(ref _totalInserted, result.CountInserted);

                // Thread-safe accumulation
                lock (this)
                {
                    _totalEncodeTime += result.EncodeTimeMs;
                    _totalNetworkTime += result.NetworkTimeMs;
                }

                if (!result.Success)
                    _allSucceeded = false;
            }
        }

        #endregion

        #region Backpressure Tests

        [Fact]
        public async Task TestBulkInserterBackpressure()
        {
            using var ctx = new TestContext("bulk_backpressure");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 50,
                MaxInFlightBatches = 5
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(500);

            // Monitor backpressure during insert
            var maxUtilization = 0.0;

            foreach (var record in records)
            {
                await inserter.InsertAsync(record);

                var metrics = inserter.GetBackpressureMetrics();
                if (metrics.UtilizationPercent > maxUtilization)
                    maxUtilization = metrics.UtilizationPercent;
            }

            await inserter.CloseAsync();

            _output.WriteLine($"Max backpressure utilization: {maxUtilization:F1}%");
            _output.WriteLine($"Final metrics: {inserter.GetBackpressureMetrics().InFlightBatches} in-flight");

            Assert.Equal(500, inserter.CountInserted);
        }

        #endregion

        #region Concurrent Insert Tests

        [Fact]
        public async Task TestBulkInserterConcurrentInserts()
        {
            using var ctx = new TestContext("bulk_concurrent");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                MaxInFlightBatches = 20,
                NumStripes = 8
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            const int numThreads = 4;
            const int recordsPerThread = 500;

            var tasks = new Task[numThreads];

            for (int t = 0; t < numThreads; t++)
            {
                int threadId = t;
                tasks[t] = Task.Run(async () =>
                {
                    var records = GenerateTestRecords(recordsPerThread, threadId * recordsPerThread);
                    foreach (var record in records)
                    {
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

        #region Flush Tests

        [Fact]
        public async Task TestBulkInserterManualFlush()
        {
            using var ctx = new TestContext("bulk_flush");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 1000 // Large batch size to prevent auto-flush
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            // Insert fewer records than batch size
            var records = GenerateTestRecords(250);
            foreach (var record in records)
            {
                inserter.Insert(record);
            }

            // Records should still be pending
            Assert.Equal(0, inserter.CountInserted);

            // Manual flush
            await inserter.FlushAsync();

            // Wait for flush to complete
            await inserter.CloseAsync();

            Assert.Equal(250, inserter.CountInserted);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task TestBulkInserterErrorQueue()
        {
            using var ctx = new TestContext("bulk_errors");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100,
                MaxRetries = 0, // No retries for this test
                MaxErrorQueueSize = 100
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            // Insert valid records
            var records = GenerateTestRecords(200);
            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            var errors = inserter.DrainErrors();
            _output.WriteLine($"Errors: {errors.Count}");

            // With valid data, we expect no errors
            Assert.Empty(errors);
            Assert.Equal(200, inserter.CountInserted);
        }

        #endregion

        #region Metrics Tests

        [Fact]
        public async Task TestBulkInserterMetrics()
        {
            using var ctx = new TestContext("bulk_metrics");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 100
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            var records = GenerateTestRecords(500);
            inserter.InsertBatch(records);

            await inserter.CloseAsync();

            _output.WriteLine($"Count Inserted: {inserter.CountInserted}");
            _output.WriteLine($"Count Updated: {inserter.CountUpdated}");
            _output.WriteLine($"Total Batches Sent: {inserter.TotalBatchesSent}");
            _output.WriteLine($"Total Batches Failed: {inserter.TotalBatchesFailed}");
            _output.WriteLine($"Pending Batches: {inserter.PendingBatches}");
            _output.WriteLine($"Error Count: {inserter.ErrorCount}");

            Assert.Equal(500, inserter.CountInserted);
            Assert.Equal(0, inserter.CountUpdated);
            Assert.True(inserter.TotalBatchesSent >= 5);
            Assert.Equal(0, inserter.TotalBatchesFailed);
            Assert.Equal(0, inserter.PendingBatches);
            Assert.Equal(0, inserter.ErrorCount);
        }

        #endregion

        #region Large Batch Tests

        [Fact]
        public async Task TestBulkInserterLargeBatch()
        {
            using var ctx = new TestContext("bulk_large");
            var (tableName, ktype) = SetupTestTable(ctx);

            var options = new BulkInserterOptions
            {
                BatchSize = 10000,
                MaxInFlightBatches = 10
            };

            await using var inserter = new BulkInserter<TestRecord>(ctx.Kinetica, tableName, ktype, options);

            const int totalRecords = 50000;
            var records = GenerateTestRecords(totalRecords);

            var sw = Stopwatch.StartNew();
            inserter.InsertBatch(records);
            await inserter.CloseAsync();
            sw.Stop();

            var recordsPerSecond = totalRecords / sw.Elapsed.TotalSeconds;

            _output.WriteLine($"Inserted {totalRecords} records in {sw.Elapsed.TotalSeconds:F2}s");
            _output.WriteLine($"Throughput: {recordsPerSecond:F0} records/second");
            _output.WriteLine($"Total batches: {inserter.TotalBatchesSent}");

            Assert.Equal(totalRecords, inserter.CountInserted);
        }

        #endregion
    }
}
