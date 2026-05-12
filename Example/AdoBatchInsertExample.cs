using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KineticaAdo;

namespace Example
{
    /// <summary>
    /// Demonstrates the high-performance batch insert feature of the Kinetica ADO.NET driver.
    /// This example shows:
    /// - Enabling batch mode via connection properties
    /// - Enabling batch mode via connection string
    /// - Performance comparison between regular and batch inserts
    /// - Various data types with batch inserts
    /// </summary>
    public static class AdoBatchInsertExample
    {
        private const string TestSchema = "ado_batch_example";

        public static async Task RunAsync(string serverUrl, string username, string password)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("= ADO.NET Batch Insert Example - Running =");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            var connectionString = $"Server={serverUrl};Username={username};Password={password}";

            try
            {
                // Setup: Create schema and tables
                await SetupAsync(connectionString);

                // Example 1: Basic batch insert with connection properties
                await BasicBatchInsertExample(connectionString);

                // Example 2: Batch insert with connection string configuration
                await ConnectionStringBatchInsertExample(serverUrl, username, password);

                // Example 3: Performance comparison
                await PerformanceComparisonExample(connectionString);

                // Example 4: Explicit flush with FlushBatchAsync
                await ExplicitFlushExample(connectionString);

                // Example 5: Batch insert with various data types
                await VariousDataTypesExample(connectionString);

                // Example 6: Auto-flush on batch size
                await AutoFlushExample(connectionString);

                // Cleanup
                await CleanupAsync(connectionString);

                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("= ADO.NET Batch Insert Example - Done    =");
                Console.WriteLine("==========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static async Task SetupAsync(string connectionString)
        {
            Console.WriteLine("Setting up test schema and tables...");

            using var connection = new KineticaConnection(connectionString);
            await connection.OpenAsync();

            // Drop and recreate schema to ensure clean state
            using (var cmd = new KineticaCommand($"DROP SCHEMA IF EXISTS {TestSchema} CASCADE", connection))
            {
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new KineticaCommand($"CREATE SCHEMA {TestSchema}", connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Create test tables (schema is fresh, no need for IF NOT EXISTS)
            var tables = new[]
            {
                $@"CREATE TABLE {TestSchema}.batch_basic (
                    id INT NOT NULL,
                    name VARCHAR(64),
                    value DOUBLE,
                    PRIMARY KEY (id)
                )",
                $@"CREATE TABLE {TestSchema}.batch_perf (
                    id INT NOT NULL,
                    data VARCHAR(128),
                    PRIMARY KEY (id)
                )",
                $@"CREATE TABLE {TestSchema}.batch_txn (
                    id INT NOT NULL,
                    status VARCHAR(32),
                    amount DOUBLE,
                    PRIMARY KEY (id)
                )",
                $@"CREATE TABLE {TestSchema}.batch_types (
                    id INT NOT NULL,
                    int_col INT,
                    long_col LONG,
                    float_col FLOAT,
                    double_col DOUBLE,
                    string_col VARCHAR(256),
                    bool_col TINYINT,
                    PRIMARY KEY (id)
                )",
                $@"CREATE TABLE {TestSchema}.batch_autoflush (
                    id INT NOT NULL,
                    data VARCHAR(64),
                    PRIMARY KEY (id)
                )"
            };

            foreach (var tableSql in tables)
            {
                using var cmd = new KineticaCommand(tableSql, connection);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Setup complete.\n");
        }

        /// <summary>
        /// Example 1: Basic batch insert using connection properties
        /// </summary>
        private static async Task BasicBatchInsertExample(string connectionString)
        {
            Console.WriteLine("Example 1: Basic Batch Insert");
            Console.WriteLine("-----------------------------");

            using var connection = new KineticaConnection(connectionString);

            // Enable batch mode via properties
            connection.BatchInsertMode = true;
            connection.BatchSize = 1000;

            await connection.OpenAsync();

            Console.WriteLine($"  Batch mode enabled: {connection.BatchInsertMode}");
            Console.WriteLine($"  Batch size: {connection.BatchSize}");

            var tableName = $"{TestSchema}.batch_basic";

            // Clear existing data
            using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
            {
                clearCmd.ExecuteNonQuery();
            }

            // Insert 500 records
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, name, value) VALUES ({i}, 'Item_{i}', {i * 1.5})",
                    connection);
                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  Records queued: 500");
            Console.WriteLine($"  Pending count: {connection.PendingBatchCount}");

