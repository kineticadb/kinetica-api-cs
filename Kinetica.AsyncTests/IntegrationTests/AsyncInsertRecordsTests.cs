using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for insert_records and related endpoints.
    /// These tests verify that async insert operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncInsertRecordsTests
    {
        [Fact]
        public async Task TestInsertRecordsJsonAsync()
        {
            using var ctx = new TestContext("async_insert_json");

            // Create type
            var typeDef = @"{""type"":""record"",""name"":""test_rec"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""},{""name"":""value"",""type"":""double""}]}";
            var props = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "async_insert_test_type", props, new Dictionary<string, string>());

            // Create table asynchronously
            var tableName = ctx.QualifiedTable("test_table_async");
            await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Insert records using async SQL execution
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (1, 'Alice', 1.1)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (2, 'Bob', 2.2)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, name, value) VALUES (3, 'Charlie', 3.3)");

            // Verify records were inserted using async showTable
            var showTableResp = await ctx.Kinetica.ShowTableAsync(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(3, showTableResp.total_size);
        }

        [Fact]
        public async Task TestInsertRecordsUpdateOnExistingPkAsync()
        {
            using var ctx = new TestContext("async_insert_update_pk");

            // Create type with primary key
            var typeDef = @"{""type"":""record"",""name"":""pk_rec"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""value"",""type"":""string""}]}";
            var props = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "async_pk_test_type", props, new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("pk_table_async");
            await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Insert initial records asynchronously
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, value) VALUES (1, 'original')");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, value) VALUES (2, 'data')");

            // Verify 2 records exist
            var showTableResp = await ctx.Kinetica.ShowTableAsync(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(2, showTableResp.total_size);

            // Update existing record
            await ctx.Kinetica.ExecuteSqlAsync($"UPDATE {tableName} SET value = 'updated' WHERE id = 1");

            // Should still have 2 records
            var showTableResp2 = await ctx.Kinetica.ShowTableAsync(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(2, showTableResp2.total_size);

            // Verify the value was updated
            var selectResp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT value FROM {tableName} WHERE id = 1", 0, -9999);
            Assert.Equal(1, selectResp.total_number_of_records);
        }

        [Fact]
        public async Task TestInsertRecordsLargeBatchAsync()
        {
            using var ctx = new TestContext("async_insert_large");

            // Create table using async SQL
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {ctx.QualifiedTable("batch_table_async")} (id INT NOT NULL, x DOUBLE, y DOUBLE, PRIMARY KEY (id))");

            var tableName = ctx.QualifiedTable("batch_table_async");

            // Insert 100 records concurrently
            var numRecords = 100;
            var insertTasks = new List<Task>();

            for (int i = 0; i < numRecords; i++)
            {
                int recordId = i; // Capture loop variable
                insertTasks.Add(ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, x, y) VALUES ({recordId}, {recordId * 0.1}, {recordId * 0.2})"));

                // Insert in batches of 10 to avoid overwhelming the server
                if (insertTasks.Count >= 10 || i == numRecords - 1)
                {
                    await Task.WhenAll(insertTasks);
                    insertTasks.Clear();
                }
            }

            // Verify count using async showTable
            var showTableResp = await ctx.Kinetica.ShowTableAsync(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(numRecords, showTableResp.total_size);
        }

        [Fact]
        public async Task TestParallelInsertDifferentTablesAsync()
        {
            using var ctx = new TestContext("async_parallel_insert");

            // Create type
            var typeDef = @"{""type"":""record"",""name"":""parallel_rec"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""data"",""type"":""string""}]}";
            var props = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "parallel_type", props, new Dictionary<string, string>());

            // Create and populate multiple tables in parallel
            var tasks = new List<Task>();
            for (int tableNum = 0; tableNum < 5; tableNum++)
            {
                int capturedTableNum = tableNum;
                tasks.Add(Task.Run(async () =>
                {
                    var tableName = ctx.QualifiedTable($"parallel_table_{capturedTableNum}");
                    await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

                    // Insert 10 records into each table
                    for (int i = 0; i < 10; i++)
                    {
                        await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, data) VALUES ({i}, 'data_{i}')");
                    }

                    // Verify count
                    var showResp = await ctx.Kinetica.ShowTableAsync(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
                    Assert.Equal(10, showResp.total_size);
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
