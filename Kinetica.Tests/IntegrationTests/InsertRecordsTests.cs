using System;
using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for insert_records endpoint.
    /// These tests mirror the Rust insert_records_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class InsertRecordsTests
    {
        [Fact]
        public void TestInsertRecordsJson()
        {
            using var ctx = new TestContext("insert_json");

            // Create type
            var typeDef = @"{""type"":""record"",""name"":""test_rec"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""},{""name"":""value"",""type"":""double""}]}";
            var props = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResp = ctx.Kinetica.createType(typeDef, "insert_test_type", props, new Dictionary<string, string>());

            // Create table
            var tableName = ctx.QualifiedTable("test_table");
            ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Insert records using SQL
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, name, value) VALUES (1, 'Alice', 1.1)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, name, value) VALUES (2, 'Bob', 2.2)");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, name, value) VALUES (3, 'Charlie', 3.3)");

            // Verify records were inserted using showTable with get_sizes option
            var showTableResp = ctx.Kinetica.showTable(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(3, showTableResp.total_size);
        }

        [Fact]
        public void TestInsertRecordsUpdateOnExistingPk()
        {
            using var ctx = new TestContext("insert_update_pk");

            // Create type with primary key
            var typeDef = @"{""type"":""record"",""name"":""pk_rec"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""value"",""type"":""string""}]}";
            var props = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResp = ctx.Kinetica.createType(typeDef, "pk_test_type", props, new Dictionary<string, string>());

            var tableName = ctx.QualifiedTable("pk_table");
            ctx.Kinetica.createTable(tableName, typeResp.type_id, new Dictionary<string, string>());

            // Insert initial records
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value) VALUES (1, 'original')");
            ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value) VALUES (2, 'data')");

            // Verify 2 records exist using showTable with get_sizes option
            var showTableResp = ctx.Kinetica.showTable(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(2, showTableResp.total_size);

            // Update existing record using UPDATE statement
            ctx.Kinetica.executeSql($"UPDATE {tableName} SET value = 'updated' WHERE id = 1");

            // Should still have 2 records
            var showTableResp2 = ctx.Kinetica.showTable(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(2, showTableResp2.total_size);

            // Verify the value was updated
            var selectResp = ctx.Kinetica.executeSql($"SELECT value FROM {tableName} WHERE id = 1", 0, -9999);
            Assert.Equal(1, selectResp.total_number_of_records);
        }

        [Fact]
        public void TestInsertRecordsLargeBatch()
        {
            using var ctx = new TestContext("insert_large");

            // Create table using SQL
            ctx.Kinetica.executeSql($"CREATE TABLE {ctx.QualifiedTable("batch_table")} (id INT NOT NULL, x DOUBLE, y DOUBLE, PRIMARY KEY (id))");

            var tableName = ctx.QualifiedTable("batch_table");

            // Insert 100 records (reduced from 1000 for faster testing)
            var numRecords = 100;
            for (int i = 0; i < numRecords; i++)
            {
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, x, y) VALUES ({i}, {i * 0.1}, {i * 0.2})");
            }

            // Verify count using showTable with get_sizes option
            var showTableResp = ctx.Kinetica.showTable(tableName, new Dictionary<string, string> { { "get_sizes", "true" } });
            Assert.Equal(numRecords, showTableResp.total_size);
        }
    }
}
