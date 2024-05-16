/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.queryGraph(string,IList{string},IList{string},string,int,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Employs a topological query on a network graph generated a-priori by
    /// <see
    /// cref="Kinetica.createGraph(string,bool,IList{string},IList{string},IList{string},IList{string},IDictionary{string, string})"
    /// /> and returns a list of adjacent edge(s) or node(s),
    /// also known as an adjacency list, depending on what's been provided to
    /// the
    /// endpoint; providing edges will return nodes and providing nodes will
    /// return
    /// edges.
    /// <br />
    /// To determine the node(s) or edge(s) adjacent to a value from a given
    /// column,
    /// provide a list of values to <see cref="queries" />. This field can be
    /// populated with
    /// column values from any table as long as the type is supported by the
    /// given
    /// identifier. See
    /// <a href="../../../graph_solver/network_graph_solver/#query-identifiers"
    /// target="_top">Query Identifiers</a>
    /// for more information.
    /// <br />
    /// To return the adjacency list in the response, leave <see
    /// cref="adjacency_table" />
    /// empty.
    /// <br />
    /// IMPORTANT: It's highly recommended that you review the
    /// <a href="../../../graph_solver/network_graph_solver/"
    /// target="_top">Network Graphs & Solvers</a>
    /// concepts documentation, the
    /// <a href="../../../guides/graph_rest_guide/" target="_top">Graph REST
    /// Tutorial</a>,
    /// and/or some
    /// <a href="../../../guide-tags/graph---query" target="_top">/match/graph
    /// examples</a>
    /// before using this endpoint.</summary>
    public class QueryGraphRequest : KineticaData
    {

        /// <summary>Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FORCE_UNDIRECTED">FORCE_UNDIRECTED</see>:</term>
        ///         <description>If set to <i>true</i>, all inbound edges and
        /// outbound edges relative to the node will be returned. If set to
        /// <i>false</i>, only outbound edges relative to the node will be
        /// returned. This parameter is only applicable if the queried graph
        /// <paramref cref="QueryGraphRequest.graph_name" /> is directed and
        /// when querying nodes. Consult <a
        /// href="../../../graph_solver/network_graph_solver/#directed-graphs"
        /// target="_top">Directed Graphs</a> for more details.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>When specified (>0), limits the number of
        /// query results. The size of the nodes table will be limited by the
        /// <i>limit</i> value.  The default value is '0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_WKT_PATH">OUTPUT_WKT_PATH</see>:</term>
        ///         <description>If true then concatenated wkt line segments
        /// will be added as the WKT column of the adjacency table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.AND_LABELS">AND_LABELS</see>:</term>
        ///         <description>If set to <i>true</i>, the result of the query
        /// has entities that satisfy all of the target labels, instead of any.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to the server, amongst those
        /// containing the corresponding graph, that has the most computational
        /// bandwidth.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_CHARN_LENGTH">OUTPUT_CHARN_LENGTH</see>:</term>
        ///         <description>When specified (>0 and <=256), limits the
        /// number of char length on the output tables for string based nodes.
        /// The default length is 64.  The default value is '64'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FIND_COMMON_LABELS">FIND_COMMON_LABELS</see>:</term>
        ///         <description>If set to true, for many-to-many queries or
        /// multi-level traversals, it lists the common labels between the
        /// source and target nodes and edge labels in each path. Otherwise
        /// (zero rings), it'll list all labels of the node(s) queried.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, all inbound edges and outbound
            /// edges relative to the node will be returned. If set to
            /// <i>false</i>, only outbound edges relative to the node will be
            /// returned. This parameter is only applicable if the queried
            /// graph <see cref="graph_name" /> is directed and when querying
            /// nodes. Consult <a
            /// href="../../../graph_solver/network_graph_solver/#directed-graphs"
            /// target="_top">Directed Graphs</a> for more details.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</summary>
            public const string FORCE_UNDIRECTED = "force_undirected";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>When specified (>0), limits the number of query
            /// results. The size of the nodes table will be limited by the
            /// <i>limit</i> value.  The default value is '0'.</summary>
            public const string LIMIT = "limit";

            /// <summary>If true then concatenated wkt line segments will be
            /// added as the WKT column of the adjacency table.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</summary>
            public const string OUTPUT_WKT_PATH = "output_wkt_path";

            /// <summary>If set to <i>true</i>, the result of the query has
            /// entities that satisfy all of the target labels, instead of any.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</summary>
            public const string AND_LABELS = "and_labels";

            /// <summary>Indicates which graph server(s) to send the request
            /// to. Default is to send to the server, amongst those containing
            /// the corresponding graph, that has the most computational
            /// bandwidth.</summary>
            public const string SERVER_ID = "server_id";

            /// <summary>When specified (>0 and <=256), limits the number of
            /// char length on the output tables for string based nodes. The
            /// default length is 64.  The default value is '64'.</summary>
            public const string OUTPUT_CHARN_LENGTH = "output_charn_length";

            /// <summary>If set to true, for many-to-many queries or
            /// multi-level traversals, it lists the common labels between the
            /// source and target nodes and edge labels in each path. Otherwise
            /// (zero rings), it'll list all labels of the node(s) queried.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</summary>
            public const string FIND_COMMON_LABELS = "find_common_labels";
        } // end struct Options


        /// <summary>Name of the graph resource to query.  </summary>
        public string graph_name { get; set; }

        /// <summary>Nodes or edges to be queried specified using <a
        /// href="../../../graph_solver/network_graph_solver/#query-identifiers"
        /// target="_top">query identifiers</a>. Identifiers can be used with
        /// existing column names, e.g., 'table.column AS QUERY_NODE_ID', raw
        /// values, e.g., '{0, 2} AS QUERY_NODE_ID', or expressions, e.g.,
        /// 'ST_MAKEPOINT(table.x, table.y) AS QUERY_NODE_WKTPOINT'. Multiple
        /// values can be provided as long as the same identifier is used for
        /// all values. If using raw values in an identifier combination, the
        /// number of values specified must match across the combination.
        /// </summary>
        public IList<string> queries { get; set; } = new List<string>();

        /// <summary>Additional restrictions to apply to the nodes/edges of an
        /// existing graph. Restrictions must be specified using <a
        /// href="../../../graph_solver/network_graph_solver/#identifiers"
        /// target="_top">identifiers</a>; identifiers are grouped as <a
        /// href="../../../graph_solver/network_graph_solver/#id-combos"
        /// target="_top">combinations</a>. Identifiers can be used with
        /// existing column names, e.g., 'table.column AS
        /// RESTRICTIONS_EDGE_ID', expressions, e.g., 'column/2 AS
        /// RESTRICTIONS_VALUECOMPARED', or raw values, e.g., '{0, 0, 0, 1} AS
        /// RESTRICTIONS_ONOFFCOMPARED'. If using raw values in an identifier
        /// combination, the number of values specified must match across the
        /// combination.  The default value is an empty {@link List}.</summary>
        public IList<string> restrictions { get; set; } = new List<string>();

        /// <summary>Name of the table to store the resulting adjacencies, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  If left blank, the query
        /// results are instead returned in the response. If the
        /// 'QUERY_TARGET_NODE_LABEL' <a
        /// href="../../../graph_solver/network_graph_solver/#query-identifiers"
        /// target="_top">query identifier</a> is used in <paramref
        /// cref="QueryGraphRequest.queries" />, then two additional columns
        /// will be available: 'PATH_ID' and 'RING_ID'. See <a
        /// href="../../../graph_solver/network_graph_solver/#using-labels"
        /// target="_top">Using Labels</a> for more information.  The default
        /// value is ''.</summary>
        public string adjacency_table { get; set; } = "";

        /// <summary>Sets the number of rings around the node to query for
        /// adjacency, with '1' being the edges directly attached to the
        /// queried node. Also known as number of hops. For example, if it is
        /// set to '2', the edge(s) directly attached to the queried node(s)
        /// will be returned; in addition, the edge(s) attached to the node(s)
        /// attached to the initial ring of edge(s) surrounding the queried
        /// node(s) will be returned. If the value is set to '0', any nodes
        /// that meet the criteria in <paramref
        /// cref="QueryGraphRequest.queries" /> and <paramref
        /// cref="QueryGraphRequest.restrictions" /> will be returned. This
        /// parameter is only applicable when querying nodes.  The default
        /// value is 1.</summary>
        public int rings { get; set; } = 1;

        /// <summary>Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FORCE_UNDIRECTED">FORCE_UNDIRECTED</see>:</term>
        ///         <description>If set to <i>true</i>, all inbound edges and
        /// outbound edges relative to the node will be returned. If set to
        /// <i>false</i>, only outbound edges relative to the node will be
        /// returned. This parameter is only applicable if the queried graph
        /// <paramref cref="QueryGraphRequest.graph_name" /> is directed and
        /// when querying nodes. Consult <a
        /// href="../../../graph_solver/network_graph_solver/#directed-graphs"
        /// target="_top">Directed Graphs</a> for more details.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>When specified (>0), limits the number of
        /// query results. The size of the nodes table will be limited by the
        /// <i>limit</i> value.  The default value is '0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_WKT_PATH">OUTPUT_WKT_PATH</see>:</term>
        ///         <description>If true then concatenated wkt line segments
        /// will be added as the WKT column of the adjacency table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.AND_LABELS">AND_LABELS</see>:</term>
        ///         <description>If set to <i>true</i>, the result of the query
        /// has entities that satisfy all of the target labels, instead of any.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to the server, amongst those
        /// containing the corresponding graph, that has the most computational
        /// bandwidth.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_CHARN_LENGTH">OUTPUT_CHARN_LENGTH</see>:</term>
        ///         <description>When specified (>0 and <=256), limits the
        /// number of char length on the output tables for string based nodes.
        /// The default length is 64.  The default value is '64'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FIND_COMMON_LABELS">FIND_COMMON_LABELS</see>:</term>
        ///         <description>If set to true, for many-to-many queries or
        /// multi-level traversals, it lists the common labels between the
        /// source and target nodes and edge labels in each path. Otherwise
        /// (zero rings), it'll list all labels of the node(s) queried.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a QueryGraphRequest object with default
        /// parameters.</summary>
        public QueryGraphRequest() { }

        /// <summary>Constructs a QueryGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name">Name of the graph resource to query.
        /// </param>
        /// <param name="queries">Nodes or edges to be queried specified using
        /// <a
        /// href="../../../graph_solver/network_graph_solver/#query-identifiers"
        /// target="_top">query identifiers</a>. Identifiers can be used with
        /// existing column names, e.g., 'table.column AS QUERY_NODE_ID', raw
        /// values, e.g., '{0, 2} AS QUERY_NODE_ID', or expressions, e.g.,
        /// 'ST_MAKEPOINT(table.x, table.y) AS QUERY_NODE_WKTPOINT'. Multiple
        /// values can be provided as long as the same identifier is used for
        /// all values. If using raw values in an identifier combination, the
        /// number of values specified must match across the combination.
        /// </param>
        /// <param name="restrictions">Additional restrictions to apply to the
        /// nodes/edges of an existing graph. Restrictions must be specified
        /// using <a
        /// href="../../../graph_solver/network_graph_solver/#identifiers"
        /// target="_top">identifiers</a>; identifiers are grouped as <a
        /// href="../../../graph_solver/network_graph_solver/#id-combos"
        /// target="_top">combinations</a>. Identifiers can be used with
        /// existing column names, e.g., 'table.column AS
        /// RESTRICTIONS_EDGE_ID', expressions, e.g., 'column/2 AS
        /// RESTRICTIONS_VALUECOMPARED', or raw values, e.g., '{0, 0, 0, 1} AS
        /// RESTRICTIONS_ONOFFCOMPARED'. If using raw values in an identifier
        /// combination, the number of values specified must match across the
        /// combination.  The default value is an empty {@link List}.</param>
        /// <param name="adjacency_table">Name of the table to store the
        /// resulting adjacencies, in [schema_name.]table_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  If left blank, the query
        /// results are instead returned in the response. If the
        /// 'QUERY_TARGET_NODE_LABEL' <a
        /// href="../../../graph_solver/network_graph_solver/#query-identifiers"
        /// target="_top">query identifier</a> is used in <paramref
        /// cref="QueryGraphRequest.queries" />, then two additional columns
        /// will be available: 'PATH_ID' and 'RING_ID'. See <a
        /// href="../../../graph_solver/network_graph_solver/#using-labels"
        /// target="_top">Using Labels</a> for more information.  The default
        /// value is ''.</param>
        /// <param name="rings">Sets the number of rings around the node to
        /// query for adjacency, with '1' being the edges directly attached to
        /// the queried node. Also known as number of hops. For example, if it
        /// is set to '2', the edge(s) directly attached to the queried node(s)
        /// will be returned; in addition, the edge(s) attached to the node(s)
        /// attached to the initial ring of edge(s) surrounding the queried
        /// node(s) will be returned. If the value is set to '0', any nodes
        /// that meet the criteria in <paramref
        /// cref="QueryGraphRequest.queries" /> and <paramref
        /// cref="QueryGraphRequest.restrictions" /> will be returned. This
        /// parameter is only applicable when querying nodes.  The default
        /// value is 1.</param>
        /// <param name="options">Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FORCE_UNDIRECTED">FORCE_UNDIRECTED</see>:</term>
        ///         <description>If set to <i>true</i>, all inbound edges and
        /// outbound edges relative to the node will be returned. If set to
        /// <i>false</i>, only outbound edges relative to the node will be
        /// returned. This parameter is only applicable if the queried graph
        /// <paramref cref="QueryGraphRequest.graph_name" /> is directed and
        /// when querying nodes. Consult <a
        /// href="../../../graph_solver/network_graph_solver/#directed-graphs"
        /// target="_top">Directed Graphs</a> for more details.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>When specified (>0), limits the number of
        /// query results. The size of the nodes table will be limited by the
        /// <i>limit</i> value.  The default value is '0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_WKT_PATH">OUTPUT_WKT_PATH</see>:</term>
        ///         <description>If true then concatenated wkt line segments
        /// will be added as the WKT column of the adjacency table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.AND_LABELS">AND_LABELS</see>:</term>
        ///         <description>If set to <i>true</i>, the result of the query
        /// has entities that satisfy all of the target labels, instead of any.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.SERVER_ID">SERVER_ID</see>:</term>
        ///         <description>Indicates which graph server(s) to send the
        /// request to. Default is to send to the server, amongst those
        /// containing the corresponding graph, that has the most computational
        /// bandwidth.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.OUTPUT_CHARN_LENGTH">OUTPUT_CHARN_LENGTH</see>:</term>
        ///         <description>When specified (>0 and <=256), limits the
        /// number of char length on the output tables for string based nodes.
        /// The default length is 64.  The default value is '64'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FIND_COMMON_LABELS">FIND_COMMON_LABELS</see>:</term>
        ///         <description>If set to true, for many-to-many queries or
        /// multi-level traversals, it lists the common labels between the
        /// source and target nodes and edge labels in each path. Otherwise
        /// (zero rings), it'll list all labels of the node(s) queried.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="QueryGraphRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public QueryGraphRequest( string graph_name,
                                  IList<string> queries,
                                  IList<string> restrictions = null,
                                  string adjacency_table = null,
                                  int? rings = null,
                                  IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.queries = queries ?? new List<string>();
            this.restrictions = restrictions ?? new List<string>();
            this.adjacency_table = adjacency_table ?? "";
            this.rings = rings ?? 1;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class QueryGraphRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.queryGraph(string,IList{string},IList{string},string,int,IDictionary{string, string})"
    /// />.</summary>
    public class QueryGraphResponse : KineticaData
    {

        /// <summary>Indicates a successful query.  </summary>
        public bool result { get; set; }

        /// <summary>The adjacency entity integer ID: either edge IDs per node
        /// requested (if using QUERY_EDGE_ID or QUERY_NODE1_ID and
        /// QUERY_NODE2_ID in the input) or two node IDs per edge requested (if
        /// using QUERY_NODE_ID in the input).  </summary>
        public IList<long> adjacency_list_int_array { get; set; } = new List<long>();

        /// <summary>The adjacency entity string ID: either edge IDs per node
        /// requested (if using QUERY_EDGE_NAME or QUERY_NODE1_NAME and
        /// QUERY_NODE2_NAME in the input) or two node IDs per edge requested
        /// (if using QUERY_NODE_NAME in the input).  </summary>
        public IList<string> adjacency_list_string_array { get; set; } = new List<string>();

        /// <summary>The adjacency entity WKTPOINT or WKTLINE ID: either edge
        /// IDs per node requested (if using QUERY_EDGE_WKTLINE or
        /// QUERY_NODE1_WKTPOINT and QUERY_NODE2_WKTPOINT in the input) or two
        /// node IDs per edge requested (if using QUERY_NODE_WKTPOINT in the
        /// input).  </summary>
        public IList<string> adjacency_list_wkt_array { get; set; } = new List<string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class QueryGraphResponse




}  // end namespace kinetica
