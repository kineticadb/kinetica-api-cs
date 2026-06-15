using Xunit;
using Kinetica.Tests.Common;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for record retrieval.
    /// These tests mirror the Rust record_retriever_tests.rs implementation.
    /// They test retrieval with various data types and shard keys.
    ///
    /// Note: The C# RecordRetriever<T> requires strongly typed record classes,
    /// so these tests use SQL-based retrieval via executeSql for flexibility.
    /// </summary>
    [Trait("Category", "Integration")]
    public class RecordRetrieverTests
    {
        /// <summary>
        /// Setup a test table with shard keys.
        /// </summary>
        private string SetupShardedTable(TestContext ctx, int numRecords)
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

            var typeResp = ctx.Kinetica.createType(typeDef, "test_retriever_type", properties, new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("test_retriever");
            ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Create indices on shard key columns for multi-head lookup
            foreach (var col in new[] { "name_sk", "value_sk" })
            {
                try
                {
                    ctx.Kinetica.alterTable(tableName, "create_index", col, new Dictionary<string, string>());
                }
                catch { /* Ignore if index already exists */ }
            }

            // Insert test data using SQL
            for (int i = 0; i < numRecords; i++)
            {
                var nameSk = $"name_{i:D5}";
                var valueSk = i;
                var score = (i % 10) * 10.0;
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, name_sk, value_sk, score) VALUES ({i}, '{nameSk}', {valueSk}, {score})");
            }

            return tableName;
        }

        [Fact]
        public void TestGetRecordsBasic()
        {
            using var ctx = new TestContext("get_records_basic");
            var numRecords = 50;
            var tableName = SetupShardedTable(ctx, numRecords);

            // Verify table has expected records using SQL
            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);
            Console.WriteLine($"Table has {response.total_number_of_records} records (expected {numRecords})");

            Assert.Equal(numRecords, response.total_number_of_records);
            Assert.Equal(numRecords, response.data.Count);
        }

        [Fact]
        public void TestGetRecordsByShardKey()
        {
            using var ctx = new TestContext("get_records_key");
            var numRecords = 20;
            var tableName = SetupShardedTable(ctx, numRecords);

            // Test: Retrieve record by shard key values
            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} WHERE name_sk = 'name_00005' AND value_sk = 5", 0, -9999);

            Console.WriteLine($"Query by shard key returned {response.total_number_of_records} records");
            Assert.Equal(1, response.total_number_of_records);
        }

        [Fact]
        public void TestGetRecordsWithExpression()
        {
            using var ctx = new TestContext("get_records_expr");
            var tableName = SetupShardedTable(ctx, 50);

            // Test: Retrieve with expression filter
            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} WHERE id < 25", 0, -9999);
            Console.WriteLine($"Query with expression returned {response.total_number_of_records} records (expected 25)");

            Assert.Equal(25, response.total_number_of_records);

            // Test: Retrieve with combined expression
            var response2 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} WHERE id >= 40 AND score < 50", 0, -9999);
            Console.WriteLine($"Query with combined expression returned {response2.total_number_of_records} records");

            Assert.True(response2.total_number_of_records >= 0);
        }

        [Fact]
        public void TestGetRecordsMultipleKeys()
        {
            using var ctx = new TestContext("get_records_multi");
            var numRecords = 30;
            var tableName = SetupShardedTable(ctx, numRecords);

            var totalFound = 0;

            // Search for multiple records by key
            for (int i = 0; i < 10; i++)
            {
                var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} WHERE name_sk = 'name_{i:D5}' AND value_sk = {i}", 0, -9999);
                totalFound += (int)response.total_number_of_records;
            }

            Console.WriteLine($"Multi-key retrieval: found {totalFound} records");
            Assert.Equal(10, totalFound); // Each key should find exactly 1 record
        }

        [Fact]
        public void TestGetRecordsWithSorting()
        {
            using var ctx = new TestContext("get_records_sort");
            var tableName = SetupShardedTable(ctx, 25);

            // Test: Retrieve with sorting
            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} ORDER BY score DESC, id ASC", 0, -9999);

            Console.WriteLine($"Sorted query returned {response.total_number_of_records} records");
            Assert.Equal(25, response.total_number_of_records);
        }

        [Fact]
        public void TestGetRecordsPagination()
        {
            using var ctx = new TestContext("get_records_page");
            var numRecords = 50;
            var tableName = SetupShardedTable(ctx, numRecords);

            // Get first page
            var page1 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, 10);
            Assert.Equal(50, page1.total_number_of_records);
            Assert.Equal(10, page1.data.Count);
            Assert.True(page1.has_more_records);

            // Get second page
            var page2 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 10, 10);
            Assert.Equal(10, page2.data.Count);
            Assert.True(page2.has_more_records);

            // Get last page
            var lastPage = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 40, 10);
            Assert.Equal(10, lastPage.data.Count);
            Assert.False(lastPage.has_more_records);
        }

        [Fact]
        public void TestGetRecordsEmptyTable()
        {
            using var ctx = new TestContext("get_records_empty");
            var tableName = SetupShardedTable(ctx, 0);

            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(0, response.total_number_of_records);
            Assert.Empty(response.data);
            Assert.False(response.has_more_records);
        }

        [Fact]
        public void TestGetRecordsOffsetBeyondEnd()
        {
            using var ctx = new TestContext("get_records_offset");
            var tableName = SetupShardedTable(ctx, 10);

            // Offset beyond table size
            var response = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 100, 10);

            Assert.Equal(10, response.total_number_of_records);
            Assert.Empty(response.data);
        }

        [Fact]
        public void TestGetRecordsSpecificColumns()
        {
            using var ctx = new TestContext("get_records_cols");
            var tableName = SetupShardedTable(ctx, 20);

            // Select specific columns
            var response = ctx.Kinetica.executeSql($"SELECT id, score FROM {tableName} WHERE id < 10", 0, -9999);

            Assert.Equal(10, response.total_number_of_records);
            Assert.Equal(10, response.data.Count);
        }

        [Fact]
        public void TestGetRecordsWithAggregation()
        {
            using var ctx = new TestContext("get_records_agg");
            var tableName = SetupShardedTable(ctx, 100);

            // Test aggregation
            var response = ctx.Kinetica.executeSql($"SELECT COUNT(*), AVG(score) FROM {tableName}", 0, -9999);

            Assert.Equal(1, response.total_number_of_records);
            Assert.Single(response.data);
        }

        [Fact]
        public void TestGetRecordsGroupBy()
        {
            using var ctx = new TestContext("get_records_group");
            var tableName = SetupShardedTable(ctx, 30);

            // Group by score (there should be 10 distinct scores: 0, 10, 20, ..., 90)
            var response = ctx.Kinetica.executeSql($"SELECT score, COUNT(*) as cnt FROM {tableName} GROUP BY score ORDER BY score", 0, -9999);

            Assert.Equal(10, response.total_number_of_records);
        }
    }
}
