using Avro.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Snappier;
using System.Net;
using System.Text;
using kinetica.Utils;

//! \mainpage Introduction
//!
//! This is the client-side C# application programming interface (API) for Kinetica.
//!
//! <br/>
//!
//! The source code can be found <a href="https://github.com/kineticadb/kinetica-api-cs" target="_top">here</a>.
//!
//! <br/>
//!
//! There are two projects here: Kinetica and Example.
//! <br/>
//!
//! The Kinetica project contains the main client source code in the kinetica namespace.
//! The <see cref="Kinetica"/> class implements the interface for the API.  The Protocol
//! subdirectory contains classes for each endpoint of the database server.
//!
//! <br/>
//!
//! The Example project contains a short example <see cref="Example.Example">here</see>.  The user
//! needs to specify the hostname of a database server (e.g. "127.0.0.1:9191") in the property
//! to properly run it.
//!

namespace kinetica;

/// <summary>
/// API to talk to Kinetica Database
/// </summary>
public partial class Kinetica : IDisposable
{
    private bool _disposed = false;
    /// <summary>
    /// No Limit
    /// </summary>
    public const int END_OF_SET = -9999;

    /// <summary>
    /// Connection Options
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Optional: User Name for Kinetica security
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Password for user
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Optional: OauthToken for user
        /// </summary>
        public string OauthToken { get; set; } = string.Empty;

        /// <summary>
        /// Use Snappy compression for requests
        /// </summary>
        public bool UseSnappy { get; set; } = false;

        /// <summary>
        /// Thread Count
        /// </summary>
        public int ThreadCount { get; set; } = 1;

        /// <summary>
        /// Whether to disable failing over to a secondary cluster when a
        /// failure event is encountered on the current cluster that is
        /// appropriate for retrying on another cluster.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Failover operates between the cluster URLs known to the client &#8212;
        /// those the user specified plus any discovered via auto-discovery
        /// (see <see cref="DisableAutoDiscovery"/>). This flag controls whether
        /// cluster rotation happens on failure; it doesn't prevent the initial
        /// polling of the server for other known HA clusters.
        /// </para>
        /// <para>
        /// When only a single URL is available, this flag has no observable effect.
        /// When multiple URLs are available (user-specified, discovered, or both),
        /// setting this to <c>true</c> short-circuits cluster rotation: failed
        /// requests are retried against the same cluster but won't be rerouted to peers.
        /// </para>
        /// </remarks>
        public bool DisableFailover { get; set; } = false;

        /// <summary>
        /// Whether to disable automatic discovery of HA clusters and worker ranks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// "Auto-discovery" encompasses two independent server-side queries the client
        /// makes during construction: HA ring topology (peer cluster URLs) and worker
        /// rank URLs (used by multi-head operations like <see cref="BulkInserter{T}"/>
        /// and <see cref="RecordRetriever{T}"/>). When this flag is <c>true</c>, both
        /// queries are suppressed and multi-head operations become unavailable as a
        /// consequence.
        /// </para>
        /// <para>
        /// When set to <c>false</c> (default), the constructor will attempt to call
        /// <c>showSystemProperties()</c> to discover HA ring topology and worker rank URLs.
        /// This network call can block if the server is slow or unreachable.
        /// </para>
        /// <para>
        /// If auto-discovery fails and <see cref="InitialConnectionAttemptTimeout"/> is 0 (default),
        /// the constructor will automatically disable auto-discovery and retry once using only 
        /// the provided URLs (without discovering additional cluster information).
        /// </para>
        /// <para>
        /// If auto-discovery fails and <see cref="InitialConnectionAttemptTimeout"/> is greater than 0,
        /// the constructor will retry with exponential backoff until the timeout is exceeded.
        /// </para>
        /// <para>
        /// For single-node deployments or when constructor blocking is unacceptable, set this to <c>true</c>
        /// along with <see cref="DisableFailover"/> = <c>true</c> to skip all network I/O during construction.
        /// </para>
        /// </remarks>
        public bool DisableAutoDiscovery { get; set; } = false;

        /// <summary>
        /// Order in which to failover to backup clusters
        /// </summary>
        public HAFailoverOrder HAFailoverOrder { get; set; } = HAFailoverOrder.Random;

        /// <summary>
        /// Overall request timeout in milliseconds. <c>0</c> (the default) means infinite — no
        /// overall timeout, matching the Java API.
        /// </summary>
        /// <remarks>
        /// This is the end-to-end per-request cap. Connection establishment is bounded separately
        /// by <see cref="ServerConnectionTimeout"/>, so even with an infinite request timeout an
        /// unreachable host fails when the connection attempt times out rather than hanging
        /// forever; an infinite value only allows an already-connected, slow-responding server to
        /// block without limit.
        /// </remarks>
        public int Timeout { get; set; } = 0;

        /// <summary>
        /// Host manager port number
        /// </summary>
        public int HostManagerPort { get; set; } = HAFailoverManager.DefaultHostManagerPort;

        /// <summary>
        /// Optional: Regex pattern to filter URLs by hostname/IP
        /// </summary>
        public string? HostnameRegex { get; set; } = null;

        /// <summary>
        /// URL of the primary cluster in the HA environment
        /// </summary>
        public string PrimaryUrl { get; set; } = string.Empty;

        /// <summary>
        /// Initial connection attempt timeout in milliseconds.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Controls retry behavior when auto-discovery fails during initialization.
        /// Auto-discovery (calling <c>showSystemProperties()</c>) is always attempted first
        /// unless <see cref="DisableAutoDiscovery"/> is set to <c>true</c>.
        /// </para>
        /// <para>
        /// When set to 0 (default): If auto-discovery fails, the client will disable
        /// auto-discovery and retry once using only the provided URLs without further
        /// cluster discovery. No exponential backoff is used.
        /// </para>
        /// <para>
        /// When set to a positive value (e.g., 300000 for 5 minutes): If auto-discovery fails,
        /// the client will retry with exponential backoff (starting at 1 minute, doubling each time)
        /// until this timeout is exceeded. If multiple URLs are given, the API will try all of them
        /// once before sleeping and retrying. After the timeout is exceeded, the client falls back
        /// to initialization without auto-discovery.
        /// </para>
        /// </remarks>
        public int InitialConnectionAttemptTimeout { get; set; } = 0;

        /// <summary>
        /// Default value (milliseconds) for <see cref="ServerConnectionTimeout"/>. Also used
        /// directly as the timeout for the <see cref="Kinetica.IsKineticaRunning(Uri)"/> liveness
        /// ping.
        /// </summary>
        public const int DefaultServerConnectionTimeout = 5000;

        /// <summary>
        /// Maximum time, in milliseconds, to establish a TCP connection to the server.
        /// Default: <see cref="DefaultServerConnectionTimeout"/> (5 seconds); 0 leaves connection
        /// establishment unbounded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Maps to <see cref="System.Net.Http.SocketsHttpHandler.ConnectTimeout"/>. It bounds the
        /// connection handshake specifically — most visibly when a host is unreachable/black-holed
        /// during initial discovery — and is independent of <see cref="Timeout"/>, which caps the
        /// overall request. For an unreachable host the client gives up after whichever of the two
        /// elapses first; since <see cref="Timeout"/> defaults to infinite, this is the effective
        /// bound on how long a connection attempt to an unreachable host blocks by default.
        /// </para>
        /// <para>
        /// This is distinct from <see cref="InitialConnectionAttemptTimeout"/>, which is the
        /// overall budget across discovery retries rather than a per-connection bound.
        /// </para>
        /// </remarks>
        public int ServerConnectionTimeout { get; set; } = DefaultServerConnectionTimeout;

