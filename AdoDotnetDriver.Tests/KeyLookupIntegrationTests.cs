using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using KineticaAdo;
using kinetica;
using kinetica.Utils;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Integration tests for key lookup functionality using GenericRecordRetriever.
    /// These tests require a running Kinetica server.
    /// </summary>
    [Trait("Category", "Integration")]
    public class KeyLookupIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Server=http://localhost:9191;Username=admin;Password=secret";
        private const string TestTableName = "ado_key_lookup_test";
        private const string TestSchemaName = "ado_test";
        private const string FullTableName = $"{TestSchemaName}.{TestTableName}";
        private KineticaConnection? _connection;
        private Kinetica? _kinetica;

        public async Task InitializeAsync()
        {
            _connection = new KineticaConnection(ConnectionString);
            await _connection.OpenAsync();

            // Create Kinetica client directly for testing
            _kinetica = new Kinetica("http://localhost:9191", new Kinetica.Options()
            {
                Username = "admin",
                Password = "secret"
            });

            // Create schema if not exists
            try
            {
                using var schemaCmd = new KineticaCommand($"CREATE SCHEMA IF NOT EXISTS {TestSchemaName}", _connection);
                await schemaCmd.ExecuteNonQueryAsync();
            }
            catch { }

            // Create test table with shard key
            using var dropCmd = new KineticaCommand($"DROP TABLE IF EXISTS {FullTableName}", _connection);
            await dropCmd.ExecuteNonQueryAsync();

            using var createCmd = new KineticaCommand($@"
                CREATE TABLE {FullTableName} (
                    customer_id INT NOT NULL,
                    order_id INT NOT NULL,
                    product VARCHAR(64),
                    quantity INT,
                    price DOUBLE,
                    order_date DATE,
                    SHARD KEY (customer_id),
                    PRIMARY KEY (customer_id, order_id)
                )", _connection);
            await createCmd.ExecuteNonQueryAsync();

            // Insert test data
            using var insertCmd = new KineticaCommand($@"
                INSERT INTO {FullTableName} VALUES
                (1, 1001, 'Widget A', 5, 10.99, '2024-01-15'),
                (1, 1002, 'Widget B', 3, 25.50, '2024-01-16'),
                (1, 1003, 'Gadget X', 2, 99.99, '2024-01-17'),
                (2, 2001, 'Widget A', 10, 10.99, '2024-02-01'),
                (2, 2002, 'Gadget Y', 1, 199.99, '2024-02-15'),
                (3, 3001, 'Widget C', 7, 15.00, '2024-03-01')
            ", _connection);
            await insertCmd.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                try
                {
                    using var cmd = new KineticaCommand($"DROP TABLE IF EXISTS {FullTableName}", _connection);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch { }

                _connection.Close();
                _connection.Dispose();
            }
        }

        #region GenericRecordKeyBuilder Tests

        [Fact]
        public void GenericRecordKeyBuilder_BuildExpression_WithShardKey()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var builder = new GenericRecordKeyBuilder(false, ktype);

            // Act
            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 1
            };
            var expression = builder.BuildExpression(keyValues);

            // Assert
            Assert.NotNull(expression);
            Assert.Contains("customer_id", expression);
            Assert.Contains("1", expression);
        }

        [Fact]
        public void GenericRecordKeyBuilder_HasKey_ReturnsTrueForShardedTable()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var builder = new GenericRecordKeyBuilder(false, ktype);

            // Act & Assert
            Assert.True(builder.HasKey());
        }

        [Fact]
        public void GenericRecordKeyBuilder_GetRoutingColumnNames_ReturnsShardKeyColumns()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var builder = new GenericRecordKeyBuilder(false, ktype);

            // Act
            var columnNames = builder.GetRoutingColumnNames();

            // Assert
            Assert.Contains("customer_id", columnNames);
        }

        [Fact]
        public void GenericRecordKeyBuilder_BuildExpression_WithPrimaryKey()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var builder = new GenericRecordKeyBuilder(true, ktype); // Primary key

            // Act
            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 1,
                ["order_id"] = 1001
            };
            var expression = builder.BuildExpression(keyValues);

            // Assert
            Assert.NotNull(expression);
            Assert.Contains("customer_id", expression);
            Assert.Contains("order_id", expression);
        }

        #endregion

        #region GenericRecordRetriever Tests

        [Fact]
        public void GenericRecordRetriever_GetRecordsByKey_SingleCustomer()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 1
            };

            // Act
            var response = retriever.GetRecordsByKey(keyValues);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.total_number_of_records); // Customer 1 has 3 orders
        }

        [Fact]
        public void GenericRecordRetriever_GetRecordsByKey_WithAdditionalExpression()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 1
            };

            // Act - Filter for orders with price > 20
            var response = retriever.GetRecordsByKey(keyValues, "price > 20");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.total_number_of_records); // Widget B (25.50) and Gadget X (99.99)
        }

        [Fact]
        public void GenericRecordRetriever_GetRecordsByKey_WithColumnSelection()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 2
            };

            // Act - Only select specific columns
            var columns = new List<string> { "order_id", "product", "price" };
            var response = retriever.GetRecordsByKey(keyValues, null, columns);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.total_number_of_records); // Customer 2 has 2 orders
        }

        [Fact]
        public void GenericRecordRetriever_GetRecordsByExpression_ReturnsResults()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            // Act - Get all Widget products
            var response = retriever.GetRecordsByExpression("product LIKE 'Widget%'");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(4, response.total_number_of_records); // 4 Widget products
        }

        [Fact]
        public void GenericRecordRetriever_HasAllShardKeyValues_ReturnsTrue()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            var keyValues = new Dictionary<string, object?>
            {
                ["customer_id"] = 1
            };

            // Act & Assert
            Assert.True(retriever.HasAllShardKeyValues(keyValues));
        }

        [Fact]
        public void GenericRecordRetriever_HasAllShardKeyValues_ReturnsFalseWhenMissing()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            var keyValues = new Dictionary<string, object?>
            {
                ["order_id"] = 1001 // Missing customer_id which is the shard key
            };

            // Act & Assert
            Assert.False(retriever.HasAllShardKeyValues(keyValues));
        }

        [Fact]
        public void GenericRecordRetriever_ShardKeyColumnNames_ReturnsCorrectColumns()
        {
            // Arrange
            var ktype = KineticaType.fromTable(_kinetica!, FullTableName);
            var retriever = new GenericRecordRetriever(_kinetica!, FullTableName, ktype);

            // Act
            var shardColumns = retriever.ShardKeyColumnNames;

            // Assert
            Assert.Single(shardColumns);
            Assert.Equal("customer_id", shardColumns[0]);
        }

        #endregion

        #region KI_HINT_KEY_LOOKUP Integration Tests

        [Fact]
        public async Task KeyLookupHint_ParsedCorrectly()
        {
            // Test that KI_HINT_KEY_LOOKUP is properly parsed
            using var cmd = new KineticaCommand(
                $"/*+ KI_HINT_KEY_LOOKUP */ SELECT * FROM {FullTableName} WHERE customer_id = 1",
                _connection!);

            using var reader = await cmd.ExecuteReaderAsync();

            int count = 0;
            while (await reader.ReadAsync())
            {
                count++;
            }

            Assert.Equal(3, count); // Customer 1 has 3 orders
        }

        [Fact]
        public async Task KeyLookup_WithPrimaryKeyFilter_ReturnsCorrectResults()
        {
            // Use full primary key for lookup
            using var cmd = new KineticaCommand(
                $"SELECT product, price FROM {FullTableName} WHERE customer_id = 1 AND order_id = 1001",
                _connection!);

            using var reader = await cmd.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());
            Assert.Equal("Widget A", reader.GetString(0));
            Assert.Equal(10.99, reader.GetDouble(1));
            Assert.False(await reader.ReadAsync()); // Only one result
        }

        #endregion
    }
}
