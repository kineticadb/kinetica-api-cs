using System;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void KineticaException_WithMessage()
        {
            var ex = new KineticaException("Test error message");

            Assert.Equal("Test error message", ex.Message);
            Assert.Equal(-1, ex.ErrorCode);
            Assert.Equal("HY000", ex.SqlState);
        }

        [Fact]
        public void KineticaException_WithInnerException()
        {
            var inner = new InvalidOperationException("Inner error");
            var ex = new KineticaException("Outer error", inner);

            Assert.Equal("Outer error", ex.Message);
            Assert.Equal(inner, ex.InnerException);
            Assert.Equal(-5, ex.ErrorCode); // InvalidOperationException maps to -5
            Assert.Equal("24000", ex.SqlState); // Invalid cursor state
        }

        [Fact]
        public void KineticaException_WithTimeoutException_MapsSqlState()
        {
            var inner = new TimeoutException("Connection timed out");
            var ex = new KineticaException("Operation failed", inner);

            Assert.Equal(-2, ex.ErrorCode);
            Assert.Equal("HYT00", ex.SqlState); // Timeout expired
        }

        [Fact]
        public void KineticaException_WithUnauthorizedAccessException_MapsSqlState()
        {
            var inner = new UnauthorizedAccessException("Access denied");
            var ex = new KineticaException("Operation failed", inner);

            Assert.Equal(-3, ex.ErrorCode);
            Assert.Equal("28000", ex.SqlState); // Invalid authorization specification
        }

        [Fact]
        public void KineticaException_WithArgumentException_MapsSqlState()
        {
            var inner = new ArgumentException("Invalid argument");
            var ex = new KineticaException("Operation failed", inner);

            Assert.Equal(-4, ex.ErrorCode);
            Assert.Equal("22000", ex.SqlState); // Data exception
        }

        [Fact]
        public void KineticaException_WithErrorCodeAndSqlState()
        {
            var ex = new KineticaException("Custom error", 42, "42S02");

            Assert.Equal("Custom error", ex.Message);
            Assert.Equal(42, ex.ErrorCode);
            Assert.Equal("42S02", ex.SqlState);
        }

        [Fact]
        public void KineticaConnectionException_DefaultSqlState()
        {
            var ex = new KineticaConnectionException("Connection failed");

            Assert.Equal("Connection failed", ex.Message);
            Assert.Equal(-100, ex.ErrorCode);
            Assert.Equal("08000", ex.SqlState); // Connection exception
        }

        [Fact]
        public void KineticaSqlException_DefaultSqlState()
        {
            var ex = new KineticaSqlException("SQL syntax error");

            Assert.Equal("SQL syntax error", ex.Message);
            Assert.Equal(-200, ex.ErrorCode);
            Assert.Equal("42000", ex.SqlState); // Syntax error or access rule violation
        }
    }
}
