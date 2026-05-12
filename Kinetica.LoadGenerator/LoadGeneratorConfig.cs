using System;

namespace Kinetica.LoadGenerator
{
    /// <summary>
    /// Configuration for the load generator, matching Rust load_generator options.
    /// All options can be configured via environment variables or command-line arguments.
    /// </summary>
    public class LoadGeneratorConfig
    {
        /// <summary>
        /// Kinetica server URL (e.g., http://localhost:9191).
        /// Environment variable: KINETICA_URL
        /// </summary>
        public string Url { get; set; } = "http://localhost:9191";

        /// <summary>
        /// Username for Kinetica authentication.
        /// Environment variable: KINETICA_USER
        /// Default: "admin"
        /// </summary>
        public string Username { get; set; } = "admin";

        /// <summary>
        /// Password for Kinetica authentication.
        /// Environment variable: KINETICA_PASSWORD
        /// Default: "secret"
        /// </summary>
        public string Password { get; set; } = "secret";

        /// <summary>
        /// Target table name. Will be created if it doesn't exist.
        /// Environment variable: KINETICA_TABLE
        /// </summary>
        public string TableName { get; set; } = "load_generator_test";

        /// <summary>
        /// Total number of records to insert.
        /// Environment variable: TOTAL_RECORDS
        /// </summary>
        public long TotalRecords { get; set; } = 1_000_000;

        /// <summary>
        /// Number of records per batch for BulkInserter.
        /// Environment variable: KINETICA_BATCH_SIZE
        /// Default: 10,000
        /// </summary>
        public int BatchSize { get; set; } = 10_000;

        /// <summary>
        /// Number of producer threads.
        /// Environment variable: NUM_THREADS
        /// Default: 1
        /// </summary>
        public int NumThreads { get; set; } = 1;

        /// <summary>
        /// Number of flush workers for BulkInserter.
        /// Environment variable: NUM_WORKERS
        /// Default: 16
        /// </summary>
        public int NumWorkers { get; set; } = 16;

        /// <summary>
        /// Size of the payload field in bytes.
        /// Environment variable: MESSAGE_SIZE
        /// Default: 1024
        /// </summary>
        public int MessageSize { get; set; } = 1024;

        /// <summary>
        /// Whether to randomize payload for each record.
        /// If false, the same payload is reused for better throughput.
        /// Environment variable: PAYLOAD_RANDOM
        /// Default: false
        /// </summary>
        public bool PayloadRandom { get; set; } = false;

        /// <summary>
        /// Use the extended schema (IngestRecord2) with thread tracking.
        /// Environment variable: BETTER_SCHEMA
        /// Default: false
        /// </summary>
        public bool BetterSchema { get; set; } = false;

        /// <summary>
        /// Enable multi-head ingest for improved performance.
        /// Environment variable: KINETICA_MULTIHEAD
        /// Default: true
        /// </summary>
        public bool UseMultihead { get; set; } = true;

        /// <summary>
        /// Interval in milliseconds for progress reporting.
        /// Set to 0 to disable progress reporting.
        /// Environment variable: PROGRESS_INTERVAL_MS
        /// Default: 1000
        /// </summary>
        public int ProgressIntervalMs { get; set; } = 1000;

        /// <summary>
        /// Log every N records (0 to disable detailed logging).
        /// Environment variable: LOG_EVERY_N
        /// Default: 0
        /// </summary>
        public long LogEveryN { get; set; } = 0;

        /// <summary>
        /// Output results in CSV format for aggregation.
        /// Environment variable: CSV_OUTPUT
        /// Default: true
        /// </summary>
        public bool CsvOutput { get; set; } = true;

        /// <summary>
        /// Truncate the table before inserting if it exists.
        /// Environment variable: TRUNCATE_TABLE
        /// Default: false
        /// </summary>
        public bool TruncateTable { get; set; } = false;

        /// <summary>
        /// Maximum number of in-flight batches for backpressure control.
        /// Environment variable: MAX_IN_FLIGHT_BATCHES
        /// Default: 100
        /// </summary>
        public int MaxInFlightBatches { get; set; } = 100;

