/*
 * Benchmark: Avro Decoding Performance
 *
 * This benchmark measures Avro decoding performance using different strategies.
 * It tests sequential vs parallel decoding for various record counts and payload sizes.
 *
 * Run with:
 *   dotnet run --project Kinetica.Benchmarks -c Release -- --filter '*Avro*'
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using kinetica;

namespace Kinetica.Benchmarks
{
    /// <summary>
    /// Avro decode benchmark record type.
    /// </summary>
    public class AvroDecodeRecord
    {
        public int id { get; set; }
        public string category { get; set; } = "";
        public double value { get; set; }
        public long timestamp { get; set; }
        public string payload { get; set; } = "";
    }

    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    [RPlotExporter]
    public class AvroDecodeBenchmarks
    {
        private kinetica.Kinetica? _kinetica;
        private string _tableName = "";
        private int _totalRecords;

        [Params(1000, 10000, 50000)]
        public int RecordCount { get; set; }

        [Params(64, 256)]
        public int PayloadSize { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            _kinetica = new kinetica.Kinetica(url, new kinetica.Kinetica.Options
            {
                Username = user,
                Password = password
            });

            // Create test table
            var schemaName = "bench_schema";
            _tableName = $"{schemaName}.avro_decode_test";

            try { _kinetica.executeSql($"CREATE SCHEMA IF NOT EXISTS {schemaName}"); } catch { }
            try { _kinetica.executeSql($"DROP TABLE IF EXISTS {_tableName}"); } catch { }

            _kinetica.executeSql($@"
                CREATE TABLE {_tableName} (
                    id INT NOT NULL,
                    category VARCHAR(64),
                    value DOUBLE,
                    timestamp LONG,
                    payload VARCHAR({PayloadSize}),
                    PRIMARY KEY (id),
                    SHARD KEY (id)
                )
            ");

            // Generate and insert test records using SQL (avoids type registration issues)
            var payloadTemplate = new string('x', PayloadSize);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Insert in batches using SQL
            const int sqlBatchSize = 1000;
            for (int batch = 0; batch < RecordCount; batch += sqlBatchSize)
            {
                var batchEnd = Math.Min(batch + sqlBatchSize, RecordCount);
                var values = new List<string>();
                for (int i = batch; i < batchEnd; i++)
                {
                    values.Add($"({i}, 'cat_{i % 100}', {i * 0.01}, {timestamp}, '{payloadTemplate}')");
                }
                _kinetica.executeSql($"INSERT INTO {_tableName} (id, category, value, timestamp, payload) VALUES {string.Join(",", values)}");
            }

            _totalRecords = RecordCount;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            try
            {
                _kinetica?.executeSql($"DROP TABLE IF EXISTS {_tableName}");
            }
            catch { }
        }

        [Benchmark(Baseline = true)]
        public List<AvroDecodeRecord> SequentialDecode()
        {
            if (_kinetica == null)
                throw new InvalidOperationException("Setup not complete");

            // Fetch records using getRecords which includes decoding
            var response = _kinetica.getRecords<AvroDecodeRecord>(
                new GetRecordsRequest(_tableName, 0, _totalRecords, null));

            return response.data.ToList();
        }

        [Benchmark]
        public List<AvroDecodeRecord> ParallelFetchDecode()
        {
            if (_kinetica == null)
                throw new InvalidOperationException("Setup not complete");

            // Parallel fetch by splitting into multiple requests
            var batchSize = Math.Max(1000, _totalRecords / Environment.ProcessorCount);
            var batches = new List<(int offset, int limit)>();

            for (int offset = 0; offset < _totalRecords; offset += batchSize)
            {
                var limit = Math.Min(batchSize, _totalRecords - offset);
                batches.Add((offset, limit));
            }

            var results = new List<AvroDecodeRecord>[batches.Count];

            Parallel.For(0, batches.Count, i =>
            {
                var (offset, limit) = batches[i];
                var response = _kinetica!.getRecords<AvroDecodeRecord>(
                    new GetRecordsRequest(_tableName, offset, limit, null));
                results[i] = response.data.ToList();
            });

            // Combine results
            var combined = new List<AvroDecodeRecord>(_totalRecords);
            foreach (var batch in results)
            {
                combined.AddRange(batch);
            }

            return combined;
        }
    }

    /// <summary>
    /// Standalone Avro decode benchmark runner (without BenchmarkDotNet).
    /// Useful for quick performance checks.
    /// </summary>
    public static class AvroDecodeBenchmarkRunner
    {
        public static void Run()
        {
            Console.WriteLine("====================================================================");
            Console.WriteLine("  AVRO DECODING PERFORMANCE BENCHMARK");
            Console.WriteLine("====================================================================");
            Console.WriteLine();
            Console.WriteLine("  Comparing:");
            Console.WriteLine("    * Sequential fetch + decode");
            Console.WriteLine("    * Parallel fetch + decode (multiple requests)");
            Console.WriteLine();

            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            Console.WriteLine($"Connected to: {url}");
            Console.WriteLine();

            var kinetica = new kinetica.Kinetica(url, new kinetica.Kinetica.Options
            {
                Username = user,
                Password = password
            });

            // Test configurations: (recordCount, payloadSize)
            var testConfigs = new (int records, int payload)[]
            {
                (1000, 64),
                (10000, 64),
                (50000, 64),
                (100000, 64),
                (100000, 256),
            };

            var schemaName = "decode_bench";
            var tableName = $"{schemaName}.decode_test";

            foreach (var (recordCount, payloadSize) in testConfigs)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine($"  Test: {recordCount:N0} records, {payloadSize} byte payload");
                Console.WriteLine("--------------------------------------------------------------------");

                // Setup
                try { kinetica.executeSql($"DROP SCHEMA IF EXISTS {schemaName} CASCADE"); } catch { }
                try { kinetica.executeSql($"CREATE SCHEMA {schemaName}"); } catch { }

                kinetica.executeSql($@"
                    CREATE TABLE {tableName} (
                        id INT NOT NULL,
                        category VARCHAR(64),
                        value DOUBLE,
                        timestamp LONG,
                        payload VARCHAR({payloadSize}),
                        PRIMARY KEY (id)
                    )
                ");

                // Insert records using SQL for simplicity (avoids type registration issues)
                Console.WriteLine($"  Inserting {recordCount:N0} records...");
                var payloadTemplate = new string('x', payloadSize);
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Insert in batches using SQL
                const int sqlBatchSize = 1000;
                for (int batch = 0; batch < recordCount; batch += sqlBatchSize)
                {
                    var batchEnd = Math.Min(batch + sqlBatchSize, recordCount);
                    var values = new List<string>();
                    for (int i = batch; i < batchEnd; i++)
                    {
                        values.Add($"({i}, 'cat_{i % 100}', {i * 0.01}, {timestamp}, '{payloadTemplate}')");
                    }
                    kinetica.executeSql($"INSERT INTO {tableName} (id, category, value, timestamp, payload) VALUES {string.Join(",", values)}");
                }

                // Warm up
                Console.WriteLine("  Warming up...");
                kinetica.getRecords<AvroDecodeRecord>(new GetRecordsRequest(tableName, 0, 100, null));

                // Benchmark: Sequential
                Console.WriteLine("  Running sequential benchmark...");
                var sw = Stopwatch.StartNew();
                var seqResponse = kinetica.getRecords<AvroDecodeRecord>(
                    new GetRecordsRequest(tableName, 0, recordCount, null));
                var seqTime = sw.Elapsed;
                var seqThroughput = recordCount / seqTime.TotalSeconds;

                // Benchmark: Parallel
                Console.WriteLine("  Running parallel benchmark...");
                sw.Restart();
                var batchSize = Math.Max(1000, recordCount / Environment.ProcessorCount);
                var batches = new List<(int offset, int limit)>();

                for (int offset = 0; offset < recordCount; offset += batchSize)
                {
                    var limit = Math.Min(batchSize, recordCount - offset);
                    batches.Add((offset, limit));
                }

                var parResults = new IList<AvroDecodeRecord>[batches.Count];
                Parallel.For(0, batches.Count, i =>
                {
                    var (offset, limit) = batches[i];
                    var response = kinetica.getRecords<AvroDecodeRecord>(
                        new GetRecordsRequest(tableName, offset, limit, null));
                    parResults[i] = response.data;
                });
                var parTime = sw.Elapsed;
                var parThroughput = recordCount / parTime.TotalSeconds;

                // Results
                Console.WriteLine();
                Console.WriteLine($"  {"Method",-45} | {"Records",-8} | {"Time",-12} | {"Throughput",-15}");
                Console.WriteLine($"  {new string('-', 85)}");
                Console.WriteLine($"  {"Sequential",-45} | {recordCount,-8:N0} | {seqTime.TotalMilliseconds,-8:F2} ms | {seqThroughput,-12:N0} rec/s");
                Console.WriteLine($"  {"Parallel (multi-request)",-45} | {recordCount,-8:N0} | {parTime.TotalMilliseconds,-8:F2} ms | {parThroughput,-12:N0} rec/s");
                Console.WriteLine();
                Console.WriteLine($"  Speedup: {seqTime.TotalMilliseconds / parTime.TotalMilliseconds:F2}x");
                Console.WriteLine();
            }

            // Cleanup
            try { kinetica.executeSql($"DROP SCHEMA IF EXISTS {schemaName} CASCADE"); } catch { }

            Console.WriteLine("====================================================================");
            Console.WriteLine("  BENCHMARK COMPLETE");
            Console.WriteLine("====================================================================");
            Console.WriteLine();
            Console.WriteLine("Summary:");
            Console.WriteLine("  * Parallel fetching can improve throughput for large result sets");
            Console.WriteLine("  * Network latency dominates for small result sets");
            Console.WriteLine("  * Use parallel fetch when results exceed 10K records");
            Console.WriteLine();
        }
    }
}
