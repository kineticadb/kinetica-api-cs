/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.deleteGraph(string,IDictionary{string, string})" />.
    /// <br />
    /// Deletes an existing graph from the graph server and/or
    /// persist.</summary>
    public class DeleteGraphRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.DELETE_PERSIST">DELETE_PERSIST</see>:</term>
        ///         <description>If set to <i>true</i>, the graph is removed
        /// from the server and persist. If set to <i>false</i>, the graph is
        /// removed from the server but is left in persist. The graph can be
        /// reloaded from persist if it is recreated with the same
        /// 'graph_name'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, the graph is removed from the
            /// server and persist. If set to <i>false</i>, the graph is
            /// removed from the server but is left in persist. The graph can
            /// be reloaded from persist if it is recreated with the same
            /// 'graph_name'.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="DeleteGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see>.</summary>
            public const string DELETE_PERSIST = "delete_persist";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the graph to be deleted.  </summary>
        public string graph_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.DELETE_PERSIST">DELETE_PERSIST</see>:</term>
        ///         <description>If set to <i>true</i>, the graph is removed
        /// from the server and persist. If set to <i>false</i>, the graph is
        /// removed from the server but is left in persist. The graph can be
        /// reloaded from persist if it is recreated with the same
        /// 'graph_name'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a DeleteGraphRequest object with default
        /// parameters.</summary>
        public DeleteGraphRequest() { }

        /// <summary>Constructs a DeleteGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name">Name of the graph to be deleted.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.DELETE_PERSIST">DELETE_PERSIST</see>:</term>
        ///         <description>If set to <i>true</i>, the graph is removed
        /// from the server and persist. If set to <i>false</i>, the graph is
        /// removed from the server but is left in persist. The graph can be
        /// reloaded from persist if it is recreated with the same
        /// 'graph_name'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="DeleteGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="DeleteGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public DeleteGraphRequest( string graph_name,
                                   IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class DeleteGraphRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.deleteGraph(string,IDictionary{string, string})"
    /// />.</summary>
    public class DeleteGraphResponse : KineticaData
    {

        /// <summary>Indicates a successful deletion.  </summary>
        public bool result { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class DeleteGraphResponse




}  // end namespace kinetica