        /// <summary>
        /// Maximum lifetime of pooled HTTP connections. Lower values improve DNS refresh
        /// frequency but increase connection churn. Default: 2 minutes.
        /// </summary>
        public TimeSpan PooledConnectionLifetime { get; set; } = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Idle timeout for pooled HTTP connections. Default: 2 minutes.
        /// </summary>
        public TimeSpan PooledConnectionIdleTimeout { get; set; } = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Optional logger factory used by the client to report diagnostics.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When supplied, the client creates per-component category loggers (e.g.
        /// <c>"Kinetica"</c>, <c>"Kinetica.HAFailover"</c>, <c>"Kinetica.BulkInserter"</c>)
        /// and emits messages through the standard <see cref="ILogger"/> abstraction. This is
        /// the channel through which non-fatal conditions &#8212; such as auto-discovery
        /// failing and the connection falling back to a degraded, single-head mode &#8212;
        /// are surfaced without throwing.
        /// </para>
        /// <para>
        /// When left <c>null</c> (default), logging is a no-op (<see cref="NullLoggerFactory"/>);
        /// no output is produced and behavior is unchanged. Pass an <see cref="ILoggerFactory"/>
        /// (e.g. obtained from dependency injection) to route messages into the host's logging
        /// pipeline.
        /// </para>
        /// </remarks>
        public ILoggerFactory? LoggerFactory { get; set; } = null;

        /// <summary>
        /// Optional client application name to include in the User-Agent HTTP header.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, this value is included in the User-Agent header sent with all HTTP
        /// requests to the Kinetica server. This helps identify the client application
        /// making requests for logging and debugging purposes.
        /// </para>
        /// <para>
        /// The User-Agent header format is:
        /// <c>Kinetica C# API/{version} ({runtime}){client_name}/{client_version}</c>
        /// where the client_name/client_version suffix is only included if these
        /// properties are set.
        /// </para>
        /// </remarks>
        public string? ClientName { get; set; } = null;

        /// <summary>
        /// Optional client application version to include in the User-Agent HTTP header.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set along with <see cref="ClientName"/>, this value is included in the
        /// User-Agent header sent with all HTTP requests to the Kinetica server.
        /// </para>
        /// <para>
        /// This property is only meaningful when <see cref="ClientName"/> is also set.
        /// If <see cref="ClientName"/> is null or empty, this property is ignored.
        /// </para>
        /// </remarks>
        public string? ClientVersion { get; set; } = null;
    }

    /// <summary>
    /// API Version
    /// </summary>
    /// <returns>Version String for API</returns>
    public static string GetApiVersion() { return API_VERSION; }

    /// <summary>
    /// URL for Kinetica Server (including "http:" and port) as a string
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// URL for Kinetica Server (including "http:" and port)
    /// </summary>
    public Uri URL { get; private set; }

    /// <summary>
    /// Optional: User Name for Kinetica security
    /// </summary>
    public string? Username { get; private set; } = null;

    /// <summary>
    /// Optional: Password for user
    /// </summary>
    private string? Password { get; set; } = null;

    /// <summary>
    /// Optional: OauthToken for user
    /// </summary>
    private string? OauthToken { get; set; } = null;

    /// <summary>
    /// Optional: Authorization for connections.
    /// </summary>
    private string? Authorization { get; set; } = null;

    /// <summary>
    /// Use Snappy
    /// </summary>
    public bool UseSnappy { get; set; } = false;

    /// <summary>
    /// Thread Count
    /// </summary>
    public int ThreadCount { get; set; } = 1;

    /// <summary>
    /// HA Failover Manager for managing cluster failover
    /// </summary>
    private HAFailoverManager? _haFailoverManager = null;

    /// <summary>
    /// Gets the HA failover manager instance.
    /// </summary>
    public HAFailoverManager? HAManager => _haFailoverManager;

    /// <summary>
    /// Gets the number of times the client has switched to a different cluster.
    /// </summary>
    public int NumClusterSwitches => _haFailoverManager?.NumClusterSwitches ?? 0;

    /// <summary>
    /// Gets the list of all cluster addresses in the HA ring.
    /// </summary>
    public IList<ClusterAddressInfo> GetHARingInfo()
    {
        return _haFailoverManager?.GetHostAddresses() ?? new List<ClusterAddressInfo>();
    }

    /// <summary>
    /// Gets the current active cluster information.
    /// </summary>
    public ClusterAddressInfo? GetCurrentClusterInfo()
    {
        return _haFailoverManager?.GetClusterInfo();
    }

    // private string authorization;
    private volatile System.Collections.Concurrent.ConcurrentDictionary<string, KineticaType> knownTypes = new();

    // private type label to type ID lookup table
    private Dictionary<string, string> typeNameLookup = [];

    /// <summary>
    /// HTTP transport layer for making requests (thread-safe, reusable)
    /// </summary>
    private readonly IHttpTransport _transport;

    /// <summary>
    /// Logger factory for the client (defaults to a no-op factory when none is supplied).
    /// </summary>
    private readonly ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    /// <summary>
    /// Logger for the top-level "Kinetica" category.
    /// </summary>
    private readonly ILogger _logger = NullLogger.Instance;

    /// <summary>
    /// Gets the logger factory configured for this client. Components such as
    /// <see cref="BulkInserter{T}"/> and <see cref="RecordRetriever{T}"/> use this to create
    /// their own category loggers. Never <c>null</c> &#8212; a no-op factory is used when the
    /// caller supplies none.
    /// </summary>
    internal ILoggerFactory LoggerFactory => _loggerFactory;

    // private object class type to KineticaType lookup table
    private Dictionary<Type, KineticaType> kineticaTypeLookup = [];

    /// <summary>
    /// The User-Agent string sent with all HTTP requests.
    /// </summary>
    private readonly string _userAgent;

