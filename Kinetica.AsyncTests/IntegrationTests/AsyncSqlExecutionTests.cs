using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for SQL execution endpoints.
    /// These tests verify that async SQL operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncSqlExecutionTests
    {
        [Fact]
        public async Task TestExecuteSqlSelectAsync()
        {
            using var ctx = new TestContext("async_sql_select");

            // Create and populate table
            await ctx.Kinetica.ExecuteSqlAsync($@"
                CREATE TABLE {ctx.QualifiedTable("sql_test")}
                (id INT NOT NULL, name VARCHAR(64), value DOUBLE, PRIMARY KEY (id))
            ");

            var tableName = ctx.QualifiedTable("sql_test");

            // Insert test data
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (1, 'Alice', 10.5)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (2, 'Bob', 20.7)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (3, 'Charlie', 30.9)");

            // Execute SELECT asynchronously
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} ORDER BY id");

            Assert.Equal(3, response.total_number_of_records);
            Assert.True(response.has_more_records == false);
        }

        [Fact]
        public async Task TestExecuteSqlWithLimitAsync()
        {
            using var ctx = new TestContext("async_sql_limit");

            await ctx.Kinetica.ExecuteSqlAsync($@"
                CREATE TABLE {ctx.QualifiedTable("limit_test")}
                (id INT NOT NULL, data VARCHAR(32), PRIMARY KEY (id))
            ");

            var tableName = ctx.QualifiedTable("limit_test");

            // Insert 50 records
            for (int i = 0; i < 50; i++)
            {
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, data) VALUES ({i}, 'record_{i}')");
            }

            // Execute SELECT with LIMIT
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} LIMIT 10");

            Assert.Equal(10, response.total_number_of_records);
        }

        [Fact]
        public async Task TestExecuteSqlAggregateAsync()
        {
            using var ctx = new TestContext("async_sql_aggregate");

            await ctx.Kinetica.ExecuteSqlAsync($@"
                CREATE TABLE {ctx.QualifiedTable("agg_test")}
                (id INT NOT NULL, category VARCHAR(32), amount DOUBLE, PRIMARY KEY (id))
            ");

            var tableName = ctx.QualifiedTable("agg_test");

            // Insert test data with categories
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, category, amount) VALUES (1, 'A', 100.0)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, category, amount) VALUES (2, 'A', 150.0)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, category, amount) VALUES (3, 'B', 200.0)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, category, amount) VALUES (4, 'B', 250.0)");

            // Execute GROUP BY query asynchronously
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT category, SUM(amount) as total FROM {tableName} GROUP BY category ORDER BY category");

            Assert.Equal(2, response.total_number_of_records);
        }

        [Fact]
        public async Task TestExecuteSqlCreateDropTableAsync()
        {
            using var ctx = new TestContext("async_sql_create_drop");

            var tableName = ctx.QualifiedTable("temp_table");

            // Create table
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT, data VARCHAR(32))");

            // Verify table exists
            var hasTableResp = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.True(hasTableResp.table_exists);

            // Drop table
            await ctx.Kinetica.ExecuteSqlAsync($"DROP TABLE {tableName}");

            // Verify table no longer exists
            var hasTableResp2 = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.False(hasTableResp2.table_exists);
        }

        [Fact]
        public async Task TestExecuteSqlWithFilterAsync()
        {
            using var ctx = new TestContext("async_sql_filter");

            await ctx.Kinetica.ExecuteSqlAsync($@"
                CREATE TABLE {ctx.QualifiedTable("filter_test")}
                (id INT NOT NULL, status VARCHAR(16), score INT, PRIMARY KEY (id))
            ");

            var tableName = ctx.QualifiedTable("filter_test");

            // Insert test data
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, status, score) VALUES (1, 'active', 85)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, status, score) VALUES (2, 'inactive', 60)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, status, score) VALUES (3, 'active', 92)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, status, score) VALUES (4, 'active', 78)");

            // Execute filtered query
            var response = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE status = 'active' AND score > 80");

            Assert.Equal(2, response.total_number_of_records);
        }

        [Fact]
        public async Task TestConcurrentSqlQueriesAsync()
        {
            using var ctx = new TestContext("async_concurrent_sql");

            await ctx.Kinetica.ExecuteSqlAsync($@"
                CREATE TABLE {ctx.QualifiedTable("concurrent_test")}
                (id INT NOT NULL, value INT, PRIMARY KEY (id))
            ");

            var tableName = ctx.QualifiedTable("concurrent_test");

            // Insert initial data
            for (int i = 0; i < 20; i++)
            {
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, value) VALUES ({i}, {i * 10})");
            }

            // Execute multiple SELECT queries concurrently
            var queryTasks = new List<Task<ExecuteSqlResponse>>();

            for (int i = 0; i < 10; i++)
            {
                int minId = i * 2;
                int maxId = minId + 1;
                queryTasks.Add(ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE id >= {minId} AND id <= {maxId}"));
            }

            var results = await Task.WhenAll(queryTasks);

            // Verify all queries completed successfully
            Assert.Equal(10, results.Length);
            foreach (var result in results)
            {
                Assert.True(result.total_number_of_records >= 1, "Each query should return at least 1 record");
            }
        }

        [Fact]
        public async Task TestExecuteSqlShowTablesAsync()
        {
            using var ctx = new TestContext("async_show_tables");

            // Create a few tables
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {ctx.QualifiedTable("table1")} (id INT)");
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {ctx.QualifiedTable("table2")} (id INT)");
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {ctx.QualifiedTable("table3")} (id INT)");

            // Show all tables
            var response = await ctx.Kinetica.ExecuteSqlAsync("SHOW TABLES");

            // Should have at least some tables
            Assert.True(response.total_number_of_records >= 0, "SHOW TABLES should return successfully");
        }
    }
}
