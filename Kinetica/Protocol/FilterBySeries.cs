/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterBySeries(string,string,string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Filters objects matching all points of the given track (works only on
    /// track type data).  It allows users to specify a particular track to
    /// find all other points in the table that fall within specified
    /// ranges-spatial and temporal-of all points of the given track.
    /// Additionally, the user can specify another track to see if the two
    /// intersect (or go close to each other within the specified ranges). The
    /// user also has the flexibility of using different metrics for the
    /// spatial distance calculation: Euclidean (flat geometry) or Great Circle
    /// (spherical geometry to approximate the Earth's surface distances). The
    /// filtered points are stored in a newly created result set. The return
    /// value of the function is the number of points in the resultant set
    /// (view).
    /// <br />
    /// This operation is synchronous, meaning that a response will not be
    /// returned until all the objects are fully available.</summary>
    public class FilterBySeriesRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_RADIUS">SPATIAL_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the radius of the search area centered around each
        /// track point's geospatial coordinates. The value is interpreted in
        /// meters. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.TIME_RADIUS">TIME_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the maximum allowable time difference between the
        /// timestamps of a filtered object and the given track's points. The
        /// value is interpreted in seconds. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_DISTANCE_METRIC">SPATIAL_DISTANCE_METRIC</see>:</term>
        ///         <description>A string representing the coordinate system to
        /// use for the spatial search criteria. Acceptable values are
        /// 'euclidean' and 'great_circle'. Optional parameter; default is
        /// 'euclidean'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.EUCLIDEAN">EUCLIDEAN</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.GREAT_CIRCLE">GREAT_CIRCLE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the newly
            /// created view. If the collection provided is non-existent, the
            /// collection will be automatically created. If empty, then the
            /// newly created view will be top-level.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>A positive number passed as a string representing the
            /// radius of the search area centered around each track point's
            /// geospatial coordinates. The value is interpreted in meters.
            /// Required parameter.</summary>
            public const string SPATIAL_RADIUS = "spatial_radius";

            /// <summary>A positive number passed as a string representing the
            /// maximum allowable time difference between the timestamps of a
            /// filtered object and the given track's points. The value is
            /// interpreted in seconds. Required parameter.</summary>
            public const string TIME_RADIUS = "time_radius";

            /// <summary>A string representing the coordinate system to use for
            /// the spatial search criteria. Acceptable values are 'euclidean'
            /// and 'great_circle'. Optional parameter; default is 'euclidean'.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="FilterBySeriesRequest.Options.EUCLIDEAN">EUCLIDEAN</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="FilterBySeriesRequest.Options.GREAT_CIRCLE">GREAT_CIRCLE</see></term>
            ///     </item>
            /// </list></summary>
            public const string SPATIAL_DISTANCE_METRIC = "spatial_distance_metric";
            public const string EUCLIDEAN = "euclidean";
            public const string GREAT_CIRCLE = "great_circle";
        } // end struct Options


        /// <summary>Name of the table on which the filter by track operation
        /// will be performed. Must be a currently existing table with a <a
        /// href="../../geospatial/geo_objects.html" target="_top">track</a>
        /// present.  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.
        /// </summary>
        public string view_name { get; set; } = "";

        /// <summary>The ID of the track which will act as the filtering
        /// points. Must be an existing track within the given table.
        /// </summary>
        public string track_id { get; set; }

        /// <summary>Up to one track ID to intersect with the "filter" track.
        /// If any provided, it must be an valid track ID within the given set.
        /// </summary>
        public IList<string> target_track_ids { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_RADIUS">SPATIAL_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the radius of the search area centered around each
        /// track point's geospatial coordinates. The value is interpreted in
        /// meters. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.TIME_RADIUS">TIME_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the maximum allowable time difference between the
        /// timestamps of a filtered object and the given track's points. The
        /// value is interpreted in seconds. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_DISTANCE_METRIC">SPATIAL_DISTANCE_METRIC</see>:</term>
        ///         <description>A string representing the coordinate system to
        /// use for the spatial search criteria. Acceptable values are
        /// 'euclidean' and 'great_circle'. Optional parameter; default is
        /// 'euclidean'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.EUCLIDEAN">EUCLIDEAN</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.GREAT_CIRCLE">GREAT_CIRCLE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterBySeriesRequest object with default
        /// parameters.</summary>
        public FilterBySeriesRequest() { }

        /// <summary>Constructs a FilterBySeriesRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the filter by
        /// track operation will be performed. Must be a currently existing
        /// table with a <a href="../../geospatial/geo_objects.html"
        /// target="_top">track</a> present.  </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results. Has the same naming restrictions
        /// as <a href="../../concepts/tables.html" target="_top">tables</a>.
        /// </param>
        /// <param name="track_id">The ID of the track which will act as the
        /// filtering points. Must be an existing track within the given table.
        /// </param>
        /// <param name="target_track_ids">Up to one track ID to intersect with
        /// the "filter" track. If any provided, it must be an valid track ID
        /// within the given set.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created view. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, then the newly
        /// created view will be top-level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_RADIUS">SPATIAL_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the radius of the search area centered around each
        /// track point's geospatial coordinates. The value is interpreted in
        /// meters. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.TIME_RADIUS">TIME_RADIUS</see>:</term>
        ///         <description>A positive number passed as a string
        /// representing the maximum allowable time difference between the
        /// timestamps of a filtered object and the given track's points. The
        /// value is interpreted in seconds. Required parameter.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.SPATIAL_DISTANCE_METRIC">SPATIAL_DISTANCE_METRIC</see>:</term>
        ///         <description>A string representing the coordinate system to
        /// use for the spatial search criteria. Acceptable values are
        /// 'euclidean' and 'great_circle'. Optional parameter; default is
        /// 'euclidean'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.EUCLIDEAN">EUCLIDEAN</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterBySeriesRequest.Options.GREAT_CIRCLE">GREAT_CIRCLE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public FilterBySeriesRequest( string table_name,
                                      string view_name,
                                      string track_id,
                                      IList<string> target_track_ids,
                                      IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.track_id = track_id ?? "";
            this.target_track_ids = target_track_ids ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterBySeriesRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterBySeries(string,string,string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class FilterBySeriesResponse : KineticaData
    {

        /// <summary>The number of records passing the series filter.
        /// </summary>
        public long count { get; set; }

    } // end class FilterBySeriesResponse




}  // end namespace kinetica
