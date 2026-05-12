using System;
using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for type management endpoints (create_type, has_type).
    /// These tests mirror the Rust type_management_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class TypeManagementTests
    {
        [Fact]
        public void TestCreateTypeBasic()
        {
            using var ctx = new TestContext("type_basic");

            var typeDef = @"{""type"":""record"",""name"":""simple"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""}]}";

            var resp = ctx.Kinetica.createType(typeDef, "simple_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
            Assert.Equal("simple_type", resp.label);
        }

        [Fact]
        public void TestCreateTypeWithPrimaryKey()
        {
            using var ctx = new TestContext("type_pk");

            var typeDef = @"{""type"":""record"",""name"":""pk_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""value"",""type"":""double""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var resp = ctx.Kinetica.createType(typeDef, "pk_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public void TestCreateTypeWithMultipleProperties()
        {
            using var ctx = new TestContext("type_props");

            var typeDef = @"{""type"":""record"",""name"":""multi_props"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""x"",""type"":""double""},{""name"":""name"",""type"":""string""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } },
                { "x", new List<string> { "data" } },
                { "name", new List<string> { "char32" } }
            };

            var resp = ctx.Kinetica.createType(typeDef, "multi_prop_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public void TestCreateTypeComplexSchema()
        {
            using var ctx = new TestContext("type_complex");

            var typeDef = @"{""type"":""record"",""name"":""complex_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""timestamp"",""type"":""long""},{""name"":""value"",""type"":""double""},{""name"":""name"",""type"":""string""},{""name"":""active"",""type"":""int""},{""name"":""score"",""type"":""float""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var resp = ctx.Kinetica.createType(typeDef, "complex_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public void TestHasTypeExists()
        {
            using var ctx = new TestContext("has_type_exists");

            // Create a type first
            var typeDef = @"{""type"":""record"",""name"":""test"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var createResp = ctx.Kinetica.createType(typeDef, "test_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            // Check it exists
            var hasResp = ctx.Kinetica.hasType(createResp.type_id, new Dictionary<string, string>());

            Assert.True(hasResp.type_exists);
        }

        [Fact]
        public void TestHasTypeNotExists()
        {
            using var ctx = new TestContext("has_type_not");

            var hasResp = ctx.Kinetica.hasType("nonexistent_type_12345", new Dictionary<string, string>());

            Assert.False(hasResp.type_exists);
        }

        [Fact]
        public void TestCreateTypeWithStringField()
        {
            using var ctx = new TestContext("type_string");

            // Simple type with string field (strings are nullable by default in Kinetica)
            var typeDef = @"{""type"":""record"",""name"":""string_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""}]}";

            var resp = ctx.Kinetica.createType(typeDef, "string_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }
    }
}
