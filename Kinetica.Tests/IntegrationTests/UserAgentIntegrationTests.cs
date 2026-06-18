using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Kinetica.Tests.Common;
using kinetica;

namespace Kinetica.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for User-Agent HTTP header functionality with a live Kinetica connection.
    /// These tests mirror the Python test_gpudb_user_agent(db) function.
    ///
    /// <para>
    /// These tests require a live Kinetica database connection. The connection
    /// is configured via environment variables:
    /// <list type="bullet">
    ///   <item>KINETICA_URL - Server URL (default: http://127.0.0.1:9191)</item>
    ///   <item>KINETICA_USER - Username (default: admin)</item>
    ///   <item>KINETICA_PASSWORD - Password (default: secret)</item>
    /// </list>
    /// </para>
    /// </summary>
    [Trait("Category", "Integration")]
    public class UserAgentIntegrationTests
    {
        /// <summary>
        /// Test Case: Verify User-Agent string with client_name and client_version.
        /// Mirrors Python: test_gpudb_user_agent(db) - first test case.
        /// </summary>
        [Fact]
        public void TestUserAgent_WithClientNameAndVersion()
        {
            // Use a capturing transport to verify the User-Agent header
            var transport = new CapturingHttpTransport();
            var (url, username, password) = TestContext.GetConnectionConfig();

            var options = new kinetica.Kinetica.Options
            {
                ClientName = "my-app",
                ClientVersion = "1.0.0",
                DisableAutoDiscovery = true,
                Username = username,
                Password = password
            };

            var client = new kinetica.Kinetica(url, transport, options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail with fake transport */ }

            var actualUserAgent = transport.CapturedUserAgent;
            var expectedPrefix = "my-app/1.0.0";

            Assert.NotNull(actualUserAgent);
            Assert.StartsWith(expectedPrefix, actualUserAgent);
            Assert.Contains("kinetica-api-cs/", actualUserAgent);
        }

        /// <summary>
        /// Test Case: Verify User-Agent string without client_name and client_version.
        /// Mirrors Python: test_gpudb_user_agent(db) - second test case.
        /// </summary>
        [Fact]
        public void TestUserAgent_WithoutClientInfo()
        {
            // Use a capturing transport to verify the User-Agent header
            var transport = new CapturingHttpTransport();
            var (url, username, password) = TestContext.GetConnectionConfig();

            var options = new kinetica.Kinetica.Options
            {
                DisableAutoDiscovery = true,
                Username = username,
                Password = password
            };

            var client = new kinetica.Kinetica(url, transport, options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail with fake transport */ }

            var actualUserAgent = transport.CapturedUserAgent;

            Assert.NotNull(actualUserAgent);
            // Should start with kinetica-api-cs (no client prefix)
            Assert.StartsWith("kinetica-api-cs/", actualUserAgent);
            // Should NOT contain any client prefix
            Assert.DoesNotContain("my-app", actualUserAgent);
        }

        /// <summary>
        /// Test Case: Verify sanitization of special characters in client_name/version.
        /// Mirrors Python: test_gpudb_user_agent(db) - third test case.
        /// </summary>
        [Fact]
        public void TestUserAgent_SanitizationOfSpecialCharacters()
        {
            // Use a capturing transport to verify the User-Agent header
            var transport = new CapturingHttpTransport();
            var (url, username, password) = TestContext.GetConnectionConfig();

            var options = new kinetica.Kinetica.Options
            {
                ClientName = "my app/special",
                ClientVersion = "1.0.0-beta+build",
                DisableAutoDiscovery = true,
                Username = username,
                Password = password
            };

            var client = new kinetica.Kinetica(url, transport, options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail with fake transport */ }

            var actualUserAgent = transport.CapturedUserAgent;

            Assert.NotNull(actualUserAgent);

            // Special characters should be replaced with underscores
            Assert.Contains("my_app_special", actualUserAgent);
            Assert.Contains("1.0.0-beta_build", actualUserAgent);
        }

        #region Test Helpers

        /// <summary>
        /// HTTP transport that captures the User-Agent header from requests.
        /// Always throws to simulate unreachable server (we only care about the headers).
        /// </summary>
        private sealed class CapturingHttpTransport : IHttpTransport
        {
            public string? CapturedUserAgent { get; private set; }

            public byte[] Post(string url, byte[] body, string contentType, string? authorization, string? userAgent, CancellationToken cancellationToken)
            {
                CapturedUserAgent = userAgent;

                // Throw to simulate connection failure - we only care about capturing the User-Agent
                throw new System.Net.Http.HttpRequestException("Simulated connection failure for User-Agent test");
            }

            public Task<byte[]> PostAsync(string url, byte[] body, string contentType, string? authorization, string? userAgent, CancellationToken cancellationToken)
            {
                return Task.FromResult(Post(url, body, contentType, authorization, userAgent, cancellationToken));
            }
        }

        #endregion
    }
}
