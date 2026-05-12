using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using kinetica;

namespace KineticaAdo
{
    // 1. Enhanced Connection with Connection Pooling and Async Support
    public class KineticaConnection : DbConnection
    {
        private static readonly KineticaConnectionPool _connectionPool = new KineticaConnectionPool();
        private Kinetica? _kineticaClient;
        private string? _connectionString;
        private ConnectionState _state = ConnectionState.Closed;
        private string _database = "";
        private KineticaConnectionStringBuilder? _connectionStringBuilder;
        private bool _pooled = true;

        // Batch insert support
        private InsertBatchManager? _batchManager;
        private bool _batchInsertMode = false;
        private InsertBatchOptions _batchOptions = new InsertBatchOptions();

        // Session state tracking
        private string? _currentSchema;
        private readonly Stack<string> _userStack = new Stack<string>();
        private string? _currentImpersonatedUser;

        public KineticaConnection() { }

        public KineticaConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string ConnectionString
        {
            get => _connectionString ?? string.Empty;
            set
            {
                _connectionString = value ?? string.Empty;
                _connectionStringBuilder = new KineticaConnectionStringBuilder(_connectionString);
                _pooled = _connectionStringBuilder.Pooling;

                // Apply batch settings from connection string
                _batchInsertMode = _connectionStringBuilder.BatchInsertMode;
                _batchOptions.BatchSize = _connectionStringBuilder.BatchSize;
                _batchOptions.UpdateOnExistingPk = _connectionStringBuilder.BatchUpdateOnExistingPk;
            }
        }

        public override string Database => _database;
        public override string DataSource => _connectionStringBuilder?.Server ?? string.Empty;
        public override string ServerVersion => "7.2"; // Default version
        public override ConnectionState State => _state;

        /// <summary>
        /// Gets or sets the current schema for this session.
        /// Can be set via SET SCHEMA command or programmatically.
        /// </summary>
        public string? CurrentSchema
        {
            get => _currentSchema ?? _connectionStringBuilder?.Schema;
            set => _currentSchema = value;
        }

        /// <summary>
        /// Gets the current impersonated user, if any.
        /// </summary>
        public string? ImpersonatedUser => _currentImpersonatedUser;

        public override void ChangeDatabase(string databaseName)
        {
            _database = databaseName;
        }

        /// <summary>
        /// Sets the current schema (equivalent to SET SCHEMA command).
        /// </summary>
        public void SetSchema(string schemaName)
        {
            _currentSchema = schemaName;
        }

        /// <summary>
        /// Sets the impersonated user context (equivalent to SET USER command).
        /// Requires appropriate permissions.
        /// </summary>
        public void SetUser(string username)
        {
            _currentImpersonatedUser = username;
        }

        /// <summary>
        /// Executes as a specific user, pushing current context to stack.
        /// Use Revert() to return to previous context.
        /// </summary>
        public void ExecuteAsUser(string username)
        {
            if (_currentImpersonatedUser != null)
            {
                _userStack.Push(_currentImpersonatedUser);
            }
            _currentImpersonatedUser = username;
        }

        /// <summary>
        /// Reverts to previous user context from ExecuteAsUser.
        /// </summary>
        public void RevertUser()
        {
            if (_userStack.Count > 0)
            {
                _currentImpersonatedUser = _userStack.Pop();
            }
            else
            {
                _currentImpersonatedUser = null;
            }
        }

        public override void Close()
        {
            if (_state == ConnectionState.Open)
            {
                // Flush and dispose batch manager before closing
                if (_batchManager != null)
                {
                    try
                    {
                        _batchManager.Dispose();
                    }
                    catch
                    {
                        // Ignore errors during close
                    }
                    _batchManager = null;
                }

                if (_pooled && _connectionString != null && _kineticaClient != null)
                {
                    _connectionPool.ReturnConnection(_connectionString, _kineticaClient);
                }
                // Note: Kinetica client doesn't implement IDisposable, so we just release the reference
                // The client will be garbage collected when no longer referenced
                _kineticaClient = null;
                _state = ConnectionState.Closed;
            }
        }

