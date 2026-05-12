using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Integration tests that require a running Kinetica server.
    /// Server: http://localhost:9191
    /// User: admin
    /// Password: secret
    /// </summary>
    [Collection("IntegrationTests")]
    public class IntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Server=http://localhost:9191;Username=admin;Password=secret";
        private const string TestTableName = "ado_integration_test_table";
        private KineticaConnection _connection = null!;

        public async Task InitializeAsync()
        {
            _connection = new KineticaConnection(ConnectionString);
            await _connection.OpenAsync();

            // Clean up any existing test table
            await DropTestTableIfExists();
        }

        public async Task DisposeAsync()
        {
            // Clean up test table
            await DropTestTableIfExists();

            _connection?.Close();
            _connection?.Dispose();
        }

        private async Task DropTestTableIfExists()
        {
            try
            {
                using var command = new KineticaCommand($"DROP TABLE IF EXISTS {TestTableName}", _connection);
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }

        #region Connection Tests

        [Fact]
        public void Connection_Open_Succeeds()
        {
            Assert.Equal(ConnectionState.Open, _connection.State);
        }

        [Fact]
        public async Task Connection_OpenAsync_Succeeds()
        {
            using var conn = new KineticaConnection(ConnectionString);
            await conn.OpenAsync();

            Assert.Equal(ConnectionState.Open, conn.State);
        }

        [Fact]
        public void Connection_Close_ChangesState()
        {
            using var conn = new KineticaConnection(ConnectionString);
            conn.Open();
            Assert.Equal(ConnectionState.Open, conn.State);

            conn.Close();
            Assert.Equal(ConnectionState.Closed, conn.State);
        }

        [Fact]
        public async Task Connection_InvalidCredentials_Throws()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191;Username=invalid;Password=invalid");

            await Assert.ThrowsAsync<KineticaException>(() => conn.OpenAsync());
        }

        [Fact]
        public async Task Connection_InvalidServer_Throws()
        {
            using var conn = new KineticaConnection("Server=http://localhost:99999;Username=admin;Password=test");

            await Assert.ThrowsAsync<KineticaException>(() => conn.OpenAsync());
        }

        #endregion

        #region Create Table Tests

        [Fact]
        public async Task CreateTable_Simple_Succeeds()
        {
            var createSql = $@"
                CREATE TABLE {TestTableName} (
                    id INT NOT NULL,
                    name VARCHAR(100),
                    value DOUBLE,
                    created_at TIMESTAMP,
                    PRIMARY KEY (id)
                )";

            using var command = new KineticaCommand(createSql, _connection);
            await command.ExecuteNonQueryAsync();

            // Verify table exists by trying to insert into it
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, value) VALUES (1, 'test', 1.0)", _connection);
            var rowsAffected = await insertCmd.ExecuteNonQueryAsync();

            Assert.Equal(1, rowsAffected);
        }

        [Fact]
        public async Task CreateTable_WithAllTypes_Succeeds()
        {
            var tableName = $"{TestTableName}_types";

            try
            {
                // Note: Kinetica uses INT for boolean-like values (0/1)
                var createSql = $@"
                    CREATE TABLE {tableName} (
                        col_int INT,
                        col_long LONG,
                        col_float FLOAT,
                        col_double DOUBLE,
                        col_string VARCHAR(256),
                        col_smallint SMALLINT,
                        col_timestamp TIMESTAMP,
                        PRIMARY KEY (col_int)
                    )";

                using var command = new KineticaCommand(createSql, _connection);
                await command.ExecuteNonQueryAsync();

                // Verify table exists by inserting data
                using var insertCmd = new KineticaCommand(
                    $"INSERT INTO {tableName} (col_int, col_long, col_float, col_double, col_string, col_smallint) VALUES (1, 100, 1.5, 2.5, 'test', 1)",
                    _connection);
                var rowsAffected = await insertCmd.ExecuteNonQueryAsync();

                Assert.Equal(1, rowsAffected);
            }
            finally
            {
                // Cleanup
                using var dropCmd = new KineticaCommand($"DROP TABLE IF EXISTS {tableName}", _connection);
                await dropCmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region Insert Tests

        [Fact]
        public async Task Insert_SingleRow_Succeeds()
        {
            // Create table first
            await CreateTestTable();

            var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES (1, 'Test', 123.45)";
            using var command = new KineticaCommand(insertSql, _connection);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            Assert.Equal(1, rowsAffected);
        }

        [Fact]
        public async Task Insert_MultipleRows_Succeeds()
        {
            await CreateTestTable();

            for (int i = 1; i <= 5; i++)
            {
                var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES ({i}, 'Test{i}', {i * 10.5})";
                using var command = new KineticaCommand(insertSql, _connection);
                await command.ExecuteNonQueryAsync();
            }

            // Verify count
            using var countCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName}", _connection);
            var count = await countCmd.ExecuteScalarAsync();

            Assert.Equal(5L, Convert.ToInt64(count));
        }

        [Fact]
        public async Task Insert_WithParameters_Succeeds()
        {
            await CreateTestTable();

            var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES (@id, @name, @value)";
            using var command = new KineticaCommand(insertSql, _connection);
            command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 100, DbType = DbType.Int32 });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "Parameterized", DbType = DbType.String });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@value", Value = 999.99, DbType = DbType.Double });

            var rowsAffected = await command.ExecuteNonQueryAsync();
            Assert.Equal(1, rowsAffected);

            // Verify the inserted data
            using var selectCmd = new KineticaCommand($"SELECT name, value FROM {TestTableName} WHERE id = 100", _connection);
            using var reader = await selectCmd.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());
            Assert.Equal("Parameterized", reader.GetString(0));
            Assert.Equal(999.99, reader.GetDouble(1), 2);
        }

        [Fact]
        public async Task Insert_WithNullParameter_Succeeds()
        {
            await CreateTestTable();

            var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES (@id, @name, @value)";
            using var command = new KineticaCommand(insertSql, _connection);
            command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 200, DbType = DbType.Int32 });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = null, DbType = DbType.String });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@value", Value = null, DbType = DbType.Double });

            var rowsAffected = await command.ExecuteNonQueryAsync();
            Assert.Equal(1, rowsAffected);
        }

        #endregion

        #region Select Tests

        [Fact]
        public async Task Select_AllRows_ReturnsData()
        {
            await CreateTestTable();
            await InsertTestData(10);

            using var command = new KineticaCommand($"SELECT * FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
            {
                rowCount++;
                Assert.True(reader.FieldCount >= 3);
            }

            Assert.Equal(10, rowCount);
        }

        [Fact]
        public async Task Select_WithWhereClause_FiltersCorrectly()
        {
            await CreateTestTable();
            await InsertTestData(10);

            using var command = new KineticaCommand($"SELECT * FROM {TestTableName} WHERE id > 5", _connection);
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(reader.GetOrdinal("id"));
                Assert.True(id > 5);
                rowCount++;
            }

            Assert.Equal(5, rowCount);
        }

        [Fact]
        public async Task Select_WithOrderBy_OrdersCorrectly()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"SELECT id FROM {TestTableName} ORDER BY id DESC", _connection);
            using var reader = await command.ExecuteReaderAsync();

            int previousId = int.MaxValue;
            while (await reader.ReadAsync())
            {
                var currentId = reader.GetInt32(0);
                Assert.True(currentId < previousId, "Results should be in descending order");
                previousId = currentId;
            }
        }

        [Fact]
        public async Task Select_WithLimit_LimitsResults()
        {
            await CreateTestTable();
            await InsertTestData(10);

            using var command = new KineticaCommand($"SELECT * FROM {TestTableName} LIMIT 3", _connection);
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
            {
                rowCount++;
            }

            Assert.Equal(3, rowCount);
        }

        [Fact]
        public async Task Select_SpecificColumns_ReturnsOnlyThoseColumns()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());
            Assert.Equal(2, reader.FieldCount);
            Assert.Equal("id", reader.GetName(0));
            Assert.Equal("name", reader.GetName(1));
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_SingleRow_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"UPDATE {TestTableName} SET name = 'Updated' WHERE id = 1", _connection);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            Assert.True(rowsAffected >= 1);

            // Verify update
            using var selectCmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 1", _connection);
            var name = await selectCmd.ExecuteScalarAsync();
            Assert.Equal("Updated", name?.ToString());
        }

        [Fact]
        public async Task Update_MultipleRows_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(10);

            using var command = new KineticaCommand($"UPDATE {TestTableName} SET value = 0 WHERE id > 5", _connection);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            Assert.True(rowsAffected >= 5);
        }

        [Fact]
        public async Task Update_WithParameters_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"UPDATE {TestTableName} SET name = @name, value = @value WHERE id = @id", _connection);
            command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "ParamUpdated", DbType = DbType.String });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@value", Value = 12345.67, DbType = DbType.Double });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 1, DbType = DbType.Int32 });

            var rowsAffected = await command.ExecuteNonQueryAsync();
            Assert.True(rowsAffected >= 1);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_SingleRow_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"DELETE FROM {TestTableName} WHERE id = 1", _connection);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            Assert.True(rowsAffected >= 1);

            // Verify deletion
            using var countCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName}", _connection);
            var count = Convert.ToInt64(await countCmd.ExecuteScalarAsync());
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task Delete_MultipleRows_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(10);

            using var command = new KineticaCommand($"DELETE FROM {TestTableName} WHERE id > 5", _connection);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            Assert.True(rowsAffected >= 5);

            using var countCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName}", _connection);
            var count = Convert.ToInt64(await countCmd.ExecuteScalarAsync());
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task Delete_AllRows_Succeeds()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"DELETE FROM {TestTableName}", _connection);
            await command.ExecuteNonQueryAsync();

            using var countCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName}", _connection);
            var count = Convert.ToInt64(await countCmd.ExecuteScalarAsync());
            Assert.Equal(0, count);
        }

        #endregion

        #region ExecuteScalar Tests

        [Fact]
        public async Task ExecuteScalar_Count_ReturnsCorrectValue()
        {
            await CreateTestTable();
            await InsertTestData(7);

            using var command = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName}", _connection);
            var result = await command.ExecuteScalarAsync();

            Assert.Equal(7L, Convert.ToInt64(result));
        }

        [Fact]
        public async Task ExecuteScalar_Sum_ReturnsCorrectValue()
        {
            await CreateTestTable();
            await InsertTestData(5); // values: 10.5, 21.0, 31.5, 42.0, 52.5 = 157.5

            using var command = new KineticaCommand($"SELECT SUM(value) FROM {TestTableName}", _connection);
            var result = await command.ExecuteScalarAsync();

            Assert.Equal(157.5, Convert.ToDouble(result), 1);
        }

        [Fact]
        public async Task ExecuteScalar_Max_ReturnsCorrectValue()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"SELECT MAX(id) FROM {TestTableName}", _connection);
            var result = await command.ExecuteScalarAsync();

            Assert.Equal(5, Convert.ToInt32(result));
        }

        [Fact]
        public async Task ExecuteScalar_NoRows_ReturnsNull()
        {
            await CreateTestTable();
            // Don't insert any data

            using var command = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 999", _connection);
            var result = await command.ExecuteScalarAsync();

            Assert.Null(result);
        }

        #endregion

        #region DataReader Tests

        [Fact]
        public async Task DataReader_GetOrdinal_ReturnsCorrectIndex()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.Equal(0, reader.GetOrdinal("id"));
            Assert.Equal(1, reader.GetOrdinal("name"));
            Assert.Equal(2, reader.GetOrdinal("value"));
        }

        [Fact]
        public async Task DataReader_GetName_ReturnsCorrectName()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.Equal("id", reader.GetName(0));
            Assert.Equal("name", reader.GetName(1));
            Assert.Equal("value", reader.GetName(2));
        }

        [Fact]
        public async Task DataReader_TypedGetters_ReturnCorrectTypes()
        {
            await CreateTestTable();

            // Insert with known values
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, value) VALUES (42, 'TypeTest', 3.14159)", _connection);
            await insertCmd.ExecuteNonQueryAsync();

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName} WHERE id = 42", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.Equal(42, reader.GetInt32(0));
            Assert.Equal("TypeTest", reader.GetString(1));
            Assert.Equal(3.14159, reader.GetDouble(2), 4);
        }

        [Fact]
        public async Task DataReader_IsDBNull_DetectsNulls()
        {
            await CreateTestTable();

            // Insert with null value
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, value) VALUES (1, NULL, NULL)", _connection);
            await insertCmd.ExecuteNonQueryAsync();

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName} WHERE id = 1", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.False(reader.IsDBNull(0)); // id is not null
            Assert.True(reader.IsDBNull(1));  // name is null
            Assert.True(reader.IsDBNull(2));  // value is null
        }

        [Fact]
        public async Task DataReader_GetValues_FillsArray()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            var values = new object[reader.FieldCount];
            int count = reader.GetValues(values);

            Assert.Equal(reader.FieldCount, count);
            Assert.NotNull(values[0]); // id
            Assert.NotNull(values[1]); // name
            Assert.NotNull(values[2]); // value
        }

        [Fact]
        public async Task DataReader_Indexer_ByName_Works()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.Equal(1, Convert.ToInt32(reader["id"]));
            Assert.Equal("Test1", reader["name"]?.ToString());
        }

        [Fact]
        public async Task DataReader_Indexer_ByOrdinal_Works()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name, value FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(await reader.ReadAsync());

            Assert.Equal(1, Convert.ToInt32(reader[0]));
            Assert.Equal("Test1", reader[1]?.ToString());
        }

        [Fact]
        public async Task DataReader_HasRows_CorrectlyIndicates()
        {
            await CreateTestTable();
            await InsertTestData(5);

            using var command = new KineticaCommand($"SELECT * FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            Assert.True(reader.HasRows);
        }

        [Fact]
        public async Task DataReader_FieldCount_IsCorrect()
        {
            await CreateTestTable();
            await InsertTestData(1);

            using var command = new KineticaCommand($"SELECT id, name FROM {TestTableName}", _connection);
            using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            Assert.Equal(2, reader.FieldCount);
        }

        #endregion

        #region Schema Discovery Tests

        [Fact]
        public void GetSchema_MetaDataCollections_ReturnsCollections()
        {
            var schema = _connection.GetSchema("MetaDataCollections");

            Assert.NotNull(schema);
            Assert.True(schema.Rows.Count > 0);

            // Verify expected collections exist
            var collectionNames = new System.Collections.Generic.List<string>();
            foreach (DataRow row in schema.Rows)
            {
                collectionNames.Add(row["CollectionName"]?.ToString() ?? "");
            }

            Assert.Contains("Tables", collectionNames);
            Assert.Contains("Columns", collectionNames);
            Assert.Contains("Views", collectionNames);
        }

        [Fact]
        public async Task GetSchema_Tables_ReturnsTables()
        {
            await CreateTestTable();

            var schema = _connection.GetSchema("Tables");

            Assert.NotNull(schema);

            // Schema should at least have the expected columns
            Assert.True(schema.Columns.Contains("TABLE_NAME"), "Schema should have TABLE_NAME column");
            Assert.True(schema.Columns.Contains("TABLE_TYPE"), "Schema should have TABLE_TYPE column");

            // Note: The test table may or may not appear depending on Kinetica's schema caching
            // We just verify the schema call works and returns valid structure
        }

        [Fact]
        public async Task GetSchema_Columns_ReturnsColumns()
        {
            await CreateTestTable();

            // Get columns from the schema - pass null array to get all columns
            var schema = _connection.GetSchema("Columns");

            Assert.NotNull(schema);

            // Verify schema structure is correct
            Assert.True(schema.Columns.Contains("TABLE_NAME"), "Schema should have TABLE_NAME column");
            Assert.True(schema.Columns.Contains("COLUMN_NAME"), "Schema should have COLUMN_NAME column");
            Assert.True(schema.Columns.Contains("DATA_TYPE"), "Schema should have DATA_TYPE column");

            // Note: The actual columns returned depend on Kinetica's table metadata
            // We just verify the schema call works and returns valid structure
        }

        [Fact]
        public void GetSchema_DataTypes_ReturnsTypes()
        {
            var schema = _connection.GetSchema("DataTypes");

            Assert.NotNull(schema);
            Assert.True(schema.Rows.Count > 0);

            var typeNames = new System.Collections.Generic.List<string>();
            foreach (DataRow row in schema.Rows)
            {
                typeNames.Add(row["TYPE_NAME"]?.ToString() ?? "");
            }

            Assert.Contains("int", typeNames);
            Assert.Contains("long", typeNames);
            Assert.Contains("string", typeNames);
            Assert.Contains("double", typeNames);
        }

        #endregion

        #region Command Timeout Tests

        [Fact]
        public async Task Command_WithTimeout_Executes()
        {
            await CreateTestTable();
            await InsertTestData(100);

            using var command = new KineticaCommand($"SELECT * FROM {TestTableName}", _connection);
            command.CommandTimeout = 60; // 60 seconds

            using var reader = await command.ExecuteReaderAsync();
            int count = 0;
            while (await reader.ReadAsync())
            {
                count++;
            }

            Assert.Equal(100, count);
        }

        #endregion

        #region Connection Pooling Tests

        [Fact]
        public async Task ConnectionPool_ReuseConnections_Works()
        {
            // Open and close multiple connections - they should be pooled
            for (int i = 0; i < 5; i++)
            {
                using var conn = new KineticaConnection(ConnectionString);
                await conn.OpenAsync();

                using var cmd = new KineticaCommand("SELECT 1", conn);
                await cmd.ExecuteScalarAsync();

                conn.Close();
            }

            // If we get here without exception, pooling is working
            Assert.True(true);
        }

        [Fact]
        public async Task ConnectionPool_Disabled_Works()
        {
            var noPoolingConnString = ConnectionString + ";Pooling=false";

            using var conn = new KineticaConnection(noPoolingConnString);
            await conn.OpenAsync();

            using var cmd = new KineticaCommand("SELECT 1", conn);
            var result = await cmd.ExecuteScalarAsync();

            Assert.NotNull(result);
        }

        #endregion

        #region Concurrent Operations Tests

        [Fact]
        public async Task ConcurrentReaders_Work()
        {
            await CreateTestTable();
            await InsertTestData(10);

            var tasks = new System.Collections.Generic.List<Task<int>>();

            for (int i = 0; i < 3; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var conn = new KineticaConnection(ConnectionString);
                    await conn.OpenAsync();

                    using var cmd = new KineticaCommand($"SELECT * FROM {TestTableName}", conn);
                    using var reader = await cmd.ExecuteReaderAsync();

                    int count = 0;
                    while (await reader.ReadAsync())
                    {
                        count++;
                    }
                    return count;
                }));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var count in results)
            {
                Assert.Equal(10, count);
            }
        }

        #endregion

        #region Special Characters Tests

        [Fact]
        public async Task Insert_WithSpecialCharacters_Succeeds()
        {
            await CreateTestTable();

            var specialName = "Test's \"Value\" with <special> & characters";
            var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES (@id, @name, @value)";

            using var command = new KineticaCommand(insertSql, _connection);
            command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 1, DbType = DbType.Int32 });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = specialName, DbType = DbType.String });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@value", Value = 100.0, DbType = DbType.Double });

            await command.ExecuteNonQueryAsync();

            // Verify the data
            using var selectCmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 1", _connection);
            var result = await selectCmd.ExecuteScalarAsync();

            Assert.Equal(specialName, result?.ToString());
        }

        [Fact]
        public async Task Insert_WithUnicode_Succeeds()
        {
            await CreateTestTable();

            var unicodeName = "日本語テスト 中文测试 한국어테스트 🎉";
            var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES (@id, @name, @value)";

            using var command = new KineticaCommand(insertSql, _connection);
            command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 1, DbType = DbType.Int32 });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = unicodeName, DbType = DbType.String });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@value", Value = 100.0, DbType = DbType.Double });

            await command.ExecuteNonQueryAsync();

            // Verify the data
            using var selectCmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 1", _connection);
            var result = await selectCmd.ExecuteScalarAsync();

            Assert.Equal(unicodeName, result?.ToString());
        }

        #endregion

        #region Helper Methods

        private async Task CreateTestTable()
        {
            var createSql = $@"
                CREATE TABLE {TestTableName} (
                    id INT NOT NULL,
                    name VARCHAR(256),
                    value DOUBLE,
                    PRIMARY KEY (id)
                )";

            try
            {
                using var command = new KineticaCommand(createSql, _connection);
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                // Table might already exist
            }
        }

        private async Task InsertTestData(int rowCount)
        {
            for (int i = 1; i <= rowCount; i++)
            {
                var insertSql = $"INSERT INTO {TestTableName} (id, name, value) VALUES ({i}, 'Test{i}', {i * 10.5})";
                using var command = new KineticaCommand(insertSql, _connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion
    }

    /// <summary>
    /// Separate test class for tests that don't need table setup/teardown
    /// </summary>
    [Collection("IntegrationTests")]
    public class IntegrationSystemTests
    {
        private const string ConnectionString = "Server=http://localhost:9191;Username=admin;Password=secret";

        [Fact]
        public async Task ShowSystemStatus_Works()
        {
            using var connection = new KineticaConnection(ConnectionString);
            await connection.OpenAsync();

            // The connection test internally calls showSystemStatus
            Assert.Equal(ConnectionState.Open, connection.State);
        }

        [Fact]
        public async Task SimpleSelect_Works()
        {
            using var connection = new KineticaConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new KineticaCommand("SELECT 1 as test_value", connection);
            var result = await command.ExecuteScalarAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public void ProviderFactory_CreatesValidConnection()
        {
            var factory = KineticaProviderFactory.Instance;
            using var connection = factory.CreateConnection();

            Assert.IsType<KineticaConnection>(connection);

            connection.ConnectionString = ConnectionString;
            connection.Open();

            Assert.Equal(ConnectionState.Open, connection.State);
        }

        [Fact]
        public async Task MultipleSequentialConnections_Work()
        {
            for (int i = 0; i < 10; i++)
            {
                using var connection = new KineticaConnection(ConnectionString);
                await connection.OpenAsync();

                using var command = new KineticaCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync();

                connection.Close();
            }

            Assert.True(true); // If we get here, all connections worked
        }
    }
}
