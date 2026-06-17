using System.Collections;
using System.Text.RegularExpressions;

namespace kinetica.Utils;

/// <summary>
/// A list of worker URLs to use for multi-head operations.
///
/// This class contains the URLs of worker nodes in a Kinetica cluster
/// for direct multi-head ingest/retrieval operations. Worker URLs are stored
/// in rank order, starting with rank 1, and all worker ranks must be included;
/// otherwise operations may fail for certain data types.
///
/// Note: Some ranks may be "removed" (represented as null entries). Use
/// <see cref="GetActiveUrls"/> or iterate with null checks when processing workers.
/// </summary>
public sealed class WorkerList : IReadOnlyList<Uri?>
{
    private readonly List<Uri?> _urls;

    /// <summary>
    /// Whether multi-head I/O is enabled on the server.
    /// </summary>
    public bool IsMultiHeadEnabled { get; private set; }

    /// <summary>
    /// The IP regex used to filter worker URLs, if one was specified.
    /// </summary>
    public Regex? IpRegex { get; private set; }

    /// <summary>
    /// Whether this worker list was created by querying the server.
    /// </summary>
    public bool IsQueriedUrlList { get; private set; }

    /// <summary>
    /// Gets the number of workers in the list (including removed ranks).
    /// </summary>
    public int Count => _urls.Count;

    /// <summary>
    /// Gets the URL at the specified index.
    /// Returns null if the rank was removed or index is out of bounds.
    /// </summary>
    public Uri? this[int index] => index >= 0 && index < _urls.Count ? _urls[index] : null;

    /// <summary>
    /// Creates an empty <see cref="WorkerList"/> that can be populated manually
    /// with worker URLs to support multi-head operations.
    ///
    /// Note that worker URLs must be added in rank order, starting with rank 1,
    /// and all worker ranks must be included; otherwise operations may fail
    /// for certain data types.
    /// </summary>
    public WorkerList()
    {
        _urls = new List<Uri?>();
        IsMultiHeadEnabled = false;
        IpRegex = null;
        IsQueriedUrlList = false;
    }

    /// <summary>
    /// Creates a <see cref="WorkerList"/> and automatically populates it with the
    /// worker URLs from Kinetica to support multi-head operations.
    ///
    /// If the specified Kinetica instance has multi-head operations disabled,
    /// the worker list will be empty and multi-head operations will not be used.
    ///
    /// Note that in some cases, workers may be configured to use more than one IP
    /// address, not all of which may be accessible to the client; this
    /// constructor uses the first IP returned by the server for each worker.
    /// To override this behavior, use the overload with an IP regex parameter.
    /// </summary>
    /// <param name="db">The <see cref="Kinetica"/> instance from which to
    /// obtain the worker URLs.</param>
    public WorkerList(Kinetica db) : this(db, (Regex?)null)
    {
    }