        /// <summary>
        /// Enable Snappy compression for HTTP requests.
        /// Reduces network transfer time for large payloads.
        /// Environment variable: SNAPPY
        /// Default: false
        /// </summary>
        public bool UseSnappy { get; set; } = false;

        /// <summary>
        /// Creates a configuration from environment variables.
        /// </summary>
        public static LoadGeneratorConfig FromEnvironment()
        {
            var config = new LoadGeneratorConfig();

            config.Url = GetEnvString("KINETICA_URL", config.Url);
            config.Username = GetEnvString("KINETICA_USER", config.Username);
            config.Password = GetEnvString("KINETICA_PASSWORD", config.Password);
            config.TableName = GetEnvString("KINETICA_TABLE", config.TableName);
            config.TotalRecords = GetEnvLong("TOTAL_RECORDS", config.TotalRecords);
            config.BatchSize = GetEnvInt("KINETICA_BATCH_SIZE", config.BatchSize);
            config.NumThreads = GetEnvInt("NUM_THREADS", config.NumThreads);
            config.NumWorkers = GetEnvInt("NUM_WORKERS", config.NumWorkers);
            config.MessageSize = GetEnvInt("MESSAGE_SIZE", config.MessageSize);
            config.PayloadRandom = GetEnvBool("PAYLOAD_RANDOM", config.PayloadRandom);
            config.BetterSchema = GetEnvBool("BETTER_SCHEMA", config.BetterSchema);
            config.UseMultihead = GetEnvBool("KINETICA_MULTIHEAD", config.UseMultihead);
            config.ProgressIntervalMs = GetEnvInt("PROGRESS_INTERVAL_MS", config.ProgressIntervalMs);
            config.LogEveryN = GetEnvLong("LOG_EVERY_N", config.LogEveryN);
            config.CsvOutput = GetEnvBool("CSV_OUTPUT", config.CsvOutput);
            config.TruncateTable = GetEnvBool("TRUNCATE_TABLE", config.TruncateTable);
            config.MaxInFlightBatches = GetEnvInt("MAX_IN_FLIGHT_BATCHES", config.MaxInFlightBatches);
            config.UseSnappy = GetEnvBool("SNAPPY", config.UseSnappy);

            return config;
        }

        /// <summary>
        /// Parses command-line arguments and merges with environment variables.
        /// Command-line arguments take precedence over environment variables.
        /// </summary>
        public static LoadGeneratorConfig Parse(string[] args)
        {
            // Start with environment variables
            var config = FromEnvironment();

            // Parse command-line arguments
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToLowerInvariant();

                if (arg == "--url" && i + 1 < args.Length)
                    config.Url = args[++i];
                else if (arg == "--user" && i + 1 < args.Length)
                    config.Username = args[++i];
                else if (arg == "--password" && i + 1 < args.Length)
                    config.Password = args[++i];
                else if (arg == "--table" && i + 1 < args.Length)
                    config.TableName = args[++i];
                else if (arg == "--records" && i + 1 < args.Length)
                    config.TotalRecords = long.Parse(args[++i]);
                else if (arg == "--batch-size" && i + 1 < args.Length)
                    config.BatchSize = int.Parse(args[++i]);
                else if (arg == "--threads" && i + 1 < args.Length)
                    config.NumThreads = int.Parse(args[++i]);
                else if (arg == "--workers" && i + 1 < args.Length)
                    config.NumWorkers = int.Parse(args[++i]);
                else if (arg == "--message-size" && i + 1 < args.Length)
                    config.MessageSize = int.Parse(args[++i]);
                else if (arg == "--random-payload")
                    config.PayloadRandom = true;
                else if (arg == "--better-schema")
                    config.BetterSchema = true;
                else if (arg == "--no-multihead")
                    config.UseMultihead = false;
                else if (arg == "--progress-interval" && i + 1 < args.Length)
                    config.ProgressIntervalMs = int.Parse(args[++i]);
                else if (arg == "--no-csv")
                    config.CsvOutput = false;
                else if (arg == "--truncate")
                    config.TruncateTable = true;
                else if (arg == "--max-in-flight" && i + 1 < args.Length)
                    config.MaxInFlightBatches = int.Parse(args[++i]);
                else if (arg == "--snappy")
                    config.UseSnappy = true;
                else if (arg == "--help" || arg == "-h")
                {
                    PrintUsage();
                    Environment.Exit(0);
                }
            }