        public override void Open()
        {
            // Use Task.Run to avoid capturing synchronization context which can cause deadlocks
            Task.Run(async () => await OpenAsync(CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (_state == ConnectionState.Open)
                return;

            try
            {
                _state = ConnectionState.Connecting;

                if (_pooled)
                {
                    _kineticaClient = await _connectionPool.GetConnectionAsync(_connectionString ?? string.Empty, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    _kineticaClient = await CreateKineticaClientAsync(_connectionStringBuilder ?? new KineticaConnectionStringBuilder(), cancellationToken).ConfigureAwait(false);
                }
                _database = _connectionStringBuilder?.Database ?? "";
                _state = ConnectionState.Open;

                // Initialize batch manager if batch mode is enabled
                if (_batchInsertMode && _kineticaClient != null)
                {
                    _batchManager = new InsertBatchManager(_kineticaClient, _batchOptions);
                }
            }
            catch (Exception ex)
            {
                _state = ConnectionState.Closed;
                throw new KineticaException($"Failed to connect to Kinetica: {ex.Message}", ex);
            }
        }

        private async Task<Kinetica> CreateKineticaClientAsync(KineticaConnectionStringBuilder builder, CancellationToken cancellationToken)
        {
            Kinetica.Options? options = null;

            // Configure authentication if credentials are provided
            if (!string.IsNullOrEmpty(builder.Username) || !string.IsNullOrEmpty(builder.Password))
            {
                options = new Kinetica.Options
                {
                    Username = builder.Username ?? string.Empty,
                    Password = builder.Password ?? string.Empty
                };
            }

            // Support OAuth token if provided
            if (!string.IsNullOrEmpty(builder.OAuthToken))
            {
                options ??= new Kinetica.Options();
                options.OauthToken = builder.OAuthToken;
            }

            var client = new Kinetica(builder.Server, options);

            // Test connection
            await TestConnectionAsync(client, cancellationToken).ConfigureAwait(false);
            return client;
        }

        private async Task TestConnectionAsync(Kinetica client, CancellationToken cancellationToken)
        {
            try
            {
                // Use a simple operation to test connectivity
                await client.ShowSystemStatusAsync(new Dictionary<string, string>(), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new KineticaException("Connection test failed", ex);
            }
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new KineticaTransaction(this, isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new KineticaCommand(this);
        }

        private async Task<string> GetServerVersionAsync()
        {
            try
            {
                if (_kineticaClient == null)
                    return "Unknown";

                var response = await _kineticaClient.ShowSystemStatusAsync(new Dictionary<string, string>()).ConfigureAwait(false);
                return response?.status_map["status"] ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        internal Kinetica GetKineticaClient() => _kineticaClient ?? throw new InvalidOperationException("Connection is not open");

        #region Batch Insert Support

        /// <summary>
        /// Gets or sets whether batch insert mode is enabled.
        /// When enabled, INSERT statements are buffered and sent in batches for improved performance.
        /// </summary>
        public bool BatchInsertMode
        {
            get => _batchInsertMode;
            set
            {
                if (_batchInsertMode == value) return;

                _batchInsertMode = value;

                if (_batchInsertMode && _state == ConnectionState.Open && _kineticaClient != null)
                {
                    // Create batch manager when enabling batch mode on open connection
                    _batchManager ??= new InsertBatchManager(_kineticaClient, _batchOptions);
                }
                else if (!_batchInsertMode && _batchManager != null)
                {
                    // Flush and dispose when disabling batch mode
                    _batchManager.Dispose();
                    _batchManager = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the batch size for batch insert operations.
        /// </summary>
        public int BatchSize
        {
            get => _batchOptions.BatchSize;
            set => _batchOptions.BatchSize = value;
        }

        /// <summary>
        /// Gets or sets whether to update existing records with matching primary keys during batch inserts.
        /// </summary>
        public bool BatchUpdateOnExistingPk
        {
            get => _batchOptions.UpdateOnExistingPk;
            set => _batchOptions.UpdateOnExistingPk = value;
        }

        /// <summary>
        /// Gets the InsertBatchManager for this connection. Returns null if batch mode is not enabled.
        /// </summary>
        internal InsertBatchManager? BatchManager => _batchManager;

        /// <summary>
        /// Gets the number of records currently pending in the batch buffer.
        /// </summary>
        public int PendingBatchCount => _batchManager?.GetTotalPendingCount() ?? 0;

        /// <summary>
        /// Flushes all pending batch inserts to the database.
        /// </summary>
        /// <returns>The number of records flushed.</returns>
        public long FlushBatch()
        {
            // Use Task.Run to avoid capturing synchronization context which can cause deadlocks
            return Task.Run(async () => await FlushBatchAsync(CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Flushes all pending batch inserts to the database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of records flushed.</returns>
        public async Task<long> FlushBatchAsync(CancellationToken cancellationToken = default)
        {
            if (_batchManager == null)
            {
                return 0;
            }

            return await _batchManager.FlushAllAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion

        // Schema Support
        public override DataTable GetSchema()
        {
            return GetSchema("MetaDataCollections");
        }

        public override DataTable GetSchema(string collectionName)
        {
            return GetSchema(collectionName, Array.Empty<string>());
        }

        public override DataTable GetSchema(string collectionName, string?[]? restrictionValues)
        {
            var schemaProvider = new KineticaSchemaProvider(this);
            return schemaProvider.GetSchema(collectionName, restrictionValues);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
            base.Dispose(disposing);
        }
    }

    // 2. Enhanced Command with SQL Parsing and Async Support
    public class KineticaCommand : DbCommand
    {
        private static readonly SQLParser _sqlParser = new SQLParser();
        private KineticaConnection? _connection;
        private string _commandText = string.Empty;
        private CommandType _commandType = CommandType.Text;
        private int _commandTimeout = 30;
        private int _fetchSize = 0; // 0 means fetch all at once
        private DbParameterCollection _parameters;
        private CancellationTokenSource? _cancellationTokenSource;
        private DbTransaction? _dbTransaction;

        public KineticaCommand()
        {
            _parameters = new KineticaParameterCollection();
        }

        public KineticaCommand(KineticaConnection connection) : this()
        {
            _connection = connection;
        }

        public KineticaCommand(string commandText, KineticaConnection connection) : this(connection)
        {
            _commandText = commandText;
        }

        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string CommandText { get => _commandText; set => _commandText = value ?? string.Empty; }
        public override int CommandTimeout { get => _commandTimeout; set => _commandTimeout = value; }
        public override CommandType CommandType { get => _commandType; set => _commandType = value; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        /// <summary>
        /// Gets or sets the fetch size (number of records to retrieve per batch).
        /// When set to 0 (default), all records are fetched at once.
        /// When set to a positive value, records are fetched in batches of that size using paging.
        /// This is similar to JDBC's setFetchSize() method.
        /// </summary>
        public int FetchSize
        {
            get => _fetchSize;
            set => _fetchSize = Math.Max(0, value);
        }

        protected override DbConnection? DbConnection
        {
            get => _connection;
            set => _connection = value as KineticaConnection;
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction? DbTransaction { get => _dbTransaction; set => _dbTransaction = value; }

        public override void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public override int ExecuteNonQuery()
        {
            // Use Task.Run to avoid capturing synchronization context which can cause deadlocks
            return Task.Run(async () => await ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            var connection = ValidateCommand();

            using (_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(_commandTimeout));
                using var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    _cancellationTokenSource.Token, timeoutToken.Token);

                try
                {
                    var client = connection.GetKineticaClient();

                    // Support multi-statement execution (statements separated by semicolons)
                    var statements = SplitStatements(_commandText);
                    long totalAffected = 0;

                    foreach (var statement in statements)
                    {
                        if (string.IsNullOrWhiteSpace(statement))
                            continue;

                        var parsedCommand = _sqlParser.Parse(statement, _parameters);
                        totalAffected += await ExecuteParsedCommandAsync(client, parsedCommand, combinedToken.Token).ConfigureAwait(false);
                    }

                    return (int)totalAffected;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"Command timeout ({_commandTimeout}s) exceeded");
                }
                catch (Exception ex)
                {
                    throw new KineticaException($"Failed to execute command: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Splits SQL text into individual statements, respecting string literals and comments.
        /// </summary>
        private static IList<string> SplitStatements(string sql)
        {
            var statements = new List<string>();
            var currentStatement = new System.Text.StringBuilder();
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            bool inLineComment = false;
            bool inBlockComment = false;

            for (int i = 0; i < sql.Length; i++)
            {
                char c = sql[i];
                char nextChar = i + 1 < sql.Length ? sql[i + 1] : '\0';

                // Handle line comment start
                if (!inSingleQuote && !inDoubleQuote && !inBlockComment && c == '-' && nextChar == '-')
                {
                    inLineComment = true;
                    currentStatement.Append(c);
                    continue;
                }

                // Handle line comment end
                if (inLineComment && (c == '\n' || c == '\r'))
                {
                    inLineComment = false;
                    currentStatement.Append(c);
                    continue;
                }

                // Handle block comment start
                if (!inSingleQuote && !inDoubleQuote && !inLineComment && c == '/' && nextChar == '*')
                {
                    inBlockComment = true;
                    currentStatement.Append(c);
                    continue;
                }

                // Handle block comment end
                if (inBlockComment && c == '*' && nextChar == '/')
                {
                    inBlockComment = false;
                    currentStatement.Append(c);
                    currentStatement.Append(nextChar);
                    i++; // Skip the '/'
                    continue;
                }

                // Skip processing in comments
                if (inLineComment || inBlockComment)
                {
                    currentStatement.Append(c);
                    continue;
                }

                // Handle string literals
                if (c == '\'' && !inDoubleQuote)
                {
                    // Check for escaped quote
                    if (inSingleQuote && nextChar == '\'')
                    {
                        currentStatement.Append(c);
                        currentStatement.Append(nextChar);
                        i++;
                        continue;
                    }
                    inSingleQuote = !inSingleQuote;
                }
                else if (c == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                }

                // Check for statement separator
                if (c == ';' && !inSingleQuote && !inDoubleQuote)
                {
                    var stmt = currentStatement.ToString().Trim();
                    if (!string.IsNullOrEmpty(stmt))
                    {
                        statements.Add(stmt);
                    }
                    currentStatement.Clear();
                    continue;
                }

                currentStatement.Append(c);
            }

            // Add last statement if any
            var lastStmt = currentStatement.ToString().Trim();
            if (!string.IsNullOrEmpty(lastStmt))
            {
                statements.Add(lastStmt);
            }

            return statements;
        }

        public override object? ExecuteScalar()
        {
            // Use Task.Run to avoid capturing synchronization context which can cause deadlocks
            return Task.Run(async () => await ExecuteScalarAsync(CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public override async Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            using (var reader = await ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return reader.GetValue(0);
                }
                return null;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            // Use Task.Run to avoid capturing synchronization context which can cause deadlocks
            return Task.Run(async () => await ExecuteReaderAsync(behavior, CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            var connection = ValidateCommand();

            using (_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(_commandTimeout));
                using var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    _cancellationTokenSource.Token, timeoutToken.Token);

                try
                {
                    var client = connection.GetKineticaClient();
                    var parsedCommand = _sqlParser.Parse(_commandText, _parameters);

                    // Use paging reader if FetchSize is set and this is a SELECT query
                    if (_fetchSize > 0 && parsedCommand.CommandType == ParsedCommandType.Select)
                    {
                        return new KineticaPagingDataReader(
                            client,
                            parsedCommand.FinalSql,
                            _fetchSize,
                            behavior,
                            combinedToken.Token);
                    }

                    ExecuteSqlResponse response;
                    if (parsedCommand.CommandType == ParsedCommandType.Select)
                    {
                        response = await client.ExecuteSqlAsync(parsedCommand.FinalSql, 0, -9999, options: null, cancellationToken: combinedToken.Token).ConfigureAwait(false);
                    }
                    else
                    {
                        response = await ExecuteParsedQueryAsync(client, parsedCommand, combinedToken.Token).ConfigureAwait(false);
                    }

                    return new KineticaDataReader(response, behavior);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"Command timeout ({_commandTimeout}s) exceeded");
                }
                catch (Exception ex)
                {
                    throw new KineticaException($"Failed to execute reader: {ex.Message}", ex);
                }
            }
        }

        private async Task<long> ExecuteParsedCommandAsync(Kinetica client, ParsedCommand parsedCommand, CancellationToken cancellationToken)
        {
            return parsedCommand.CommandType switch
            {
                ParsedCommandType.Insert => await ExecuteInsertAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.InsertFromFile => await ExecuteInsertFromFileAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.Update => await ExecuteUpdateAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.Delete => await ExecuteDeleteAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.CreateTable => await ExecuteCreateTableAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.DropTable => await ExecuteDropTableAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false),
                ParsedCommandType.SetSchema => ExecuteSetSchema(parsedCommand),
                ParsedCommandType.SetUser => ExecuteSetUser(parsedCommand),
                ParsedCommandType.ExecuteAsUser => ExecuteAsUser(parsedCommand),
                ParsedCommandType.Revert => ExecuteRevert(),
                _ => await ExecuteGenericSqlAsync(client, parsedCommand, cancellationToken).ConfigureAwait(false)
            };
        }

        private long ExecuteSetSchema(ParsedCommand command)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection is not open");

            var schemaName = _sqlParser.ExtractSchemaName(command.OriginalSql);
            if (!string.IsNullOrEmpty(schemaName))
            {
                _connection.SetSchema(schemaName);
            }
            return 0;
        }

        private long ExecuteSetUser(ParsedCommand command)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection is not open");

            if (command.UserImpersonation?.Username != null)
            {
                _connection.SetUser(command.UserImpersonation.Username);
            }
            return 0;
        }

        private long ExecuteAsUser(ParsedCommand command)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection is not open");

            if (command.UserImpersonation?.Username != null)
            {
                _connection.ExecuteAsUser(command.UserImpersonation.Username);
            }
            return 0;
        }

        private long ExecuteRevert()
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection is not open");

            _connection.RevertUser();
            return 0;
        }

        private async Task<ExecuteSqlResponse> ExecuteParsedQueryAsync(Kinetica client, ParsedCommand parsedCommand, CancellationToken cancellationToken)
        {
            return await client.ExecuteSqlAsync(parsedCommand.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<long> ExecuteInsertAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            // Check if batch mode is enabled on the connection
            if (_connection?.BatchInsertMode == true && _connection.BatchManager != null)
            {
                // Try to parse the INSERT statement for batch processing
                if (InsertStatementParser.TryParse(command.FinalSql, out var parsedInsert) && parsedInsert != null)
                {
                    // Use batch insert
                    await _connection.BatchManager.InsertAsync(
                        parsedInsert.TableName,
                        parsedInsert.ColumnNames,
                        parsedInsert.Values,
                        cancellationToken).ConfigureAwait(false);

                    return 1; // Return 1 to indicate one record was queued
                }
            }

            // Fall back to SQL execution
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.count_affected;
        }

        private async Task<long> ExecuteInsertFromFileAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            if (command.InsertFromFile == null)
            {
                throw new KineticaException("INSERT FROM FILE command is missing file information");
            }

            var fileInfo = command.InsertFromFile;

            // Apply hints to file options
            if (command.Hints.BatchSize.HasValue)
                fileInfo.Options.BatchSize = command.Hints.BatchSize.Value;
            if (command.Hints.TruncateStrings)
                fileInfo.Options.TruncateStrings = true;
            if (command.Hints.UpdateOnExistingPk)
                fileInfo.Options.UpdateOnExistingPk = true;
            if (command.Hints.IgnoreExistingPk)
                fileInfo.Options.IgnoreExistingPk = true;

            // Use CsvFileReader to read and insert
            var reader = new CsvFileReader(client, fileInfo);
            var recordCount = await reader.ReadAndInsertAsync(cancellationToken).ConfigureAwait(false);

            return recordCount;
        }

        private async Task<long> ExecuteUpdateAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.count_affected;
        }

        private async Task<long> ExecuteDeleteAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.count_affected;
        }

        private async Task<int> ExecuteCreateTableAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return 1; // Table created
        }

        private async Task<int> ExecuteDropTableAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return 1; // Table dropped
        }

        private async Task<long> ExecuteGenericSqlAsync(Kinetica client, ParsedCommand command, CancellationToken cancellationToken)
        {
            var response = await client.ExecuteSqlAsync(command.FinalSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.count_affected;
        }

        protected override DbParameter CreateDbParameter()
        {
            return new KineticaParameter();
        }

        public override void Prepare()
        {
            // Validate SQL and parameters
            ValidateCommand();
            try
            {
                _sqlParser.Parse(_commandText, _parameters);
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to prepare command: {ex.Message}", ex);
            }
        }

        private KineticaConnection ValidateCommand()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection is not open");

            if (string.IsNullOrEmpty(_commandText))
                throw new InvalidOperationException("Command text is not set");

            return _connection;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // 3. Enhanced DataReader with Async Support
    public class KineticaDataReader : DbDataReader
    {
        private readonly ExecuteSqlResponse _response;
        private readonly CommandBehavior _behavior;
        private readonly IList<string> _columnNames;
        private readonly IList<string> _columnTypes;
        private int _currentRow = -1;
        private bool _closed = false;

        public KineticaDataReader(ExecuteSqlResponse response, CommandBehavior behavior)
        {
            _response = response;
            _behavior = behavior;

            // Extract column information from the first record's schema if available
            _columnNames = new List<string>();
            _columnTypes = new List<string>();

            if (_response.data != null && _response.data.Count > 0)
            {
                var firstRecord = _response.data[0];
                if (firstRecord?.Schema?.Fields != null)
                {
                    foreach (var field in firstRecord.Schema.Fields)
                    {
                        _columnNames.Add(field.Name);
                        _columnTypes.Add(GetAvroFieldType(field.Schema));
                    }
                }
            }
        }

        private static string GetAvroFieldType(Avro.Schema schema)
        {
            if (schema is Avro.UnionSchema unionSchema)
            {
                // For union types, find the non-null type
                foreach (var s in unionSchema.Schemas)
                {
                    if (s.Tag != Avro.Schema.Type.Null)
                        return GetAvroFieldType(s);
                }
            }

            return schema.Tag switch
            {
                Avro.Schema.Type.Int => "int",
                Avro.Schema.Type.Long => "long",
                Avro.Schema.Type.Float => "float",
                Avro.Schema.Type.Double => "double",
                Avro.Schema.Type.String => "string",
                Avro.Schema.Type.Boolean => "boolean",
                Avro.Schema.Type.Bytes => "bytes",
                _ => "string"
            };
        }

        public override bool HasRows => _response.total_number_of_records > 0;
        public override bool IsClosed => _closed;
        public override int RecordsAffected => (int)(_response.count_affected);
        public override int FieldCount => _columnNames.Count;
        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));
        public override int Depth => 0;

        public override bool Read()
        {
            // Read is synchronous - data is already loaded in memory
            if (_closed || _response.data == null)
                return false;

            _currentRow++;
            return _currentRow < _response.data.Count;
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            // Yield to make it properly async (Task.Yield doesn't support ConfigureAwait)
            await Task.Yield();

            if (_closed || _response.data == null)
                return false;

            _currentRow++;
            return _currentRow < _response.data.Count;
        }

        public override bool NextResult()
        {
            return false; // Kinetica doesn't support multiple result sets
        }

        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            // Yield to make it properly async (Task.Yield doesn't support ConfigureAwait)
            await Task.Yield();
            return false; // Kinetica doesn't support multiple result sets
        }

        public override void Close()
        {
            _closed = true;
        }

        public override bool GetBoolean(int ordinal) => Convert.ToBoolean(GetValue(ordinal));
        public override byte GetByte(int ordinal) => Convert.ToByte(GetValue(ordinal));
        public override char GetChar(int ordinal) => Convert.ToChar(GetValue(ordinal));
        public override DateTime GetDateTime(int ordinal) => Convert.ToDateTime(GetValue(ordinal));
        public override decimal GetDecimal(int ordinal) => Convert.ToDecimal(GetValue(ordinal));
        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));
        public override float GetFloat(int ordinal) => Convert.ToSingle(GetValue(ordinal));
        public override Guid GetGuid(int ordinal) => Guid.Parse(GetValue(ordinal)?.ToString() ?? "");
        public override short GetInt16(int ordinal) => Convert.ToInt16(GetValue(ordinal));
        public override int GetInt32(int ordinal) => Convert.ToInt32(GetValue(ordinal));
        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));
        public override string GetString(int ordinal) => GetValue(ordinal)?.ToString() ?? "";

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            var value = GetValue(ordinal);
            if (value == null || value == DBNull.Value)
                return 0;

            byte[] bytes;
            if (value is byte[] byteArray)
            {
                bytes = byteArray;
            }
            else if (value is string str)
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(str);
            }
            else
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(value.ToString() ?? "");
            }

            // If buffer is null, return total length
            if (buffer == null)
                return bytes.Length;

            // Calculate how many bytes to copy
            long availableBytes = bytes.Length - dataOffset;
            if (availableBytes <= 0)
                return 0;

            int bytesToCopy = (int)Math.Min(availableBytes, length);
            Array.Copy(bytes, dataOffset, buffer, bufferOffset, bytesToCopy);
            return bytesToCopy;
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            var value = GetValue(ordinal);
            if (value == null || value == DBNull.Value)
                return 0;

            string str = value.ToString() ?? "";

            // If buffer is null, return total length
            if (buffer == null)
                return str.Length;

            // Calculate how many chars to copy
            long availableChars = str.Length - dataOffset;
            if (availableChars <= 0)
                return 0;

            int charsToCopy = (int)Math.Min(availableChars, length);
            str.CopyTo((int)dataOffset, buffer, bufferOffset, charsToCopy);
            return charsToCopy;
        }

        public override string GetDataTypeName(int ordinal)
        {
            if (ordinal >= 0 && ordinal < _columnTypes.Count)
                return _columnTypes[ordinal];
            return "string";
        }

        public override Type GetFieldType(int ordinal)
        {
            var dataType = GetDataTypeName(ordinal).ToLower();
            return dataType switch
            {
                // Integer types
                "int" or "integer" or "int32" => typeof(int),
                "int8" or "tinyint" => typeof(sbyte),
                "int16" or "smallint" => typeof(short),
                "long" or "bigint" or "int64" => typeof(long),
                "ulong" or "uint64" => typeof(ulong),

                // Floating point types
                "float" or "real" => typeof(float),
                "double" or "float8" => typeof(double),
                "decimal" or "numeric" => typeof(decimal),

                // Boolean
                "bool" or "boolean" => typeof(bool),

                // String types (including charN variants)
                "string" or "varchar" or "text" => typeof(string),
                var s when s.StartsWith("char") => typeof(string),

                // Date/Time types
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "datetime" or "timestamp" => typeof(DateTime),

                // Special types
                "uuid" or "guid" => typeof(Guid),
                "ipv4" => typeof(string),
                "json" => typeof(string),
                "wkt" => typeof(string),

                // Binary
                "bytes" or "binary" or "varbinary" => typeof(byte[]),

                // Vector and array types - return as object
                "vector" => typeof(float[]),
                var a when a.StartsWith("array") => typeof(object),

                // Default
                _ => typeof(string)
            };
        }

        public override string GetName(int ordinal)
        {
            if (ordinal >= 0 && ordinal < _columnNames.Count)
                return _columnNames[ordinal];
            return $"Column{ordinal}";
        }

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < _columnNames.Count; i++)
            {
                if (string.Equals(_columnNames[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            throw new ArgumentException($"Column '{name}' not found");
        }

        public override object GetValue(int ordinal)
        {
            if (_currentRow < 0 || _response.data == null || _currentRow >= _response.data.Count)
                throw new InvalidOperationException("No current row");

            if (ordinal < 0 || ordinal >= _columnNames.Count)
                throw new ArgumentOutOfRangeException(nameof(ordinal));

            var record = _response.data[_currentRow];
            var fieldName = _columnNames[ordinal];

            if (record.TryGetValue(fieldName, out var value))
            {
                return value ?? DBNull.Value;
            }

            return DBNull.Value;
        }

        public override int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }

        public override bool IsDBNull(int ordinal)
        {
            var value = GetValue(ordinal);
            return value == null || value == DBNull.Value;
        }

        public override IEnumerator<IDataRecord> GetEnumerator()
        {
            while (Read())
            {
                yield return this;
            }
        }
    }

    /// <summary>
    /// A paging data reader that fetches records in batches for large result sets.
    /// Similar to JDBC's forward-only result set with fetch size.
    /// </summary>
    public class KineticaPagingDataReader : DbDataReader
    {
        private readonly Kinetica _client;
        private readonly string _originalSql;
        private readonly int _fetchSize;
        private readonly CommandBehavior _behavior;
        private readonly CancellationToken _cancellationToken;

        private ExecuteSqlResponse? _currentPage;
        private IList<string>? _columnNames;
        private IList<string>? _columnTypes;
        private int _currentRowInPage = -1;
        private long _totalRowsRead = 0;
        private long _offset = 0;
        private bool _closed = false;
        private bool _hasMore = true;
        private long _totalRecordCount = -1;

        public KineticaPagingDataReader(
            Kinetica client,
            string sql,
            int fetchSize,
            CommandBehavior behavior,
            CancellationToken cancellationToken = default)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _originalSql = sql ?? throw new ArgumentNullException(nameof(sql));
            _fetchSize = fetchSize;
            _behavior = behavior;
            _cancellationToken = cancellationToken;

            // Fetch first page to get schema
            FetchNextPage();
        }

        private void FetchNextPage()
        {
            // Synchronous wrapper for constructor and synchronous Read()
            // Uses Task.Run to avoid deadlocks
            Task.Run(async () => await FetchNextPageAsync(_cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private async Task FetchNextPageAsync(CancellationToken cancellationToken = default)
        {
            if (!_hasMore || _closed)
                return;

            // Build paged query using LIMIT and OFFSET
            var pagedSql = BuildPagedQuery(_originalSql, _offset, _fetchSize);

            var response = await _client.ExecuteSqlAsync(pagedSql, 0, -9999, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            _currentPage = response;
            _currentRowInPage = -1;

            // Initialize column info from first page
            if (_columnNames == null && response.data != null && response.data.Count > 0)
            {
                _columnNames = new List<string>();
                _columnTypes = new List<string>();

                var firstRecord = response.data[0];
                if (firstRecord?.Schema?.Fields != null)
                {
                    foreach (var field in firstRecord.Schema.Fields)
                    {
                        _columnNames.Add(field.Name);
                        _columnTypes!.Add(GetAvroFieldType(field.Schema));
                    }
                }
            }

            // Track total record count from first response
            if (_totalRecordCount < 0)
            {
                _totalRecordCount = response.total_number_of_records;
            }

            // Check if there are more records to fetch
            var recordsInPage = response.data?.Count ?? 0;
            _offset += recordsInPage;
            _hasMore = recordsInPage == _fetchSize && _offset < _totalRecordCount;
        }

        private static string BuildPagedQuery(string sql, long offset, int limit)
        {
            // Check if query already has LIMIT/OFFSET
            var upperSql = sql.ToUpperInvariant();
            if (upperSql.Contains(" LIMIT ") || upperSql.Contains(" OFFSET "))
            {
                // Don't modify queries that already have paging
                return sql;
            }

            // Append LIMIT and OFFSET
            return $"{sql.TrimEnd(';', ' ')} LIMIT {limit} OFFSET {offset}";
        }

        private static string GetAvroFieldType(Avro.Schema schema)
        {
            if (schema is Avro.UnionSchema unionSchema)
            {
                foreach (var s in unionSchema.Schemas)
                {
                    if (s.Tag != Avro.Schema.Type.Null)
                        return GetAvroFieldType(s);
                }
            }

            return schema.Tag switch
            {
                Avro.Schema.Type.Int => "int",
                Avro.Schema.Type.Long => "long",
                Avro.Schema.Type.Float => "float",
                Avro.Schema.Type.Double => "double",
                Avro.Schema.Type.String => "string",
                Avro.Schema.Type.Boolean => "boolean",
                Avro.Schema.Type.Bytes => "bytes",
                _ => "string"
            };
        }

        public override bool HasRows => _totalRecordCount > 0;
        public override bool IsClosed => _closed;
        public override int RecordsAffected => (int)_totalRecordCount;
        public override int FieldCount => _columnNames?.Count ?? 0;
        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));
        public override int Depth => 0;

        /// <summary>
        /// Gets the total number of records read so far.
        /// </summary>
        public long TotalRowsRead => _totalRowsRead;

        /// <summary>
        /// Gets the total number of records in the result set.
        /// </summary>
        public long TotalRecordCount => _totalRecordCount;

        public override bool Read()
        {
            if (_closed || _currentPage?.data == null)
                return false;

            _currentRowInPage++;

            // Check if we need to fetch next page
            if (_currentRowInPage >= _currentPage.data.Count)
            {
                if (!_hasMore)
                    return false;

                FetchNextPage();

                if (_currentPage?.data == null || _currentPage.data.Count == 0)
                    return false;

                _currentRowInPage = 0;
            }

            _totalRowsRead++;
            return true;
        }

        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (_closed || _currentPage?.data == null)
                return false;

            _currentRowInPage++;

            // Check if we need to fetch next page
            if (_currentRowInPage >= _currentPage.data.Count)
            {
                if (!_hasMore)
                    return false;

                await FetchNextPageAsync(cancellationToken).ConfigureAwait(false);

                if (_currentPage?.data == null || _currentPage.data.Count == 0)
                    return false;

                _currentRowInPage = 0;
            }

            _totalRowsRead++;
            return true;
        }

        public override bool NextResult() => false;

        public override void Close()
        {
            _closed = true;
            _currentPage = null;
        }

        public override DataTable GetSchemaTable()
        {
            var schemaTable = new DataTable("SchemaTable");
            schemaTable.Columns.Add("ColumnName", typeof(string));
            schemaTable.Columns.Add("ColumnOrdinal", typeof(int));
            schemaTable.Columns.Add("DataType", typeof(Type));
            schemaTable.Columns.Add("ColumnSize", typeof(int));

            if (_columnNames != null)
            {
                for (int i = 0; i < _columnNames.Count; i++)
                {
                    var row = schemaTable.NewRow();
                    row["ColumnName"] = _columnNames[i];
                    row["ColumnOrdinal"] = i;
                    row["DataType"] = GetFieldType(i);
                    row["ColumnSize"] = -1;
                    schemaTable.Rows.Add(row);
                }
            }

            return schemaTable;
        }

        public override bool GetBoolean(int ordinal) => Convert.ToBoolean(GetValue(ordinal));
        public override byte GetByte(int ordinal) => Convert.ToByte(GetValue(ordinal));
        public override char GetChar(int ordinal) => Convert.ToChar(GetValue(ordinal));
        public override DateTime GetDateTime(int ordinal) => Convert.ToDateTime(GetValue(ordinal));
        public override decimal GetDecimal(int ordinal) => Convert.ToDecimal(GetValue(ordinal));
        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));
        public override float GetFloat(int ordinal) => Convert.ToSingle(GetValue(ordinal));
        public override Guid GetGuid(int ordinal) => Guid.Parse(GetValue(ordinal)?.ToString() ?? Guid.Empty.ToString());
        public override short GetInt16(int ordinal) => Convert.ToInt16(GetValue(ordinal));
        public override int GetInt32(int ordinal) => Convert.ToInt32(GetValue(ordinal));
        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));
        public override string GetString(int ordinal) => GetValue(ordinal)?.ToString() ?? string.Empty;

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            var value = GetValue(ordinal);
            if (value is not byte[] bytes)
                return 0;

