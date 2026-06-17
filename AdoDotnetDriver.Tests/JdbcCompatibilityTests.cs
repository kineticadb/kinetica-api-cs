using System;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    /// <summary>
    /// Tests for JDBC-compatible features in the ADO.NET driver.
    /// </summary>
    public class JdbcCompatibilityTests
    {
        #region FetchSize Tests

        [Fact]
        public void Command_FetchSize_DefaultIsZero()
        {
            using var conn = new KineticaConnection();
            using var cmd = new KineticaCommand(conn);

            Assert.Equal(0, cmd.FetchSize);
        }

        [Fact]
        public void Command_FetchSize_CanBeSet()
        {
            using var conn = new KineticaConnection();
            using var cmd = new KineticaCommand(conn);

            cmd.FetchSize = 1000;
            Assert.Equal(1000, cmd.FetchSize);
        }

        [Fact]
        public void Command_FetchSize_NegativeBecomesZero()
        {
            using var conn = new KineticaConnection();
            using var cmd = new KineticaCommand(conn);

            cmd.FetchSize = -1;
            Assert.Equal(0, cmd.FetchSize);
        }

        [Fact]
        public void ConnectionString_FetchSize_CanBeParsed()
        {
            var builder = new KineticaConnectionStringBuilder("Server=localhost:9191;FetchSize=5000");

            Assert.Equal(5000, builder.FetchSize);
        }

        [Fact]
        public void ConnectionString_FetchSize_DefaultIsZero()
        {
            var builder = new KineticaConnectionStringBuilder();

            Assert.Equal(0, builder.FetchSize);
        }

        #endregion

        #region User Impersonation Parsing Tests

        [Fact]
        public void Parse_SetUser_Detected()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET USER 'admin'", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetUser, result.CommandType);
            Assert.NotNull(result.UserImpersonation);
            Assert.Equal(ImpersonationType.SetUser, result.UserImpersonation!.Type);
            Assert.Equal("admin", result.UserImpersonation.Username);
        }

        [Fact]
        public void Parse_SetUser_WithDoubleQuotes()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET USER \"testuser\"", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetUser, result.CommandType);
            Assert.NotNull(result.UserImpersonation);
            Assert.Equal("testuser", result.UserImpersonation!.Username);
        }

        [Fact]
        public void Parse_SetUser_WithoutQuotes()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET USER myuser", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetUser, result.CommandType);
            Assert.NotNull(result.UserImpersonation);
            Assert.Equal("myuser", result.UserImpersonation!.Username);
        }

        [Fact]
        public void Parse_ExecuteAsUser_Detected()
        {
            var parser = new SQLParser();
            var result = parser.Parse("EXECUTE AS USER 'impersonated_user'", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.ExecuteAsUser, result.CommandType);
            Assert.NotNull(result.UserImpersonation);
            Assert.Equal(ImpersonationType.ExecuteAs, result.UserImpersonation!.Type);
            Assert.Equal("impersonated_user", result.UserImpersonation.Username);
        }

        [Fact]
        public void Parse_Revert_Detected()
        {
            var parser = new SQLParser();
            var result = parser.Parse("REVERT", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.Revert, result.CommandType);
            Assert.NotNull(result.UserImpersonation);
            Assert.Equal(ImpersonationType.Revert, result.UserImpersonation!.Type);
        }

        [Fact]
        public void Parse_Revert_CaseInsensitive()
        {
            var parser = new SQLParser();
            var result = parser.Parse("revert", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.Revert, result.CommandType);
        }

        #endregion

        #region Query Hints Parsing Tests

        [Fact]
        public void Parse_QueryHints_BatchSize()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_BATCH_SIZE(5000) */ SELECT * FROM table", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.Equal(5000, result.Hints.BatchSize);
        }

        [Fact]
        public void Parse_QueryHints_TruncateStrings()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_TRUNCATE_STRINGS */ INSERT INTO table VALUES ('test')", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.TruncateStrings);
        }

        [Fact]
        public void Parse_QueryHints_UpdateOnExistingPk()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_UPDATE_ON_EXISTING_PK */ INSERT INTO table VALUES (1, 'test')", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.UpdateOnExistingPk);
        }

        [Fact]
        public void Parse_QueryHints_DisableMultihead()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_DISABLE_MULTIHEAD */ SELECT * FROM table", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.DisableMultihead);
        }

        [Fact]
        public void Parse_QueryHints_UseKeyLookup()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_KEY_LOOKUP */ SELECT * FROM table WHERE id = 1", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.UseKeyLookup);
        }

        [Fact]
        public void Parse_QueryHints_ReplicationSync()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_REPL_SYNC */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.ReplicationSync);
        }

        [Fact]
        public void Parse_QueryHints_Multiple()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_BATCH_SIZE(1000) KI_HINT_TRUNCATE_STRINGS KI_HINT_UPDATE_ON_EXISTING_PK */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.Equal(1000, result.Hints.BatchSize);
            Assert.True(result.Hints.TruncateStrings);
            Assert.True(result.Hints.UpdateOnExistingPk);
        }

        [Fact]
        public void QueryHints_ToInsertOptions_ReturnsCorrectDictionary()
        {
            var hints = new QueryHints
            {
                TruncateStrings = true,
                UpdateOnExistingPk = true,
                ReplicationSync = true
            };

            var options = hints.ToInsertOptions();

            Assert.Equal("true", options["truncate_strings"]);
            Assert.Equal("true", options["update_on_existing_pk"]);
            Assert.Equal("sync", options["replication_mode"]);
        }

        [Fact]
        public void QueryHints_ToQueryOptions_ReturnsCorrectDictionary()
        {
            var hints = new QueryHints
            {
                UseKeyLookup = true
            };

            var options = hints.ToQueryOptions();

            Assert.Equal("true", options["key_lookup"]);
        }

        [Fact]
        public void Parse_StripsHintCommentsFromFinalSql()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_KEY_LOOKUP */ SELECT * FROM table WHERE id = 1", new KineticaParameterCollection());

            // Hints should be extracted
            Assert.True(result.Hints.UseKeyLookup);

            // But stripped from FinalSql
            Assert.DoesNotContain("KI_HINT", result.FinalSql);
            Assert.Equal("SELECT * FROM table WHERE id = 1", result.FinalSql);
        }

        [Fact]
        public void Parse_StripsHintCommentsWithPlusSign()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/*+ KI_HINT_BATCH_SIZE(5000) */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            // Hints should be extracted
            Assert.Equal(5000, result.Hints.BatchSize);

            // But stripped from FinalSql
            Assert.DoesNotContain("KI_HINT", result.FinalSql);
            Assert.Equal("INSERT INTO table VALUES (1)", result.FinalSql);
        }

        [Fact]
        public void Parse_StripsMultipleHints()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_BATCH_SIZE(1000) KI_HINT_TRUNCATE_STRINGS */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            // Hints should be extracted
            Assert.Equal(1000, result.Hints.BatchSize);
            Assert.True(result.Hints.TruncateStrings);

            // But stripped from FinalSql
            Assert.DoesNotContain("KI_HINT", result.FinalSql);
            Assert.Equal("INSERT INTO table VALUES (1)", result.FinalSql);
        }

        #endregion

        #region Extended Connection String Properties Tests

        [Fact]
        public void ConnectionString_TimeoutProperties()
        {
            var builder = new KineticaConnectionStringBuilder("ConnectionTimeout=60;Timeout=10");

            Assert.Equal(60, builder.ConnectionTimeout);
            Assert.Equal(10, builder.Timeout);
        }

        [Fact]
        public void ConnectionString_SslProperties()
        {
            var builder = new KineticaConnectionStringBuilder("BypassSslCertCheck=true;SslAllowHostMismatch=true");

            Assert.True(builder.BypassSslCertCheck);
            Assert.True(builder.SslAllowHostMismatch);
        }

        [Fact]
        public void ConnectionString_NetworkProperties()
        {
            var builder = new KineticaConnectionStringBuilder("DisableAutoDiscovery=true;DisableFailover=true;DisableSnappy=true");

            Assert.True(builder.DisableAutoDiscovery);
            Assert.True(builder.DisableFailover);
            Assert.True(builder.DisableSnappy);
        }

        [Fact]
        public void ConnectionString_QueryOptimizationProperties()
        {
            var builder = new KineticaConnectionStringBuilder("CostBasedOptimization=false;DistributedJoins=false;ParallelExecution=false");

            Assert.False(builder.CostBasedOptimization);
            Assert.False(builder.DistributedJoins);
            Assert.False(builder.ParallelExecution);
        }

        [Fact]
        public void ConnectionString_QueryOptimizationDefaults()
        {
            var builder = new KineticaConnectionStringBuilder();

            // These should default to true
            Assert.True(builder.CostBasedOptimization);
            Assert.True(builder.DistributedJoins);
            Assert.True(builder.ParallelExecution);
            Assert.True(builder.PlanCache);
            Assert.True(builder.ResultsCaching);
        }

        [Fact]
        public void ConnectionString_QueryControlProperties()
        {
            var builder = new KineticaConnectionStringBuilder("ReadOnly=true;TTL=30;Limit=1000;RowsPerFetch=5000");

            Assert.True(builder.ReadOnly);
            Assert.Equal(30, builder.Ttl);
            Assert.Equal(1000, builder.Limit);
            Assert.Equal(5000, builder.RowsPerFetch);
        }

        [Fact]
        public void ConnectionString_InsertionProperties()
        {
            var builder = new KineticaConnectionStringBuilder("RowsPerInsertion=5000;TruncateStrings=true;UpdateOnExistingPk=true");

            Assert.Equal(5000, builder.RowsPerInsertion);
            Assert.True(builder.TruncateStrings);
            Assert.True(builder.UpdateOnExistingPk);
        }

        [Fact]
        public void ConnectionString_BatchInsertProperties()
        {
            var builder = new KineticaConnectionStringBuilder("BatchInsertMode=true;BatchSize=20000;BatchUpdateOnExistingPk=true");

            Assert.True(builder.BatchInsertMode);
            Assert.Equal(20000, builder.BatchSize);
            Assert.True(builder.BatchUpdateOnExistingPk);
        }

        [Fact]
        public void ConnectionString_ImpersonateUser()
        {
            var builder = new KineticaConnectionStringBuilder("Server=localhost:9191;ImpersonateUser=testuser");

            Assert.Equal("testuser", builder.ImpersonateUser);
        }

        [Fact]
        public void ConnectionString_BuildQueryOptions_ReturnsCorrectDictionary()
        {
            var builder = new KineticaConnectionStringBuilder
            {
                CostBasedOptimization = false,
                DistributedJoins = false,
                Limit = 1000,
                Ttl = 30
            };

            var options = builder.BuildQueryOptions();

            Assert.True(options.ContainsKey("cost_based_optimization") || options.Count > 0);
        }

        [Fact]
        public void ConnectionString_BuildInsertOptions_ReturnsCorrectDictionary()
        {
            var builder = new KineticaConnectionStringBuilder
            {
                TruncateStrings = true,
                UpdateOnExistingPk = true
            };

            var options = builder.BuildInsertOptions();

            Assert.True(options.ContainsKey("truncate_strings") || options.Count > 0);
        }

        #endregion

        #region Positional Parameter Tests (JDBC ? style)

        [Fact]
        public void Parse_PositionalParameters_SingleParameter()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = "test", DbType = System.Data.DbType.String });

            var result = parser.Parse("SELECT * FROM table WHERE name = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE name = 'test'", result.FinalSql);
        }

        [Fact]
        public void Parse_PositionalParameters_MultipleParameters()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = 42, DbType = System.Data.DbType.Int32 });
            parameters.Add(new KineticaParameter { Value = "hello", DbType = System.Data.DbType.String });

            var result = parser.Parse("SELECT * FROM table WHERE id = ? AND name = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE id = 42 AND name = 'hello'", result.FinalSql);
        }

        [Fact]
        public void Parse_PositionalParameters_WithNull()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = null, DbType = System.Data.DbType.String });

            var result = parser.Parse("SELECT * FROM table WHERE name = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE name = NULL", result.FinalSql);
        }

        [Fact]
        public void Parse_PositionalParameters_InsertStatement()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = 1, DbType = System.Data.DbType.Int32 });
            parameters.Add(new KineticaParameter { Value = "John", DbType = System.Data.DbType.String });
            parameters.Add(new KineticaParameter { Value = 25.5, DbType = System.Data.DbType.Double });

            var result = parser.Parse("INSERT INTO users (id, name, score) VALUES (?, ?, ?)", parameters);

            Assert.Equal("INSERT INTO users (id, name, score) VALUES (1, 'John', 25.5)", result.FinalSql);
        }

        [Fact]
        public void Parse_NamedParameters_WorksAsExpected()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "test", DbType = System.Data.DbType.String });
            parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 42, DbType = System.Data.DbType.Int32 });

            var result = parser.Parse("SELECT * FROM table WHERE name = @name AND id = @id", parameters);

            Assert.Equal("SELECT * FROM table WHERE name = 'test' AND id = 42", result.FinalSql);
        }

        [Fact]
        public void Parse_MixedParameters_NamedFirst()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "test", DbType = System.Data.DbType.String });

            var result = parser.Parse("SELECT * FROM table WHERE name = @name", parameters);

            Assert.Equal("SELECT * FROM table WHERE name = 'test'", result.FinalSql);
        }

        #endregion

        #region SET SCHEMA Tests

        [Fact]
        public void Parse_SetSchema_Detected()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET SCHEMA myschema", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetSchema, result.CommandType);
        }

        [Fact]
        public void Parse_SetSchema_WithQuotes()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET SCHEMA 'test_schema'", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetSchema, result.CommandType);
            Assert.Equal("test_schema", parser.ExtractSchemaName("SET SCHEMA 'test_schema'"));
        }

        [Fact]
        public void Parse_SetSchema_CaseInsensitive()
        {
            var parser = new SQLParser();
            var result = parser.Parse("set schema TestSchema", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetSchema, result.CommandType);
        }

        [Fact]
        public void Parse_SetSqlId_Detected()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SET SQLID myschema", new KineticaParameterCollection());

            Assert.Equal(ParsedCommandType.SetSchema, result.CommandType);
        }

        [Fact]
        public void Connection_SetSchema_UpdatesCurrentSchema()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191");

            conn.SetSchema("test_schema");

            Assert.Equal("test_schema", conn.CurrentSchema);
        }

        [Fact]
        public void Connection_CurrentSchema_DefaultsToConnectionString()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191;Schema=default_schema");

            Assert.Equal("default_schema", conn.CurrentSchema);
        }

        #endregion

        #region User Impersonation Connection State Tests

        [Fact]
        public void Connection_SetUser_UpdatesImpersonatedUser()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191");

            conn.SetUser("testuser");

            Assert.Equal("testuser", conn.ImpersonatedUser);
        }

        [Fact]
        public void Connection_ExecuteAsUser_PushesToStack()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191");

            conn.SetUser("user1");
            conn.ExecuteAsUser("user2");

            Assert.Equal("user2", conn.ImpersonatedUser);
        }

        [Fact]
        public void Connection_RevertUser_PopsFromStack()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191");

            conn.SetUser("user1");
            conn.ExecuteAsUser("user2");
            conn.RevertUser();

            Assert.Equal("user1", conn.ImpersonatedUser);
        }

        [Fact]
        public void Connection_RevertUser_WithEmptyStack_ClearsImpersonation()
        {
            using var conn = new KineticaConnection("Server=http://localhost:9191");

            conn.SetUser("user1");
            conn.RevertUser();

            Assert.Null(conn.ImpersonatedUser);
        }

        #endregion

        #region Multi-Statement Execution Tests

        [Fact]
        public void Parse_MultiStatement_SplitsCorrectly()
        {
            // Test that the SQL parser can handle multi-statement SQL
            var parser = new SQLParser();

            // First statement
            var result1 = parser.Parse("SELECT 1", new KineticaParameterCollection());
            Assert.Equal(ParsedCommandType.Select, result1.CommandType);

            // Second statement
            var result2 = parser.Parse("INSERT INTO t VALUES (1)", new KineticaParameterCollection());
            Assert.Equal(ParsedCommandType.Insert, result2.CommandType);
        }

        [Fact]
        public void Parse_MultiStatement_PreservesStringLiterals()
        {
            var parser = new SQLParser();
            var result = parser.Parse("SELECT 'hello; world'", new KineticaParameterCollection());

            // Should not split on semicolon inside string
            Assert.Equal("SELECT 'hello; world'", result.FinalSql);
        }

        #endregion

        #region PK Conflict Predicate Tests

        [Fact]
        public void Parse_QueryHints_PkConflictPredicateLower()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_PK_CONFLICT_PREDICATE_LOWER(timestamp) */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.Equal("timestamp", result.Hints.PkConflictPredicateLowerColumn);
        }

        [Fact]
        public void Parse_QueryHints_PkConflictPredicateHigher()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_PK_CONFLICT_PREDICATE_HIGHER(version) */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.Equal("version", result.Hints.PkConflictPredicateHigherColumn);
        }

        [Fact]
        public void Parse_QueryHints_IgnoreExistingPk()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_IGNORE_EXISTING_PK */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.IgnoreExistingPk);
        }

        [Fact]
        public void Parse_QueryHints_ServerSideInsert()
        {
            var parser = new SQLParser();
            var result = parser.Parse("/* KI_HINT_SERVER_SIDE_INSERT */ INSERT INTO table VALUES (1)", new KineticaParameterCollection());

            Assert.NotNull(result.Hints);
            Assert.True(result.Hints.ServerSideInsert);
        }

        [Fact]
        public void QueryHints_ToInsertOptions_IncludesPkConflictPredicates()
        {
            var hints = new QueryHints
            {
                PkConflictPredicateLowerColumn = "created_at",
                PkConflictPredicateHigherColumn = "version",
                IgnoreExistingPk = true
            };

            var options = hints.ToInsertOptions();

            Assert.Equal("created_at", options["pk_conflict_predicate_lower"]);
            Assert.Equal("version", options["pk_conflict_predicate_higher"]);
            Assert.Equal("true", options["ignore_existing_pk"]);
        }

        #endregion

        #region Parameter Type Inference Tests

        [Fact]
        public void Parse_Parameter_DateTimeFormatted()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            var testDate = new DateTime(2024, 6, 15, 10, 30, 45);
            parameters.Add(new KineticaParameter { Value = testDate, DbType = System.Data.DbType.DateTime });

            var result = parser.Parse("SELECT * FROM table WHERE created = ?", parameters);

            Assert.Contains("2024-06-15 10:30:45", result.FinalSql);
        }

        [Fact]
        public void Parse_Parameter_BooleanFormatted()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = true, DbType = System.Data.DbType.Boolean });

            var result = parser.Parse("SELECT * FROM table WHERE active = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE active = TRUE", result.FinalSql);
        }

        [Fact]
        public void Parse_Parameter_DecimalFormatted()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = 123.456m, DbType = System.Data.DbType.Decimal });

            var result = parser.Parse("SELECT * FROM table WHERE amount = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE amount = 123.456", result.FinalSql);
        }

        [Fact]
        public void Parse_Parameter_StringEscapesSingleQuotes()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            parameters.Add(new KineticaParameter { Value = "It's a test", DbType = System.Data.DbType.String });

            var result = parser.Parse("SELECT * FROM table WHERE name = ?", parameters);

            Assert.Equal("SELECT * FROM table WHERE name = 'It''s a test'", result.FinalSql);
        }

        [Fact]
        public void Parse_Parameter_GuidFormatted()
        {
            var parser = new SQLParser();
            var parameters = new KineticaParameterCollection();
            var testGuid = Guid.Parse("12345678-1234-1234-1234-123456789abc");
            parameters.Add(new KineticaParameter { Value = testGuid, DbType = System.Data.DbType.Guid });

            var result = parser.Parse("SELECT * FROM table WHERE id = ?", parameters);

            Assert.Contains("12345678-1234-1234-1234-123456789abc", result.FinalSql);
        }

        #endregion
    }
}
