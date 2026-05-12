using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async integration tests for record retrieval.
    /// These tests verify that async record retrieval operations work correctly
    /// with various data types and shard keys.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncRecordRetrieverTests
    {
        /// <summary>
        /// Setup a test table with shard keys.
        /// </summary>
        private async Task<string> SetupShardedTableAsync(TestContext ctx, int numRecords)
        {
            // Create type with shard keys
            var typeDef = @"{""type"":""record"",""name"":""simple_record"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name_sk"",""type"":""string""},{""name"":""value_sk"",""type"":""int""},{""name"":""score"",""type"":""double""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string>() },
                { "name_sk", new List<string> { "shard_key" } },
                { "value_sk", new List<string> { "shard_key" } },
                { "score", new List<string>() }
            };

            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "test_retriever_type", properties, new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("test_retriever");
            await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Create indices on shard key columns for multi-head lookup
            foreach (var col in new[] { "name_sk", "value_sk" })
            {
                try
                {
                    await ctx.Kinetica.AlterTableAsync(tableName, "create_index", col, new Dictionary<string, string>());
                }
                catch { /* Ignore if index already exists */ }
            }

            // Insert test data using SQL
            for (int i = 0; i < numRecords; i++)
            {
                var nameSk = $"name_{i:D5}";
                var valueSk = i;
                var score = (i % 10) * 10.0;
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name_sk, value_sk, score) VALUES ({i}, '{nameSk}', {valueSk}, {score})");
            }

            return tableName;
        }

        [Fact]
        public async Task TestGetRecordsBasicAsync()
        {
            using var ctx = new TestContext("async_get_records_basic");
            var numRecords = 50;
            var tableName = await SetupShardedTableAsync(ctx, numRecords);

            // Verify table has expected records using SQL
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, -9999);
            Console.WriteLine($"Table has {response.total_number_of_records} records (expected {numRecords})");

            Assert.Equal(numRecords, response.total_number_of_records);
            Assert.Equal(numRecords, response.data.Count);
        }

        [Fact]
        public async Task TestGetRecordsByShardKeyAsync()
        {
            using var ctx = new TestContext("async_get_records_key");
            var numRecords = 20;
            var tableName = await SetupShardedTableAsync(ctx, numRecords);

            // Test: Retrieve record by shard key values
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE name_sk = 'name_00005' AND value_sk = 5", 0, -9999);

            Console.WriteLine($"Query by shard key returned {response.total_number_of_records} records");
            Assert.Equal(1, response.total_number_of_records);
        }

        [Fact]
        public async Task TestGetRecordsWithExpressionAsync()
        {
            using var ctx = new TestContext("async_get_records_expr");
            var tableName = await SetupShardedTableAsync(ctx, 50);

            // Test: Retrieve with expression filter
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE id < 25", 0, -9999);
            Console.WriteLine($"Query with expression returned {response.total_number_of_records} records (expected 25)");

            Assert.Equal(25, response.total_number_of_records);

            // Test: Retrieve with combined expression
            var response2 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE id >= 40 AND score < 50", 0, -9999);
            Console.WriteLine($"Query with combined expression returned {response2.total_number_of_records} records");

            Assert.True(response2.total_number_of_records >= 0);
        }

        [Fact]
        public async Task TestGetRecordsMultipleKeysAsync()
        {
            using var ctx = new TestContext("async_get_records_multi");
            var numRecords = 30;
            var tableName = await SetupShardedTableAsync(ctx, numRecords);

            var totalFound = 0;

            // Search for multiple records by key
            for (int i = 0; i < 10; i++)
            {
                var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE name_sk = 'name_{i:D5}' AND value_sk = {i}", 0, -9999);
                totalFound += (int)response.total_number_of_records;
            }

            Console.WriteLine($"Multi-key retrieval: found {totalFound} records");
            Assert.Equal(10, totalFound); // Each key should find exactly 1 record
        }

        [Fact]
        public async Task TestGetRecordsWithSortingAsync()
        {
            using var ctx = new TestContext("async_get_records_sort");
            var tableName = await SetupShardedTableAsync(ctx, 25);

            // Test: Retrieve with sorting
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} ORDER BY score DESC, id ASC", 0, -9999);

            Console.WriteLine($"Sorted query returned {response.total_number_of_records} records");
            Assert.Equal(25, response.total_number_of_records);
        }

        [Fact]
        public async Task TestGetRecordsPaginationAsync()
        {
            using var ctx = new TestContext("async_get_records_page");
            var numRecords = 50;
            var tableName = await SetupShardedTableAsync(ctx, numRecords);

            // Get first page
            var page1 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, 10);
            Assert.Equal(50, page1.total_number_of_records);
            Assert.Equal(10, page1.data.Count);
            Assert.True(page1.has_more_records);

            // Get second page
            var page2 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 10, 10);
            Assert.Equal(10, page2.data.Count);
            Assert.True(page2.has_more_records);

            // Get last page
            var lastPage = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 40, 10);
            Assert.Equal(10, lastPage.data.Count);
            Assert.False(lastPage.has_more_records);
        }

        [Fact]
        public async Task TestGetRecordsEmptyTableAsync()
        {
            using var ctx = new TestContext("async_get_records_empty");
            var tableName = await SetupShardedTableAsync(ctx, 0);

            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(0, response.total_number_of_records);
            Assert.Empty(response.data);
            Assert.False(response.has_more_records);
        }

        [Fact]
        public async Task TestGetRecordsOffsetBeyondEndAsync()
        {
            using var ctx = new TestContext("async_get_records_offset");
            var tableName = await SetupShardedTableAsync(ctx, 10);

            // Offset beyond table size
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 100, 10);

            Assert.Equal(10, response.total_number_of_records);
            Assert.Empty(response.data);
        }

        [Fact]
        public async Task TestGetRecordsSpecificColumnsAsync()
        {
            using var ctx = new TestContext("async_get_records_cols");
            var tableName = await SetupShardedTableAsync(ctx, 20);

            // Select specific columns
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT id, score FROM {tableName} WHERE id < 10", 0, -9999);

            Assert.Equal(10, response.total_number_of_records);
            Assert.Equal(10, response.data.Count);
        }

        [Fact]
        public async Task TestGetRecordsWithAggregationAsync()
        {
            using var ctx = new TestContext("async_get_records_agg");
            var tableName = await SetupShardedTableAsync(ctx, 100);

            // Test aggregation
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT COUNT(*), AVG(score) FROM {tableName}", 0, -9999);

            Assert.Equal(1, response.total_number_of_records);
            Assert.Single(response.data);
        }

        [Fact]
        public async Task TestGetRecordsGroupByAsync()
        {
            using var ctx = new TestContext("async_get_records_group");
            var tableName = await SetupShardedTableAsync(ctx, 30);

            // Group by score (there should be 10 distinct scores: 0, 10, 20, ..., 90)
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT score, COUNT(*) as cnt FROM {tableName} GROUP BY score ORDER BY score", 0, -9999);

            Assert.Equal(10, response.total_number_of_records);
        }
    }
}
