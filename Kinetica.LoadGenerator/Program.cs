using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;

namespace Kinetica.LoadGenerator
{
    /// <summary>
    /// High-performance load generator for Kinetica bulk insert benchmarking.
    /// Matches the Rust load_generator implementation with multi-threaded producers,
    /// real-time progress reporting, and CSV output for result aggregation.
    /// </summary>
    public class Program
    {
        // Progress tracking batch size to reduce atomic operation contention
        private const int ProgressBatch = 10;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("= Kinetica Load Generator for C#    =");
            Console.WriteLine("======================================\n");

            // Parse configuration
            var config = LoadGeneratorConfig.Parse(args);
            config.Print();

            try
            {
                if (config.BetterSchema)
                {
                    await RunLoadGenerator<IngestRecord2>(config);
                }
                else
                {
                    await RunLoadGenerator<IngestRecord>(config);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }

        private static async Task RunLoadGenerator<T>(LoadGeneratorConfig config) where T : class, IShardKeyExtractor, new()
        {
            // Connect to Kinetica
            Console.WriteLine($"Connecting to Kinetica at {config.Url}...");
            var options = new kinetica.Kinetica.Options
            {
                Username = config.Username,
                Password = config.Password,
                UseSnappy = config.UseSnappy
            };

            var kinetica = new kinetica.Kinetica(config.Url, options);
            Console.WriteLine("Connected successfully.\n");

            // Ensure table exists
            await EnsureTableExists<T>(kinetica, config);

            // Create BulkInserter
            Console.WriteLine("Creating BulkInserter...");
            var ktype = KineticaType.fromClass(typeof(T), GetTypeProperties<T>());
            ktype.create(kinetica);

            var bulkInserterOptions = new BulkInserterOptions
            {
                BatchSize = config.BatchSize,
                MaxFlushWorkers = config.NumWorkers,
                MaxInFlightBatches = config.MaxInFlightBatches,
                MaxRetries = 3
            };

            using var inserter = new BulkInserter<T>(kinetica, config.TableName, ktype, bulkInserterOptions);
            Console.WriteLine($"BulkInserter created. Multi-head: {inserter.NumWorkers > 1}, Workers: {inserter.NumWorkers}\n");

            // Set up progress tracking
            var globalCounter = new Counter();
            var startTime = Stopwatch.StartNew();
            var startTimestamp = DateTime.UtcNow;

            // Start progress reporter task
            using var progressCts = new CancellationTokenSource();
            Task? progressTask = null;

            if (config.ProgressIntervalMs > 0)
            {
                progressTask = StartProgressReporter(globalCounter, startTime, config.ProgressIntervalMs, config.TotalRecords, progressCts.Token);
            }

            // Calculate work distribution per thread
            var basePerThread = config.TotalRecords / config.NumThreads;
            var remainder = config.TotalRecords % config.NumThreads;

            Console.WriteLine($"Starting {config.NumThreads} producer threads...\n");

            // Use a barrier for synchronized start
            using var barrier = new Barrier(config.NumThreads);

            // Launch producer threads
            var producerTasks = new List<Task>();
            for (int t = 0; t < config.NumThreads; t++)
            {
                var threadId = t;
                var threadRecords = basePerThread + (threadId < remainder ? 1 : 0);

                var task = Task.Run(() => ProducerThread<T>(
                    inserter, config, threadId, threadRecords, barrier, globalCounter));
                producerTasks.Add(task);
            }

            // Wait for all producers to complete
            await Task.WhenAll(producerTasks);

            Console.WriteLine("\nAll producer threads completed. Closing BulkInserter (flush + wait)...");

            // Close the inserter - this flushes all batches and waits for completion
            await inserter.CloseAsync();

            // Stop progress reporter
            progressCts.Cancel();
            if (progressTask != null)
            {
                try { await progressTask; } catch (OperationCanceledException) { }
            }

            // Final metrics
            startTime.Stop();
            var endTimestamp = DateTime.UtcNow;
            var elapsedMs = startTime.Elapsed.TotalMilliseconds;
            var elapsedSec = startTime.Elapsed.TotalSeconds;
            var finalTotal = inserter.CountInserted;
            var avgRate = finalTotal / elapsedSec;

            // Print results
            Console.WriteLine("\n======================================");
            Console.WriteLine("=           RESULTS                  =");
            Console.WriteLine("======================================");
            Console.WriteLine($"  Start Time:     {startTimestamp:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  End Time:       {endTimestamp:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Elapsed:        {elapsedSec:F2} seconds");
            Console.WriteLine($"  Records:        {finalTotal:N0}");
            Console.WriteLine($"  Throughput:     {avgRate:N2} records/second");
            Console.WriteLine($"  Updated:        {inserter.CountUpdated:N0}");
            Console.WriteLine($"  Errors:         {inserter.ErrorCount}");
            Console.WriteLine("======================================\n");

            // CSV output
            if (config.CsvOutput)
            {
                var hostname = Environment.MachineName;
                var csvLine = $"{startTimestamp:yyyy-MM-dd HH:mm:ss}," +
                              $"{hostname}," +
                              $"{config.Url}," +
                              $"{config.TableName}," +
                              $"CSharpAPI," +
                              $"{config.TotalRecords}," +
                              $"{config.MessageSize}," +
                              $"{config.NumThreads}," +
                              $"{config.BatchSize}," +
                              $"{elapsedSec:F2}," +
                              $"{avgRate:F2}";

                Console.WriteLine("CSV Output:");
                Console.WriteLine("start_ts,hostname,url,table,api,total_records,message_size,num_threads,batch_size,elapsed_sec,avg_rate");
                Console.WriteLine(csvLine);
            }

            // Check for errors
            var errors = inserter.DrainErrors();
            if (errors.Count > 0)
            {
                Console.WriteLine($"\nEncountered {errors.Count} errors:");
                foreach (var error in errors.Take(10))
                {
                    Console.WriteLine($"  - {error.Message} (records: {error.RecordCount})");
                }
                if (errors.Count > 10)
                {
                    Console.WriteLine($"  ... and {errors.Count - 10} more errors");
                }
            }
        }

        private static void ProducerThread<T>(
            BulkInserter<T> inserter,
            LoadGeneratorConfig config,
            int threadId,
            long threadRecords,
            Barrier barrier,
            Counter globalCounter) where T : class, IShardKeyExtractor, new()
        {
            // Pre-generate payload as string
            var rng = new Random(threadId * 12345 + Environment.TickCount);
            var payloadChars = new char[config.MessageSize];
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < config.MessageSize; i++)
            {
                payloadChars[i] = chars[rng.Next(chars.Length)];
            }
            var payload = new string(payloadChars);

            long localCount = 0;
            long lastReported = 0;

            // Synchronized start
            barrier.SignalAndWait();

            for (long j = 0; j < threadRecords; j++)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var nano = Stopwatch.GetTimestamp() * 1_000_000_000L / Stopwatch.Frequency;

                // Optionally randomize payload
                if (config.PayloadRandom)
                {
                    for (int i = 0; i < config.MessageSize; i++)
                    {
                        payloadChars[i] = chars[rng.Next(chars.Length)];
                    }
                    payload = new string(payloadChars);
                }

                // Build record based on schema type
                T record;
                if (typeof(T) == typeof(IngestRecord))
                {
                    var r = new IngestRecord
                    {
                        TimestampMillis = now,
                        Source = config.MessageSize,
                        Payload = payload
                    };
                    record = (T)(object)r;
                }
                else
                {
                    var r = new IngestRecord2
                    {
                        TimestampMillis = now,
                        Source = config.MessageSize,
                        ThreadId = threadId,
                        LocalCount = localCount,
                        Nanosecond = nano,
                        Payload = payload
                    };
                    record = (T)(object)r;
                }

                try
                {
                    inserter.Insert(record);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Thread {threadId} insert error: {ex.Message}");
                }

                localCount++;

                // Periodically update global counter in batches
                if (localCount - lastReported >= ProgressBatch)
                {
                    globalCounter.Add(localCount - lastReported);
                    lastReported = localCount;
                }

                // Optional detailed logging
                if (config.LogEveryN > 0 && localCount % config.LogEveryN == 0)
                {
                    Console.WriteLine($"[Thread {threadId}] Inserted {localCount:N0} records");
                }
            }

