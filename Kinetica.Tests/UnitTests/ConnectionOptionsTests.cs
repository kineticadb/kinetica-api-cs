using System;
using System.Collections.Generic;
using Xunit;
using kinetica;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Unit tests covering the connection <see cref="kinetica.Kinetica.Options"/> that govern
    /// high-availability behavior — specifically the four permutations of
    /// <c>DisableAutoDiscovery</c> and <c>DisableFailover</c>.
    ///
    /// <para>
    /// These use the public <see cref="kinetica.Kinetica"/> constructor (no injected transport).
    /// To stay deterministic and avoid depending on a live server, they point at a closed loopback
    /// port with a short <c>Timeout</c>. For the auto-discovery-enabled permutations the constructor
    /// does attempt <c>showSystemProperties()</c>, but the connection is refused immediately;
    /// <c>HAFailoverManager.Initialize</c> handles that by falling back to minimal, per-URL cluster
    /// info, so construction always succeeds quickly with a one-cluster ring.
    /// </para>
    ///
    /// <para>
    /// The values asserted are the ones reachable from a constructed client:
    /// <c>client.HAManager.DisableFailover</c>, <c>client.HAManager.DisableAutoDiscovery</c>,
    /// <c>client.HARingSize</c>, and <c>client.NumClusterSwitches</c>. Because the discovery failure
    /// is absorbed inside <c>Initialize</c> (rather than thrown), the public constructor's retry path
    /// never flips <c>DisableAutoDiscovery</c>, so both flags round-trip exactly as supplied.
    /// </para>
    /// </summary>
    [Trait("Category", "Unit")]
    public class ConnectionOptionsTests
    {
        // A loopback port that is essentially always closed, so the auto-discovery POST is refused
        // immediately instead of hanging. The short Timeout is a backstop for environments that drop
        // (rather than reject) packets on a closed port.
        private const string UnreachableUrl = "http://127.0.0.1:59999";

        private static kinetica.Kinetica CreateClient(bool disableAutoDiscovery, bool disableFailover)
        {
            var options = new kinetica.Kinetica.Options
            {
                Username = "admin",
                Password = "secret",
                UseSnappy = false,
                Timeout = 2000, // ms; bound the worst case if the port filters instead of refuses
                DisableAutoDiscovery = disableAutoDiscovery,
                DisableFailover = disableFailover
            };

            return new kinetica.Kinetica(new List<string> { UnreachableUrl }, options);
        }

        /// <summary>
        /// All four permutations: the HA manager must reflect exactly the flags supplied via Options,
        /// and the ring must contain the single user-given URL (an unreachable server means no
        /// discovery added clusters).
        /// </summary>
        [Theory]
        [InlineData(false, false)] // discovery on,  failover on   (defaults)
        [InlineData(false, true)]  // discovery on,  failover off
        [InlineData(true, false)]  // discovery off, failover on
        [InlineData(true, true)]   // discovery off, failover off  (no network I/O at all)
        public void Options_HaFlags_AreReflectedOnHaManager(bool disableAutoDiscovery, bool disableFailover)
        {
            var client = CreateClient(disableAutoDiscovery, disableFailover);

            Assert.NotNull(client.HAManager);
            Assert.Equal(disableAutoDiscovery, client.HAManager.DisableAutoDiscovery);
            Assert.Equal(disableFailover, client.HAManager.DisableFailover);

            // With an unreachable server the ring can only ever contain the single user-given URL,
            // regardless of whether auto-discovery was attempted.
            Assert.Equal(1, client.HARingSize);

            // A freshly constructed client has not switched clusters.
            Assert.Equal(0, client.NumClusterSwitches);
        }

        /// <summary>
        /// When both flags are <c>true</c> the manager is documented to skip all network I/O during
        /// construction. Verify the manager still initializes the single given URL as its active URL.
        /// </summary>
        [Fact]
        public void Options_BothDisabled_SkipsDiscovery_AndUsesGivenUrl()
        {
            var client = CreateClient(disableAutoDiscovery: true, disableFailover: true);

            Assert.NotNull(client.HAManager);
            Assert.True(client.HAManager.DisableAutoDiscovery);
            Assert.True(client.HAManager.DisableFailover);

            var activeUrl = client.HAManager.GetUrl();
            Assert.NotNull(activeUrl);
            Assert.Equal("127.0.0.1", activeUrl.Host);
            Assert.Equal(59999, activeUrl.Port);

            // GetHostAddresses() is part of the public client surface and should expose exactly the
            // one cluster built from the supplied URL.
            Assert.Single(client.HAManager.GetHostAddresses());
        }
    }
}
