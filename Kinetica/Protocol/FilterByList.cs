/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByList(string,string,IDictionary{string, IList{string}},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates which records from a table have values in the given list
    /// for the corresponding column. The operation is synchronous, meaning
    /// that a
    /// response will not be returned until all the objects are fully
    /// available. The
    /// response payload provides the count of the resulting set. A new
    /// resultant set
    /// (view) which satisfies the input filter specification is also created
    /// if a
    /// <see cref="view_name" /> is passed in as part of the request.
    /// <br />
    /// For example, if a type definition has the columns 'x' and 'y', then a
    /// filter by
    /// list query with the column map
    /// {"x":["10.1", "2.3"], "y":["0.0", "-31.5", "42.0"]} will return
    /// the count of all data points whose x and y values match both in the
    /// respective
    /// x- and y-lists, e.g., "x = 10.1 and y = 0.0", "x = 2.3 and y = -31.5",
    /// etc.
    /// However, a record with "x = 10.1 and y = -31.5" or "x = 2.3 and y =
    /// 0.0"
    /// would not be returned because the values in the given lists do not
    /// correspond.</summary>
    public class FilterByListRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterByListRequest.view_name" /> and use /create/schema to
        /// create the schema if non-existent]  Name of a schema for the newly
        /// created view. If the schema provided is non-existent, it will be
        /// automatically created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// 'in_list' or 'not_in_list'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are in
        /// the provided list(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.NOT_IN_LIST">NOT_IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are not
        /// in the provided list(s).</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>[DEPRECATED--please specify the containing schema for
            /// the view as part of <see cref="view_name" /> and use <see
            /// cref="Kinetica.createSchema(string,IDictionary{string, string})"
            /// /> to create the schema if non-existent]  Name of a schema for
            /// the newly created view. If the schema provided is non-existent,
            /// it will be automatically created.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>String indicating the filter mode, either 'in_list' or
            /// 'not_in_list'.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>:</term>
            ///         <description>The filter will match all items that are
            /// in the provided list(s).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="FilterByListRequest.Options.NOT_IN_LIST">NOT_IN_LIST</see>:</term>
            ///         <description>The filter will match all items that are
            /// not in the provided list(s).</description>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>.</summary>
            public const string FILTER_MODE = "filter_mode";

            /// <summary>The filter will match all items that are in the
            /// provided list(s).</summary>
            public const string IN_LIST = "in_list";

            /// <summary>The filter will match all items that are not in the
            /// provided list(s).</summary>
            public const string NOT_IN_LIST = "not_in_list";
        } // end struct Options


        /// <summary>Name of the table to filter, in [schema_name.]table_name
        /// format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  This may be the name of a
        /// table or a view (when chaining queries).  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results, in [schema_name.]view_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  Must not be an already
        /// existing table or view.  The default value is ''.</summary>
        public string view_name { get; set; } = "";

        /// <summary>List of values for the corresponding column in the table
        /// </summary>
        public IDictionary<string, IList<string>> column_values_map { get; set; } = new Dictionary<string, IList<string>>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterByListRequest.view_name" /> and use /create/schema to
        /// create the schema if non-existent]  Name of a schema for the newly
        /// created view. If the schema provided is non-existent, it will be
        /// automatically created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// 'in_list' or 'not_in_list'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are in
        /// the provided list(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.NOT_IN_LIST">NOT_IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are not
        /// in the provided list(s).</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByListRequest object with default
        /// parameters.</summary>
        public FilterByListRequest() { }

        /// <summary>Constructs a FilterByListRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table to filter, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  This may be the name of a
        /// table or a view (when chaining queries).  </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results, in [schema_name.]view_name format,
        /// using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  Must not be an already
        /// existing table or view.  The default value is ''.</param>
        /// <param name="column_values_map">List of values for the
        /// corresponding column in the table  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterByListRequest.view_name" /> and use /create/schema to
        /// create the schema if non-existent]  Name of a schema for the newly
        /// created view. If the schema provided is non-existent, it will be
        /// automatically created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// 'in_list' or 'not_in_list'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are in
        /// the provided list(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByListRequest.Options.NOT_IN_LIST">NOT_IN_LIST</see>:</term>
        ///         <description>The filter will match all items that are not
        /// in the provided list(s).</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByListRequest.Options.IN_LIST">IN_LIST</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public FilterByListRequest( string table_name,
                                    string view_name,
                                    IDictionary<string, IList<string>> column_values_map,
                                    IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.column_values_map = column_values_map ?? new Dictionary<string, IList<string>>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByListRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByList(string,string,IDictionary{string, IList{string}},IDictionary{string, string})"
    /// />.</summary>
    public class FilterByListResponse : KineticaData
    {

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>:</term>
        ///         <description>The fully qualified name of the view (i.e.
        /// including the schema)</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <member name="info"
        /// />.</summary>
        public struct Info
        {

            /// <summary>The fully qualified name of the view (i.e. including
            /// the schema)</summary>
            public const string QUALIFIED_VIEW_NAME = "qualified_view_name";
        } // end struct Info


        /// <summary>The number of records passing the list filter.  </summary>
        public long count { get; set; }

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByListResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>:</term>
        ///         <description>The fully qualified name of the view (i.e.
        /// including the schema)</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class FilterByListResponse




}  // end namespace kinetica
