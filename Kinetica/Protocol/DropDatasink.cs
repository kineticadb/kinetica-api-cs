/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.dropDatasink(DropDatasinkRequest)">Kinetica.dropDatasink</see>.
    /// </summary>
    /// <remarks><para>Drops an existing <a
    /// href="../../../concepts/data_sinks/" target="_top">data
    /// sink</a>.</para>
    /// <para>By default, if any <a href="../../../concepts/table_monitors"
    /// target="_top">table monitors</a> use this sink as a destination, the
    /// request will be blocked unless option <see
    /// cref="Options.CLEAR_TABLE_MONITORS">CLEAR_TABLE_MONITORS</see> is <see
    /// cref="Options.TRUE">TRUE</see>.</para></remarks>
    public class DropDatasinkRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>If <see cref="Options.TRUE">TRUE</see>, any <a
            /// href="../../../concepts/table_monitors/" target="_top">table
            /// monitors</a> that use this data sink will be cleared.</summary>
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
            public const string CLEAR_TABLE_MONITORS = "clear_table_monitors";

            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options

        /// <summary>Name of the data sink to be dropped.</summary>
        /// <remarks><para>Must be an existing data sink.</para></remarks>
        public string name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.CLEAR_TABLE_MONITORS">CLEAR_TABLE_MONITORS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, any <a
        ///         href="../../../concepts/table_monitors/"
        ///         target="_top">table monitors</a> that use this data sink
        ///         will be cleared.
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
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a DropDatasinkRequest object with default
        /// parameters.</summary>
        public DropDatasinkRequest() { }

        /// <summary>Constructs a DropDatasinkRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="name">Name of the data sink to be dropped. Must be an
        /// existing data sink.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.CLEAR_TABLE_MONITORS">CLEAR_TABLE_MONITORS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, any <a
        ///         href="../../../concepts/table_monitors/"
        ///         target="_top">table monitors</a> that use this data sink
        ///         will be cleared.
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
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public DropDatasinkRequest( string name,
                                    IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class DropDatasinkRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.dropDatasink(DropDatasinkRequest)">Kinetica.dropDatasink</see>.
    /// </summary>
    public class DropDatasinkResponse : KineticaData
    {
        /// <summary>Value of <see cref="DropDatasinkRequest.name">name</see>.
        /// </summary>
        public string name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class DropDatasinkResponse
} // end namespace kinetica
