/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByRadius(string,string,string,double,string,double,double,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates which objects from a table lie within a circle with the
    /// given radius and center point (i.e. circular NAI). The operation is
    /// synchronous, meaning that a response will not be returned until all the
    /// objects are fully available. The response payload provides the count of
    /// the resulting set. A new resultant set (view) which satisfies the input
    /// circular NAI restriction specification is also created if a <see
    /// cref="view_name" /> is passed in as part of the request.
    /// <br />
    /// For track data, all track points that lie within the circle plus one
    /// point on either side of the circle (if the track goes beyond the
    /// circle) will be included in the result.</summary>
    public class FilterByRadiusRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRadiusRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the newly
            /// created view. If the collection provided is non-existent, the
            /// collection will be automatically created. If empty, then the
            /// newly created view will be top-level.</summary>
            public const string COLLECTION_NAME = "collection_name";
        } // end struct Options


        /// <summary>Name of the table on which the filter by radius operation
        /// will be performed.  Must be an existing table.  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.  The
        /// default value is ''.</summary>
        public string view_name { get; set; } = "";

        /// <summary>Name of the column to be used for the x-coordinate (the
        /// longitude) of the center.  </summary>
        public string x_column_name { get; set; }

        /// <summary>Value of the longitude of the center. Must be within
        /// [-180.0, 180.0].  The minimum allowed value is -180. The maximum
        /// allowed value is 180. </summary>
        public double x_center { get; set; }

        /// <summary>Name of the column to be used for the y-coordinate-the
        /// latitude-of the center.  </summary>
        public string y_column_name { get; set; }

        /// <summary>Value of the latitude of the center. Must be within
        /// [-90.0, 90.0].  The minimum allowed value is -90. The maximum
        /// allowed value is 90. </summary>
        public double y_center { get; set; }

        /// <summary>The radius of the circle within which the search will be
        /// performed. Must be a non-zero positive value. It is in meters; so,
        /// for example, a value of '42000' means 42 km.  The minimum allowed
        /// value is 0. The maximum allowed value is MAX_INT. </summary>
        public double radius { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRadiusRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByRadiusRequest object with default
        /// parameters.</summary>
        public FilterByRadiusRequest() { }

        /// <summary>Constructs a FilterByRadiusRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the filter by
        /// radius operation will be performed.  Must be an existing table.
        /// </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results. Has the same naming restrictions
        /// as <a href="../../concepts/tables.html" target="_top">tables</a>.
        /// The default value is ''.</param>
        /// <param name="x_column_name">Name of the column to be used for the
        /// x-coordinate (the longitude) of the center.  </param>
        /// <param name="x_center">Value of the longitude of the center. Must
        /// be within [-180.0, 180.0].  The minimum allowed value is -180. The
        /// maximum allowed value is 180. </param>
        /// <param name="y_column_name">Name of the column to be used for the
        /// y-coordinate-the latitude-of the center.  </param>
        /// <param name="y_center">Value of the latitude of the center. Must be
        /// within [-90.0, 90.0].  The minimum allowed value is -90. The
        /// maximum allowed value is 90. </param>
        /// <param name="radius">The radius of the circle within which the
        /// search will be performed. Must be a non-zero positive value. It is
        /// in meters; so, for example, a value of '42000' means 42 km.  The
        /// minimum allowed value is 0. The maximum allowed value is MAX_INT.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByRadiusRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public FilterByRadiusRequest( string table_name,
                                      string view_name,
                                      string x_column_name,
                                      double x_center,
                                      string y_column_name,
                                      double y_center,
                                      double radius,
                                      IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.x_column_name = x_column_name ?? "";
            this.x_center = x_center;
            this.y_column_name = y_column_name ?? "";
            this.y_center = y_center;
            this.radius = radius;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByRadiusRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByRadius(string,string,string,double,string,double,double,IDictionary{string, string})"
    /// />.</summary>
    public class FilterByRadiusResponse : KineticaData
    {

        /// <summary>The number of records passing the radius filter.
        /// </summary>
        public long count { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class FilterByRadiusResponse




}  // end namespace kinetica