            return config;
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"
Kinetica Load Generator - High-performance bulk insert benchmark tool

Usage: Kinetica.LoadGenerator [options]

Options:
  --url <url>              Kinetica server URL (default: http://localhost:9191)
  --user <username>        Username (default: admin)
  --password <password>    Password (default: secret)
  --table <name>           Target table name (default: load_generator_test)
  --records <count>        Total records to insert (default: 1,000,000)
  --batch-size <size>      Records per batch (default: 10,000)
  --threads <count>        Number of producer threads (default: 1)
  --workers <count>        Number of flush workers (default: 16)
  --message-size <bytes>   Payload size in bytes (default: 1024)
  --random-payload         Randomize payload for each record
  --better-schema          Use extended schema with thread tracking
  --no-multihead           Disable multi-head ingest
  --progress-interval <ms> Progress report interval in ms (default: 1000)
  --no-csv                 Disable CSV output
  --truncate               Truncate table before inserting
  --max-in-flight <count>  Max in-flight batches (default: 100)
  --snappy                 Enable Snappy compression for HTTP requests
  --help, -h               Show this help message

Environment Variables:
  KINETICA_URL, KINETICA_USER, KINETICA_PASSWORD, KINETICA_TABLE,
  TOTAL_RECORDS, KINETICA_BATCH_SIZE, NUM_THREADS, NUM_WORKERS,
  MESSAGE_SIZE, PAYLOAD_RANDOM, BETTER_SCHEMA, KINETICA_MULTIHEAD,
  PROGRESS_INTERVAL_MS, LOG_EVERY_N, CSV_OUTPUT, TRUNCATE_TABLE,
  MAX_IN_FLIGHT_BATCHES, SNAPPY

Command-line arguments take precedence over environment variables.
");
        }

        public void Print()
        {
            Console.WriteLine("=== Load Generator Configuration ===");
            Console.WriteLine($"  URL:              {Url}");
            Console.WriteLine($"  User:             {Username}");
            Console.WriteLine($"  Table:            {TableName}");
            Console.WriteLine($"  Total Records:    {TotalRecords:N0}");
            Console.WriteLine($"  Batch Size:       {BatchSize:N0}");
            Console.WriteLine($"  Threads:          {NumThreads}");
            Console.WriteLine($"  Workers:          {NumWorkers}");
            Console.WriteLine($"  Message Size:     {MessageSize} bytes");
            Console.WriteLine($"  Random Payload:   {PayloadRandom}");
            Console.WriteLine($"  Better Schema:    {BetterSchema}");
            Console.WriteLine($"  Multi-head:       {UseMultihead}");
            Console.WriteLine($"  Max In-Flight:    {MaxInFlightBatches}");
            Console.WriteLine($"  Snappy:           {UseSnappy}");
            Console.WriteLine($"  Progress Interval:{ProgressIntervalMs}ms");
            Console.WriteLine($"  CSV Output:       {CsvOutput}");
            Console.WriteLine($"  Truncate Table:   {TruncateTable}");
            Console.WriteLine("====================================\n");
        }

        private static string GetEnvString(string name, string defaultValue) =>
            Environment.GetEnvironmentVariable(name) ?? defaultValue;

        private static int GetEnvInt(string name, int defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        private static long GetEnvLong(string name, long defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return long.TryParse(value, out var result) ? result : defaultValue;
        }

        private static bool GetEnvBool(string name, bool defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("1", StringComparison.Ordinal) ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
