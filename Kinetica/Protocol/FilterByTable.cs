/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.filterByTable(string,string,string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Filters objects in one table based on objects in another table. The
    /// user must specify matching column types from the two tables (i.e. the
    /// target table from which objects will be filtered and the source table
    /// based on which the filter will be created); the column names need not
    /// be the same. If a <see cref="view_name" /> is specified, then the
    /// filtered objects will then be put in a newly created view. The
    /// operation is synchronous, meaning that a response will not be returned
    /// until all objects are fully available in the result view. The return
    /// value contains the count (i.e. the size) of the resulting
    /// view.</summary>
    public class FilterByTableRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// <i>in_table</i> or <i>not_in_table</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NOT_IN_TABLE">NOT_IN_TABLE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MODE">MODE</see>:</term>
        ///         <description>Mode - should be either <i>spatial</i> or
        /// <i>normal</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.SPATIAL">SPATIAL</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER">BUFFER</see>:</term>
        ///         <description>Buffer size, in meters. Only relevant for
        /// <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER_METHOD">BUFFER_METHOD</see>:</term>
        ///         <description>Method used to buffer polygons.  Only relevant
        /// for <i>spatial</i> mode.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.GEOS">GEOS</see>:</term>
        ///         <description>Use geos 1 edge per corner
        /// algorithm</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SIZE">MAX_PARTITION_SIZE</see>:</term>
        ///         <description>Maximum number of points in a partition. Only
        /// relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SCORE">MAX_PARTITION_SCORE</see>:</term>
        ///         <description>Maximum number of points * edges in a
        /// partition. Only relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.X_COLUMN_NAME">X_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing x value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.Y_COLUMN_NAME">Y_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing y value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>String indicating the filter mode, either
            /// <i>in_table</i> or <i>not_in_table</i>.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.NOT_IN_TABLE">NOT_IN_TABLE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see>.</summary>
            public const string FILTER_MODE = "filter_mode";
            public const string IN_TABLE = "in_table";
            public const string NOT_IN_TABLE = "not_in_table";

            /// <summary>Mode - should be either <i>spatial</i> or
            /// <i>normal</i>.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.SPATIAL">SPATIAL</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</summary>
            public const string MODE = "mode";
            public const string NORMAL = "normal";
            public const string SPATIAL = "spatial";

            /// <summary>Buffer size, in meters. Only relevant for
            /// <i>spatial</i> mode.</summary>
            public const string BUFFER = "buffer";

            /// <summary>Method used to buffer polygons.  Only relevant for
            /// <i>spatial</i> mode.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="FilterByTableRequest.Options.GEOS">GEOS</see>:</term>
            ///         <description>Use geos 1 edge per corner
            /// algorithm</description>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</summary>
            public const string BUFFER_METHOD = "buffer_method";

            /// <summary>Use geos 1 edge per corner algorithm</summary>
            public const string GEOS = "geos";

            /// <summary>Maximum number of points in a partition. Only relevant
            /// for <i>spatial</i> mode.</summary>
            public const string MAX_PARTITION_SIZE = "max_partition_size";

            /// <summary>Maximum number of points * edges in a partition. Only
            /// relevant for <i>spatial</i> mode.</summary>
            public const string MAX_PARTITION_SCORE = "max_partition_score";

            /// <summary>Name of column containing x value of point being
            /// filtered in <i>spatial</i> mode.</summary>
            public const string X_COLUMN_NAME = "x_column_name";

            /// <summary>Name of column containing y value of point being
            /// filtered in <i>spatial</i> mode.</summary>
            public const string Y_COLUMN_NAME = "y_column_name";
        } // end struct Options


        /// <summary>Name of the table whose data will be filtered. Must be an
        /// existing table.  </summary>
        public string table_name { get; set; }

        /// <summary>If provided, then this will be the name of the view
        /// containing the results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.
        /// </summary>
        public string view_name { get; set; } = "";

        /// <summary>Name of the column by whose value the data will be
        /// filtered from the table designated by <paramref
        /// cref="FilterByTableRequest.table_name" />.  </summary>
        public string column_name { get; set; }

        /// <summary>Name of the table whose data will be compared against in
        /// the table called <paramref cref="FilterByTableRequest.table_name"
        /// />. Must be an existing table.  </summary>
        public string source_table_name { get; set; }

        /// <summary>Name of the column in the <paramref
        /// cref="FilterByTableRequest.source_table_name" /> whose values will
        /// be used as the filter for table <paramref
        /// cref="FilterByTableRequest.table_name" />. Must be a geospatial
        /// geometry column if in 'spatial' mode; otherwise, Must match the
        /// type of the <paramref cref="FilterByTableRequest.column_name" />.
        /// </summary>
        public string source_table_column_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// <i>in_table</i> or <i>not_in_table</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NOT_IN_TABLE">NOT_IN_TABLE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MODE">MODE</see>:</term>
        ///         <description>Mode - should be either <i>spatial</i> or
        /// <i>normal</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.SPATIAL">SPATIAL</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER">BUFFER</see>:</term>
        ///         <description>Buffer size, in meters. Only relevant for
        /// <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER_METHOD">BUFFER_METHOD</see>:</term>
        ///         <description>Method used to buffer polygons.  Only relevant
        /// for <i>spatial</i> mode.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.GEOS">GEOS</see>:</term>
        ///         <description>Use geos 1 edge per corner
        /// algorithm</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SIZE">MAX_PARTITION_SIZE</see>:</term>
        ///         <description>Maximum number of points in a partition. Only
        /// relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SCORE">MAX_PARTITION_SCORE</see>:</term>
        ///         <description>Maximum number of points * edges in a
        /// partition. Only relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.X_COLUMN_NAME">X_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing x value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.Y_COLUMN_NAME">Y_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing y value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a FilterByTableRequest object with default
        /// parameters.</summary>
        public FilterByTableRequest() { }

        /// <summary>Constructs a FilterByTableRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table whose data will be
        /// filtered. Must be an existing table.  </param>
        /// <param name="view_name">If provided, then this will be the name of
        /// the view containing the results. Has the same naming restrictions
        /// as <a href="../../concepts/tables.html" target="_top">tables</a>.
        /// </param>
        /// <param name="column_name">Name of the column by whose value the
        /// data will be filtered from the table designated by <paramref
        /// cref="FilterByTableRequest.table_name" />.  </param>
        /// <param name="source_table_name">Name of the table whose data will
        /// be compared against in the table called <paramref
        /// cref="FilterByTableRequest.table_name" />. Must be an existing
        /// table.  </param>
        /// <param name="source_table_column_name">Name of the column in the
        /// <paramref cref="FilterByTableRequest.source_table_name" /> whose
        /// values will be used as the filter for table <paramref
        /// cref="FilterByTableRequest.table_name" />. Must be a geospatial
        /// geometry column if in 'spatial' mode; otherwise, Must match the
        /// type of the <paramref cref="FilterByTableRequest.column_name" />.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.FILTER_MODE">FILTER_MODE</see>:</term>
        ///         <description>String indicating the filter mode, either
        /// <i>in_table</i> or <i>not_in_table</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NOT_IN_TABLE">NOT_IN_TABLE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.IN_TABLE">IN_TABLE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MODE">MODE</see>:</term>
        ///         <description>Mode - should be either <i>spatial</i> or
        /// <i>normal</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.SPATIAL">SPATIAL</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER">BUFFER</see>:</term>
        ///         <description>Buffer size, in meters. Only relevant for
        /// <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.BUFFER_METHOD">BUFFER_METHOD</see>:</term>
        ///         <description>Method used to buffer polygons.  Only relevant
        /// for <i>spatial</i> mode.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.GEOS">GEOS</see>:</term>
        ///         <description>Use geos 1 edge per corner
        /// algorithm</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="FilterByTableRequest.Options.NORMAL">NORMAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SIZE">MAX_PARTITION_SIZE</see>:</term>
        ///         <description>Maximum number of points in a partition. Only
        /// relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.MAX_PARTITION_SCORE">MAX_PARTITION_SCORE</see>:</term>
        ///         <description>Maximum number of points * edges in a
        /// partition. Only relevant for <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.X_COLUMN_NAME">X_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing x value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="FilterByTableRequest.Options.Y_COLUMN_NAME">Y_COLUMN_NAME</see>:</term>
        ///         <description>Name of column containing y value of point
        /// being filtered in <i>spatial</i> mode.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public FilterByTableRequest( string table_name,
                                     string view_name,
                                     string column_name,
                                     string source_table_name,
                                     string source_table_column_name,
                                     IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.view_name = view_name ?? "";
            this.column_name = column_name ?? "";
            this.source_table_name = source_table_name ?? "";
            this.source_table_column_name = source_table_column_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class FilterByTableRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.filterByTable(string,string,string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class FilterByTableResponse : KineticaData
    {

        /// <summary>The number of records in <paramref
        /// cref="FilterByTableRequest.table_name" /> that have <paramref
        /// cref="FilterByTableRequest.column_name" /> values matching
        /// <paramref cref="FilterByTableRequest.source_table_column_name" />
        /// values in <paramref cref="FilterByTableRequest.source_table_name"
        /// />.  </summary>
        public long count { get; set; }

    } // end class FilterByTableResponse




}  // end namespace kinetica
