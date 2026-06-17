using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using kinetica;
using kinetica.Records;

namespace Kinetica.Benchmarks
{
    /// <summary>
    /// Benchmarks for the BulkInserter comparing different batch sizes,
    /// insertion methods, and concurrency patterns.
    /// </summary>
    [Config(typeof(BenchmarkConfig))]
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 1, iterationCount: 3)]
    public class BulkInserterBenchmarks
    {
        private kinetica.Kinetica _kinetica = null!;
        private string _tableName = null!;
        private KineticaType _ktype = null!;
        private List<BenchRecord> _records = null!;

        #region Setup

        [GlobalSetup]
        public void GlobalSetup()
        {
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var username = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            var options = new kinetica.Kinetica.Options
            {
                Username = username,
                Password = password,
                UseSnappy = false
            };

            _kinetica = new kinetica.Kinetica(url, options);

            // Generate records for benchmarks
            _records = GenerateRecords(RecordCount);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            // Create a fresh table for each iteration
            var schemaName = $"bench_{Guid.NewGuid():N}".Substring(0, 20);

            try
            {
                _kinetica.createSchema(schemaName, new Dictionary<string, string> { { "no_error_if_exists", "true" } });
            }
            catch { }

            var typeDef = @"{""type"":""record"",""name"":""bench_record"",""fields"":[" +
                @"{""name"":""id"",""type"":""int""}," +
                @"{""name"":""thread_id"",""type"":""int""}," +
                @"{""name"":""timestamp"",""type"":""long""}," +
                @"{""name"":""name"",""type"":""string""}," +
                @"{""name"":""score"",""type"":""double""}]}";

            var properties = new Dictionary<string, IList<string>>
            {
                { "id", new List<string> { "primary_key" } },
                { "thread_id", new List<string>() },
                { "timestamp", new List<string>() },
                { "name", new List<string>() },
                { "score", new List<string>() }
            };

            var typeResp = _kinetica.createType(typeDef, "bench_type", properties, new Dictionary<string, string>());
            _tableName = $"{schemaName}.bench_table";
            _kinetica.createTable(_tableName, typeResp.type_id, new Dictionary<string, string>());
            _ktype = KineticaType.fromTable(_kinetica, _tableName);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            try
            {
                var schemaName = _tableName.Split('.')[0];
                _kinetica.executeSql($"DROP SCHEMA IF EXISTS {schemaName} CASCADE", 0, -9999);
            }
            catch { }
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            // Cleanup handled by IterationCleanup
        }

        #endregion

        #region Parameters

        [Params(10000, 50000)]
        public int RecordCount { get; set; }

        [Params(1000, 10000)]
        public int BatchSize { get; set; }

        #endregion

        #region Record Generation

        public class BenchRecord : IShardKeyExtractor
        {
            public int id { get; set; }
            public int thread_id { get; set; }
            public long timestamp { get; set; }
            public string name { get; set; } = string.Empty;
            public double score { get; set; }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues();
            }
        }

        private List<BenchRecord> GenerateRecords(int count)
        {
            var records = new List<BenchRecord>(count);
            var baseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (int i = 0; i < count; i++)
            {
                records.Add(new BenchRecord
                {
                    id = i,
                    thread_id = Environment.CurrentManagedThreadId,
                    timestamp = baseTimestamp + i,
                    name = $"record_{i:D8}",
                    score = i * 1.5
                });
            }

            return records;
        }

        #endregion

        #region Benchmarks

        [Benchmark(Baseline = true)]
        public async Task BulkInserter_InsertBatch()
        {
            var options = new BulkInserterOptions
            {
                BatchSize = BatchSize
            };

            await using var inserter = new BulkInserter<BenchRecord>(_kinetica, _tableName, _ktype, options);

            inserter.InsertBatch(_records);

            await inserter.CloseAsync();
        }

        [Benchmark]
        public async Task BulkInserter_InsertBatchAsync()
        {
            var options = new BulkInserterOptions
            {
                BatchSize = BatchSize,
                MaxInFlightBatches = 20
            };

            await using var inserter = new BulkInserter<BenchRecord>(_kinetica, _tableName, _ktype, options);

            await inserter.InsertBatchAsync(_records);

            await inserter.CloseAsync();
        }

        [Benchmark]
        public async Task BulkInserter_SingleInserts()
        {
            var options = new BulkInserterOptions
            {
                BatchSize = BatchSize
            };

            await using var inserter = new BulkInserter<BenchRecord>(_kinetica, _tableName, _ktype, options);

            foreach (var record in _records)
            {
                inserter.Insert(record);
            }

            await inserter.CloseAsync();
        }

        [Benchmark]
        public async Task BulkInserter_ConcurrentInserts()
        {
            var options = new BulkInserterOptions
            {
                BatchSize = BatchSize,
                NumStripes = 8,
                MaxInFlightBatches = 20
            };

            await using var inserter = new BulkInserter<BenchRecord>(_kinetica, _tableName, _ktype, options);

            var chunkSize = _records.Count / 4;
            var tasks = new Task[4];

            for (int t = 0; t < 4; t++)
            {
                var start = t * chunkSize;
                var end = (t == 3) ? _records.Count : (t + 1) * chunkSize;
                var chunk = _records.GetRange(start, end - start);

                tasks[t] = Task.Run(async () =>
                {
                    foreach (var record in chunk)
                    {
                        await inserter.InsertAsync(record);
                    }
                });
            }

            await Task.WhenAll(tasks);
            await inserter.CloseAsync();
        }

        // These benchmarks intentionally exercise the deprecated
        // KineticaIngestor<T> so its performance can be compared against
        // BulkInserter<T>; suppress the obsolete-usage warning for this region.
#pragma warning disable CS0618 // Type or member is obsolete
        [Benchmark]
        public void LegacyIngestor_SingleInserts()
        {
            var ingestor = new KineticaIngestor<BenchRecord>(
                _kinetica,
                _tableName,
                BatchSize,
                _ktype);

            foreach (var record in _records)
            {
                ingestor.insert(record);
            }

            ingestor.flush();
        }

        [Benchmark]
        public void LegacyIngestor_BatchInsert()
        {
            var ingestor = new KineticaIngestor<BenchRecord>(
                _kinetica,
                _tableName,
                BatchSize,
                _ktype);

            ingestor.insert(_records);
            ingestor.flush();
        }
#pragma warning restore CS0618

        #endregion
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddJob(Job.Default
                .WithWarmupCount(1)
                .WithIterationCount(3));
        }
    }
}