    /// <summary>
    /// Creates a <see cref="WorkerList"/> and automatically populates it with the
    /// worker URLs from Kinetica to support multi-head operations.
    ///
    /// If the specified Kinetica instance has multi-head operations disabled,
    /// the worker list will be empty and multi-head operations will not be used.
    ///
    /// Note that in some cases, workers may be configured to use more than one IP
    /// address, not all of which may be accessible to the client; the
    /// optional <paramref name="ip_regex"/> parameter can be used in such cases
    /// to filter for an IP range that is accessible, e.g., a regex of
    /// <c>192\.168\..*</c> will use worker IP addresses in the 192.168.* range.
    /// </summary>
    /// <param name="db">The <see cref="Kinetica"/> instance from which to
    /// obtain the worker URLs.</param>
    /// <param name="ip_regex">Optional IP regex to match.</param>
    public WorkerList(Kinetica db, Regex? ip_regex)
    {
        _urls = new List<Uri?>();
        IpRegex = ip_regex;
        IsQueriedUrlList = true;
        IsMultiHeadEnabled = false;

        if (db == null)
            throw new ArgumentNullException(nameof(db));

        // Check if auto-discovery is enabled on the connection
        // If HAManager exists and has auto-discovery disabled, return empty list
        if (db.HAManager?.DisableAutoDiscovery == true)
        {
            return;
        }

        // Get the system properties from the database server
        IDictionary<string, string> system_properties = db.showSystemProperties().property_map;

        // Find out if multi-head ingest is turned on or not
        if (!system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS, out string? multi_head_ingestion_param))
            throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS);

        bool is_multi_head_ingest_enabled = multi_head_ingestion_param?.Equals(ShowSystemPropertiesResponse.PropertyMap.TRUE, StringComparison.OrdinalIgnoreCase) ?? false;

        // Nothing to do if multi-head ingestion is disabled
        if (!is_multi_head_ingest_enabled)
        {
            return;
        }

        IsMultiHeadEnabled = true;

        // Multi-head ingestion IS enabled; find the worker URLs
        if (system_properties.TryGetValue("conf.worker_http_server_urls", out string? worker_urls_str) && !string.IsNullOrEmpty(worker_urls_str))
        {
            ParseWorkerUrls(worker_urls_str);
        }
        else
        {
            // Fall back to IPs and ports
            ParseWorkerIpsAndPorts(system_properties, db);
        }

        // Check that this list is not empty (but only if no removed ranks exist)
        if (_urls.Count == 0)
            throw new KineticaException("No worker HTTP servers found.");
    }

    /// <summary>
    /// Creates a <see cref="WorkerList"/> from explicit URLs.
    /// </summary>
    /// <param name="urls">List of URLs to use for worker ranks.</param>
    /// <returns>A new WorkerList containing the specified URLs.</returns>
    public static WorkerList FromUrls(IEnumerable<Uri> urls)
    {
        var workerList = new WorkerList();
        foreach (var url in urls)
        {
            workerList._urls.Add(url);
        }
        return workerList;
    }

    /// <summary>
    /// Creates a <see cref="WorkerList"/> and automatically populates it with the
    /// worker URLs from Kinetica, filtering by IP prefix.
    ///
    /// The <paramref name="ip_prefix"/> parameter can be used to filter for
    /// an IP range that is accessible, e.g., a prefix of <c>192.168.</c> will
    /// use worker IP addresses in the 192.168.* range.
    /// </summary>
    /// <param name="db">The <see cref="Kinetica"/> instance from which to
    /// obtain the worker URLs.</param>
    /// <param name="ip_prefix">IP prefix to match (e.g., "192.168.").</param>
    /// <returns>A new WorkerList containing matching worker URLs.</returns>
    public static WorkerList WithIpPrefix(Kinetica db, string? ip_prefix)
    {
        if (string.IsNullOrEmpty(ip_prefix))
        {
            return new WorkerList(db);
        }
        var regex = new Regex("^" + Regex.Escape(ip_prefix) + ".*");
        return new WorkerList(db, regex);
    }

    /// <summary>
    /// Parses worker URLs from the conf.worker_http_server_urls property.
    /// Format: "url1,url2;url3,url4;..." where semicolons separate ranks
    /// and commas separate alternate URLs for the same rank.
    /// </summary>
    private void ParseWorkerUrls(string worker_urls_str)
    {
        string[] worker_url_lists = worker_urls_str.Split(';');

        // Skip rank 0 (index 0), start from rank 1
        for (int i = 1; i < worker_url_lists.Length; ++i)
        {
            string url_list = worker_url_lists[i];

            // Handle removed ranks (empty string)
            if (string.IsNullOrEmpty(url_list))
            {
                _urls.Add(null);
                continue;
            }

            // Need to split each of the URL lists on a comma
            string[] urls = url_list.Split(',');

            bool matching_url_found = false;

            // Find at least one URL to work with
            foreach (string url_str in urls)
            {
                try
                {
                    // If a regular expression is given, then see if this one is a match
                    if (IpRegex != null)
                    {
                        // Extract host from URL for matching
                        var tempUri = new Uri(url_str);
                        matching_url_found = IpRegex.IsMatch(tempUri.Host);
                    }
                    else
                    {
                        // No regex given, so take the first URL encountered for this worker
                        matching_url_found = true;
                    }

                    if (matching_url_found)
                    {
                        Uri url = new Uri(url_str);
                        _urls.Add(url);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new KineticaException($"Invalid URL '{url_str}': {ex.Message}");
                }
            }

            if (!matching_url_found)
                throw new KineticaException($"No matching URL found for worker #{i}.");
        }
    }

    /// <summary>
    /// Parses worker IPs and ports from the conf.worker_http_server_ips and
    /// conf.worker_http_server_ports properties.
    /// </summary>
    private void ParseWorkerIpsAndPorts(IDictionary<string, string> system_properties, Kinetica db)
    {
        if (!system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS, out string? worker_ips_str) || string.IsNullOrEmpty(worker_ips_str))
            throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS);

        if (!system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS, out string? worker_ports_str) || string.IsNullOrEmpty(worker_ports_str))
            throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS);

        // Split the strings
        string[] worker_ip_lists = worker_ips_str.Split(';');
        string[] worker_ports = worker_ports_str.Split(';');

        // Check that there are the same number of IPs and ports supplied
        if (worker_ip_lists.Length != worker_ports.Length)
            throw new KineticaException("Inconsistent number of values for "
                                         + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS
                                         + " and "
                                         + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS);

        // Get the protocol from the database URL
        string scheme = db.URL.Scheme;

        // Skip rank 0 (index 0), start from rank 1
        for (int i = 1; i < worker_ip_lists.Length; ++i)
        {
            string ip_list = worker_ip_lists[i];

            // Handle removed ranks (empty string)
            if (string.IsNullOrEmpty(ip_list))
            {
                _urls.Add(null);
                continue;
            }

            // Need to split each of the IP lists on a comma
            string[] ips = ip_list.Split(',');

            bool matching_ip_found = false;

            // Find at least one IP to work with
            foreach (string ip in ips)
            {
                try
                {
                    // If a regular expression is given, then see if this one is a match
                    if (IpRegex != null)
                        matching_ip_found = IpRegex.IsMatch(ip);
                    else
                        matching_ip_found = true;

                    if (matching_ip_found)
                    {
                        UriBuilder uri_builder = new UriBuilder(scheme, ip, int.Parse(worker_ports[i]));
                        Uri url = uri_builder.Uri;
                        _urls.Add(url);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new KineticaException($"Error creating URL for worker #{i}: {ex.Message}");
                }
            }

            if (!matching_ip_found)
                throw new KineticaException($"No matching IP found for worker #{i}.");
        }
    }

    // ==================== Mutation Methods ====================

    /// <summary>
    /// Adds a URL to the worker list.
    /// </summary>
    /// <param name="url">The URL to add.</param>
    public void Add(Uri url)
    {
        _urls.Add(url);
    }

    /// <summary>
    /// Adds a URL or null (for removed rank) to the worker list.
    /// </summary>
    /// <param name="url">The URL to add, or null for a removed rank.</param>
    public void AddOrNull(Uri? url)
    {
        _urls.Add(url);
    }

    /// <summary>
    /// Clears all URLs from the worker list.
    /// </summary>
    public void Clear()
    {
        _urls.Clear();
    }

    // ==================== Query Methods ====================

    /// <summary>
    /// Returns whether the worker list is empty.
    /// </summary>
    public bool IsEmpty => _urls.Count == 0;

    /// <summary>
    /// Gets all active (non-null) URLs in the worker list.
    /// </summary>
    /// <returns>An enumerable of active worker URLs.</returns>
    public IEnumerable<Uri> GetActiveUrls()
    {
        return _urls.Where(u => u != null).Cast<Uri>();
    }

    /// <summary>
    /// Gets the number of active (non-null) workers in the list.
    /// </summary>
    public int ActiveCount => _urls.Count(u => u != null);

    /// <summary>
    /// Converts the worker list to a list of URL strings.
    /// Only includes active (non-null) URLs.
    /// </summary>
    /// <returns>A list of URL strings for all active workers.</returns>
    public List<string> ToUrlStrings()
    {
        return _urls
            .Where(u => u != null)
            .Select(u => u!.ToString())
            .ToList();
    }

    /// <summary>
    /// Gets the URL at the specified index, or null if the rank was removed
    /// or index is out of bounds.
    /// </summary>
    /// <param name="index">The index of the worker URL to get (0-based, corresponds to rank-1).</param>
    /// <returns>The URL at the specified index, or null.</returns>
    public Uri? Get(int index)
    {
        return index >= 0 && index < _urls.Count ? _urls[index] : null;
    }

    // ==================== IReadOnlyList Implementation ====================

    /// <summary>
    /// Returns an enumerator that iterates through the worker URLs.
    /// Note: May include null entries for removed ranks.
    /// </summary>
    public IEnumerator<Uri?> GetEnumerator()
    {
        return _urls.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ==================== Backward Compatibility ====================

    /// <summary>
    /// Returns an enumerator that iterates through only active (non-null) worker URLs.
    /// This provides backward compatibility with code that expects to iterate
    /// over a List&lt;Uri&gt;.
    /// </summary>
    public IEnumerable<Uri> GetActiveUrlsEnumerator()
    {
        return GetActiveUrls();
    }
}
