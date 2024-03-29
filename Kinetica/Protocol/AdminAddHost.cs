/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminAddHost(AdminAddHostRequest)">Kinetica.adminAddHost</see>.
    /// </summary>
    /// <remarks><para>Adds a host to an existing cluster.</para></remarks>
    public class AdminAddHostRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>If set to <see cref="Options.TRUE">TRUE</see>, only
            /// validation checks will be performed.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string DRY_RUN = "dry_run";

            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If set to <see cref="Options.TRUE">TRUE</see>, the
            /// host will accept processes (ranks, graph server, etc.) in the
            /// event of a failover on another node in the cluster.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string ACCEPTS_FAILOVER = "accepts_failover";

            /// <summary>The publicly-accessible IP address for the host being
            /// added, typically specified for clients using multi-head
            /// operations.</summary>
            /// <remarks><para>This setting is required if any other host(s) in
            /// the cluster specify a public address.</para></remarks>
            public const string PUBLIC_ADDRESS = "public_address";

            /// <summary>The publicly-accessible full path URL to the host
            /// manager on the host being added, e.g.,
            /// 'http://172.123.45.67:9300'.</summary>
            /// <remarks><para>The default host manager port can be found in
            /// the <a href="../../../install/shared/ports/" target="_top">list
            /// of ports</a> used by Kinetica.</para></remarks>
            public const string HOST_MANAGER_PUBLIC_URL = "host_manager_public_url";

            /// <summary>The desired RAM limit for the host being added, i.e.
            /// the sum of RAM usage for all processes on the host will not be
            /// able to exceed this value.</summary>
            /// <remarks><para>Supported units: K (thousand), KB (kilobytes), M
            /// (million), MB (megabytes), G (billion), GB (gigabytes); if no
            /// unit is provided, the value is assumed to be in bytes. For
            /// example, if <see cref="Options.RAM_LIMIT">RAM_LIMIT</see> is
            /// set to 10M, the resulting RAM limit is 10 million bytes. Set
            /// <see cref="Options.RAM_LIMIT">RAM_LIMIT</see> to -1 to have no
            /// RAM limit.</para></remarks>
            public const string RAM_LIMIT = "ram_limit";

            /// <summary>Comma-delimited list of GPU indices (starting at 1)
            /// that are eligible for running worker processes.</summary>
            /// <remarks><para>If left blank, all GPUs on the host being added
            /// will be eligible.</para></remarks>
            public const string GPUS = "gpus";
        } // end struct Options

        /// <summary>IP address of the host that will be added to the cluster.
        /// </summary>
        /// <remarks><para>This host must have installed the same version of
        /// Kinetica as the cluster to which it is being added.</para>
        /// </remarks>
        public string host_address { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <see cref="Options.TRUE">TRUE</see>,
        ///         only validation checks will be performed. No host is added.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.ACCEPTS_FAILOVER">ACCEPTS_FAILOVER</see>:
        ///         </term>
        ///         <description>If set to <see cref="Options.TRUE">TRUE</see>,
        ///         the host will accept processes (ranks, graph server, etc.)
        ///         in the event of a failover on another node in the cluster.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.PUBLIC_ADDRESS">PUBLIC_ADDRESS</see>:</term>
        ///         <description>The publicly-accessible IP address for the
        ///         host being added, typically specified for clients using
        ///         multi-head operations. This setting is required if any
        ///         other host(s) in the cluster specify a public address.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.HOST_MANAGER_PUBLIC_URL">HOST_MANAGER_PUBLIC_URL</see>:
        ///         </term>
        ///         <description>The publicly-accessible full path URL to the
        ///         host manager on the host being added, e.g.,
        ///         'http://172.123.45.67:9300'. The default host manager port
        ///         can be found in the <a
        ///         href="../../../install/shared/ports/" target="_top">list of
        ///         ports</a> used by Kinetica.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.RAM_LIMIT">RAM_LIMIT</see>:</term>
        ///         <description>The desired RAM limit for the host being
        ///         added, i.e. the sum of RAM usage for all processes on the
        ///         host will not be able to exceed this value. Supported
        ///         units: K (thousand), KB (kilobytes), M (million), MB
        ///         (megabytes), G (billion), GB (gigabytes); if no unit is
        ///         provided, the value is assumed to be in bytes. For example,
        ///         if <see cref="Options.RAM_LIMIT">RAM_LIMIT</see> is set to
        ///         10M, the resulting RAM limit is 10 million bytes. Set <see
        ///         cref="Options.RAM_LIMIT">RAM_LIMIT</see> to -1 to have no
        ///         RAM limit.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.GPUS">GPUS</see>:</term>
        ///         <description>Comma-delimited list of GPU indices (starting
        ///         at 1) that are eligible for running worker processes. If
        ///         left blank, all GPUs on the host being added will be
        ///         eligible.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
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
        ///         <term><see cref="Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <see cref="Options.TRUE">TRUE</see>,
        ///         only validation checks will be performed. No host is added.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.ACCEPTS_FAILOVER">ACCEPTS_FAILOVER</see>:
        ///         </term>
        ///         <description>If set to <see cref="Options.TRUE">TRUE</see>,
        ///         the host will accept processes (ranks, graph server, etc.)
        ///         in the event of a failover on another node in the cluster.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.PUBLIC_ADDRESS">PUBLIC_ADDRESS</see>:</term>
        ///         <description>The publicly-accessible IP address for the
        ///         host being added, typically specified for clients using
        ///         multi-head operations. This setting is required if any
        ///         other host(s) in the cluster specify a public address.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.HOST_MANAGER_PUBLIC_URL">HOST_MANAGER_PUBLIC_URL</see>:
        ///         </term>
        ///         <description>The publicly-accessible full path URL to the
        ///         host manager on the host being added, e.g.,
        ///         'http://172.123.45.67:9300'. The default host manager port
        ///         can be found in the <a
        ///         href="../../../install/shared/ports/" target="_top">list of
        ///         ports</a> used by Kinetica.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.RAM_LIMIT">RAM_LIMIT</see>:</term>
        ///         <description>The desired RAM limit for the host being
        ///         added, i.e. the sum of RAM usage for all processes on the
        ///         host will not be able to exceed this value. Supported
        ///         units: K (thousand), KB (kilobytes), M (million), MB
        ///         (megabytes), G (billion), GB (gigabytes); if no unit is
        ///         provided, the value is assumed to be in bytes. For example,
        ///         if <see cref="Options.RAM_LIMIT">RAM_LIMIT</see> is set to
        ///         10M, the resulting RAM limit is 10 million bytes. Set <see
        ///         cref="Options.RAM_LIMIT">RAM_LIMIT</see> to -1 to have no
        ///         RAM limit.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.GPUS">GPUS</see>:</term>
        ///         <description>Comma-delimited list of GPU indices (starting
        ///         at 1) that are eligible for running worker processes. If
        ///         left blank, all GPUs on the host being added will be
        ///         eligible.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public AdminAddHostRequest( string host_address,
                                    IDictionary<string, string> options = null)
        {
            this.host_address = host_address ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AdminAddHostRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminAddHost(AdminAddHostRequest)">Kinetica.adminAddHost</see>.
    /// </summary>
    public class AdminAddHostResponse : KineticaData
    {
        /// <summary>Identifier for the newly added host, of the format 'hostN'
        /// where N is the integer identifier of that host.</summary>
        /// <remarks><para>Note that the host identifier is transient, i.e. it
        /// may change in the future if other hosts are removed.</para>
        /// </remarks>
        public string added_host { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AdminAddHostResponse
} // end namespace kinetica
