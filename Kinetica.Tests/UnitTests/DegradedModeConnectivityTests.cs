using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro.IO;
using Avro.Specific;
using Microsoft.Extensions.Logging;
using Xunit;
using kinetica;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for the "degraded mode" connectivity check ported from the Java client.
    ///
    /// <para>
    /// Scenario: the client connects with a URL it can reach (e.g. an external/global-DNS
    /// address), <c>showSystemProperties()</c> succeeds and advertises the cluster's ranks via
    /// <c>conf.worker_http_server_urls</c>, but those server-known URLs are internal/LAN addresses
    /// the client cannot route to. The client can still talk to the head node via the user-given
    /// URL, but cannot reach any rank directly — so multi-head operations must be disabled. The API
    /// detects this during <c>HAFailoverManager.Initialize</c> by probing the server-known head node
    /// URL with <see cref="kinetica.Kinetica.IsSystemRunning(Uri)"/> and, on failure, throwing so the
    /// connection is reprocessed with auto-discovery disabled.
    /// </para>
    ///
    /// <para>
    /// These use the <c>internal</c> transport-injecting constructor (reachable via
    /// <c>InternalsVisibleTo</c>) so the server's responses are fully controlled by a fake transport.
    /// Note that constructor calls <c>HAFailoverManager.Initialize</c> directly (it does NOT go
    /// through <c>InitializeWithRetry</c>), so the connectivity failure surfaces as the thrown
    /// <see cref="KineticaException"/> — the reprocess-and-fall-back-to-degraded-mode behavior is
    /// owned by the public constructor's retry loop and is covered separately by integration tests.
    /// </para>
    /// </summary>
    [Trait("Category", "Unit")]
    public class DegradedModeConnectivityTests
    {
        private const string ExternalUrl = "http://external.example.com:9191";
        private const string InternalHeadHost = "10.0.0.5";
        private const string InternalHeadUrl = "http://10.0.0.5:9191";
        private const string InternalWorkerUrl = "http://10.0.0.6:9191";

        /// <summary>
        /// Server advertises a single internal rank URL (which becomes the head node URL) with
        /// multi-head enabled, but that internal URL is unreachable from the client. Construction must
        /// throw <see cref="KineticaException"/> (the trigger to reprocess without auto-discovery),
        /// and a degraded-mode <c>Warning</c> must be logged.
        /// </summary>
        [Fact]
        public void Initialize_ServerUrlUnreachable_ThrowsAndWarns()
        {
            var transport = new FakeHttpTransport
            {
                PropertyMap = MultiHeadProps(InternalHeadUrl),
                UnreachableHosts = { InternalHeadHost } // probe to the internal head fails
            };
            var capture = new CapturingLoggerFactory();
            var options = new kinetica.Kinetica.Options
            {
                Username = "admin",
                Password = "secret",
                LoggerFactory = capture
            };

            var ex = Assert.Throws<KineticaException>(
                () => new kinetica.Kinetica(new List<string> { ExternalUrl }, transport, options));

            // The exception names both URLs so an operator can see the external/internal split.
            Assert.Contains(InternalHeadHost, ex.Message);

            // The probe to the server-known head node URL actually happened.
            Assert.Contains(transport.PostedUrls, u => new Uri(u).Host == InternalHeadHost);

            // A degraded-mode warning was emitted on the HAFailover category.
            Assert.Contains(capture.Entries, e =>
                e.Level == LogLevel.Warning &&
                e.Message.Contains("multi-head", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Same discovery, but the server-known head node URL IS reachable. Construction must succeed,
        /// the active head node URL must be the server-advertised internal URL, and the remaining rank
        /// must be exposed as a worker URL (multi-head intact). No degraded-mode warning.
        /// </summary>
        [Fact]
        public void Initialize_ServerUrlReachable_KeepsMultiHead()
        {
            var transport = new FakeHttpTransport
            {
                // conf.worker_http_server_urls uses ';' between ranks ( ',' would be alternate
                // addresses for a single rank). Two ranks: rankUrls[0] becomes the head node, the
                // rest are workers.
                PropertyMap = MultiHeadProps(InternalHeadUrl + ";" + InternalWorkerUrl)
                // No unreachable hosts: every probe succeeds.
            };
            var options = new kinetica.Kinetica.Options { Username = "admin", Password = "secret" };

            var client = new kinetica.Kinetica(new List<string> { ExternalUrl }, transport, options);

            var clusterInfo = client.GetCurrentClusterInfo();
            Assert.NotNull(clusterInfo);
            Assert.Equal(InternalHeadHost, clusterInfo.ActiveHeadNodeUrl.Host);

            var workerUrls = client.GetCurrentWorkerUrls();
            Assert.NotNull(workerUrls);
            Assert.Contains(workerUrls, u => u.Host == "10.0.0.6");
        }

        /// <summary>
        /// Direct coverage of the probe primitive: <see cref="kinetica.Kinetica.IsSystemRunning(Uri)"/>
        /// returns true when the target responds and false when the transport errors.
        /// </summary>
        [Fact]
        public void IsSystemRunning_ReflectsReachability()
        {
            var transport = new FakeHttpTransport
            {
                UnreachableHosts = { InternalHeadHost }
            };
            // Auto-discovery off so construction performs no network I/O and no probing.
            var options = new kinetica.Kinetica.Options
            {
                Username = "admin",
                Password = "secret",
                DisableAutoDiscovery = true,
                DisableFailover = true
            };
            var client = new kinetica.Kinetica(new List<string> { ExternalUrl }, transport, options);

            Assert.False(client.IsSystemRunning(new Uri(InternalHeadUrl))); // host errors -> not running
            Assert.True(client.IsSystemRunning(new Uri(InternalWorkerUrl))); // responds -> running
        }

        // ----- helpers -------------------------------------------------------------------------

        private static Dictionary<string, string> MultiHeadProps(string serverUrls) => new()
        {
            ["conf.enable_worker_http_servers"] = "TRUE",
            ["conf.worker_http_server_urls"] = serverUrls
            // conf.enable_ha omitted -> no HA ring URLs are discovered/enqueued.
        };

        /// <summary>
        /// Avro-encodes an <see cref="ISpecificRecord"/> exactly as <c>Kinetica.AvroEncode</c> does,
        /// then wraps it in the <see cref="RawKineticaResponse"/> envelope the transport returns.
        /// </summary>
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

        /// <summary>
        /// Fake <see cref="IHttpTransport"/>: returns canned <c>show/system/properties</c> responses for
        /// the user-given URL, throws for hosts marked unreachable (to simulate an unroutable internal
        /// rank URL), and returns an empty <c>show/system/status</c> response otherwise.
        /// </summary>
        private sealed class FakeHttpTransport : IHttpTransport
        {
            public IDictionary<string, string> PropertyMap { get; set; } = new Dictionary<string, string>();
            public HashSet<string> UnreachableHosts { get; } = new();
            public List<string> PostedUrls { get; } = new();

            public byte[] Post(string url, byte[] body, string contentType, string? authorization, string? userAgent, CancellationToken cancellationToken)
            {
                PostedUrls.Add(url);
                var uri = new Uri(url);

                if (UnreachableHosts.Contains(uri.Host))
                    throw new System.Net.Http.HttpRequestException($"simulated: cannot connect to {uri.Host}");

                if (url.Contains("/show/system/properties", StringComparison.OrdinalIgnoreCase))
                    return WrapResponse(new ShowSystemPropertiesResponse { property_map = PropertyMap });

                // Any other reachable POST (e.g. the IsSystemRunning /show/system/status probe).
                return WrapResponse(new ShowSystemStatusResponse());
            }

            public Task<byte[]> PostAsync(string url, byte[] body, string contentType, string? authorization, string? userAgent, CancellationToken cancellationToken)
                => Task.FromResult(Post(url, body, contentType, authorization, userAgent, cancellationToken));
        }

        /// <summary>Minimal <see cref="ILoggerFactory"/> that records every log entry for assertions.</summary>
        private sealed class CapturingLoggerFactory : ILoggerFactory
        {
            public readonly List<(string Category, LogLevel Level, string Message)> Entries = new();
            public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, Entries);
            public void AddProvider(ILoggerProvider provider) { }
            public void Dispose() { }

            private sealed class CapturingLogger : ILogger
            {
                private readonly string _category;
                private readonly List<(string, LogLevel, string)> _sink;
                public CapturingLogger(string category, List<(string, LogLevel, string)> sink) { _category = category; _sink = sink; }
                public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
                public bool IsEnabled(LogLevel logLevel) => true;
                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                    => _sink.Add((_category, logLevel, formatter(state, exception)));

                private sealed class NullScope : IDisposable
                {
                    public static readonly NullScope Instance = new();
                    public void Dispose() { }
                }
            }
        }
    }
}
