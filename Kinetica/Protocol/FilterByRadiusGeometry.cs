/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByRadiusGeometry(FilterByRadiusGeometryRequest)">Kinetica.filterByRadiusGeometry</see>.
    /// </summary>
    /// <remarks><para>Calculates which geospatial geometry objects from a
    /// table intersect a circle with the given radius and center point (i.e.
    /// circular NAI). The operation is synchronous, meaning that a response
    /// will not be returned until all the objects are fully available. The
    /// response payload provides the count of the resulting set. A new
    /// resultant set (view) which satisfies the input circular NAI restriction
    /// specification is also created if a <see cref="view_name" /> is passed
    /// in as part of the request.</para></remarks>
    public class FilterByRadiusGeometryRequest : KineticaData
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
            /// <remarks><para>If the schema provided is non-existent, it will
            /// be automatically created.</para></remarks>
            public const string COLLECTION_NAME = "collection_name";
        } // end struct Options

        /// <summary>Name of the table on which the filter by radius operation
        /// will be performed, in [schema_name.]table_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
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

        /// <summary>Name of the geospatial geometry column to be filtered.
        /// </summary>
        public string column_name { get; set; }

        /// <summary>Value of the longitude of the center.</summary>
        /// <remarks><para>Must be within [-180.0, 180.0]. The minimum allowed
        /// value is -180. The maximum allowed value is 180.</para></remarks>
        public double x_center { get; set; }

        /// <summary>Value of the latitude of the center.</summary>
        /// <remarks><para>Must be within [-90.0, 90.0]. The minimum allowed
        /// value is -90. The maximum allowed value is 90.</para></remarks>
        public double y_center { get; set; }

        /// <summary>The radius of the circle within which the search will be
        /// performed.</summary>
        /// <remarks><para>Must be a non-zero positive value. It is in meters;
        /// so, for example, a value of '42000' means 42 km. The minimum
        /// allowed value is 0. The maximum allowed value is MAX_INT.</para>
        /// </remarks>
        public double radius { get; set; }

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
        ///         cref="FilterByRadiusGeometryResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>.
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
        ///         the newly created view. If the schema provided is
        ///         non-existent, it will be automatically created.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a FilterByRadiusGeometryRequest object with
        /// default parameters.</summary>
        public FilterByRadiusGeometryRequest() { }

        /// <summary>Constructs a FilterByRadiusGeometryRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Name of the table on which the filter by
        /// radius operation will be performed, in [schema_name.]table_name
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
        /// <param name="column_name">Name of the geospatial geometry column to
        /// be filtered.</param>
        /// <param name="x_center">Value of the longitude of the center. Must
        /// be within [-180.0, 180.0]. The minimum allowed value is -180. The
        /// maximum allowed value is 180.</param>
        /// <param name="y_center">Value of the latitude of the center. Must be
        /// within [-90.0, 90.0]. The minimum allowed value is -90. The maximum
        /// allowed value is 90.</param>
        /// <param name="radius">The radius of the circle within which the
        /// search will be performed. Must be a non-zero positive value. It is
        /// in meters; so, for example, a value of '42000' means 42 km. The
        /// minimum allowed value is 0. The maximum allowed value is MAX_INT.
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
        ///         cref="FilterByRadiusGeometryResponse.Info.QUALIFIED_VIEW_NAME">QUALIFIED_VIEW_NAME</see>.
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
        ///         the newly created view. If the schema provided is
        ///         non-existent, it will be automatically created.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public FilterByRadiusGeometryRequest( string table_name,
                                              string view_name,
                                              string column_name,
                                              double x_center,
                                              double y_center,
                                              double radius,
                                              IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.column_name = column_name ?? "";
            this.x_center = x_center;
            this.y_center = y_center;
            this.radius = radius;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class FilterByRadiusGeometryRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByRadiusGeometry(FilterByRadiusGeometryRequest)">Kinetica.filterByRadiusGeometry</see>.
    /// </summary>
    public class FilterByRadiusGeometryResponse : KineticaData
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

        /// <summary>The number of records passing the radius filter.</summary>
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
    } // end class FilterByRadiusGeometryResponse
} // end namespace kinetica
