using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for type management endpoints (create_type, has_type).
    /// These tests verify that async type operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncTypeManagementTests
    {
        [Fact]
        public async Task TestCreateTypeBasicAsync()
        {
            using var ctx = new TestContext("async_type_basic");

            var typeDef = @"{""type"":""record"",""name"":""simple"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""}]}";

            var resp = await ctx.Kinetica.CreateTypeAsync(typeDef, "simple_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
            Assert.Equal("simple_type", resp.label);
        }

        [Fact]
        public async Task TestCreateTypeWithPrimaryKeyAsync()
        {
            using var ctx = new TestContext("async_type_pk");

            var typeDef = @"{""type"":""record"",""name"":""pk_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""value"",""type"":""double""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var resp = await ctx.Kinetica.CreateTypeAsync(typeDef, "pk_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public async Task TestCreateTypeWithMultiplePropertiesAsync()
        {
            using var ctx = new TestContext("async_type_props");

            var typeDef = @"{""type"":""record"",""name"":""multi_props"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""x"",""type"":""double""},{""name"":""name"",""type"":""string""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } },
                { "x", new List<string> { "data" } },
                { "name", new List<string> { "char32" } }
            };

            var resp = await ctx.Kinetica.CreateTypeAsync(typeDef, "multi_prop_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public async Task TestCreateTypeComplexSchemaAsync()
        {
            using var ctx = new TestContext("async_type_complex");

            var typeDef = @"{""type"":""record"",""name"":""complex_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""timestamp"",""type"":""long""},{""name"":""value"",""type"":""double""},{""name"":""name"",""type"":""string""},{""name"":""active"",""type"":""int""},{""name"":""score"",""type"":""float""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var resp = await ctx.Kinetica.CreateTypeAsync(typeDef, "complex_type", properties, new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }

        [Fact]
        public async Task TestHasTypeExistsAsync()
        {
            using var ctx = new TestContext("async_has_type_exists");

            // Create a type first
            var typeDef = @"{""type"":""record"",""name"":""test"",""fields"":[{""name"":""id"",""type"":""int""}]}";
            var createResp = await ctx.Kinetica.CreateTypeAsync(typeDef, "test_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            // Check it exists
            var hasResp = await ctx.Kinetica.HasTypeAsync(createResp.type_id, new Dictionary<string, string>());

            Assert.True(hasResp.type_exists);
        }

        [Fact]
        public async Task TestHasTypeNotExistsAsync()
        {
            using var ctx = new TestContext("async_has_type_not");

            var hasResp = await ctx.Kinetica.HasTypeAsync("nonexistent_type_async_12345", new Dictionary<string, string>());

            Assert.False(hasResp.type_exists);
        }

        [Fact]
        public async Task TestCreateTypeWithStringFieldAsync()
        {
            using var ctx = new TestContext("async_type_string");

            // Simple type with string field (strings are nullable by default in Kinetica)
            var typeDef = @"{""type"":""record"",""name"":""string_type"",""fields"":[{""name"":""id"",""type"":""int""},{""name"":""name"",""type"":""string""}]}";

            var resp = await ctx.Kinetica.CreateTypeAsync(typeDef, "string_type", new Dictionary<string, IList<string>>(), new Dictionary<string, string>());

            Assert.False(string.IsNullOrEmpty(resp.type_id));
        }
    }
}
