/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.repartitionGraph(RepartitionGraphRequest)">Kinetica.repartitionGraph</see>.
    /// </summary>
    /// <remarks><para>Rebalances an existing partitioned graph.</para>
    /// <para>IMPORTANT: It's highly recommended that you review the <a
    /// href="../../../graph_solver/network_graph_solver/" target="_top">Graphs
    /// & Solvers</a> concepts documentation, the <a
    /// href="../../../guides/graph_rest_guide/" target="_top">Graph REST
    /// Tutorial</a>, and/or some <a href="../../../guide-tags/graph/"
    /// target="_top">graph examples</a> before using this endpoint.</para>
    /// </remarks>
    public class RepartitionGraphRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>If a non-empty value is specified, the original graph
            /// will be kept (non-default behaviour) and a new balanced graph
            /// will be created under this given name.</summary>
            /// <remarks><para> When the value is empty (default), the
            /// generated 'balanced' graph will replace the original
            /// 'unbalanced' graph under the same graph name. The default value
            /// is ''.</para></remarks>
            public const string NEW_GRAPH_NAME = "new_graph_name";

            /// <summary>The distributed shortest path solve is run from this
            /// source node to all the nodes in the graph to create balaced
            /// partitions using the iso-distance levels of the solution.
            /// </summary>
            /// <remarks><para> The source node is selected by the rebalance
            /// algorithm automatically (default case when the value is an
            /// empty string). Otherwise, the user specified node is used as
            /// the source. The default value is ''.</para></remarks>
            public const string SOURCE_NODE = "source_node";

            /// <summary>The default value is ''.</summary>
            public const string SQL_REQUEST_AVRO_JSON = "sql_request_avro_json";
        } // end struct Options

        /// <summary>Name of the graph resource to rebalance.</summary>
        public string graph_name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.NEW_GRAPH_NAME">NEW_GRAPH_NAME</see>:</term>
        ///         <description>If a non-empty value is specified, the
        ///         original graph will be kept (non-default behaviour) and a
        ///         new balanced graph will be created under this given name.
        ///         When the value is empty (default), the generated 'balanced'
        ///         graph will replace the original 'unbalanced' graph under
        ///         the same graph name. The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.SOURCE_NODE">SOURCE_NODE</see>:
        ///         </term>
        ///         <description>The distributed shortest path solve is run
        ///         from this source node to all the nodes in the graph to
        ///         create balaced partitions using the iso-distance levels of
        ///         the solution.  The source node is selected by the rebalance
        ///         algorithm automatically (default case when the value is an
        ///         empty string). Otherwise, the user specified node is used
        ///         as the source. The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.SQL_REQUEST_AVRO_JSON">SQL_REQUEST_AVRO_JSON</see>:
        ///         </term>
        ///         <description>The default value is ''.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a RepartitionGraphRequest object with default
        /// parameters.</summary>
        public RepartitionGraphRequest() { }

        /// <summary>Constructs a RepartitionGraphRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="graph_name">Name of the graph resource to rebalance.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.NEW_GRAPH_NAME">NEW_GRAPH_NAME</see>:</term>
        ///         <description>If a non-empty value is specified, the
        ///         original graph will be kept (non-default behaviour) and a
        ///         new balanced graph will be created under this given name.
        ///         When the value is empty (default), the generated 'balanced'
        ///         graph will replace the original 'unbalanced' graph under
        ///         the same graph name. The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.SOURCE_NODE">SOURCE_NODE</see>:
        ///         </term>
        ///         <description>The distributed shortest path solve is run
        ///         from this source node to all the nodes in the graph to
        ///         create balaced partitions using the iso-distance levels of
        ///         the solution.  The source node is selected by the rebalance
        ///         algorithm automatically (default case when the value is an
        ///         empty string). Otherwise, the user specified node is used
        ///         as the source. The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.SQL_REQUEST_AVRO_JSON">SQL_REQUEST_AVRO_JSON</see>:
        ///         </term>
        ///         <description>The default value is ''.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public RepartitionGraphRequest( string graph_name,
                                        IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class RepartitionGraphRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.repartitionGraph(RepartitionGraphRequest)">Kinetica.repartitionGraph</see>.
    /// </summary>
    public class RepartitionGraphResponse : KineticaData
    {
        /// <summary>Indicates a successful rebalancing on all servers.
        /// </summary>
        public bool result { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class RepartitionGraphResponse
} // end namespace kinetica
