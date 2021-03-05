/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showGraph(string,IDictionary{string, string})" />.
    /// <br />
    /// Shows information and characteristics of graphs that exist on the graph
    /// server.</summary>
    public class ShowGraphRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SHOW_ORIGINAL_REQUEST">SHOW_ORIGINAL_REQUEST</see>:</term>
        ///         <description>If set to <i>true</i>, the request that was
        /// originally used to create the graph is also returned as JSON.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to get information about all the
        /// servers.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, the request that was originally
            /// used to create the graph is also returned as JSON.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowGraphRequest.Options.TRUE">TRUE</see>.</summary>
            public const string SHOW_ORIGINAL_REQUEST = "show_original_request";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Indicates which graph server(s) to send the request
            /// to. Default is to send to get information about all the
            /// servers.</summary>
            public const string SERVER_ID = "server_id";
        } // end struct Options


        /// <summary>Name of the graph on which to retrieve information. If
        /// left as the default value, information about all graphs is
        /// returned.  The default value is ''.</summary>
        public string graph_name { get; set; } = "";

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SHOW_ORIGINAL_REQUEST">SHOW_ORIGINAL_REQUEST</see>:</term>
        ///         <description>If set to <i>true</i>, the request that was
        /// originally used to create the graph is also returned as JSON.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to get information about all the
        /// servers.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowGraphRequest object with default
        /// parameters.</summary>
        public ShowGraphRequest() { }

        /// <summary>Constructs a ShowGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name">Name of the graph on which to retrieve
        /// information. If left as the default value, information about all
        /// graphs is returned.  The default value is ''.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SHOW_ORIGINAL_REQUEST">SHOW_ORIGINAL_REQUEST</see>:</term>
        ///         <description>If set to <i>true</i>, the request that was
        /// originally used to create the graph is also returned as JSON.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to get information about all the
        /// servers.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowGraphRequest( string graph_name = null,
                                 IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowGraphRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showGraph(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowGraphResponse : KineticaData
    {

        /// <summary>Indicates a success. This call will fails of the graph
        /// specified in the request does not exist.  </summary>
        public bool result { get; set; }

        /// <summary>A percentage approximating the current computational load
        /// on the server.  </summary>
        public IList<int> load { get; set; } = new List<int>();

        /// <summary>Available memory.  </summary>
        public IList<long> memory { get; set; } = new List<long>();

        /// <summary>Name(s) of the graph(s).  </summary>
        public IList<string> graph_names { get; set; } = new List<string>();

        /// <summary>Id(s) of the graph(s).  </summary>
        public IList<int> graph_server_ids { get; set; } = new List<int>();

        /// <summary>Whether or not the edges of the graph have directions
        /// (bi-directional edges can still exist in directed graphs). Consult
        /// <a
        /// href="../../../graph_solver/network_graph_solver/#directed-graphs"
        /// target="_top">Directed Graphs</a> for more details.  </summary>
        public IList<bool> directed { get; set; } = new List<bool>();

        /// <summary>Total number of nodes in the graph.  </summary>
        public IList<long> num_nodes { get; set; } = new List<long>();

        /// <summary>Total number of edges in the graph.  </summary>
        public IList<long> num_edges { get; set; } = new List<long>();

        /// <summary>Shows whether or not the graph is persisted (saved and
        /// loaded on launch).  </summary>
        public IList<bool> is_persisted { get; set; } = new List<bool>();

        /// <summary>Shows whether or not the graph is linked to the original
        /// tables that created it, and will potentially be re-created instead
        /// loaded from persist on launch.  </summary>
        public IList<bool> is_sync_db { get; set; } = new List<bool>();

        /// <summary>Shows whether or not the graph has an insert table monitor
        /// attached to it.  </summary>
        public IList<bool> has_insert_table_monitor { get; set; } = new List<bool>();

        /// <summary>The orignal client request used to create the graph
        /// (before any expression evaluation or separator processing).
        /// </summary>
        public IList<string> original_request { get; set; } = new List<string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowGraphResponse




}  // end namespace kinetica
