using System;
using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Tests for get_records endpoint.
    /// These tests mirror the Rust get_records_tests.rs implementation.
    /// </summary>
    [Trait("Category", "Integration")]
    public class GetRecordsTests
    {
        private string SetupTestTable(TestContext ctx, int numRecords)
        {
            var tableName = ctx.QualifiedTable("test_table");

            // Create table using SQL
            ctx.Kinetica.executeSql($"CREATE TABLE {tableName} (id INT NOT NULL, x DOUBLE, y DOUBLE, PRIMARY KEY (id))");

            // Insert records
            for (int i = 0; i < numRecords; i++)
            {
                ctx.Kinetica.executeSql($"INSERT INTO {tableName} (id, x, y) VALUES ({i}, {i * 0.5}, {i * 1.5})");
            }

            return tableName;
        }

        [Fact]
        public void TestGetRecordsAll()
        {
            using var ctx = new TestContext("get_all");
            var tableName = SetupTestTable(ctx, 100);

            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(100, resp.total_number_of_records);
            Assert.False(resp.has_more_records);
        }

        [Fact]
        public void TestGetRecordsPagination()
        {
            using var ctx = new TestContext("get_pagn");
            var tableName = SetupTestTable(ctx, 100);

            // Get first page
            var page1 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, 25);

            Assert.Equal(100, page1.total_number_of_records);
            Assert.True(page1.has_more_records);

            // Get second page
            var page2 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 25, 25);

            Assert.True(page2.has_more_records);

            // Get last page
            var page4 = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 75, 25);

            Assert.False(page4.has_more_records);
        }

        [Fact]
        public void TestGetRecordsWithFilter()
        {
            using var ctx = new TestContext("get_filter");
            var tableName = SetupTestTable(ctx, 100);

            // Filter for x < 25
            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} WHERE x < 25", 0, -9999);

            Assert.Equal(50, resp.total_number_of_records); // x = id * 0.5, so x < 25 means id < 50
        }

        [Fact]
        public void TestGetRecordsWithSorting()
        {
            using var ctx = new TestContext("get_sort");
            var tableName = SetupTestTable(ctx, 10);

            // Sort by x descending
            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName} ORDER BY x DESC", 0, -9999);

            Assert.Equal(10, resp.total_number_of_records);
            // First record should have highest x value (9 * 0.5 = 4.5)
            Assert.True(resp.data.Count > 0);
        }

        [Fact]
        public void TestGetRecordsLimitZero()
        {
            using var ctx = new TestContext("get_limit0");
            var tableName = SetupTestTable(ctx, 100);

            // Limit 0 should return no data but correct total count
            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, 0);

            Assert.Equal(100, resp.total_number_of_records);
            Assert.Empty(resp.data);
        }

        [Fact]
        public void TestGetRecordsFromEmptyTable()
        {
            using var ctx = new TestContext("get_empty");
            var tableName = SetupTestTable(ctx, 0);

            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 0, -9999);

            Assert.Equal(0, resp.total_number_of_records);
            Assert.Empty(resp.data);
            Assert.False(resp.has_more_records);
        }

        [Fact]
        public void TestGetRecordsOffsetBeyondEnd()
        {
            using var ctx = new TestContext("get_offset");
            var tableName = SetupTestTable(ctx, 10);

            // Offset beyond table size
            var resp = ctx.Kinetica.executeSql($"SELECT * FROM {tableName}", 100, 10);

            Assert.Equal(10, resp.total_number_of_records);
            Assert.Empty(resp.data);
        }
    }
}
