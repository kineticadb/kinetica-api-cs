/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.listGraph(string,IDictionary{string, string})" />.
    /// <br />
    /// Lists basic information about one or all graphs that exist on the graph
    /// server.</summary>
    public class ListGraphRequest : KineticaData
    {

        /// <summary>Name of the graph on which to retrieve information. If
        /// empty, information about all graphs is returned.  The default value
        /// is ''.</summary>
        public string graph_name { get; set; } = "";

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ListGraphRequest object with default
        /// parameters.</summary>
        public ListGraphRequest() { }

        /// <summary>Constructs a ListGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name">Name of the graph on which to retrieve
        /// information. If empty, information about all graphs is returned.
        /// The default value is ''.</param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public ListGraphRequest( string graph_name = null,
                                 IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ListGraphRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.listGraph(string,IDictionary{string, string})"
    /// />.</summary>
    public class ListGraphResponse : KineticaData
    {

        /// <summary>Indicates a successful listing.  </summary>
        public bool result { get; set; }

        /// <summary>Name(s) of the graph(s).  </summary>
        public IList<string> graph_names { get; set; } = new List<string>();

        /// <summary>Total number of nodes in the graph.  </summary>
        public IList<long> num_nodes { get; set; } = new List<long>();

        /// <summary>Total number of edges in the graph.  </summary>
        public IList<long> num_edges { get; set; } = new List<long>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ListGraphResponse




}  // end namespace kinetica