            // Flush the batch
            var flushed = await connection.FlushBatchAsync();
            sw.Stop();

            Console.WriteLine($"  Records flushed: {flushed}");
            Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");

            // Verify
            using (var verifyCmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", connection))
            {
                using var reader = verifyCmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"  Verified records: {reader.GetValue(0)}");
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Example 2: Batch insert with connection string configuration
        /// </summary>
        private static async Task ConnectionStringBatchInsertExample(string serverUrl, string username, string password)
        {
            Console.WriteLine("Example 2: Connection String Configuration");
            Console.WriteLine("------------------------------------------");

            // Configure batch settings in connection string
            var batchConnectionString = $"Server={serverUrl};Username={username};Password={password};" +
                                        "Batch Insert Mode=true;Batch Size=500;Batch Update On Existing Pk=false";

            using var connection = new KineticaConnection(batchConnectionString);
            await connection.OpenAsync();

            Console.WriteLine($"  Connection string batch mode: {connection.BatchInsertMode}");
            Console.WriteLine($"  Connection string batch size: {connection.BatchSize}");

            var tableName = $"{TestSchema}.batch_basic";

            // Insert some records using the connection string configuration
            for (int i = 1000; i < 1100; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, name, value) VALUES ({i}, 'ConnStr_{i}', {i * 2.0})",
                    connection);
                await cmd.ExecuteNonQueryAsync();
            }

