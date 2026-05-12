using System;
using System.Threading.Tasks;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Integration tests for prepared statements, parameters, and new JDBC-compatible features.
    /// These tests require a running Kinetica server.
    /// </summary>
    [Trait("Category", "Integration")]
    public class PreparedStatementIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Server=http://localhost:9191;Username=admin;Password=secret";
        private const string TestTableName = "ado_prepared_stmt_test";
        private KineticaConnection? _connection;

        public async Task InitializeAsync()
        {
            _connection = new KineticaConnection(ConnectionString);
            await _connection.OpenAsync();

            // Create test table
            using var dropCmd = new KineticaCommand($"DROP TABLE IF EXISTS {TestTableName}", _connection);
            await dropCmd.ExecuteNonQueryAsync();

            using var createCmd = new KineticaCommand($@"
                CREATE TABLE {TestTableName} (
                    id INT NOT NULL,
                    name VARCHAR(64),
                    score DOUBLE,
                    active BOOLEAN,
                    created TIMESTAMP,
                    PRIMARY KEY (id)
                )", _connection);
            await createCmd.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                try
                {
                    using var cmd = new KineticaCommand($"DROP TABLE IF EXISTS {TestTableName}", _connection);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch { }

                _connection.Close();
                _connection.Dispose();
            }
        }

        #region Positional Parameter Tests (?)

        [Fact]
        public async Task PositionalParameters_Insert_SingleRow()
        {
            using var cmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (?, ?, ?)", _connection!);
            cmd.Parameters.Add(new KineticaParameter { Value = 1 });
            cmd.Parameters.Add(new KineticaParameter { Value = "Alice", DbType = System.Data.DbType.String });
            cmd.Parameters.Add(new KineticaParameter { Value = 95.5, DbType = System.Data.DbType.Double });

            var affected = await cmd.ExecuteNonQueryAsync();

            Assert.Equal(1, affected);

            // Verify
            using var selectCmd = new KineticaCommand($"SELECT name, score FROM {TestTableName} WHERE id = 1", _connection);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            Assert.Equal("Alice", reader.GetString(0));
            Assert.Equal(95.5, reader.GetDouble(1));
        }

        [Fact]
        public async Task PositionalParameters_Select_WithWhere()
        {
            // Insert test data first
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (10, 'Bob', 80.0)", _connection!);
            await insertCmd.ExecuteNonQueryAsync();

            // Select with positional parameter
            using var cmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = ?", _connection);
            cmd.Parameters.Add(new KineticaParameter { Value = 10 });

            using var reader = await cmd.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            Assert.Equal("Bob", reader.GetString(0));
        }

        [Fact]
        public async Task PositionalParameters_Update()
        {
            // Insert test data
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (20, 'Charlie', 70.0)", _connection!);
            await insertCmd.ExecuteNonQueryAsync();

            // Update with positional parameters
            using var updateCmd = new KineticaCommand($"UPDATE {TestTableName} SET score = ? WHERE id = ?", _connection);
            updateCmd.Parameters.Add(new KineticaParameter { Value = 85.0, DbType = System.Data.DbType.Double });
            updateCmd.Parameters.Add(new KineticaParameter { Value = 20 });

            var affected = await updateCmd.ExecuteNonQueryAsync();
            Assert.Equal(1, affected);

            // Verify
            using var selectCmd = new KineticaCommand($"SELECT score FROM {TestTableName} WHERE id = 20", _connection);
            var score = await selectCmd.ExecuteScalarAsync();
            Assert.Equal(85.0, Convert.ToDouble(score));
        }

        [Fact]
        public async Task PositionalParameters_Delete()
        {
            // Insert test data
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name) VALUES (30, 'ToDelete')", _connection!);
            await insertCmd.ExecuteNonQueryAsync();

            // Delete with positional parameter
            using var deleteCmd = new KineticaCommand($"DELETE FROM {TestTableName} WHERE id = ?", _connection);
            deleteCmd.Parameters.Add(new KineticaParameter { Value = 30 });

            var affected = await deleteCmd.ExecuteNonQueryAsync();
            Assert.Equal(1, affected);

            // Verify deleted
            using var selectCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName} WHERE id = 30", _connection);
            var count = await selectCmd.ExecuteScalarAsync();
            Assert.Equal(0L, Convert.ToInt64(count));
        }

        #endregion

        #region Named Parameter Tests (@param)

        [Fact]
        public async Task NamedParameters_Insert()
        {
            using var cmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (@id, @name, @score)", _connection!);
            cmd.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 100 });
            cmd.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "NamedTest", DbType = System.Data.DbType.String });
            cmd.Parameters.Add(new KineticaParameter { ParameterName = "@score", Value = 99.9, DbType = System.Data.DbType.Double });

            var affected = await cmd.ExecuteNonQueryAsync();
            Assert.Equal(1, affected);

            // Verify
            using var selectCmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 100", _connection);
            var name = await selectCmd.ExecuteScalarAsync();
            Assert.Equal("NamedTest", name);
        }

        [Fact]
        public async Task NamedParameters_Select_WithMultipleConditions()
        {
            // Insert test data
            using var insertCmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (110, 'MultiCondition', 75.0)", _connection!);
            await insertCmd.ExecuteNonQueryAsync();

            using var cmd = new KineticaCommand($"SELECT id FROM {TestTableName} WHERE name = @name AND score >= @minScore", _connection);
            cmd.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "MultiCondition", DbType = System.Data.DbType.String });
            cmd.Parameters.Add(new KineticaParameter { ParameterName = "@minScore", Value = 70.0, DbType = System.Data.DbType.Double });

            var id = await cmd.ExecuteScalarAsync();
            Assert.Equal(110L, Convert.ToInt64(id));
        }

        #endregion

        #region Multi-Statement Execution Tests

        [Fact]
        public async Task MultiStatement_InsertMultipleRows()
        {
            using var cmd = new KineticaCommand($@"
                INSERT INTO {TestTableName} (id, name, score) VALUES (200, 'Multi1', 60.0);
                INSERT INTO {TestTableName} (id, name, score) VALUES (201, 'Multi2', 65.0);
                INSERT INTO {TestTableName} (id, name, score) VALUES (202, 'Multi3', 70.0)
            ", _connection!);

            var affected = await cmd.ExecuteNonQueryAsync();
            Assert.Equal(3, affected);

            // Verify
            using var countCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName} WHERE id >= 200 AND id <= 202", _connection);
            var count = await countCmd.ExecuteScalarAsync();
            Assert.Equal(3L, Convert.ToInt64(count));
        }

        [Fact]
        public async Task MultiStatement_MixedOperations()
        {
            // Insert, then update in one command
            using var cmd = new KineticaCommand($@"
                INSERT INTO {TestTableName} (id, name, score) VALUES (300, 'MixedOp', 50.0);
                UPDATE {TestTableName} SET score = 55.0 WHERE id = 300
            ", _connection!);

            var affected = await cmd.ExecuteNonQueryAsync();
            Assert.True(affected >= 1); // At least the insert

            // Verify the update took effect
            using var selectCmd = new KineticaCommand($"SELECT score FROM {TestTableName} WHERE id = 300", _connection);
            var score = await selectCmd.ExecuteScalarAsync();
            Assert.Equal(55.0, Convert.ToDouble(score));
        }

        #endregion

        #region SET SCHEMA Tests

        [Fact]
        public async Task SetSchema_UpdatesConnectionSchema()
        {
            using var cmd = new KineticaCommand("SET SCHEMA test_schema", _connection!);
            await cmd.ExecuteNonQueryAsync();

            Assert.Equal("test_schema", _connection!.CurrentSchema);
        }

        [Fact]
        public async Task SetSqlId_UpdatesConnectionSchema()
        {
            using var cmd = new KineticaCommand("SET SQLID another_schema", _connection!);
            await cmd.ExecuteNonQueryAsync();

            Assert.Equal("another_schema", _connection!.CurrentSchema);
        }

        #endregion

        #region User Impersonation Tests

        [Fact]
        public async Task SetUser_UpdatesImpersonatedUser()
        {
            using var cmd = new KineticaCommand("SET USER testuser", _connection!);
            await cmd.ExecuteNonQueryAsync();

            Assert.Equal("testuser", _connection!.ImpersonatedUser);
        }

        [Fact]
        public async Task ExecuteAsUser_ThenRevert()
        {
            using var setCmd = new KineticaCommand("SET USER user1", _connection!);
            await setCmd.ExecuteNonQueryAsync();

            using var execCmd = new KineticaCommand("EXECUTE AS USER user2", _connection!);
            await execCmd.ExecuteNonQueryAsync();
            Assert.Equal("user2", _connection!.ImpersonatedUser);

            using var revertCmd = new KineticaCommand("REVERT", _connection!);
            await revertCmd.ExecuteNonQueryAsync();
            Assert.Equal("user1", _connection!.ImpersonatedUser);
        }

        #endregion

        #region Data Type Tests

        [Fact]
        public async Task Parameters_BooleanType()
        {
            // Insert with boolean parameter (gets converted to TRUE/FALSE SQL literal)
            using var cmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, active) VALUES (?, ?, ?)", _connection!);
            cmd.Parameters.Add(new KineticaParameter { Value = 400 });
            cmd.Parameters.Add(new KineticaParameter { Value = "BoolTest", DbType = System.Data.DbType.String });
            cmd.Parameters.Add(new KineticaParameter { Value = true, DbType = System.Data.DbType.Boolean });

            await cmd.ExecuteNonQueryAsync();

            // Verify via count (avoid reading boolean column directly due to Kinetica client limitation)
            using var selectCmd = new KineticaCommand($"SELECT COUNT(*) FROM {TestTableName} WHERE id = 400 AND active = TRUE", _connection);
            var count = await selectCmd.ExecuteScalarAsync();
            Assert.Equal(1L, Convert.ToInt64(count));
        }

        [Fact]
        public async Task Parameters_NullValue()
        {
            using var cmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name, score) VALUES (?, ?, ?)", _connection!);
            cmd.Parameters.Add(new KineticaParameter { Value = 500 });
            cmd.Parameters.Add(new KineticaParameter { Value = "NullScoreTest", DbType = System.Data.DbType.String });
            cmd.Parameters.Add(new KineticaParameter { Value = null, DbType = System.Data.DbType.Double });

            await cmd.ExecuteNonQueryAsync();

            using var selectCmd = new KineticaCommand($"SELECT score FROM {TestTableName} WHERE id = 500", _connection);
            using var reader = await selectCmd.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            Assert.True(reader.IsDBNull(0));
        }

        [Fact]
        public async Task Parameters_StringWithSpecialCharacters()
        {
            using var cmd = new KineticaCommand($"INSERT INTO {TestTableName} (id, name) VALUES (?, ?)", _connection!);
            cmd.Parameters.Add(new KineticaParameter { Value = 600 });
            cmd.Parameters.Add(new KineticaParameter { Value = "It's a \"test\" with special chars: <>&", DbType = System.Data.DbType.String });

            await cmd.ExecuteNonQueryAsync();

            using var selectCmd = new KineticaCommand($"SELECT name FROM {TestTableName} WHERE id = 600", _connection);
            var name = await selectCmd.ExecuteScalarAsync();
            Assert.Equal("It's a \"test\" with special chars: <>&", name);
        }

        #endregion
    }
}
