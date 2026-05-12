# Kinetica.AsyncTests

Async integration tests for the Kinetica C# API.

## Overview

This test project contains comprehensive async tests that verify all asynchronous methods in the Kinetica C# API work correctly. The tests are separated from the main `Kinetica.Tests` project to:

- Provide clear separation between sync and async test suites
- Enable independent test execution and filtering
- Organize async-specific test scenarios
- Validate the async HTTP transport layer and 632 async endpoint methods

## Test Structure

```
Kinetica.AsyncTests/
├── Common/
│   └── TestContext.cs          # Test context helper for schema isolation
└── IntegrationTests/
    ├── AsyncBasicTests.cs      # Core async API operations (7 tests)
    ├── AsyncInsertRecordsTests.cs  # Async insert operations (4 tests)
    └── AsyncSqlExecutionTests.cs   # Async SQL operations (7 tests)
```

## Current Test Coverage

**Total: 18 async tests**

### AsyncBasicTests.cs (7 tests)
- `TestHasTypeAsync()` - Async type creation and existence checking
- `TestHasTableAsync()` - Async table creation and existence checking
- `TestCreateAndClearTableAsync()` - Async table lifecycle
- `TestShowSystemPropertiesAsync()` - Async system properties retrieval
- `TestShowSystemStatusAsync()` - Async system status retrieval
- `TestConcurrentAsyncOperations()` - 5 concurrent table creates
- `TestAsyncWithCancellationToken()` - Cancellation token support

### AsyncInsertRecordsTests.cs (4 tests)
- `TestInsertRecordsJsonAsync()` - Basic async inserts via SQL
- `TestInsertRecordsUpdateOnExistingPkAsync()` - Primary key updates
- `TestInsertRecordsLargeBatchAsync()` - 100 concurrent inserts in batches
- `TestParallelInsertDifferentTablesAsync()` - 5 parallel table operations

### AsyncSqlExecutionTests.cs (7 tests)
- `TestExecuteSqlSelectAsync()` - Basic async SELECT
- `TestExecuteSqlWithLimitAsync()` - LIMIT clause
- `TestExecuteSqlAggregateAsync()` - GROUP BY and aggregation
- `TestExecuteSqlCreateDropTableAsync()` - DDL operations
- `TestExecuteSqlWithFilterAsync()` - WHERE clause filtering
- `TestConcurrentSqlQueriesAsync()` - 10 concurrent queries
- `TestExecuteSqlShowTablesAsync()` - SHOW TABLES command

## Running Tests

### Run all async tests:
```bash
dotnet test Kinetica.AsyncTests
```

### Run with verbose output:
```bash
dotnet test Kinetica.AsyncTests --logger "console;verbosity=normal"
```

### Run specific test category:
```bash
dotnet test Kinetica.AsyncTests --filter "Category=Integration"
dotnet test Kinetica.AsyncTests --filter "Category=Async"
```

### Run specific test:
```bash
dotnet test Kinetica.AsyncTests --filter "FullyQualifiedName~TestConcurrentSqlQueriesAsync"
```

## Configuration

Tests connect to Kinetica using environment variables (with defaults):

- `KINETICA_URL` - Server URL (default: `http://localhost:9191`)
- `KINETICA_USER` - Username (default: `admin`)
- `KINETICA_PASSWORD` - Password (default: `secret`)

Example with custom configuration:
```bash
KINETICA_URL="http://kinetica-server:9191" \
KINETICA_USER="testuser" \
KINETICA_PASSWORD="testpass" \
dotnet test Kinetica.AsyncTests
```

## Test Isolation

All tests use `TestContext` which:
- Creates unique schema per test (`async_<test_name>`)
- Automatically cleans up schema on disposal
- Prevents test interference
- Mirrors the Rust test implementation pattern

## Future Expansion

This project will grow to include async versions of:
- Type management tests
- Table management tests
- Get records tests
- Aggregate tests
- Record retriever tests
- BulkInserter async tests
- KiFS async file operations
- And more...

Target: ~146 total async tests covering all convertible integration scenarios.
