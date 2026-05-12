# Kinetica ADO.NET Driver

A fully-featured ADO.NET data provider for Kinetica, enabling .NET applications to connect to and interact with Kinetica databases using standard ADO.NET patterns.

## Features

- **Full ADO.NET Compliance**: Implements `DbConnection`, `DbCommand`, `DbDataReader`, `DbParameter`, and `DbTransaction` (API compatibility only - see [Limitations](#limitations))
- **Connection Pooling**: Built-in connection pool with configurable size and automatic cleanup
- **Async Support**: Full async/await support for all database operations
- **High-Performance Batch Inserts**: Optional batch mode buffers INSERT statements for 10-100x performance improvement
- **File-Based Data Ingestion**: `INSERT INTO...SELECT FROM FILE` support for CSV/TSV/PSV (client-side) and Parquet/JSON/Avro/Shapefile (server-side via KiFS)
- **SQL Parameter Substitution**: Named parameter support (`@paramName`) with automatic type handling
- **Schema Discovery**: Complete schema metadata retrieval (Tables, Columns, Views, Indexes, Procedures, Users, Roles, DataTypes)
- **Authentication**: Supports username/password and OAuth token authentication
- **Provider Factory**: Standard `DbProviderFactory` implementation for dependency injection scenarios

## Installation

Add a reference to the `AdoDotnetDriver` project or the compiled `AdoDotnetDriver.dll` assembly.

### Dependencies

- .NET 8.0 or later
- Kinetica C# API (`Kinetica.dll`)
- Avro library (`Avro.dll`)

## Quick Start

### Basic Connection and Query

```csharp
using KineticaAdo;

// Create and open a connection
using var connection = new KineticaConnection("Server=localhost:9191;Username=admin;Password=secret");
await connection.OpenAsync();

// Execute a query
using var command = new KineticaCommand("SELECT * FROM my_table WHERE id = @id", connection);
command.Parameters.Add(new KineticaParameter { ParameterName = "@id", Value = 123, DbType = DbType.Int32 });

using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    Console.WriteLine($"Column1: {reader["column1"]}, Column2: {reader.GetInt32(1)}");
}
```

### Connection String Parameters

| Parameter | Description | Default |
|-----------|-------------|---------|
| `Server` | Kinetica server address (host:port) | `127.0.0.1:9191` |
| `Username` | Authentication username | (empty) |
| `Password` | Authentication password | (empty) |
| `OauthToken` | OAuth authentication token | (empty) |
| `Database` | Default database name | (empty) |
| `Pooling` | Enable connection pooling | `true` |
| `Max Pool Size` | Maximum connections in pool | `100` |
| `Min Pool Size` | Minimum connections in pool | `0` |
| `Connection Timeout` | Connection timeout in seconds | `30` |
| `Batch Insert Mode` | Enable high-performance batch inserts | `false` |
| `Batch Size` | Number of records per batch | `10000` |
| `Batch Update On Existing Pk` | Update existing records on PK conflict | `false` |

### Example Connection Strings

```
Server=192.168.1.100:9191;Username=admin;Password=secret
Server=kinetica.example.com:9191;Pooling=true;Max Pool Size=50
Server=localhost:9191;OauthToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Server=localhost:9191;Username=admin;Password=secret;Batch Insert Mode=true;Batch Size=5000
```

## Usage Examples

### ExecuteNonQuery (INSERT, UPDATE, DELETE)

```csharp
using var connection = new KineticaConnection(connectionString);
await connection.OpenAsync();

using var command = new KineticaCommand(
    "INSERT INTO users (name, email) VALUES (@name, @email)",
    connection);
command.Parameters.Add(new KineticaParameter { ParameterName = "@name", Value = "John Doe" });
command.Parameters.Add(new KineticaParameter { ParameterName = "@email", Value = "john@example.com" });

int rowsAffected = await command.ExecuteNonQueryAsync();
Console.WriteLine($"Rows affected: {rowsAffected}");
```

### ExecuteScalar

```csharp
using var command = new KineticaCommand("SELECT COUNT(*) FROM users", connection);
var count = await command.ExecuteScalarAsync();
Console.WriteLine($"Total users: {count}");
```

## High-Performance Batch Inserts

The ADO.NET driver includes a high-performance batch insert mode that can dramatically improve insert throughput by buffering INSERT statements and submitting them in batches using Kinetica's native binary protocol.

### Features

- **Multi-head Ingest** - Automatic worker discovery and shard key routing for parallel inserts
- **Backpressure Control** - Semaphore-based flow control to prevent memory exhaustion
- **Snappy Compression** - Automatic compression via Kinetica client settings
- **Parallel Flush Workers** - Concurrent batch processing for maximum throughput
- **Striped Worker Queues** - Reduced lock contention for high concurrency

### Performance Comparison

| Method | Records/Second | Notes |
|--------|---------------|-------|
| Individual `executeSql()` INSERTs | ~500-1,000 | Network round-trip per record |
| Batch Insert Mode | ~50,000-100,000+ | Binary Avro encoding, multi-head ingest, batched network calls |

### Enabling Batch Mode

**Option 1: Connection String**
```csharp
var connectionString = "Server=http://localhost:9191;Batch Insert Mode=true;Batch Size=10000";
using var connection = new KineticaConnection(connectionString);
await connection.OpenAsync();
```

**Option 2: Connection Properties**
```csharp
using var connection = new KineticaConnection("Server=http://localhost:9191");
connection.BatchInsertMode = true;
connection.BatchSize = 10000;
connection.BatchUpdateOnExistingPk = false; // Set to true for upsert behavior
await connection.OpenAsync();
```

### Batch Insert Example

```csharp
using var connection = new KineticaConnection(connectionString);
connection.BatchInsertMode = true;
connection.BatchSize = 5000;
await connection.OpenAsync();

// INSERT statements are automatically batched
for (int i = 0; i < 100000; i++)
{
    using var cmd = new KineticaCommand(
        $"INSERT INTO my_table (id, value) VALUES ({i}, {i * 1.5})",
        connection);
    await cmd.ExecuteNonQueryAsync();
}

// Check pending records
Console.WriteLine($"Pending records: {connection.PendingBatchCount}");

// Explicit flush (optional - happens automatically)
var flushed = await connection.FlushBatchAsync();
Console.WriteLine($"Flushed {flushed} records");

// Records are also auto-flushed on Close() or when batch size is reached
connection.Close();
```

### Batch Flush Triggers

Records are automatically flushed in the following scenarios:

1. **Batch Size Reached**: When the number of pending records reaches `BatchSize`
2. **Manual Flush**: Call `FlushBatch()` or `FlushBatchAsync()`
3. **Connection Close**: Closing the connection flushes any remaining records

### Batch Insert Properties

| Property | Description |
|----------|-------------|
| `BatchInsertMode` | Enable/disable batch insert mode |
| `BatchSize` | Number of records to buffer before auto-flush (default: 10,000) |
| `BatchUpdateOnExistingPk` | Update existing records on primary key conflict (upsert) |
| `PendingBatchCount` | Get the current number of pending records |
| `FlushBatch()` / `FlushBatchAsync()` | Manually flush pending records |

### Schema Discovery

The ADO.NET driver provides schema discovery through the standard `GetSchema()` method, similar to how the JDBC driver implements `DatabaseMetaData`.

```csharp
// Get all tables
DataTable tables = connection.GetSchema("Tables");
foreach (DataRow row in tables.Rows)
{
    Console.WriteLine($"Table: {row["TABLE_NAME"]}");
}

// Get columns for a specific table
DataTable columns = connection.GetSchema("Columns", new[] { null, null, "my_table", null });
foreach (DataRow row in columns.Rows)
{
    Console.WriteLine($"  Column: {row["COLUMN_NAME"]} ({row["DATA_TYPE"]})");
}

// Available schema collections:
// - MetaDataCollections
// - Tables
// - Columns
// - Views
// - Indexes
// - Procedures
// - Users
// - Roles
// - DataTypes
```

#### Schema Collections Reference

| Collection | Restrictions | Description |
|------------|--------------|-------------|
| `MetaDataCollections` | None | Lists all available schema collections |
| `Tables` | catalog, schema, table, type | Table names and types |
| `Columns` | catalog, schema, table, column | Column details including data types |
| `Views` | catalog, schema, view | View definitions and types |
| `Indexes` | catalog, schema, table, index | Index information |
| `Procedures` | catalog, schema, procedure, type | Stored procedures |
| `Users` | username | Database users |
| `Roles` | rolename | Database roles |
| `DataTypes` | None | Supported Kinetica data types |

#### JDBC vs ADO.NET Metadata Comparison

The JDBC driver implements `DatabaseMetaData` with ~150 individual methods, while ADO.NET uses the `GetSchema()` pattern with collection names. Here's how they compare:

| Feature | JDBC `DatabaseMetaData` | ADO.NET `KineticaSchemaProvider` |
|---------|-------------------------|----------------------------------|
| Tables | `getTables()` via SQL | `GetSchema("Tables")` via API |
| Columns | `getColumns()` via SQL | `GetSchema("Columns")` via API |
| Views | Part of `getTables()` | `GetSchema("Views")` via API |
| Indexes | `getIndexInfo()` via SQL | `GetSchema("Indexes")` via API |
| Procedures | `getProcedures()` via SQL | `GetSchema("Procedures")` via API |
| Primary Keys | `getPrimaryKeys()` | Not yet implemented |
| Data Types | `getTypeInfo()` via SQL | `GetSchema("DataTypes")` hardcoded |
| Users/Roles | Not available | `GetSchema("Users")`, `GetSchema("Roles")` |
| DB Capabilities | `supports*()` methods | Not applicable to ADO.NET pattern |
| Max Limits | `getMax*()` methods | Not applicable to ADO.NET pattern |

**Implementation Differences:**
- JDBC queries `information_schema` tables directly via SQL
- ADO.NET uses Kinetica API calls (`showTable`, `showProc`, `showSecurity`)

### Using DbProviderFactory

```csharp
// Register the provider (typically done at application startup)
KineticaProviderRegistration.RegisterProvider();

// Use the factory
var factory = DbProviderFactories.GetFactory("KineticaAdo");
using var connection = factory.CreateConnection();
connection.ConnectionString = "Server=localhost:9191";
await connection.OpenAsync();

using var command = factory.CreateCommand();
command.Connection = connection;
command.CommandText = "SELECT * FROM my_table";

using var reader = await command.ExecuteReaderAsync();
// ... process results
```

## API Reference

### KineticaConnection

| Member | Description |
|--------|-------------|
| `ConnectionString` | Gets or sets the connection string |
| `State` | Gets the current connection state |
| `Database` | Gets the current database name |
| `DataSource` | Gets the server address |
| `ServerVersion` | Gets the server version |
| `Open()` / `OpenAsync()` | Opens the connection |
| `Close()` | Closes the connection (flushes pending batches) |
| `CreateCommand()` | Creates a new command |
| `BeginTransaction()` | Returns a transaction object (API compatibility only - see [Limitations](#limitations)) |
| `GetSchema()` | Retrieves schema information |
| `ChangeDatabase()` | Changes the current database |
| `BatchInsertMode` | Gets or sets whether batch insert mode is enabled |
| `BatchSize` | Gets or sets the batch size |
| `BatchUpdateOnExistingPk` | Gets or sets upsert behavior |
| `PendingBatchCount` | Gets the number of pending records |
| `FlushBatch()` / `FlushBatchAsync()` | Flushes pending batch inserts |

### KineticaCommand

| Member | Description |
|--------|-------------|
| `CommandText` | Gets or sets the SQL command |
| `CommandTimeout` | Gets or sets the timeout in seconds (default: 30) |
| `CommandType` | Gets or sets the command type |
| `Connection` | Gets or sets the connection |
| `Parameters` | Gets the parameter collection |
| `Transaction` | Gets or sets the transaction (API compatibility only) |
| `ExecuteNonQuery()` / `ExecuteNonQueryAsync()` | Executes a non-query command |
| `ExecuteReader()` / `ExecuteReaderAsync()` | Executes a query and returns a reader |
| `ExecuteScalar()` / `ExecuteScalarAsync()` | Executes and returns the first column of the first row |
| `Prepare()` | Validates the command |
| `Cancel()` | Cancels the current operation |
| `CreateParameter()` | Creates a new parameter |

### KineticaDataReader

| Member | Description |
|--------|-------------|
| `FieldCount` | Gets the number of columns |
| `HasRows` | Indicates if there are rows |
| `IsClosed` | Indicates if the reader is closed |
| `RecordsAffected` | Gets the number of affected records |
| `Read()` / `ReadAsync()` | Advances to the next row |
| `GetValue()` | Gets the value at the specified ordinal |
| `GetString()`, `GetInt32()`, etc. | Typed accessors |
| `GetName()` | Gets the column name |
| `GetOrdinal()` | Gets the column ordinal by name |
| `GetFieldType()` | Gets the .NET type of a column |
| `GetDataTypeName()` | Gets the database type name |
| `IsDBNull()` | Checks if a value is null |
| `GetBytes()` / `GetChars()` | Streaming access for large data |

### KineticaParameter

| Member | Description |
|--------|-------------|
| `ParameterName` | Gets or sets the parameter name |
| `Value` | Gets or sets the parameter value |
| `DbType` | Gets or sets the data type |
| `Direction` | Gets or sets the parameter direction |
| `IsNullable` | Gets or sets nullability |
| `Size` | Gets or sets the size |

## Supported Data Types

| Kinetica Type | .NET Type | DbType |
|---------------|-----------|--------|
| `int` | `Int32` | `DbType.Int32` |
| `long` | `Int64` | `DbType.Int64` |
| `float` | `Single` | `DbType.Single` |
| `double` | `Double` | `DbType.Double` |
| `string` | `String` | `DbType.String` |
| `bytes` | `Byte[]` | `DbType.Binary` |
| `boolean` | `Boolean` | `DbType.Boolean` |
| `timestamp` | `DateTime` | `DbType.DateTime` |
| `date` | `DateTime` | `DbType.Date` |
| `time` | `DateTime` | `DbType.Time` |
| `decimal` | `Decimal` | `DbType.Decimal` |
| `uuid` | `Guid` | `DbType.Guid` |

## Error Handling

The driver throws specific exception types for different error conditions:

```csharp
try
{
    await connection.OpenAsync();
    // ... operations
}
catch (KineticaConnectionException ex)
{
    // Connection-related errors (SqlState: 08000)
    Console.WriteLine($"Connection error: {ex.Message}");
}
catch (KineticaSqlException ex)
{
    // SQL syntax or execution errors (SqlState: 42000)
    Console.WriteLine($"SQL error: {ex.Message}");
}
catch (KineticaException ex)
{
    // General Kinetica errors
    Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message} (SqlState: {ex.SqlState})");
}
```

## Connection Pooling

Connection pooling is enabled by default and helps improve performance by reusing connections:

- Connections are validated before reuse (using `showSystemStatus`)
- Expired connections (>30 minutes) are automatically cleaned up
- Pool size is configurable via connection string

To disable pooling:
```
Server=localhost:9191;Pooling=false
```

## Thread Safety

- `KineticaConnection`: Not thread-safe. Use one connection per thread or implement synchronization.
- `KineticaConnectionPool`: Thread-safe. Shared across all connections.
- `KineticaCommand`: Not thread-safe. Create new commands for each operation.
- `KineticaDataReader`: Not thread-safe. Read sequentially on a single thread.

### Async/Await Best Practices

The driver is designed to be safe from deadlocks when mixing synchronous and asynchronous code:

- All async methods use `ConfigureAwait(false)` internally to avoid capturing synchronization contexts
- Synchronous methods (e.g., `Open()`, `ExecuteNonQuery()`) safely wrap async operations using `Task.Run()` to prevent deadlocks in UI or ASP.NET contexts
- **Recommended**: Use async methods (`OpenAsync()`, `ExecuteNonQueryAsync()`, etc.) whenever possible for best performance

### Batch Insert Best Practices

```csharp
// Best practice: explicitly flush before closing
using var connection = new KineticaConnection(connectionString);
connection.BatchInsertMode = true;
await connection.OpenAsync();

// ... insert operations ...

// Explicitly flush to ensure all records are committed
var flushed = await connection.FlushBatchAsync();
Console.WriteLine($"Flushed {flushed} records");

// Close is now safe
connection.Close();
```

## File-Based Data Ingestion

The ADO.NET driver supports importing data from various file formats directly into Kinetica tables using the `INSERT INTO...SELECT FROM FILE` SQL syntax.

### Supported File Formats

| Format | Extension | Processing | Description |
|--------|-----------|------------|-------------|
| CSV | `.csv` | Client-side | Comma-separated values |
| TSV | `.tsv` | Client-side | Tab-separated values |
| PSV | `.psv` | Client-side | Pipe-separated values |
| Parquet | `.parquet` | Server-side (KiFS) | Apache Parquet columnar format |
| JSON | `.json` | Server-side (KiFS) | JSON records |
| Avro | `.avro` | Server-side (KiFS) | Apache Avro binary format |
| Shapefile | `.shp` | Server-side (KiFS) | ESRI Shapefile for geospatial data |

**Note**: Server-side formats (Parquet, JSON, Avro, Shapefile) require files to be stored in KiFS and are processed using Kinetica's `InsertRecordsFromFiles` API.

### Basic File Import

```csharp
using var connection = new KineticaConnection(connectionString);
await connection.OpenAsync();

// Import all columns from a local CSV file
using var cmd = new KineticaCommand(
    @"INSERT INTO users SELECT * FROM FILE.""/data/users.csv""",
    connection);
var rowsInserted = await cmd.ExecuteNonQueryAsync();
Console.WriteLine($"Imported {rowsInserted} rows");
```

### Import with Column Mapping

```csharp
// Map specific file columns to table columns
using var cmd = new KineticaCommand(
    @"INSERT INTO users (user_id, user_name, user_email)
      SELECT id, name, email FROM FILE.""/data/users.csv""",
    connection);
await cmd.ExecuteNonQueryAsync();
```

### Import from KiFS

```csharp
// Import from Kinetica File System (server-side file)
using var cmd = new KineticaCommand(
    @"INSERT INTO users SELECT * FROM 'kifs://data/users.csv'",
    connection);
await cmd.ExecuteNonQueryAsync();
```

### Import Parquet Files (Server-Side)

```csharp
// Import Parquet file from KiFS (processed server-side)
using var cmd = new KineticaCommand(
    @"INSERT INTO analytics_data SELECT * FROM 'kifs://data/events.parquet'",
    connection);
await cmd.ExecuteNonQueryAsync();

// Import with explicit format specification
using var cmd2 = new KineticaCommand(
    @"INSERT INTO analytics_data SELECT * FROM 'kifs://data/events.dat'
      WITH OPTIONS (FORMAT = PARQUET)",
    connection);
await cmd2.ExecuteNonQueryAsync();
```

### Import JSON Files (Server-Side)

```csharp
// Import JSON records from KiFS
using var cmd = new KineticaCommand(
    @"INSERT INTO logs SELECT * FROM 'kifs://data/app_logs.json'",
    connection);
await cmd.ExecuteNonQueryAsync();
```

### Import with Options

```csharp
using var cmd = new KineticaCommand(@"
    INSERT INTO users SELECT * FROM FILE.""/data/users.psv""
    WITH OPTIONS (
        BATCH_SIZE = 5000,
        DELIMITER = '|',
        SKIP = 1,
        HEADER = TRUE,
        ON_ERROR = SKIP,
        UPDATE_ON_EXISTING_PK = TRUE
    )", connection);
await cmd.ExecuteNonQueryAsync();
```

### Supported File Import Options

| Option | Description | Default |
|--------|-------------|---------|
| `BATCH_SIZE` | Records per batch for bulk insert | 10000 |
| `DELIMITER` | Field delimiter character | `,` (auto-detects from extension) |
| `QUOTE` | Quote character for string fields | `"` |
| `ESCAPE` | Escape character | (none) |
| `NULL` | String representing NULL values | `\N` |
| `COMMENT` | Comment line prefix | (none) |
| `SKIP` | Lines to skip from beginning | 0 |
| `LIMIT` | Max rows to read (0 = unlimited) | 0 |
| `HEADER` | File has header row | TRUE |
| `INITIAL_CLEAR` | Clear table before inserting | FALSE |
| `ON_ERROR` | `ABORT`, `SKIP`, or `PERMISSIVE` | ABORT |
| `IGNORE_EXISTING_PK` | Ignore duplicate primary keys | FALSE |
| `UPDATE_ON_EXISTING_PK` | Update on duplicate primary keys | FALSE |
| `TRUNCATE_STRINGS` | Truncate oversized strings | FALSE |
| `DRY_RUN` | Validate without inserting | FALSE |
| `FORMAT` | File format: `DELIMITED_TEXT`, `PARQUET`, `JSON`, `AVRO`, `SHAPEFILE` | Auto-detected from extension |

### Auto-Detection

The driver automatically detects:
- **File format by extension**: `.csv`/`.tsv`/`.psv` → client-side parsing, `.parquet`/`.json`/`.avro`/`.shp` → server-side API
- **Delimiter by file extension**: `.csv` → `,`, `.psv` → `|`, `.tsv` → `\t`
- **Column mapping**: Matches file header names to table column names

### Glob Pattern Support

```csharp
// Import multiple files matching a pattern
using var cmd = new KineticaCommand(
    @"INSERT INTO logs SELECT * FROM FILE.""/data/logs/*.csv""",
    connection);
await cmd.ExecuteNonQueryAsync();
```

## Key Lookup and Multi-Head Retrieval

### JDBC vs ADO.NET Approach

The JDBC driver uses `RecordRetriever` for optimized key-based lookups. This section explains what that means and why the ADO.NET driver takes a different approach.

#### What JDBC Does with RecordRetriever

The JDBC driver (`KiRecordRetriever`) detects SELECT queries that filter on shard/primary keys and routes them through a special path:

1. **Query Detection**: Parses SQL to check if it's a key-based lookup
2. **Validation**: Verifies all shard key columns are specified with equality operators
3. **Direct Routing**: Uses `getRecords` API to bypass the head node and talk directly to worker nodes

#### The Multi-Head Benefit

| Approach | API | Network Path | Hops |
|----------|-----|--------------|------|
| `executeSql()` | SQL | Client → Head Node → Workers → Head Node → Client | 2 |
| `getRecords` via RecordRetriever | Binary | Client → **Direct to Worker** → Client | 1 |

With `RecordRetriever`:
- Client computes the shard hash locally
- Client routes directly to the correct worker node
- No head node involvement = lower latency for point lookups

#### Why ADO.NET Uses executeSql Instead

The ADO.NET driver uses `executeSql` for all queries. Here's why:

1. **Server-Side Optimization Exists**: When you query by shard key, Kinetica's query planner uses `FAST_INDEX_LOOKUP` internally, routing to the correct worker server-side.

2. **Marginal Benefit for Most Use Cases**:
   - For bulk operations, `executeSql` performs well
   - The direct-routing benefit is most noticeable for high-frequency single-row lookups (thousands/second)
   - Most applications don't need sub-millisecond point lookup optimization

3. **Reduced Complexity**:
   - No SQL parsing to detect key-lookup eligibility
   - No client-side shard key computation
   - No worker URL mapping maintenance
   - Simpler codebase, easier to maintain

4. **Available Components**: The C# API includes `RecordRetriever<T>` and `DynamicRecordRetriever` classes that could be integrated if needed.

#### When Multi-Head Retrieval Would Help

Consider adding true multi-head retrieval if your application has:
- High-frequency point lookups (thousands per second)
- Latency-sensitive single-row reads where every millisecond matters
- Many concurrent single-row read operations

#### Feature Comparison

| Feature | JDBC Driver | ADO.NET Driver |
|---------|-------------|----------------|
| Key lookup detection | ✅ Automatic SQL parsing | ❌ Not implemented |
| Multi-head direct routing | ✅ Via `RecordRetriever` | ❌ Uses head node |
| Server-side optimization | ✅ `FAST_INDEX_LOOKUP` | ✅ `FAST_INDEX_LOOKUP` |
| Performance for bulk queries | Good | Good |
| Performance for point lookups | Best (direct routing) | Good (server-optimized) |
| Implementation complexity | Higher | Lower |

---

## Limitations

1. **Transactions**: Kinetica does not support transactions. The `KineticaTransaction` class exists **only for ADO.NET API compatibility**:
   - **No Atomicity**: Each SQL command executes and commits immediately when called
   - **No Isolation**: Other connections see changes immediately after each command
   - **No Rollback**: `Rollback()` is a no-op and cannot undo already-executed commands
   - **IsolationLevel Ignored**: The isolation level parameter is accepted but has no effect
   - If your application requires transactional guarantees, you must implement application-level compensation logic
2. **Multiple Result Sets**: Not supported. `NextResult()` always returns `false`.
3. **Stored Procedures**: Limited support through `ExecuteNonQuery` with procedure calls.
4. **Data Source Enumeration**: Not supported (`CanCreateDataSourceEnumerator` returns `false`).

---

## Test Coverage

The ADO.NET driver includes a comprehensive test suite with **240+ tests** covering all major components including batch insert and file operations functionality.

### Test Categories

#### Unit Tests

| Test Class | Tests | Description |
|------------|-------|-------------|
| `ConnectionStringBuilderTests` | 15 | Connection string parsing, property getters/setters, default values |
| `ConnectionTests` | 10 | Connection state management, command creation |
| `CommandTests` | 14 | Command properties, parameter handling, execution validation |
| `ParameterTests` | 10 | Parameter properties, collection operations |
| `ProviderFactoryTests` | 8 | Factory methods, singleton pattern, object creation |
| `TransactionTests` | 6 | Transaction API compatibility (state management only - no transactional behavior) |
| `SQLParserTests` | 55+ | SQL command type detection, parameter substitution, INSERT FROM FILE parsing, file format detection |
| `CsvParserTests` | 16 | CSV/TSV/PSV parsing, quoted fields, delimiters, escape characters |
| `ExceptionTests` | 10 | Exception types, error codes, SQL states |
| `InsertStatementParserTests` | 36 | INSERT statement parsing, edge cases, special characters |

#### Integration Tests

| Test Class | Tests | Description |
|------------|-------|-------------|
| `IntegrationTests` | 43+ | Full CRUD operations, schema discovery, pooling |
| `IntegrationSystemTests` | 4 | System-level tests, provider factory, multiple connections |
| `BatchInsertTests` | 14 | Batch insert mode, various data types, flush behavior, auto-flush |

**Integration Test Requirements:**
- Running Kinetica server at `http://localhost:9191`
- User: `admin`
- Password: `secret`

### Test Details

#### ConnectionStringBuilderTests
- Default values for all properties
- Parsing connection strings with various parameters
- Server, Username, Password, Database properties
- Pooling and pool size configuration
- OAuth token support
- ToString() serialization

#### ConnectionTests
- Default connection state (Closed)
- Setting and getting connection string
- DataSource property extraction
- Command creation returns correct type
- Transaction object creation (API compatibility)
- Database change operations
- Close on already closed connection

#### CommandTests
- Default command type and timeout
- CommandText property
- Parameter creation and collection
- Constructor overloads
- Execution without connection throws
- Execution without command text throws
- Prepare validation
- Cancel operation
- Multiple dispose calls

#### ParameterTests
- Default property values
- Property setters
- ResetDbType functionality
- Collection Add/Remove operations
- Collection indexing by name and position
- AddRange and Insert operations
- Clear and RemoveAt operations

#### SQLParserTests
- Detection of SELECT, INSERT, UPDATE, DELETE commands
- Detection of CREATE TABLE, DROP TABLE commands
- Detection of INSERT FROM FILE commands
- Unknown command type handling
- Parameter substitution for strings, integers, booleans
- DateTime parameter formatting
- NULL parameter handling
- Multiple parameter substitution
- Empty parameter collection handling
- INSERT FROM FILE parsing:
  - Table name extraction (with schema support)
  - File path extraction (local and KiFS)
  - Column list parsing (INSERT and SELECT)
  - WITH OPTIONS parsing (all supported options)
  - Auto-detection of delimiter from file extension
- File format detection:
  - CSV, TSV, PSV → DelimitedText
  - Parquet, JSON, Avro, Shapefile → respective formats
  - FORMAT option override
  - Server-side routing for binary formats

#### CsvParserTests
- Simple CSV line parsing
- Quoted fields with embedded delimiters
- Escaped quotes (double-quote escaping)
- Empty fields handling
- Pipe-separated values (PSV)
- Tab-separated values (TSV)
- Custom quote characters
- Escape character handling
- Numeric values
- Mixed content parsing
- Single field lines
- Empty lines
- Whitespace preservation
- Newlines in quoted fields

#### TransactionTests (API Compatibility Only)
> **Note**: These tests verify the ADO.NET API contract, not transactional behavior.
> Kinetica does not support transactions.

- Isolation level is stored (but has no effect)
- Connection reference is set correctly
- Rollback marks transaction as completed
- Commit after rollback throws
- Dispose marks transaction as completed
- Multiple dispose calls are safe

#### ExceptionTests
- KineticaException with message
- KineticaException with inner exception
- Error code and SQL state mapping
- KineticaConnectionException properties
- KineticaSqlException properties
- Exception inheritance

#### IntegrationTests (Requires Kinetica Server)
- **Connection Tests**: Open/close, async open, invalid credentials, invalid server
- **Create Table Tests**: Simple table creation, all data types
- **Insert Tests**: Single row, multiple rows, with parameters, with NULL values
- **Select Tests**: All rows, WHERE clause, ORDER BY, LIMIT, specific columns
- **Update Tests**: Single row, multiple rows, with parameters
- **Delete Tests**: Single row, multiple rows, all rows
- **ExecuteScalar Tests**: COUNT, SUM, MAX, empty results
- **DataReader Tests**: GetOrdinal, GetName, typed getters, IsDBNull, GetValues, indexers, HasRows, FieldCount
- **Schema Discovery Tests**: MetaDataCollections, Tables, Columns, DataTypes
- **Connection Pooling Tests**: Reuse connections, disabled pooling
- **Concurrent Operations Tests**: Multiple concurrent readers
- **Special Characters Tests**: Special characters, Unicode

#### IntegrationSystemTests (Requires Kinetica Server)
- ShowSystemStatus works
- Simple SELECT works
- ProviderFactory creates valid connection
- Multiple sequential connections work

### Running Tests

#### Prerequisites

- .NET 8.0 SDK or later
- xUnit test framework (automatically restored via NuGet)

#### Command Line

```bash
# Navigate to the solution directory
cd /path/to/gpudb-api-cs

# Run all tests (unit + integration)
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj

# Run with detailed output
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --logger "console;verbosity=detailed"

# Run with code coverage (requires coverlet)
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~ConnectionTests"

# Run specific test method
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName=AdoDotnetDriver.Tests.CommandTests.KineticaCommand_DefaultCommandType_IsText"

# Run tests and generate test results file
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --logger "trx;LogFileName=test-results.trx"

# Run tests in Release configuration
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj -c Release
```

#### Running Unit Tests Only (No Kinetica Server Required)

```bash
# Run only unit tests (exclude integration tests)
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName!~Integration"

# Run specific unit test classes
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~ConnectionStringBuilderTests"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~ParameterTests"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~SQLParserTests"
```

#### Running Integration Tests Only (Requires Kinetica Server)

**Prerequisites:**
- Kinetica server running at `http://localhost:9191`
- Valid credentials: `admin` / `secret`

```bash
# Run all integration tests
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~Integration"

# Run specific integration test categories
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.Connection"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.Insert"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.Select"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.Update"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.Delete"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.DataReader"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.GetSchema"
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationTests.GetSchema"

# Run integration system tests
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj --filter "FullyQualifiedName~IntegrationSystemTests"
```

#### Visual Studio / Rider

1. Open the solution `gpudb-api-cs.sln`
2. Build the solution
3. Open Test Explorer (Test > Test Explorer in Visual Studio)
4. Run all tests or select specific tests to run

#### Expected Output

**All tests (Kinetica server required):**
```
Passed!  - Failed:     0, Passed:   190, Skipped:     0, Total:   190, Duration: 12 s
```

### Adding New Tests

Test files are located in `AdoDotnetDriver.Tests/` directory. To add new tests:

1. Create a new test class or add to existing class
2. Use `[Fact]` attribute for single tests or `[Theory]` for parameterized tests
3. Follow the naming convention: `ClassName_MethodOrScenario_ExpectedBehavior`

Example:
```csharp
[Fact]
public void KineticaConnection_OpenTwice_DoesNotThrow()
{
    using var conn = new KineticaConnection("Server=localhost:9191");
    // Test implementation
}
```

---

## Project Structure

```
AdoDotnetDriver/
├── KineticaAdo.cs           # Main ADO.NET implementation
│   ├── KineticaConnection   # DbConnection implementation
│   ├── KineticaCommand      # DbCommand implementation
│   ├── KineticaDataReader   # DbDataReader implementation
│   ├── KineticaTransaction  # DbTransaction implementation
│   ├── KineticaParameter    # DbParameter implementation
│   ├── KineticaConnectionStringBuilder
│   ├── KineticaConnectionPool
│   ├── KineticaSchemaProvider
│   ├── SQLParser            # SQL parsing and parameter substitution
│   ├── InsertFromFileInfo   # File import configuration
│   ├── FileInsertOptions    # File import options
│   └── KineticaException    # Custom exception types
│
├── InsertBatchManager.cs    # High-performance batch insert support
│   ├── InsertBatchManager   # Manages batch contexts per table
│   ├── TableBatchContext    # Per-table batching logic
│   ├── DynamicAvroEncoder   # Avro binary encoding for dynamic data
│   ├── InsertStatementParser # SQL INSERT parsing
│   ├── InsertBatchOptions   # Batch configuration
│   └── ParsedInsert         # Parsed INSERT result
│
├── CsvFileReader.cs         # File-based data ingestion
│   ├── CsvFileReader        # Reads CSV/TSV/PSV files and bulk inserts
│   └── CsvParser            # CSV line parsing with quote/escape handling
│
└── AdoDotnetDriver.csproj   # Project file
```

---

## Building

```bash
# Build just the driver
dotnet build AdoDotnetDriver/AdoDotnetDriver.csproj

# Build in Release mode
dotnet build AdoDotnetDriver/AdoDotnetDriver.csproj -c Release

# Build the entire solution
dotnet build gpudb-api-cs.sln
```

## License

This driver is part of the Kinetica C# API and is subject to the same license terms.

## Support

For issues and feature requests, please contact Kinetica support or submit an issue to the repository.