            // Final progress update
            var leftover = localCount - lastReported;
            if (leftover > 0)
            {
                globalCounter.Add(leftover);
            }
        }

        private static async Task StartProgressReporter(
            Counter globalCounter,
            Stopwatch startTime,
            int intervalMs,
            long totalRecords,
            CancellationToken cancellationToken)
        {
            long lastCount = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(intervalMs, cancellationToken);

                    var total = globalCounter.Value;
                    var delta = total - lastCount;
                    lastCount = total;

                    var rate = delta / (intervalMs / 1000.0);
                    var elapsedSec = startTime.Elapsed.TotalSeconds;
                    var overallRate = total / Math.Max(elapsedSec, 0.001);
                    var percentComplete = (double)total / totalRecords * 100;

                    Console.WriteLine($"[PROGRESS] {DateTime.UtcNow:HH:mm:ss} elapsed={elapsedSec:F1}s total={total:N0} " +
                                    $"rate={rate:N0} rec/s overall={overallRate:N0} rec/s ({percentComplete:F1}%)");
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        private static Task EnsureTableExists<T>(kinetica.Kinetica kinetica, LoadGeneratorConfig config)
        {
            Console.WriteLine($"Checking if table '{config.TableName}' exists...");

            // Check if table exists
            var hasTableResponse = kinetica.hasTable(config.TableName);
            var tableExists = hasTableResponse.table_exists;

            if (tableExists)
            {
                Console.WriteLine($"Table '{config.TableName}' exists.");

                if (config.TruncateTable)
                {
                    Console.WriteLine($"Truncating table '{config.TableName}'...");
                    kinetica.clearTable(config.TableName, "", new Dictionary<string, string>
                    {
                        [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE
                    });
                    Console.WriteLine("Table truncated.");
                }
            }
            else
            {
                Console.WriteLine($"Table '{config.TableName}' does not exist. Creating...");

                // Check if schema needs to be created
                var tableParts = config.TableName.Split('.');
                if (tableParts.Length > 1)
                {
                    var schemaName = tableParts[0];
                    try
                    {
                        kinetica.createSchema(schemaName, new Dictionary<string, string>
                        {
                            [CreateSchemaRequest.Options.NO_ERROR_IF_EXISTS] = CreateSchemaRequest.Options.TRUE
                        });
                        Console.WriteLine($"Schema '{schemaName}' created or already exists.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not create schema '{schemaName}': {ex.Message}");
                    }
                }

                // Create the type
                var typeProperties = GetTypeProperties<T>();
                var ktype = KineticaType.fromClass(typeof(T), typeProperties);
                var typeId = ktype.create(kinetica);
                Console.WriteLine($"Type created with ID: {typeId}");

                // Create the table
                kinetica.createTable(config.TableName, typeId, new Dictionary<string, string>());
                Console.WriteLine($"Table '{config.TableName}' created.\n");
            }

            return Task.CompletedTask;
        }

        private static Dictionary<string, IList<string>> GetTypeProperties<T>()
        {
            if (typeof(T) == typeof(IngestRecord))
            {
                return IngestRecord.GetTypeProperties();
            }
            else if (typeof(T) == typeof(IngestRecord2))
            {
                return IngestRecord2.GetTypeProperties();
            }
            else
            {
                return new Dictionary<string, IList<string>>();
            }
        }

        /// <summary>
        /// Thread-safe counter with minimal contention.
        /// </summary>
        private class Counter
        {
            private long _value;

            public long Value => Interlocked.Read(ref _value);

            public void Add(long amount) => Interlocked.Add(ref _value, amount);
        }
    }
}
