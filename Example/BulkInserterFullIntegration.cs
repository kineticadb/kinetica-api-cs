/// <summary>
/// BulkInserter Full Integration Example
///
/// This example demonstrates a complete, production-like integration of the
/// BulkInserter with all features working together:
///
/// 1. Multiple Data Sources - Simulates multiple producers (threads/tasks)
/// 2. Real-Time Monitoring - Dashboard tracks throughput, queue depth, errors
/// 3. Backpressure Control - Memory-bounded operation with semaphore limits
/// 4. Error Handling - Comprehensive error tracking and reporting
/// 5. Graceful Shutdown - Signal handling with proper flush and cleanup
/// 6. Progress Tracking - Periodic status updates during ingestion
///
/// Environment Variables:
///   KINETICA_URL          - Server URL (default: http://localhost:9191)
///   KINETICA_USER         - Username (default: admin)
///   KINETICA_PASSWORD     - Password (default: secret)
///   RECORDS_PER_PRODUCER  - Records each producer generates (default: 1000000)
///   NUM_PRODUCERS         - Number of concurrent producer tasks (default: 3)
///   BATCH_SIZE            - BulkInserter batch size (default: 10000)
///   MAX_IN_FLIGHT         - Max concurrent in-flight batches (default: 50)
///   MONITOR_INTERVAL_MS   - Dashboard update interval in ms (default: 500)
///
/// Run with:
///   KINETICA_URL="http://localhost:9191" dotnet run --project Example -- --integration
/// </summary>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;
using kinetica.Utils;

namespace Example
{
    #region Configuration

    public class IntegrationConfig
    {
        public int RecordsPerProducer { get; set; } = 1_000_000;
        public int NumProducers { get; set; } = 3;
        public int BatchSize { get; set; } = 10_000;
        public int MaxInFlight { get; set; } = 50;
        public int MonitorIntervalMs { get; set; } = 500;

        public static IntegrationConfig FromEnvironment()
        {
            return new IntegrationConfig
            {
                RecordsPerProducer = GetEnvInt("RECORDS_PER_PRODUCER", 1_000_000),
                NumProducers = GetEnvInt("NUM_PRODUCERS", 3),
                BatchSize = GetEnvInt("BATCH_SIZE", 10_000),
                MaxInFlight = GetEnvInt("MAX_IN_FLIGHT", 50),
                MonitorIntervalMs = GetEnvInt("MONITOR_INTERVAL_MS", 500),
            };
        }

        private static int GetEnvInt(string name, int defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
    }

    #endregion

    #region Sensor Record

    /// <summary>
    /// Sensor reading from IoT devices - test record type.
    /// </summary>
    public class SensorReading : IShardKeyExtractor
    {
        public int sensor_id { get; set; }
        public string location { get; set; } = "";
        public double temperature { get; set; }
        public double humidity { get; set; }
        public long timestamp { get; set; }

        public ShardKeyValues GetShardKeyValues()
        {
            return new ShardKeyValues();
        }
    }

    #endregion

    #region Producer Statistics

    public class ProducerStats
    {
        private long _recordsGenerated;
        private long _recordsQueued;
        private long _batchesSent;

        public long RecordsGenerated => Interlocked.Read(ref _recordsGenerated);
        public long RecordsQueued => Interlocked.Read(ref _recordsQueued);
        public long BatchesSent => Interlocked.Read(ref _batchesSent);

        public void AddGenerated(int count) => Interlocked.Add(ref _recordsGenerated, count);
        public void AddQueued(int count) => Interlocked.Add(ref _recordsQueued, count);
        public void IncrementBatches() => Interlocked.Increment(ref _batchesSent);
    }

    #endregion

    #region Monitor

    public class IntegrationMonitor<T> where T : IShardKeyExtractor, new()
    {
        private readonly BulkInserter<T> _inserter;
        private long _prevInserted;
        private Stopwatch _prevTimestamp;

        public IntegrationMonitor(BulkInserter<T> inserter)
        {
            _inserter = inserter;
            _prevTimestamp = Stopwatch.StartNew();
        }

        public (long inserted, long updated, long pending, double utilization, int errors, double rate) GetSnapshot()
        {
            var inserted = _inserter.CountInserted;
            var updated = _inserter.CountUpdated;
            var bp = _inserter.GetBackpressureMetrics();
            var errors = _inserter.ErrorCount;

            var elapsed = _prevTimestamp.Elapsed.TotalSeconds;
            _prevTimestamp.Restart();

            var rate = elapsed > 0 ? (inserted - _prevInserted) / elapsed : 0;
            _prevInserted = inserted;

            return (inserted, updated, bp.PendingBatches, bp.UtilizationPercent, errors, rate);
        }
    }

    #endregion

    #region Full Integration Example

    public static class BulkInserterFullIntegrationExample
    {
        private static readonly string[] Locations = {
            "warehouse-a", "warehouse-b", "factory-1", "factory-2",
            "office-hq", "office-branch", "datacenter-1", "datacenter-2"
        };

