using Xunit;
using Kinetica.Tests.Common;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Basic integration tests for core API functionality.
    /// These tests mirror the Rust basic_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class BasicTests
    {
        [Fact]
        public void TestHasType()
        {
            using var ctx = new TestContext("has_type");

            // Create a simple type
            var typeDefinition = @"{
                ""type"": ""record"",
                ""name"": ""test_record"",
                ""fields"": [
                    {""name"": ""id"", ""type"": ""int""},
                    {""name"": ""name"", ""type"": ""string""}
                ]
            }";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            // Create the type
            var createResponse = ctx.Kinetica.createType(typeDefinition, "test_type", properties, new Dictionary<string, string>());
            var typeId = createResponse.type_id;

            // Verify type exists using has_type
            var hasTypeResponse = ctx.Kinetica.hasType(typeId, new Dictionary<string, string>());
            Assert.True(hasTypeResponse.type_exists, "Type should exist");

            // Verify non-existent type returns false
            var hasTypeResponseNonexistent = ctx.Kinetica.hasType("nonexistent_type_12345", new Dictionary<string, string>());
            Assert.False(hasTypeResponseNonexistent.type_exists, "Non-existent type should not exist");
        }

        [Fact]
        public void TestHasTable()
        {
            using var ctx = new TestContext("has_table");

            // Create a type
            var typeDefinition = @"{
                ""type"": ""record"",
                ""name"": ""test_record"",
                ""fields"": [
                    {""name"": ""id"", ""type"": ""int""},
                    {""name"": ""value"", ""type"": ""double""}
                ]
            }";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResponse = ctx.Kinetica.createType(typeDefinition, "has_table_type", properties, new Dictionary<string, string>());

            // Create a table
            var tableName = ctx.QualifiedTable("test_table");
            ctx.Kinetica.createTable(tableName, typeResponse.type_id, new Dictionary<string, string>());

            // Verify table exists
            var hasTableResponse = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.True(hasTableResponse.table_exists, "Table should exist");
            Assert.Equal(tableName, hasTableResponse.table_name);

            // Verify non-existent table returns false
            var nonexistentTable = ctx.QualifiedTable("nonexistent_table");
            var hasTableResponseNonexistent = ctx.Kinetica.hasTable(nonexistentTable, new Dictionary<string, string>());
            Assert.False(hasTableResponseNonexistent.table_exists, "Non-existent table should not exist");
        }

        [Fact]
        public void TestCreateAndClearTable()
        {
            using var ctx = new TestContext("create_clear");

            // Create type
            var typeDefinition = @"{
                ""type"": ""record"",
                ""name"": ""simple_record"",
                ""fields"": [
                    {""name"": ""id"", ""type"": ""int""}
                ]
            }";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResponse = ctx.Kinetica.createType(typeDefinition, "simple_type", properties, new Dictionary<string, string>());

            // Create table
            var tableName = ctx.QualifiedTable("simple_table");
            var createTableResponse = ctx.Kinetica.createTable(tableName, typeResponse.type_id, new Dictionary<string, string>());
            Assert.Equal(tableName, createTableResponse.table_name);

            // Verify table exists
            var hasTableResponse = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.True(hasTableResponse.table_exists);

            // Clear table
            var clearResponse = ctx.Kinetica.clearTable(tableName, null, new Dictionary<string, string>());
            Assert.Equal(tableName, clearResponse.table_name);

            // Table should no longer exist after clear
            var hasTableResponseAfter = ctx.Kinetica.hasTable(tableName, new Dictionary<string, string>());
            Assert.False(hasTableResponseAfter.table_exists, "Table should not exist after clear");
        }

        [Fact]
        public void TestSchemaIsolation()
        {
            // Verify that schemas don't interfere with each other
            using var ctx1 = new TestContext("isolation_1");
            using var ctx2 = new TestContext("isolation_2");

            Assert.NotEqual(ctx1.SchemaName, ctx2.SchemaName);
        }

        [Fact]
        public void TestContextCreation()
        {
            using var ctx = new TestContext("context_test");
            Assert.Equal("test_context_test", ctx.SchemaName);
            Assert.Equal("test_context_test.my_table", ctx.QualifiedTable("my_table"));
        }
    }
}
