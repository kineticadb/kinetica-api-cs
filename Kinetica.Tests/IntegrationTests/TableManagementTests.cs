using System;
using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for table management endpoints.
    /// These tests mirror the Rust table_management_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class TableManagementTests
    {
        [Fact]
        public void TestCreateTableBasic()
        {
            using var ctx = new TestContext("table_basic");

            var typeDef = @"{""type"":""record"",""name"":""test"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = ctx.Kinetica.createType(typeDef, "test_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("test_table");
            var resp = ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            Assert.Equal(tableName, resp.table_name);
            Assert.Equal(typeResp.type_id, resp.type_id);
        }

        [Fact]
        public void TestCreateTableWithOptions()
        {
            using var ctx = new TestContext("table_opts");

            var typeDef = @"{""type"":""record"",""name"":""opts"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = ctx.Kinetica.createType(typeDef, "opts_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("opts_table");
            var options = new Dictionary<string, string>
            {
                { "is_replicated", "true" }
            };

            var resp = ctx.Kinetica.createTable(tableName, typeResp.type_id, options);

            Assert.Equal(tableName, resp.table_name);
        }

        [Fact]
        public void TestHasTableExists()
        {
            using var ctx = new TestContext("has_table_y");

            var typeDef = @"{""type"":""record"",""name"":""exists"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = ctx.Kinetica.createType(typeDef, "exists_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("exists_table");
            ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            var hasResp = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());

            Assert.True(hasResp.table_exists);
            Assert.Equal(tableName, hasResp.table_name);
        }

        [Fact]
        public void TestHasTableNotExists()
        {
            using var ctx = new TestContext("has_table_n");

            var tableName = ctx.QualifiedTable("nonexistent");
            var hasResp = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());

            Assert.False(hasResp.table_exists);
        }

        [Fact]
        public void TestClearTable()
        {
            using var ctx = new TestContext("clear_table");

            // Create table using SQL (simpler approach)
            var tableName = ctx.QualifiedTable("clear_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, PRIMARY KEY (id))");

            // Insert records using SQL
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id) VALUES (1)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id) VALUES (2)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id) VALUES (3)");

            // Verify records exist
            var getBefore = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);
            Assert.Equal(3, getBefore.total_number_of_records);

            // Clear table
            var clearResp = ctx.Kinetica.clearTable(tableName, null, new Dictionary<string, string>());
            Assert.Equal(tableName, clearResp.table_name);

            // Table should not exist after clear
            var hasAfter = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.False(hasAfter.table_exists);
        }

        [Fact]
        public void TestCreateMultipleTablesSameType()
        {
            using var ctx = new TestContext("multi_tables");

            var typeDef = @"{""type"":""record"",""name"":""shared"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var typeResp = ctx.Kinetica.createType(typeDef, "shared_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            // Create multiple tables with same type
            var table1 = ctx.QualifiedTable("table1");
            var table2 = ctx.QualifiedTable("table2");
            var table3 = ctx.QualifiedTable("table3");

            ctx.Kinetica.createTable(table1, typeResp.type_id, new Dictionary<string, string>());
            ctx.Kinetica.createTable(table2, typeResp.type_id, new Dictionary<string, string>());
            ctx.Kinetica.createTable(table3, typeResp.type_id, new Dictionary<string, string>());

            // Verify all exist
            foreach (var table in new[] { table1, table2, table3 })
            {
                var hasResp = ctx.Kinetica.hasTable(table, new Dictionary<string, string>());
                Assert.True(hasResp.table_exists);
            }
        }
    }
}
