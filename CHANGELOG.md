# Kinetica C# API Changelog


## Version 7.2

### Version 7.2.3.3 - 2026-06-17

#### Changed

- Aligned ADO.NET connection string options with existing option format
- Cleaned up documentation of public members

#### Fixed

- Sync EOF check in async CSV read method


### Version 7.2.3.2 - 2026-06-15

#### Added

- GitHub Issue #12 - Multi-targeting of .NET 8 & .NET 10 frameworks
- GitHub Issue #13
  - Added logging via `Microsoft.Extensions.Logging`
    - New optional `Kinetica.Options.LoggerFactory` (`ILoggerFactory`); defaults
      to a no-op factory so behavior is unchanged when none is supplied
    - Per-component log categories: `Kinetica`, `Kinetica.HAFailover`,
      `Kinetica.BulkInserter`, & `Kinetica.RecordRetriever`
    - `Warning` logged when client can't connect to server-known URL(s) and is
      put into degraded mode, where multi-head operations are disabled

#### Changed

- GitHub Issue #13
  - Better aligned initial connection negotiation with the Java API
  - Improved documentation surrounding `DisableFailover` &
    `DisableAutodiscovery` options

#### Fixed

- GitHub Issue #10 - Connection option usage example in README
- GitHub Issue #11 - Geometry type mapping in `KineticaType.fromDynamicSchema()`
- `Options.ServerConnectionTimeout` is now wired in (previously declared but
  unused): it maps to `SocketsHttpHandler.ConnectTimeout`, bounding TCP
  connection establishment (e.g. for unreachable hosts during initial
  discovery) independently of the overall request `Timeout`
- `Options.Timeout` of `0` now correctly means infinite (no overall request
  timeout)
- GitHub Issue #13 - Degraded mode handling

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.3.1 - 2026-05-12

#### Added

##### New BulkInserter System

A completely redesigned high-performance bulk insertion system:

- `BulkInserter<T>`
  - Generic bulk inserter supporting both `KineticaRecord` and `IList<object>` record types
  - Configurable batch size, flush intervals, and retry policies
  - Thread-safe concurrent insertion with automatic batching
  - Async/await support with `InsertAsync()` and `FlushAsync()` methods
  - Event-driven architecture with `IBatchInsertionListener` callbacks
  - Automatic worker distribution for multi-head ingest
  - Metrics tracking: `CountInserted`, `CountUpdated`, `CountFailed`

- `GenericRecord` Support for `BulkInserter`
  - Dynamic schema records similar to the Java API's `GenericRecord`
  - Supports all Kinetica data types
  - Fluent GenericRecord API: `record.Put("name", value)` and `record.PutNull("name")`
  - Rust-style fluent type builder: `Type.Builder("name").AddIntColumn("id").Build()`
  - Factory method: `Type.FromTable(kinetica, tableName)` to fetch schema from database
  - High-performance Avro encoder for `GenericRecord` instances

##### High Availability (HA) Failover Support

- Automatic cluster failover on connection failures
- Support for multiple cluster URLs with random or sequential failover order
- Automatic worker rank discovery and URL filtering

- New `Kinetica.Options` properties:
  - `DisableFailover` - Disable automatic failover
  - `DisableAutoDiscovery` - Disable automatic cluster/worker discovery and
    distributed I/O operations
  - `HAFailoverOrder` - Random or Sequential failover
  - `HostManagerPort` - Custom host manager port
  - `HostnameRegex` - Filter URLs by hostname pattern
  - `PrimaryUrl` - Primary cluster URL specification

##### ADO.NET Driver

- ADO.NET compatible database driver for Kinetica
- Implements `IDbConnection`, `IDbCommand`, `IDataReader` interfaces
- JDBC.NET.Data bridge for Java JDBC driver compatibility
- Connection string support for Kinetica URLs

##### Snappy Compression Support

- **Snappier Integration** - High-performance Snappy compression using Snappier library
- **Transparent Compression** - Automatic compression/decompression in HTTP requests

##### Decimal Shard Key Support

- `RecordKey.addDecimal(string value, int precision, int scale)`
  - 8-byte encoding for precision ≤ 18 (scaled Int64)
  - 12-byte encoding for precision > 18 (scaled BigInteger)
  - Proper sign extension for negative values

##### Example Applications

- `AllTypesExample.cs` - Demonstrates all Kinetica data types
- `ShardKeyExample.cs` - Shard key configuration examples

##### Comprehensive Test & Benchmarking Suite

- **Kinetica.Tests** - New xUnit-based test project with 88 tests:
- **Kinetica.Benchmarks** - BenchmarkDotNet-based performance benchmarks
- **Kinetica.LoadGenerator** - High-performance load generator for benchmarking

#### Changed

##### Naming Consistency

- **Renamed for API consistency**:
  - `DynamicRecordKeyBuilder` → `GenericRecordKeyBuilder`
  - `DynamicRecordRetriever` → `GenericRecordRetriever`
  - Aligns with GenericRecord naming used across all Kinetica APIs (Java, Python)

##### BulkInserter Enhancements

- **Backpressure Control** - Implemented Rust-style backpressure with semaphore-based flow control:
  - Async methods (`InsertAsync`, `InsertRangeAsync`, `FlushAsync`) use semaphore for backpressure
  - Sync methods (`Insert`, `InsertRange`) are unbounded for backwards compatibility
  - `FlushJob` struct tracks which code path was used for proper semaphore release
  - `MaxInFlightBatches` option controls maximum concurrent batches (default: 100)

