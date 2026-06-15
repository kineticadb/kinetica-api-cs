using Xunit;
using Xunit.Abstractions;
using Kinetica.Tests.Common;
using kinetica;
using kinetica.Records;

namespace Kinetica.Tests.IntegrationTests;

/// <summary>
/// Integration tests for decimal12 (12-byte decimal, precision > 18) shard key functionality.
///
/// These tests verify that:
/// 1. BulkInserter correctly routes records with decimal12 shard keys
/// 2. RecordRetriever correctly retrieves records by decimal12 shard key lookups
/// 3. GenericRecord works correctly with decimal12 shard keys
///
/// Decimal12 is used when precision > 18 (e.g., DECIMAL(26,13)).
/// The wire format uses 96-bit (12-byte) little-endian integer encoding.
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Decimal12")]
public class Decimal12ShardKeyTests
{
    private readonly ITestOutputHelper _output;

    public Decimal12ShardKeyTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Test Record Types

    /// <summary>
    /// Record with a decimal12 shard key column.
    /// DECIMAL(26,13) has precision > 18, so it uses 12-byte encoding.
    /// </summary>
    public class Decimal12ShardedRecord : IShardKeyExtractor
    {
        public int id { get; set; }
        public string big_decimal_sk { get; set; } = "0";
        public string name { get; set; } = string.Empty;
        public double value { get; set; }

        public ShardKeyValues GetShardKeyValues()
        {
            return new ShardKeyValues(("big_decimal_sk", ShardKeyValue.Decimal(big_decimal_sk)));
        }
    }

    /// <summary>
    /// Record with composite shard key including decimal12.
    /// Tests that decimal12 works correctly alongside other shard key types.
    /// </summary>
    public class CompositeDecimal12Record : IShardKeyExtractor
    {
        public int id { get; set; }
        public string big_decimal_sk { get; set; } = "0";
        public int int_sk { get; set; }
        public string name { get; set; } = string.Empty;

        public ShardKeyValues GetShardKeyValues()
        {
            return new ShardKeyValues(
                ("big_decimal_sk", ShardKeyValue.Decimal(big_decimal_sk)),
                ("int_sk", ShardKeyValue.Int(int_sk))
            );
        }
    }

    #endregion

    #region Helper Methods

    private (string tableName, KineticaType ktype) SetupDecimal12ShardTable(TestContext ctx, string suffix = "dec12_shard")
    {
        var tableName = ctx.QualifiedTable(suffix);

        // Create table with DECIMAL(26,13) as shard key
        // Precision 26 > 18, so this uses 12-byte decimal encoding
        ctx.Kinetica.executeSql($@"CREATE TABLE {tableName} (
            id INT NOT NULL,
            big_decimal_sk DECIMAL(26,13) NOT NULL,
            name VARCHAR(128),
            value DOUBLE,
            PRIMARY KEY (id, big_decimal_sk),
            SHARD KEY (big_decimal_sk)
        )");

        var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);

        // Verify the decimal column has correct precision
        var columns = ktype.getColumns();
        var decCol = columns[1]; // big_decimal_sk
        _output.WriteLine($"Decimal column: precision={decCol.getDecimalPrecision()}, scale={decCol.getDecimalScale()}, byteSize={decCol.getDecimalByteSize()}");

        Assert.Equal(26, decCol.getDecimalPrecision());
        Assert.Equal(13, decCol.getDecimalScale());
        Assert.Equal(12, decCol.getDecimalByteSize());

