using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using kinetica;

namespace Kinetica.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for User-Agent HTTP header functionality.
    /// These tests mirror the Python test_gpudb_user_agent.py implementation.
    ///
    /// <para>
    /// These tests do NOT require a database connection - they use the internal
    /// transport-injecting constructor to fully control HTTP behavior.
    /// </para>
    /// </summary>
    [Trait("Category", "Unit")]
    public class UserAgentTests
    {
        // Regex pattern for User-Agent format validation.
        // Matches: kinetica-api-cs/<version> (<runtime>; <OS>/<release>; <arch>)
        // Environment-specific values (runtime version, OS, release, arch) are flexible.
        private const string BaseUserAgentPattern =
            @"kinetica-api-cs/[\d.]+ " +
            @"\([^;]+; " +
            @"[^;]+/[^;]+; " +
            @"[^)]+\)";

        // Regex pattern for User-Agent with client app info prefix.
        // Matches: <client_name>/<client_version> kinetica-api-cs/...
        private static string UserAgentWithClientPattern(string clientName, string clientVersion) =>
            $@"^{Regex.Escape(clientName)}/{Regex.Escape(clientVersion)} {BaseUserAgentPattern}$";

        // Regex pattern for User-Agent without client app info.
        private const string UserAgentWithoutClientPattern = @"^" + BaseUserAgentPattern + @"$";

        #region test_user_agent_format() equivalent tests

        /// <summary>
        /// Test Case: Validate general structure of base USER_AGENT.
        /// Mirrors Python: test_user_agent_format() - first test case.
        /// </summary>
        [Fact]
        public void TestUserAgentFormat_ValidateGeneralStructure()
        {
            var transport = new CapturingHttpTransport();
            var options = new kinetica.Kinetica.Options
            {
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            var client = new kinetica.Kinetica(
                new List<string> { "http://127.0.0.1:59999" },
                transport,
                options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail - no real server */ }

            Assert.NotNull(transport.CapturedUserAgent);

            // Simple pattern to verify the header starts with expected product/version
            // and contains runtime info - proves the header is what we expect
            var generalPattern = @"^kinetica-api-cs/[.0-9]+ \(.+\)$";
            Assert.Matches(generalPattern, transport.CapturedUserAgent);
        }

        /// <summary>
        /// Test Case: Validate detailed base USER_AGENT format (without client info).
        /// Mirrors Python: test_user_agent_format() - second test case.
        /// </summary>
        [Fact]
        public void TestUserAgentFormat_ValidateDetailedBaseFormat()
        {
            var transport = new CapturingHttpTransport();
            var options = new kinetica.Kinetica.Options
            {
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            var client = new kinetica.Kinetica(
                new List<string> { "http://127.0.0.1:59999" },
                transport,
                options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail - no real server */ }

            Assert.NotNull(transport.CapturedUserAgent);
            Assert.Matches(UserAgentWithoutClientPattern, transport.CapturedUserAgent);
        }

        /// <summary>
        /// Test Case: Validate User-Agent format with client info.
        /// Mirrors Python: test_user_agent_format() - third test case.
        /// </summary>
        [Fact]
        public void TestUserAgentFormat_ValidateFormatWithClientInfo()
        {
            var transport = new CapturingHttpTransport();
            var clientName = "my-test-app";
            var clientVersion = "2.5.0";

            var options = new kinetica.Kinetica.Options
            {
                ClientName = clientName,
                ClientVersion = clientVersion,
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            var client = new kinetica.Kinetica(
                new List<string> { "http://127.0.0.1:59999" },
                transport,
                options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail - no real server */ }

            Assert.NotNull(transport.CapturedUserAgent);

            var pattern = UserAgentWithClientPattern(clientName, clientVersion);
            Assert.Matches(pattern, transport.CapturedUserAgent);
        }

        /// <summary>
        /// Test Case: Validate format with various client name/version styles.
        /// Mirrors Python: test_user_agent_format() - fourth test case.
        /// </summary>
        [Theory]
        [InlineData("simple-app", "1.0.0")]
        [InlineData("my_app", "0.0.1")]
        [InlineData("App123", "10.20.30")]
        [InlineData("app", "1.0.0-beta")]
        [InlineData("my-long-application-name", "1.0.0-rc1")]
        public void TestUserAgentFormat_VariousClientStyles(string clientName, string clientVersion)
        {
            var transport = new CapturingHttpTransport();
            var options = new kinetica.Kinetica.Options
            {
                ClientName = clientName,
                ClientVersion = clientVersion,
                DisableAutoDiscovery = true,
                DisableFailover = true
            };

            var client = new kinetica.Kinetica(
                new List<string> { "http://127.0.0.1:59999" },
                transport,
                options);

            // Make a request to capture the User-Agent
            try { client.showSystemStatus(); } catch { /* Expected to fail - no real server */ }

            Assert.NotNull(transport.CapturedUserAgent);

            var pattern = UserAgentWithClientPattern(clientName, clientVersion);
            Assert.Matches(pattern, transport.CapturedUserAgent);
        }

        #endregion

        #region test_gpudb_options_client_properties() equivalent tests

        /// <summary>
        /// Test Case: Default values for client_name and client_version are null.
        /// Mirrors Python: test_gpudb_options_client_properties() - first test case.
        /// </summary>
        [Fact]
        public void TestOptionsClientProperties_DefaultValuesAreNull()
        {
            var options = new kinetica.Kinetica.Options();

            Assert.Null(options.ClientName);
            Assert.Null(options.ClientVersion);
        }

        /// <summary>
        /// Test Case: Setting client_name and client_version with valid values.
        /// Mirrors Python: test_gpudb_options_client_properties() - third test case.
        /// </summary>
        [Fact]
        public void TestOptionsClientProperties_SetClientNameAndVersion()
        {
            var options = new kinetica.Kinetica.Options();
            options.ClientName = "my-app";
            options.ClientVersion = "1.0.0";

            Assert.Equal("my-app", options.ClientName);
            Assert.Equal("1.0.0", options.ClientVersion);
        }

        /// <summary>
        /// Test Case: Setting only client_name.
        /// Mirrors Python: test_gpudb_options_client_properties() - fourth test case.
        /// </summary>
        [Fact]
        public void TestOptionsClientProperties_SetOnlyClientName()
        {
            var options = new kinetica.Kinetica.Options();
            options.ClientName = "another-app";

            Assert.Equal("another-app", options.ClientName);
            Assert.Null(options.ClientVersion);
        }

        /// <summary>
        /// Test Case: Setting only client_version.
        /// Mirrors Python: test_gpudb_options_client_properties() - fifth test case.
        /// </summary>
        [Fact]
        public void TestOptionsClientProperties_SetOnlyClientVersion()
        {
            var options = new kinetica.Kinetica.Options();
            options.ClientVersion = "2.0.0";

            Assert.Null(options.ClientName);
            Assert.Equal("2.0.0", options.ClientVersion);
        }

        /// <summary>
        /// Test Case: Setting values to null explicitly.
        /// Mirrors Python: test_gpudb_options_client_properties() - sixth test case.
        /// </summary>
        [Fact]
        public void TestOptionsClientProperties_SetValuesToNullExplicitly()
        {
            var options = new kinetica.Kinetica.Options();
            options.ClientName = "test-app";
            options.ClientVersion = "1.0.0";
            options.ClientName = null;
            options.ClientVersion = null;

            Assert.Null(options.ClientName);
            Assert.Null(options.ClientVersion);
        }

        // Note: Python tests for invalid types (int, float) are not applicable in C#
        // because the properties are strongly typed as string? and the compiler
        // prevents assigning incompatible types.

        // Note: Python tests for dict constructor and copy constructor are not directly
        // applicable as C# Options class doesn't have those constructors.

        #endregion

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