            await connection.FlushBatchAsync();
            Console.WriteLine("  Inserted 100 records via connection string batch mode\n");
        }

        /// <summary>
        /// Example 3: Performance comparison between regular and batch inserts
        /// </summary>
        private static async Task PerformanceComparisonExample(string connectionString)
        {
            Console.WriteLine("Example 3: Performance Comparison");
            Console.WriteLine("---------------------------------");

            const int recordCount = 1000;
            var tableName = $"{TestSchema}.batch_perf";

            // Regular inserts (no batch mode)
            using (var connection = new KineticaConnection(connectionString))
            {
                await connection.OpenAsync();

                // Clear table
                using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
                {
                    clearCmd.ExecuteNonQuery();
                }

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < recordCount; i++)
                {
                    using var cmd = new KineticaCommand(
                        $"INSERT INTO {tableName} (id, data) VALUES ({i}, 'Regular_Insert_{i}')",
                        connection);
                    await cmd.ExecuteNonQueryAsync();
                }
                sw.Stop();

                Console.WriteLine($"  Regular inserts ({recordCount} records): {sw.ElapsedMilliseconds}ms");
                Console.WriteLine($"  Rate: {recordCount * 1000.0 / sw.ElapsedMilliseconds:F0} records/sec");
            }

            // Batch inserts
            using (var connection = new KineticaConnection(connectionString))
            {
                connection.BatchInsertMode = true;
                connection.BatchSize = 5000;
                await connection.OpenAsync();

                // Clear table
                using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
                {
                    clearCmd.ExecuteNonQuery();
                }

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < recordCount; i++)
                {
                    using var cmd = new KineticaCommand(
                        $"INSERT INTO {tableName} (id, data) VALUES ({i}, 'Batch_Insert_{i}')",
                        connection);
                    await cmd.ExecuteNonQueryAsync();
                }
                await connection.FlushBatchAsync();
                sw.Stop();

                Console.WriteLine($"  Batch inserts ({recordCount} records): {sw.ElapsedMilliseconds}ms");
                Console.WriteLine($"  Rate: {recordCount * 1000.0 / sw.ElapsedMilliseconds:F0} records/sec");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Example 4: Explicit flush control with FlushBatchAsync
        /// Shows how to explicitly flush pending batch inserts at specific points.
        /// </summary>
        private static async Task ExplicitFlushExample(string connectionString)
        {
            Console.WriteLine("Example 4: Explicit Flush Control");
            Console.WriteLine("----------------------------------");

            var tableName = $"{TestSchema}.batch_txn";

            using var connection = new KineticaConnection(connectionString);
            connection.BatchInsertMode = true;
            connection.BatchSize = 1000; // Large batch size so auto-flush won't trigger
            await connection.OpenAsync();

            // Clear table
            using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
            {
                clearCmd.ExecuteNonQuery();
            }

            // Insert records - they will be buffered
            for (int i = 0; i < 50; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, status, amount) VALUES ({i}, 'Flushed', {i * 10.0})",
                    connection);
                await cmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  Before flush - Pending: {connection.PendingBatchCount}");

            // Explicitly flush the batch to send all pending records to the database
            var flushedCount = await connection.FlushBatchAsync();

            Console.WriteLine($"  After flush - Pending: {connection.PendingBatchCount}");
            Console.WriteLine($"  Records flushed: {flushedCount}");

            // Verify
            using (var verifyCmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", connection))
            {
                using var reader = verifyCmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"  Records in table: {reader.GetValue(0)}");
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Example 5: Batch insert with various data types
        /// </summary>
        private static async Task VariousDataTypesExample(string connectionString)
        {
            Console.WriteLine("Example 5: Various Data Types");
            Console.WriteLine("-----------------------------");

            var tableName = $"{TestSchema}.batch_types";

            using var connection = new KineticaConnection(connectionString);
            connection.BatchInsertMode = true;
            connection.BatchSize = 1000;
            await connection.OpenAsync();

            // Clear table
            using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
            {
                clearCmd.ExecuteNonQuery();
            }

            // Insert records with various data types
            for (int i = 0; i < 100; i++)
            {
                var boolVal = i % 2 == 0 ? 1 : 0;
                var sql = $@"INSERT INTO {tableName}
                    (id, int_col, long_col, float_col, double_col, string_col, bool_col)
                    VALUES ({i}, {i * 10}, {i * 100000L}, {i * 1.5f}, {i * 2.5}, 'String_{i}', {boolVal})";

                using var cmd = new KineticaCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync();
            }

            // Test NULL values
            for (int i = 100; i < 110; i++)
            {
                var sql = $@"INSERT INTO {tableName}
                    (id, int_col, long_col, float_col, double_col, string_col, bool_col)
                    VALUES ({i}, NULL, NULL, NULL, NULL, NULL, NULL)";

                using var cmd = new KineticaCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync();
            }

            var flushed = await connection.FlushBatchAsync();
            Console.WriteLine($"  Inserted {flushed} records with various data types");

            // Verify
            using (var verifyCmd = new KineticaCommand(
                $"SELECT id, int_col, string_col, bool_col FROM {tableName} WHERE id < 5 ORDER BY id", connection))
            {
                using var reader = verifyCmd.ExecuteReader();
                Console.WriteLine("  Sample data:");
                while (reader.Read())
                {
                    Console.WriteLine($"    id={reader["id"]}, int_col={reader["int_col"]}, " +
                                      $"string_col={reader["string_col"]}, bool_col={reader["bool_col"]}");
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Example 6: Auto-flush when batch size is reached
        /// </summary>
        private static async Task AutoFlushExample(string connectionString)
        {
            Console.WriteLine("Example 6: Auto-Flush on Batch Size");
            Console.WriteLine("-----------------------------------");

            var tableName = $"{TestSchema}.batch_autoflush";

            using var connection = new KineticaConnection(connectionString);
            connection.BatchInsertMode = true;
            connection.BatchSize = 50; // Small batch size to trigger auto-flush
            await connection.OpenAsync();

            // Clear table
            using (var clearCmd = new KineticaCommand($"DELETE FROM {tableName}", connection))
            {
                clearCmd.ExecuteNonQuery();
            }

            Console.WriteLine($"  Batch size set to: {connection.BatchSize}");

            // Insert 120 records (should trigger 2 auto-flushes)
            for (int i = 0; i < 120; i++)
            {
                using var cmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (id, data) VALUES ({i}, 'AutoFlush_{i}')",
                    connection);
                await cmd.ExecuteNonQueryAsync();

                // Log when auto-flush happens
                if ((i + 1) % 50 == 0)
                {
                    Console.WriteLine($"  After {i + 1} inserts - Pending: {connection.PendingBatchCount}");
                }
            }

            Console.WriteLine($"  Final pending count: {connection.PendingBatchCount}");

            // Flush remaining
            var remaining = await connection.FlushBatchAsync();
            Console.WriteLine($"  Flushed remaining: {remaining}");

            // Verify total
            using (var verifyCmd = new KineticaCommand($"SELECT COUNT(*) FROM {tableName}", connection))
            {
                using var reader = verifyCmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"  Total records in table: {reader.GetValue(0)}");
                }
            }

            Console.WriteLine();
        }

        private static async Task CleanupAsync(string connectionString)
        {
            Console.WriteLine("Cleaning up test schema...");

            using var connection = new KineticaConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new KineticaCommand($"DROP SCHEMA IF EXISTS {TestSchema} CASCADE", connection);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Cleanup complete.");
        }
    }
}
