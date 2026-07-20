using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace kinetica;

/// <summary>
/// High availability failover order options.
/// </summary>
public enum HAFailoverOrder
{
    /// <summary>
    /// Failover to clusters in a random order
    /// </summary>
    Random,

    /// <summary>
    /// Failover to clusters in sequential order (default)
    /// </summary>
    Sequential
}

/// <summary>
/// Contains address information for a Kinetica cluster.
/// Stores information about a cluster including its active head node URL,
/// worker rank URLs, hostnames, host manager URL, and system properties.
/// </summary>
public class ClusterAddressInfo
{
    /// <summary>
    /// The active head node URL for the cluster
    /// </summary>
    public Uri ActiveHeadNodeUrl { get; set; }

    /// <summary>
    /// System properties map from the cluster
    /// </summary>
    public IDictionary<string, string> SystemProperties { get; set; }

    /// <summary>
    /// List of worker rank URLs
    /// </summary>
    public IList<Uri> WorkerRankUrls { get; set; }

    /// <summary>
    /// Set of hostnames/IP addresses in the cluster
    /// </summary>
    public HashSet<string> HostNames { get; set; }

    /// <summary>
    /// URL for the host manager
    /// </summary>
    public Uri HostManagerUrl { get; set; }

    /// <summary>
    /// Whether this is the primary cluster
    /// </summary>
    public bool IsPrimaryCluster { get; set; }

    /// <summary>
    /// The last-seen HA drained status for this cluster
    /// (<c>drained</c>/<c>draining</c>/<c>not_drained</c>), or <c>null</c> if unknown/not HA.
    /// Recorded during discovery; consulted when deciding whether the cluster may be routed to.
    /// </summary>
    public string HaStatus { get; set; }

    /// <summary>
    /// Creates a new ClusterAddressInfo for an active cluster.
    /// </summary>
    /// <param name="activeHeadNodeUrl">The URL of the active head node</param>
    /// <param name="systemProperties">System properties map from the cluster</param>
    /// <param name="workerRankUrls">List of worker rank URLs</param>
    /// <param name="hostNames">Set of hostnames/IP addresses in the cluster</param>
    /// <param name="hostManagerUrl">URL for the host manager</param>
    public ClusterAddressInfo(
        Uri activeHeadNodeUrl,
        IDictionary<string, string> systemProperties,
        IList<Uri> workerRankUrls,
        HashSet<string> hostNames,
        Uri hostManagerUrl)
    {
        ActiveHeadNodeUrl = activeHeadNodeUrl;
        SystemProperties = systemProperties ?? new Dictionary<string, string>();
        WorkerRankUrls = workerRankUrls ?? new List<Uri>();
        HostNames = hostNames ?? new HashSet<string>();
        HostManagerUrl = hostManagerUrl;
        IsPrimaryCluster = false;

        // Ensure that all the known ranks' hostnames are also accounted for
        UpdateHostnamesBasedOnRankUrls();
    }

    /// <summary>
    /// Creates a ClusterAddressInfo with just an active URL and host manager port.
    /// </summary>
    /// <param name="activeHeadNodeUrl">The URL of the active head node</param>
    /// <param name="hostManagerPort">The port for the host manager</param>
    public ClusterAddressInfo(Uri activeHeadNodeUrl, int hostManagerPort)
    {
        ActiveHeadNodeUrl = activeHeadNodeUrl;
        SystemProperties = new Dictionary<string, string>();
        WorkerRankUrls = new List<Uri>();
        HostNames = new HashSet<string>();
        IsPrimaryCluster = false;

        // Create host manager URL
        var builder = new UriBuilder(activeHeadNodeUrl)
        {
            Port = hostManagerPort,
            Path = string.Empty
        };
        HostManagerUrl = builder.Uri;

        // Ensure that all the known ranks' hostnames are also accounted for
        UpdateHostnamesBasedOnRankUrls();
    }

