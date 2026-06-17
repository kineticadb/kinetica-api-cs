/*
 * Table Utility Examples
 *
 * This file contains utility examples for common table operations:
 *   - ClearTable: Delete all records from a table (keeping structure)
 *   - CleanupAll: Drop all test tables matching a pattern
 *
 * Run with:
 *   dotnet run --project Example -- --clear-table [tableName]
 *   dotnet run --project Example -- --cleanup-all [schemaPattern]
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kinetica;

namespace Example
{
    #region Clear Table

    /// <summary>
    /// Example: Clear All Data from a Table
    ///
    /// This example demonstrates how to delete all records from a table
    /// while keeping the table structure intact.
    ///
    /// WARNING: This operation is IRREVERSIBLE!
    /// </summary>
    public static class ClearTableExample
    {
        public static async Task RunAsync(string tableName)
        {
            Console.WriteLine("=== Clear Table Example ===\n");

            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Usage: dotnet run --project Example -- --clear-table <table_name>");
                Console.WriteLine("Example: dotnet run --project Example -- --clear-table test_schema.my_table");
                return;
            }

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            Console.WriteLine($"! WARNING: This will delete ALL data from table '{tableName}'");
            Console.WriteLine("This operation is IRREVERSIBLE!\n");

            Console.WriteLine("Waiting 3 seconds... (Press Ctrl+C to cancel)");
            for (int i = 3; i >= 1; i--)
            {
                Console.WriteLine($"{i}...");
                await Task.Delay(1000);
            }

            Console.WriteLine($"\nClearing table '{tableName}'...");

            try
            {
                var request = new ClearTableRequest(tableName);
                var response = kinetica.clearTable(request);

                Console.WriteLine("+ Table cleared successfully!");
                Console.WriteLine($"  Table Name: {response.table_name}");

                if (response.info.TryGetValue("count", out var count))
                {
                    Console.WriteLine($"  Records Deleted: {count}");
                }
            }
            catch (KineticaException ex)
            {
                Console.WriteLine($"x Error clearing table: {ex.Message}");
                return;
            }

            Console.WriteLine($"\nAll data has been removed from '{tableName}'");
        }
    }

    #endregion

    #region Cleanup All

    /// <summary>
    /// Example: Cleanup All Test Tables
    ///
    /// This example demonstrates how to drop all tables matching a pattern
    /// in a schema. Useful for cleaning up after test runs.
    ///
    /// WARNING: This operation is IRREVERSIBLE!
    /// </summary>
    public static class CleanupAllExample
    {
        public static async Task RunAsync(string schemaPattern = "test_schema")
        {
            Console.WriteLine("=== Cleanup All Test Tables Example ===\n");

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            Console.WriteLine($"! WARNING: This will DROP all tables in schema matching '{schemaPattern}'");
            Console.WriteLine("This operation is IRREVERSIBLE!\n");

            // First, list all tables in the schema
            Console.WriteLine("Discovering tables...");

            IList<string> tablesToDrop = new List<string>();

            try
            {
                var showTableResponse = kinetica.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" }
                });

                foreach (var tbl in showTableResponse.table_names)
                {
                    if (tbl.StartsWith(schemaPattern))
                    {
                        tablesToDrop.Add(tbl);
                    }
                }
            }
            catch (KineticaException ex)
            {
                Console.WriteLine($"Error discovering tables: {ex.Message}");
                return;
            }

            if (tablesToDrop.Count == 0)
            {
                Console.WriteLine($"No tables found matching pattern '{schemaPattern}'");
                return;
            }

            Console.WriteLine($"\nFound {tablesToDrop.Count} table(s) to drop:");
            foreach (var tbl in tablesToDrop)
            {
                Console.WriteLine($"  - {tbl}");
            }

            Console.WriteLine("\nWaiting 5 seconds... (Press Ctrl+C to cancel)");
            for (int i = 5; i >= 1; i--)
            {
                Console.WriteLine($"{i}...");
                await Task.Delay(1000);
            }

            Console.WriteLine("\nDropping tables...");

            var droppedCount = 0;
            var failedCount = 0;

            foreach (var tbl in tablesToDrop)
            {
                try
                {
                    kinetica.executeSql($"DROP TABLE IF EXISTS {tbl}");
                    Console.WriteLine($"  + Dropped: {tbl}");
                    droppedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  x Failed to drop {tbl}: {ex.Message}");
                    failedCount++;
                }
            }

            // Also try to drop the schema if it's empty
            try
            {
                if (schemaPattern.Contains('.'))
                {
                    schemaPattern = schemaPattern.Split('.')[0];
                }

                kinetica.executeSql($"DROP SCHEMA IF EXISTS {schemaPattern}");
                Console.WriteLine($"\n  + Dropped schema: {schemaPattern}");
            }
            catch
            {
                // Schema might not be empty or might not exist, that's ok
            }

            Console.WriteLine($"\nCleanup complete!");
            Console.WriteLine($"  Dropped: {droppedCount} table(s)");
            if (failedCount > 0)
            {
                Console.WriteLine($"  Failed:  {failedCount} table(s)");
            }
        }
    }

    #endregion

    #region Show Tables

    /// <summary>
    /// Example: List All Tables
    ///
    /// Displays all tables in the database with their record counts.
    /// </summary>
    public static class ShowTablesExample
    {
        public static void Run(string schemaFilter = "")
        {
            Console.WriteLine("=== Show Tables Example ===\n");

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            Console.WriteLine($"Connected to: {url}\n");

            try
            {
                var showTableResponse = kinetica.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" },
                    { "get_sizes", "true" }
                });

                Console.WriteLine($"{"Table Name",-50} {"Records",-15} {"Type ID",-40}");
                Console.WriteLine(new string('-', 105));

                var tableCount = 0;
                for (int i = 0; i < showTableResponse.table_names.Count; i++)
                {
                    var tableName = showTableResponse.table_names[i];

                    // Apply schema filter if provided
                    if (!string.IsNullOrEmpty(schemaFilter) && !tableName.StartsWith(schemaFilter))
                        continue;

                    var typeId = i < showTableResponse.type_ids.Count
                        ? showTableResponse.type_ids[i]
                        : "N/A";

                    var recordCount = "N/A";
                    if (i < showTableResponse.sizes.Count)
                    {
                        recordCount = showTableResponse.sizes[i].ToString("N0");
                    }

                    Console.WriteLine($"{tableName,-50} {recordCount,-15} {typeId,-40}");
                    tableCount++;
                }

                Console.WriteLine(new string('-', 105));
                Console.WriteLine($"Total: {tableCount} table(s)");
            }
            catch (KineticaException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    #endregion

    #region Truncate Table

    /// <summary>
    /// Example: Truncate Table (faster than ClearTable for large tables)
    ///
    /// Uses SQL TRUNCATE which is faster than DELETE for large tables.
    /// </summary>
    public static class TruncateTableExample
    {
        public static async Task RunAsync(string tableName)
        {
            Console.WriteLine("=== Truncate Table Example ===\n");

            if (string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("Usage: dotnet run --project Example -- --truncate <table_name>");
                return;
            }

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            Console.WriteLine($"! WARNING: This will TRUNCATE table '{tableName}'");
            Console.WriteLine("This operation is IRREVERSIBLE!\n");

            Console.WriteLine("Waiting 3 seconds... (Press Ctrl+C to cancel)");
            for (int i = 3; i >= 1; i--)
            {
                Console.WriteLine($"{i}...");
                await Task.Delay(1000);
            }

            Console.WriteLine($"\nTruncating table '{tableName}'...");

            try
            {
                kinetica.executeSql($"TRUNCATE TABLE {tableName}");
                Console.WriteLine("+ Table truncated successfully!");
            }
            catch (KineticaException ex)
            {
                Console.WriteLine($"x Error truncating table: {ex.Message}");
                return;
            }

            Console.WriteLine($"\nAll data has been removed from '{tableName}'");
        }
    }

    #endregion
}