        public static async Task RunAsync()
        {
            var config = IntegrationConfig.FromEnvironment();

            Console.WriteLine("+==============================================================================+");
            Console.WriteLine("|              BULKINSERTER FULL INTEGRATION EXAMPLE                          |");
            Console.WriteLine("+==============================================================================+");
            Console.WriteLine("|                                                                              |");
            Console.WriteLine("|  This example demonstrates:                                                  |");
            Console.WriteLine("|    * Multiple concurrent producers inserting data                            |");
            Console.WriteLine("|    * Real-time monitoring of throughput and queue depth                      |");
            Console.WriteLine("|    * Backpressure control to bound memory usage                              |");
            Console.WriteLine("|    * Graceful shutdown with Ctrl+C                                           |");
            Console.WriteLine("|                                                                              |");
            Console.WriteLine("+==============================================================================+");
            Console.WriteLine();

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            Console.WriteLine("Configuration:");
            Console.WriteLine($"  Kinetica URL: {url}");
            Console.WriteLine($"  Producers: {config.NumProducers}");
            Console.WriteLine($"  Records per producer: {config.RecordsPerProducer:N0}");
            Console.WriteLine($"  Total records: {config.NumProducers * config.RecordsPerProducer:N0}");
            Console.WriteLine($"  Batch size: {config.BatchSize:N0}");
            Console.WriteLine($"  Max in-flight batches: {config.MaxInFlight}");
            Console.WriteLine();

            // Create Kinetica connection
            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            // Setup table
            var schemaName = "test_schema";
            var tableName = $"{schemaName}.integration_test";

            // Create schema
            try { kinetica.executeSql($"CREATE SCHEMA IF NOT EXISTS {schemaName}"); } catch { }

            // Drop existing table
            try { kinetica.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }

            // Create table
            kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    sensor_id INT NOT NULL,
                    location VARCHAR(64),
                    temperature DOUBLE,
                    humidity DOUBLE,
                    timestamp LONG,
                    SHARD KEY (sensor_id, location)
                )
            ");

            Console.WriteLine($"+ Table created: {tableName}");

            // Get KineticaType
            var showTableResponse = kinetica.showTable(tableName, null);
            var typeId = showTableResponse.type_ids[0];
            var ktype = KineticaType.fromTypeID(kinetica, typeId);

            // Create BulkInserter
            var options = new BulkInserterOptions
            {
                BatchSize = config.BatchSize,
                MaxInFlightBatches = config.MaxInFlight,
                FlushIntervalSeconds = 30,
            };

            var inserter = new BulkInserter<SensorReading>(
                kinetica, tableName, ktype, options);

            Console.WriteLine($"+ BulkInserter created");
            Console.WriteLine($"  Workers: {inserter.NumWorkers}");
            Console.WriteLine();

            // Setup shutdown handling
            var cts = new CancellationTokenSource();
            var running = true;

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\n\n! Shutdown signal received (Ctrl+C)");
                running = false;
                cts.Cancel();
            };

            // Start monitoring task
            var monitor = new IntegrationMonitor<SensorReading>(inserter);
            var startTime = Stopwatch.StartNew();

