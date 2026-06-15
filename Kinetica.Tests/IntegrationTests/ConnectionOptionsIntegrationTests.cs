using System.Collections.Generic;
using Xunit;
using Kinetica.Tests.Common;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the HA-related connection <see cref="kinetica.Kinetica.Options"/> —
    /// the four permutations of <c>DisableAutoDiscovery</c> and <c>DisableFailover</c> — exercised
    /// against a live Kinetica server (configured via <c>KINETICA_URL</c> / <c>KINETICA_USER</c> /
    /// <c>KINETICA_PASSWORD</c>, see <see cref="TestContext.GetConnectionConfig"/>).
    ///
    /// <para>
    /// On a single-node server <c>HARingSize</c> is 1 regardless of discovery, so the discriminating,
    /// client-accessible value is <c>GetClusterInfo().SystemProperties</c>: enabling auto-discovery
    /// makes the client call <c>showSystemProperties()</c> and store the result on the cluster, while
    /// disabling it leaves that map empty (minimal, URL-only cluster info). Both flags also round-trip
    /// onto <c>HAManager</c>, and the client must remain usable under every permutation.
    /// </para>
    /// </summary>
    [Trait("Category", "Integration")]
    public class ConnectionOptionsIntegrationTests
    {
        private static kinetica.Kinetica CreateClient(bool disableAutoDiscovery, bool disableFailover)
        {
            var (url, username, password) = TestContext.GetConnectionConfig();

            var options = new kinetica.Kinetica.Options
            {
                Username = username,
                Password = password,
                UseSnappy = false,
                DisableAutoDiscovery = disableAutoDiscovery,
                DisableFailover = disableFailover
            };

            return new kinetica.Kinetica(new List<string> { url }, options);
        }

        /// <summary>
        /// Every permutation must round-trip its flags onto the HA manager, track at least the
        /// user-given cluster, and leave the client able to talk to the server.
        /// </summary>
        [Theory]
        [InlineData(false, false)] // discovery on,  failover on   (defaults)
        [InlineData(false, true)]  // discovery on,  failover off
        [InlineData(true, false)]  // discovery off, failover on
        [InlineData(true, true)]   // discovery off, failover off
        public void Options_HaFlags_RoundTrip_AndClientIsUsable(bool disableAutoDiscovery, bool disableFailover)
        {
            var client = CreateClient(disableAutoDiscovery, disableFailover);

            Assert.NotNull(client.HAManager);
            Assert.Equal(disableAutoDiscovery, client.HAManager.DisableAutoDiscovery);
            Assert.Equal(disableFailover, client.HAManager.DisableFailover);

            // At minimum the user-given cluster is tracked.
            Assert.True(client.HARingSize >= 1, "HA ring should contain at least the given URL");

            // Regardless of the HA options, a freshly constructed client can reach the server.
            var props = client.showSystemProperties().property_map;
            Assert.NotNull(props);
            Assert.NotEmpty(props);
        }

        /// <summary>
        /// With auto-discovery enabled, the client queries the server during construction and stores
        /// the system properties on the active cluster — observable via the public HA manager.
        /// </summary>
        [Fact]
        public void AutoDiscoveryEnabled_PopulatesClusterSystemProperties()
        {
            var client = CreateClient(disableAutoDiscovery: false, disableFailover: false);

            var info = client.HAManager.GetClusterInfo();
            Assert.NotNull(info);
            Assert.NotEmpty(info.SystemProperties);
        }

        /// <summary>
        /// With auto-discovery disabled, construction performs no system-property query, so the active
        /// cluster carries only minimal, URL-derived info (empty system properties).
        /// </summary>
        [Fact]
        public void AutoDiscoveryDisabled_LeavesClusterSystemPropertiesEmpty()
        {
            var client = CreateClient(disableAutoDiscovery: true, disableFailover: false);

            var info = client.HAManager.GetClusterInfo();
            Assert.NotNull(info);
            Assert.Empty(info.SystemProperties);
        }

        /// <summary>
        /// The <see cref="TestContext"/> options overload feeds custom HA options through to the live
        /// connection: the manager reflects the flags and the isolated schema is still created
        /// successfully, proving the connection works under those options.
        /// </summary>
        [Fact]
        public void TestContext_WithCustomOptions_ConnectsAndAppliesFlags()
        {
            var options = new kinetica.Kinetica.Options
            {
                UseSnappy = false,
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            using var ctx = new TestContext("conn_options", options);

            Assert.True(ctx.Kinetica.HAManager.DisableAutoDiscovery);
            Assert.True(ctx.Kinetica.HAManager.DisableFailover);

            // The schema was created during construction, so a round-trip call must succeed.
            Assert.True(ctx.Kinetica.hasSchema(ctx.SchemaName, null).schema_exists,
                        "Test schema should exist after construction");
        }
    }
}
