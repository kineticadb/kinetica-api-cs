using Xunit;
using Kinetica.Tests.Common;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for aggregate endpoints.
    /// These tests mirror the Rust aggregate_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class AggregateTests
    {
        /// <summary>
        /// Helper to setup a test table with numeric data.
        /// </summary>
        private string SetupNumericTable(TestContext ctx, int numRecords)
        {
            var tableName = ctx.QualifiedTable("numeric_table");

            // Create table using SQL
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, value DOUBLE, category VARCHAR(16), score INT, PRIMARY KEY (id))");

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
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, value, category, score) VALUES ({i}, {value}, '{category}', {score})");
            }

            return tableName;
        }

        // ============================================================================
        // AGGREGATE UNIQUE TESTS
        // ============================================================================

        [Fact]
        public void TestAggregateUniqueBasic()
        {
            using var ctx = new TestContext("agg_unique_basic");
            var tableName = SetupNumericTable(ctx, 30);

            // Get unique categories (should be A, B, C)
            var resp = ctx.Kinetica.aggregateUnique(tableName, "category", 0, -9999, new Dictionary<string, string>());

            // Response has data as IList<KineticaRecord> - should have 3 unique categories
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public void TestAggregateUniqueNumeric()
        {
            using var ctx = new TestContext("agg_unique_num");
            var tableName = SetupNumericTable(ctx, 30);

            // Get unique scores (0-9, repeating pattern)
            var resp = ctx.Kinetica.aggregateUnique(tableName, "score", 0, -9999, new Dictionary<string, string>());

            // Should have 10 unique scores (0-9)
            Assert.Equal(10, resp.data.Count);
        }

        [Fact]
        public void TestAggregateUniqueWithLimit()
        {
            using var ctx = new TestContext("agg_unique_limit");
            var tableName = SetupNumericTable(ctx, 30);

            // Get unique categories with limit
            var resp = ctx.Kinetica.aggregateUnique(tableName, "category", 0, 2, new Dictionary<string, string>());

            // Should be limited to 2
            Assert.Equal(2, resp.data.Count);
        }

        [Fact]
        public void TestAggregateUniqueEmptyTable()
        {
            using var ctx = new TestContext("agg_unique_empty");
            var tableName = SetupNumericTable(ctx, 0);

            var resp = ctx.Kinetica.aggregateUnique(tableName, "category", 0, -9999, new Dictionary<string, string>());

            // Empty table returns empty data
            Assert.Empty(resp.data);
        }

        // ============================================================================
        // AGGREGATE MIN/MAX TESTS
        // ============================================================================

        [Fact]
        public void TestAggregateMinMaxBasic()
        {
            using var ctx = new TestContext("agg_minmax_basic");
            var tableName = SetupNumericTable(ctx, 100);

            var resp = ctx.Kinetica.aggregateMinMax(tableName, "id", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.min);
            Assert.Equal(99.0, resp.max);
        }

        [Fact]
        public void TestAggregateMinMaxDoubleColumn()
        {
            using var ctx = new TestContext("agg_minmax_double");
            var tableName = SetupNumericTable(ctx, 100);

            var resp = ctx.Kinetica.aggregateMinMax(tableName, "value", new Dictionary<string, string>());

            // value = id * 1.5, so min = 0.0, max = 99 * 1.5 = 148.5
            Assert.Equal(0.0, resp.min);
            Assert.Equal(148.5, resp.max);
        }

        [Fact]
        public void TestAggregateMinMaxSingleValue()
        {
            using var ctx = new TestContext("agg_minmax_single");
            var tableName = SetupNumericTable(ctx, 1);

            var resp = ctx.Kinetica.aggregateMinMax(tableName, "id", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.min);
            Assert.Equal(0.0, resp.max);
        }

        // ============================================================================
        // AGGREGATE HISTOGRAM TESTS
        // ============================================================================

        [Fact]
        public void TestAggregateHistogramBasic()
        {
            using var ctx = new TestContext("agg_hist_basic");
            var tableName = SetupNumericTable(ctx, 100);

            // Create histogram with 10 bins over range 0-99
            var resp = ctx.Kinetica.aggregateHistogram(tableName, "id", 0.0, 100.0, 10.0, new Dictionary<string, string>());

            Assert.Equal(10, resp.counts.Count); // Should have 10 bins
            // Each bin should have approximately 10 values
            foreach (var count in resp.counts)
            {
                Assert.True(count >= 9.0 && count <= 11.0); // Allow some tolerance
            }
        }

        [Fact]
        public void TestAggregateHistogramUnevenDistribution()
        {
            using var ctx = new TestContext("agg_hist_uneven");
            var tableName = SetupNumericTable(ctx, 50);

            // Create histogram on score column (values 0-9 repeating)
            var resp = ctx.Kinetica.aggregateHistogram(tableName, "score", 0.0, 10.0, 2.0, new Dictionary<string, string>());

            Assert.Equal(5, resp.counts.Count); // Should have 5 bins
            Assert.Equal(0.0, resp.start);
            Assert.Equal(10.0, resp.end);
        }

        [Fact]
        public void TestAggregateHistogramEmptyTable()
        {
            using var ctx = new TestContext("agg_hist_empty");
            var tableName = SetupNumericTable(ctx, 0);

            var resp = ctx.Kinetica.aggregateHistogram(tableName, "id", 0.0, 100.0, 10.0, new Dictionary<string, string>());

            // All bins should be empty
            foreach (var count in resp.counts)
            {
                Assert.Equal(0.0, count);
            }
        }

        [Fact]
        public void TestAggregateHistogramSingleBin()
        {
            using var ctx = new TestContext("agg_hist_single");
            var tableName = SetupNumericTable(ctx, 100);

            // Create histogram with single bin covering all data
            var resp = ctx.Kinetica.aggregateHistogram(tableName, "id", 0.0, 100.0, 100.0, new Dictionary<string, string>());

            Assert.Single(resp.counts);
            Assert.Equal(100.0, resp.counts[0]); // All records in one bin
        }

        // ============================================================================
        // AGGREGATE STATISTICS TESTS
        // ============================================================================

        [Fact]
        public void TestAggregateStatisticsBasic()
        {
            using var ctx = new TestContext("agg_stats_basic");
            var tableName = SetupNumericTable(ctx, 100);

            var resp = ctx.Kinetica.aggregateStatistics(tableName, "id", "mean,stdv,count", new Dictionary<string, string>());

            // For 0-99: mean = 49.5, count = 100
            Assert.True(Math.Abs(resp.stats["mean"] - 49.5) < 0.1);
            Assert.Equal(100.0, resp.stats["count"]);
            Assert.True(resp.stats.ContainsKey("stdv"));
        }

        [Fact]
        public void TestAggregateStatisticsAllStats()
        {
            using var ctx = new TestContext("agg_stats_all");
            var tableName = SetupNumericTable(ctx, 100);

            var resp = ctx.Kinetica.aggregateStatistics(tableName, "value", "mean,stdv,variance,skew,kurtosis,sum", new Dictionary<string, string>());

            Assert.True(resp.stats.ContainsKey("mean"));
            Assert.True(resp.stats.ContainsKey("stdv"));
            Assert.True(resp.stats.ContainsKey("variance"));
            Assert.True(resp.stats.ContainsKey("sum"));
        }

        [Fact]
        public void TestAggregateStatisticsEmptyTable()
        {
            using var ctx = new TestContext("agg_stats_empty");
            var tableName = SetupNumericTable(ctx, 0);

            var resp = ctx.Kinetica.aggregateStatistics(tableName, "id", "mean,count", new Dictionary<string, string>());

            Assert.Equal(0.0, resp.stats["count"]);
        }

        // ============================================================================
        // AGGREGATE GROUP BY TESTS
        // ============================================================================

        [Fact]
        public void TestAggregateGroupByCount()
        {
            using var ctx = new TestContext("agg_groupby_count");
            var tableName = SetupNumericTable(ctx, 30);

            // Group by category and count
            var resp = ctx.Kinetica.aggregateGroupBy(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // Should have 3 groups (A, B, C)
            Assert.Equal(3, resp.total_number_of_records);
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public void TestAggregateGroupByWithAggregates()
        {
            using var ctx = new TestContext("agg_groupby_agg");
            var tableName = SetupNumericTable(ctx, 30);

            // Group by category
            var resp = ctx.Kinetica.aggregateGroupBy(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // 3 categories
            Assert.Equal(3, resp.data.Count);
        }

        [Fact]
        public void TestAggregateGroupByMultipleColumns()
        {
            using var ctx = new TestContext("agg_groupby_multi");

            // Create table with more grouping dimensions
            var tableName = ctx.QualifiedTable("multi_group_table");
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, cat1 VARCHAR(16), cat2 VARCHAR(16), value DOUBLE, PRIMARY KEY (id))");

            // Insert test data
            for (int i = 0; i < 20; i++)
            {
                var cat1 = i < 10 ? "X" : "Y";
                var cat2 = (i % 2) == 0 ? "P" : "Q";
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, cat1, cat2, value) VALUES ({i}, '{cat1}', '{cat2}', {(double)i})");
            }

            // Group by both columns
            var resp = ctx.Kinetica.aggregateGroupBy(tableName, new List<string> { "cat1", "cat2" }, 0, -9999, new Dictionary<string, string>());

            // Should have 4 groups: (X,P), (X,Q), (Y,P), (Y,Q)
            Assert.Equal(4, resp.data.Count);
        }

        [Fact]
        public void TestAggregateGroupByEmptyTable()
        {
            using var ctx = new TestContext("agg_groupby_empty");
            var tableName = SetupNumericTable(ctx, 0);

            var resp = ctx.Kinetica.aggregateGroupBy(tableName, new List<string> { "category" }, 0, -9999, new Dictionary<string, string>());

            // Empty table returns empty data
            Assert.Empty(resp.data);
        }

        [Fact]
        public void TestAggregateGroupByWithLimit()
        {
            using var ctx = new TestContext("agg_groupby_limit");
            var tableName = SetupNumericTable(ctx, 30);

            // Group by category with limit
            var resp = ctx.Kinetica.aggregateGroupBy(tableName, new List<string> { "category" }, 0, 2, new Dictionary<string, string>());

            // Limited to 2 groups
            Assert.Equal(2, resp.data.Count);
        }
    }
}
