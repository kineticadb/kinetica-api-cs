/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showResourceObjects(ShowResourceObjectsRequest)">Kinetica.showResourceObjects</see>.
    /// </summary>
    /// <remarks><para>Returns information about the internal sub-components
    /// (tiered objects) which use resources of the system. The request can
    /// either return results from actively used objects (default) or it can be
    /// used to query the status of the objects of a given list of tables.
    /// Returns detailed information about the requested resource objects.
    /// </para></remarks>
    public class ShowResourceObjectsRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Comma-separated list of tiers to query, leave blank
            /// for all tiers.</summary>
            public const string TIERS = "tiers";

            /// <summary>An expression to filter the returned objects.
            /// </summary>
            /// <remarks><para>Expression is limited to the following
            /// operators: =,!=,&lt;,&lt;=,&gt;,&gt;=,+,-,*,AND,OR,LIKE. For
            /// details see <a href="../../../concepts/expressions/"
            /// target="_top">Expressions</a>. To use a more complex
            /// expression, query the ki_catalog.ki_tiered_objects table
            /// directly.</para></remarks>
            public const string EXPRESSION = "expression";

            /// <summary>Single column to be sorted by as well as the sort
            /// direction, e.g., 'size asc'.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.SIZE">SIZE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.ID">ID</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.PRIORITY">PRIORITY</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.TIER">TIER</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.EVICTABLE">EVICTABLE</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="Options.OWNER_RESOURCE_GROUP">OWNER_RESOURCE_GROUP</see>
            ///         </term>
            ///     </item>
            /// </list></remarks>
            public const string ORDER_BY = "order_by";

            public const string SIZE = "size";
            public const string ID = "id";
            public const string PRIORITY = "priority";
            public const string TIER = "tier";
            public const string EVICTABLE = "evictable";
            public const string OWNER_RESOURCE_GROUP = "owner_resource_group";

            /// <summary>An integer indicating the maximum number of results to
            /// be returned, per rank, or (-1) to indicate that the maximum
            /// number of results allowed by the server should be returned.
            /// </summary>
            /// <remarks><para> The number of records returned will never
            /// exceed the server's own limit, defined by the <a
            /// href="../../../config/#config-main-general"
            /// target="_top">max_get_records_size</a> parameter in the server
            /// configuration. The default value is '100'.</para></remarks>
            public const string LIMIT = "limit";

            /// <summary>Comma-separated list of tables to restrict the results
            /// to.</summary>
            /// <remarks><para>Use '*' to show all tables.</para></remarks>
            public const string TABLE_NAMES = "table_names";
        } // end struct Options

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.TIERS">TIERS</see>:</term>
        ///         <description>Comma-separated list of tiers to query, leave
        ///         blank for all tiers.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.EXPRESSION">EXPRESSION</see>:
        ///         </term>
        ///         <description>An expression to filter the returned objects.
        ///         Expression is limited to the following operators:
        ///         =,!=,&lt;,&lt;=,&gt;,&gt;=,+,-,*,AND,OR,LIKE. For details
        ///         see <a href="../../../concepts/expressions/"
        ///         target="_top">Expressions</a>. To use a more complex
        ///         expression, query the ki_catalog.ki_tiered_objects table
        ///         directly.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Single column to be sorted by as well as the
        ///         sort direction, e.g., 'size asc'.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.SIZE">SIZE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.ID">ID</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.PRIORITY">PRIORITY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.TIER">TIER</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.EVICTABLE">EVICTABLE</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="Options.OWNER_RESOURCE_GROUP">OWNER_RESOURCE_GROUP</see>
        ///                 </term>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.LIMIT">LIMIT</see>:</term>
        ///         <description>An integer indicating the maximum number of
        ///         results to be returned, per rank, or (-1) to indicate that
        ///         the maximum number of results allowed by the server should
        ///         be returned.  The number of records returned will never
        ///         exceed the server's own limit, defined by the <a
        ///         href="../../../config/#config-main-general"
        ///         target="_top">max_get_records_size</a> parameter in the
        ///         server configuration. The default value is '100'.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.TABLE_NAMES">TABLE_NAMES</see>:
        ///         </term>
        ///         <description>Comma-separated list of tables to restrict the
        ///         results to. Use '*' to show all tables.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a ShowResourceObjectsRequest object with
        /// default parameters.</summary>
        public ShowResourceObjectsRequest() { }

        /// <summary>Constructs a ShowResourceObjectsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.TIERS">TIERS</see>:</term>
        ///         <description>Comma-separated list of tiers to query, leave
        ///         blank for all tiers.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.EXPRESSION">EXPRESSION</see>:
        ///         </term>
        ///         <description>An expression to filter the returned objects.
        ///         Expression is limited to the following operators:
        ///         =,!=,&lt;,&lt;=,&gt;,&gt;=,+,-,*,AND,OR,LIKE. For details
        ///         see <a href="../../../concepts/expressions/"
        ///         target="_top">Expressions</a>. To use a more complex
        ///         expression, query the ki_catalog.ki_tiered_objects table
        ///         directly.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Single column to be sorted by as well as the
        ///         sort direction, e.g., 'size asc'.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.SIZE">SIZE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.ID">ID</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.PRIORITY">PRIORITY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.TIER">TIER</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.EVICTABLE">EVICTABLE</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="Options.OWNER_RESOURCE_GROUP">OWNER_RESOURCE_GROUP</see>
        ///                 </term>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.LIMIT">LIMIT</see>:</term>
        ///         <description>An integer indicating the maximum number of
        ///         results to be returned, per rank, or (-1) to indicate that
        ///         the maximum number of results allowed by the server should
        ///         be returned.  The number of records returned will never
        ///         exceed the server's own limit, defined by the <a
        ///         href="../../../config/#config-main-general"
        ///         target="_top">max_get_records_size</a> parameter in the
        ///         server configuration. The default value is '100'.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.TABLE_NAMES">TABLE_NAMES</see>:
        ///         </term>
        ///         <description>Comma-separated list of tables to restrict the
        ///         results to. Use '*' to show all tables.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public ShowResourceObjectsRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowResourceObjectsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showResourceObjects(ShowResourceObjectsRequest)">Kinetica.showResourceObjects</see>.
    /// </summary>
    public class ShowResourceObjectsResponse : KineticaData
    {
        /// <summary>Tier usage across ranks.</summary>
        /// <remarks><para>Layout is:
        /// response.rank_usage[rank_number][resource_group_name] = group_usage
        /// (as stringified json)</para></remarks>
        public IDictionary<string, string> rank_objects { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowResourceObjectsResponse
} // end namespace kinetica
