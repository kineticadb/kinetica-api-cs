using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace kinetica.Utils
{
    /// <summary>
    /// A list of worker URLs to use for multi-head ingest.
    /// </summary>
    public sealed class WorkerList : List<System.Uri>
    {
        /// <summary>
        /// Creates an empty {@link WorkerList} that can be populated manually
        /// with worker URLs to support multi-head ingest.Note that worker URLs
        /// must be added in rank order, starting with rank 1, and all worker
        /// ranks must be included; otherwise insertion may fail for certain
        /// data types.
        /// </summary>
        public WorkerList() { }

        /// <summary>
        /// Creates a <see cref="WorkerList"/> object and automatically populates it with the
        /// worker URLs from GPUdb to support multi-head ingest. ( If the
        /// specified GPUdb instance has multi-head ingest disabled, the worker
        /// list will be empty and multi-head ingest will not be used.) Note that
        /// in some cases, workers may be configured to use more than one IP
        /// address, not all of which may be accessible to the client; this
        /// constructor uses the first IP returned by the server for each worker.
        /// </summary>
        /// 
        /// <param name="db">The <see cref="Kinetica"/> instance from which to
        /// obtain the worker URLs.</param>
        /// <param name="ip_regex">Optional IP regex to match.</param>
        public WorkerList(Kinetica db, Regex ip_regex = null)
        {
            // Get the system properties from the database server
            IDictionary<string, string> system_properties = db.showSystemProperties().property_map;

            // Find out if multi-head ingest is turned on or not
            string multi_head_ingestion_param;
            system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS, out multi_head_ingestion_param);
            if (multi_head_ingestion_param == null)
                throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS);
            bool is_multi_head_ingest_enabled = multi_head_ingestion_param.Equals(ShowSystemPropertiesResponse.PropertyMap.TRUE);

            // Nothing to do if multi-head ingestion is disabled
            if (!is_multi_head_ingest_enabled)
            {
                return;
            }

            // Multi-head ingestion IS enabled; find the worker URLs; then
            // add them to the worker url vector.
            // -----------------------------------------------------------
            string worker_urls_str;
            system_properties.TryGetValue("conf.worker_http_server_urls", out worker_urls_str);
            if ( worker_urls_str.Length > 0 )  // found some URLs
            {
                // Parse the URLs
                // --------------
                // Split the strings
                string[] worker_url_lists = worker_urls_str.Split(';');

                // Ignoring the very first rank (rank-0), add all matching
                // URLs (if any regex is given), or just add the first one
                for (int i = 1; i < worker_url_lists.Length; ++i)
                {
                    string url_list = worker_url_lists[i];

                    // Need to split each of the URL lists on a comma
                    string[] urls = url_list.Split(',');

                    bool matching_url_found = false;

                    // Find at least one URL to work with
                    foreach (string url_str in urls)
                    {
                        // Try to create the URL
                        try
                        {
                            // If a regular expression is given, then see if this one is a match
                            if (ip_regex != null)
                                matching_url_found = ip_regex.IsMatch(url_str);
                            else  // no regex given, so take the first URL encountered for this worker
                                matching_url_found = true;

                            if (matching_url_found)
                            {
                                Uri url = new Uri( url_str );

                                // Add the URL to this WorkerList
                                this.Add(url);
                                break;  // don't keep trying to match URLs in this group
                            }  // end inner if
                        }  // end try
                        catch (Exception ex)
                        {
                            throw new KineticaException(ex.Message);
                        }
                    }  // end inner foreach

                    if (!matching_url_found)
                        throw new KineticaException($"No matching URL found for worker #{i}.");
                }  // end outer for
            }
            else // construct the URLs from IP addresses and ports
            {
                // Get the worker IPs and ports
                string worker_ips_str, worker_ports_str;
                system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS, out worker_ips_str);
                system_properties.TryGetValue(ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS, out worker_ports_str);

                // Check that we got them
                if (worker_ips_str.Length == 0)
                    throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS);
                if (worker_ports_str.Length == 0)
                    throw new KineticaException("Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS);

                // Parse the IPs and the ports
                // ---------------------------
                // Split the strings
                string[] worker_ip_lists = worker_ips_str.Split(';');
                string[] worker_ports = worker_ports_str.Split(';');

                // Check that there are the same number of IPs and ports supplied
                if (worker_ip_lists.Length != worker_ports.Length)
                    throw new KineticaException("Inconsistent number of values for "
                                                 + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS
                                                 + " and "
                                                 + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS);
                // Create the URLs using the IPs and the ports, but
                // ignore the very first rank (rank-0)
                for (int i = 1; i < worker_ip_lists.Length; ++i)
                {
                    string ip_list = worker_ip_lists[i];

                    // Need to split each of the IP lists on a comma
                    string[] ips = ip_list.Split(',');

                    bool matching_ip_found = false;

                    // Find at least one IP to work with
                    foreach (string ip in ips)
                    {
                        // Try to create the URL
                        try
                        {
                            // If a regular expression is given, then see if this one is a match
                            if (ip_regex != null)
                                matching_ip_found = ip_regex.IsMatch(ip);
                            else  // no regex given, so take the first IP encountered for this worker
                                matching_ip_found = true;

                            if (matching_ip_found)
                            {
                                UriBuilder uri_builder = new UriBuilder( db.URL.Scheme,
                                                                         ip, Int32.Parse(worker_ports[i]) );
                                Uri url = uri_builder.Uri;

                                // Add the URL to this WorkerList
                                this.Add(url);
                                break;  // don't keep trying to match IPs in this group
                            }  // end inner if
                        }  // end try
                        catch (Exception ex)
                        {
                            throw new KineticaException(ex.Message);
                        }
                    }  // end inner foreach

                    if (!matching_ip_found)
                        throw new KineticaException($"No matching IP found for worker #{i}.");
                }  // end outer for
            }  // end if-else

            // Check that this list is not empty
            if (this.Count == 0)
                throw new KineticaException("No worker HTTP servers found.");
        }  // end constructor

    }  // end class WorkerList

}   // end namespace kinetica.Utils
