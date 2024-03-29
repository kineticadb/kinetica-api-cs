/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByBox(FilterByBoxRequest)">Kinetica.filterByBox</see>.
    /// </summary>
    /// <remarks><para>Calculates how many objects within the given table lie
    /// in a rectangular box. The operation is synchronous, meaning that a
    /// response will not be returned until all the objects are fully
    /// available. The response payload provides the count of the resulting
    /// set. A new resultant set which satisfies the input NAI restriction
    /// specification is also created when a <see cref="view_name" /> is passed
    /// in as part of the input payload.</para></remarks>
    public class FilterByBoxRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>If <see cref="Options.TRUE">TRUE</see>, a unique
            /// temporary table name will be generated in the sys_temp schema
            /// and used in place of <see cref="view_name" />.</summary>
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
            public const string CREATE_TEMP_TABLE = "create_temp_table";

            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>[DEPRECATED--please specify the containing schema for
            /// the view as part of <see cref="view_name" /> and use <see
            /// cref="Kinetica.createSchema(CreateSchemaRequest)">Kinetica.createSchema</see>
            /// to create the schema if non-existent]  Name of a schema for the
            /// newly created view.</summary>
            /// <remarks><para>If the schema is non-existent, it will be
            /// automatically created.</para></remarks>
            public const string COLLECTION_NAME = "collection_name";
        } // end struct Options

        /// <summary>Name of the table on which the bounding box operation will
        /// be performed, in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</summary>
        /// <remarks><para> Must be an existing table.</para></remarks>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results, in [schema_name.]view_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.</summary>
        /// <remarks><para> Must not be an already existing table or view. The
        /// default value is ''.</para></remarks>
        public string view_name { get; set; } = "";

        /// <summary>Name of the column on which to perform the bounding box
        /// query.</summary>
        /// <remarks><para>Must be a valid numeric column.</para></remarks>
        public string x_column_name { get; set; }

        /// <summary>Lower bound for the column chosen by <see
        /// cref="x_column_name" />.</summary>
        /// <remarks><para> Must be less than or equal to <see cref="max_x" />.
        /// </para></remarks>
        public double min_x { get; set; }

        /// <summary>Upper bound for <see cref="x_column_name" />.</summary>
        /// <remarks><para> Must be greater than or equal to <see cref="min_x"
        /// />.</para></remarks>
        public double max_x { get; set; }

        /// <summary>Name of a column on which to perform the bounding box
        /// query.</summary>
        /// <remarks><para>Must be a valid numeric column.</para></remarks>
        public string y_column_name { get; set; }

        /// <summary>Lower bound for <see cref="y_column_name" />.</summary>
        /// <remarks><para>Must be less than or equal to <see cref="max_y" />.
        /// </para></remarks>
        public double min_y { get; set; }

        /// <summary>Upper bound for <see cref="y_column_name" />.</summary>
        /// <remarks><para>Must be greater than or equal to <see cref="min_y"
        /// />.</para></remarks>
        public double max_y { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, a
        ///         unique temporary table name will be generated in the
        ///         sys_temp schema and used in place of <see cref="view_name"
        ///         />. This is always allowed even if the caller does not have
        ///         permission to create tables. The generated name is returned
        ///         in <see
        ///         cref="FilterByBoxResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>.
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
        ///     <item>
        ///         <term><see
        ///         cref="Options.COLLECTION_NAME">COLLECTION_NAME</see>:
        ///         </term>
        ///         <description>[DEPRECATED--please specify the containing
        ///         schema for the view as part of <see cref="view_name" /> and
        ///         use <see
        ///         cref="Kinetica.createSchema(CreateSchemaRequest)">Kinetica.createSchema</see>
        ///         to create the schema if non-existent]  Name of a schema for
        ///         the newly created view. If the schema is non-existent, it
        ///         will be automatically created.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a FilterByBoxRequest object with default
        /// parameters.</summary>
        public FilterByBoxRequest() { }

        /// <summary>Constructs a FilterByBoxRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="table_name">Name of the table on which the bounding
        /// box operation will be performed, in [schema_name.]table_name
        /// format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Must be an existing
        /// table.</param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results, in [schema_name.]view_name format,
        /// using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  Must not be an already
        /// existing table or view. The default value is ''.</param>
        /// <param name="x_column_name">Name of the column on which to perform
        /// the bounding box query. Must be a valid numeric column.</param>
        /// <param name="min_x">Lower bound for the column chosen by <paramref
        /// name="x_column_name" />.  Must be less than or equal to <paramref
        /// name="max_x" />.</param>
        /// <param name="max_x">Upper bound for <paramref name="x_column_name"
        /// />.  Must be greater than or equal to <paramref name="min_x" />.
        /// </param>
        /// <param name="y_column_name">Name of a column on which to perform
        /// the bounding box query. Must be a valid numeric column.</param>
        /// <param name="min_y">Lower bound for <paramref name="y_column_name"
        /// />. Must be less than or equal to <paramref name="max_y" />.
        /// </param>
        /// <param name="max_y">Upper bound for <paramref name="y_column_name"
        /// />. Must be greater than or equal to <paramref name="min_y" />.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, a
        ///         unique temporary table name will be generated in the
        ///         sys_temp schema and used in place of <paramref
        ///         name="view_name" />. This is always allowed even if the
        ///         caller does not have permission to create tables. The
        ///         generated name is returned in <see
        ///         cref="FilterByBoxResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>.
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
        ///     <item>
        ///         <term><see
        ///         cref="Options.COLLECTION_NAME">COLLECTION_NAME</see>:
        ///         </term>
        ///         <description>[DEPRECATED--please specify the containing
        ///         schema for the view as part of <paramref name="view_name"
        ///         /> and use <see
        ///         cref="Kinetica.createSchema(CreateSchemaRequest)">Kinetica.createSchema</see>
        ///         to create the schema if non-existent]  Name of a schema for
        ///         the newly created view. If the schema is non-existent, it
        ///         will be automatically created.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public FilterByBoxRequest( string table_name,
                                   string view_name,
                                   string x_column_name,
                                   double min_x,
                                   double max_x,
                                   string y_column_name,
                                   double min_y,
                                   double max_y,
                                   IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.x_column_name = x_column_name ?? "";
            this.min_x = min_x;
            this.max_x = max_x;
            this.y_column_name = y_column_name ?? "";
            this.min_y = min_y;
            this.max_y = max_y;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class FilterByBoxRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByBox(FilterByBoxRequest)">Kinetica.filterByBox</see>.
    /// </summary>
    public class FilterByBoxResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="info" />.</summary>
        /// <remarks><para>Additional information.</para></remarks>
        public struct Info
        {
            /// <summary>The fully qualified name of the view (i.e. including
            /// the schema)</summary>
            public const string QUALIFIED_VIEW_NAME = "qualified_view_name";
        } // end struct Info

        /// <summary>The number of records passing the box filter.</summary>
        public long count { get; set; }

        /// <summary>Additional information.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>:
        ///         </term>
        ///         <description>The fully qualified name of the view (i.e.
        ///         including the schema)</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class FilterByBoxResponse
} // end namespace kinetica
