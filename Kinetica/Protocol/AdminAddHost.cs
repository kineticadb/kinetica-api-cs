/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminAddHost(string,IDictionary{string, string})" />.
    /// <br />
    /// Adds a host to an existing cluster.</summary>
    public class AdminAddHostRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is added.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.ACCEPTS_FAILOVER">ACCEPTS_FAILOVER</see>:</term>
        ///         <description>If set to <i>true</i>, the host will accept
        /// processes (ranks, graph server, etc.) in the event of a failover on
        /// another node in the cluster. See <a href="../../../n_plus_1/"
        /// target="_top">Cluster Resilience</a> for more information.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.PUBLIC_ADDRESS">PUBLIC_ADDRESS</see>:</term>
        ///         <description>The publicly-accessible IP address for the
        /// host being added, typically specified for clients using multi-head
        /// operations. This setting is required if any other host(s) in the
        /// cluster specify a public address.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.HOST_MANAGER_PUBLIC_URL">HOST_MANAGER_PUBLIC_URL</see>:</term>
        ///         <description>The publicly-accessible full path URL to the
        /// host manager on the host being added, e.g.,
        /// 'http://172.123.45.67:9300'. The default host manager port can be
        /// found in the <a href="../../../install/shared/ports/"
        /// target="_top">list of ports</a> used by Kinetica.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.RAM_LIMIT">RAM_LIMIT</see>:</term>
        ///         <description>The desired RAM limit for the host being
        /// added, i.e. the sum of RAM usage for all processes on the host will
        /// not be able to exceed this value. Supported units: K (thousand), KB
        /// (kilobytes), M (million), MB (megabytes), G (billion), GB
        /// (gigabytes); if no unit is provided, the value is assumed to be in
        /// bytes. For example, if <i>ram_limit</i> is set to 10M, the
        /// resulting RAM limit is 10 million bytes. Set <i>ram_limit</i> to -1
        /// to have no RAM limit.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.GPUS">GPUS</see>:</term>
        ///         <description>Comma-delimited list of GPU indices (starting
        /// at 1) that are eligible for running worker processes. If left
        /// blank, all GPUs on the host being added will be
        /// eligible.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, only validation checks will be
            /// performed. No host is added.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</summary>
            public const string DRY_RUN = "dry_run";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If set to <i>true</i>, the host will accept processes
            /// (ranks, graph server, etc.) in the event of a failover on
            /// another node in the cluster. See <a href="../../../n_plus_1/"
            /// target="_top">Cluster Resilience</a> for more information.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</summary>
            public const string ACCEPTS_FAILOVER = "accepts_failover";

            /// <summary>The publicly-accessible IP address for the host being
            /// added, typically specified for clients using multi-head
            /// operations. This setting is required if any other host(s) in
            /// the cluster specify a public address.</summary>
            public const string PUBLIC_ADDRESS = "public_address";

            /// <summary>The publicly-accessible full path URL to the host
            /// manager on the host being added, e.g.,
            /// 'http://172.123.45.67:9300'. The default host manager port can
            /// be found in the <a href="../../../install/shared/ports/"
            /// target="_top">list of ports</a> used by Kinetica.</summary>
            public const string HOST_MANAGER_PUBLIC_URL = "host_manager_public_url";

            /// <summary>The desired RAM limit for the host being added, i.e.
            /// the sum of RAM usage for all processes on the host will not be
            /// able to exceed this value. Supported units: K (thousand), KB
            /// (kilobytes), M (million), MB (megabytes), G (billion), GB
            /// (gigabytes); if no unit is provided, the value is assumed to be
            /// in bytes. For example, if <i>ram_limit</i> is set to 10M, the
            /// resulting RAM limit is 10 million bytes. Set <i>ram_limit</i>
            /// to -1 to have no RAM limit.</summary>
            public const string RAM_LIMIT = "ram_limit";

            /// <summary>Comma-delimited list of GPU indices (starting at 1)
            /// that are eligible for running worker processes. If left blank,
            /// all GPUs on the host being added will be eligible.</summary>
            public const string GPUS = "gpus";
        } // end struct Options


        /// <summary>IP address of the host that will be added to the cluster.
        /// This host must have installed the same version of Kinetica as the
        /// cluster to which it is being added.  </summary>
        public string host_address { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is added.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.ACCEPTS_FAILOVER">ACCEPTS_FAILOVER</see>:</term>
        ///         <description>If set to <i>true</i>, the host will accept
        /// processes (ranks, graph server, etc.) in the event of a failover on
        /// another node in the cluster. See <a href="../../../n_plus_1/"
        /// target="_top">Cluster Resilience</a> for more information.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.PUBLIC_ADDRESS">PUBLIC_ADDRESS</see>:</term>
        ///         <description>The publicly-accessible IP address for the
        /// host being added, typically specified for clients using multi-head
        /// operations. This setting is required if any other host(s) in the
        /// cluster specify a public address.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.HOST_MANAGER_PUBLIC_URL">HOST_MANAGER_PUBLIC_URL</see>:</term>
        ///         <description>The publicly-accessible full path URL to the
        /// host manager on the host being added, e.g.,
        /// 'http://172.123.45.67:9300'. The default host manager port can be
        /// found in the <a href="../../../install/shared/ports/"
        /// target="_top">list of ports</a> used by Kinetica.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.RAM_LIMIT">RAM_LIMIT</see>:</term>
        ///         <description>The desired RAM limit for the host being
        /// added, i.e. the sum of RAM usage for all processes on the host will
        /// not be able to exceed this value. Supported units: K (thousand), KB
        /// (kilobytes), M (million), MB (megabytes), G (billion), GB
        /// (gigabytes); if no unit is provided, the value is assumed to be in
        /// bytes. For example, if <i>ram_limit</i> is set to 10M, the
        /// resulting RAM limit is 10 million bytes. Set <i>ram_limit</i> to -1
        /// to have no RAM limit.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.GPUS">GPUS</see>:</term>
        ///         <description>Comma-delimited list of GPU indices (starting
        /// at 1) that are eligible for running worker processes. If left
        /// blank, all GPUs on the host being added will be
        /// eligible.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminAddHostRequest object with default
        /// parameters.</summary>
        public AdminAddHostRequest() { }

        /// <summary>Constructs an AdminAddHostRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="host_address">IP address of the host that will be
        /// added to the cluster. This host must have installed the same
        /// version of Kinetica as the cluster to which it is being added.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is added.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.ACCEPTS_FAILOVER">ACCEPTS_FAILOVER</see>:</term>
        ///         <description>If set to <i>true</i>, the host will accept
        /// processes (ranks, graph server, etc.) in the event of a failover on
        /// another node in the cluster. See <a href="../../../n_plus_1/"
        /// target="_top">Cluster Resilience</a> for more information.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminAddHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.PUBLIC_ADDRESS">PUBLIC_ADDRESS</see>:</term>
        ///         <description>The publicly-accessible IP address for the
        /// host being added, typically specified for clients using multi-head
        /// operations. This setting is required if any other host(s) in the
        /// cluster specify a public address.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.HOST_MANAGER_PUBLIC_URL">HOST_MANAGER_PUBLIC_URL</see>:</term>
        ///         <description>The publicly-accessible full path URL to the
        /// host manager on the host being added, e.g.,
        /// 'http://172.123.45.67:9300'. The default host manager port can be
        /// found in the <a href="../../../install/shared/ports/"
        /// target="_top">list of ports</a> used by Kinetica.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.RAM_LIMIT">RAM_LIMIT</see>:</term>
        ///         <description>The desired RAM limit for the host being
        /// added, i.e. the sum of RAM usage for all processes on the host will
        /// not be able to exceed this value. Supported units: K (thousand), KB
        /// (kilobytes), M (million), MB (megabytes), G (billion), GB
        /// (gigabytes); if no unit is provided, the value is assumed to be in
        /// bytes. For example, if <i>ram_limit</i> is set to 10M, the
        /// resulting RAM limit is 10 million bytes. Set <i>ram_limit</i> to -1
        /// to have no RAM limit.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminAddHostRequest.Options.GPUS">GPUS</see>:</term>
        ///         <description>Comma-delimited list of GPU indices (starting
        /// at 1) that are eligible for running worker processes. If left
        /// blank, all GPUs on the host being added will be
        /// eligible.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AdminAddHostRequest( string host_address,
                                    IDictionary<string, string> options = null)
        {
            this.host_address = host_address ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminAddHostRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminAddHost(string,IDictionary{string, string})"
    /// />.</summary>
    public class AdminAddHostResponse : KineticaData
    {

        /// <summary>Identifier for the newly added host, of the format 'hostN'
        /// where N is the integer identifier of that host. Note that the host
        /// identifier is transient, i.e. it may change in the future if other
        /// hosts are removed.  </summary>
        public string added_host { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AdminAddHostResponse




}  // end namespace kinetica