            var monitorTask = Task.Run(async () =>
            {
                while (running && !cts.Token.IsCancellationRequested)
                {
                    var (inserted, updated, pending, util, errors, rate) = monitor.GetSnapshot();

                    Console.Write($"\r[{startTime.Elapsed.TotalSeconds,6:F1}s] " +
                        $"Inserted: {FormatNumber(inserted),10} | " +
                        $"Rate: {rate,10:F0}/s | " +
                        $"Queue: {pending,3} ({util,5:F1}%) | " +
                        $"Errors: {errors}   ");

                    try
                    {
                        await Task.Delay(config.MonitorIntervalMs, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });

            // Start producer tasks
            Console.WriteLine($"Starting {config.NumProducers} producers...");
            Console.WriteLine();

            var producerStats = new ProducerStats[config.NumProducers];
            for (int i = 0; i < config.NumProducers; i++)
                producerStats[i] = new ProducerStats();

            var producerTasks = new Task<int>[config.NumProducers];
            for (int i = 0; i < config.NumProducers; i++)
            {
                var producerId = i;
                var stats = producerStats[i];
                var recordCount = config.RecordsPerProducer;

                producerTasks[i] = Task.Run(() =>
                    RunProducer(producerId, inserter, recordCount, cts.Token, stats));
            }

            // Wait for producers to complete
            var producerResults = await Task.WhenAll(producerTasks);
            var totalProduced = producerResults.Sum();

            var produceDuration = startTime.Elapsed;

            // Flush remaining batches
            Console.WriteLine("\n\nProducers complete. Flushing remaining batches...");

            running = false;
            cts.Cancel();
            try { await monitorTask; } catch { }

            var flushStart = Stopwatch.StartNew();
            await inserter.CloseAsync();
            var flushDuration = flushStart.Elapsed;
            var totalDuration = startTime.Elapsed;

            // Print results
            Console.WriteLine();
            Console.WriteLine("+==============================================================================+");
            Console.WriteLine("|                              FINAL RESULTS                                   |");
            Console.WriteLine("+==============================================================================+");

            var inserted = inserter.CountInserted;
            var updated = inserter.CountUpdated;
            var totalDb = inserted + updated;

            Console.WriteLine("|  RECORDS                                                                     |");
            Console.WriteLine($"|    Generated:     {FormatNumber(totalProduced),12}                                               |");
            Console.WriteLine($"|    Inserted:      {FormatNumber(inserted),12}                                               |");
            Console.WriteLine($"|    Updated:       {FormatNumber(updated),12}                                               |");
            Console.WriteLine($"|    Total in DB:   {FormatNumber(totalDb),12}                                               |");

            Console.WriteLine("+------------------------------------------------------------------------------+");
            Console.WriteLine("|  TIMING                                                                      |");
            Console.WriteLine($"|    Produce time:  {produceDuration.TotalSeconds,10:F2}s                                                |");
            Console.WriteLine($"|    Flush time:    {flushDuration.TotalSeconds,10:F2}s                                                |");
            Console.WriteLine($"|    Total time:    {totalDuration.TotalSeconds,10:F2}s                                                |");

            var throughput = totalDuration.TotalSeconds > 0 ? totalDb / totalDuration.TotalSeconds : 0;
            Console.WriteLine("+------------------------------------------------------------------------------+");
            Console.WriteLine("|  THROUGHPUT                                                                  |");
            Console.WriteLine($"|    Overall:       {throughput,10:F0} rec/s                                          |");

            var errors = inserter.DrainErrors();
            Console.WriteLine("+------------------------------------------------------------------------------+");
            Console.WriteLine("|  ERRORS                                                                      |");
            Console.WriteLine($"|    Errors:        {errors.Count,12}                                               |");

            if (errors.Count > 0)
            {
                Console.WriteLine("|                                                                              |");
                Console.WriteLine("|    Recent errors:                                                            |");
                foreach (var err in errors.Take(5))
                {
                    var msg = err.Message.Length > 40 ? err.Message[..37] + "..." : err.Message;
                    Console.WriteLine($"|      - {msg} ({err.RecordCount} records)");
                }
            }

            Console.WriteLine("+==============================================================================+");

            // Data integrity check
            var recordsLost = totalProduced - totalDb;
            if (recordsLost > 0)
            {
                Console.WriteLine();
                Console.WriteLine("! WARNING: Some records may not have been inserted!");
                Console.WriteLine($"  Generated: {totalProduced:N0}  Inserted: {totalDb:N0}  Lost: {recordsLost:N0}");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"+ DATA INTEGRITY CHECK PASSED: All {totalProduced:N0} records inserted successfully.");
            }

            // Cleanup
            Console.WriteLine("\nCleaning up...");
            try { kinetica.executeSql($"DROP TABLE IF EXISTS {tableName}"); } catch { }
            Console.WriteLine("+ Cleanup complete");

            Console.WriteLine();
            Console.WriteLine("Integration test finished!");
        }

        private static async Task<int> RunProducer(
            int producerId,
            BulkInserter<SensorReading> inserter,
            int recordsToGenerate,
            CancellationToken cancellationToken,
            ProducerStats stats)
        {
            const int batchSize = 1000;
            var totalSent = 0;
            var sensorIdBase = producerId * 10000;

            while (totalSent < recordsToGenerate && !cancellationToken.IsCancellationRequested)
            {
                var recordsInBatch = Math.Min(batchSize, recordsToGenerate - totalSent);

                var batch = new List<SensorReading>(recordsInBatch);
                for (int i = 0; i < recordsInBatch; i++)
                {
                    sensorIdBase++;
                    batch.Add(new SensorReading
                    {
                        sensor_id = sensorIdBase,
                        location = Locations[i % Locations.Length],
                        temperature = 20.0 + (sensorIdBase % 15),
                        humidity = 40.0 + (sensorIdBase % 30),
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    });
                }

                stats.AddGenerated(recordsInBatch);

                try
                {
                    await inserter.InsertBatchAsync(batch, cancellationToken);
                    stats.AddQueued(recordsInBatch);
                    stats.IncrementBatches();
                    totalSent += recordsInBatch;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"\nProducer {producerId} error: {ex.Message}");
                }

                // Yield periodically
                if (totalSent % 10000 == 0)
                    await Task.Yield();
            }

            return totalSent;
        }

        private static string FormatNumber(long n)
        {
            if (n >= 1_000_000)
                return $"{n / 1_000_000.0:F2}M";
            if (n >= 1_000)
                return $"{n / 1_000.0:F2}K";
            return n.ToString();
        }
    }

    #endregion
}
