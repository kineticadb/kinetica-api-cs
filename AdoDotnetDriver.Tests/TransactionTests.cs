using System;
using System.Data;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Tests for KineticaTransaction API compatibility.
    ///
    /// IMPORTANT: Kinetica does not support transactions. The KineticaTransaction class
    /// exists solely for ADO.NET API compatibility. These tests verify:
    /// - The API contract is maintained (methods exist, proper exceptions thrown)
    /// - State management works correctly (completed state tracking)
    ///
    /// These tests do NOT verify transactional behavior because Kinetica has none:
    /// - Rollback() is a no-op (does not undo any commands)
    /// - Commit() only flushes batch inserts (does not provide atomicity)
    /// - IsolationLevel is stored but has no effect
    /// </summary>
    public class TransactionTests
    {
        [Fact]
        public void KineticaTransaction_IsolationLevel_IsStoredForApiCompatibility()
        {
            // Note: IsolationLevel is stored but has no effect - Kinetica doesn't support isolation levels
            using var conn = new KineticaConnection("Server=localhost:9191");
            var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
        }

        [Fact]
        public void KineticaTransaction_Connection_IsSetCorrectly()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            Assert.Same(conn, transaction.Connection);
        }

        [Fact]
        public void KineticaTransaction_Rollback_MarksTransactionCompleted()
        {
            // Note: Rollback() is a no-op in terms of data - it only marks the transaction as completed
            using var conn = new KineticaConnection("Server=localhost:9191");
            using var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            transaction.Rollback();

            // Second rollback should throw because transaction is already completed
            Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
        }

        [Fact]
        public void KineticaTransaction_Commit_AfterRollback_Throws()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            using var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            transaction.Rollback();

            Assert.Throws<InvalidOperationException>(() => transaction.Commit());
        }

        [Fact]
        public void KineticaTransaction_Dispose_MarksTransactionCompleted()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            // Should not throw during dispose
            transaction.Dispose();

            // After dispose, should not be able to commit (transaction is completed)
            Assert.Throws<InvalidOperationException>(() => transaction.Commit());
        }

        [Fact]
        public void KineticaTransaction_MultipleDispose_DoesNotThrow()
        {
            using var conn = new KineticaConnection("Server=localhost:9191");
            var transaction = new KineticaTransaction(conn, IsolationLevel.ReadCommitted);

            transaction.Dispose();
            transaction.Dispose(); // Should not throw - idempotent
        }
    }
}