    /// <summary>
    /// Regex pattern for characters NOT allowed in User-Agent tokens.
    /// Matches any character that is not a word character, dot, or hyphen.
    /// </summary>
    private static readonly System.Text.RegularExpressions.Regex UserAgentDisallowedChars =
        new(@"[^\w.\-]", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// Sanitizes a User-Agent token by replacing disallowed characters with underscores.
    /// </summary>
    /// <remarks>
    /// Allowed characters are: word characters (a-zA-Z0-9_), dots (.), and hyphens (-).
    /// All other characters are replaced with underscores (_).
    /// </remarks>
    /// <param name="value">The value to sanitize</param>
    /// <returns>The sanitized value</returns>
    private static string SanitizeUserAgentToken(string value)
    {
        return UserAgentDisallowedChars.Replace(value, "_");
    }

    /// <summary>
    /// Builds the User-Agent string for HTTP requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The format is: <c>[{client_name}/{client_version} ]kinetica-api-cs/{version} ({runtime}; {os}/{os_version}; {arch})</c>
    /// where the client prefix is only included if both <see cref="Options.ClientName"/> and
    /// <see cref="Options.ClientVersion"/> are provided.
    /// </para>
    /// <para>
    /// Special characters in client_name and client_version are sanitized: any character
    /// that is not a word character (a-zA-Z0-9_), dot (.), or hyphen (-) is replaced with
    /// an underscore (_).
    /// </para>
    /// </remarks>
    /// <param name="options">Connection options (may be null)</param>
    /// <returns>The User-Agent string to use for HTTP requests</returns>
    private static string BuildUserAgentString(Options? options)
    {
        // Get runtime, OS, and architecture info (similar to Python's platform module)
        var runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        var osDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        var arch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();

        // Extract OS name and version from OSDescription
        // OSDescription examples: "Microsoft Windows 10.0.19045", "Linux 5.15.0-generic", "Darwin 21.6.0"
        var osName = GetOSName();
        var osVersion = GetOSVersion(osDescription);

        // Base User-Agent: "kinetica-api-cs/{version} ({runtime}; {os}/{os_version}; {arch})"
        var baseUserAgent = $"kinetica-api-cs/{API_VERSION} ({runtime}; {osName}/{osVersion}; {arch})";

        // Prepend client identification if BOTH client_name and client_version are provided
        if (options != null &&
            !string.IsNullOrEmpty(options.ClientName) &&
            !string.IsNullOrEmpty(options.ClientVersion))
        {
            var sanitizedName = SanitizeUserAgentToken(options.ClientName);
            var sanitizedVersion = SanitizeUserAgentToken(options.ClientVersion);
            return $"{sanitizedName}/{sanitizedVersion} {baseUserAgent}";
        }

        return baseUserAgent;
    }

    /// <summary>
    /// Gets the OS name (Windows, Linux, macOS, etc.)
    /// </summary>
    private static string GetOSName()
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
            return "Windows";
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Linux))
            return "Linux";
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.OSX))
            return "macOS";
        return "Unknown";
    }

    /// <summary>
    /// Extracts the OS version from the OS description string.
    /// </summary>
    private static string GetOSVersion(string osDescription)
    {
        // Try to extract version number from OS description
        // Examples: "Microsoft Windows 10.0.19045" -> "10.0.19045"
        //           "Linux 5.15.0-generic" -> "5.15.0-generic"
        //           "Darwin 21.6.0" -> "21.6.0"
        var match = System.Text.RegularExpressions.Regex.Match(
            osDescription, @"[\d]+\.[\d]+[\.\d\-\w]*");
        return match.Success ? match.Value : Environment.OSVersion.Version.ToString();
    }

    /// <summary>
    /// Internal constructor for testing that accepts a custom HTTP transport.
    /// </summary>
    /// <param name="url_str">URL for Kinetica Server</param>
    /// <param name="transport">Custom HTTP transport implementation</param>
    /// <param name="options">Optional connection options</param>
    internal Kinetica(string url_str, IHttpTransport transport, Options? options = null)
        : this(new List<string> { url_str }, transport, options)
    {
    }

    /// <summary>
    /// Internal constructor for testing with multiple URLs and custom HTTP transport.
    /// </summary>
    /// <param name="urls">List of URLs for Kinetica Servers</param>
    /// <param name="transport">Custom HTTP transport implementation</param>
    /// <param name="options">Optional connection options</param>
    internal Kinetica(IList<string> urls, IHttpTransport transport, Options? options = null)
    {
        if (urls == null || urls.Count == 0)
            throw new KineticaException("At least one URL must be provided");

        // Treat null options as defaults; behavior is identical to passing
        // `new Options()`. Auto-discovery is on by default, which is correct for
        // Kinetica deployments that don't require authentication.
        options ??= new Options();

        // Use the first URL as the primary
        Url = urls[0].TrimEnd('/');
        URL = new Uri(Url);

        // Use the provided transport (for testing)
        _transport = transport;

        // Build and store the User-Agent string
        _userAgent = BuildUserAgentString(options);

        // Set up logging (no-op factory when the caller supplies none)
        _loggerFactory = options.LoggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger("Kinetica");

        // Initialize other properties from options
        Username = options.Username;
        Password = options.Password;
        OauthToken = options.OauthToken;
        UseSnappy = options.UseSnappy;
        ThreadCount = options.ThreadCount;

        // Create authorization header
        Authorization = CreateAuthorizationHeader();

        // Initialize HA failover manager
        // (always construct when options are provided; gating on URL count
        // would skip discovery of additional HA clusters and worker ranks)
        _haFailoverManager = new HAFailoverManager
        {
            DisableFailover = options.DisableFailover,
            DisableAutoDiscovery = options.DisableAutoDiscovery,
            HostManagerPort = options.HostManagerPort,
            FailoverOrder = options.HAFailoverOrder,
            Logger = _loggerFactory.CreateLogger("Kinetica.HAFailover")
        };

        if (!string.IsNullOrEmpty(options.HostnameRegex))
        {
            _haFailoverManager.HostnameRegex = new System.Text.RegularExpressions.Regex(options.HostnameRegex);
        }

        var uriList = urls.Select(u => new Uri(u.TrimEnd('/'))).ToList();
        _haFailoverManager.Initialize(uriList, this);
    }

    /// <summary>
    /// API Constructor
    /// </summary>
    /// <param name="url_str">URL for Kinetica Server (including "http:" and port)</param>
    /// <param name="options">Optional connection options</param>
    public Kinetica( string url_str, Options? options = null )
        : this(new List<string> { url_str }, options)
    {
    }

    /// <summary>
    /// API Constructor with multiple URLs for HA failover support.
    /// </summary>
    /// <param name="urls">List of URLs for Kinetica Servers (including "http:" and port)</param>
    /// <param name="options">Optional connection options</param>
    public Kinetica( IList<string> urls, Options? options = null )
    {
        if (urls == null || urls.Count == 0)
            throw new KineticaException("At least one URL must be provided");

        // Treat null options as defaults so behavior is identical to passing
        // `new Options()`. Auto-discovery is on by default, which is correct for
        // Kinetica deployments that don't require authentication.
        options ??= new Options();

        // Use the first URL as the primary
        Url = urls[0].TrimEnd('/');
        URL = new Uri(Url);

        // Initialize HTTP transport layer.
        var timeout = options.Timeout > 0
            ? TimeSpan.FromMilliseconds(options.Timeout)
            : System.Threading.Timeout.InfiniteTimeSpan;

        // Bound connection establishment (e.g. for unreachable hosts during initial discovery)
        // by ServerConnectionTimeout; 0 leaves it unbounded (limited only by the request timeout).
        TimeSpan? connectTimeout = options.ServerConnectionTimeout > 0
            ? TimeSpan.FromMilliseconds(options.ServerConnectionTimeout)
            : null;

        _transport = new HttpClientTransport(
            timeout,
            options.PooledConnectionLifetime,
            options.PooledConnectionIdleTimeout,
            connectTimeout);

        // Build and store the User-Agent string
        _userAgent = BuildUserAgentString(options);

        // Set up logging (no-op factory when the caller supplies none)
        _loggerFactory = options.LoggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger("Kinetica");

        Username = options.Username;
        Password = options.Password;
        OauthToken = options.OauthToken;

        // Handle authorization
        Authorization = CreateAuthorizationHeader();

        UseSnappy = options.UseSnappy;
        ThreadCount = options.ThreadCount;

        // Convert string URLs to Uri objects
        var uriList = urls.Select(u => new Uri(u.TrimEnd('/'))).ToList();

        // Initialize HA failover manager
        // Match Java: DisableAutoDiscovery is set from options, may be changed during initialization
        _haFailoverManager = new HAFailoverManager
        {
            DisableFailover = options.DisableFailover,
            DisableAutoDiscovery = options.DisableAutoDiscovery,
            HostManagerPort = options.HostManagerPort,
            FailoverOrder = options.HAFailoverOrder,
            Logger = _loggerFactory.CreateLogger("Kinetica.HAFailover")
        };

        if (!string.IsNullOrEmpty(options.HostnameRegex))
        {
            _haFailoverManager.HostnameRegex = new System.Text.RegularExpressions.Regex(options.HostnameRegex);
        }

        // Match Java pattern: always attempt initialization, handle failures based on InitialConnectionAttemptTimeout
        InitializeWithRetry(uriList, options);

        // Update the URL to the current active cluster
        var currentUrl = _haFailoverManager.GetUrl();
        if (currentUrl != null)
        {
            Url = currentUrl.ToString().TrimEnd('/');
            URL = currentUrl;
        }
    }

    /// <summary>
    /// Initializes the HA failover manager following Java's processUrls() pattern.
    ///
    /// Java behavior:
    /// 1. Always attempt auto-discovery first (unless DisableAutoDiscovery is true)
    /// 2. If auto-discovery fails and InitialConnectionAttemptTimeout == 0 (default),
    ///    set DisableAutoDiscovery = true and retry once without auto-discovery
    /// 3. If InitialConnectionAttemptTimeout > 0, retry with exponential backoff
    /// </summary>
    /// <param name="uriList">List of URLs to initialize with</param>
    /// <param name="options">Connection options</param>
    private void InitializeWithRetry(IList<Uri> uriList, Options options)
    {
        if (_haFailoverManager == null)
            throw new InvalidOperationException("HAFailoverManager not initialized");

        // use nanoseconds internally for precision
        long initialConnectionAttemptTimeoutNs = options.InitialConnectionAttemptTimeout * 1_000_000L;
        long startTimeNs = DateTime.UtcNow.Ticks * 100; // Ticks are 100ns each
        int reattemptWaitIntervalMs = 60_000; // Java: starts at 1 minute

        bool keepTrying = true;
        while (keepTrying)
        {
            try
            {
                // Attempt to initialize (with or without auto-discovery based on current flag)
                _logger.LogDebug(
                    "[Kinetica] Attempting to parse URLs (DisableAutoDiscovery={DisableAutoDiscovery})",
                    _haFailoverManager.DisableAutoDiscovery);

                _haFailoverManager.Initialize(uriList, _haFailoverManager.DisableAutoDiscovery ? null : this);

                _logger.LogDebug("[Kinetica] Processed cluster URLs successfully");
                return; // Success
            }
            catch (Exception ex) when (ex.Message.Contains("hostname", StringComparison.OrdinalIgnoreCase) &&
                                       ex.Message.Contains("regex", StringComparison.OrdinalIgnoreCase))
            {
                // GPUdbHostnameRegexFailureException - no point retrying
                throw new KineticaException(
                    $"Could not connect to any working Kinetica server due to hostname regex mismatch: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex.Message.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
                                       ex.Message.Contains("401", StringComparison.OrdinalIgnoreCase))
            {
                // GPUdbUnauthorizedAccessException - cannot proceed
                _logger.LogError("[Kinetica] Got Unauthorized while communicating to server, cannot proceed");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Kinetica] Attempt at parsing URLs failed: {Message}", ex.Message);

                // if timeout is 0 (default), disable auto-discovery and retry once
                if (initialConnectionAttemptTimeoutNs == 0)
                {
                    if (!_haFailoverManager.DisableAutoDiscovery)
                    {
                        // First failure with timeout=0: disable auto-discovery and try again.
                        // Note: a failed showSystemProperties() call is absorbed inside
                        // HAFailoverManager.Initialize (minimal cluster info), so it does not reach
                        // here; the failure that does reach here is typically the post-discovery
                        // connectivity check (the server-advertised head node URL was unreachable)
                        // or an otherwise-unusable cluster. The specific cause is in the preceding
                        // warning.
                        _logger.LogWarning(
                            "[Kinetica] Initialization with auto-discovery enabled did not succeed " +
                            "and InitialConnectionAttemptTimeout " +
                            "is 0; retrying once without auto-discovery. The connection will then operate in " +
                            "degraded mode: it uses only the user-given URLs and skips HA ring/worker-rank " +
                            "discovery, so multi-head operations (BulkInserter, RecordRetriever) will fall " +
                            "back to single-head via the head node.");
                        _haFailoverManager.DisableAutoDiscovery = true;
                        // Continue loop - will retry with auto-discovery disabled
                    }
                    else
                    {
                        // Already tried without auto-discovery, give up
                        throw new KineticaException(
                            $"Could not connect to any working Kinetica server: {ex.Message}", ex);
                    }
                }
                else
                {
                    // Check if we should keep trying
                    long elapsedNs = (DateTime.UtcNow.Ticks * 100) - startTimeNs;
                    keepTrying = elapsedNs <= initialConnectionAttemptTimeoutNs;

                    _logger.LogDebug("[Kinetica] Keep trying to parse URLs?: {KeepTrying}", keepTrying);

                    if (keepTrying)
                    {
                        _logger.LogInformation(
                            "[Kinetica] Attempt at parsing URLs failed; waiting for {Minutes} minute(s) before retrying",
                            reattemptWaitIntervalMs / 60000);

                        Thread.Sleep(reattemptWaitIntervalMs);

                        // Double the wait interval for next time (Java pattern)
                        reattemptWaitIntervalMs *= 2;
                    }
                    else
                    {
                        // Timeout exceeded - fall back to initialization without auto-discovery
                        _logger.LogWarning(
                            "[Kinetica] InitialConnectionAttemptTimeout exceeded; falling back to initialization without " +
                            "auto-discovery. The connection will operate in degraded mode: multi-head operations " +
                            "(BulkInserter, RecordRetriever) will fall back to single-head via the head node.");
                        _haFailoverManager.DisableAutoDiscovery = true;
                        // One more try without auto-discovery
                    }
                }
            }
        }

        // Should not reach here, but just in case
        if (_haFailoverManager.HARingSize == 0)
        {
            throw new KineticaException("Could not connect to any working Kinetica server");
        }
    }

    /// <summary>
    /// Gets the current worker URLs for multi-head operations.
    /// Used by BulkInserter and RecordRetriever to get updated worker URLs after a failover.
    /// </summary>
    /// <returns>List of worker rank URLs, or null if multi-head is not enabled</returns>
    public IList<Uri>? GetCurrentWorkerUrls()
    {
        var clusterInfo = _haFailoverManager?.GetClusterInfo();
        if (clusterInfo?.WorkerRankUrls != null && clusterInfo.WorkerRankUrls.Count > 0)
        {
            return clusterInfo.WorkerRankUrls;
        }

        // Try to get fresh worker URLs from the server
        try
        {
            var workers = new WorkerList(this);
            if (workers.Count > 0)
            {
                return workers.GetActiveUrls().ToList();
            }
        }
        catch
        {
            // Fall back to cached URLs or null
        }

        return null;
    }

    /// <summary>
    /// Gets the current routing table for multi-head operations.
    /// Used by BulkInserter and RecordRetriever to get updated routing after a failover.
    /// </summary>
    /// <returns>The current routing table, or null if unavailable</returns>
    public IList<int>? GetCurrentRoutingTable()
    {
        try
        {
            return adminShowShards().rank;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Refreshes cluster information after a failover.
    /// Called by BulkInserter and RecordRetriever after a cluster switch.
    /// </summary>
    /// <returns>True if the refresh was successful, false otherwise</returns>
    public bool RefreshClusterInfo()
    {
        if (_haFailoverManager == null)
            return false;

        try
        {
            // Get fresh system properties to update cluster info
            var clusterInfo = _haFailoverManager.GetClusterInfo();
            if (clusterInfo == null)
                return false;

            // Update system properties from the server
            var systemProps = showSystemProperties().property_map;
            clusterInfo.SystemProperties = systemProps;

            // Update worker URLs if multi-head is enabled
            var workers = new WorkerList(this);
            if (workers.Count > 0)
            {
                clusterInfo.WorkerRankUrls = workers.GetActiveUrls().ToList();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    internal string? CreateAuthorizationHeader() {
        string? authorization = null;
        // Handle authorization
        if( OauthToken != null && OauthToken.Length > 0 ) {
            authorization = "Bearer " + OauthToken;
        }
        else if ( ( Username != null && ( Username.Length > 0 ) ) || ( Password != null && ( Password.Length > 0 ) ) )
        {
            authorization = ( "Basic " +
                                Convert.ToBase64String( Encoding.GetEncoding( "ISO-8859-1" ).GetBytes( Username + ":" + Password ) ) );
        }

        return authorization;
    }

    /// <summary>
    /// Disposes the Kinetica client and releases HTTP resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_transport is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Given a table name, add its record type to enable proper encoding of records
    /// for insertion or updates.
    /// </summary>
    /// <param name="table_name">Name of the table.</param>
    /// <param name="obj_type">The type associated with the table.</param>
    public void AddTableType( string table_name, Type obj_type )
    {
        try
        {
            // Get the type from the table
            KineticaType ktype = KineticaType.fromTable( this, table_name );
            if ( ktype.getTypeID() == null )
                throw new KineticaException( $"Could not get type ID for table '{table_name}'" );
            this.knownTypes.TryAdd( ktype.getTypeID(), ktype );

            // Save a mapping of the object to the KineticaType
            if ( obj_type != null )
                this.SetKineticaSourceClassToTypeMapping( obj_type, ktype );

        } catch ( KineticaException ex )
        {
            throw new KineticaException( "Error creating type from table", ex );
        }
    }  // end AddTableType

    /// <summary>
    /// Saves an object class type to a KineticaType association.  If the class type already exists
    /// in the map, replaces the old KineticaType value.
    /// </summary>
    /// <param name="objectType">The type of the object.</param>
    /// <param name="kineticaType">The associated KinetiaType object.</param>
    public void SetKineticaSourceClassToTypeMapping( Type? objectType, KineticaType kineticaType )
    {
        if ( objectType != null )
            this.kineticaTypeLookup[objectType] = kineticaType;
        return;
    }  // end SetKineticaSourceClassToTypeMapping



    /// <summary>
    /// Given a KineticaType object for a certain record type, decode binary data into distinct
    /// records (objects).
    /// </summary>
    /// <typeparam name="T">The type of the records.</typeparam>
    /// <param name="record_type">The type for the records.</param>
    /// <param name="records_binary">The binary encoded data to be decoded.</param>
    /// <param name="records">The decoded objects/records.</param>
    public void DecodeRawBinaryDataUsingRecordType<T>( KineticaType record_type,
                                                       IList<byte[]> records_binary,
                                                       IList<T> records ) where T : new()
    {
        // Using the KineticaType object, decode all the records from avro binary encoding
        foreach ( var bin_record in records_binary )
        {
            T obj = AvroDecode<T>( bin_record, record_type );
            records.Add( obj );
        }
    }  // DecodeRawBinaryDataUsingRecordType


    /// <summary>
    /// Given a schema string for a certain record type, decode binary data into distinct
    /// records (objects).
    /// </summary>
    /// <typeparam name="T">The type of the records.</typeparam>
    /// <param name="schema_string">The schema for the records.</param>
    /// <param name="records_binary">The binary encoded data to be decoded.</param>
    /// <param name="records">The decoded objects/records.</param>
    public void DecodeRawBinaryDataUsingSchemaString<T>( string schema_string,
                                                         IList<byte[]> records_binary,
                                                         IList<T> records ) where T : new()
    {
        // Create a KineticaType object based on the schema string
        KineticaType ktype = new("", schema_string, null);

        // Using the KineticaType object, decode all the records from avro binary encoding
        foreach ( var bin_record in records_binary )
        {
            T obj = AvroDecode<T>( bin_record, ktype );
            records.Add( obj );
        }
    }  // DecodeRawBinaryDataUsingSchemaString

    /// <summary>
    /// Given a list of schema strings, decode binary data into distinct
    /// records (objects).
    /// </summary>
    /// <typeparam name="T">The type of the records.</typeparam>
    /// <param name="schema_strings">The schemas for the records.</param>
    /// <param name="lists_records_binary">The binary encoded data to be decoded (the data is
    /// in a 2D list).</param>
    /// <param name="record_lists">The decoded objects/records in a 2d list.</param>
    public void DecodeRawBinaryDataUsingSchemaString<T>( IList<string> schema_strings,
                                                         IList<IList<byte[]>> lists_records_binary,
                                                         IList<IList<T>> record_lists ) where T : new()
    {
        // Check that the list of schemas and list of binary encode data match in length
        if ( schema_strings.Count != lists_records_binary.Count )
            throw new KineticaException( "List of schemas and list of binary encoded data do not match in count." );

        // Using the KineticaType object, decode all the records from avro binary encoding
        for ( int i = 0; i < schema_strings.Count; ++i )
        {
            // Create a KineticaType object based on the schema string
            KineticaType ktype = new( "", schema_strings[ i ], null );

            // Get the binary encoded data for this list
            IList<byte[]> records_binary = lists_records_binary[ i ];

            // Create a container to put the decoded records
            IList<T> records = [];

            // The inner list actually contains the binary data
            foreach ( var bin_record in records_binary )
            {
                T obj = AvroDecode<T>( bin_record, ktype );
                records.Add( obj );
            }
            // Add the records into the outgoing list
            record_lists.Add( records );
        }
    }  // DecodeRawBinaryDataUsingSchemaString


    /// <summary>
    /// Given IDs of records types registered with Kinetica, decode binary
    /// data into distinct records (objects).
    /// </summary>
    /// <typeparam name="T">The type of the records.</typeparam>
    /// <param name="type_ids">The IDs for each of the records' types.</param>
    /// <param name="records_binary">The binary encoded data to be decoded.</param>
    /// <param name="records">The decoded objects/records.</param>
    public void DecodeRawBinaryDataUsingTypeIDs<T>( IList<string> type_ids,
                                                    IList<byte[]> records_binary,
                                                    IList<T> records ) where T : new()
    {
        // Make sure that the length of the type IDs and records are the same
        if ( type_ids.Count != records_binary.Count )
            throw new KineticaException( "Unequal numbers of type IDs and binary encoded data objects provided." );

        // Decode all the records
        for ( int i = 0; i < records_binary.Count; ++i )
        {
            // Per object, use the respective type ID to create the appropriate KineticaType
            KineticaType ktype = KineticaType.fromTypeID( this, type_ids[ i ] );

            // Using the KineticaType object, decode the record.
            T obj = AvroDecode<T>( records_binary[ i ], ktype );
            records.Add( obj );
        }
    }  // DecodeRawBinaryDataUsingTypeIDs


    /// <summary>
    /// Given IDs of records types registered with Kinetica, decode binary
    /// data into distinct records (objects).
    /// </summary>
    /// <typeparam name="T">The type of the records.</typeparam>
    /// <param name="type_ids">The IDs for each of the lists of records.</param>
    /// <param name="lists_records_binary">The binary encoded data to be decoded in a 2d list.</param>
    /// <param name="record_lists">The decoded objects/records in a 2d list.</param>
    public void DecodeRawBinaryDataUsingTypeIDs<T>( IList<string> type_ids,
                                                    IList<IList<byte[]>> lists_records_binary,
                                                    IList<IList<T>> record_lists ) where T : new()
    {
        // Make sure that the length of the type IDs and records are the same
        if ( type_ids.Count != lists_records_binary.Count )
            throw new KineticaException( "Unequal numbers of type IDs and binary encoded data objects provided." );

        // Decode all the records
        for ( int i = 0; i < lists_records_binary.Count; ++i )
        {
            // Per object, use the respective type ID to create the appropriate KineticaType
            KineticaType ktype = KineticaType.fromTypeID( this, type_ids[ i ] );

            // Get the binary encoded data for this list
            IList<byte[]> records_binary = lists_records_binary[ i ];

            // Create a container to put the decoded records
            IList<T> records = [];

            // The inner list actually contains the binary data
            foreach ( var bin_record in records_binary )
            {
                // Using the KineticaType object, decode the record.
                T obj = AvroDecode<T>( bin_record, ktype );
                records.Add( obj );
            }
            // Add the records into the outgoing list
            record_lists.Add( records );
        }
    }  // DecodeRawBinaryDataUsingTypeIDs

    #region Request Submission API

    /// <summary>
    /// Submit a request to a Kinetica endpoint with HA failover support.
    /// This is the primary method for all API calls.
    ///
    /// Matches Rust gpudb.rs submit_request() - public with HA failover.
    /// </summary>
    /// <typeparam name="TResponse">Kinetica Response Object Type</typeparam>
    /// <param name="endpoint">Kinetica Endpoint to call (e.g., "/show/table")</param>
    /// <param name="request">Kinetica Request Object</param>
    /// <param name="enableCompression">Use Compression</param>
    /// <param name="avroEncoding">Use Avro Encoding</param>
    /// <returns>Response Object</returns>
    private TResponse SubmitRequest<TResponse>(string endpoint, object request, bool enableCompression = false, bool avroEncoding = true) where TResponse : new()
    {
        // Encode the request
        byte[] requestBytes = avroEncoding
            ? AvroEncode(request)
            : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

        // If HA failover is not available, just submit the request directly
        if (_haFailoverManager == null || _haFailoverManager.HARingSize <= 1)
        {
            string fullUrl = Url + endpoint;
            RawKineticaResponse kineticaResponse = SubmitRequestToUrlInternal(fullUrl, requestBytes, enableCompression, avroEncoding);
            return DecodeResponse<TResponse>(kineticaResponse, avroEncoding);
        }

        // HA failover is available - attempt with failover logic
        var currentUrl = _haFailoverManager.GetUrl();
        if (currentUrl == null)
        {
            throw new KineticaException("No URL available");
        }

        var originalUrl = currentUrl;
        int currentSwitchCount = _haFailoverManager.NumClusterSwitches;

        while (true)
        {
            try
            {
                // Build the full URL with the endpoint
                string fullUrl = currentUrl.ToString().TrimEnd('/') + endpoint;
                RawKineticaResponse kineticaResponse = SubmitRequestToUrlInternal(fullUrl, requestBytes, enableCompression, avroEncoding);
                return DecodeResponse<TResponse>(kineticaResponse, avroEncoding);
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                // This is a connection error - attempt failover
                try
                {
                    currentUrl = _haFailoverManager.SwitchUrl(originalUrl, currentSwitchCount, IsKineticaRunning);
                    // Update the main URL reference
                    Url = currentUrl.ToString().TrimEnd('/');
                    URL = currentUrl;
                }
                catch (KineticaException)
                {
                    // Failover failed - re-throw the original exception
                    throw new KineticaException($"Connection failed and HA failover unsuccessful: {ex.Message}", ex);
                }
            }
            catch (KineticaException)
            {
                // API error from server - don't failover, just rethrow
                throw;
            }
        }
    }

    /// <summary>
    /// Submit a request asynchronously to a Kinetica endpoint with HA failover support.
    /// This is the async version of SubmitRequest for non-blocking API calls.
    ///
    /// Matches Rust gpudb.rs submit_request() - public with HA failover.
    /// </summary>
    /// <typeparam name="TResponse">Kinetica Response Object Type</typeparam>
    /// <param name="endpoint">Kinetica Endpoint to call (e.g., "/show/table")</param>
    /// <param name="request">Kinetica Request Object</param>
    /// <param name="enableCompression">Use Compression</param>
    /// <param name="avroEncoding">Use Avro Encoding</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request</param>
    /// <returns>Task that returns the Response Object</returns>
    private async System.Threading.Tasks.Task<TResponse> SubmitRequestAsync<TResponse>(
        string endpoint,
        object request,
        bool enableCompression = false,
        bool avroEncoding = true,
        System.Threading.CancellationToken cancellationToken = default)
        where TResponse : new()
    {
        // Encode the request
        byte[] requestBytes = avroEncoding
            ? AvroEncode(request)
            : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

        // If HA failover is not available, just submit the request directly
        if (_haFailoverManager == null || _haFailoverManager.HARingSize <= 1)
        {
            string fullUrl = Url + endpoint;
            RawKineticaResponse kineticaResponse = await SubmitRequestToUrlInternalAsync(
                fullUrl, requestBytes, enableCompression, avroEncoding, cancellationToken);
            return DecodeResponse<TResponse>(kineticaResponse, avroEncoding);
        }

        // HA failover is available - attempt with failover logic
        var currentUrl = _haFailoverManager.GetUrl();
        if (currentUrl == null)
        {
            throw new KineticaException("No URL available");
        }

        var originalUrl = currentUrl;
        int currentSwitchCount = _haFailoverManager.NumClusterSwitches;

        while (true)
        {
            try
            {
                // Build the full URL with the endpoint
                string fullUrl = currentUrl.ToString().TrimEnd('/') + endpoint;
                RawKineticaResponse kineticaResponse = await SubmitRequestToUrlInternalAsync(
                    fullUrl, requestBytes, enableCompression, avroEncoding, cancellationToken);
                return DecodeResponse<TResponse>(kineticaResponse, avroEncoding);
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                // This is a connection error - attempt failover
                try
                {
                    currentUrl = _haFailoverManager.SwitchUrl(originalUrl, currentSwitchCount, IsKineticaRunning);
                    // Update the main URL reference
                    Url = currentUrl.ToString().TrimEnd('/');
                    URL = currentUrl;
                }
                catch (KineticaException)
                {
                    // Failover failed - re-throw the original exception
                    throw new KineticaException($"Connection failed and HA failover unsuccessful: {ex.Message}", ex);
                }
            }
            catch (KineticaException)
            {
                // API error from server - don't failover, just rethrow
                throw;
            }
        }
    }

    /// <summary>
    /// Submit a request directly to a specific URL without HA failover.
    /// Used for worker URLs in multi-head ingest, or when you have a specific URL to target.
    ///
    /// Matches Rust gpudb.rs submit_request_raw() - public without HA failover.
    /// </summary>
    /// <typeparam name="TResponse">Kinetica Response Object Type</typeparam>
    /// <param name="url">Full URL to submit the request to</param>
    /// <param name="request">Kinetica Request Object</param>
    /// <param name="enableCompression">Use Compression</param>
    /// <param name="avroEncoding">Use Avro Encoding</param>
    /// <returns>Response Object</returns>
    public TResponse SubmitRequestRaw<TResponse>(Uri url, object request, bool enableCompression = false, bool avroEncoding = true) where TResponse : new()
    {
        // Encode the request
        byte[] requestBytes = avroEncoding
            ? AvroEncode(request)
            : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

        RawKineticaResponse kineticaResponse = SubmitRequestToUrlInternal(url.ToString(), requestBytes, enableCompression, avroEncoding);
        return DecodeResponse<TResponse>(kineticaResponse, avroEncoding);
    }

    /// <summary>
    /// Submit pre-encoded request bytes directly to a specific URL without HA failover.
    /// Returns the raw Kinetica response wrapper (status, message, data) for error checking.
    /// This is the lowest-level method for maximum performance (used by BulkInserter).
    ///
    /// Matches Rust gpudb.rs submit_request_raw() with pre-encoded bytes.
    /// </summary>
    /// <param name="url">Full URL to submit to.</param>
    /// <param name="requestBytes">Pre-encoded Avro request bytes.</param>
    /// <returns>Raw response wrapper containing status, message, and data bytes.</returns>
    public RawKineticaResponse SubmitRequestRawBytes(Uri url, byte[] requestBytes)
    {
        return SubmitRequestToUrlInternal(url.ToString(), requestBytes, UseSnappy, true);
    }

    /// <summary>
    /// Authenticated API-reachability probe: returns <c>true</c> if the client can make API
    /// calls against the Kinetica server at the given URL, <c>false</c> otherwise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used during connection initialization to verify that a server-advertised head node URL
    /// (derived from the <c>conf.worker_http_server_urls</c> system property) is actually
    /// reachable from the client before committing to multi-head operations. A cheap
    /// <c>/show/system/status</c> request is issued directly to <paramref name="url"/> (no HA
    /// failover) through the client's configured <see cref="IHttpTransport"/>, so it carries the
    /// client's authorization and transport settings; any failure is treated as "not reachable".
    /// Mirrors the Java client's <c>isSystemRunning()</c> check.
    /// </para>
    /// <para>
    /// This differs from <see cref="IsKineticaRunning(Uri)"/>, which is an <em>unauthenticated</em>
    /// liveness ping (a raw <c>GET</c> on its own <c>HttpClient</c>) used as the failover
    /// candidate-selection predicate. This method answers "can I actually issue API requests
    /// here?" rather than "is a Kinetica process up?", which is the question that matters when
    /// deciding whether multi-head operations are viable. It also goes through the injected
    /// transport, so it is exercisable in unit tests with a fake transport.
    /// </para>
    /// </remarks>
    /// <param name="url">The server URL to probe.</param>
    /// <returns><c>true</c> if the server responds; <c>false</c> on any error.</returns>
    internal bool IsSystemRunning(Uri url)
    {
        try
        {
            SubmitRequestRaw<ShowSystemStatusResponse>(url, new ShowSystemStatusRequest());
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Internal HTTP Helpers

    /// <summary>
    /// Decode a RawKineticaResponse into the expected response type.
    /// </summary>
    private TResponse DecodeResponse<TResponse>(RawKineticaResponse kineticaResponse, bool avroEncoding) where TResponse : new()
    {
        if (avroEncoding)
        {
            return AvroDecode<TResponse>(kineticaResponse.data);
        }
        else // JSON
        {
            kineticaResponse.data_str = kineticaResponse.data_str.Replace("\\U", "\\u");
            return JsonConvert.DeserializeObject<TResponse>(kineticaResponse.data_str);
        }
    }

    /// <summary>
    /// Unauthenticated liveness ping: checks whether a Kinetica process is up at the given URL.
    /// </summary>
    /// <remarks>
    /// Issues a raw <c>GET</c> on its own short-lived <see cref="HttpClient"/> (no authorization,
    /// independent of the configured <see cref="IHttpTransport"/>) and looks for the server's
    /// "Kinetica is running!" landing response. This is the predicate passed to
    /// <c>HAFailoverManager.SwitchUrl</c> for failover candidate selection: it answers "is there
    /// a live server to fail over to?" cheaply, without depending on valid credentials or a
    /// functioning API endpoint.
    /// <para>
    /// For the stronger, authenticated "can I actually make API calls here?" check used when
    /// deciding multi-head viability during initialization, see <see cref="IsSystemRunning(Uri)"/>.
    /// The two are intentionally distinct probes; do not collapse one into the other without
    /// accounting for the auth/transport and failover-behavior differences.
    /// </para>
    /// </remarks>
    /// <param name="url">The URL to check</param>
    /// <returns>True if Kinetica is running, false otherwise</returns>
    public bool IsKineticaRunning(Uri url)
    {
        try
        {
            // Simple GET request to check if server is running
            // We can't use the transport layer here as it only supports POST
            // So we'll create a temporary HttpClient for this check, bounded by the default
            // server-connection timeout.
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(Options.DefaultServerConnectionTimeout)
            };
            using var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseText = response.Content.ReadAsStringAsync().Result;
                return responseText.Contains("Kinetica is running!");
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Internal method to submit encoded request bytes to a URL and receive a raw response.
    /// This is the lowest-level HTTP method - all other submit methods call this.
    /// </summary>
    /// <param name="url">Full URL to submit to</param>
    /// <param name="requestBytes">Binary data to send</param>
    /// <param name="enableCompression">Are we using compression (Snappy)?</param>
    /// <param name="avroEncoding">Use Avro encoding</param>
    /// <returns>RawKineticaResponse Object</returns>
    private RawKineticaResponse SubmitRequestToUrlInternal(string url, byte[] requestBytes, bool enableCompression, bool avroEncoding)
    {
        try
        {
            // Apply Snappy compression if enabled
            byte[] bodyBytes;
            string contentType;

            if (enableCompression && avroEncoding)
            {
                // Compress using Snappier (pure managed Snappy implementation)
                bodyBytes = Snappy.CompressToArray(requestBytes);
                contentType = "application/x-snappy";
            }
            else
            {
                bodyBytes = requestBytes;
                contentType = avroEncoding ? "application/octet-stream" : "application/json";
            }

            // Use the HTTP transport layer to send the request
            var responseBytes = _transport.Post(
                url,
                bodyBytes,
                contentType,
                Authorization,
                _userAgent,
                System.Threading.CancellationToken.None);

            // Decode the response
            if (avroEncoding)
            {
                return AvroDecode<RawKineticaResponse>(responseBytes);
            }
            else // JSON
            {
                var responseString = Encoding.UTF8.GetString(responseBytes);
                responseString = responseString.Replace("\\U", "\\u");
                return JsonConvert.DeserializeObject<RawKineticaResponse>(responseString)
                       ?? throw new KineticaException("Failed to deserialize response");
            }
        }
        catch (KineticaTransportException tex)
        {
            // HTTP transport returned a non-2xx status code
            // The server may have encoded an error message in the response body
            try
            {
                RawKineticaResponse? serverResponse;
                if (avroEncoding)
                {
                    serverResponse = AvroDecode<RawKineticaResponse>(tex.Body);
                }
                else // JSON
                {
                    var responseString = Encoding.UTF8.GetString(tex.Body);
                    serverResponse = JsonConvert.DeserializeObject<RawKineticaResponse>(responseString);
                }

                throw new KineticaException(
                    serverResponse?.message ?? $"Server returned HTTP {tex.StatusCode}",
                    tex.StatusCode,
                    tex);
            }
            catch (KineticaException)
            {
                throw;
            }
            catch
            {
                // Could not decode error response - throw with status code
                throw new KineticaException($"Server returned HTTP {tex.StatusCode}", tex.StatusCode, tex);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new KineticaException(ex.ToString(), ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new KineticaException("Request timed out: " + ex.ToString(), ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new KineticaException("Request cancelled: " + ex.ToString(), ex);
        }
        catch (KineticaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new KineticaException(ex.ToString(), ex);
        }
    }

    /// <summary>
    /// Internal async method to submit encoded request bytes to a URL and receive a raw response.
    /// This is the async version of SubmitRequestToUrlInternal for future async API support.
    /// </summary>
    /// <param name="url">Full URL to submit to</param>
    /// <param name="requestBytes">Binary data to send</param>
    /// <param name="enableCompression">Are we using compression (Snappy)?</param>
    /// <param name="avroEncoding">Use Avro encoding</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that returns RawKineticaResponse Object</returns>
    private async Task<RawKineticaResponse> SubmitRequestToUrlInternalAsync(
        string url,
        byte[] requestBytes,
        bool enableCompression,
        bool avroEncoding,
        System.Threading.CancellationToken cancellationToken = default)
    {
        try
        {
            // Apply Snappy compression if enabled
            byte[] bodyBytes;
            string contentType;

            if (enableCompression && avroEncoding)
            {
                // Compress using Snappier (pure managed Snappy implementation)
                bodyBytes = Snappy.CompressToArray(requestBytes);
                contentType = "application/x-snappy";
            }
            else
            {
                bodyBytes = requestBytes;
                contentType = avroEncoding ? "application/octet-stream" : "application/json";
            }

            // Use the HTTP transport layer to send the request asynchronously
            var responseBytes = await _transport
                .PostAsync(url, bodyBytes, contentType, Authorization, _userAgent, cancellationToken)
                .ConfigureAwait(false);

            // Decode the response
            if (avroEncoding)
            {
                return AvroDecode<RawKineticaResponse>(responseBytes);
            }
            else // JSON
            {
                var responseString = Encoding.UTF8.GetString(responseBytes);
                responseString = responseString.Replace("\\U", "\\u");
                return JsonConvert.DeserializeObject<RawKineticaResponse>(responseString)
                       ?? throw new KineticaException("Failed to deserialize response");
            }
        }
        catch (KineticaTransportException tex)
        {
            // HTTP transport returned a non-2xx status code
            // The server may have encoded an error message in the response body
            try
            {
                RawKineticaResponse? serverResponse;
                if (avroEncoding)
                {
                    serverResponse = AvroDecode<RawKineticaResponse>(tex.Body);
                }
                else // JSON
                {
                    var responseString = Encoding.UTF8.GetString(tex.Body);
                    serverResponse = JsonConvert.DeserializeObject<RawKineticaResponse>(responseString);
                }

                throw new KineticaException(
                    serverResponse?.message ?? $"Server returned HTTP {tex.StatusCode}",
                    tex.StatusCode,
                    tex);
            }
            catch (KineticaException)
            {
                throw;
            }
            catch
            {
                // Could not decode error response - throw with status code
                throw new KineticaException($"Server returned HTTP {tex.StatusCode}", tex.StatusCode, tex);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new KineticaException(ex.ToString(), ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new KineticaException("Request timed out: " + ex.ToString(), ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new KineticaException("Request cancelled: " + ex.ToString(), ex);
        }
        catch (KineticaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new KineticaException(ex.ToString(), ex);
        }
    }

    /// <summary>
    /// Checks if an exception is a connection error that warrants HA failover.
    /// API errors (from server) should not trigger failover.
    /// Connection errors (network issues) should trigger failover.
    /// </summary>
    public static bool IsConnectionError(Exception ex)
    {
        return ex is System.Net.WebException webEx && webEx.Status != WebExceptionStatus.ProtocolError ||
               ex is System.Net.Sockets.SocketException ||
               ex is IOException ||
               ex is System.Net.Http.HttpRequestException ||
               ex is TaskCanceledException ||
               (ex is KineticaException kex && kex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Forces a failover to the next cluster in the HA ring.
    /// Returns the new head node URL if successful, null otherwise.
    /// </summary>
    /// <param name="currentUrl">The current URL that failed.</param>
    /// <param name="currentSwitchCount">The current cluster switch count.</param>
    /// <returns>The new URL after failover, or null if failover failed.</returns>
    internal Uri? ForceHAFailover(Uri currentUrl, int currentSwitchCount)
    {
        if (_haFailoverManager == null || _haFailoverManager.HARingSize <= 1)
        {
            return null;
        }

        try
        {
            var newUrl = _haFailoverManager.SwitchUrl(currentUrl, currentSwitchCount, IsKineticaRunning);
            Url = newUrl.ToString().TrimEnd('/');
            URL = newUrl;
            return newUrl;
        }
        catch (KineticaException)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the HA ring size.
    /// </summary>
    public int HARingSize => _haFailoverManager?.HARingSize ?? 1;

    #endregion

    #region Type Registration and Encoding

    private void SetDecoderIfMissing(string typeId, string label, string schemaString, IDictionary<string, IList<string>> properties)
    {
        // If the table is a collection, it does not have a proper type so ignore it

        if (typeId == "<collection>")
        {
            return;
        }

        knownTypes.GetOrAdd(typeId, (s) =>
        {
            return new KineticaType(label, schemaString, properties);
        });
        typeNameLookup[label] = typeId;
    }


    /// <summary>
    /// Retrieve a KineticaType object by the type label.
    /// </summary>
    /// <param name="typeName">The label/name of the type.</param>
    /// <returns></returns>
    private KineticaType? GetType(string typeName)
    {
        KineticaType? type = null;
        if (typeNameLookup.TryGetValue(typeName, out string? typeId))
        {
            knownTypes.TryGetValue(typeId, out type);
        }

        return type;
    }


    /// <summary>
    /// Given a class type, look up the associated KineticaType.  If none is found, return null.
    /// </summary>
    /// <param name="objectType">The type of the object whose associated KineticaType we need.</param>
    /// <returns></returns>
    private KineticaType? LookupKineticaType( Type objectType )
    {
        if (!kineticaTypeLookup.TryGetValue(objectType, out KineticaType? value))
            return null; // none found

        return value;
    }  // LookupKineticaType()


    /// <summary>
    /// Encode specified object using Avro
    /// </summary>
    /// <param name="obj">Object to encode</param>
    /// <returns>Byte array of binary Avro-encoded data</returns>
    internal byte[] AvroEncode(object obj)
    {
        // Create a stream that will allow us to view the underlying memory
        using ( var ms = new MemoryStream())
        {
            // Write the object to the memory stream
            // If obj is an ISpecificRecord, this is more efficient
            if ( obj is Avro.Specific.ISpecificRecord)
            {
                var schema = (obj as Avro.Specific.ISpecificRecord).Schema;
                Avro.Specific.SpecificDefaultWriter writer = new(schema);
                writer.Write(schema, obj, new BinaryEncoder(ms));
            }
            else // Not an ISpecificRecord - this way is less efficient
            {
                // Get the KineticaType associated with the object to be encoded
                Type obj_type = obj.GetType();
                KineticaType? ktype = LookupKineticaType( obj_type );
                if ( ktype == null )
                {
                    throw new KineticaException( "No known KineticaType associated with the given object.  " +
                                                 "Need a known KineticaType to encode the object." );
                }

                // Make a copy of the object to send as a GenericRecord, then write that to the memory stream
                var schema = KineticaData.SchemaFromType( obj.GetType(), ktype );
                var recordToSend = MakeGenericRecord( obj, ktype );
                var writer = new Avro.Generic.DefaultWriter(schema);
                writer.Write(schema, recordToSend, new BinaryEncoder(ms));
            }

            // Get the memory from the stream
            return ms.ToArray();
        }
    }  // end AvroEncode

    /// <summary>
    /// Make a copy of an object as an Avro GenericRecord
    /// </summary>
    /// <param name="obj">Original object</param>
    /// <param name="ktype">An associated KineticaType object that
    /// describes the original object.</param>
    /// <returns>GenericRecord object which is a copy of the specified object</returns>
    private Avro.Generic.GenericRecord MakeGenericRecord( object obj, KineticaType ktype )
    {
        // Get the schema
        var schema = KineticaData.SchemaFromType( obj.GetType(), ktype );

        // Create a new GenericRecord for this schema
        var recordToSend = new Avro.Generic.GenericRecord(schema);

        // Copy each field from obj to recordToSend
        foreach ( var field in schema.Fields)
        {
            var property = obj.GetType()
                            .GetProperties()
                            .FirstOrDefault(prop => prop.Name.ToLowerInvariant() == field.Name.ToLowerInvariant());

            if (property == null) continue;

            recordToSend.Add(field.Name, property.GetValue(obj, null));
        }

        // Return the newly created object
        return recordToSend;
    }

    /// <summary>
    /// Decode binary Avro data into an object.
    /// </summary>
    /// <typeparam name="T">Type of expected object</typeparam>
    /// <param name="bytes">Binary Avro data</param>
    /// <param name="ktype">An optional KineticaType object to help in decoding the object.</param>
    /// <returns>New object</returns>
    internal T AvroDecode<T>(byte[] bytes, KineticaType? ktype = null) where T : new()
    {
        // Get the schema
        var schema = KineticaData.SchemaFromType( typeof(T), ktype );

        // Create a stream to read the binary data
        using (var ms = new MemoryStream(bytes))
        {
            // Create a new object to return
            T obj = new();
            if (obj is Avro.Specific.ISpecificRecord)
            {
                var reader = new Avro.Specific.SpecificDefaultReader(schema, schema);
                reader.Read(obj, new BinaryDecoder(ms));
            }
            else
            {
                // Not ISpecificRecord, so first read into a new GenericRecord
                var reader = new Avro.Generic.DefaultReader(schema, schema);
                Avro.Generic.GenericRecord recordToReceive = new(schema);
                reader.Read(recordToReceive, new BinaryDecoder(ms));

                // Now, copy all the fields from the GenericRecord to obj
                foreach (var field in schema.Fields)
                {
                    var property = obj.GetType()
                                    .GetProperties()
                                    .FirstOrDefault(prop => prop.Name.ToLowerInvariant() == field.Name.ToLowerInvariant());

                    if (property == null) continue;

                    // Try to get the property
                    if (recordToReceive.TryGetValue(field.Name, out object val))
                    {
                        // If successful, write the property to obj
                        property.SetValue(obj, val);
                    }
                }  // end foreach
            }  // end if-else

            // Return the new object
            return obj;
        }  // end using
    }  // end AvroDecode<T>


    /// <summary>
    /// Decode binary Avro data from a stream into an object
    /// </summary>
    /// <typeparam name="T">Type of expected object</typeparam>
    /// <param name="stream">Stream to read for object data</param>
    /// <returns>New object</returns>
    internal T AvroDecode<T>(Stream stream) where T : Avro.Specific.ISpecificRecord, new()
    {
        // T obj = new T(); // Activator.CreateInstance<T>();
        var schema = KineticaData.SchemaFromType( typeof(T), null );
        var reader = new Avro.Specific.SpecificReader<T>(schema, schema);
        return reader.Read(default, new BinaryDecoder(stream));
    }

    #endregion
}  // end class Kinetica


