using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for table management endpoints.
    /// These tests verify that async table operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncTableManagementTests
    {
        [Fact]
        public async Task TestCreateTableBasicAsync()
        {
            using var ctx = new TestContext("async_table_basic");

            var typeDef = @"{""type"":""record"",""name"":""test"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "test_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("test_table");
            var resp = await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

            Assert.Equal(tableName, resp.table_name);
            Assert.Equal(typeResp.type_id, resp.type_id);
        }

        [Fact]
        public async Task TestCreateTableWithOptionsAsync()
        {
            using var ctx = new TestContext("async_table_opts");

            var typeDef = @"{""type"":""record"",""name"":""opts"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "opts_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("opts_table");
            var options = new Dictionary<string, string>
            {
                { "is_replicated", "true" }
            };

            var resp = await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, options);

            Assert.Equal(tableName, resp.table_name);
        }

        [Fact]
        public async Task TestHasTableExistsAsync()
        {
            using var ctx = new TestContext("async_has_table_y");

            var typeDef = @"{""type"":""record"",""name"":""exists"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "exists_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("exists_table");
            await ctx.Kinetica.CreateTableAsync(tableName, typeResp.type_id, new Dictionary<string, string>());

            var hasResp = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());

            Assert.True(hasResp.table_exists);
            Assert.Equal(tableName, hasResp.table_name);
        }

        [Fact]
        public async Task TestHasTableNotExistsAsync()
        {
            using var ctx = new TestContext("async_has_table_n");

            var tableName = ctx.QualifiedTable("nonexistent");
            var hasResp = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());

            Assert.False(hasResp.table_exists);
        }

        [Fact]
        public async Task TestClearTableAsync()
        {
            using var ctx = new TestContext("async_clear_table");

            // Create table using SQL (simpler approach)
            var tableName = ctx.QualifiedTable("clear_table");
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT NOT NULL, PRIMARY KEY (id))");

            // Insert records using SQL
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id) VALUES (1)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id) VALUES (2)");
            await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id) VALUES (3)");

            // Verify records exist
            var getBefore = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, -9999);
            Assert.Equal(3, getBefore.total_number_of_records);

            // Clear table
            var clearResp = await ctx.Kinetica.ClearTableAsync(tableName, null, new Dictionary<string, string>());
            Assert.Equal(tableName, clearResp.table_name);

            // Table should not exist after clear
            var hasAfter = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.False(hasAfter.table_exists);
        }

        [Fact]
        public async Task TestCreateMultipleTablesSameTypeAsync()
        {
            using var ctx = new TestContext("async_multi_tables");

            var typeDef = @"{""type"":""record"",""name"":""shared"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "shared_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            // Create multiple tables with same type
            var table1 = ctx.QualifiedTable("table1");
            var table2 = ctx.QualifiedTable("table2");
            var table3 = ctx.QualifiedTable("table3");

            await ctx.Kinetica.CreateTableAsync(table1, typeResp.type_id, new Dictionary<string, string>());
            await ctx.Kinetica.CreateTableAsync(table2, typeResp.type_id, new Dictionary<string, string>());
            await ctx.Kinetica.CreateTableAsync(table3, typeResp.type_id, new Dictionary<string, string>());

            // Verify all exist
            foreach (var table in new[] { table1, table2, table3 })
            {
                var hasResp = await ctx.Kinetica.HasTableAsync(table, new Dictionary<string, string>());
                Assert.True(hasResp.table_exists);
            }
        }
    }
}