- **CloseAsync Reliability** - Redesigned close sequence to prevent data loss:
  - Waits indefinitely for all pending batches to complete
  - Only respects cancellation token for abort
  - Timeout only applies to worker shutdown phase after batches complete

- **Shard Key Routing** - Verified correct routing using MurmurHash128 and routing table from `adminShowShards`

##### HttpClient Migration

- Replaced obsolete **WebRequest** with **HttpClient**

##### Project Configuration

- **Target Framework**: .NET 8.0 for all projects
- **Central Package Management (CPM)**:
  - Adopted MSBuild Central Package Management for all 9 projects
  - All NuGet package versions centralized in `Directory.Packages.props`
  - Common project properties in `Directory.Build.props`
  - Simplified dependency management and version consistency
- **InternalsVisibleTo**: Kinetica.Tests can access internal members

##### Avro Serialization

- Migrated to Apache.Avro 1.12.1 NuGet package

##### NuGet Package Publishing

- **NuGet Package Metadata**
  - Added comprehensive package metadata for NuGet.org publishing
- **Symbol packages**: SourceLink integration for source-level debugging

#### Removed

- Old `addDecimal(string value)` method - Replaced with precision/scale aware version

#### Fixed

- **CloseAsync data loss prevention** - `CloseAsync()` now waits indefinitely
  for pending batches to complete, preventing data loss from timeouts. Only the
  cancellation token can abort the wait.
- **StringEncoder buffer overflow** - Fixed Avro string encoding to properly
  handle multi-byte UTF-8 characters by pre-calculating encoded byte length
- **GenericRecordEncoder Avro union index** - Fixed Avro union encoding to
  handle Kinetica's `["type", "null"]` ordering (non-null type first, null
  second). Previously assumed null was always index 0, causing
  "invalid nullable value" errors.

#### Dependencies

- **Apache.Avro** 1.12.1 - Official Apache Avro serialization library (replaces vendored 0.9.0)
- **NetCoreServer** 8.0.7 - High-performance socket server
- **JDBC.NET.Data** 3.5.10 - JDBC bridge for ADO.NET
- **Newtonsoft.Json** 13.0.3 - JSON serialization
- **Snappier** 1.3.0 - High-performance Snappy compression (port of Google's Snappy)
- **System.CodeDom** 8.0.0 - Code generation support (updated from 7.0.0 for Apache.Avro compatibility)
- **BenchmarkDotNet** (benchmarks) - Performance measurement
- **xUnit** 2.4.2 (tests) - Unit testing framework

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.3.0 - 2025-09-03

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.2.3 - 2025-04-14

#### Fixed
-   Issue with URL handling in bulk ingestor

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.2.2 - 2025-04-08

#### Fixed
-   Avro encoding issue with `updateRecords()`

#### Changed
-   Code styling update

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.2.1 - 2025-03-28

#### Added
-   Example using a JDBC bridge driver with Kinetica JDBC

#### Changed
-   Code styling update

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.2.0 - 2024-10-15

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.2.1.0 - 2024-09-07

#### Added
-   OAuth2 authentication support


### Version 7.2.0.0 - 2024-02-11

#### Added
-   Support for Array & Vector types

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



## Version 7.1

### Version 7.1.10.0 - 2024-05-16

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.9.1 - 2023-09-17

#### Changed
-   Converted to .NET Core project


### Version 7.1.9.0 - 2023-03-19

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.8.0 - 2022-10-22

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.7.0 - 2022-07-18

#### Changed
-   Removed client-side primary key check, to improve performance and make
    returned errors more consistently delivered

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.6.0 - 2022-01-27

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.5.0 - 2021-10-13

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.4.0 - 2021-07-29

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.1.3.0 - 2021-03-05

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.1.2.0 - 2021-01-25

#### Notes
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.1.1.0 - 2020-10-28

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.1.0.0 - 2020-08-18

#### Note
-   Version release
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



## Version 7.0

### Version 7.0.20.0 - 2020-11-25

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.19.0 - 2020-08-24

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.18.0 - 2020-07-30

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.17.0 - 2020-07-06

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.16.0 - 2020-05-28

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.15.0 - 2020-04-27

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.0.14.0 - 2020-03-25

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.0.13.0 - 2020-03-10

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.



### Version 7.0.12.0 - 2020-01-17

#### Note
-   Check CHANGELOG-FUNCTIONS.md for endpoint-related changes.


### Version 7.0.5.0 - 2019-06-26

#### Added
-   Minor documentation and some options for some endpoints

#### Changed
-   Parameters for /visualize/isoschrone


### Version 7.0.4.0 -- 2019-06-20

#### Fixed
-   Protocol classes' constructor parameters with default values
    are now correctly defined and assigned.

### Version 7.0.0.0 - 2019-01-31

-   Version release



## Version 6.2

## Version 6.2.0.1 - 2019-06-20

#### Changed
-   Protocol classes' constructor parameters with default values
    are defined and assigned differently (using nullable types).


## Version 6.2.0.0 - 2018-03-25

-   Added new RecordRetriever class to support multi-head record lookup by
    shard key.
-   Refactored the following classes from KineticaIngestor.cs to
    the kinetica.Utils namespace:
    -   WorkerList
    -   RecordKey
    -   RecordKeyBuilder
    -   WorkerQueue



## Version 6.1

## Version 6.1.0 - 2017-10-05

-   Added support for datetime



## Version 6.0

## Version 6.0.1 - 2017-06-19

-   Added multi-head ingestion support


## Version 6.0.0 - 2017-05-03

-   Version release

---

## License

Copyright (c) 2016-2026 Kinetica DB Inc.
