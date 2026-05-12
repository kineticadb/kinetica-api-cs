using System;
using System.Data;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void KineticaConnection_DefaultState_IsClosed()
        {
            using var conn = new KineticaConnection();

            Assert.Equal(ConnectionState.Closed, conn.State);
        }

        [Fact]
        public void KineticaConnection_SetConnectionString()
        {
            using var conn = new KineticaConnection();
            conn.ConnectionString = "Server=localhost:9191;Username=admin;Password=secret";

            Assert.Equal("Server=localhost:9191;Username=admin;Password=secret", conn.ConnectionString);
        }

        [Fact]
        public void KineticaConnection_ConstructorWithConnectionString()
        {
            using var conn = new KineticaConnection("Server=myserver:9191");

            Assert.Contains("myserver:9191", conn.ConnectionString);
        }

        [Fact]
        public void KineticaConnection_DataSource_ReturnsServer()
        {
            using var conn = new KineticaConnection("Server=testserver:9191");

            Assert.Equal("testserver:9191", conn.DataSource);
        }

        [Fact]
        public void KineticaConnection_CreateCommand_ReturnsKineticaCommand()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            var command = conn.CreateCommand();

            Assert.IsType<KineticaCommand>(command);
        }

        [Fact]
        public void KineticaConnection_BeginTransaction_ReturnsKineticaTransaction()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");

            // BeginTransaction on a closed connection may either:
            // 1. Return a transaction (Kinetica pseudo-transactions)
            // 2. Throw an exception
            // The actual behavior depends on implementation
            var transaction = conn.BeginTransaction();
            Assert.IsType<KineticaTransaction>(transaction);
        }

        [Fact]
        public void KineticaConnection_ChangeDatabase_UpdatesDatabase()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            conn.ChangeDatabase("new_database");

            Assert.Equal("new_database", conn.Database);
        }

        [Fact]
        public void KineticaConnection_Close_WhenAlreadyClosed_DoesNothing()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");

            // Should not throw
            conn.Close();

            Assert.Equal(ConnectionState.Closed, conn.State);
        }

        [Fact]
        public void KineticaConnection_ConnectionStringBuilder_ParsesPooling()
        {
            using var conn = new KineticaConnection("Server=localhost:9191;Pooling=false");

            // Connection should be configured for non-pooled operation
            Assert.Contains("Pooling=false", conn.ConnectionString, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void KineticaConnection_EmptyConnectionString_UsesDefaults()
        {
            using var conn = new KineticaConnection();
            conn.ConnectionString = "";

            // When empty string is set, the builder may use default values
            // or remain empty depending on implementation
            // The default server is 127.0.0.1:9191
            Assert.Equal("http://127.0.0.1:9191", conn.DataSource);
        }
    }
}
