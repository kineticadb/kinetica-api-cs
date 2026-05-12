using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async integration tests for core API functionality.
    /// These tests verify that async methods in KineticaFunctions.cs work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncBasicTests
    {
        [Fact]
        public async Task TestHasTypeAsync()
        {
            using var ctx = new TestContext("async_has_type");

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

            // Create the type asynchronously
            var createResponse = await ctx.Kinetica.CreateTypeAsync(typeDefinition, "test_type_async", properties, new Dictionary<string, string>());
            var typeId = createResponse.type_id;

            // Verify type exists using async has_type
            var hasTypeResponse = await ctx.Kinetica.HasTypeAsync(typeId, new Dictionary<string, string>());
            Assert.True(hasTypeResponse.type_exists, "Type should exist");

            // Verify non-existent type returns false
            var hasTypeResponseNonexistent = await ctx.Kinetica.HasTypeAsync("nonexistent_type_async_12345", new Dictionary<string, string>());
            Assert.False(hasTypeResponseNonexistent.type_exists, "Non-existent type should not exist");
        }

        [Fact]
        public async Task TestHasTableAsync()
        {
            using var ctx = new TestContext("async_has_table");

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

            var typeResponse = await ctx.Kinetica.CreateTypeAsync(typeDefinition, "async_has_table_type", properties, new Dictionary<string, string>());

            // Create a table asynchronously
            var tableName = ctx.QualifiedTable("test_table_async");
            await ctx.Kinetica.CreateTableAsync(tableName, typeResponse.type_id, new Dictionary<string, string>());

            // Verify table exists
            var hasTableResponse = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.True(hasTableResponse.table_exists, "Table should exist");
            Assert.Equal(tableName, hasTableResponse.table_name);

            // Verify non-existent table returns false
            var nonexistentTable = ctx.QualifiedTable("nonexistent_table_async");
            var hasTableResponseNonexistent = await ctx.Kinetica.HasTableAsync(nonexistentTable, new Dictionary<string, string>());
            Assert.False(hasTableResponseNonexistent.table_exists, "Non-existent table should not exist");
        }

        [Fact]
        public async Task TestCreateAndClearTableAsync()
        {
            using var ctx = new TestContext("async_create_clear");

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

            var typeResponse = await ctx.Kinetica.CreateTypeAsync(typeDefinition, "simple_type_async", properties, new Dictionary<string, string>());

            // Create table asynchronously
            var tableName = ctx.QualifiedTable("simple_table_async");
            var createTableResponse = await ctx.Kinetica.CreateTableAsync(tableName, typeResponse.type_id, new Dictionary<string, string>());
            Assert.Equal(tableName, createTableResponse.table_name);

            // Verify table exists
            var hasTableResponse = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.True(hasTableResponse.table_exists);

            // Clear table asynchronously
            var clearResponse = await ctx.Kinetica.ClearTableAsync(tableName, null, new Dictionary<string, string>());
            Assert.Equal(tableName, clearResponse.table_name);

            // Table should no longer exist after clear
            var hasTableResponseAfter = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
            Assert.False(hasTableResponseAfter.table_exists, "Table should not exist after clear");
        }

        [Fact]
        public async Task TestShowSystemPropertiesAsync()
        {
            using var ctx = new TestContext("async_show_system_props");

            // Call async show_system_properties
            var response = await ctx.Kinetica.ShowSystemPropertiesAsync(new Dictionary<string, string>());

            Assert.NotNull(response);
            Assert.NotNull(response.property_map);
            Assert.True(response.property_map.Count > 0, "Should have system properties");

            // Verify some expected properties exist
            Assert.True(response.property_map.ContainsKey("conf.version") ||
                       response.property_map.ContainsKey("version.gpudb_core_version"),
                       "Should contain version information");
        }

        [Fact]
        public async Task TestShowSystemStatusAsync()
        {
            using var ctx = new TestContext("async_show_system_status");

            // Call async show_system_status
            var response = await ctx.Kinetica.ShowSystemStatusAsync(new Dictionary<string, string>());

            Assert.NotNull(response);
            Assert.NotNull(response.status_map);
            Assert.True(response.status_map.Count > 0, "Should have system status information");
        }

        [Fact]
        public async Task TestConcurrentAsyncOperations()
        {
            using var ctx = new TestContext("async_concurrent");

            // Create type
            var typeDefinition = @"{
                ""type"": ""record"",
                ""name"": ""concurrent_record"",
                ""fields"": [
                    {""name"": ""id"", ""type"": ""int""},
                    {""name"": ""data"", ""type"": ""string""}
                ]
            }";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "int", "primary_key" } }
            };

            var typeResponse = await ctx.Kinetica.CreateTypeAsync(typeDefinition, "concurrent_type", properties, new Dictionary<string, string>());

            // Create multiple tables concurrently
            var tasks = new List<Task>();
            var tableNames = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                var tableName = ctx.QualifiedTable($"concurrent_table_{i}");
                tableNames.Add(tableName);
                tasks.Add(ctx.Kinetica.CreateTableAsync(tableName, typeResponse.type_id, new Dictionary<string, string>()));
            }

            // Wait for all tables to be created
            await Task.WhenAll(tasks);

            // Verify all tables exist concurrently
            var verifyTasks = tableNames.Select(async tableName =>
            {
                var hasTableResponse = await ctx.Kinetica.HasTableAsync(tableName, new Dictionary<string, string>());
                Assert.True(hasTableResponse.table_exists, $"Table {tableName} should exist");
            });

            await Task.WhenAll(verifyTasks);
        }

        [Fact]
        public async Task TestAsyncWithCancellationToken()
        {
            using var ctx = new TestContext("async_cancellation");

            var cts = new System.Threading.CancellationTokenSource();

            // This should complete successfully
            var response = await ctx.Kinetica.ShowSystemPropertiesAsync(new Dictionary<string, string>(), cts.Token);
            Assert.NotNull(response);

            // Test cancellation (create token that's already cancelled)
            var cancelledCts = new System.Threading.CancellationTokenSource();
            cancelledCts.Cancel();

            // The transport layer wraps cancellation in KineticaException
            var exception = await Assert.ThrowsAsync<KineticaException>(async () =>
            {
                await ctx.Kinetica.ShowSystemPropertiesAsync(new Dictionary<string, string>(), cancelledCts.Token);
            });

            // Verify the exception is related to cancellation/timeout
            Assert.Contains("timed out", exception.Message.ToLower());
        }
    }
}
