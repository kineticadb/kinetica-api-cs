/*
 * BulkInserter Monitoring Dashboard Example
 *
 * This example demonstrates how to build a real-time monitoring dashboard
 * for a BulkInserter instance. The dashboard components (MetricsCollector
 * and ConsoleDashboard) are designed to be reusable and can be integrated
 * into any application using BulkInserter.
 *
 * Run with:
 *   KINETICA_URL="http://localhost:9191" dotnet run --project Example -- --dashboard
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using kinetica;
using kinetica.Records;
using kinetica.Utils;

namespace Example
{
    #region Dashboard Metrics

    /// <summary>
    /// Comprehensive metrics collected from a BulkInserter.
    /// This struct contains all the metrics that can be displayed in a dashboard.
    /// </summary>
    public class DashboardMetrics
    {
        // Timestamp
        public long TimestampMs { get; set; }
        public double UptimeSecs { get; set; }

        // Record counts
        public long TotalInserted { get; set; }
        public long TotalUpdated { get; set; }
        public long TotalProcessed { get; set; }

        // Rates (per second)
        public double InsertRate { get; set; }
        public double UpdateRate { get; set; }
        public double ThroughputRate { get; set; }

        // Queue metrics
        public long PendingBatches { get; set; }
        public int InFlightBatches { get; set; }
        public int AvailablePermits { get; set; }
        public int MaxInFlight { get; set; }
        public double UtilizationPct { get; set; }

        // Error metrics
        public int ErrorCount { get; set; }
        public double ErrorRate { get; set; }
        public List<ErrorSummary> RecentErrors { get; set; } = new();

        // Configuration
        public int WorkerCount { get; set; }
        public int BatchSize { get; set; }
        public bool MultiHeadEnabled { get; set; }

        // Derived metrics
        public double AvgBatchTimeMs { get; set; }
        public double EstimatedQueueDrainSecs { get; set; }
    }

    public class ErrorSummary
    {
        public string WorkerUrl { get; set; } = "";
        public string Message { get; set; } = "";
        public int RecordCount { get; set; }
        public double AgeSecs { get; set; }
    }

    #endregion

    #region Metrics Collector

    /// <summary>
    /// Collects and aggregates metrics from a BulkInserter.
    /// The collector does NOT own the BulkInserter and does NOT interfere
    /// with insert operations.
    /// </summary>
    public class MetricsCollector<T> where T : IShardKeyExtractor, new()
    {
        private readonly BulkInserter<T> _inserter;
        private readonly Stopwatch _startTime;

        // Previous values for rate calculation
        private long _prevInserted;
        private long _prevUpdated;
        private int _prevErrors;
        private Stopwatch _prevTimestamp;

        // Historical data for averaging
        private readonly ConcurrentQueue<double> _rateHistory = new();
        private const int MaxHistorySize = 60;

        public MetricsCollector(BulkInserter<T> inserter)
        {
            _inserter = inserter;
            _startTime = Stopwatch.StartNew();
            _prevTimestamp = Stopwatch.StartNew();
        }

        /// <summary>
        /// Collect current metrics snapshot from the BulkInserter.
        /// This method is thread-safe and does not interfere with insert operations.
        /// </summary>
        public DashboardMetrics Collect()
        {
            var now = Stopwatch.StartNew();
            var uptime = _startTime.Elapsed;

            // Read metrics from the BulkInserter
            var totalInserted = _inserter.CountInserted;
            var totalUpdated = _inserter.CountUpdated;
            var totalProcessed = totalInserted + totalUpdated;

            // Backpressure metrics
            var bpMetrics = _inserter.GetBackpressureMetrics();

            // Error counts
            var errorCount = _inserter.ErrorCount;

            // Configuration
            var workerCount = _inserter.NumWorkers;
            var batchSize = bpMetrics.MaxInFlightBatches > 0 ? 10000 : 10000; // Default batch size
            var multiHeadEnabled = workerCount > 1;

            // Calculate rates
            var elapsed = _prevTimestamp.Elapsed.TotalSeconds;
            _prevTimestamp.Restart();

            var insertRate = elapsed > 0 ? (totalInserted - _prevInserted) / elapsed : 0;
            var updateRate = elapsed > 0 ? (totalUpdated - _prevUpdated) / elapsed : 0;
            var throughputRate = insertRate + updateRate;
            var errorRate = elapsed > 0 ? (errorCount - _prevErrors) / elapsed : 0;

            // Update rate history
            _rateHistory.Enqueue(throughputRate);
            while (_rateHistory.Count > MaxHistorySize)
                _rateHistory.TryDequeue(out _);

            // Store current values for next rate calculation
            _prevInserted = totalInserted;
            _prevUpdated = totalUpdated;
            _prevErrors = errorCount;

            // Calculate derived metrics
            var avgBatchTimeMs = throughputRate > 0
                ? (batchSize / throughputRate) * 1000.0
                : 0;

            var estimatedQueueDrainSecs = throughputRate > 0
                ? (bpMetrics.PendingBatches * batchSize) / throughputRate
                : double.PositiveInfinity;

            return new DashboardMetrics
            {
                TimestampMs = (long)uptime.TotalMilliseconds,
                UptimeSecs = uptime.TotalSeconds,

                TotalInserted = totalInserted,
                TotalUpdated = totalUpdated,
                TotalProcessed = totalProcessed,

                InsertRate = insertRate,
                UpdateRate = updateRate,
                ThroughputRate = throughputRate,

                PendingBatches = bpMetrics.PendingBatches,
                InFlightBatches = bpMetrics.InFlightBatches,
                AvailablePermits = bpMetrics.AvailablePermits,
                MaxInFlight = bpMetrics.MaxInFlightBatches,
                UtilizationPct = bpMetrics.UtilizationPercent,

                ErrorCount = errorCount,
                ErrorRate = errorRate,

                WorkerCount = workerCount,
                BatchSize = batchSize,
                MultiHeadEnabled = multiHeadEnabled,

                AvgBatchTimeMs = avgBatchTimeMs,
                EstimatedQueueDrainSecs = estimatedQueueDrainSecs,
            };
        }

        public double GetAvgThroughput()
        {
            if (_rateHistory.IsEmpty) return 0;
            double sum = 0;
            int count = 0;
            foreach (var rate in _rateHistory)
            {
                sum += rate;
                count++;
            }
            return count > 0 ? sum / count : 0;
        }
    }

    #endregion

    #region Console Dashboard

    /// <summary>
    /// Renders DashboardMetrics to the console with a visual layout.
    /// </summary>
    public class ConsoleDashboard
    {
        private readonly TimeSpan _refreshInterval;
        private readonly bool _showErrors;

        public ConsoleDashboard(TimeSpan refreshInterval, bool showErrors = true)
        {
            _refreshInterval = refreshInterval;
            _showErrors = showErrors;
        }

        private void ClearScreen()
        {
            Console.Write("\x1B[2J\x1B[H");
        }

        public void Render(DashboardMetrics metrics)
        {
            ClearScreen();

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|                    BULKINSERTER MONITORING DASHBOARD                            |");
            Console.WriteLine("+----------------------------------------------------------------------------------+");

            // Uptime and timestamp
            Console.WriteLine($"|  Uptime: {metrics.UptimeSecs,10:F1}s                                   {DateTime.Now:yyyy-MM-dd HH:mm:ss} |");

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|  THROUGHPUT                                                                      |");
            Console.WriteLine("+----------------------------------------------------------------------------------+");

            // Record counts
            Console.WriteLine($"|  Records Inserted: {FormatNumber(metrics.TotalInserted),15}    Records Updated: {FormatNumber(metrics.TotalUpdated),15}      |");
            Console.WriteLine($"|  Total Processed:  {FormatNumber(metrics.TotalProcessed),15}                                            |");
            Console.WriteLine("|                                                                                  |");

            // Rates
            var rateBar = CreateRateBar(metrics.ThroughputRate, 500_000, 30);
            Console.WriteLine($"|  Insert Rate:  {metrics.InsertRate,12:F0} rec/s                                          |");
            Console.WriteLine($"|  Update Rate:  {metrics.UpdateRate,12:F0} rec/s                                          |");
            Console.WriteLine($"|  Total Rate:   {metrics.ThroughputRate,12:F0} rec/s  [{rateBar}]       |");

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|  QUEUE & BACKPRESSURE                                                            |");
            Console.WriteLine("+----------------------------------------------------------------------------------+");

            // Queue status
            Console.WriteLine($"|  Pending Batches:  {metrics.PendingBatches,6}    In-Flight: {metrics.InFlightBatches,6}    Available: {metrics.AvailablePermits,6}          |");

            // Utilization bar
            var utilBar = CreateUtilizationBar(metrics.UtilizationPct, 40);
            var utilStatus = metrics.UtilizationPct > 90 ? "BACKPRESSURE"
                : metrics.UtilizationPct > 70 ? "HIGH"
                : metrics.UtilizationPct > 30 ? "NORMAL"
                : "LOW";

            Console.WriteLine($"|  Utilization: {metrics.UtilizationPct,5:F1}%  [{utilBar}]  {utilStatus,12} |");

            if (!double.IsInfinity(metrics.EstimatedQueueDrainSecs))
            {
                Console.WriteLine($"|  Est. Queue Drain: {metrics.EstimatedQueueDrainSecs,6:F1}s                                                  |");
            }
            else
            {
                Console.WriteLine($"|  Est. Queue Drain:    N/A (no throughput)                                        |");
            }

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|  ERRORS                                                                          |");
            Console.WriteLine("+----------------------------------------------------------------------------------+");

            var errorStatus = metrics.ErrorCount > 0 ? "!" : "+";
            Console.WriteLine($"|  Errors: {metrics.ErrorCount,6} {errorStatus}    Error Rate: {metrics.ErrorRate,8:F2}/s                                  |");

            // Show recent errors if any
            if (_showErrors && metrics.RecentErrors.Count > 0)
            {
                Console.WriteLine("|                                                                                  |");
                Console.WriteLine("|  Recent Errors:                                                                  |");
                for (int i = 0; i < Math.Min(3, metrics.RecentErrors.Count); i++)
                {
                    var err = metrics.RecentErrors[i];
                    var msg = err.Message.Length > 50 ? err.Message[..47] + "..." : err.Message;
                    Console.WriteLine($"|    {i + 1}. {msg} ({err.RecordCount} recs)");
                }
            }

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine("|  CONFIGURATION                                                                   |");
            Console.WriteLine("+----------------------------------------------------------------------------------+");

            var multiHead = metrics.MultiHeadEnabled ? "Enabled" : "Disabled";
            Console.WriteLine($"|  Workers: {metrics.WorkerCount,3}    Batch Size: {metrics.BatchSize,6}    Multi-Head: {multiHead,8}                 |");
            Console.WriteLine($"|  Max In-Flight: {metrics.MaxInFlight,3}    Avg Batch Time: {metrics.AvgBatchTimeMs,8:F1}ms                            |");

            Console.WriteLine("+----------------------------------------------------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop");
        }

        private static string FormatNumber(long n)
        {
            if (n >= 1_000_000_000)
                return $"{n / 1_000_000_000.0:F2}B";
            if (n >= 1_000_000)
                return $"{n / 1_000_000.0:F2}M";
            if (n >= 1_000)
                return $"{n / 1_000.0:F2}K";
            return n.ToString();
        }

        private static string CreateRateBar(double rate, double maxRate, int width)
        {
            var filled = (int)Math.Min(width, (rate / maxRate) * width);
            var empty = width - filled;
            return new string('#', filled) + new string('-', empty);
        }

        private static string CreateUtilizationBar(double pct, int width)
        {
            var filled = (int)Math.Min(width, (pct / 100.0) * width);
            var empty = width - filled;

            var barChar = pct > 90 ? '!' : pct > 70 ? '*' : '#';
            return new string(barChar, filled) + new string('-', empty);
        }
    }

    #endregion

    #region Dashboard Example

    public static class BulkInserterDashboardExample
    {
        /// <summary>
        /// Test record type for the dashboard example.
        /// </summary>
        private class DashboardTestRecord : IShardKeyExtractor
        {
            public int id { get; set; }
            public string category { get; set; } = "";
            public double value { get; set; }
            public long timestamp { get; set; }

            public ShardKeyValues GetShardKeyValues()
            {
                return new ShardKeyValues();
            }
        }

        public static async Task RunAsync()
        {
            Console.WriteLine("=============================================================");
            Console.WriteLine("  BulkInserter Monitoring Dashboard Example");
            Console.WriteLine("=============================================================");
            Console.WriteLine();
            Console.WriteLine("This example demonstrates how to:");
            Console.WriteLine("  1. Create a BulkInserter for data ingestion");
            Console.WriteLine("  2. Create a MetricsCollector to monitor the inserter");
            Console.WriteLine("  3. Run monitoring in parallel with data ingestion");
            Console.WriteLine();

            // Get connection settings
            var url = Environment.GetEnvironmentVariable("KINETICA_URL") ?? "http://localhost:9191";
            var user = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";

            Console.WriteLine($"Connecting to Kinetica at: {url}");

            // Create Kinetica connection
            var kinetica = new Kinetica(url, new Kinetica.Options
            {
                Username = user,
                Password = password
            });

            // Setup table
            var schemaName = "test_schema";
            var tableName = $"{schemaName}.dashboard_test";

            // Create schema (ignore if exists)
            try
            {
                kinetica.executeSql($"CREATE SCHEMA IF NOT EXISTS {schemaName}");
            }
            catch { }

            // Drop existing table
            try
            {
                kinetica.executeSql($"DROP TABLE IF EXISTS {tableName}");
            }
            catch { }

            // Create table
            kinetica.executeSql($@"
                CREATE TABLE {tableName} (
                    id INT NOT NULL,
                    category VARCHAR(64),
                    value DOUBLE,
                    timestamp LONG,
                    PRIMARY KEY (id),
                    SHARD KEY (id)
                )
            ");

            Console.WriteLine($"Created table: {tableName}");

            // Get the KineticaType for the table
            var showTableResponse = kinetica.showTable(tableName, null);
            var typeId = showTableResponse.type_ids[0];
            var ktype = KineticaType.fromTypeID(kinetica, typeId);

            // Create BulkInserter
            var options = new BulkInserterOptions
            {
                BatchSize = 10000,
                MaxInFlightBatches = 50,
                FlushIntervalSeconds = 10,
            };

            var inserter = new BulkInserter<DashboardTestRecord>(
                kinetica, tableName, ktype, options);

            Console.WriteLine("BulkInserter created");
            Console.WriteLine();

            // Create MetricsCollector
            var collector = new MetricsCollector<DashboardTestRecord>(inserter);
            Console.WriteLine("MetricsCollector created, connected to the BulkInserter");
            Console.WriteLine();

            // Create console dashboard
            var dashboard = new ConsoleDashboard(TimeSpan.FromSeconds(1));

            // Cancellation token for graceful shutdown
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // Start producer task
            var producerTask = Task.Run(async () =>
            {
                int id = 0;
                var categories = new[] { "A", "B", "C", "D", "E" };

                while (!cts.Token.IsCancellationRequested)
                {
                    // Generate batch of records
                    var batch = new List<DashboardTestRecord>();
                    for (int i = 0; i < 1000; i++)
                    {
                        id++;
                        batch.Add(new DashboardTestRecord
                        {
                            id = id,
                            category = categories[i % categories.Length],
                            value = id * 0.01,
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        });
                    }

                    // Insert using the BulkInserter
                    try
                    {
                        await inserter.InsertBatchAsync(batch);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Insert error: {ex.Message}");
                    }

                    await Task.Delay(10, cts.Token);
                }
            }, cts.Token);

            // Dashboard update loop
            Console.WriteLine("Starting dashboard - monitoring the active BulkInserter...\n");
            await Task.Delay(1000);

            while (!cts.Token.IsCancellationRequested)
            {
                var metrics = collector.Collect();
                dashboard.Render(metrics);

                try
                {
                    await Task.Delay(1000, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            // Graceful shutdown
            Console.WriteLine("\nShutting down...");

            // Wait for producer to stop
            try
            {
                await producerTask;
            }
            catch (OperationCanceledException) { }

            // Flush and close inserter
            await inserter.CloseAsync();

            Console.WriteLine($"Final stats:");
            Console.WriteLine($"  Inserted: {inserter.CountInserted}");
            Console.WriteLine($"  Updated: {inserter.CountUpdated}");
            Console.WriteLine($"  Errors: {inserter.ErrorCount}");

            // Cleanup
            try
            {
                kinetica.executeSql($"DROP TABLE IF EXISTS {tableName}");
            }
            catch { }

            Console.WriteLine("Done!");
        }
    }

    #endregion
}
