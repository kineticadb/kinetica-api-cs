using System;
using System.Collections.Generic;
using kinetica;

namespace Kinetica.AsyncTests.Common
{
    /// <summary>
    /// Test context that manages schema and cleanup for integration tests.
    /// This mirrors the Rust TestContext implementation.
    /// </summary>
    public class TestContext : IDisposable
    {
        public kinetica.Kinetica Kinetica { get; }
        public string SchemaName { get; }

        private bool _disposed = false;

        /// <summary>
        /// Get connection configuration from environment variables.
        /// </summary>
        public static (string url, string username, string password) GetConnectionConfig()
        {
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var username = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            return (url, username, password);
        }

        /// <summary>
        /// Create a new test context with an isolated schema.
        /// </summary>
        /// <param name="testName">Name of the test (used for schema naming)</param>
        public TestContext(string testName)
        {
            var (url, username, password) = GetConnectionConfig();

            var options = new kinetica.Kinetica.Options
            {
                Username = username,
                Password = password,
                UseSnappy = false
            };

            Kinetica = new kinetica.Kinetica(url, options);
            SchemaName = $"test_{testName}";

            // Drop schema if it exists (CASCADE drops all contained objects)
            try
            {
                var dropSql = $"DROP SCHEMA IF EXISTS {SchemaName} CASCADE";
                Kinetica.executeSql(dropSql, 0, -9999);
            }
            catch { /* Ignore errors on drop */ }

            // Create fresh schema
            try
            {
                var schemaOptions = new Dictionary<string, string>
                {
                    { "no_error_if_exists", "true" }
                };
                Kinetica.createSchema(SchemaName, schemaOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create test schema '{SchemaName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get a qualified table name (schema.table).
        /// </summary>
        public string QualifiedTable(string tableName)
        {
            return $"{SchemaName}.{tableName}";
        }

        /// <summary>
        /// Cleanup - drop the test schema.
        /// </summary>
        public void Cleanup()
        {
            try
            {
                var dropSql = $"DROP SCHEMA IF EXISTS {SchemaName} CASCADE";
                Kinetica.executeSql(dropSql, 0, -9999);
            }
            catch { /* Best effort cleanup */ }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Cleanup();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Data generators for testing.
    /// </summary>
    public static class DataGenerators
    {
        private static readonly Random _random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }
            return new string(result);
        }

        public static int RandomInt()
        {
            return _random.Next();
        }

        public static double RandomDouble()
        {
            return _random.NextDouble();
        }
    }
}