    /// <summary>
    /// Adds the hostnames of the head and worker rank URLs to the list of hostnames
    /// if they are not already part of it.
    /// </summary>
    private void UpdateHostnamesBasedOnRankUrls()
    {
        // Put the head rank's hostname in the saved hostnames
        if (ActiveHeadNodeUrl != null && !string.IsNullOrEmpty(ActiveHeadNodeUrl.Host))
        {
            if (!DoesClusterContainNode(ActiveHeadNodeUrl.Host))
            {
                HostNames.Add(ActiveHeadNodeUrl.Host);
            }
        }

        // Put each worker rank's hostname in the saved hostnames
        if (WorkerRankUrls != null)
        {
            foreach (var workerUrl in WorkerRankUrls)
            {
                if (workerUrl != null && !string.IsNullOrEmpty(workerUrl.Host))
                {
                    if (!DoesClusterContainNode(workerUrl.Host))
                    {
                        HostNames.Add(workerUrl.Host);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if the given hostname (or IP address) is part of this cluster.
    /// </summary>
    /// <param name="hostName">The hostname or IP address to check</param>
    /// <returns>true if this cluster contains a machine with the given hostname or IP address</returns>
    public bool DoesClusterContainNode(string hostName)
    {
        return HostNames.Contains(hostName);
    }

    /// <summary>
    /// Returns a string representation of this cluster address info.
    /// </summary>
    public override string ToString()
    {
        var workerUrls = string.Join(", ", WorkerRankUrls.Select(u => u.ToString()));
        var hostnames = string.Join(", ", HostNames);
        return $"{{ activeHeadNodeUrl: {ActiveHeadNodeUrl}, workerRankUrls: [{workerUrls}], hostNames: [{hostnames}], hostManagerUrl: {HostManagerUrl}, isPrimaryCluster: {IsPrimaryCluster} }}";
    }

    /// <summary>
    /// Equality check based on active head node URL.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is ClusterAddressInfo other)
        {
            return ActiveHeadNodeUrl?.ToString() == other.ActiveHeadNodeUrl?.ToString();
        }
        return false;
    }

    /// <summary>
    /// Hash code based on active head node URL.
    /// </summary>
    public override int GetHashCode()
    {
        return ActiveHeadNodeUrl?.ToString().GetHashCode() ?? 0;
    }
}

/// <summary>
/// Manages high availability failover for Kinetica connections.
/// This class tracks multiple cluster addresses and handles failover logic
/// when a cluster becomes unavailable.
/// </summary>
public class HAFailoverManager
{
    /// <summary>The default port used for host-manager URLs (9300).</summary>
    public const int DefaultHostManagerPort = 9300;

    // System properties response keys
    private const string SYSTEM_PROP_ENABLE_HTTPD = "conf.enable_httpd_proxy";
    private const string SYSTEM_PROP_ENABLE_MH = "conf.enable_worker_http_servers";
    private const string SYSTEM_PROP_NUM_HOSTS = "conf.number_of_hosts";
    private const string SYSTEM_PROP_HEAD_NODE_URLS = "conf.ha_ring_head_nodes_full";
    private const string SYSTEM_PROP_SERVER_URLS = "conf.worker_http_server_urls";
    private const string SYSTEM_PROP_ENABLE_HA = "conf.enable_ha";
    private const string SYSTEM_PROP_TRUE = "TRUE";

    private readonly List<ClusterAddressInfo> _hostAddresses;
    private readonly List<int> _haUrlIndices;
    private string _primaryUrlHostname;
    private int _currentClusterIndexPointer;
    private int _numClusterSwitches;
    private readonly object _lock = new object();

    /// <summary>
    /// Whether failover is disabled
    /// </summary>
    public bool DisableFailover { get; set; }

    /// <summary>
    /// Whether auto-discovery is disabled
    /// </summary>
    public bool DisableAutoDiscovery { get; set; }

    /// <summary>
    /// Host manager port
    /// </summary>
    public int HostManagerPort { get; set; }

    /// <summary>
    /// HA failover order
    /// </summary>
    public HAFailoverOrder FailoverOrder { get; set; }

    /// <summary>
    /// Optional hostname regex for filtering URLs
    /// </summary>
    public Regex? HostnameRegex { get; set; }

    /// <summary>
    /// Optional logger (category "Kinetica.HAFailover"). Defaults to no-op when unset.
    /// </summary>
    public ILogger Logger { get; set; } = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    /// <summary>
    /// Creates a new HAFailoverManager.
    /// </summary>
    public HAFailoverManager()
    {
        _hostAddresses = new List<ClusterAddressInfo>();
        _haUrlIndices = new List<int>();
        _primaryUrlHostname = string.Empty;
        _currentClusterIndexPointer = 0;
        _numClusterSwitches = 0;
        DisableFailover = false;
        DisableAutoDiscovery = false;
        HostManagerPort = DefaultHostManagerPort;
        FailoverOrder = HAFailoverOrder.Sequential;
    }

    /// <summary>
    /// Gets the number of clusters in the HA ring.
    /// </summary>
    public int HARingSize
    {
        get
        {
            lock (_lock)
            {
                return _hostAddresses.Count;
            }
        }
    }

    /// <summary>
    /// Gets the number of times the client has switched to a different cluster.
    /// </summary>
    public int NumClusterSwitches
    {
        get
        {
            lock (_lock)
            {
                return _numClusterSwitches;
            }
        }
    }

    /// <summary>
    /// Gets all cluster addresses.
    /// </summary>
    public IList<ClusterAddressInfo> GetHostAddresses()
    {
        lock (_lock)
        {
            return new List<ClusterAddressInfo>(_hostAddresses);
        }
    }

    /// <summary>
    /// Gets the list of URLs of the active head ranks of all clusters.
    /// </summary>
    public IList<Uri> GetUrls()
    {
        lock (_lock)
        {
            return _hostAddresses.Select(h => h.ActiveHeadNodeUrl).ToList();
        }
    }

    /// <summary>
    /// Gets the list of URLs in failover order.
    /// </summary>
    public IList<Uri> GetFailoverUrls()
    {
        lock (_lock)
        {
            return _haUrlIndices
                .Where(i => i < _hostAddresses.Count)
                .Select(i => _hostAddresses[i].ActiveHeadNodeUrl)
                .ToList();
        }
    }

    /// <summary>
    /// Gets the active cluster's information.
    /// </summary>
    public ClusterAddressInfo? GetClusterInfo()
    {
        lock (_lock)
        {
            if (_hostAddresses.Count == 0)
                return null;

            if (_hostAddresses.Count == 1)
                return _hostAddresses[0];

            if (_currentClusterIndexPointer < _haUrlIndices.Count)
            {
                int index = _haUrlIndices[_currentClusterIndexPointer];
                if (index < _hostAddresses.Count)
                    return _hostAddresses[index];
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the current active URL.
    /// </summary>
    public Uri? GetUrl()
    {
        return GetClusterInfo()?.ActiveHeadNodeUrl;
    }

    /// <summary>
    /// Adds a cluster address to the manager.
    /// </summary>
    public void AddCluster(ClusterAddressInfo clusterInfo)
    {
        lock (_lock)
        {
            _hostAddresses.Add(clusterInfo);
        }
    }

    /// <summary>
    /// Initializes the manager with a list of URLs.
    /// </summary>
    /// <param name="urls">The URLs to initialize with</param>
    /// <param name="kinetica">The Kinetica client for system property queries</param>
    public void Initialize(IList<Uri> urls, Kinetica? kinetica = null)
    {
        lock (_lock)
        {
            _hostAddresses.Clear();
            _haUrlIndices.Clear();
            _currentClusterIndexPointer = 0;
            _numClusterSwitches = 0;

            var urlQueue = new Queue<Uri>(urls);
            int numUserGivenUrls = urlQueue.Count;
            int numProcessedUrls = 0;
            var clusterIndicesOfUserGivenUrls = new List<int>();

            while (urlQueue.Count > 0)
            {
                var url = urlQueue.Dequeue();
                bool isUserGivenUrl = numProcessedUrls < numUserGivenUrls;
                numProcessedUrls++;

                // Check if this hostname is already in a known cluster
                int? existingIndex = GetIndexOfClusterContainingNode(url.Host);
                if (existingIndex.HasValue)
                {
                    if (isUserGivenUrl)
                        clusterIndicesOfUserGivenUrls.Add(existingIndex.Value);
                    continue;
                }

                // Create minimal cluster info
                ClusterAddressInfo? clusterInfo = null;

                if (!DisableAutoDiscovery && kinetica != null)
                {
                    // Ask THIS specific URL for its running + drain status (per-URL; does not
                    // trigger failover).
                    var status = kinetica.GetSystemRunningStatus(url, quickCheck: false);

                    if (!status.IsRunning)
                    {
                        // Down or unreachable: record it minimally; it may come up later. A draining
                        // node reports running (see below), so this branch is genuinely "not up".
                        Logger.LogWarning(
                            "Adding cluster with URL {URL} to the ring though it is not confirmed running.",
                            url);
                        clusterInfo = new ClusterAddressInfo(url, HostManagerPort) { HaStatus = status.HaStatus };
                    }
                    else
                    {
                        try
                        {
                            // Running (possibly draining): fetch full properties from THIS URL.
                            var systemProps = kinetica.GetSystemProperties(url);
                            clusterInfo = CreateClusterAddressInfo(url, systemProps);
                            clusterInfo.HaStatus = status.HaStatus;

                            // Get HA ring URLs and add them to the queue
                            var haRingUrls = GetHARingHeadNodeUrls(systemProps);
                            foreach (var haUrl in haRingUrls)
                            {
                                if (!GetIndexOfClusterContainingNode(haUrl.Host).HasValue &&
                                    !urlQueue.Contains(haUrl))
                                {
                                    urlQueue.Enqueue(haUrl);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning(ex,
                                "Adding failed connectivity check cluster to cluster list with URL: {URL}.",
                                url);
                            clusterInfo = new ClusterAddressInfo(url, HostManagerPort) { HaStatus = status.HaStatus };
                        }
                    }
                }
                else
                {
                    clusterInfo = new ClusterAddressInfo(url, HostManagerPort);
                }

                // For a user-given URL with auto-discovery enabled, verify the cluster is also
                // reachable at the server-known head node URL we just learned via auto-discovery.
                // The server advertises its ranks via conf.worker_http_server_urls (see
                // CreateClusterAddressInfo); when the client connected with an external/global-DNS
                // URL but the server returns internal/LAN URLs, the client can still talk to the
                // head node via the user-given URL but cannot reach any rank directly. That
                // implicitly eliminates multi-head operations. Mirror the Java client: warn, then
                // throw so the connection is reprocessed with auto-discovery disabled (i.e.
                // degraded mode).
                if (isUserGivenUrl && !DisableAutoDiscovery && kinetica != null)
                {
                    Uri clusterHeadNodeUrl = clusterInfo.ActiveHeadNodeUrl;
                    if (clusterHeadNodeUrl != null && !url.Equals(clusterHeadNodeUrl))
                    {
                        if (!kinetica.IsSystemRunning(clusterHeadNodeUrl))
                        {
                            Logger.LogWarning(
                                "Disabling auto-discovery & multi-head operations: cluster reachable with " +
                                "user-given URL <{UserUrl}> but not with server-known URL <{ServerUrl}>. " +
                                "Reprocessing the connection without auto-discovery; database commands will " +
                                "still work via the head node, but multi-head operations (BulkInserter, " +
                                "RecordRetriever) will be unavailable.",
                                url, clusterHeadNodeUrl);

                            // Deliberately do NOT set DisableAutoDiscovery here. Unlike the Java client
                            // (which sets the flag before throwing), the C# InitializeWithRetry loop owns
                            // that state transition: it catches this exception, flips DisableAutoDiscovery,
                            // and retries using only the user-given URLs. Pre-setting the flag would make
                            // the retry loop treat this as a second failure and give up.
                            throw new KineticaException(
                                $"Could not connect to user-given URL {url} via server-known head node URL {clusterHeadNodeUrl}");
                        }

                        Logger.LogDebug(
                            "Verified connectivity with user-given URL {UserUrl} at server-known URL {ServerUrl}",
                            url, clusterHeadNodeUrl);
                    }
                }

                if (isUserGivenUrl)
                    clusterIndicesOfUserGivenUrls.Add(_hostAddresses.Count);

                _hostAddresses.Add(clusterInfo);
            }

            if (_hostAddresses.Count == 0)
                throw new KineticaException("Could not connect to any working Kinetica server");

            // Set the primary cluster
            if (_hostAddresses.Count == 1)
            {
                _hostAddresses[0].IsPrimaryCluster = true;
                _primaryUrlHostname = _hostAddresses[0].ActiveHeadNodeUrl.Host;
            }
            else
            {
                // If all user-given URLs belong to the same cluster, make that the primary
                if (string.IsNullOrEmpty(_primaryUrlHostname))
                {
                    var uniqueIndices = new HashSet<int>(clusterIndicesOfUserGivenUrls);
                    if (uniqueIndices.Count == 1 && clusterIndicesOfUserGivenUrls.Count > 0)
                    {
                        int primaryIndex = clusterIndicesOfUserGivenUrls[0];
                        _primaryUrlHostname = _hostAddresses[primaryIndex].ActiveHeadNodeUrl.Host;
                    }
                }
            }

            // Flag the primary cluster and move it to the front
            if (!string.IsNullOrEmpty(_primaryUrlHostname))
            {
                int? primaryIndex = GetIndexOfClusterContainingNode(_primaryUrlHostname);
                if (primaryIndex.HasValue)
                {
                    _hostAddresses[primaryIndex.Value].IsPrimaryCluster = true;
                    if (primaryIndex.Value > 0)
                    {
                        var primary = _hostAddresses[primaryIndex.Value];
                        _hostAddresses.RemoveAt(primaryIndex.Value);
                        _hostAddresses.Insert(0, primary);
                    }
                }
            }

            // Randomize URLs for failover
            RandomizeUrls();
        }
    }

    /// <summary>
    /// Finds the index of the cluster containing the given hostname.
    /// </summary>
    private int? GetIndexOfClusterContainingNode(string hostname)
    {
        for (int i = 0; i < _hostAddresses.Count; i++)
        {
            if (_hostAddresses[i].DoesClusterContainNode(hostname))
                return i;
        }
        return null;
    }

    /// <summary>
    /// Creates a ClusterAddressInfo from a URL and system properties.
    /// </summary>
    private ClusterAddressInfo CreateClusterAddressInfo(Uri url, IDictionary<string, string> systemProperties)
    {
        bool isHttpdEnabled = GetSystemPropertyBool(systemProperties, SYSTEM_PROP_ENABLE_HTTPD);
        bool isMultiHeadEnabled = GetSystemPropertyBool(systemProperties, SYSTEM_PROP_ENABLE_MH);

        // Get rank URLs
        var rankUrls = new List<Uri>();
        if (isMultiHeadEnabled && systemProperties.TryGetValue(SYSTEM_PROP_SERVER_URLS, out var serverUrlsStr))
        {
            rankUrls = ParseRankUrls(serverUrlsStr);
        }

        // Get head node URL (first rank URL, or fall back to given URL)
        Uri activeHeadNodeUrl;
        if (rankUrls.Count > 0)
        {
            activeHeadNodeUrl = rankUrls[0];
            rankUrls.RemoveAt(0);
        }
        else
        {
            activeHeadNodeUrl = url;
        }

        // Get hostnames
        var hostNames = GetHostnamesFromSystemProperties(systemProperties);

        // Create host manager URL
        Uri hostManagerUrl;
        if (isHttpdEnabled && !string.IsNullOrEmpty(activeHeadNodeUrl.AbsolutePath) && activeHeadNodeUrl.AbsolutePath != "/")
        {
            var builder = new UriBuilder(activeHeadNodeUrl)
            {
                Path = "/gpudb-host-manager"
            };
            hostManagerUrl = builder.Uri;
        }
        else
        {
            var builder = new UriBuilder(activeHeadNodeUrl)
            {
                Port = HostManagerPort,
                Path = string.Empty
            };
            hostManagerUrl = builder.Uri;
        }

        return new ClusterAddressInfo(activeHeadNodeUrl, systemProperties, rankUrls, hostNames, hostManagerUrl);
    }

    /// <summary>
    /// Parses rank URLs from the system property string.
    /// </summary>
    private List<Uri> ParseRankUrls(string serverUrlsStr)
    {
        var result = new List<Uri>();
        var rankEntries = serverUrlsStr.Split(';');

        foreach (var entry in rankEntries)
        {
            if (string.IsNullOrEmpty(entry))
                continue;

            var urls = entry.Split(',');
            foreach (var urlStr in urls)
            {
                if (Uri.TryCreate(urlStr.Trim(), UriKind.Absolute, out var parsedUrl))
                {
                    bool shouldAdd = HostnameRegex == null || HostnameRegex.IsMatch(parsedUrl.Host);
                    if (shouldAdd)
                    {
                        result.Add(parsedUrl);
                        break;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets hostnames from system properties.
    /// </summary>
    private HashSet<string> GetHostnamesFromSystemProperties(IDictionary<string, string> systemProperties)
    {
        var hostnames = new HashSet<string>();

        if (!systemProperties.TryGetValue(SYSTEM_PROP_NUM_HOSTS, out var numHostsStr) ||
            !int.TryParse(numHostsStr, out int numHosts))
        {
            return hostnames;
        }

        for (int i = 0; i < numHosts; i++)
        {
            string key = $"conf.host{i}_public_urls";
            if (systemProperties.TryGetValue(key, out var hostnameStr))
            {
                var hostUrls = hostnameStr.Split(',');
                foreach (var hostname in hostUrls)
                {
                    var host = hostname.Trim();
                    // Strip protocol if present
                    int idx = host.IndexOf("://");
                    if (idx >= 0)
                        host = host.Substring(idx + 3);

                    bool shouldAdd = HostnameRegex == null || HostnameRegex.IsMatch(host);
                    if (shouldAdd)
                    {
                        hostnames.Add(host);
                        break;
                    }
                }
            }
        }

        return hostnames;
    }

    /// <summary>
    /// Gets HA ring head node URLs from system properties.
    /// </summary>
    private List<Uri> GetHARingHeadNodeUrls(IDictionary<string, string> systemProperties)
    {
        var haUrls = new List<Uri>();

        if (!GetSystemPropertyBool(systemProperties, SYSTEM_PROP_ENABLE_HA))
            return haUrls;

        if (!systemProperties.TryGetValue(SYSTEM_PROP_HEAD_NODE_URLS, out var haRingStr) ||
            string.IsNullOrEmpty(haRingStr))
        {
            return haUrls;
        }

        var clusterEntries = haRingStr.Split(';');
        foreach (var entry in clusterEntries)
        {
            if (string.IsNullOrEmpty(entry))
                continue;

            var urls = entry.Split(',');
            foreach (var urlStr in urls)
            {
                if (Uri.TryCreate(urlStr.Trim(), UriKind.Absolute, out var parsedUrl))
                {
                    bool shouldAdd = HostnameRegex == null || HostnameRegex.IsMatch(parsedUrl.Host);
                    if (shouldAdd)
                    {
                        haUrls.Add(parsedUrl);
                        break;
                    }
                }
            }
        }

        return haUrls;
    }

    /// <summary>
    /// Gets a boolean system property value.
    /// </summary>
    private static bool GetSystemPropertyBool(IDictionary<string, string> properties, string key)
    {
        return properties.TryGetValue(key, out var value) &&
               value.Equals(SYSTEM_PROP_TRUE, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Randomizes the HA URL indices for failover, keeping the primary cluster first.
    /// </summary>
    private void RandomizeUrls()
    {
        _haUrlIndices.Clear();
        for (int i = 0; i < _hostAddresses.Count; i++)
        {
            _haUrlIndices.Add(i);
        }

        if (FailoverOrder == HAFailoverOrder.Random && _haUrlIndices.Count > 1)
        {
            // Keep primary (index 0) first, shuffle the rest
            var rng = new Random();
            int n = _haUrlIndices.Count;
            for (int i = n - 1; i > 1; i--)
            {
                int j = rng.Next(1, i + 1);
                (_haUrlIndices[i], _haUrlIndices[j]) = (_haUrlIndices[j], _haUrlIndices[i]);
            }
        }
    }

    /// <summary>
    /// Selects the next cluster based on the HA failover priority.
    /// </summary>
    private void SelectNextCluster()
    {
        _currentClusterIndexPointer = (_currentClusterIndexPointer + 1) % _hostAddresses.Count;
        Interlocked.Increment(ref _numClusterSwitches);
    }

    /// <summary>
    /// Switches to the next available cluster URL for HA failover.
    /// </summary>
    /// <param name="oldUrl">The URL that was in use when the failure occurred</param>
    /// <param name="oldNumClusterSwitches">The switch count before this switch was initiated</param>
    /// <param name="isClusterUsable">Predicate deciding whether a candidate cluster may be routed
    /// to: reachable, running, and not draining. A bare liveness ping is <em>not</em> sufficient --
    /// a draining cluster answers a ping but rejects queries -- so this must consult the cluster's
    /// drain state (see <c>Kinetica.IsClusterUsable</c>).</param>
    /// <returns>The new URL to use</returns>
    /// <exception cref="KineticaException">If failover is not possible</exception>
    public Uri SwitchUrl(Uri oldUrl, int oldNumClusterSwitches, Func<Uri, bool>? isClusterUsable = null)
    {
        lock (_lock)
        {
            if (DisableFailover)
            {
                throw new KineticaException("Failover is disabled!");
            }

            // If there is only one URL, then we can't switch URLs
            if (_hostAddresses.Count == 1)
            {
                throw new KineticaException("Only one cluster in ring; HA failover unavailable");
            }

            // Get how many more times other threads have switched clusters
            int countSwitchesSinceInvocation = _numClusterSwitches - oldNumClusterSwitches;

            // Check if another thread has tried all the clusters in the HA ring
            if (countSwitchesSinceInvocation >= _hostAddresses.Count)
            {
                throw new KineticaException($"Fail-over attempted as many times as clusters in the ring; URLs attempted: {string.Join(", ", GetUrls())}");
            }

            // Check if another thread beat us to switching the URL
            var currentUrl = GetUrl();
            if (currentUrl != null && !currentUrl.Equals(oldUrl) && countSwitchesSinceInvocation > 0)
            {
                return currentUrl;
            }

            // This thread is the first one here--select the next cluster to use
            while (true)
            {
                SelectNextCluster();

                currentUrl = GetUrl();
                if (currentUrl == null)
                {
                    throw new KineticaException("No current URL available");
                }

                if (currentUrl.Equals(oldUrl))
                {
                    // Re-shuffle and throw exception
                    RandomizeUrls();
                    throw new KineticaException($"Circled back to original URL; no clusters available for fail-over among these: {string.Join(", ", GetUrls())}");
                }

                // Accept the candidate only if it is usable (reachable + running + not draining).
                // A draining cluster passes a liveness ping but rejects queries, so a ping-only
                // test here would let failover land on a draining cluster and thrash.
                bool usable = isClusterUsable?.Invoke(currentUrl) ?? true;
                if (usable)
                {
                    return currentUrl;
                }
            }
        }
    }
}
