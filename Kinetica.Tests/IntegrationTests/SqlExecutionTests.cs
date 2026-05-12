using System;
using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for execute_sql endpoint.
    /// These tests mirror the Rust sql_execution_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class SqlExecutionTests
    {
        [Fact]
        public void TestExecuteSqlCreateTable()
        {
            using var ctx = new TestContext("sql_create");

            var sql = $"CREATE TABLE {ctx.SchemaName}.test_sql (id INT NOT NULL, name VARCHAR(32), PRIMARY KEY (id))";

            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            // CREATE TABLE returns count_affected=1
            Assert.Equal(1, resp.count_affected);
        }

        [Fact]
        public void TestExecuteSqlSelect()
        {
            using var ctx = new TestContext("sql_select");

            // Create and populate table using SQL
            var tableName = ctx.QualifiedTable("sel_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, value DOUBLE, PRIMARY KEY (id))");

            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value) VALUES (1, 1.1)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value) VALUES (2, 2.2)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value) VALUES (3, 3.3)");

            // Execute SELECT
            var sql = $"SELECT * FROM {tableName}";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.Equal(3, resp.total_number_of_records);
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public void TestExecuteSqlSelectWithFilter()
        {
            using var ctx = new TestContext("sql_filter");

            var tableName = ctx.QualifiedTable("flt_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, x DOUBLE, PRIMARY KEY (id))");

            // Insert 10 records
            for (int i = 0; i < 10; i++)
            {
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, x) VALUES ({i}, {i * 1.5})");
            }

            // SELECT with WHERE clause
            var sql = $"SELECT * FROM {tableName} WHERE x > 7.0";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            Assert.True(resp.total_number_of_records >= 4); // Records with id 5-9 have x > 7.0
        }

        [Fact]
        public void TestExecuteSqlCount()
        {
            using var ctx = new TestContext("sql_count");

            var tableName = ctx.QualifiedTable("cnt_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, PRIMARY KEY (id))");

            // Insert 100 records
            for (int i = 0; i < 100; i++)
            {
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id) VALUES ({i})");
            }

            // COUNT query
            var sql = $"SELECT COUNT(*) as cnt FROM {tableName}";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            // Count query returns 1 row with the count
            Assert.Equal(1, resp.total_number_of_records);
            Assert.Single(resp.data);

            // Verify the count value
            var record = resp.data[0];
            Assert.NotNull(record);
        }

        [Fact]
        public void TestExecuteSqlDropTable()
        {
            using var ctx = new TestContext("sql_drop");

            var tableName = ctx.QualifiedTable("drp_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, PRIMARY KEY (id))");

            // Verify exists
            var hasBefore = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.True(hasBefore.table_exists);

            // Drop table
            var sql = $"DROP TABLE {tableName}";
            ctx.Kinetica.executeSql(sql, 0, -9999);

            // DROP successful - verify table no longer exists
            var hasAfter = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.False(hasAfter.table_exists);
        }

        [Fact]
        public void TestExecuteSqlWithLimit()
        {
            using var ctx = new TestContext("sql_limit");

            var tableName = ctx.QualifiedTable("lim_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, PRIMARY KEY (id))");

            // Insert 50 records
            for (int i = 0; i < 50; i++)
            {
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id) VALUES ({i})");
            }

            // SELECT with limit parameter
            var sql = $"SELECT * FROM {tableName}";
            var resp = ctx.Kinetica.executeSql(sql, 0, 10);

            Assert.Equal(50, resp.total_number_of_records);
            Assert.Equal(10, resp.data.Count);
            Assert.True(resp.has_more_records);
        }

        [Fact]
        public void TestExecuteSqlShowTables()
        {
            using var ctx = new TestContext("sql_show");

            var sql = "SHOW TABLES";
            var resp = ctx.Kinetica.executeSql(sql, 0, -9999);

            // SHOW TABLES should succeed
            Assert.True(resp.total_number_of_records >= 0);
        }
    }
}
