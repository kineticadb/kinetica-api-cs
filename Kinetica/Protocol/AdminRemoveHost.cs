/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminRemoveHost(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Removes a host from an existing cluster. If the host to be removed has
    /// any ranks running on it, the ranks must be removed using <see
    /// cref="Kinetica.adminRemoveRanks(IList{string},IDictionary{string, string})"
    /// /> or manually switched over to a new host using <see
    /// cref="Kinetica.adminSwitchover(IList{string},IList{string},IDictionary{string, string})"
    /// /> prior to host removal. If the host to be removed has the graph
    /// server or SQL planner running on it, these must be manually switched
    /// over to a new host using <see
    /// cref="Kinetica.adminSwitchover(IList{string},IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class AdminRemoveHostRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is removed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, only validation checks will be
            /// performed. No host is removed.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminRemoveHostRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see>.</summary>
            public const string DRY_RUN = "dry_run";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Identifies the host this applies to. Can be the host
        /// address, or formatted as 'hostN' where N is the host number as
        /// specified in gpudb.conf  </summary>
        public string host { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is removed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminRemoveHostRequest object with default
        /// parameters.</summary>
        public AdminRemoveHostRequest() { }

        /// <summary>Constructs an AdminRemoveHostRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="host">Identifies the host this applies to. Can be the
        /// host address, or formatted as 'hostN' where N is the host number as
        /// specified in gpudb.conf  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.DRY_RUN">DRY_RUN</see>:</term>
        ///         <description>If set to <i>true</i>, only validation checks
        /// will be performed. No host is removed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminRemoveHostRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AdminRemoveHostRequest( string host,
                                       IDictionary<string, string> options = null)
        {
            this.host = host ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminRemoveHostRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminRemoveHost(string,IDictionary{string, string})"
    /// />.</summary>
    public class AdminRemoveHostResponse : KineticaData
    {

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AdminRemoveHostResponse




}  // end namespace kinetica
