using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for get_records endpoint.
    /// These tests verify that async record retrieval operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncGetRecordsTests
    {
        private async Task<string> SetupTestTableAsync(TestContext ctx, int numRecords)
        {
            var tableName = ctx.QualifiedTable("test_table");

            // Create table using SQL
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT NOT NULL, x DOUBLE, y DOUBLE, PRIMARY KEY (id))");

            // Insert records
            for (int i = 0; i < numRecords; i++)
            {
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, x, y) VALUES ({i}, {i * 0.5}, {i * 1.5})");
            }

            return tableName;
        }

        [Fact]
        public async Task TestGetRecordsAllAsync()
        {
            using var ctx = new TestContext("async_get_all");
            var tableName = await SetupTestTableAsync(ctx, 100);

            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(100, resp.total_number_of_records);
            Assert.False(resp.has_more_records);
        }

        [Fact]
        public async Task TestGetRecordsPaginationAsync()
        {
            using var ctx = new TestContext("async_get_pagn");
            var tableName = await SetupTestTableAsync(ctx, 100);

            // Get first page
            var page1 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, 25);

            Assert.Equal(100, page1.total_number_of_records);
            Assert.True(page1.has_more_records);

            // Get second page
            var page2 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 25, 25);

            Assert.True(page2.has_more_records);

            // Get last page
            var page4 = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 75, 25);

            Assert.False(page4.has_more_records);
        }

        [Fact]
        public async Task TestGetRecordsWithFilterAsync()
        {
            using var ctx = new TestContext("async_get_filter");
            var tableName = await SetupTestTableAsync(ctx, 100);

            // Filter for x < 25
            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} WHERE x < 25", 0, -9999);

            Assert.Equal(50, resp.total_number_of_records); // x = id * 0.5, so x < 25 means id < 50
        }

        [Fact]
        public async Task TestGetRecordsWithSortingAsync()
        {
            using var ctx = new TestContext("async_get_sort");
            var tableName = await SetupTestTableAsync(ctx, 10);

            // Sort by x descending
            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName} ORDER BY x DESC", 0, -9999);

            Assert.Equal(10, resp.total_number_of_records);
            // First record should have highest x value (9 * 0.5 = 4.5)
            Assert.True(resp.data.Count > 0);
        }

        [Fact]
        public async Task TestGetRecordsLimitZeroAsync()
        {
            using var ctx = new TestContext("async_get_limit0");
            var tableName = await SetupTestTableAsync(ctx, 100);

            // Limit 0 should return no data but correct total count
            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, 0);

            Assert.Equal(100, resp.total_number_of_records);
            Assert.Empty(resp.data);
        }

        [Fact]
        public async Task TestGetRecordsFromEmptyTableAsync()
        {
            using var ctx = new TestContext("async_get_empty");
            var tableName = await SetupTestTableAsync(ctx, 0);

            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(0, resp.total_number_of_records);
            Assert.Empty(resp.data);
            Assert.False(resp.has_more_records);
        }

        [Fact]
        public async Task TestGetRecordsOffsetBeyondEndAsync()
        {
            using var ctx = new TestContext("async_get_offset");
            var tableName = await SetupTestTableAsync(ctx, 10);

            // Offset beyond table size
            var resp = await ctx.Kinetica.ExecuteSqlAsync($"SELECT * FROM {tableName}", 100, 10);

            Assert.Equal(10, resp.total_number_of_records);
            Assert.Empty(resp.data);
        }
    }
}