            if (buffer == null)
                return bytes.Length;

            int bytesToCopy = Math.Min(bytes.Length - (int)dataOffset, length);
            Array.Copy(bytes, (int)dataOffset, buffer, bufferOffset, bytesToCopy);
            return bytesToCopy;
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            var str = GetString(ordinal);
            if (buffer == null)
                return str.Length;

            int availableChars = str.Length - (int)dataOffset;
            int charsToCopy = Math.Min(availableChars, length);
            str.CopyTo((int)dataOffset, buffer, bufferOffset, charsToCopy);
            return charsToCopy;
        }

        public override string GetDataTypeName(int ordinal)
        {
            if (_columnTypes != null && ordinal >= 0 && ordinal < _columnTypes.Count)
                return _columnTypes[ordinal];
            return "string";
        }

        public override Type GetFieldType(int ordinal)
        {
            var dataType = GetDataTypeName(ordinal).ToLower();
            return dataType switch
            {
                "int" or "integer" or "int32" => typeof(int),
                "int8" or "tinyint" => typeof(sbyte),
                "int16" or "smallint" => typeof(short),
                "long" or "bigint" or "int64" => typeof(long),
                "float" or "real" => typeof(float),
                "double" or "float8" => typeof(double),
                "decimal" or "numeric" => typeof(decimal),
                "bool" or "boolean" => typeof(bool),
                "string" or "varchar" or "text" => typeof(string),
                var s when s.StartsWith("char") => typeof(string),
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "datetime" or "timestamp" => typeof(DateTime),
                "uuid" or "guid" => typeof(Guid),
                "bytes" or "binary" => typeof(byte[]),
                _ => typeof(string)
            };
        }

        public override string GetName(int ordinal)
        {
            if (_columnNames != null && ordinal >= 0 && ordinal < _columnNames.Count)
                return _columnNames[ordinal];
            return $"Column{ordinal}";
        }

        public override int GetOrdinal(string name)
        {
            if (_columnNames != null)
            {
                for (int i = 0; i < _columnNames.Count; i++)
                {
                    if (string.Equals(_columnNames[i], name, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }
            throw new ArgumentException($"Column '{name}' not found");
        }

        public override object GetValue(int ordinal)
        {
            if (_currentRowInPage < 0 || _currentPage?.data == null || _currentRowInPage >= _currentPage.data.Count)
                throw new InvalidOperationException("No current row");

            if (ordinal < 0 || ordinal >= FieldCount)
                throw new ArgumentOutOfRangeException(nameof(ordinal));

            var record = _currentPage.data[_currentRowInPage];
            if (record == null)
                return DBNull.Value;

            var fieldName = GetName(ordinal);
            if (record.TryGetValue(fieldName, out var value))
            {
                return value ?? DBNull.Value;
            }

            return DBNull.Value;
        }

        public override int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }

        public override bool IsDBNull(int ordinal)
        {
            var value = GetValue(ordinal);
            return value == null || value == DBNull.Value;
        }

        public override IEnumerator<IDataRecord> GetEnumerator()
        {
            while (Read())
            {
                yield return this;
            }
        }
    }

    // 4. Connection String Builder - JDBC-Compatible Properties
    /// <summary>
    /// Provides a comprehensive connection string builder with JDBC-compatible properties.
    /// Supports all connection options from the Java JDBC driver.
    /// </summary>
    public class KineticaConnectionStringBuilder
    {
        private readonly Dictionary<string, object> _properties;

        // Property key constants matching JDBC driver
        public static class PropertyKeys
        {
            // Connection properties
            public const string Server = "Server";
            public const string Url = "URL";
            public const string PrimaryUrl = "PrimaryURL";
            public const string Username = "Username";
            public const string Password = "Password";
            public const string OAuthToken = "OAuthToken";
            public const string Database = "Database";
            public const string Schema = "Schema";
            public const string ImpersonateUser = "ImpersonateUser";

            // Timeout properties
            public const string Timeout = "Timeout";
            public const string ConnectionTimeout = "Connection Timeout";
            public const string InitialConnectionTimeout = "InitialConnectionTimeoutSeconds";
            public const string ServerConnectionTimeout = "ServerConnectionTimeoutSeconds";

            // SSL/TLS properties
            public const string BypassSslCertCheck = "BypassSslCertCheck";
            public const string SslCaCertPath = "SslCACertPath";
            public const string SslCertPassword = "SslCertPassword";
            public const string SslAllowHostMismatch = "SslAllowHostMismatch";

            // Network properties
            public const string DisableAutoDiscovery = "DisableAutoDiscovery";
            public const string DisableFailover = "DisableFailover";
            public const string FailoverOrder = "FailoverOrder";
            public const string FailbackPollInterval = "FailbackPollInterval";
            public const string DisableSnappy = "DisableSnappy";

            // Query optimization properties
            public const string CostBasedOptimization = "CostBasedOptimization";
            public const string DistributedJoins = "DistributedJoins";
            public const string ParallelExecution = "ParallelExecution";
            public const string PlanCache = "PlanCache";
            public const string ResultsCaching = "ResultsCaching";
            public const string RuleBasedOptimizations = "RuleBasedOptimizations";
            public const string SsqOptimizations = "SsqOptimizations";
            public const string UseApproxCountDistinct = "UseApproxCountDistinct";
            public const string ValidateChange = "ValidateChange";

            // Query control properties
            public const string ReadOnly = "ReadOnly";
            public const string Ttl = "TTL";
            public const string PagingTableTtl = "PagingTableTTL";
            public const string Limit = "Limit";
            public const string RowsPerFetch = "RowsPerFetch";
            public const string FetchSize = "FetchSize";

            // Insertion properties
            public const string RowsPerInsertion = "RowsPerInsertion";
            public const string DisableMultiheadInsert = "DisableMultiheadInsert";
            public const string IgnoreExistingPk = "IgnoreExistingPk";
            public const string TruncateStrings = "TruncateStrings";
            public const string UpdateOnExistingPk = "UpdateOnExistingPk";
            public const string ErrorMode = "ErrorMode";
            public const string Replication = "Replication";
            public const string NoSync = "NoSync";

            // File I/O properties
            public const string FileReadDelimiter = "FileReadDelimiter";
            public const string FileReadHasHeader = "FileReadHasHeader";
            public const string FileReadNullString = "FileReadNullString";
            public const string FileReadEscapeChar = "FileReadEscapeChar";
            public const string FileReadQuoteChar = "FileReadQuoteChar";
            public const string FileReadComment = "FileReadComment";
            public const string FileReadInitialClear = "FileReadInitialClear";
            public const string FileReadLimit = "FileReadLimit";
            public const string FileReadSkip = "FileReadSkip";

            // Connection pooling
            public const string Pooling = "Pooling";
            public const string MaxPoolSize = "Max Pool Size";
            public const string MinPoolSize = "Min Pool Size";

            // Batch insert mode
            public const string BatchInsertMode = "Batch Insert Mode";
            public const string BatchSize = "Batch Size";
            public const string BatchUpdateOnExistingPk = "Batch Update On Existing Pk";

            // Misc
            public const string TimeZoneOverride = "TimeZoneOverride";
            public const string TokenNameClaim = "TokenNameClaim";
            public const string UseKeyLookup = "UseKeyLookup";
            public const string FakeTransactions = "FakeTransactions";
            public const string LogLevel = "LogLevel";
        }

        public KineticaConnectionStringBuilder() : this(string.Empty) { }

        public KineticaConnectionStringBuilder(string connectionString)
        {
            _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            ParseConnectionString(connectionString);
        }

        #region Connection Properties

        /// <summary>
        /// Gets or sets the server URL (e.g., "http://localhost:9191").
        /// Aliases: Server, URL, Host, Data Source
        /// </summary>
        public string Server
        {
            get => GetProperty<string>(PropertyKeys.Server, "http://127.0.0.1:9191");
            set => SetProperty(PropertyKeys.Server, value);
        }

        /// <summary>
        /// Gets or sets the primary URL for HA configurations.
        /// </summary>
        public string PrimaryUrl
        {
            get => GetProperty<string>(PropertyKeys.PrimaryUrl, "");
            set => SetProperty(PropertyKeys.PrimaryUrl, value);
        }

        /// <summary>
        /// Gets or sets the username for authentication.
        /// Aliases: Username, UID, User ID
        /// </summary>
        public string Username
        {
            get => GetProperty<string>(PropertyKeys.Username, "");
            set => SetProperty(PropertyKeys.Username, value);
        }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// Aliases: Password, PWD
        /// </summary>
        public string Password
        {
            get => GetProperty<string>(PropertyKeys.Password, "");
            set => SetProperty(PropertyKeys.Password, value);
        }

        /// <summary>
        /// Gets or sets the OAuth token for authentication.
        /// </summary>
        public string OAuthToken
        {
            get => GetProperty<string>(PropertyKeys.OAuthToken, "");
            set => SetProperty(PropertyKeys.OAuthToken, value);
        }

        /// <summary>
        /// Gets or sets the database name.
        /// Aliases: Database, Initial Catalog
        /// </summary>
        public string? Database
        {
            get => GetProperty<string?>(PropertyKeys.Database, null);
            set => SetProperty(PropertyKeys.Database, value);
        }

        /// <summary>
        /// Gets or sets the default schema.
        /// </summary>
        public string? Schema
        {
            get => GetProperty<string?>(PropertyKeys.Schema, null);
            set => SetProperty(PropertyKeys.Schema, value);
        }

        /// <summary>
        /// Gets or sets the user to impersonate. Requires appropriate permissions.
        /// </summary>
        public string? ImpersonateUser
        {
            get => GetProperty<string?>(PropertyKeys.ImpersonateUser, null);
            set => SetProperty(PropertyKeys.ImpersonateUser, value);
        }

        #endregion

        #region Timeout Properties

        /// <summary>
        /// Gets or sets the query timeout in minutes. -1 means no limit.
        /// </summary>
        public int Timeout
        {
            get => GetProperty<int>(PropertyKeys.Timeout, -1);
            set => SetProperty(PropertyKeys.Timeout, value);
        }

        /// <summary>
        /// Gets or sets the connection timeout in seconds.
        /// </summary>
        public int ConnectionTimeout
        {
            get => GetProperty<int>(PropertyKeys.ConnectionTimeout, 30);
            set => SetProperty(PropertyKeys.ConnectionTimeout, value);
        }

        /// <summary>
        /// Gets or sets the initial connection timeout in seconds. 0 means no limit.
        /// </summary>
        public int InitialConnectionTimeout
        {
            get => GetProperty<int>(PropertyKeys.InitialConnectionTimeout, -1);
            set => SetProperty(PropertyKeys.InitialConnectionTimeout, value);
        }

        /// <summary>
        /// Gets or sets the server connection timeout in seconds. 0 means no limit.
        /// </summary>
        public int ServerConnectionTimeout
        {
            get => GetProperty<int>(PropertyKeys.ServerConnectionTimeout, -1);
            set => SetProperty(PropertyKeys.ServerConnectionTimeout, value);
        }

        #endregion

        #region SSL/TLS Properties

        /// <summary>
        /// Gets or sets whether to bypass SSL certificate validation.
        /// </summary>
        public bool BypassSslCertCheck
        {
            get => GetProperty<bool>(PropertyKeys.BypassSslCertCheck, false);
            set => SetProperty(PropertyKeys.BypassSslCertCheck, value);
        }

        /// <summary>
        /// Gets or sets the path to the SSL CA certificate.
        /// </summary>
        public string? SslCaCertPath
        {
            get => GetProperty<string?>(PropertyKeys.SslCaCertPath, null);
            set => SetProperty(PropertyKeys.SslCaCertPath, value);
        }

        /// <summary>
        /// Gets or sets the SSL certificate password.
        /// </summary>
        public string? SslCertPassword
        {
            get => GetProperty<string?>(PropertyKeys.SslCertPassword, null);
            set => SetProperty(PropertyKeys.SslCertPassword, value);
        }

        /// <summary>
        /// Gets or sets whether to allow SSL host mismatch.
        /// </summary>
        public bool SslAllowHostMismatch
        {
            get => GetProperty<bool>(PropertyKeys.SslAllowHostMismatch, false);
            set => SetProperty(PropertyKeys.SslAllowHostMismatch, value);
        }

        #endregion

        #region Network Properties

        /// <summary>
        /// Gets or sets whether to disable auto-discovery of cluster nodes.
        /// </summary>
        public bool DisableAutoDiscovery
        {
            get => GetProperty<bool>(PropertyKeys.DisableAutoDiscovery, false);
            set => SetProperty(PropertyKeys.DisableAutoDiscovery, value);
        }

        /// <summary>
        /// Gets or sets whether to disable automatic failover.
        /// </summary>
        public bool DisableFailover
        {
            get => GetProperty<bool>(PropertyKeys.DisableFailover, false);
            set => SetProperty(PropertyKeys.DisableFailover, value);
        }

        /// <summary>
        /// Gets or sets the HA failover order.
        /// </summary>
        public string? FailoverOrder
        {
            get => GetProperty<string?>(PropertyKeys.FailoverOrder, null);
            set => SetProperty(PropertyKeys.FailoverOrder, value);
        }

        /// <summary>
        /// Gets or sets the failback poll interval in seconds. -1 means no limit.
        /// </summary>
        public int FailbackPollInterval
        {
            get => GetProperty<int>(PropertyKeys.FailbackPollInterval, -1);
            set => SetProperty(PropertyKeys.FailbackPollInterval, value);
        }

        /// <summary>
        /// Gets or sets whether to disable Snappy compression.
        /// </summary>
        public bool DisableSnappy
        {
            get => GetProperty<bool>(PropertyKeys.DisableSnappy, false);
            set => SetProperty(PropertyKeys.DisableSnappy, value);
        }

        #endregion

        #region Query Optimization Properties

        /// <summary>
        /// Gets or sets whether cost-based optimization is enabled. Default: true.
        /// </summary>
        public bool CostBasedOptimization
        {
            get => GetProperty<bool>(PropertyKeys.CostBasedOptimization, true);
            set => SetProperty(PropertyKeys.CostBasedOptimization, value);
        }

        /// <summary>
        /// Gets or sets whether distributed joins are enabled. Default: true.
        /// </summary>
        public bool DistributedJoins
        {
            get => GetProperty<bool>(PropertyKeys.DistributedJoins, true);
            set => SetProperty(PropertyKeys.DistributedJoins, value);
        }

        /// <summary>
        /// Gets or sets whether parallel execution is enabled. Default: true.
        /// </summary>
        public bool ParallelExecution
        {
            get => GetProperty<bool>(PropertyKeys.ParallelExecution, true);
            set => SetProperty(PropertyKeys.ParallelExecution, value);
        }

        /// <summary>
        /// Gets or sets whether plan caching is enabled. Default: true.
        /// </summary>
        public bool PlanCache
        {
            get => GetProperty<bool>(PropertyKeys.PlanCache, true);
            set => SetProperty(PropertyKeys.PlanCache, value);
        }

        /// <summary>
        /// Gets or sets whether results caching is enabled. Default: true.
        /// </summary>
        public bool ResultsCaching
        {
            get => GetProperty<bool>(PropertyKeys.ResultsCaching, true);
            set => SetProperty(PropertyKeys.ResultsCaching, value);
        }

        /// <summary>
        /// Gets or sets whether rule-based optimizations are enabled. Default: true.
        /// </summary>
        public bool RuleBasedOptimizations
        {
            get => GetProperty<bool>(PropertyKeys.RuleBasedOptimizations, true);
            set => SetProperty(PropertyKeys.RuleBasedOptimizations, value);
        }

        /// <summary>
        /// Gets or sets whether SSQ optimizations are enabled. Default: true.
        /// </summary>
        public bool SsqOptimizations
        {
            get => GetProperty<bool>(PropertyKeys.SsqOptimizations, true);
            set => SetProperty(PropertyKeys.SsqOptimizations, value);
        }

        /// <summary>
        /// Gets or sets whether to use approximate count distinct. Default: false.
        /// </summary>
        public bool UseApproxCountDistinct
        {
            get => GetProperty<bool>(PropertyKeys.UseApproxCountDistinct, false);
            set => SetProperty(PropertyKeys.UseApproxCountDistinct, value);
        }

        #endregion

        #region Query Control Properties

        /// <summary>
        /// Gets or sets whether the connection is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get => GetProperty<bool>(PropertyKeys.ReadOnly, false);
            set => SetProperty(PropertyKeys.ReadOnly, value);
        }

        /// <summary>
        /// Gets or sets the time-to-live in minutes for query results. Default: 20.
        /// </summary>
        public int Ttl
        {
            get => GetProperty<int>(PropertyKeys.Ttl, 20);
            set => SetProperty(PropertyKeys.Ttl, value);
        }

        /// <summary>
        /// Gets or sets the TTL for paging tables in minutes. Default: 20.
        /// </summary>
        public int PagingTableTtl
        {
            get => GetProperty<int>(PropertyKeys.PagingTableTtl, 20);
            set => SetProperty(PropertyKeys.PagingTableTtl, value);
        }

        /// <summary>
        /// Gets or sets the result row limit. -1 means no limit.
        /// </summary>
        public int Limit
        {
            get => GetProperty<int>(PropertyKeys.Limit, -1);
            set => SetProperty(PropertyKeys.Limit, value);
        }

        /// <summary>
        /// Gets or sets the number of rows per fetch. Default: 10000.
        /// </summary>
        public int RowsPerFetch
        {
            get => GetProperty<int>(PropertyKeys.RowsPerFetch, 10000);
            set => SetProperty(PropertyKeys.RowsPerFetch, value);
        }

        /// <summary>
        /// Gets or sets the default fetch size for paged result sets.
        /// When set to a positive value, SELECT queries will use paging to fetch results.
        /// 0 (default) means fetch all results at once.
        /// </summary>
        public int FetchSize
        {
            get => GetProperty<int>(PropertyKeys.FetchSize, 0);
            set => SetProperty(PropertyKeys.FetchSize, value);
        }

        /// <summary>
        /// Gets or sets whether to use key lookup optimization.
        /// </summary>
        public bool UseKeyLookup
        {
            get => GetProperty<bool>(PropertyKeys.UseKeyLookup, false);
            set => SetProperty(PropertyKeys.UseKeyLookup, value);
        }

        #endregion

        #region Insertion Properties

        /// <summary>
        /// Gets or sets the number of rows per insertion batch. Default: 10000.
        /// </summary>
        public int RowsPerInsertion
        {
            get => GetProperty<int>(PropertyKeys.RowsPerInsertion, 10000);
            set => SetProperty(PropertyKeys.RowsPerInsertion, value);
        }

        /// <summary>
        /// Gets or sets whether to disable multi-head insert.
        /// </summary>
        public bool DisableMultiheadInsert
        {
            get => GetProperty<bool>(PropertyKeys.DisableMultiheadInsert, false);
            set => SetProperty(PropertyKeys.DisableMultiheadInsert, value);
        }

        /// <summary>
        /// Gets or sets whether to ignore existing primary keys on insert.
        /// </summary>
        public bool IgnoreExistingPk
        {
            get => GetProperty<bool>(PropertyKeys.IgnoreExistingPk, false);
            set => SetProperty(PropertyKeys.IgnoreExistingPk, value);
        }

        /// <summary>
        /// Gets or sets whether to truncate strings that exceed column length.
        /// </summary>
        public bool TruncateStrings
        {
            get => GetProperty<bool>(PropertyKeys.TruncateStrings, false);
            set => SetProperty(PropertyKeys.TruncateStrings, value);
        }

        /// <summary>
        /// Gets or sets whether to update on existing primary key.
        /// </summary>
        public bool UpdateOnExistingPk
        {
            get => GetProperty<bool>(PropertyKeys.UpdateOnExistingPk, false);
            set => SetProperty(PropertyKeys.UpdateOnExistingPk, value);
        }

        /// <summary>
        /// Gets or sets the error mode for insertions.
        /// </summary>
        public string? ErrorMode
        {
            get => GetProperty<string?>(PropertyKeys.ErrorMode, null);
            set => SetProperty(PropertyKeys.ErrorMode, value);
        }

        /// <summary>
        /// Gets or sets the replication mode.
        /// </summary>
        public string? Replication
        {
            get => GetProperty<string?>(PropertyKeys.Replication, null);
            set => SetProperty(PropertyKeys.Replication, value);
        }

        /// <summary>
        /// Gets or sets whether to disable sync mode.
        /// </summary>
        public bool NoSync
        {
            get => GetProperty<bool>(PropertyKeys.NoSync, false);
            set => SetProperty(PropertyKeys.NoSync, value);
        }

        #endregion

        #region File I/O Properties

        /// <summary>
        /// Gets or sets the file read delimiter. Default: ",".
        /// </summary>
        public string FileReadDelimiter
        {
            get => GetProperty<string>(PropertyKeys.FileReadDelimiter, ",");
            set => SetProperty(PropertyKeys.FileReadDelimiter, value);
        }

        /// <summary>
        /// Gets or sets whether the file has a header row.
        /// </summary>
        public bool FileReadHasHeader
        {
            get => GetProperty<bool>(PropertyKeys.FileReadHasHeader, false);
            set => SetProperty(PropertyKeys.FileReadHasHeader, value);
        }

        /// <summary>
        /// Gets or sets the null string representation in files. Default: "\\N".
        /// </summary>
        public string FileReadNullString
        {
            get => GetProperty<string>(PropertyKeys.FileReadNullString, "\\N");
            set => SetProperty(PropertyKeys.FileReadNullString, value);
        }

        /// <summary>
        /// Gets or sets the file escape character.
        /// </summary>
        public string FileReadEscapeChar
        {
            get => GetProperty<string>(PropertyKeys.FileReadEscapeChar, "\\0");
            set => SetProperty(PropertyKeys.FileReadEscapeChar, value);
        }

        /// <summary>
        /// Gets or sets the file quote character. Default: "\"".
        /// </summary>
        public string FileReadQuoteChar
        {
            get => GetProperty<string>(PropertyKeys.FileReadQuoteChar, "\"");
            set => SetProperty(PropertyKeys.FileReadQuoteChar, value);
        }

        /// <summary>
        /// Gets or sets the file comment character. Default: "#".
        /// </summary>
        public string FileReadComment
        {
            get => GetProperty<string>(PropertyKeys.FileReadComment, "#");
            set => SetProperty(PropertyKeys.FileReadComment, value);
        }

        /// <summary>
        /// Gets or sets whether to clear the table initially when loading from file.
        /// </summary>
        public bool FileReadInitialClear
        {
            get => GetProperty<bool>(PropertyKeys.FileReadInitialClear, false);
            set => SetProperty(PropertyKeys.FileReadInitialClear, value);
        }

        /// <summary>
        /// Gets or sets the file read limit. Default: int.MaxValue.
        /// </summary>
        public int FileReadLimit
        {
            get => GetProperty<int>(PropertyKeys.FileReadLimit, int.MaxValue);
            set => SetProperty(PropertyKeys.FileReadLimit, value);
        }

        /// <summary>
        /// Gets or sets the number of records to skip when reading a file.
        /// </summary>
        public int FileReadSkip
        {
            get => GetProperty<int>(PropertyKeys.FileReadSkip, 0);
            set => SetProperty(PropertyKeys.FileReadSkip, value);
        }

        #endregion

        #region Connection Pooling Properties

        /// <summary>
        /// Gets or sets whether connection pooling is enabled. Default: true.
        /// </summary>
        public bool Pooling
        {
            get => GetProperty<bool>(PropertyKeys.Pooling, true);
            set => SetProperty(PropertyKeys.Pooling, value);
        }

        /// <summary>
        /// Gets or sets the maximum pool size. Default: 100.
        /// </summary>
        public int MaxPoolSize
        {
            get => GetProperty<int>(PropertyKeys.MaxPoolSize, 100);
            set => SetProperty(PropertyKeys.MaxPoolSize, value);
        }

        /// <summary>
        /// Gets or sets the minimum pool size. Default: 0.
        /// </summary>
        public int MinPoolSize
        {
            get => GetProperty<int>(PropertyKeys.MinPoolSize, 0);
            set => SetProperty(PropertyKeys.MinPoolSize, value);
        }

        #endregion

        #region Batch Insert Properties

        /// <summary>
        /// Gets or sets whether batch insert mode is enabled.
        /// When enabled, INSERT statements are buffered and sent in batches.
        /// </summary>
        public bool BatchInsertMode
        {
            get => GetProperty<bool>(PropertyKeys.BatchInsertMode, false);
            set => SetProperty(PropertyKeys.BatchInsertMode, value);
        }

        /// <summary>
        /// Gets or sets the batch size for batch insert operations. Default: 10000.
        /// </summary>
        public int BatchSize
        {
            get => GetProperty<int>(PropertyKeys.BatchSize, 10000);
            set => SetProperty(PropertyKeys.BatchSize, value);
        }

        /// <summary>
        /// Gets or sets whether to update existing records with matching primary keys during batch inserts.
        /// </summary>
        public bool BatchUpdateOnExistingPk
        {
            get => GetProperty<bool>(PropertyKeys.BatchUpdateOnExistingPk, false);
            set => SetProperty(PropertyKeys.BatchUpdateOnExistingPk, value);
        }

        #endregion

        #region Misc Properties

        /// <summary>
        /// Gets or sets the time zone override.
        /// </summary>
        public string? TimeZoneOverride
        {
            get => GetProperty<string?>(PropertyKeys.TimeZoneOverride, null);
            set => SetProperty(PropertyKeys.TimeZoneOverride, value);
        }

        /// <summary>
        /// Gets or sets the token name claim for OAuth. Default: "sub".
        /// </summary>
        public string TokenNameClaim
        {
            get => GetProperty<string>(PropertyKeys.TokenNameClaim, "sub");
            set => SetProperty(PropertyKeys.TokenNameClaim, value);
        }

        /// <summary>
        /// Gets or sets whether to use fake transactions for API compatibility.
        /// </summary>
        public bool FakeTransactions
        {
            get => GetProperty<bool>(PropertyKeys.FakeTransactions, false);
            set => SetProperty(PropertyKeys.FakeTransactions, value);
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public string? LogLevel
        {
            get => GetProperty<string?>(PropertyKeys.LogLevel, null);
            set => SetProperty(PropertyKeys.LogLevel, value);
        }

        #endregion

        #region Property Access Methods

        private T GetProperty<T>(string key, T defaultValue)
        {
            if (_properties.TryGetValue(key, out var value))
            {
                if (value == null)
                    return defaultValue;

                // Handle boolean parsing from string
                if (typeof(T) == typeof(bool) && value is string strVal)
                {
                    if (bool.TryParse(strVal, out var boolResult))
                        return (T)(object)boolResult;
                    // Also handle "1" and "0"
                    if (strVal == "1" || strVal.Equals("true", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)true;
                    if (strVal == "0" || strVal.Equals("false", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)false;
                }

                // Handle int parsing from string
                if (typeof(T) == typeof(int) && value is string strIntVal)
                {
                    if (int.TryParse(strIntVal, out var intResult))
                        return (T)(object)intResult;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            return defaultValue;
        }

        private void SetProperty(string key, object? value)
        {
            if (value != null)
                _properties[key] = value;
            else
                _properties.Remove(key);
        }

        /// <summary>
        /// Gets all properties as a dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _properties;

        /// <summary>
        /// Gets or sets a property by key.
        /// </summary>
        public object? this[string key]
        {
            get => _properties.TryGetValue(key, out var value) ? value : null;
            set => SetProperty(key, value);
        }

        #endregion

        #region Parsing

        private void ParseConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            var pairs = connectionString.Split(';');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    // Handle aliases for compatibility
                    key = NormalizePropertyKey(key);
                    _properties[key] = value;
                }
            }
        }

        private static string NormalizePropertyKey(string key)
        {
            // Normalize common aliases to canonical property names
            return key.ToUpperInvariant() switch
            {
                "URL" or "HOST" or "DATA SOURCE" => PropertyKeys.Server,
                "UID" or "USER" or "USER ID" => PropertyKeys.Username,
                "PWD" => PropertyKeys.Password,
                "INITIAL CATALOG" => PropertyKeys.Database,
                "OAUTHTOKEN" or "OAUTH TOKEN" or "OAUTH_TOKEN" => PropertyKeys.OAuthToken,
                _ => key
            };
        }

        public override string ToString()
        {
            return string.Join(";", _properties.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        #endregion

        #region Query Options Builder

        /// <summary>
        /// Builds a dictionary of query options for the executeSql API based on connection settings.
        /// </summary>
        public IDictionary<string, string> BuildQueryOptions()
        {
            var options = new Dictionary<string, string>();

            // Query optimization options
            if (!CostBasedOptimization)
                options["cost_based_optimization"] = "false";
            if (!DistributedJoins)
                options["distributed_joins"] = "false";
            if (!ParallelExecution)
                options["parallel_execution"] = "false";
            if (!PlanCache)
                options["plan_cache"] = "false";
            if (!ResultsCaching)
                options["results_caching"] = "false";
            if (!RuleBasedOptimizations)
                options["rule_based_optimizations"] = "false";
            if (!SsqOptimizations)
                options["ssq_optimization"] = "false";
            if (UseApproxCountDistinct)
                options["use_approx_count_distinct"] = "true";

            // TTL and paging
            if (Ttl != 20)
                options["ttl"] = Ttl.ToString();
            if (PagingTableTtl != 20)
                options["paging_table_ttl"] = PagingTableTtl.ToString();
            if (Limit > 0)
                options["limit"] = Limit.ToString();

            return options;
        }

        /// <summary>
        /// Builds a dictionary of insert options for the insertRecords API based on connection settings.
        /// </summary>
        public IDictionary<string, string> BuildInsertOptions()
        {
            var options = new Dictionary<string, string>();

            if (UpdateOnExistingPk)
                options["update_on_existing_pk"] = "true";
            if (IgnoreExistingPk)
                options["ignore_existing_pk"] = "true";
            if (TruncateStrings)
                options["truncate_strings"] = "true";
            if (NoSync)
                options["no_sync"] = "true";
            if (!string.IsNullOrEmpty(ErrorMode))
                options["error_mode"] = ErrorMode;
            if (!string.IsNullOrEmpty(Replication))
                options["replication_mode"] = Replication;

            return options;
        }

        #endregion
    }

    // 5. Connection Pool Implementation
    public class KineticaConnectionPool
    {
        private readonly ConcurrentDictionary<string, ConnectionPoolEntry> _pools = new();
        private readonly Timer _cleanupTimer;

        public KineticaConnectionPool()
        {
            _cleanupTimer = new Timer(CleanupExpiredConnections!, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public async Task<Kinetica> GetConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            var pool = _pools.GetOrAdd(connectionString, cs => new ConnectionPoolEntry(cs));
            return await pool.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void ReturnConnection(string connectionString, Kinetica connection)
        {
            if (_pools.TryGetValue(connectionString, out var pool))
            {
                pool.ReturnConnection(connection);
            }
        }

        private void CleanupExpiredConnections(object state)
        {
            foreach (var pool in _pools.Values)
            {
                pool.CleanupExpiredConnections();
            }
        }

        private class ConnectionPoolEntry
        {
            private readonly KineticaConnectionStringBuilder _connectionStringBuilder;
            private readonly ConcurrentQueue<PooledConnection> _connections = new();
            private readonly SemaphoreSlim _semaphore;
            private int _currentCount = 0;

            public ConnectionPoolEntry(string connectionString)
            {
                _connectionStringBuilder = new KineticaConnectionStringBuilder(connectionString);
                _semaphore = new SemaphoreSlim(_connectionStringBuilder.MaxPoolSize, _connectionStringBuilder.MaxPoolSize);
            }

            public async Task<Kinetica> GetConnectionAsync(CancellationToken cancellationToken)
            {
                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    // Try to get an existing connection
                    while (_connections.TryDequeue(out var pooledConnection))
                    {
                        if (pooledConnection.IsValid && await ValidateConnectionAsync(pooledConnection.Connection, cancellationToken).ConfigureAwait(false))
                        {
                            return pooledConnection.Connection;
                        }
                        // Note: Kinetica doesn't implement IDisposable, just release the reference
                        Interlocked.Decrement(ref _currentCount);
                    }

                    // Create a new connection with authentication
                    Kinetica.Options? options = null;

                    if (!string.IsNullOrEmpty(_connectionStringBuilder.Username) || !string.IsNullOrEmpty(_connectionStringBuilder.Password))
                    {
                        options = new Kinetica.Options
                        {
                            Username = _connectionStringBuilder.Username ?? string.Empty,
                            Password = _connectionStringBuilder.Password ?? string.Empty
                        };
                    }

                    if (!string.IsNullOrEmpty(_connectionStringBuilder.OAuthToken))
                    {
                        options ??= new Kinetica.Options();
                        options.OauthToken = _connectionStringBuilder.OAuthToken;
                    }

                    var client = new Kinetica(_connectionStringBuilder.Server, options);

                    // Test connection
                    await client.ShowSystemStatusAsync(new Dictionary<string, string>(), cancellationToken).ConfigureAwait(false);

                    Interlocked.Increment(ref _currentCount);
                    return client;
                }
                catch
                {
                    _semaphore.Release();
                    throw;
                }
            }

            private async Task<bool> ValidateConnectionAsync(Kinetica connection, CancellationToken cancellationToken)
            {
                try
                {
                    // Validate the connection is still alive by making a lightweight call
                    await connection.ShowSystemStatusAsync(new Dictionary<string, string>(), cancellationToken).ConfigureAwait(false);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public void ReturnConnection(Kinetica connection)
            {
                try
                {
                    _connections.Enqueue(new PooledConnection(connection));
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public void CleanupExpiredConnections()
            {
                var activeConnections = new List<PooledConnection>();

                while (_connections.TryDequeue(out var pooledConnection))
                {
                    if (pooledConnection.IsValid)
                    {
                        activeConnections.Add(pooledConnection);
                    }
                    else
                    {
                        // Note: Kinetica doesn't implement IDisposable, just release the reference
                        Interlocked.Decrement(ref _currentCount);
                    }
                }

                foreach (var connection in activeConnections)
                {
                    _connections.Enqueue(connection);
                }
            }

            private class PooledConnection
            {
                public Kinetica Connection { get; }
                public DateTime CreatedAt { get; }

                public PooledConnection(Kinetica connection)
                {
                    Connection = connection;
                    CreatedAt = DateTime.UtcNow;
                }

                public bool IsValid => DateTime.UtcNow - CreatedAt < TimeSpan.FromMinutes(30); // 30 minute timeout
            }
        }
    }

    // 6. SQL Parser for Better Command Handling
    /// <summary>
    /// SQL parser with support for JDBC-compatible features including:
    /// - User impersonation (SET USER, EXECUTE AS USER, REVERT)
    /// - Query optimization hints (KI_HINT_*)
    /// - Parameter substitution
    /// - Multi-statement parsing
    /// </summary>
    public class SQLParser
    {
        // Statement type patterns
        private static readonly Regex _selectRegex = new Regex(@"^\s*SELECT\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _insertRegex = new Regex(@"^\s*INSERT\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _updateRegex = new Regex(@"^\s*UPDATE\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _deleteRegex = new Regex(@"^\s*DELETE\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _createTableRegex = new Regex(@"^\s*CREATE\s+TABLE\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _dropTableRegex = new Regex(@"^\s*DROP\s+TABLE\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _parameterRegex = new Regex(@"@(\w+)", RegexOptions.Compiled);
        // JDBC-style positional parameter placeholder
        private static readonly Regex _positionalParamRegex = new Regex(@"\?", RegexOptions.Compiled);

        // User impersonation patterns (JDBC-compatible)
        private static readonly Regex _setUserRegex = new Regex(
            @"^\s*SET\s+USER\s+['""]?(\w+)['""]?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _executeAsUserRegex = new Regex(
            @"^\s*EXECUTE\s+AS\s+USER\s+['""]?(\w+)['""]?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _revertRegex = new Regex(
            @"^\s*REVERT\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // SET SCHEMA / SET SQLID pattern
        private static readonly Regex _setSchemaRegex = new Regex(
            @"^\s*SET\s+(?:SCHEMA|SQLID)\s+['""]?([.\w]+)['""]?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Query hint patterns (KI_HINT_*)
        // Matches both /*+ KI_HINT_* */ and /* KI_HINT_* */ formats
        private static readonly Regex _hintPattern = new Regex(
            @"/\*\+?\s*(?:KI_HINT_\w+(?:\([^)]*\))?\s*)+\*/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _batchSizeHint = new Regex(
            @"KI_HINT_BATCH_SIZE\s*\(\s*(\d+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _truncateStringsHint = new Regex(
            @"KI_HINT_TRUNCATE_STRINGS",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _updateOnPkHint = new Regex(
            @"KI_HINT_UPDATE_ON_EXISTING_PK",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _disableMultiheadHint = new Regex(
            @"KI_HINT_DISABLE_MULTIHEAD",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _keyLookupHint = new Regex(
            @"KI_HINT_KEY_LOOKUP",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _replSyncHint = new Regex(
            @"KI_HINT_REPL_SYNC(?:_PARALLEL)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _ignoreExistingPkHint = new Regex(
            @"KI_HINT_IGNORE_EXISTING_PK",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _serverSideInsertHint = new Regex(
            @"KI_HINT_SERVER_SIDE_INSERT",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // PK conflict predicate hints
        private static readonly Regex _pkConflictPredicateLowerHint = new Regex(
            @"KI_HINT_PK_CONFLICT_PREDICATE_LOWER\s*\(\s*(\w+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _pkConflictPredicateHigherHint = new Regex(
            @"KI_HINT_PK_CONFLICT_PREDICATE_HIGHER\s*\(\s*(\w+)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // INSERT INTO...SELECT FROM FILE pattern
        // Matches: INSERT INTO [schema.]table [(columns)] SELECT [*|(columns)] FROM FILE."path" [WITH OPTIONS (...)]
        // Also matches kifs:// paths: INSERT INTO [schema.]table [(columns)] SELECT [*|(columns)] FROM 'kifs://path' [WITH OPTIONS (...)]
        private static readonly Regex _insertFromFileRegex = new Regex(
            @"^\s*INSERT\s+INTO\s+(?<table>[\w\.]+)\s*(?:\(\s*(?<insertColumns>[\w\s,""]+)\s*\))?\s*SELECT\s+(?:(?<selectAll>\*)|(?<selectColumns>[\w\s,""]+))\s+FROM\s+(?:FILE\s*\.\s*""(?<filePath>[^""]+)""|'(?<kifsPath>kifs://[^']+)')(?:\s+WITH\s+OPTIONS\s*\(\s*(?<options>[^)]*)\s*\))?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ParsedCommand Parse(string sql, DbParameterCollection parameters)
        {
            var commandType = DetermineCommandType(sql);
            var hints = ExtractHints(sql);

            // Strip hints from SQL before parameter substitution
            var strippedSql = StripHints(sql);
            var finalSql = SubstituteParameters(strippedSql, parameters);

            // Handle user impersonation commands
            var userImpersonation = ExtractUserImpersonation(sql);

            // Parse INSERT FROM FILE details if applicable
            InsertFromFileInfo? insertFromFileInfo = null;
            if (commandType == ParsedCommandType.InsertFromFile)
            {
                insertFromFileInfo = ExtractInsertFromFileInfo(sql);
            }

            return new ParsedCommand
            {
                OriginalSql = sql,
                FinalSql = finalSql,
                CommandType = commandType,
                Parameters = parameters,
                Hints = hints,
                UserImpersonation = userImpersonation,
                InsertFromFile = insertFromFileInfo
            };
        }

        /// <summary>
        /// Strips hint comments (/*+ KI_HINT_* */) from SQL.
        /// </summary>
        private string StripHints(string sql)
        {
            // Remove hint comments in format /*+ KI_HINT_... */
            return _hintPattern.Replace(sql, "").Trim();
        }

        private ParsedCommandType DetermineCommandType(string sql)
        {
            // Check session/user management commands first
            if (_setUserRegex.IsMatch(sql)) return ParsedCommandType.SetUser;
            if (_executeAsUserRegex.IsMatch(sql)) return ParsedCommandType.ExecuteAsUser;
            if (_revertRegex.IsMatch(sql)) return ParsedCommandType.Revert;
            if (_setSchemaRegex.IsMatch(sql)) return ParsedCommandType.SetSchema;

            // Standard SQL commands
            if (_selectRegex.IsMatch(sql)) return ParsedCommandType.Select;
            // Check INSERT FROM FILE before regular INSERT
            if (_insertFromFileRegex.IsMatch(sql)) return ParsedCommandType.InsertFromFile;
            if (_insertRegex.IsMatch(sql)) return ParsedCommandType.Insert;
            if (_updateRegex.IsMatch(sql)) return ParsedCommandType.Update;
            if (_deleteRegex.IsMatch(sql)) return ParsedCommandType.Delete;
            if (_createTableRegex.IsMatch(sql)) return ParsedCommandType.CreateTable;
            if (_dropTableRegex.IsMatch(sql)) return ParsedCommandType.DropTable;
            return ParsedCommandType.Other;
        }

        /// <summary>
        /// Extracts query hints from SQL (/*+ KI_HINT_* */).
        /// </summary>
        private QueryHints ExtractHints(string sql)
        {
            var hints = new QueryHints();

            // Check for batch size hint
            var batchMatch = _batchSizeHint.Match(sql);
            if (batchMatch.Success && int.TryParse(batchMatch.Groups[1].Value, out var batchSize))
            {
                hints.BatchSize = batchSize;
            }

            // Check for boolean hints
            hints.TruncateStrings = _truncateStringsHint.IsMatch(sql);
            hints.UpdateOnExistingPk = _updateOnPkHint.IsMatch(sql);
            hints.IgnoreExistingPk = _ignoreExistingPkHint.IsMatch(sql);
            hints.DisableMultihead = _disableMultiheadHint.IsMatch(sql);
            hints.UseKeyLookup = _keyLookupHint.IsMatch(sql);
            hints.ReplicationSync = _replSyncHint.IsMatch(sql);
            hints.ServerSideInsert = _serverSideInsertHint.IsMatch(sql);

            // Check for PK conflict predicate hints
            var lowerMatch = _pkConflictPredicateLowerHint.Match(sql);
            if (lowerMatch.Success)
            {
                hints.PkConflictPredicateLowerColumn = lowerMatch.Groups[1].Value;
            }

            var higherMatch = _pkConflictPredicateHigherHint.Match(sql);
            if (higherMatch.Success)
            {
                hints.PkConflictPredicateHigherColumn = higherMatch.Groups[1].Value;
            }

            return hints;
        }

        /// <summary>
        /// Extracts schema name from SET SCHEMA command.
        /// </summary>
        public string? ExtractSchemaName(string sql)
        {
            var match = _setSchemaRegex.Match(sql);
            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Extracts user impersonation info from SET USER or EXECUTE AS USER commands.
        /// </summary>
        private UserImpersonationInfo? ExtractUserImpersonation(string sql)
        {
            var setUserMatch = _setUserRegex.Match(sql);
            if (setUserMatch.Success)
            {
                return new UserImpersonationInfo
                {
                    Type = ImpersonationType.SetUser,
                    Username = setUserMatch.Groups[1].Value
                };
            }

            var executeAsMatch = _executeAsUserRegex.Match(sql);
            if (executeAsMatch.Success)
            {
                return new UserImpersonationInfo
                {
                    Type = ImpersonationType.ExecuteAs,
                    Username = executeAsMatch.Groups[1].Value
                };
            }

            if (_revertRegex.IsMatch(sql))
            {
                return new UserImpersonationInfo
                {
                    Type = ImpersonationType.Revert,
                    Username = null
                };
            }

            return null;
        }

        /// <summary>
        /// Extracts INSERT FROM FILE information from SQL.
        /// </summary>
        private InsertFromFileInfo? ExtractInsertFromFileInfo(string sql)
        {
            var match = _insertFromFileRegex.Match(sql);
            if (!match.Success)
                return null;

            var info = new InsertFromFileInfo
            {
                TableName = match.Groups["table"].Value.Trim(),
                SelectAll = match.Groups["selectAll"].Success && match.Groups["selectAll"].Value == "*",
                FilePath = match.Groups["filePath"].Success ? match.Groups["filePath"].Value : null,
                KifsPath = match.Groups["kifsPath"].Success ? match.Groups["kifsPath"].Value : null
            };

            // Parse INSERT columns
            if (match.Groups["insertColumns"].Success && !string.IsNullOrEmpty(match.Groups["insertColumns"].Value))
            {
                info.InsertColumns = ParseColumnList(match.Groups["insertColumns"].Value);
            }

            // Parse SELECT columns
            if (!info.SelectAll && match.Groups["selectColumns"].Success && !string.IsNullOrEmpty(match.Groups["selectColumns"].Value))
            {
                info.SelectColumns = ParseColumnList(match.Groups["selectColumns"].Value);
            }

            // Parse WITH OPTIONS
            if (match.Groups["options"].Success && !string.IsNullOrEmpty(match.Groups["options"].Value))
            {
                ParseFileOptions(match.Groups["options"].Value, info.Options);
            }

            // Auto-detect format and delimiter based on file extension
            var path = info.ActualPath.ToLowerInvariant();
            if (info.Options.Format == FileFormat.Auto)
            {
                if (path.EndsWith(".parquet") || path.EndsWith(".pqt"))
                    info.Options.Format = FileFormat.Parquet;
                else if (path.EndsWith(".json") || path.EndsWith(".jsonl"))
                    info.Options.Format = FileFormat.Json;
                else if (path.EndsWith(".avro"))
                    info.Options.Format = FileFormat.Avro;
                else if (path.EndsWith(".shp"))
                    info.Options.Format = FileFormat.Shapefile;
                else
                    info.Options.Format = FileFormat.DelimitedText;
            }

            // Auto-detect delimiter for delimited text
            if (info.Options.Format == FileFormat.DelimitedText)
            {
                if (path.EndsWith(".psv"))
                    info.Options.Delimiter = '|';
                else if (path.EndsWith(".tsv"))
                    info.Options.Delimiter = '\t';
            }

            return info;
        }

        /// <summary>
        /// Parses a comma-separated column list, handling quoted identifiers.
        /// </summary>
        private static List<string> ParseColumnList(string columns)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuote = false;

            foreach (char c in columns)
            {
                if (c == '"')
                {
                    inQuote = !inQuote;
                }
                else if (c == ',' && !inQuote)
                {
                    var col = current.ToString().Trim().Trim('"');
                    if (!string.IsNullOrEmpty(col))
                        result.Add(col);
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            // Add last column
            var lastCol = current.ToString().Trim().Trim('"');
            if (!string.IsNullOrEmpty(lastCol))
                result.Add(lastCol);

            return result;
        }

        /// <summary>
        /// Parses WITH OPTIONS clause for file insert operations.
        /// </summary>
        private static void ParseFileOptions(string optionsStr, FileInsertOptions options)
        {
            // Pattern to match option = value pairs
            var optionPattern = new Regex(
                @"(\w+)\s*=\s*(?:'([^']*)'|""([^""]*)""|(\d+)|(\w+))",
                RegexOptions.IgnoreCase);

            var matches = optionPattern.Matches(optionsStr);
            foreach (Match m in matches)
            {
                var name = m.Groups[1].Value.ToUpperInvariant().Replace("_", "");
                var value = m.Groups[2].Success ? m.Groups[2].Value :
                           m.Groups[3].Success ? m.Groups[3].Value :
                           m.Groups[4].Success ? m.Groups[4].Value :
                           m.Groups[5].Value;

                switch (name)
                {
                    case "BATCHSIZE":
                        if (int.TryParse(value, out var batchSize))
                            options.BatchSize = batchSize;
                        break;
                    case "DELIMITER":
                        if (!string.IsNullOrEmpty(value))
                            options.Delimiter = value[0];
                        break;
                    case "QUOTE":
                        if (!string.IsNullOrEmpty(value))
                            options.QuoteChar = value[0];
                        break;
                    case "ESCAPE":
                        if (!string.IsNullOrEmpty(value))
                            options.EscapeChar = value[0];
                        break;
                    case "NULL":
                    case "NULLSTRING":
                        options.NullString = value;
                        break;
                    case "COMMENT":
                        options.CommentPrefix = value;
                        break;
                    case "SKIP":
                        if (int.TryParse(value, out var skip))
                            options.Skip = skip;
                        break;
                    case "LIMIT":
                        if (int.TryParse(value, out var limit))
                            options.Limit = limit;
                        break;
                    case "HEADER":
                    case "HASHEADER":
                        options.HasHeader = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                          value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                          value == "1";
                        break;
                    case "INITIALCLEAR":
                        options.InitialClear = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                              value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                              value == "1";
                        break;
                    case "ONERROR":
                    case "ERRORMODE":
                        if (Enum.TryParse<FileErrorMode>(value, true, out var errorMode))
                            options.ErrorMode = errorMode;
                        break;
                    case "IGNOREEXISTINGPK":
                        options.IgnoreExistingPk = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                                   value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                                   value == "1";
                        break;
                    case "UPDATEONEXISTINGPK":
                        options.UpdateOnExistingPk = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                                    value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                                    value == "1";
                        break;
                    case "TRUNCATESTRINGS":
                        options.TruncateStrings = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                                  value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                                  value == "1";
                        break;
                    case "DRYRUN":
                        options.DryRun = value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                        value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                                        value == "1";
                        break;
                    case "FORMAT":
                    case "FILETYPE":
                        if (Enum.TryParse<FileFormat>(value, true, out var format))
                            options.Format = format;
                        else if (value.Equals("DELIMITED_TEXT", StringComparison.OrdinalIgnoreCase) ||
                                 value.Equals("CSV", StringComparison.OrdinalIgnoreCase) ||
                                 value.Equals("TSV", StringComparison.OrdinalIgnoreCase) ||
                                 value.Equals("PSV", StringComparison.OrdinalIgnoreCase))
                            options.Format = FileFormat.DelimitedText;
                        break;
                }
            }
        }

        private string SubstituteParameters(string sql, DbParameterCollection parameters)
        {
            // First, substitute named parameters (@param style)
            var result = _parameterRegex.Replace(sql, match =>
            {
                var paramName = match.Groups[1].Value;
                var param = parameters.Cast<DbParameter>().FirstOrDefault(p =>
                    p.ParameterName.Equals($"@{paramName}", StringComparison.OrdinalIgnoreCase) ||
                    p.ParameterName.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                return FormatParameterValue(param);
            });

            // Then, substitute positional parameters (? style, JDBC-compatible)
            if (_positionalParamRegex.IsMatch(result))
            {
                var positionalParams = parameters.Cast<DbParameter>()
                    .Where(p => string.IsNullOrEmpty(p.ParameterName) || p.ParameterName.StartsWith("?"))
                    .ToList();

                // Also include numbered parameters like @0, @1, etc. that weren't already substituted
                var numberedParams = parameters.Cast<DbParameter>()
                    .Where(p => int.TryParse(p.ParameterName?.TrimStart('@'), out _))
                    .OrderBy(p => int.Parse(p.ParameterName!.TrimStart('@')))
                    .ToList();

                var allPositional = positionalParams.Count > 0 ? positionalParams : numberedParams;

                int paramIndex = 0;
                result = _positionalParamRegex.Replace(result, match =>
                {
                    if (paramIndex < allPositional.Count)
                    {
                        return FormatParameterValue(allPositional[paramIndex++]);
                    }
                    // If no more parameters, leave the ? as is (will cause SQL error)
                    return "?";
                });
            }

            return result;
        }

        private string FormatParameterValue(DbParameter? param)
        {
            if (param?.Value == null || param.Value == DBNull.Value)
                return "NULL";

            // Handle based on DbType for proper SQL formatting
            return param.DbType switch
            {
                // String types - escape single quotes
                DbType.String or DbType.AnsiString or DbType.StringFixedLength or DbType.AnsiStringFixedLength
                    => $"'{EscapeString(param.Value.ToString())}'",

                // Date/Time types - format appropriately for Kinetica
                DbType.DateTime or DbType.DateTime2 => FormatDateTime(param.Value),
                DbType.Date => FormatDate(param.Value),
                DbType.Time => FormatTime(param.Value),
                DbType.DateTimeOffset => FormatDateTimeOffset(param.Value),

                // Boolean
                DbType.Boolean => FormatBoolean(param.Value),

                // GUID/UUID
                DbType.Guid => $"'{FormatGuid(param.Value)}'",

                // Binary data
                DbType.Binary => FormatBinary(param.Value),

                // Numeric types - use invariant culture to ensure correct decimal separator
                DbType.Decimal or DbType.Currency or DbType.VarNumeric
                    => Convert.ToDecimal(param.Value).ToString(System.Globalization.CultureInfo.InvariantCulture),
                DbType.Double => Convert.ToDouble(param.Value).ToString(System.Globalization.CultureInfo.InvariantCulture),
                DbType.Single => Convert.ToSingle(param.Value).ToString(System.Globalization.CultureInfo.InvariantCulture),

                // Default: try to infer type from value
                _ => FormatValueByType(param.Value)
            };
        }

        private static string FormatValueByType(object value)
        {
            return value switch
            {
                string s => $"'{EscapeString(s)}'",
                DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss.fff}'",
                DateTimeOffset dto => $"'{dto:yyyy-MM-dd HH:mm:ss.fff}'",
                DateOnly d => $"'{d:yyyy-MM-dd}'",
                TimeOnly t => $"'{t:HH:mm:ss.fff}'",
                TimeSpan ts => $"'{ts:hh\\:mm\\:ss\\.fff}'",
                bool b => b ? "TRUE" : "FALSE",
                Guid g => $"'{g}'",
                byte[] bytes => "0x" + BitConverter.ToString(bytes).Replace("-", ""),
                decimal dec => dec.ToString(System.Globalization.CultureInfo.InvariantCulture),
                double dbl => dbl.ToString(System.Globalization.CultureInfo.InvariantCulture),
                float flt => flt.ToString(System.Globalization.CultureInfo.InvariantCulture),
                _ => value.ToString() ?? "NULL"
            };
        }

        private static string EscapeString(string? value)
        {
            if (value == null) return "";
            return value.Replace("'", "''");
        }

        private static string FormatDateTime(object value)
        {
            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd HH:mm:ss.fff}'";
            if (value is DateTimeOffset dto)
                return $"'{dto:yyyy-MM-dd HH:mm:ss.fff}'";
            return $"'{value}'";
        }

        private static string FormatDate(object value)
        {
            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd}'";
            if (value is DateTimeOffset dto)
                return $"'{dto:yyyy-MM-dd}'";
            if (value is DateOnly d)
                return $"'{d:yyyy-MM-dd}'";
            return $"'{value}'";
        }

        private static string FormatTime(object value)
        {
            if (value is DateTime dt)
                return $"'{dt:HH:mm:ss.fff}'";
            if (value is TimeSpan ts)
                return $"'{ts:hh\\:mm\\:ss\\.fff}'";
            if (value is TimeOnly t)
                return $"'{t:HH:mm:ss.fff}'";
            return $"'{value}'";
        }

        private static string FormatDateTimeOffset(object value)
        {
            if (value is DateTimeOffset dto)
                return $"'{dto:yyyy-MM-dd HH:mm:ss.fffzzz}'";
            return FormatDateTime(value);
        }

        private static string FormatBoolean(object value)
        {
            if (value is bool b)
                return b ? "TRUE" : "FALSE";
            // Handle numeric boolean representations
            if (value is int i)
                return i != 0 ? "TRUE" : "FALSE";
            if (value is string s)
                return s.Equals("true", StringComparison.OrdinalIgnoreCase) || s == "1" ? "TRUE" : "FALSE";
            return Convert.ToBoolean(value) ? "TRUE" : "FALSE";
        }

        private static string FormatGuid(object value)
        {
            if (value is Guid g)
                return g.ToString();
            return value.ToString() ?? "";
        }

        private static string FormatBinary(object value)
        {
            if (value is byte[] bytes)
            {
                // Format as hex string with 0x prefix for Kinetica
                return "0x" + BitConverter.ToString(bytes).Replace("-", "");
            }
            return "NULL";
        }
    }

    /// <summary>
    /// Represents a parsed SQL command with extracted metadata.
    /// </summary>
    public class ParsedCommand
    {
        public string OriginalSql { get; set; } = string.Empty;
        public string FinalSql { get; set; } = string.Empty;
        public ParsedCommandType CommandType { get; set; }
        public DbParameterCollection? Parameters { get; set; }
        public QueryHints Hints { get; set; } = new();
        public UserImpersonationInfo? UserImpersonation { get; set; }
        public InsertFromFileInfo? InsertFromFile { get; set; }
    }

    /// <summary>
    /// Types of parsed SQL commands.
    /// </summary>
    public enum ParsedCommandType
    {
        Select,
        Insert,
        InsertFromFile,
        Update,
        Delete,
        CreateTable,
        DropTable,
        SetUser,
        ExecuteAsUser,
        Revert,
        SetSchema,
        Other
    }

    /// <summary>
    /// Query optimization hints extracted from SQL.
    /// Supports JDBC-compatible KI_HINT_* syntax.
    /// </summary>
    public class QueryHints
    {
        /// <summary>
        /// Batch size hint for bulk operations (KI_HINT_BATCH_SIZE).
        /// </summary>
        public int? BatchSize { get; set; }

        /// <summary>
        /// Whether to truncate strings that exceed column length (KI_HINT_TRUNCATE_STRINGS).
        /// </summary>
        public bool TruncateStrings { get; set; }

        /// <summary>
        /// Whether to update on existing primary key (KI_HINT_UPDATE_ON_EXISTING_PK).
        /// </summary>
        public bool UpdateOnExistingPk { get; set; }

        /// <summary>
        /// Whether to ignore existing primary key (KI_HINT_IGNORE_EXISTING_PK).
        /// </summary>
        public bool IgnoreExistingPk { get; set; }

        /// <summary>
        /// Whether to disable multi-head insert (KI_HINT_DISABLE_MULTIHEAD).
        /// </summary>
        public bool DisableMultihead { get; set; }

        /// <summary>
        /// Whether to use key lookup optimization (KI_HINT_KEY_LOOKUP).
        /// </summary>
        public bool UseKeyLookup { get; set; }

        /// <summary>
        /// Whether to use synchronous replication (KI_HINT_REPL_SYNC).
        /// </summary>
        public bool ReplicationSync { get; set; }

        /// <summary>
        /// Whether to force server-side insert execution (KI_HINT_SERVER_SIDE_INSERT).
        /// </summary>
        public bool ServerSideInsert { get; set; }

        /// <summary>
        /// Column name for lower predicate PK conflict resolution (KI_HINT_PK_CONFLICT_PREDICATE_LOWER).
        /// When a PK conflict occurs, the row with the lower value in this column wins.
        /// </summary>
        public string? PkConflictPredicateLowerColumn { get; set; }

        /// <summary>
        /// Column name for higher predicate PK conflict resolution (KI_HINT_PK_CONFLICT_PREDICATE_HIGHER).
        /// When a PK conflict occurs, the row with the higher value in this column wins.
        /// </summary>
        public string? PkConflictPredicateHigherColumn { get; set; }

        /// <summary>
        /// Converts hints to insert options dictionary.
        /// </summary>
        public IDictionary<string, string> ToInsertOptions()
        {
            var options = new Dictionary<string, string>();

            if (TruncateStrings)
                options["truncate_strings"] = "true";
            if (UpdateOnExistingPk)
                options["update_on_existing_pk"] = "true";
            if (IgnoreExistingPk)
                options["ignore_existing_pk"] = "true";
            if (ReplicationSync)
                options["replication_mode"] = "sync";
            if (!string.IsNullOrEmpty(PkConflictPredicateLowerColumn))
                options["pk_conflict_predicate_lower"] = PkConflictPredicateLowerColumn;
            if (!string.IsNullOrEmpty(PkConflictPredicateHigherColumn))
                options["pk_conflict_predicate_higher"] = PkConflictPredicateHigherColumn;

            return options;
        }

        /// <summary>
        /// Converts hints to query options dictionary.
        /// </summary>
        public IDictionary<string, string> ToQueryOptions()
        {
            var options = new Dictionary<string, string>();

            if (UseKeyLookup)
                options["key_lookup"] = "true";

            return options;
        }
    }

    /// <summary>
    /// User impersonation information for SET USER/EXECUTE AS commands.
    /// </summary>
    public class UserImpersonationInfo
    {
        public ImpersonationType Type { get; set; }
        public string? Username { get; set; }
    }

    /// <summary>
    /// Types of user impersonation commands.
    /// </summary>
    public enum ImpersonationType
    {
        /// <summary>SET USER "username" - Changes execution context.</summary>
        SetUser,
        /// <summary>EXECUTE AS USER "username" - Temporary context switch (can be reverted).</summary>
        ExecuteAs,
        /// <summary>REVERT - Returns to previous user context.</summary>
        Revert
    }

    /// <summary>
    /// Information about an INSERT INTO...SELECT FROM FILE statement.
    /// </summary>
    public class InsertFromFileInfo
    {
        /// <summary>Target table name (may include schema).</summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>Column names specified in the INSERT INTO clause (optional).</summary>
        public List<string> InsertColumns { get; set; } = new();

        /// <summary>Column names specified in the SELECT clause (optional, * means all).</summary>
        public List<string> SelectColumns { get; set; } = new();

        /// <summary>Whether SELECT * was used.</summary>
        public bool SelectAll { get; set; }

        /// <summary>File path (local file path for FILE."path" syntax).</summary>
        public string? FilePath { get; set; }

        /// <summary>KiFS path (for kifs:// syntax).</summary>
        public string? KifsPath { get; set; }

        /// <summary>Whether this is a KiFS file operation.</summary>
        public bool IsKifsPath => !string.IsNullOrEmpty(KifsPath);

        /// <summary>The actual path to use (either FilePath or KifsPath).</summary>
        public string ActualPath => IsKifsPath ? KifsPath! : FilePath ?? string.Empty;

        /// <summary>File insert options parsed from WITH OPTIONS clause.</summary>
        public FileInsertOptions Options { get; set; } = new();
    }

    /// <summary>
    /// Options for INSERT FROM FILE operations.
    /// Matches JDBC driver's supported options.
    /// </summary>
    public class FileInsertOptions
    {
        /// <summary>File format. Default: Auto (detected from extension).</summary>
        public FileFormat Format { get; set; } = FileFormat.Auto;

        /// <summary>Batch size for bulk insert operations. Default: 10000.</summary>
        public int BatchSize { get; set; } = 10000;

        /// <summary>Field delimiter character. Default: comma for CSV, auto-detected from extension.</summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>Quote character for string fields. Default: double-quote.</summary>
        public char QuoteChar { get; set; } = '"';

        /// <summary>Escape character for special chars. Default: none.</summary>
        public char EscapeChar { get; set; } = '\0';

        /// <summary>String that represents NULL values. Default: \N.</summary>
        public string NullString { get; set; } = "\\N";

        /// <summary>Comment line prefix (lines starting with this are skipped).</summary>
        public string? CommentPrefix { get; set; }

        /// <summary>Number of lines to skip from beginning. Default: 0.</summary>
        public int Skip { get; set; } = 0;

        /// <summary>Maximum number of rows to read. Default: unlimited (0).</summary>
        public int Limit { get; set; } = 0;

        /// <summary>Whether the file has a header row.</summary>
        public bool HasHeader { get; set; } = true;

        /// <summary>Whether to clear the table before inserting. Default: false.</summary>
        public bool InitialClear { get; set; } = false;

        /// <summary>Error handling mode.</summary>
        public FileErrorMode ErrorMode { get; set; } = FileErrorMode.Abort;

        /// <summary>Whether to ignore records with duplicate primary keys. Default: false.</summary>
        public bool IgnoreExistingPk { get; set; } = false;

        /// <summary>Whether to update records with duplicate primary keys. Default: false.</summary>
        public bool UpdateOnExistingPk { get; set; } = false;

        /// <summary>Whether to truncate strings that exceed column length. Default: false.</summary>
        public bool TruncateStrings { get; set; } = false;

        /// <summary>Whether this is a dry run (validate without inserting). Default: false.</summary>
        public bool DryRun { get; set; } = false;
    }

    /// <summary>
    /// Supported file formats for INSERT FROM FILE operations.
    /// </summary>
    public enum FileFormat
    {
        /// <summary>Auto-detect format from file extension.</summary>
        Auto,
        /// <summary>Delimited text (CSV, TSV, PSV).</summary>
        DelimitedText,
        /// <summary>Apache Parquet format.</summary>
        Parquet,
        /// <summary>JSON format.</summary>
        Json,
        /// <summary>Apache Avro format.</summary>
        Avro,
        /// <summary>Shapefile format.</summary>
        Shapefile
    }

    /// <summary>
    /// Error handling modes for file insert operations.
    /// </summary>
    public enum FileErrorMode
    {
        /// <summary>Abort on first error.</summary>
        Abort,
        /// <summary>Skip problematic rows and continue.</summary>
        Skip,
        /// <summary>Attempt to insert partial records.</summary>
        Permissive
    }

    // 7. Schema Provider Implementation
    public class KineticaSchemaProvider
    {
        private readonly KineticaConnection _connection;

        public KineticaSchemaProvider(KineticaConnection connection)
        {
            _connection = connection;
        }

        public DataTable GetSchema(string collectionName, string?[]? restrictionValues)
        {
            return collectionName.ToUpper() switch
            {
                "METADATACOLLECTIONS" => GetMetaDataCollections(),
                "TABLES" => GetTables(restrictionValues),
                "COLUMNS" => GetColumns(restrictionValues),
                "VIEWS" => GetViews(restrictionValues),
                "INDEXES" => GetIndexes(restrictionValues),
                "PROCEDURES" => GetProcedures(restrictionValues),
                "USERS" => GetUsers(restrictionValues),
                "ROLES" => GetRoles(restrictionValues),
                "DATATYPES" => GetDataTypes(),
                _ => throw new ArgumentException($"Unsupported schema collection: {collectionName}")
            };
        }

        private DataTable GetMetaDataCollections()
        {
            var table = new DataTable("MetaDataCollections");
            table.Columns.Add("CollectionName", typeof(string));
            table.Columns.Add("NumberOfRestrictions", typeof(int));
            table.Columns.Add("NumberOfIdentifierParts", typeof(int));

            table.Rows.Add("MetaDataCollections", 0, 0);
            table.Rows.Add("Tables", 4, 3);
            table.Rows.Add("Columns", 4, 4);
            table.Rows.Add("Views", 3, 3);
            table.Rows.Add("Indexes", 4, 3);
            table.Rows.Add("Procedures", 4, 3);
            table.Rows.Add("Users", 1, 1);
            table.Rows.Add("Roles", 1, 1);
            table.Rows.Add("DataTypes", 0, 0);

            return table;
        }

        private DataTable GetTables(string?[]? restrictionValues)
        {
            var table = new DataTable("Tables");
            table.Columns.Add("TABLE_CATALOG", typeof(string));
            table.Columns.Add("TABLE_SCHEMA", typeof(string));
            table.Columns.Add("TABLE_NAME", typeof(string));
            table.Columns.Add("TABLE_TYPE", typeof(string));

            try
            {
                var client = _connection.GetKineticaClient();
                // showTable requires a table_name parameter - use empty string or "*" to get all tables
                var response = client.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" }
                });

                foreach (var tableName in response.table_names ?? new List<string>())
                {
                    // Apply restrictions if provided
                    if (restrictionValues != null && restrictionValues.Length > 2 &&
                        !string.IsNullOrEmpty(restrictionValues[2]) &&
                        !tableName.Equals(restrictionValues[2], StringComparison.OrdinalIgnoreCase))
                        continue;

                    table.Rows.Add(null, "public", tableName, "BASE TABLE");
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to retrieve table schema: {ex.Message}", ex);
            }

            return table;
        }

        private DataTable GetColumns(string?[]? restrictionValues)
        {
            var table = new DataTable("Columns");
            table.Columns.Add("TABLE_CATALOG", typeof(string));
            table.Columns.Add("TABLE_SCHEMA", typeof(string));
            table.Columns.Add("TABLE_NAME", typeof(string));
            table.Columns.Add("COLUMN_NAME", typeof(string));
            table.Columns.Add("ORDINAL_POSITION", typeof(int));
            table.Columns.Add("COLUMN_DEFAULT", typeof(string));
            table.Columns.Add("IS_NULLABLE", typeof(string));
            table.Columns.Add("DATA_TYPE", typeof(string));
            table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
            table.Columns.Add("NUMERIC_PRECISION", typeof(int));
            table.Columns.Add("NUMERIC_SCALE", typeof(int));

            try
            {
                var client = _connection.GetKineticaClient();
                var tablesResponse = client.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" }
                });

                foreach (var tableName in tablesResponse.table_names ?? new List<string>())
                {
                    // Apply table name restriction
                    if (restrictionValues != null && restrictionValues.Length > 2 &&
                        !string.IsNullOrEmpty(restrictionValues[2]) &&
                        !tableName.Equals(restrictionValues[2], StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        var tableInfo = client.showTable(tableName);
                        var typeSchemas = tableInfo.type_schemas;
                        var properties = tableInfo.properties;

                        if (typeSchemas != null && typeSchemas.Count > 0)
                        {
                            // type_schemas is a list of Avro schema JSON strings
                            var schemaJson = typeSchemas[0];

                            // Parse the Avro schema to extract field information
                            var avroSchema = Avro.Schema.Parse(schemaJson) as Avro.RecordSchema;
                            if (avroSchema != null)
                            {
                                // Get column properties if available (unused for now but available for future use)
                                IDictionary<string, IList<string>>? columnProperties = null;
                                if (properties != null && properties.Count > 0)
                                {
                                    columnProperties = properties[0];
                                }
                                _ = columnProperties; // Suppress unused variable warning

                                int ordinal = 1;
                                foreach (var field in avroSchema.Fields)
                                {
                                    var columnName = field.Name;
                                    var dataType = GetKineticaTypeFromAvro(field.Schema);

                                    // Apply column name restriction
                                    if (restrictionValues != null && restrictionValues.Length > 3 &&
                                        !string.IsNullOrEmpty(restrictionValues[3]) &&
                                        !columnName.Equals(restrictionValues[3], StringComparison.OrdinalIgnoreCase))
                                    {
                                        ordinal++;
                                        continue;
                                    }

                                    var isNullable = IsNullableField(field.Schema) ? "YES" : "NO";
                                    var maxLength = GetMaxLength(dataType);
                                    var (precision, scale) = GetPrecisionAndScale(dataType);

                                    table.Rows.Add(
                                        null,           // TABLE_CATALOG
                                        "public",       // TABLE_SCHEMA
                                        tableName,      // TABLE_NAME
                                        columnName,     // COLUMN_NAME
                                        ordinal,        // ORDINAL_POSITION
                                        null,           // COLUMN_DEFAULT
                                        isNullable,     // IS_NULLABLE
                                        MapKineticaTypeToSqlType(dataType), // DATA_TYPE
                                        maxLength,      // CHARACTER_MAXIMUM_LENGTH
                                        precision,      // NUMERIC_PRECISION
                                        scale           // NUMERIC_SCALE
                                    );
                                    ordinal++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip tables that can't be described
                        System.Diagnostics.Debug.WriteLine($"Failed to get schema for table {tableName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to retrieve column schema: {ex.Message}", ex);
            }

            return table;
        }

        private DataTable GetViews(string?[]? restrictionValues)
        {
            var table = new DataTable("Views");
            table.Columns.Add("TABLE_CATALOG", typeof(string));
            table.Columns.Add("TABLE_SCHEMA", typeof(string));
            table.Columns.Add("TABLE_NAME", typeof(string));
            table.Columns.Add("VIEW_DEFINITION", typeof(string));
            table.Columns.Add("VIEW_TYPE", typeof(string));
            table.Columns.Add("IS_UPDATABLE", typeof(string));

            try
            {
                var client = _connection.GetKineticaClient();
                // Get all tables/views with show_children=true to retrieve views
                var response = client.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" }
                });

                for (int i = 0; i < response.table_names.Count; i++)
                {
                    var viewName = response.table_names[i];
                    var descriptions = response.table_descriptions[i];

                    // Check if this is a view type (LOGICAL_VIEW, MATERIALIZED_VIEW, VIEW)
                    bool isView = descriptions.Any(d =>
                        d == "LOGICAL_VIEW" ||
                        d == "MATERIALIZED_VIEW" ||
                        d == "VIEW" ||
                        d == "MATERIALIZED_VIEW_MEMBER");

                    if (!isView)
                        continue;

                    // Apply restrictions if provided
                    if (restrictionValues != null && restrictionValues.Length > 2 &&
                        !string.IsNullOrEmpty(restrictionValues[2]) &&
                        !viewName.Equals(restrictionValues[2], StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Determine schema name from additional_info if available
                    string schemaName = "public";
                    string viewDefinition = "";
                    if (response.additional_info != null && i < response.additional_info.Count)
                    {
                        var info = response.additional_info[i];
                        if (info.TryGetValue("schema_name", out var schema))
                            schemaName = schema;
                        if (info.TryGetValue("request_avro_json", out var definition))
                            viewDefinition = definition;
                    }

                    // Determine view type
                    string viewType = descriptions.Contains("MATERIALIZED_VIEW") ? "MATERIALIZED" : "LOGICAL";

                    table.Rows.Add(
                        null,               // TABLE_CATALOG
                        schemaName,         // TABLE_SCHEMA
                        viewName,           // TABLE_NAME
                        viewDefinition,     // VIEW_DEFINITION
                        viewType,           // VIEW_TYPE
                        "NO"                // IS_UPDATABLE (views are generally not directly updatable)
                    );
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to retrieve views: {ex.Message}", ex);
            }

            return table;
        }

        private DataTable GetIndexes(string?[]? restrictionValues)
        {
            var table = new DataTable("Indexes");
            table.Columns.Add("TABLE_CATALOG", typeof(string));
            table.Columns.Add("TABLE_SCHEMA", typeof(string));
            table.Columns.Add("TABLE_NAME", typeof(string));
            table.Columns.Add("INDEX_NAME", typeof(string));
            table.Columns.Add("INDEX_TYPE", typeof(string));
            table.Columns.Add("COLUMN_NAME", typeof(string));
            table.Columns.Add("ORDINAL_POSITION", typeof(int));
            table.Columns.Add("IS_UNIQUE", typeof(bool));

            try
            {
                var client = _connection.GetKineticaClient();
                var tablesResponse = client.showTable("", new Dictionary<string, string>
                {
                    { "show_children", "true" }
                });

                for (int i = 0; i < tablesResponse.table_names.Count; i++)
                {
                    var tableName = tablesResponse.table_names[i];
                    var descriptions = tablesResponse.table_descriptions[i];

                    // Skip schemas and views
                    if (descriptions.Contains("SCHEMA") ||
                        descriptions.Contains("LOGICAL_VIEW") ||
                        descriptions.Contains("MATERIALIZED_VIEW"))
                        continue;

                    // Apply table name restriction
                    if (restrictionValues != null && restrictionValues.Length > 2 &&
                        !string.IsNullOrEmpty(restrictionValues[2]) &&
                        !tableName.Equals(restrictionValues[2], StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Get index information from additional_info
                    string schemaName = "public";
                    string attributeIndexes = "";

                    if (tablesResponse.additional_info != null && i < tablesResponse.additional_info.Count)
                    {
                        var info = tablesResponse.additional_info[i];
                        if (info.TryGetValue("schema_name", out var schema))
                            schemaName = schema;
                        if (info.TryGetValue("attribute_indexes", out var indexes))
                            attributeIndexes = indexes;
                    }

                    // Parse attribute_indexes (semicolon-separated list)
                    // Format: column_name or index_type@column_list@column_options
                    if (!string.IsNullOrEmpty(attributeIndexes))
                    {
                        var indexList = attributeIndexes.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        int ordinal = 1;

                        foreach (var indexEntry in indexList)
                        {
                            string indexName;
                            string indexType;
                            string columnName;

                            if (indexEntry.Contains('@'))
                            {
                                // Complex index format: index_type@column_list@options
                                var parts = indexEntry.Split('@');
                                indexType = parts[0];
                                columnName = parts.Length > 1 ? parts[1] : indexEntry;
                                indexName = $"{indexType}_{columnName.Replace(",", "_")}";
                            }
                            else
                            {
                                // Simple column index
                                indexType = "COLUMN";
                                columnName = indexEntry.Trim();
                                indexName = $"IX_{tableName}_{columnName}";
                            }

                            // Apply index name restriction
                            if (restrictionValues != null && restrictionValues.Length > 3 &&
                                !string.IsNullOrEmpty(restrictionValues[3]) &&
                                !indexName.Equals(restrictionValues[3], StringComparison.OrdinalIgnoreCase))
                                continue;

                            // Handle multiple columns in index
                            var columns = columnName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            int colOrdinal = 1;
                            foreach (var col in columns)
                            {
                                table.Rows.Add(
                                    null,               // TABLE_CATALOG
                                    schemaName,         // TABLE_SCHEMA
                                    tableName,          // TABLE_NAME
                                    indexName,          // INDEX_NAME
                                    indexType,          // INDEX_TYPE
                                    col.Trim(),         // COLUMN_NAME
                                    colOrdinal++,       // ORDINAL_POSITION
                                    false               // IS_UNIQUE (assume non-unique by default)
                                );
                            }

                            ordinal++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to retrieve indexes: {ex.Message}", ex);
            }

            return table;
        }

        private DataTable GetProcedures(string?[]? restrictionValues)
        {
            var table = new DataTable("Procedures");
            table.Columns.Add("PROCEDURE_CATALOG", typeof(string));
            table.Columns.Add("PROCEDURE_SCHEMA", typeof(string));
            table.Columns.Add("PROCEDURE_NAME", typeof(string));
            table.Columns.Add("PROCEDURE_TYPE", typeof(string));
            table.Columns.Add("EXECUTION_MODE", typeof(string));
            table.Columns.Add("COMMAND", typeof(string));

            try
            {
                var client = _connection.GetKineticaClient();

                // Get proc name filter from restrictions
                string procNameFilter = "";
                if (restrictionValues != null && restrictionValues.Length > 2 &&
                    !string.IsNullOrEmpty(restrictionValues[2]))
                {
                    procNameFilter = restrictionValues[2] ?? "";
                }

                var response = client.showProc(procNameFilter);

                for (int i = 0; i < response.proc_names.Count; i++)
                {
                    var procName = response.proc_names[i];
                    var executionMode = i < response.execution_modes.Count ? response.execution_modes[i] : "unknown";
                    var command = i < response.commands.Count ? response.commands[i] : "";

                    table.Rows.Add(
                        null,               // PROCEDURE_CATALOG
                        "public",           // PROCEDURE_SCHEMA
                        procName,           // PROCEDURE_NAME
                        "PROCEDURE",        // PROCEDURE_TYPE
                        executionMode,      // EXECUTION_MODE
                        command             // COMMAND
                    );
                }
            }
            catch (Exception ex)
            {
                // If showProc fails (e.g., no permissions), return empty table
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve procedures: {ex.Message}");
            }

            return table;
        }

        private DataTable GetUsers(string?[]? restrictionValues)
        {
            var table = new DataTable("Users");
            table.Columns.Add("USER_NAME", typeof(string));
            table.Columns.Add("USER_TYPE", typeof(string));
            table.Columns.Add("RESOURCE_GROUP", typeof(string));

            try
            {
                var client = _connection.GetKineticaClient();

                // Get user name filter from restrictions
                var names = new List<string>();
                if (restrictionValues != null && restrictionValues.Length > 0 &&
                    !string.IsNullOrEmpty(restrictionValues[0]))
                {
                    names.Add(restrictionValues[0]!);
                }

                var response = client.showSecurity(names);

                foreach (var kvp in response.types)
                {
                    var userName = kvp.Key;
                    var userType = kvp.Value;

                    // Only include users (not roles)
                    if (userType == "role")
                        continue;

                    // Get resource group if available
                    string resourceGroup = "";
                    if (response.resource_groups.TryGetValue(userName, out var rg))
                        resourceGroup = rg;

                    table.Rows.Add(
                        userName,           // USER_NAME
                        userType,           // USER_TYPE (internal_user or external_user)
                        resourceGroup       // RESOURCE_GROUP
                    );
                }
            }
            catch (Exception ex)
            {
                // If showSecurity fails (e.g., no permissions), return empty table
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve users: {ex.Message}");
            }

            return table;
        }

        private DataTable GetRoles(string?[]? restrictionValues)
        {
            var table = new DataTable("Roles");
            table.Columns.Add("ROLE_NAME", typeof(string));
            table.Columns.Add("MEMBER_ROLES", typeof(string));

            try
            {
                var client = _connection.GetKineticaClient();

                // Get role name filter from restrictions
                var names = new List<string>();
                if (restrictionValues != null && restrictionValues.Length > 0 &&
                    !string.IsNullOrEmpty(restrictionValues[0]))
                {
                    names.Add(restrictionValues[0]!);
                }

                var response = client.showSecurity(names);

                foreach (var kvp in response.types)
                {
                    var name = kvp.Key;
                    var type = kvp.Value;

                    // Only include roles
                    if (type != "role")
                        continue;

                    // Get member roles if available
                    string memberRoles = "";
                    if (response.roles.TryGetValue(name, out var roles) && roles != null)
                        memberRoles = string.Join(", ", roles);

                    table.Rows.Add(
                        name,               // ROLE_NAME
                        memberRoles         // MEMBER_ROLES
                    );
                }
            }
            catch (Exception ex)
            {
                // If showSecurity fails (e.g., no permissions), return empty table
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve roles: {ex.Message}");
            }

            return table;
        }

        private DataTable GetDataTypes()
        {
            var table = new DataTable("DataTypes");
            table.Columns.Add("TYPE_NAME", typeof(string));
            table.Columns.Add("PROVIDER_TYPE", typeof(int));
            table.Columns.Add("COLUMN_SIZE", typeof(int));
            table.Columns.Add("LITERAL_PREFIX", typeof(string));
            table.Columns.Add("LITERAL_SUFFIX", typeof(string));
            table.Columns.Add("IS_NULLABLE", typeof(bool));
            table.Columns.Add("IS_CASE_SENSITIVE", typeof(bool));
            table.Columns.Add("IS_SEARCHABLE", typeof(bool));
            table.Columns.Add("IS_UNSIGNED", typeof(bool));
            table.Columns.Add("IS_FIXED_PRECISION_SCALE", typeof(bool));
            table.Columns.Add("IS_AUTO_INCREMENT", typeof(bool));
            table.Columns.Add("MINIMUM_SCALE", typeof(short));
            table.Columns.Add("MAXIMUM_SCALE", typeof(short));

            // Kinetica supported data types
            // See: https://docs.kinetica.com/7.2/concepts/types/

            // Integer types
            table.Rows.Add("int", (int)DbType.Int32, 10, null, null, true, false, true, false, true, false, (short)0, (short)0);
            table.Rows.Add("int8", (int)DbType.SByte, 3, null, null, true, false, true, false, true, false, (short)0, (short)0);
            table.Rows.Add("int16", (int)DbType.Int16, 5, null, null, true, false, true, false, true, false, (short)0, (short)0);
            table.Rows.Add("long", (int)DbType.Int64, 19, null, null, true, false, true, false, true, false, (short)0, (short)0);
            table.Rows.Add("ulong", (int)DbType.UInt64, 20, null, null, true, false, true, true, true, false, (short)0, (short)0);

            // Floating point types
            table.Rows.Add("float", (int)DbType.Single, 7, null, null, true, false, true, false, false, false, (short)0, (short)7);
            table.Rows.Add("double", (int)DbType.Double, 15, null, null, true, false, true, false, false, false, (short)0, (short)15);
            table.Rows.Add("decimal", (int)DbType.Decimal, 38, null, null, true, false, true, false, true, false, (short)0, (short)38);

            // Boolean
            table.Rows.Add("boolean", (int)DbType.Boolean, 1, null, null, true, false, true, false, true, false, (short)0, (short)0);

            // String types
            table.Rows.Add("string", (int)DbType.String, 8000, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            // Note: Kinetica supports char1 through char256, but we represent them as string with size
            table.Rows.Add("char1", (int)DbType.StringFixedLength, 1, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char2", (int)DbType.StringFixedLength, 2, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char4", (int)DbType.StringFixedLength, 4, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char8", (int)DbType.StringFixedLength, 8, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char16", (int)DbType.StringFixedLength, 16, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char32", (int)DbType.StringFixedLength, 32, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char64", (int)DbType.StringFixedLength, 64, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char128", (int)DbType.StringFixedLength, 128, "'", "'", true, true, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("char256", (int)DbType.StringFixedLength, 256, "'", "'", true, true, true, false, false, false, (short)0, (short)0);

            // Binary
            table.Rows.Add("bytes", (int)DbType.Binary, 8000, "0x", null, true, false, false, false, false, false, (short)0, (short)0);

            // Date/Time types
            table.Rows.Add("date", (int)DbType.Date, 10, "'", "'", true, false, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("time", (int)DbType.Time, 12, "'", "'", true, false, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("datetime", (int)DbType.DateTime, 23, "'", "'", true, false, true, false, false, false, (short)0, (short)3);
            table.Rows.Add("timestamp", (int)DbType.Int64, 19, null, null, true, false, true, false, false, false, (short)0, (short)0); // milliseconds since epoch

            // Special types
            table.Rows.Add("uuid", (int)DbType.Guid, 36, "'", "'", true, false, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("ipv4", (int)DbType.String, 15, "'", "'", true, false, true, false, false, false, (short)0, (short)0);
            table.Rows.Add("json", (int)DbType.String, -1, "'", "'", true, true, true, false, false, false, (short)0, (short)0);

            // Geospatial
            table.Rows.Add("wkt", (int)DbType.String, -1, "'", "'", true, false, true, false, false, false, (short)0, (short)0);

            // Vector type (for vector search/embeddings)
            table.Rows.Add("vector", (int)DbType.Object, -1, null, null, true, false, false, false, false, false, (short)0, (short)0);

            // Array types (represented as Object since .NET doesn't have a direct DbType for arrays)
            table.Rows.Add("array", (int)DbType.Object, -1, null, null, true, false, false, false, false, false, (short)0, (short)0);

            return table;
        }

        private static string GetKineticaTypeFromAvro(Avro.Schema schema)
        {
            if (schema is Avro.UnionSchema unionSchema)
            {
                // For union types (nullable), find the non-null type
                foreach (var s in unionSchema.Schemas)
                {
                    if (s.Tag != Avro.Schema.Type.Null)
                        return GetKineticaTypeFromAvro(s);
                }
            }

            return schema.Tag switch
            {
                Avro.Schema.Type.Int => "int",
                Avro.Schema.Type.Long => "long",
                Avro.Schema.Type.Float => "float",
                Avro.Schema.Type.Double => "double",
                Avro.Schema.Type.String => "string",
                Avro.Schema.Type.Boolean => "boolean",
                Avro.Schema.Type.Bytes => "bytes",
                Avro.Schema.Type.Fixed => "bytes",
                _ => "string"
            };
        }

        private static bool IsNullableField(Avro.Schema schema)
        {
            if (schema is Avro.UnionSchema unionSchema)
            {
                return unionSchema.Schemas.Any(s => s.Tag == Avro.Schema.Type.Null);
            }
            return false;
        }

        private string MapKineticaTypeToSqlType(string kineticaType)
        {
            var typeLower = kineticaType.ToLower();

            // Handle charN types (char1, char2, char4, ..., char256)
            if (typeLower.StartsWith("char") && typeLower.Length > 4)
                return "char";

            // Handle array types
            if (typeLower.StartsWith("array"))
                return "array";

            // Handle vector types
            if (typeLower.StartsWith("vector"))
                return "vector";

            return typeLower switch
            {
                // Integer types
                "int" or "integer" or "int32" => "int",
                "int8" or "tinyint" => "tinyint",
                "int16" or "smallint" => "smallint",
                "long" or "bigint" or "int64" => "bigint",
                "ulong" or "uint64" => "bigint unsigned",

                // Floating point
                "float" or "real" => "real",
                "double" or "float8" => "float",
                "decimal" or "numeric" => "decimal",

                // Boolean
                "bool" or "boolean" => "boolean",

                // String types
                "string" or "varchar" or "text" => "varchar",
                "char" => "char",

                // Date/Time
                "date" => "date",
                "time" => "time",
                "datetime" => "datetime",
                "timestamp" => "timestamp",

                // Special types
                "uuid" => "uuid",
                "ipv4" => "varchar",
                "json" => "json",
                "wkt" => "geometry",

                // Binary
                "bytes" or "binary" => "varbinary",

                _ => "varchar"
            };
        }

        private int? GetMaxLength(string dataType)
        {
            return dataType.ToLower() switch
            {
                "string" or "varchar" => 8000, // Default max length
                "char" => 1,
                _ => null
            };
        }

        private (int? precision, int? scale) GetPrecisionAndScale(string dataType)
        {
            return dataType.ToLower() switch
            {
                "int" or "integer" => (10, 0),
                "long" or "bigint" => (19, 0),
                "float" => (24, null),
                "double" => (53, null),
                "decimal" => (18, 2), // Default precision and scale
                _ => (null, null)
            };
        }
    }

    // 8. Custom Exception Classes with Error Mapping
    public class KineticaException : SystemException
    {
        public int ErrorCode { get; }
        public string SqlState { get; }
        
        public KineticaException(string message) : base(message)
        {
            ErrorCode = -1;
            SqlState = "HY000"; // General error
        }

        public KineticaException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = MapExceptionToErrorCode(innerException);
            SqlState = MapExceptionToSqlState(innerException);
        }

        public KineticaException(string message, int errorCode, string sqlState) : base(message)
        {
            ErrorCode = errorCode;
            SqlState = sqlState;
        }

        private int MapExceptionToErrorCode(Exception exception)
        {
            return exception switch
            {
                TimeoutException => -2,
                UnauthorizedAccessException => -3,
                ArgumentException => -4,
                InvalidOperationException => -5,
                _ => -1
            };
        }

        private string MapExceptionToSqlState(Exception exception)
        {
            return exception switch
            {
                TimeoutException => "HYT00", // Timeout expired
                UnauthorizedAccessException => "28000", // Invalid authorization specification
                ArgumentException => "22000", // Data exception
                InvalidOperationException => "24000", // Invalid cursor state
                _ => "HY000" // General error
            };
        }
    }

    public class KineticaConnectionException : KineticaException
    {
        public KineticaConnectionException(string message) : base(message, -100, "08000") { }
        public KineticaConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class KineticaSqlException : KineticaException
    {
        public KineticaSqlException(string message) : base(message, -200, "42000") { }
        public KineticaSqlException(string message, Exception innerException) : base(message, innerException) { }
    }

    // 9. Parameter Classes (keeping existing implementation)
    public class KineticaParameter : DbParameter
    {
        public override DbType DbType { get; set; } = DbType.String;
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; }
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string ParameterName { get; set; } = string.Empty;
        public override int Size { get; set; }
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string SourceColumn { get; set; } = string.Empty;
        public override bool SourceColumnNullMapping { get; set; }
        public override object? Value { get; set; }

        public override void ResetDbType()
        {
            DbType = DbType.String;
        }
    }

    public class KineticaParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = new List<DbParameter>();

        public override int Count => _parameters.Count;
        public override object SyncRoot => _parameters;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (DbParameter param in values)
            {
                _parameters.Add(param);
            }
        }

        public override void Clear() => _parameters.Clear();
        public override bool Contains(object value) => _parameters.Contains((DbParameter)value);
        public override bool Contains(string value) => _parameters.Any(p => p.ParameterName == value);
        public override void CopyTo(Array array, int index) => _parameters.CopyTo((DbParameter[])array, index);
        public override System.Collections.IEnumerator GetEnumerator() => _parameters.GetEnumerator();
        public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _parameters.FindIndex(p => p.ParameterName == parameterName);
        public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _parameters.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0) RemoveAt(index);
        }

        protected override DbParameter GetParameter(int index) => _parameters[index];
        protected override DbParameter GetParameter(string parameterName)
        {
            var index = IndexOf(parameterName);
            return index >= 0 ? _parameters[index] : throw new ArgumentException($"Parameter '{parameterName}' not found");
        }

        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
                _parameters[index] = value;
            else
                _parameters.Add(value);
        }
    }

    /// <summary>
    /// Provides ADO.NET transaction API compatibility for Kinetica.
    ///
    /// <para>
    /// <strong>IMPORTANT: Kinetica does not support transactions.</strong>
    /// </para>
    ///
    /// <para>
    /// This class exists solely for API compatibility with ADO.NET patterns and tools that
    /// expect transaction support. It provides <strong>NO transactional guarantees</strong>:
    /// </para>
    ///
    /// <list type="bullet">
    ///   <item><description><strong>No Atomicity:</strong> Each SQL command executes independently and is immediately committed.</description></item>
    ///   <item><description><strong>No Isolation:</strong> Other connections can see changes immediately after each command executes.</description></item>
    ///   <item><description><strong>No Rollback:</strong> The <see cref="Rollback"/> method is a no-op; already-executed commands cannot be undone.</description></item>
    ///   <item><description><strong>IsolationLevel Ignored:</strong> The isolation level parameter is accepted but has no effect.</description></item>
    /// </list>
    ///
    /// <para>
    /// <strong>Behavior:</strong>
    /// </para>
    /// <list type="bullet">
    ///   <item><description><see cref="Commit"/>: Flushes any pending batch inserts (if batch mode is enabled) and marks the transaction as completed.</description></item>
    ///   <item><description><see cref="Rollback"/>: Marks the transaction as completed without any effect on already-executed commands.</description></item>
    /// </list>
    ///
    /// <para>
    /// If your application requires true transactional semantics, you must implement
    /// application-level compensation logic or use a different database that supports transactions.
    /// </para>
    /// </summary>
    public class KineticaTransaction : DbTransaction
    {
        private readonly KineticaConnection _connection;
        private bool _completed = false;

        public KineticaTransaction(KineticaConnection connection, IsolationLevel isolationLevel)
        {
            _connection = connection;
            IsolationLevel = isolationLevel;
        }

        /// <summary>
        /// Gets the isolation level for this transaction.
        /// <para>
        /// <strong>Note:</strong> This value is stored for API compatibility only.
        /// Kinetica does not support transaction isolation levels.
        /// </para>
        /// </summary>
        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnection DbConnection => _connection;

        /// <summary>
        /// Commits the transaction.
        /// <para>
        /// <strong>Note:</strong> Since Kinetica does not support transactions, this method
        /// only flushes any pending batch inserts (if batch mode is enabled) and marks the
        /// transaction as completed. All commands executed within this transaction have
        /// already been committed individually when they were executed.
        /// </para>
        /// </summary>
        public override void Commit()
        {
            Task.Run(async () => await CommitAsync(CancellationToken.None).ConfigureAwait(false))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Asynchronously commits the transaction.
        /// <para>
        /// <strong>Note:</strong> Since Kinetica does not support transactions, this method
        /// only flushes any pending batch inserts (if batch mode is enabled) and marks the
        /// transaction as completed. All commands executed within this transaction have
        /// already been committed individually when they were executed.
        /// </para>
        /// </summary>
        public override async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_completed)
                throw new InvalidOperationException("Transaction already completed");

            try
            {
                // Flush any pending batch inserts
                if (_connection.BatchManager != null)
                {
                    await _connection.FlushBatchAsync(cancellationToken).ConfigureAwait(false);
                }

                _completed = true;
            }
            catch (Exception ex)
            {
                throw new KineticaException($"Failed to flush batch inserts during commit: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Rolls back the transaction.
        /// <para>
        /// <strong>WARNING:</strong> This method is a no-op. Kinetica does not support rollback.
        /// Any commands that have already been executed are permanently committed and cannot be undone.
        /// This method only marks the transaction as completed for API compatibility.
        /// </para>
        /// <para>
        /// If your application requires rollback capability, you must implement application-level
        /// compensation logic (e.g., delete inserted records, restore updated values from a backup).
        /// </para>
        /// </summary>
        public override void Rollback()
        {
            if (_completed)
                throw new InvalidOperationException("Transaction already completed");

            // Kinetica does not support rollback. All executed commands are already committed.
            // This method exists only for ADO.NET API compatibility.
            _completed = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_completed)
            {
                // Mark as completed on dispose; no actual rollback occurs
                _completed = true;
            }
            base.Dispose(disposing);
        }
    }

    // 11. Factory Class
    public class KineticaProviderFactory : DbProviderFactory
    {
        public static readonly KineticaProviderFactory Instance = new KineticaProviderFactory();

        public override DbCommand CreateCommand() => new KineticaCommand();
        public override DbConnection CreateConnection() => new KineticaConnection();
        public override DbParameter CreateParameter() => new KineticaParameter();
        
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => 
            new DbConnectionStringBuilder(); // Could create custom KineticaConnectionStringBuilder wrapper

        public override bool CanCreateDataSourceEnumerator => false;
        public override DbDataSourceEnumerator CreateDataSourceEnumerator() => 
            throw new NotSupportedException("Data source enumeration not supported");
    }

    // 12. Configuration and Registration Helper
    public static class KineticaProviderRegistration
    {
        private static bool _registered = false;
        private static readonly object _lock = new object();

        public static void RegisterProvider()
        {
            if (_registered) return;

            lock (_lock)
            {
                if (_registered) return;

                try
                {
                    // Register the provider in the current application domain
                    var factoryType = typeof(KineticaProviderFactory);
                    var invariantName = "KineticaAdo";
                    
                    // This would typically be done through configuration files,
                    // but can also be done programmatically
                    DbProviderFactories.RegisterFactory(invariantName, factoryType);
                    
                    _registered = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to register Kinetica provider: {ex.Message}", ex);
                }
            }
        }
    }
}

// Usage Examples:
/*
// 1. Basic Usage with Connection Pooling
using (var connection = new KineticaConnection("Server=192.168.1.100:9191;Username=admin;Password=password;Pooling=true;Max Pool Size=50"))
{
    await connection.OpenAsync();

    using (var command = new KineticaCommand("SELECT * FROM my_table WHERE id = @id", connection))
    {
        command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 123 });

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Value: {reader[0]}");
            }
        }
    }
}

// 1b. High-Performance Batch Insert Mode
// Enable batch insert mode for high-throughput INSERT operations.
// Records are buffered and sent in batches for 10-100x better performance.
using (var connection = new KineticaConnection("Server=192.168.1.100:9191;Username=admin;Password=password;Batch Insert Mode=true;Batch Size=10000"))
{
    await connection.OpenAsync();

    // All INSERTs are automatically batched
    for (int i = 0; i < 100000; i++)
    {
        using (var command = new KineticaCommand($"INSERT INTO my_table (id, name, value) VALUES ({i}, 'name{i}', {i * 1.5})", connection))
        {
            await command.ExecuteNonQueryAsync();  // Record is buffered, not sent immediately
        }
    }

    // Explicitly flush remaining records (also happens automatically on Close/Dispose)
    long flushed = await connection.FlushBatchAsync();
    Console.WriteLine($"Flushed {flushed} records");
}

// 1c. Batch Insert with Programmatic Configuration
using (var connection = new KineticaConnection("Server=192.168.1.100:9191;Username=admin;Password=password"))
{
    // Enable batch mode programmatically
    connection.BatchInsertMode = true;
    connection.BatchSize = 5000;
    connection.BatchUpdateOnExistingPk = true;  // UPSERT behavior

    await connection.OpenAsync();

    // Insert records...
    // Records are automatically sent when BatchSize is reached

    // Check pending count
    Console.WriteLine($"Pending records: {connection.PendingBatchCount}");
}

// 2. Schema Discovery
using (var connection = new KineticaConnection(connectionString))
{
    connection.Open();
    
    // Get all tables
    var tables = connection.GetSchema("Tables");
    foreach (DataRow row in tables.Rows)
    {
        Console.WriteLine($"Table: {row["TABLE_NAME"]}");
    }
    
    // Get columns for a specific table
    var columns = connection.GetSchema("Columns", new[] { null, null, "my_table", null });
    foreach (DataRow row in columns.Rows)
    {
        Console.WriteLine($"Column: {row["COLUMN_NAME"]} ({row["DATA_TYPE"]})");
    }
}

// 3. Provider Registration and Factory Usage
KineticaProviderRegistration.RegisterProvider();

var factory = DbProviderFactories.GetFactory("KineticaAdo");
using (var connection = factory.CreateConnection())
{
    connection.ConnectionString = "Server=localhost:9191";
    connection.Open();
    
    using (var command = factory.CreateCommand())
    {
        command.Connection = connection;
        command.CommandText = "SELECT COUNT(*) FROM my_table";
        
        var count = command.ExecuteScalar();
        Console.WriteLine($"Row count: {count}");
    }
}
*/