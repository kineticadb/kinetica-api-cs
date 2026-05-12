using System;
using System.Data;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class CommandTests
    {
        [Fact]
        public void KineticaCommand_DefaultCommandType_IsText()
        {
            var command = new KineticaCommand();

            Assert.Equal(CommandType.Text, command.CommandType);
        }

        [Fact]
        public void KineticaCommand_DefaultTimeout_Is30()
        {
            var command = new KineticaCommand();

            Assert.Equal(30, command.CommandTimeout);
        }

        [Fact]
        public void KineticaCommand_SetCommandText()
        {
            var command = new KineticaCommand
            {
                CommandText = "SELECT * FROM users"
            };

            Assert.Equal("SELECT * FROM users", command.CommandText);
        }

        [Fact]
        public void KineticaCommand_SetTimeout()
        {
            var command = new KineticaCommand
            {
                CommandTimeout = 60
            };

            Assert.Equal(60, command.CommandTimeout);
        }

        [Fact]
        public void KineticaCommand_CreateParameter_ReturnsKineticaParameter()
        {
            var command = new KineticaCommand();
            var param = command.CreateParameter();

            Assert.IsType<KineticaParameter>(param);
        }

        [Fact]
        public void KineticaCommand_Parameters_IsNotNull()
        {
            var command = new KineticaCommand();

            Assert.NotNull(command.Parameters);
        }

        [Fact]
        public void KineticaCommand_ConstructorWithConnection()
        {
            using var connection = new KineticaConnection("Server=localhost:9191");
            var command = new KineticaCommand(connection);

            Assert.Same(connection, command.Connection);
        }

        [Fact]
        public void KineticaCommand_ConstructorWithCommandTextAndConnection()
        {
            using var connection = new KineticaConnection("Server=localhost:9191");
            var command = new KineticaCommand("SELECT 1", connection);

            Assert.Equal("SELECT 1", command.CommandText);
            Assert.Same(connection, command.Connection);
        }

        [Fact]
        public void KineticaCommand_ExecuteNonQuery_WithoutConnection_Throws()
        {
            var command = new KineticaCommand
            {
                CommandText = "SELECT 1"
            };

            Assert.Throws<InvalidOperationException>(() => command.ExecuteNonQuery());
        }

        [Fact]
        public void KineticaCommand_ExecuteNonQuery_WithoutCommandText_Throws()
        {
            using var connection = new KineticaConnection("Server=localhost:9191");
            var command = new KineticaCommand(connection);

            Assert.Throws<InvalidOperationException>(() => command.ExecuteNonQuery());
        }

        [Fact]
        public void KineticaCommand_Prepare_WithoutCommandText_Throws()
        {
            using var connection = new KineticaConnection("Server=localhost:9191");
            var command = new KineticaCommand(connection);

            Assert.Throws<InvalidOperationException>(() => command.Prepare());
        }

        [Fact]
        public void KineticaCommand_SetConnection()
        {
            using var connection = new KineticaConnection("Server=localhost:9191");
            var command = new KineticaCommand();

            command.Connection = connection;

            Assert.Same(connection, command.Connection);
        }

        [Fact]
        public void KineticaCommand_Cancel_DoesNotThrowWhenNotRunning()
        {
            var command = new KineticaCommand
            {
                CommandText = "SELECT 1"
            };

            // Should not throw even if nothing is running
            command.Cancel();
        }

        [Fact]
        public void KineticaCommand_AddParameter_IncreasesCount()
        {
            var command = new KineticaCommand();

            command.Parameters.Add(new KineticaParameter { ParameterName = "@p1", Value = 1 });
            command.Parameters.Add(new KineticaParameter { ParameterName = "@p2", Value = "test" });

            Assert.Equal(2, command.Parameters.Count);
        }

        [Fact]
        public void KineticaCommand_Dispose_CanBeCalledMultipleTimes()
        {
            var command = new KineticaCommand();

            command.Dispose();
            command.Dispose(); // Should not throw
        }
    }
}
