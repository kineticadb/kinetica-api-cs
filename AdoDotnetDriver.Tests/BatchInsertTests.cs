using System;
using System.Threading.Tasks;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Integration tests for the ADO.NET driver batch insert functionality.
    /// These tests require a properly configured Kinetica server with multi-head ingest enabled.
    /// Run with: dotnet test --filter "Category=BatchInsert"
    /// Skip with: dotnet test --filter "Category!=BatchInsert"
    /// </summary>
    [Trait("Category", "BatchInsert")]
    [Trait("Category", "Integration")]
    public class BatchInsertTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _testSchema;
        private readonly KineticaConnection _connection;

        public BatchInsertTests()
        {
            // Get connection string from environment or use default (consistent with IntegrationTests)
            var server = Environment.GetEnvironmentVariable("KINETICA_SERVER") ?? "http://localhost:9191";
            var username = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            _testSchema = $"test_ado_batch_{DateTime.Now:yyyyMMdd_HHmmss}";
            _connectionString = $"Server={server};Username={username};Password={password}";

            _connection = new KineticaConnection(_connectionString);
            _connection.Open();

            // Create test schema
            using var cmd = new KineticaCommand($"CREATE SCHEMA IF NOT EXISTS {_testSchema}", _connection);
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            try
            {
                // Drop test schema
                using var cmd = new KineticaCommand($"DROP SCHEMA IF EXISTS {_testSchema} CASCADE", _connection);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore cleanup errors
            }

            _connection.Close();
            _connection.Dispose();
        }

        private string QualifiedTable(string tableName) => $"{_testSchema}.{tableName}";

        [Fact]
        public async Task TestBatchInsertMode_BasicInsert()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_basic");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    name VARCHAR(64),
                    value DOUBLE,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create connection with batch mode enabled
            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records
            for (int i = 0; i < 50; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, name, value) VALUES ({i}, 'name{i}', {i * 1.5})",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Check pending count
            Assert.True(batchConnection.PendingBatchCount > 0, "Should have pending records");

            // Flush
            var flushed = await batchConnection.FlushBatchAsync();
            Assert.Equal(50, flushed);

            // Verify records were inserted
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) as cnt FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                // The count is returned as the first column value
                var count = Convert.ToInt64(reader.GetValue(0));
                Assert.Equal(50, count);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_AutoFlushOnBatchSize()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_autoflush");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    data VARCHAR(128),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create connection with small batch size to trigger auto-flush
            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 25; // Small batch size
            await batchConnection.OpenAsync();

            // Insert more records than batch size
            for (int i = 0; i < 60; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, data) VALUES ({i}, 'data{i}')",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Flush all records (auto-flush may have already sent some batches internally)
            await batchConnection.FlushBatchAsync();

            // After flush, pending should be 0
            var pending = batchConnection.PendingBatchCount;
            Assert.Equal(0, pending);

            // Verify all records were inserted
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                var count = Convert.ToInt64(reader.GetValue(0));
                Assert.Equal(60, count);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_FlushOnClose()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_closeflush");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    value INT,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create connection with batch mode
            using (var batchConnection = new KineticaConnection(_connectionString))
            {
                batchConnection.BatchInsertMode = true;
                batchConnection.BatchSize = 1000; // Large batch size - won't auto-flush
                await batchConnection.OpenAsync();

                // Insert records without manual flush
                for (int i = 0; i < 30; i++)
                {
                    using var cmd = new KineticaCommand(
                        $"INSERT INTO {tableName} (id, value) VALUES ({i}, {i * 10})",
                        batchConnection);
                    await cmd.ExecuteNonQueryAsync();
                }

                Assert.Equal(30, batchConnection.PendingBatchCount);

                // Best practice: explicitly flush before close
                // Note: Close() will also attempt to flush, but explicit flush is recommended
                var flushed = await batchConnection.FlushBatchAsync();
                Assert.Equal(30, flushed);

                batchConnection.Close();
            }

            // Verify records were inserted after close
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                var count = Convert.ToInt64(reader.GetValue(0));
                Assert.Equal(30, count);
            }
        }

        [Fact]
        public async Task TestBatchInsertMode_ConnectionStringConfig()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_connstr");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    name VARCHAR(32),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create connection with batch settings in connection string
            var batchConnStr = $"{_connectionString};BatchInsertMode=true;BatchSize=50";
            using var batchConnection = new KineticaConnection(batchConnStr);
            await batchConnection.OpenAsync();

            Assert.True(batchConnection.BatchInsertMode);
            Assert.Equal(50, batchConnection.BatchSize);

            // Insert records
            for (int i = 0; i < 20; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, name) VALUES ({i}, 'test{i}')",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            await batchConnection.FlushBatchAsync();

            // Verify
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                var count = Convert.ToInt64(reader.GetValue(0));
                Assert.Equal(20, count);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_FlushBatchAsync()
        {
            // This test verifies that FlushBatchAsync() sends all pending batch inserts to the database.
            // Note: This replaces the previous transaction-based test. Kinetica does not support
            // transactions, so we use explicit FlushBatchAsync() instead.

            // Create table
            var tableName = QualifiedTable("batch_test_flush");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    value VARCHAR(64),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create connection with batch mode
            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 1000; // Large batch size so auto-flush won't trigger
            await batchConnection.OpenAsync();

            // Insert records - they will be buffered
            for (int i = 0; i < 15; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, value) VALUES ({i}, 'flush_value{i}')",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Records should be pending
            Assert.Equal(15, batchConnection.PendingBatchCount);

            // FlushBatchAsync should send all pending records to the database
            var flushedCount = await batchConnection.FlushBatchAsync();

            // Should have no pending records after flush
            Assert.Equal(0, batchConnection.PendingBatchCount);
            Assert.Equal(15, flushedCount);

            // Verify records were inserted
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                var count = Convert.ToInt64(reader.GetValue(0));
                Assert.Equal(15, count);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_VariousDataTypes()
        {
            // Create table with various data types
            // Note: Kinetica uses TINYINT for boolean values (0/1)
            var tableName = QualifiedTable("batch_test_types");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    int_col INT,
                    long_col LONG,
                    float_col FLOAT,
                    double_col DOUBLE,
                    string_col VARCHAR(256),
                    bool_col TINYINT,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 1000;
            await batchConnection.OpenAsync();

            // Insert records with various data types
            for (int i = 0; i < 50; i++)
            {
                var boolVal = i % 2 == 0 ? 1 : 0;
                var sql = $@"INSERT INTO {tableName}
                    (id, int_col, long_col, float_col, double_col, string_col, bool_col)
                    VALUES ({i}, {i * 10}, {i * 100000L}, {i * 1.5f}, {i * 2.5}, 'String_{i}', {boolVal})";

                using var cmd = new KineticaCommand(sql, batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            var flushed = await batchConnection.FlushBatchAsync();
            Assert.Equal(50, flushed);

            // Verify count
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(50, Convert.ToInt64(reader.GetValue(0)));
            }

            // Verify sample data
            using (var cmd = new KineticaCommand($"SELECT * FROM {tableName} WHERE id = 5", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(5, Convert.ToInt32(reader["id"]));
                Assert.Equal(50, Convert.ToInt32(reader["int_col"]));
                Assert.Equal("String_5", reader["string_col"]?.ToString());
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_NullValues()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_nulls");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    nullable_int INT,
                    nullable_string VARCHAR(64),
                    nullable_double DOUBLE,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records with NULL values
            for (int i = 0; i < 20; i++)
            {
                string sql;
                if (i % 2 == 0)
                {
                    // Even IDs have NULL values
                    sql = $"INSERT INTO {tableName} (id, nullable_int, nullable_string, nullable_double) VALUES ({i}, NULL, NULL, NULL)";
                }
                else
                {
                    // Odd IDs have actual values
                    sql = $"INSERT INTO {tableName} (id, nullable_int, nullable_string, nullable_double) VALUES ({i}, {i * 10}, 'value_{i}', {i * 1.5})";
                }

                using var cmd = new KineticaCommand(sql, batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            await batchConnection.FlushBatchAsync();

            // Verify nulls
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName} WHERE nullable_int IS NULL", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(10, Convert.ToInt64(reader.GetValue(0)));
            }

            // Verify non-nulls
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName} WHERE nullable_int IS NOT NULL", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(10, Convert.ToInt64(reader.GetValue(0)));
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_LargeStrings()
        {
            // Create table with large string column
            var tableName = QualifiedTable("batch_test_large_str");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    large_text VARCHAR(4096),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records with large strings
            for (int i = 0; i < 10; i++)
            {
                var largeText = new string('X', 1000 + i * 100);
                var sql = $"INSERT INTO {tableName} (id, large_text) VALUES ({i}, '{largeText}')";

                using var cmd = new KineticaCommand(sql, batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            await batchConnection.FlushBatchAsync();

            // Verify
            using (var cmd = new KineticaCommand($"SELECT id, LENGTH(large_text) as len FROM {tableName} ORDER BY id", _connection))
            {
                using var reader = cmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    var expectedLen = 1000 + count * 100;
                    Assert.Equal(expectedLen, Convert.ToInt32(reader["len"]));
                    count++;
                }
                Assert.Equal(10, count);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_SpecialCharacters()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_special");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    text_col VARCHAR(256),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records with special characters
            var specialStrings = new[]
            {
                "Normal text",
                "Text with 'single quotes'",
                "Text, with, commas",
                "Text with numbers 12345",
                "Special @#$%^&*!",
                "Unicode: Caf\u00e9 \u4e2d\u6587"
            };

            for (int i = 0; i < specialStrings.Length; i++)
            {
                // Escape single quotes for SQL
                var escaped = specialStrings[i].Replace("'", "''");
                var sql = $"INSERT INTO {tableName} (id, text_col) VALUES ({i}, '{escaped}')";

                using var cmd = new KineticaCommand(sql, batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            await batchConnection.FlushBatchAsync();

            // Verify count
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(specialStrings.Length, Convert.ToInt64(reader.GetValue(0)));
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_NegativeNumbers()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_negative");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    neg_int INT,
                    neg_double DOUBLE,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records with negative numbers
            for (int i = 0; i < 20; i++)
            {
                var negInt = -i * 100;
                var negDouble = -i * 3.14;
                var sql = $"INSERT INTO {tableName} (id, neg_int, neg_double) VALUES ({i}, {negInt}, {negDouble})";

                using var cmd = new KineticaCommand(sql, batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            await batchConnection.FlushBatchAsync();

            // Verify
            using (var cmd = new KineticaCommand($"SELECT * FROM {tableName} WHERE id = 5", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(-500, Convert.ToInt32(reader["neg_int"]));
                Assert.True(Math.Abs(Convert.ToDouble(reader["neg_double"]) - (-5 * 3.14)) < 0.01);
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_MultipleTablesSequentially()
        {
            // Create two tables
            var tableName1 = QualifiedTable("batch_test_multi1");
            var tableName2 = QualifiedTable("batch_test_multi2");

            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName1} (
                    id INT NOT NULL,
                    value VARCHAR(32),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName2} (
                    id INT NOT NULL,
                    amount DOUBLE,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert into first table
            for (int i = 0; i < 25; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName1} (id, value) VALUES ({i}, 'table1_{i}')",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Insert into second table
            for (int i = 0; i < 30; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName2} (id, amount) VALUES ({i}, {i * 99.99})",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Flush all tables
            var totalFlushed = await batchConnection.FlushBatchAsync();
            Assert.Equal(55, totalFlushed);

            // Verify table 1
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName1}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(25, Convert.ToInt64(reader.GetValue(0)));
            }

            // Verify table 2
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName2}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(30, Convert.ToInt64(reader.GetValue(0)));
            }

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_EnableDisableToggle()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_toggle");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    mode VARCHAR(32),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var connection = new KineticaConnection(_connectionString);
            await connection.OpenAsync();

            // Initially batch mode is off
            Assert.False(connection.BatchInsertMode);

            // Insert without batch mode (immediate)
            for (int i = 0; i < 5; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, mode) VALUES ({i}, 'non_batch')",
                    connection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Enable batch mode
            connection.BatchInsertMode = true;
            Assert.True(connection.BatchInsertMode);

            // Insert with batch mode
            for (int i = 5; i < 15; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, mode) VALUES ({i}, 'batch')",
                    connection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Should have pending records
            Assert.True(connection.PendingBatchCount > 0);

            // Flush
            await connection.FlushBatchAsync();

            // Verify total count
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(15, Convert.ToInt64(reader.GetValue(0)));
            }

            connection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_EmptyFlush()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_empty");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            await batchConnection.OpenAsync();

            // Flush without inserting anything
            var flushed = await batchConnection.FlushBatchAsync();
            Assert.Equal(0, flushed);

            // Pending count should be 0
            Assert.Equal(0, batchConnection.PendingBatchCount);

            batchConnection.Close();
        }

        [Fact]
        public async Task TestBatchInsertMode_SyncFlush()
        {
            // Create table
            var tableName = QualifiedTable("batch_test_sync");
            using (var cmd = new KineticaCommand($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    data VARCHAR(32),
                    PRIMARY KEY (id)
                )", _connection))
            {
                cmd.ExecuteNonQuery();
            }

            using var batchConnection = new KineticaConnection(_connectionString);
            batchConnection.BatchInsertMode = true;
            batchConnection.BatchSize = 100;
            await batchConnection.OpenAsync();

            // Insert records
            for (int i = 0; i < 20; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, data) VALUES ({i}, 'sync_{i}')",
                    batchConnection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Use synchronous flush
            var flushed = batchConnection.FlushBatch();
            Assert.Equal(20, flushed);

            // Verify
            using (var cmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", _connection))
            {
                using var reader = cmd.ExecuteReader();
                Assert.True(reader.Read());
                Assert.Equal(20, Convert.ToInt64(reader.GetValue(0)));
            }

            batchConnection.Close();
        }
    }
}

