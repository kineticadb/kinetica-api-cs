/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filter(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Filters data based on the specified expression.  The results are
    /// stored in a <a href="../../../concepts/filtered_views/"
    /// target="_top">result set</a> with the
    /// given <see cref="view_name" />.
    /// <br />
    /// For details see <a href="../../../concepts/expressions/"
    /// target="_top">Expressions</a>.
    /// <br />
    /// The response message contains the number of points for which the
    /// expression
    /// evaluated to be true, which is equivalent to the size of the result
    /// view.</summary>
    public class FilterRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterRequest.view_name" /> and use /create/schema to create
        /// the schema if non-existent]  Name of a schema for the newly created
        /// view. If the schema is non-existent, it will be automatically
        /// created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this filtered-view is part of.  The
        /// default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the view specified in <paramref
        /// cref="FilterRequest.view_name" />.</description>
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
            /// the newly created view. If the schema is non-existent, it will
            /// be automatically created.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>view this filtered-view is part of.  The default value
            /// is ''.</summary>
            public const string VIEW_ID = "view_id";

            /// <summary>Sets the <a href="../../../concepts/ttl/"
            /// target="_top">TTL</a> of the view specified in <see
            /// cref="view_name" />.</summary>
            public const string TTL = "ttl";
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

        /// <summary>The select expression to filter the specified table.  For
        /// details see <a href="../../../concepts/expressions/"
        /// target="_top">Expressions</a>.  </summary>
        public string expression { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterRequest.view_name" /> and use /create/schema to create
        /// the schema if non-existent]  Name of a schema for the newly created
        /// view. If the schema is non-existent, it will be automatically
        /// created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this filtered-view is part of.  The
        /// default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the view specified in <paramref
        /// cref="FilterRequest.view_name" />.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterRequest object with default
        /// parameters.</summary>
        public FilterRequest() { }

        /// <summary>Constructs a FilterRequest object with the specified
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
        /// <param name="expression">The select expression to filter the
        /// specified table.  For details see <a
        /// href="../../../concepts/expressions/"
        /// target="_top">Expressions</a>.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the view as part of <paramref
        /// cref="FilterRequest.view_name" /> and use /create/schema to create
        /// the schema if non-existent]  Name of a schema for the newly created
        /// view. If the schema is non-existent, it will be automatically
        /// created.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this filtered-view is part of.  The
        /// default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the view specified in <paramref
        /// cref="FilterRequest.view_name" />.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public FilterRequest( string table_name,
                              string view_name,
                              string expression,
                              IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.expression = expression ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filter(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class FilterResponse : KineticaData
    {

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>:</term>
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


        /// <summary>The number of records that matched the given select
        /// expression.  </summary>
        public long count { get; set; }

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>:</term>
        ///         <description>The fully qualified name of the view (i.e.
        /// including the schema)</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class FilterResponse




}  // end namespace kinetica
