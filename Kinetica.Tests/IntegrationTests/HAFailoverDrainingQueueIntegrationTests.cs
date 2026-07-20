using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using kinetica;
using Newtonsoft.Json.Linq;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// HA Failover + Failback (Draining Queue) Integration Test
    ///
    /// <para>
    /// C# port of the Java client's manual <c>HAFailover_DrainingQueue_test</c>.
    /// </para>
    ///
    /// <para>
    /// <b>Call Type:</b>  DDL via /execute/sql (CREATE OR REPLACE TABLE)<br/>
    /// <b>Threaded:</b>   Single-Threaded (each request has a sequence number)<br/>
    /// <b>Run Length:</b> Configurable (NUM_REQUESTS environment variable)<br/>
    /// <b>Failover:</b>   Triggered Externally (operator shuts the primary down)<br/>
    /// <b>Failback:</b>   Monitored (waits for the primary's HA queues to drain)
    /// </para>
    ///
    /// <para>
    /// This test drives the full failover-then-failback cycle to exercise the HA draining queue:
    /// <list type="number">
    ///   <item>Waits for the operator to shut the PRIMARY cluster down (auto-detected
    ///     by polling the primary URL until it stops responding).</item>
    ///   <item>Sends a sequence of /execute/sql requests, each issuing
    ///     <c>CREATE OR REPLACE TABLE ha_draining_queue_test (i&lt;iter&gt; INT)</c>
    ///     where &lt;iter&gt; is the sequence number of the request. With the primary
    ///     down, the first request fails over to the next cluster in the ring;
    ///     those DDL operations are queued on the secondary for HA replication
    ///     back to the primary.</item>
    ///   <item>Waits for the operator to start the PRIMARY back up, then monitors two
    ///     things until failback completes:
    ///     <list type="bullet">
    ///       <item><b>Drained status:</b> polls the primary's showSystemStatus "ha_status"
    ///         until its HA queues report "drained" (caught up / fully online).</item>
    ///       <item><b>Connectivity:</b> whenever the drained status reads "draining", merely
    ///         opening a full HA-aware connection should detect the draining
    ///         primary during its connection sequence and fail over to a backup
    ///         cluster -- without issuing any query.</item>
    ///     </list>
    ///   </item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Because it needs the operator to stop/start a cluster, it is meant to be run
    /// manually as a standalone console application.
    /// </para>
    ///
    /// <para>
    /// <b>Example usage:</b>
    /// <code>
    /// KINETICA_URL="http://primary:9191;http://secondary:9191" \
    /// KINETICA_PRIMARY_URL="http://primary:9191" \
    /// KINETICA_USER="admin" \
    /// KINETICA_PASSWORD="secret" \
    /// NUM_REQUESTS=1000 \
    /// POLL_SECONDS=5 \
    /// dotnet run --project Kinetica.Tests/IntegrationTests/HAFailoverDrainingQueueIntegrationTests.cs
    /// </code>
    /// </para>
    /// </summary>
    public class HAFailoverDrainingQueueIntegrationTests
    {
        // Name of table to be used in the test
        private const string TableName = "ha_draining_queue_test";

        // Keys used to read the HA drain state out of showSystemStatus()
        private const string HA_STATUS_KEY = "ha_status";
        private const string HA_STATUS_DRAINED_KEY = "drained";
        private const string HA_STATUS_VALUE_DRAINING = "draining";

        private static readonly string LineSeparator = new string('=', 80);

        // Configuration from environment variables
        private readonly int _numRequests;
        private readonly long _pollIntervalMs;
        private readonly string _urls;
        private readonly string _primaryUrl;
        private readonly string _username;
        private readonly string _password;

        public static int Main(string[] args)
        {
            Console.WriteLine("HA Failover Draining Queue Integration Test");
            Console.WriteLine(new string('=', 50));

            try
            {
                var test = new HAFailoverDrainingQueueIntegrationTests();
                test.Run();
                Console.WriteLine();
                Console.WriteLine("TEST PASSED");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"TEST FAILED: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        public HAFailoverDrainingQueueIntegrationTests()
        {
            // Read configuration from environment variables (mirrors Java's -D properties)
            _numRequests = int.Parse(Environment.GetEnvironmentVariable("NUM_REQUESTS") ?? "1000");
            _pollIntervalMs = long.Parse(Environment.GetEnvironmentVariable("POLL_SECONDS") ?? "5") * 1000L;

            // URL list: semicolon-separated list of cluster URLs (primary first)
            // e.g., "http://primary:9191;http://secondary:9191"
            _urls = Environment.GetEnvironmentVariable("KINETICA_URL")
                ?? throw new InvalidOperationException(
                    "KINETICA_URL environment variable must be set to a semicolon-separated list of cluster URLs");

            // Primary URL for monitoring drain status
            _primaryUrl = Environment.GetEnvironmentVariable("KINETICA_PRIMARY_URL")
                ?? throw new InvalidOperationException(
                    "KINETICA_PRIMARY_URL environment variable must be set to the primary cluster URL");

            _username = Environment.GetEnvironmentVariable("KINETICA_USER") ?? "admin";
            _password = Environment.GetEnvironmentVariable("KINETICA_PASSWORD") ?? "secret";
        }

        public void Run()
        {
            // Parse URLs into a list
            var urlList = new List<string>(_urls.Split(';', StringSplitOptions.RemoveEmptyEntries));
            var primaryUri = new Uri(_primaryUrl);

            // Build the main HA-aware client (primary first, sequential failover)
            // Set a reasonable connection timeout so we don't hang if the primary is already down
            var options = new kinetica.Kinetica.Options
            {
                Username = _username,
                Password = _password,
                HAFailoverOrder = HAFailoverOrder.Sequential,
                InitialConnectionAttemptTimeout = 30000  // 30 seconds max for initial connection
            };

            Console.WriteLine("Creating Kinetica client...");
            var gpudb = new kinetica.Kinetica(urlList, options);
            Console.WriteLine($"Client created, active URL: {gpudb.URL}");

            Console.WriteLine($"Primary cluster URL: {primaryUri}");
            Console.WriteLine($"HA ring URLs: {string.Join(", ", urlList)}");

            // Clear table if it exists (start from a clean slate)
            Console.WriteLine($"Clearing table <{TableName}> if it exists");
            ClearTable(gpudb, TableName);

            // ---- Step 1: wait for the operator to shut the primary down --------
            Console.WriteLine(LineSeparator);
            Console.WriteLine($"ACTION REQUIRED: shut DOWN the PRIMARY cluster now ({primaryUri})");
            Console.WriteLine("Waiting for the primary to become unreachable...");

            while (gpudb.IsKineticaRunning(primaryUri))
            {
                Thread.Sleep((int)_pollIntervalMs);
            }

            Console.WriteLine("Primary is DOWN. The next DB request will fail over to a backup cluster.");

            // ---- Step 2: send a sequence of /execute/sql DDL requests ----------
            Console.WriteLine($"Sending {_numRequests} /execute/sql requests (CREATE OR REPLACE TABLE {TableName} (i<iter> INT))...");

            long errorCount = 0;
            var stopwatch = Stopwatch.StartNew();

            for (int iter = 0; iter < _numRequests; iter++)
            {
                string sql = $"CREATE OR REPLACE TABLE {TableName} (i{iter} INT)";
                try
                {
                    gpudb.executeSql(sql, 0, -9999, null, null, new Dictionary<string, string>());
                }
                catch (KineticaException ex)
                {
                    errorCount++;
                    if (ex.Message.Contains("connection") || ex.Message.Contains("Connection"))
                    {
                        throw new Exception($"Test stopped due to HA ring being killed: {ex.Message}", ex);
                    }
                    else
                    {
                        Console.WriteLine($"Request <{iter}> failed: {ex.Message}");
                    }
                }

                if (iter == 0)
                {
                    Console.WriteLine($"Active cluster after failover: {gpudb.URL}");
                }
            }

            stopwatch.Stop();
            double elapsedSec = stopwatch.Elapsed.TotalSeconds;
            double reqPerSec = elapsedSec > 0 ? _numRequests / elapsedSec : 0.0;

            Console.WriteLine($"Finished sending {_numRequests} requests, errors={errorCount} in {elapsedSec:F1}s ({reqPerSec:F0} req/s)");
            Console.WriteLine($"DDL applied on active (secondary) cluster {gpudb.URL}; operations are queued for the primary.");

            // ---- Step 3: wait for the operator to restart the primary,
            //              then monitor drain status until fully online ----------
            Console.WriteLine(LineSeparator);
            Console.WriteLine($"ACTION REQUIRED: START the PRIMARY cluster back up now ({primaryUri})");
            Console.WriteLine("The secondary will replay its HA queue into the primary. Monitoring drain status...");

            MonitorDrainedStatus(primaryUri, urlList);

            Console.WriteLine("Primary is back online and its HA queues are DRAINED.");
            Console.WriteLine($"Test complete for table <{TableName}>");
        }

        /// <summary>
        /// Polls the primary directly (failover/auto-discovery disabled) and logs its
        /// HA drain state until the primary is reachable AND its queues are drained.
        /// Whenever the state reads "draining", it also runs a connectivity check
        /// verifying that a full HA-aware connection fails over to a backup cluster.
        /// </summary>
        private void MonitorDrainedStatus(Uri primaryUrl, List<string> allUrls)
        {
            // A dedicated, failover-free connection so we only ever talk to the
            // primary URL and never accidentally hit the still-live secondary.
            var probeOptions = new kinetica.Kinetica.Options
            {
                Username = _username,
                Password = _password,
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            while (true)
            {
                string state = ProbeDrainState(primaryUrl, probeOptions);

                switch (state)
                {
                    case "OFFLINE":
                        Console.WriteLine("Primary not reachable yet -- waiting for startup...");
                        break;

                    case "DRAINING":
                        Console.WriteLine("Primary is UP but HA queues are still DRAINING...");
                        // Connectivity check: while the primary drains, a full
                        // HA-aware connection should fail over to a backup cluster.
                        CheckHaFailoverWhileDraining(primaryUrl, allUrls);
                        break;

                    case "DRAINED":
                        Console.WriteLine("Primary is UP and HA queues are DRAINED -- fully online.");
                        return;

                    default:
                        Console.WriteLine($"Primary reachable; HA status = '{state}' (treating as drained/online).");
                        return;
                }

                Thread.Sleep((int)_pollIntervalMs);
            }
        }

        /// <summary>
        /// Connectivity check run while the primary reports "draining": a full, HA-aware
        /// connection (failover enabled, primary first) should detect the draining primary
        /// during its initial connection sequence and fail over to a backup cluster --
        /// without issuing any query.
        /// </summary>
        private void CheckHaFailoverWhileDraining(Uri primaryUrl, List<string> allUrls)
        {
            // Full HA-aware options: failover + auto-discovery left at their
            // enabled defaults; primary listed first so the client would start
            // there absent the draining-triggered failover.
            var haOptions = new kinetica.Kinetica.Options
            {
                Username = _username,
                Password = _password,
                PrimaryUrl = primaryUrl.ToString(),
                HAFailoverOrder = HAFailoverOrder.Sequential
            };

            try
            {
                // Constructing the client runs the initial connection sequence,
                // which detects the primary's draining status and fails over.
                var haClient = new kinetica.Kinetica(allUrls, haOptions);

                var active = haClient.URL;
                if (active.Host.Equals(primaryUrl.Host, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Connectivity: HA-aware client connected to the primary <{active}> despite it " +
                        "draining -- expected the connection to fail over to a backup");
                }
                else
                {
                    Console.WriteLine($"Connectivity: HA-aware connection detected the draining primary and " +
                        $"failed over to backup <{active}> during connect");
                }
            }
            catch (KineticaException e)
            {
                Console.WriteLine($"Connectivity: HA-aware connection attempt failed while primary draining: {e.Message}");
            }
        }

        /// <summary>
        /// Attempts a single probe of the primary's HA drain state.
        /// </summary>
        /// <returns>
        /// One of: "OFFLINE" (cannot connect), "DRAINING" (queues syncing),
        /// "DRAINED" (caught up), or the raw status value if it is neither.
        /// </returns>
        private string ProbeDrainState(Uri primaryUrl, kinetica.Kinetica.Options probeOptions)
        {
            try
            {
                var primary = new kinetica.Kinetica(primaryUrl.ToString(), probeOptions);

                var response = primary.showSystemStatus(new Dictionary<string, string>());
                var statusMap = response.status_map;

                if (statusMap == null || !statusMap.TryGetValue(HA_STATUS_KEY, out var haStatusJson))
                {
                    // No HA status reported (e.g. HA not configured) => nothing to drain.
                    return "DRAINED";
                }

                var haStatusObj = JObject.Parse(haStatusJson);
                var drainedValue = haStatusObj[HA_STATUS_DRAINED_KEY]?.ToString();

                if (string.Equals(drainedValue, HA_STATUS_VALUE_DRAINING, StringComparison.Ordinal))
                {
                    return "DRAINING";
                }

                return string.IsNullOrEmpty(drainedValue) ? "DRAINED" : drainedValue.ToUpperInvariant();
            }
            catch (Exception)
            {
                // Connection refused, still starting, or draining-queue error =>
                // treat the primary as not-yet-ready.
                return "OFFLINE";
            }
        }

        /// <summary>
        /// Clears a table if it exists (best effort).
        /// </summary>
        private void ClearTable(kinetica.Kinetica gpudb, string tableName)
        {
            try
            {
                gpudb.clearTable(tableName, null, new Dictionary<string, string>());
            }
            catch
            {
                // Ignore errors (table may not exist)
            }
        }
    }
}
