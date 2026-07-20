using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro.IO;
using Avro.Specific;
using Xunit;
using kinetica;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Unit-test port of the Java client's manual <c>HAFailover_DrainingQueue_test</c>.
    ///
    /// <para>
    /// The Java test is operator-driven (it waits for a human to stop/start a real primary cluster
    /// and watch its HA queues drain), so it cannot run verbatim in CI. Its central client-behavior
    /// assertion, however, is deterministic and unit-testable: <b>a full HA-aware client whose
    /// primary is draining must detect that during the connection sequence and fail over to a backup
    /// cluster -- without issuing any query</b> (Java <c>checkHaFailoverWhileDraining</c>). A draining
    /// cluster answers status endpoints and rejects only queries/DML, so the client cannot rely on
    /// "the request will fail"; it must read the drain state explicitly and route around it.
    /// </para>
    ///
    /// <para>
    /// These tests drive that logic through a fake <see cref="IHttpTransport"/> whose per-host state
    /// (healthy / draining / down) is scripted, so no live server or operator interaction is needed.
    /// They cover the three drain-aware behaviors added to the C# client:
    /// <list type="bullet">
    ///   <item>Discovery traverses a running-but-draining node (running is drain-agnostic), so the
    ///     HA ring is still discovered through a draining primary.</item>
    ///   <item>Connect-time failover off a down/draining initially-selected cluster.</item>
    ///   <item>The failover-selection predicate (<c>IsClusterUsable</c>) skips draining clusters,
    ///     both at connect and in the runtime <c>SwitchUrl</c> loop.</item>
    /// </list>
    /// </para>
    /// </summary>
    [Trait("Category", "Unit")]
    public class HAFailoverDrainingQueueTests
    {
        private const string PrimaryHost = "primary.example.com";
        private const string BackupHost = "backup.example.com";
        private const string PrimaryUrl = "http://primary.example.com:9191";
        private const string BackupUrl = "http://backup.example.com:9191";

        // conf.ha_ring_head_nodes_full: ';' separates clusters, ',' separates alternate addresses
        // for a single cluster. Two clusters: primary then backup.
        private const string HaRing = PrimaryUrl + ";" + BackupUrl;

        // ---- Phase 3 (connect-time failover): the Java test's core assertion ----------------------

        /// <summary>
        /// Primary listed first but draining, backup healthy. Merely constructing a full HA-aware
        /// client must detect the draining primary during connect and fail over to the backup -- the
        /// active URL afterward is the backup, not the draining primary. This also proves discovery
        /// traversed the draining primary (running is drain-agnostic) to learn the ring in the first
        /// place: the client is given only the primary URL and finds the backup via the HA ring.
        /// </summary>
        [Fact]
        public void Connect_PrimaryDraining_FailsOverToBackup()
        {
            var transport = new DrainAwareTransport
            {
                States = { [PrimaryHost] = HostState.Draining, [BackupHost] = HostState.Healthy }
            };
            var options = HaOptions();

            var client = new kinetica.Kinetica(new List<string> { PrimaryUrl }, transport, options);

            // Failed over during connect -- active cluster is the backup, not the draining primary.
            Assert.Equal(2, client.HARingSize);
            Assert.Equal(BackupHost, client.URL.Host);
        }

        /// <summary>
        /// Both clusters healthy: the client stays on the primary (index 0); no connect-time failover.
        /// </summary>
        [Fact]
        public void Connect_PrimaryHealthy_StaysOnPrimary()
        {
            var transport = new DrainAwareTransport
            {
                States = { [PrimaryHost] = HostState.Healthy, [BackupHost] = HostState.Healthy }
            };
            var options = HaOptions();

            var client = new kinetica.Kinetica(new List<string> { PrimaryUrl }, transport, options);

            Assert.Equal(2, client.HARingSize);
            Assert.Equal(PrimaryHost, client.URL.Host);
        }

        /// <summary>
        /// Primary down (both URLs supplied, list-constructor style as in the Java run). Discovery
        /// records the down primary minimally and the healthy backup fully; connect-time failover
        /// then moves off the down primary to the backup.
        /// </summary>
        [Fact]
        public void Connect_PrimaryDown_FailsOverToBackup()
        {
            var transport = new DrainAwareTransport
            {
                States = { [PrimaryHost] = HostState.Down, [BackupHost] = HostState.Healthy }
            };
            var options = HaOptions();

            var client = new kinetica.Kinetica(new List<string> { PrimaryUrl, BackupUrl }, transport, options);

            Assert.Equal(2, client.HARingSize);
            Assert.Equal(BackupHost, client.URL.Host);
        }

        // ---- Phase 1 (drain-aware status plumbing): the parsing primitives ------------------------

        /// <summary>
        /// <c>GetSystemRunningStatus</c> and <c>IsClusterUsable</c> classify a healthy, a draining, and
        /// a down cluster correctly. A draining cluster is still "running" (running is drain-agnostic),
        /// but it is NOT usable; a down cluster is neither.
        /// </summary>
        [Fact]
        public void StatusPredicates_ClassifyHealthyDrainingDown()
        {
            var transport = new DrainAwareTransport
            {
                States =
                {
                    [PrimaryHost] = HostState.Healthy,
                    [BackupHost] = HostState.Draining,
                    ["down.example.com"] = HostState.Down
                }
            };
            // Direct connection: no discovery/probing during construction, so we can call the
            // predicates against specific URLs ourselves.
            var options = new kinetica.Kinetica.Options
            {
                Username = "admin",
                Password = "secret",
                DisableAutoDiscovery = true,
                DisableFailover = true
            };
            var client = new kinetica.Kinetica(new List<string> { PrimaryUrl }, transport, options);

            var healthy = client.GetSystemRunningStatus(new Uri(PrimaryUrl), quickCheck: false);
            Assert.True(healthy.IsRunning);
            Assert.NotEqual(kinetica.Kinetica.HA_STATUS_DRAINING_VALUE, healthy.HaStatus);
            Assert.True(client.IsClusterUsable(new Uri(PrimaryUrl), quickCheck: false));

            var draining = client.GetSystemRunningStatus(new Uri(BackupUrl), quickCheck: false);
            Assert.True(draining.IsRunning); // draining is still "running"
            Assert.Equal(kinetica.Kinetica.HA_STATUS_DRAINING_VALUE, draining.HaStatus);
            Assert.False(client.IsClusterUsable(new Uri(BackupUrl), quickCheck: false)); // ...but not usable

            var down = client.GetSystemRunningStatus(new Uri("http://down.example.com:9191"), quickCheck: false);
            Assert.False(down.IsRunning);
            Assert.False(client.IsClusterUsable(new Uri("http://down.example.com:9191"), quickCheck: false));
        }

        // ---- Phase 2 (failover selection): SwitchUrl skips draining clusters ----------------------

        /// <summary>
        /// The runtime failover routine must skip a draining candidate and land on the next usable
        /// cluster -- a bare liveness ping (which a draining node passes) would wrongly accept it.
        /// Exercised directly on <see cref="HAFailoverManager"/> with a scripted usable-predicate so
        /// the acceptance test is isolated from the HTTP layer.
        /// </summary>
        [Fact]
        public void SwitchUrl_SkipsDrainingCandidate()
        {
            var a = new Uri("http://a.example.com:9191");
            var b = new Uri("http://b.example.com:9191");
            var c = new Uri("http://c.example.com:9191");

            // DisableAutoDiscovery => Initialize builds a minimal 3-cluster ring from the given URLs
            // with no network I/O; Sequential keeps the order [a, b, c].
            var mgr = new HAFailoverManager
            {
                DisableAutoDiscovery = true,
                DisableFailover = false,
                FailoverOrder = HAFailoverOrder.Sequential
            };
            mgr.Initialize(new List<Uri> { a, b, c });

            Assert.Equal(a, mgr.GetUrl()); // start on a

            // b is draining (unusable); c is usable. Failing over from a must skip b and land on c.
            var result = mgr.SwitchUrl(a, mgr.NumClusterSwitches, u => !u.Equals(b));

            Assert.Equal(c, result);
            Assert.Equal(c, mgr.GetUrl());
        }

        /// <summary>
        /// If every other cluster is unusable (e.g. all draining/down), <c>SwitchUrl</c> circles the
        /// ring and raises HA-unavailable rather than returning a bad cluster.
        /// </summary>
        [Fact]
        public void SwitchUrl_AllUnusable_Throws()
        {
            var a = new Uri("http://a.example.com:9191");
            var b = new Uri("http://b.example.com:9191");
            var c = new Uri("http://c.example.com:9191");

            var mgr = new HAFailoverManager
            {
                DisableAutoDiscovery = true,
                DisableFailover = false,
                FailoverOrder = HAFailoverOrder.Sequential
            };
            mgr.Initialize(new List<Uri> { a, b, c });

            // Nothing is usable -> the loop circles back to the original URL and throws.
            Assert.Throws<KineticaException>(
                () => mgr.SwitchUrl(a, mgr.NumClusterSwitches, _ => false));
        }

        // ----- helpers -----------------------------------------------------------------------------

        private static kinetica.Kinetica.Options HaOptions() => new()
        {
            Username = "admin",
            Password = "secret",
            // Full HA-aware defaults: failover + auto-discovery enabled. Sequential so backup order
            // is deterministic (matches the Java run's -DhaFailoverOrder=sequential).
            HAFailoverOrder = HAFailoverOrder.Sequential
        };

        private enum HostState { Healthy, Draining, Down }

        /// <summary>
        /// Fake <see cref="IHttpTransport"/> whose per-host state is scripted. Answers
        /// <c>/show/system/status</c> and <c>/show/system/properties</c>:
        /// <list type="bullet">
        ///   <item><b>Down</b>: throws (connection refused) for every request to that host.</item>
        ///   <item><b>Draining</b>: reports <c>system=running</c> and <c>ha_status.drained=draining</c>.</item>
        ///   <item><b>Healthy</b>: reports <c>system=running</c> and <c>ha_status.drained=drained</c>.</item>
        /// </list>
        /// Properties advertise a two-cluster HA ring (primary + backup) and no multi-head worker
        /// servers, so each cluster's head-node URL stays the given URL.
        /// </summary>
        private sealed class DrainAwareTransport : IHttpTransport
        {
            public Dictionary<string, HostState> States { get; } = new();

            private HostState StateOf(string host) =>
                States.TryGetValue(host, out var s) ? s : HostState.Healthy;

            public byte[] Post(string url, byte[] body, string contentType, string authorization, string userAgent, CancellationToken cancellationToken)
            {
                var uri = new Uri(url);
                var state = StateOf(uri.Host);

                if (state == HostState.Down)
                    throw new System.Net.Http.HttpRequestException($"simulated: cannot connect to {uri.Host}");

                if (url.Contains("/show/system/properties", StringComparison.OrdinalIgnoreCase))
                {
                    var props = new Dictionary<string, string>
                    {
                        ["conf.enable_ha"] = "TRUE",
                        ["conf.ha_ring_head_nodes_full"] = HaRing
                        // conf.enable_worker_http_servers omitted -> no multi-head; head node URL
                        // stays the given URL.
                    };
                    return WrapResponse(new ShowSystemPropertiesResponse { property_map = props });
                }

                // /show/system/status (and any other reachable POST): running, with a drain state.
                var status = new ShowSystemStatusResponse();
                status.status_map["system"] = "{\"status\":\"running\"}";
                status.status_map["ha_status"] = state == HostState.Draining
                    ? "{\"drained\":\"draining\"}"
                    : "{\"drained\":\"drained\"}";
                return WrapResponse(status);
            }

            public Task<byte[]> PostAsync(string url, byte[] body, string contentType, string authorization, string userAgent, CancellationToken cancellationToken)
                => Task.FromResult(Post(url, body, contentType, authorization, userAgent, cancellationToken));
        }

        private static byte[] EncodeSpecific(ISpecificRecord rec)
        {
            using var ms = new MemoryStream();
            var writer = new SpecificDefaultWriter(rec.Schema);
            writer.Write(rec.Schema, rec, new BinaryEncoder(ms));
            return ms.ToArray();
        }

        private static byte[] WrapResponse(ISpecificRecord inner)
        {
            var raw = new RawKineticaResponse
            {
                status = "OK",
                message = string.Empty,
                data_type = string.Empty,
                data = EncodeSpecific(inner),
                data_str = string.Empty
            };
            return EncodeSpecific(raw);
        }
    }
}
