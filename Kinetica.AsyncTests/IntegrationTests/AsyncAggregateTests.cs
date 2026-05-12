using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;

namespace Kinetica.AsyncTests.IntegrationTests
{
    /// <summary>
    /// Async tests for aggregate endpoints.
    /// These tests verify that async aggregate operations work correctly.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncAggregateTests
    {
        /// <summary>
        /// Helper to setup a test table with numeric data.
        /// </summary>
        private async Task<string> SetupNumericTableAsync(TestContext ctx, int numRecords)
        {
            var tableName = ctx.QualifiedTable("numeric_table");

            // Create table using SQL
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT NOT NULL, value DOUBLE, category VARCHAR(16), score INT, PRIMARY KEY (id))");

            // Insert records with varying data
            for (int i = 0; i < numRecords; i++)
            {
                var category = (i % 3) switch
                {
                    0 => "A",
                    1 => "B",
                    _ => "C"
                };
                var value = i * 1.5;
                var score = i % 10;
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, value, category, score) VALUES ({i}, {value}, '{category}', {score})");
            }

            return tableName;
        }

        // ============================================================================
        // AGGREGATE UNIQUE TESTS
        // ============================================================================

        [Fact]
        public async Task TestAggregateUniqueBasicAsync()
        {
            using var ctx = new TestContext("async_agg_unique_basic");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Get unique categories (should be A, B, C)
            var resp = await ctx.Kinetica.AggregateUniqueAsync(tableName, "category", 0, -9999, new Dictionary<string, string>());

            // Response has data as IList<KineticaRecord> - should have 3 unique categories
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateUniqueNumericAsync()
        {
            using var ctx = new TestContext("async_agg_unique_num");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Get unique scores (0-9, repeating pattern)
            var resp = await ctx.Kinetica.AggregateUniqueAsync(tableName, "score", 0, -9999, new Dictionary<string, string>());

            // Should have 10 unique scores (0-9)
            Assert.Equal(10, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateUniqueWithLimitAsync()
        {
            using var ctx = new TestContext("async_agg_unique_limit");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Get unique categories with limit
            var resp = await ctx.Kinetica.AggregateUniqueAsync(tableName, "category", 0, 2, new Dictionary<string, string>());

            // Should be limited to 2
            Assert.Equal(2, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateUniqueEmptyTableAsync()
        {
            using var ctx = new TestContext("async_agg_unique_empty");
            var tableName = await SetupNumericTableAsync(ctx, 0);

            var resp = await ctx.Kinetica.AggregateUniqueAsync(tableName, "category", 0, -9999, new Dictionary<string, string>());

            // Empty table returns empty data
            Assert.Empty(resp.data);
        }

        // ============================================================================
        // AGGREGATE MIN/MAX TESTS
        // ============================================================================

        [Fact]
        public async Task TestAggregateMinMaxBasicAsync()
        {
            using var ctx = new TestContext("async_agg_minmax_basic");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            var resp = await ctx.Kinetica.AggregateMinMaxAsync(tableName, "id", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.min);
            Assert.Equal(99.0, resp.max);
        }

        [Fact]
        public async Task TestAggregateMinMaxDoubleColumnAsync()
        {
            using var ctx = new TestContext("async_agg_minmax_double");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            var resp = await ctx.Kinetica.AggregateMinMaxAsync(tableName, "value", new Dictionary<string, string>());

            // value = id * 1.5, so min = 0.0, max = 99 * 1.5 = 148.5
            Assert.Equal(0.0, resp.min);
            Assert.Equal(148.5, resp.max);
        }

        [Fact]
        public async Task TestAggregateMinMaxSingleValueAsync()
        {
            using var ctx = new TestContext("async_agg_minmax_single");
            var tableName = await SetupNumericTableAsync(ctx, 1);

            var resp = await ctx.Kinetica.AggregateMinMaxAsync(tableName, "id", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.min);
            Assert.Equal(0.0, resp.max);
        }

        // ============================================================================
        // AGGREGATE HISTOGRAM TESTS
        // ============================================================================

        [Fact]
        public async Task TestAggregateHistogramBasicAsync()
        {
            using var ctx = new TestContext("async_agg_hist_basic");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            // Create histogram with 10 bins over range 0-99
            var resp = await ctx.Kinetica.AggregateHistogramAsync(tableName, "id", 0.0, 100.0, 10.0, new Dictionary<string, string>());

            Assert.Equal(10, resp.counts.Count); // Should have 10 bins
            // Each bin should have approximately 10 values
            foreach (var count in resp.counts)
            {
                Assert.True(count >= 9.0 && count <= 11.0); // Allow some tolerance
            }
        }

        [Fact]
        public async Task TestAggregateHistogramUnevenDistributionAsync()
        {
            using var ctx = new TestContext("async_agg_hist_uneven");
            var tableName = await SetupNumericTableAsync(ctx, 50);

            // Create histogram on score column (values 0-9 repeating)
            var resp = await ctx.Kinetica.AggregateHistogramAsync(tableName, "score", 0.0, 10.0, 2.0, new Dictionary<string, string>());

            Assert.Equal(5, resp.counts.Count); // Should have 5 bins
            Assert.Equal(0.0, resp.start);
            Assert.Equal(10.0, resp.end);
        }

        [Fact]
        public async Task TestAggregateHistogramEmptyTableAsync()
        {
            using var ctx = new TestContext("async_agg_hist_empty");
            var tableName = await SetupNumericTableAsync(ctx, 0);

            var resp = await ctx.Kinetica.AggregateHistogramAsync(tableName, "id", 0.0, 100.0, 10.0, new Dictionary<string, string>());

            // All bins should be empty
            foreach (var count in resp.counts)
            {
                Assert.Equal(0.0, count);
            }
        }

        [Fact]
        public async Task TestAggregateHistogramSingleBinAsync()
        {
            using var ctx = new TestContext("async_agg_hist_single");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            // Create histogram with single bin covering all data
            var resp = await ctx.Kinetica.AggregateHistogramAsync(tableName, "id", 0.0, 100.0, 100.0, new Dictionary<string, string>());

            Assert.Single(resp.counts);
            Assert.Equal(100.0, resp.counts[0]); // All records in one bin
        }

        // ============================================================================
        // AGGREGATE STATISTICS TESTS
        // ============================================================================

        [Fact]
        public async Task TestAggregateStatisticsBasicAsync()
        {
            using var ctx = new TestContext("async_agg_stats_basic");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            var resp = await ctx.Kinetica.AggregateStatisticsAsync(tableName, "id", "mean,stdv,count", new Dictionary<string, string>());

            // For 0-99: mean = 49.5, count = 100
            Assert.True(Math.Abs(resp.stats["mean"] - 49.5) < 0.1);
            Assert.Equal(100.0, resp.stats["count"]);
            Assert.True(resp.stats.ContainsKey("stdv"));
        }

        [Fact]
        public async Task TestAggregateStatisticsAllStatsAsync()
        {
            using var ctx = new TestContext("async_agg_stats_all");
            var tableName = await SetupNumericTableAsync(ctx, 100);

            var resp = await ctx.Kinetica.AggregateStatisticsAsync(tableName, "value", "mean,stdv,variance,skew,kurtosis,sum", new Dictionary<string, string>());

            Assert.True(resp.stats.ContainsKey("mean"));
            Assert.True(resp.stats.ContainsKey("stdv"));
            Assert.True(resp.stats.ContainsKey("variance"));
            Assert.True(resp.stats.ContainsKey("sum"));
        }

        [Fact]
        public async Task TestAggregateStatisticsEmptyTableAsync()
        {
            using var ctx = new TestContext("async_agg_stats_empty");
            var tableName = await SetupNumericTableAsync(ctx, 0);

            var resp = await ctx.Kinetica.AggregateStatisticsAsync(tableName, "id", "mean,count", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.stats["count"]);
        }

        // ============================================================================
        // AGGREGATE GROUP BY TESTS
        // ============================================================================

        [Fact]
        public async Task TestAggregateGroupByCountAsync()
        {
            using var ctx = new TestContext("async_agg_groupby_count");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Group by category and count
            var resp = await ctx.Kinetica.AggregateGroupByAsync(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // Should have 3 groups (A, B, C)
            Assert.Equal(3, resp.total_number_of_records);
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateGroupByWithAggregatesAsync()
        {
            using var ctx = new TestContext("async_agg_groupby_agg");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Group by category
            var resp = await ctx.Kinetica.AggregateGroupByAsync(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // 3 categories
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateGroupByMultipleColumnsAsync()
        {
            using var ctx = new TestContext("async_agg_groupby_multi");

            // Create table with more grouping dimensions
            var tableName = ctx.QualifiedTable("multi_group_table");
            await ctx.Kinetica.ExecuteSqlAsync($"CREATE TABLE {tableName} (id INT NOT NULL, cat1 VARCHAR(16), cat2 VARCHAR(16), value DOUBLE, PRIMARY KEY (id))");

            // Insert test data
            for (int i = 0; i < 20; i++)
            {
                var cat1 = i < 10 ? "X" : "Y";
                var cat2 = (i % 2) == 0 ? "P" : "Q";
                await ctx.Kinetica.ExecuteSqlAsync($"INSERT INTO {tableName} (id, cat1, cat2, value) VALUES ({i}, '{cat1}', '{cat2}', {(double)i})");
            }

            // Group by both columns
            var resp = await ctx.Kinetica.AggregateGroupByAsync(tableName, new List<string> { "cat1", "cat2" }, 0, -9999, new Dictionary<string, string>());

            // Should have 4 groups: (X,P), (X,Q), (Y,P), (Y,Q)
            Assert.Equal(4, resp.data.Count);
        }

        [Fact]
        public async Task TestAggregateGroupByEmptyTableAsync()
        {
            using var ctx = new TestContext("async_agg_groupby_empty");
            var tableName = await SetupNumericTableAsync(ctx, 0);

            var resp = await ctx.Kinetica.AggregateGroupByAsync(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // Empty table returns empty data
            Assert.Empty(resp.data);
        }

        [Fact]
        public async Task TestAggregateGroupByWithLimitAsync()
        {
            using var ctx = new TestContext("async_agg_groupby_limit");
            var tableName = await SetupNumericTableAsync(ctx, 30);

            // Group by category with limit
            var resp = await ctx.Kinetica.AggregateGroupByAsync(tableName, new List<string> { "category" }, 0, 2, new Dictionary<string, string>());

            // Limited to 2 groups
            Assert.Equal(2, resp.data.Count);
        }
    }
}
