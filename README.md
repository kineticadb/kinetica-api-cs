<h3 align="center" style="margin:0px">
	<img width="200" src="https://kinetica-web-assets.s3.us-east-1.amazonaws.com/assets/kinetica_logo_gray.svg" alt="Kinetica Logo"/>
</h3>
<h5 align="center" style="margin:0px">
	<a href="https://www.kinetica.com/">Website</a>
	|
	<a href="https://docs.kinetica.com/7.2/">Docs</a>
	|
	<a href="https://docs.kinetica.com/7.2/api/csharp/">API Docs</a>
	|
	<a href="https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg">Community Slack</a>
</h5>

# Kinetica C# API

Official C# API for Kinetica, the real-time analytics database. Provides
high-performance data ingestion with `BulkInserter`, comprehensive database
operations, and support for both strongly-typed POCOs and dynamic
`GenericRecord` types.

-  [Overview](#overview)
-  [Key Features](#key-features)
-  [Installation](#installation)
-  [Quick Start](#quick-start)
-  [Core Components](#core-components)
-  [Building the API](#building-the-api)
-  [Project Structure](#project-structure)
-  [Examples](#examples)
-  [Testing](#testing)
-  [Documentation](#documentation)
-  [Support](#support)
-  [License](#license)


## Overview

The Kinetica C# API provides a comprehensive interface to interact with
Kinetica, a distributed, in-memory database designed for real-time analytics on
massive datasets. This API supports high-performance bulk data insertion, schema
management, SQL execution, and a full range of database operations.


## Key Features

### High-Performance Data Ingestion
- **BulkInserter<T>**: Thread-safe, high-performance bulk data insertion
  - Configurable batch sizes and automatic flushing
  - Async/await support with `InsertAsync()` and `FlushAsync()`
  - Automatic distributed ingest large workloads
  - Event-driven architecture with insertion listeners
  - Retry policies with exponential backoff
  - Real-time metrics: records inserted, updated, and failed

### Flexible Record Types
- **GenericRecord**: Dynamic schema support similar to Apache Avro
  - Fluent API: `record.Put("column", value)`
  - Type-safe value storage with `RecordValue` discriminated union
  - No compile-time schema requirements
  - Automatic shard key extraction for routing

- **KineticaRecord**: Strongly-typed POCO support
  - Compile-time type safety
  - Attribute-based schema definition
  - Full IntelliSense support

### Type System
- **Type.Builder**: Fluent type builder for creating schemas programmatically
  ```csharp
  var type = Type.Builder("my_table")
      .AddIntColumn("id")
      .AddStringColumn("name")
      .AddTimestampColumn("created_at")
      .Build();
  ```
- **Type.FromTable()**: Fetch schema directly from existing tables
- Support for all Kinetica data types

### ADO.NET Driver
- Full ADO.NET provider implementation (`AdoDotnetDriver`)
- Compatible with Entity Framework and other ORMs
- Standard database operations using familiar ADO.NET interfaces
- Connection pooling and transaction support

### KiFS (Kinetica File System) API
- Complete file system operations for managing data files in Kinetica
- **Upload & Download**: Transfer files between local and KiFS with progress callbacks
  - Single and batch file operations
  - Multi-part uploads for large files (>60MB)
  - Glob pattern support (`*.csv`, `data/**`, `file_?.txt`)
  - Async/await support
- **Directory Management**: Create, delete, and list KiFS directories
- **File Operations**: Delete files, check existence, get file metadata
- **Integration**: Seamless integration with data ingestion workflows
- **Performance**: Automatic batching and multi-part uploads for large files

### Comprehensive API Coverage
- Complete protocol support for all Kinetica endpoints
- SQL execution with parameterized queries
- Table and schema management
- Record retrieval with filtering and pagination
- Aggregations, joins, and analytics operations
- Graph analytics support


## Installation

### NuGet Package (Recommended)

```bash
dotnet add package Kinetica
```

Or via the NuGet Package Manager:

```powershell
Install-Package Kinetica
```

### From Source

```bash
git clone git@github.com:kineticadb/kinetica-api-cs.git
cd kinetica-api-cs
dotnet build
```


## Quick Start

### Basic Connection and Table Operations

```csharp
using kinetica;

// Create a connection to Kinetica
var kinetica = new Kinetica("http://localhost:9191",
    new KineticaOptions
    {
        Username = "admin",
        Password = "secret"
    });

// Create a table using Type Builder
var type = Type.Builder("example_table")
    .AddIntColumn("id", isPrimaryKey: true)
    .AddStringColumn("name")
    .AddDoubleColumn("value")
    .Build();

string tableName = "my_schema.my_table";
kinetica.createTable(tableName, type);
```

### High-Performance Bulk Insert with GenericRecord

```csharp
using kinetica;
using kinetica.Records;
using kinetica.BulkInserter;

// Fetch schema from existing table
var type = Type.FromTable(kinetica, "my_schema.my_table");

// Create BulkInserter with custom options
var options = new BulkInserterOptions
{
    BatchSize = 10000,
    FlushIntervalSeconds = 5.0,
    UpdateOnExistingPk = true
};

using var inserter = new BulkInserter<GenericRecord>(
    kinetica,
    "my_schema.my_table",
    type,
    options);

// Insert records
for (int i = 0; i < 1_000_000; i++)
{
    var record = new GenericRecord(type)
        .Put("id", i)
        .Put("name", $"Record_{i}")
        .Put("value", i * 1.5);

    await inserter.InsertAsync(record);
}

// Flush remaining records
await inserter.FlushAsync();

Console.WriteLine($"Inserted: {inserter.CountInserted}");
Console.WriteLine($"Updated: {inserter.CountUpdated}");
```

### Using Strongly-Typed Records (POCOs)

```csharp
using kinetica;
using kinetica.BulkInserter;

// Define a POCO class
public class MyRecord : KineticaRecord
{
    [KineticaColumn(IsPrimaryKey = true)]
    public int Id { get; set; }

    [KineticaColumn]
    public string Name { get; set; }

    [KineticaColumn]
    public double Value { get; set; }
}

// Create type from POCO
var type = KineticaType.FromClass<MyRecord>();
kinetica.createTable("my_schema.typed_table", type);

// Bulk insert with POCOs
using var inserter = new BulkInserter<MyRecord>(
    kinetica,
    "my_schema.typed_table");

var record = new MyRecord
{
    Id = 1,
    Name = "Test",
    Value = 42.0
};

await inserter.InsertAsync(record);
await inserter.FlushAsync();
```

### SQL Execution

```csharp
using kinetica;

var response = kinetica.executeSql(
    "SELECT * FROM my_schema.my_table WHERE value > 100 LIMIT 10");

// Process results
foreach (var record in response.records)
{
    Console.WriteLine($"Record: {record}");
}
```

### ADO.NET Usage

```csharp
using KineticaAdo;

var connectionString = "Server=http://localhost:9191;UID=admin;PWD=secret";

using var connection = new KineticaConnection(connectionString);
await connection.OpenAsync();

using var command = connection.CreateCommand();
command.CommandText = "SELECT COUNT(*) FROM my_schema.my_table";

var count = (long)await command.ExecuteScalarAsync();
Console.WriteLine($"Total records: {count}");
```

### KiFS File System Operations

```csharp
using kinetica;
using kinetica.FileSystem;

// Create a file handler
var fileHandler = new KineticaFileHandler(kinetica);

// Create a directory in KiFS
fileHandler.CreateDirectory("data");

// Upload files to KiFS (supports glob patterns)
fileHandler.Upload("/local/path/data.csv", "data");
fileHandler.Upload("/local/path/*.parquet", "data");

// Upload with progress callback
fileHandler.Upload(
    new List<string> { "/local/large_file.csv" },
    "data",
    new UploadOptions { DeleteIfExists = true },
    new FileUploadListener());

// Download files from KiFS
fileHandler.Download("data/data.csv", "/local/download/");

// List files in a directory
var files = fileHandler.ShowFiles(new List<string> { "data" });
foreach (var file in files)
{
    Console.WriteLine($"{file.FileName}: {file.FileSize} bytes");
}

// Check if file exists
if (fileHandler.KifsFileExists("data/data.csv"))
{
    Console.WriteLine("File exists in KiFS");
}

// Delete files
fileHandler.DeleteFiles(new List<string> { "data/old_data.csv" });

// Async operations
await fileHandler.UploadAsync(
    new List<string> { "/local/path/*.csv" },
    "data");
```


## Core Components

### Kinetica Client (`Kinetica/Kinetica.cs`)
The main client class for interacting with Kinetica. Provides methods for:
- Table creation, modification, and deletion
- Record insertion, retrieval, and updates
- SQL execution
- Schema management
- System administration

### BulkInserter (`Kinetica/BulkInserter/`)
High-performance bulk data insertion system:
- **BulkInserter<T>**: Main bulk inserter class
- **BulkInserterOptions**: Configuration options
- **IBatchInsertionListener**: Event callbacks for monitoring
- **WorkerList**: Distributed ingest worker management

### Record Types (`Kinetica/Records/`)
- **GenericRecord**: Dynamic schema records
- **Type**: Schema definition and type builder
- **RecordValue**: Type-safe value storage
- **KineticaRecord**: Base class for POCOs

### Protocol (`Kinetica/Protocol/`)
Auto-generated protocol classes for all Kinetica API endpoints:
- Request/Response classes for every operation
- Strongly-typed options and parameters
- Comprehensive API coverage

### ADO.NET Driver (`AdoDotnetDriver/`)
Full ADO.NET 2.0+ provider implementation:
- `KineticaConnection`
- `KineticaCommand`
- `KineticaDataReader`
- `KineticaDataAdapter`
- Connection pooling

### File System (`Kinetica/FileSystem/`)
KiFS (Kinetica File System) integration:
- **KineticaFileHandler**: Main file operations handler
- **UploadOptions/DownloadOptions**: File transfer configuration
- **KifsFileInfo/KifsDirectoryInfo**: File and directory metadata
- **IFileUploadListener/IFileDownloadListener**: Progress callback interfaces
- Glob pattern matching for batch operations
- Multi-part upload support for large files


## Building the API

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (optional, for Windows users)
- **Visual Studio Code** (optional, cross-platform)

### Build from Command Line

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Create NuGet package
dotnet pack -c Release
```

### Build in Visual Studio (Windows)

1. Open `gpudb-api-cs.sln` in Visual Studio 2022
2. Restore NuGet packages: **Tools > NuGet Package Manager > Restore NuGet Packages**
3. Build the solution: **Build > Build Solution** (Ctrl+Shift+B)

### Build on Linux/macOS

```bash
# Install .NET 8.0 SDK
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Build
dotnet build
```


## Project Structure

```
gpudb-api-cs/
├── Kinetica/                    # Main API library
│   ├── BulkInserter/           # High-performance bulk insertion
│   ├── Protocol/               # Auto-generated API protocol classes
│   ├── Records/                # GenericRecord and Type system
│   ├── Schema/                 # Schema management
│   ├── FileSystem/             # KiFS file system operations
│   ├── Utils/                  # Utility classes
│   └── Kinetica.cs             # Main client class
├── AdoDotnetDriver/            # ADO.NET provider
├── Kinetica.Tests/             # Unit and integration tests
├── AdoDotnetDriver.Tests/      # ADO.NET driver tests
├── Example/                    # Example applications
│   ├── Example.cs              # Basic usage examples
│   ├── BulkInserterFullIntegration.cs
│   ├── AllTypesExample.cs      # All data types demonstration
│   ├── ShardKeyExample.cs      # Shard key configuration
│   └── SchemaBuilderDemo.cs    # Dynamic schema building
├── Kinetica.Benchmarks/        # Performance benchmarks
├── Kinetica.LoadGenerator/     # Load testing tools
└── README.md                   # This file
```


## Examples

The `Example/` directory contains comprehensive examples:

### Basic Usage
- **Example.cs**: Basic CRUD operations, table management, SQL execution
- **AllTypesExample.cs**: Working with all Kinetica data types

### Advanced Features
- **BulkInserterFullIntegration.cs**: Complete BulkInserter workflow
- **ShardKeyExample.cs**: Distributed ingest with shard keys
- **SchemaBuilderDemo.cs**: Dynamic schema creation
- **AdoBatchInsertExample.cs**: ADO.NET batch operations
- **BulkInserterDashboard.cs**: Real-time metrics and monitoring

### Running Examples

```bash
cd Example
dotnet run
```


## Testing

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Project

```bash
# Kinetica API tests
dotnet test Kinetica.Tests/Kinetica.Tests.csproj

# ADO.NET driver tests
dotnet test AdoDotnetDriver.Tests/AdoDotnetDriver.Tests.csproj
```

### Run Tests with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Prerequisites for Integration Tests

Integration tests require a running Kinetica instance:

```bash
# Default connection: http://localhost:9191
# Username: admin
# Password: secret

# Set custom connection via environment variables
export KINETICA_URL="http://your-server:9191"
export KINETICA_USERNAME="your-username"
export KINETICA_PASSWORD="your-password"

dotnet test
```


## Support

For bugs, please submit an
[issue on Github](https://github.com/kineticadb/kinetica-api-cs/issues).

For support, you can post on
[stackoverflow](https://stackoverflow.com/questions/tagged/kinetica) under the ``kinetica`` tag or
[Slack](https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg).


## Contact Us

* Ask a question on Slack:
  [Slack](https://join.slack.com/t/kinetica-community/shared_invite/zt-1bt9x3mvr-uMKrXlSDXfy3oU~sKi84qg)
* Follow on GitHub:
  [Follow @kineticadb](https://github.com/kineticadb) 
* Email us:  <support@kinetica.com>
* Visit:  <https://www.kinetica.com/contact/>


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Copyright © 2026 Kinetica. All rights reserved.