        return (tableName, ktype);
    }

    private (string tableName, KineticaType ktype) SetupCompositeDecimal12Table(TestContext ctx, string suffix = "comp_dec12")
    {
        var tableName = ctx.QualifiedTable(suffix);

        // Create table with composite shard key including decimal12
        ctx.Kinetica.executeSql($@"CREATE TABLE {tableName} (
            id INT NOT NULL,
            big_decimal_sk DECIMAL(26,13) NOT NULL,
            int_sk INT NOT NULL,
            name VARCHAR(128),
            PRIMARY KEY (id, big_decimal_sk, int_sk),
            SHARD KEY (big_decimal_sk, int_sk)
        )");

        var ktype = KineticaType.fromTable(ctx.Kinetica, tableName);
        return (tableName, ktype);
    }

    private List<Decimal12ShardedRecord> GenerateDecimal12Records(int count, int startId = 0)
    {
        var records = new List<Decimal12ShardedRecord>(count);

        for (int i = 0; i < count; i++)
        {
            // Generate high-precision decimal values that require 12 bytes
            var decimalValue = $"{1000000000000 + startId + i}.{(i % 10000000000000):D13}";

            records.Add(new Decimal12ShardedRecord
            {
                id = startId + i,
                big_decimal_sk = decimalValue,
                name = $"record_{startId + i:D8}",
                value = (startId + i) * 1.5
            });
        }

        return records;
    }

    private List<CompositeDecimal12Record> GenerateCompositeRecords(int count, int startId = 0)
    {
        var records = new List<CompositeDecimal12Record>(count);

        for (int i = 0; i < count; i++)
        {
            var decimalValue = $"{1000000000000 + startId + i}.{(i % 10000000000000):D13}";

            records.Add(new CompositeDecimal12Record
            {
                id = startId + i,
                big_decimal_sk = decimalValue,
                int_sk = (startId + i) % 100,
                name = $"composite_{startId + i:D8}"
            });
        }

        return records;
    }

    #endregion

    #region BulkInserter Tests

    [Fact]
    public async Task BulkInserter_Decimal12ShardKey_InsertsCorrectly()
    {
        using var ctx = new TestContext("dec12_bulk_basic");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx);

        var options = new BulkInserterOptions
        {
            BatchSize = 100
        };

        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        var records = GenerateDecimal12Records(500);

        foreach (var record in records)
        {
            inserter.Insert(record);
        }

        await inserter.CloseAsync();

        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        _output.WriteLine($"Batches: {inserter.TotalBatchesSent}");
        _output.WriteLine($"Workers: {inserter.NumWorkers}");

        Assert.Equal(500, inserter.CountInserted);

        // Verify in database
        var response = ctx.Kinetica.executeSql($"SELECT COUNT(*) AS cnt FROM {tableName}", 0, -9999);
        Assert.Equal(1, response.total_number_of_records);
    }

    [Fact]
    public async Task BulkInserter_Decimal12ShardKey_BatchInsert()
    {
        using var ctx = new TestContext("dec12_bulk_batch");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "batch_test");

        var options = new BulkInserterOptions
        {
            BatchSize = 1000,
            MaxInFlightBatches = 5
        };

        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        var records = GenerateDecimal12Records(5000);

        inserter.InsertBatch(records);

        await inserter.CloseAsync();

        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        _output.WriteLine($"Batches sent: {inserter.TotalBatchesSent}");

        Assert.Equal(5000, inserter.CountInserted);
    }

    [Fact]
    public async Task BulkInserter_Decimal12ShardKey_VariousValues()
    {
        using var ctx = new TestContext("dec12_bulk_various");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "various_test");

        var options = new BulkInserterOptions
        {
            BatchSize = 50
        };

        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        // Test various decimal12 edge cases
        var testCases = new[]
        {
            "0.0000000000000",                    // Zero
            "1.0000000000000",                    // One
            "-1.0000000000000",                   // Negative one
            "9999999999999.9999999999999",        // Near max positive
            "-9999999999999.9999999999999",       // Near max negative
            "1234567890123.1234567890123",        // High precision positive
            "-1234567890123.1234567890123",       // High precision negative
            "0.0000000000001",                    // Smallest positive
            "-0.0000000000001",                   // Smallest negative
            "5555555555555.5555555555555",        // Repeating digits
        };

        for (int i = 0; i < testCases.Length; i++)
        {
            inserter.Insert(new Decimal12ShardedRecord
            {
                id = i,
                big_decimal_sk = testCases[i],
                name = $"test_case_{i}",
                value = i * 1.0
            });
        }

        await inserter.CloseAsync();

        Assert.Equal(testCases.Length, inserter.CountInserted);

        // Verify values can be retrieved correctly
        var response = ctx.Kinetica.executeSql($"SELECT id, big_decimal_sk FROM {tableName} ORDER BY id", 0, -9999);
        Assert.Equal(testCases.Length, response.total_number_of_records);

        _output.WriteLine("Verified decimal12 values:");
        for (int i = 0; i < response.data.Count; i++)
        {
            var row = response.data[i];
            _output.WriteLine($"  id={row["id"]}, big_decimal_sk={row["big_decimal_sk"]}");
        }
    }

    [Fact]
    public async Task BulkInserter_CompositeDecimal12ShardKey_InsertsCorrectly()
    {
        using var ctx = new TestContext("dec12_bulk_composite");
        var (tableName, ktype) = SetupCompositeDecimal12Table(ctx);

        var options = new BulkInserterOptions
        {
            BatchSize = 100
        };

        await using var inserter = new BulkInserter<CompositeDecimal12Record>(ctx.Kinetica, tableName, ktype, options);

        var records = GenerateCompositeRecords(500);

        inserter.InsertBatch(records);

        await inserter.CloseAsync();

        _output.WriteLine($"RecordsQueued: {inserter.TotalRecordsQueued}");
        _output.WriteLine($"RecordsSubmitted: {inserter.TotalRecordsSubmitted}");
        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        _output.WriteLine($"Batches: {inserter.TotalBatchesSent}");
        _output.WriteLine($"Failed batches: {inserter.TotalBatchesFailed}");
        _output.WriteLine($"Errors: {inserter.ErrorCount}");
        if (inserter.ErrorCount > 0)
        {
            foreach (var error in inserter.PeekErrors())
            {
                _output.WriteLine($"  Error: {error.Message}");
            }
        }

        Assert.Equal(500, inserter.TotalRecordsQueued);
        Assert.Equal(500, inserter.TotalRecordsSubmitted);
        Assert.Equal(500, inserter.CountInserted);
    }

    #endregion

    #region RecordRetriever Tests (SQL-based)

    [Fact]
    public async Task RecordRetriever_Decimal12ShardKey_RetrievesCorrectly()
    {
        using var ctx = new TestContext("dec12_retriever");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "retriever_test");

        // Insert test data
        var options = new BulkInserterOptions { BatchSize = 100 };
        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        var records = GenerateDecimal12Records(100);
        inserter.InsertBatch(records);
        await inserter.CloseAsync();

        _output.WriteLine($"Inserted {inserter.CountInserted} records for retrieval test");

        // Test retrieval by shard key
        var targetDecimal = records[50].big_decimal_sk;
        var response = ctx.Kinetica.executeSql(
            $"SELECT * FROM {tableName} WHERE big_decimal_sk = {targetDecimal}",
            0, -9999);

        Assert.Equal(1, response.total_number_of_records);

        var retrieved = response.data[0];
        Assert.Equal(50, Convert.ToInt32(retrieved["id"]));

        _output.WriteLine($"Retrieved record: id={retrieved["id"]}, big_decimal_sk={retrieved["big_decimal_sk"]}");
    }

    [Fact]
    public async Task RecordRetriever_Decimal12ShardKey_MultipleRecords()
    {
        using var ctx = new TestContext("dec12_retriever_multi");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "multi_retriever");

        // Insert test data with some duplicate shard key values
        var options = new BulkInserterOptions { BatchSize = 50 };
        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        // Insert records with controlled decimal values
        for (int i = 0; i < 30; i++)
        {
            // Use 3 distinct shard key values, 10 records each
            var shardGroup = i / 10;
            var decimalValue = $"{1000000000000 + shardGroup}.{shardGroup:D13}";

            inserter.Insert(new Decimal12ShardedRecord
            {
                id = i,
                big_decimal_sk = decimalValue,
                name = $"record_{i}",
                value = i * 1.5
            });
        }

        await inserter.CloseAsync();

        // Query for records in one shard group
        var targetDecimal = "1000000000001.0000000000001";
        var response = ctx.Kinetica.executeSql(
            $"SELECT COUNT(*) as cnt FROM {tableName} WHERE big_decimal_sk = {targetDecimal}",
            0, -9999);

        // Should find 10 records with this shard key value
        var countRow = response.data[0];
        var count = Convert.ToInt64(countRow["cnt"]);

        _output.WriteLine($"Found {count} records with shard key {targetDecimal}");
        Assert.Equal(10, count);
    }

    [Fact]
    public async Task RecordRetriever_Decimal12ShardKey_NegativeValues()
    {
        using var ctx = new TestContext("dec12_retriever_neg");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "neg_retriever");

        var options = new BulkInserterOptions { BatchSize = 50 };
        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        // Insert records with negative decimal shard keys
        var negativeValues = new[]
        {
            "-1234567890123.1234567890123",
            "-9999999999999.9999999999999",
            "-0.0000000000001",
        };

        for (int i = 0; i < negativeValues.Length; i++)
        {
            inserter.Insert(new Decimal12ShardedRecord
            {
                id = i,
                big_decimal_sk = negativeValues[i],
                name = $"negative_{i}",
                value = i * -1.0
            });
        }

        await inserter.CloseAsync();

        // Verify retrieval of negative values
        var response = ctx.Kinetica.executeSql(
            $"SELECT * FROM {tableName} WHERE big_decimal_sk = -1234567890123.1234567890123",
            0, -9999);

        Assert.Equal(1, response.total_number_of_records);

        var retrieved = response.data[0];
        _output.WriteLine($"Retrieved negative: id={retrieved["id"]}, big_decimal_sk={retrieved["big_decimal_sk"]}");
    }

    [Fact]
    public async Task RecordRetriever_CompositeDecimal12ShardKey_RetrievesCorrectly()
    {
        using var ctx = new TestContext("dec12_retriever_comp");
        var (tableName, ktype) = SetupCompositeDecimal12Table(ctx, "comp_retriever");

        var options = new BulkInserterOptions { BatchSize = 50 };
        await using var inserter = new BulkInserter<CompositeDecimal12Record>(ctx.Kinetica, tableName, ktype, options);

        var records = GenerateCompositeRecords(100);
        inserter.InsertBatch(records);
        await inserter.CloseAsync();

        // Query by composite shard key
        var targetRecord = records[25];
        var response = ctx.Kinetica.executeSql(
            $"SELECT * FROM {tableName} WHERE big_decimal_sk = {targetRecord.big_decimal_sk} AND int_sk = {targetRecord.int_sk}",
            0, -9999);

        Assert.Equal(1, response.total_number_of_records);

        var retrieved = response.data[0];
        Assert.Equal(25, Convert.ToInt32(retrieved["id"]));

        _output.WriteLine($"Retrieved composite: id={retrieved["id"]}, big_decimal_sk={retrieved["big_decimal_sk"]}, int_sk={retrieved["int_sk"]}");
    }

    #endregion

    #region GenericRecord Tests

    [Fact]
    public async Task GenericRecord_Decimal12ShardKey_InsertsCorrectly()
    {
        using var ctx = new TestContext("dec12_generic");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "generic_test");

        var recordType = kinetica.Records.Type.FromTable(ctx.Kinetica, tableName);

        var options = new BulkInserterOptions
        {
            BatchSize = 100
        };

        await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

        // Insert GenericRecords with decimal12 shard keys
        for (int i = 0; i < 200; i++)
        {
            var decimalValue = $"{1000000000000 + i}.{(i % 10000000000000):D13}";

            var record = recordType.NewInstance();
            record.Put("id", i);
            record.Put("big_decimal_sk", decimalValue);
            record.Put("name", $"generic_{i:D8}");
            record.Put("value", i * 2.5);

            inserter.Insert(record);
        }

        await inserter.CloseAsync();

        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        Assert.Equal(200, inserter.CountInserted);

        // Verify data
        var response = ctx.Kinetica.executeSql($"SELECT COUNT(*) FROM {tableName}", 0, -9999);
        Assert.Equal(1, response.total_number_of_records);
    }

    [Fact]
    public async Task GenericRecord_Decimal12ShardKey_VariousFormats()
    {
        using var ctx = new TestContext("dec12_generic_formats");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "format_test");

        var recordType = kinetica.Records.Type.FromTable(ctx.Kinetica, tableName);

        var options = new BulkInserterOptions { BatchSize = 20 };

        await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

        // Test various input formats for decimal values
        var testValues = new[]
        {
            ("0", 0),
            ("0.0", 1),
            ("123.456", 2),
            ("123.4567890123456", 3),           // More than scale 13
            ("-999.999", 4),
            ("9999999999999.9999999999999", 5), // Max positive
            ("-9999999999999.9999999999999", 6), // Max negative
        };

        foreach (var (decVal, id) in testValues)
        {
            var record = recordType.NewInstance();
            record.Put("id", id);
            record.Put("big_decimal_sk", decVal);
            record.Put("name", $"format_test_{id}");
            record.Put("value", id * 1.0);

            inserter.Insert(record);
        }

        await inserter.CloseAsync();

        Assert.Equal(testValues.Length, inserter.CountInserted);

        // Verify all records inserted correctly
        var response = ctx.Kinetica.executeSql(
            $"SELECT id, big_decimal_sk FROM {tableName} ORDER BY id",
            0, -9999);

        Assert.Equal(testValues.Length, response.total_number_of_records);

        _output.WriteLine("GenericRecord decimal12 format test results:");
        foreach (var row in response.data)
        {
            _output.WriteLine($"  id={row["id"]}, big_decimal_sk={row["big_decimal_sk"]}");
        }
    }

    [Fact]
    public async Task GenericRecord_CompositeDecimal12ShardKey_InsertsCorrectly()
    {
        using var ctx = new TestContext("dec12_generic_comp");
        var (tableName, ktype) = SetupCompositeDecimal12Table(ctx, "generic_comp");

        var recordType = kinetica.Records.Type.FromTable(ctx.Kinetica, tableName);

        var options = new BulkInserterOptions { BatchSize = 50 };

        await using var inserter = new BulkInserter<GenericRecord>(ctx.Kinetica, tableName, ktype, options);

        for (int i = 0; i < 150; i++)
        {
            var decimalValue = $"{1000000000000 + i}.{(i % 10000000000000):D13}";

            var record = recordType.NewInstance();
            record.Put("id", i);
            record.Put("big_decimal_sk", decimalValue);
            record.Put("int_sk", i % 50);
            record.Put("name", $"generic_comp_{i:D8}");

            inserter.Insert(record);
        }

        await inserter.CloseAsync();

        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        Assert.Equal(150, inserter.CountInserted);
    }

    #endregion

    #region Shard Routing Verification Tests

    [Fact]
    public async Task Decimal12ShardKey_ShardRoutingConsistency()
    {
        using var ctx = new TestContext("dec12_routing");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "routing_test");

        var options = new BulkInserterOptions
        {
            BatchSize = 100,
            MaxInFlightBatches = 5
        };

        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        // Insert records with the same shard key value (should all route to same shard)
        var sharedDecimal = "1234567890123.1234567890123";
        for (int i = 0; i < 50; i++)
        {
            inserter.Insert(new Decimal12ShardedRecord
            {
                id = i,
                big_decimal_sk = sharedDecimal,
                name = $"same_shard_{i}",
                value = i * 1.0
            });
        }

        await inserter.CloseAsync();

        _output.WriteLine($"Inserted: {inserter.CountInserted}");
        _output.WriteLine($"Workers: {inserter.NumWorkers}");
        _output.WriteLine($"Batches: {inserter.TotalBatchesSent}");

        Assert.Equal(50, inserter.CountInserted);

        // Verify all records are present and queryable
        var response = ctx.Kinetica.executeSql(
            $"SELECT COUNT(*) as cnt FROM {tableName} WHERE big_decimal_sk = {sharedDecimal}",
            0, -9999);

        var count = Convert.ToInt64(response.data[0]["cnt"]);
        Assert.Equal(50, count);
    }

    [Fact]
    public async Task Decimal12ShardKey_ConcurrentInserts()
    {
        using var ctx = new TestContext("dec12_concurrent");
        var (tableName, ktype) = SetupDecimal12ShardTable(ctx, "concurrent_test");

        var options = new BulkInserterOptions
        {
            BatchSize = 100,
            MaxInFlightBatches = 20,
            NumStripes = 8
        };

        await using var inserter = new BulkInserter<Decimal12ShardedRecord>(ctx.Kinetica, tableName, ktype, options);

        const int numThreads = 4;
        const int recordsPerThread = 250;

        var tasks = new Task[numThreads];

        for (int t = 0; t < numThreads; t++)
        {
            int threadId = t;
            tasks[t] = Task.Run(async () =>
            {
                var threadRecords = GenerateDecimal12Records(recordsPerThread, threadId * recordsPerThread);
                foreach (var record in threadRecords)
                {
                    await inserter.InsertAsync(record);
                }
            });
        }

        await Task.WhenAll(tasks);
        await inserter.CloseAsync();

        _output.WriteLine($"Total inserted: {inserter.CountInserted}");
        _output.WriteLine($"Total batches: {inserter.TotalBatchesSent}");

        Assert.Equal(numThreads * recordsPerThread, inserter.CountInserted);
    }

    #endregion
}
