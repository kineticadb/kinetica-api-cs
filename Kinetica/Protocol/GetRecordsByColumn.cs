/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.getRecordsByColumn(string,IList{string},long,long,IDictionary{string, string})"
    /// />.
    /// <br />
    /// For a given table, retrieves the values from the requested
    /// column(s). Maps of column name to the array of values as well as the
    /// column data
    /// type are returned. This endpoint supports pagination with the <see
    /// cref="offset" />
    /// and <see cref="limit" /> parameters.
    /// <br />
    /// <a href="../../../concepts/window/" target="_top">Window functions</a>,
    /// which can perform
    /// operations like moving averages, are available through this endpoint as
    /// well as
    /// <see
    /// cref="Kinetica.createProjection(string,string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// When using pagination, if the table (or the underlying table in the
    /// case of a
    /// view) is modified (records are inserted, updated, or deleted) during a
    /// call to
    /// the endpoint, the records or values retrieved may differ between calls
    /// based on
    /// the type of the update, e.g., the contiguity across pages cannot be
    /// relied upon.
    /// <br />
    /// If <see cref="table_name" /> is empty, selection is performed against a
    /// single-row
    /// virtual table.  This can be useful in executing temporal
    /// (<a href="../../../concepts/expressions/#date-time-functions"
    /// target="_top">NOW()</a>), identity
    /// (<a href="../../../concepts/expressions/#user-security-functions"
    /// target="_top">USER()</a>), or
    /// constant-based functions
    /// (<a href="../../../concepts/expressions/#scalar-functions"
    /// target="_top">GEODIST(-77.11, 38.88, -71.06, 42.36)</a>).
    /// <br />
    /// The response is returned as a dynamic schema. For details see:
    /// <a href="../../../api/#dynamic-schemas" target="_top">dynamic schemas
    /// documentation</a>.</summary>
    public class GetRecordsByColumnRequest : KineticaData
    {

        /// <summary>Specifies the encoding for returned records; either
        /// <i>binary</i> or <i>json</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see>.
        /// A set of string constants for the parameter <see cref="encoding"
        /// />.</summary>
        public struct Encoding
        {
            public const string BINARY = "binary";
            public const string JSON = "json";
        } // end struct Encoding


        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Used in conjunction with <i>sort_order</i>. The <i>order_by</i>
        /// option can be used in lieu of <i>sort_by</i> / <i>sort_order</i>.
        /// The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - <i>ascending</i> or <i>descending</i>. If
        /// <i>sort_order</i> is provided, <i>sort_by</i> has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by as well as the sort direction, e.g., 'timestamp asc, x
        /// desc'.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.CONVERT_WKTS_TO_WKBS">CONVERT_WKTS_TO_WKBS</see>:</term>
        ///         <description>If <i>true</i>, then WKT string columns will
        /// be returned as WKB bytes.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Optional filter expression to apply to the
            /// table.</summary>
            public const string EXPRESSION = "expression";

            /// <summary>Optional column that the data should be sorted by.
            /// Used in conjunction with <i>sort_order</i>. The <i>order_by</i>
            /// option can be used in lieu of <i>sort_by</i> /
            /// <i>sort_order</i>.  The default value is ''.</summary>
            public const string SORT_BY = "sort_by";

            /// <summary>String indicating how the returned values should be
            /// sorted - <i>ascending</i> or <i>descending</i>. If
            /// <i>sort_order</i> is provided, <i>sort_by</i> has to be
            /// provided.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsByColumnRequest.Options.DESCENDING">DESCENDING</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see>.</summary>
            public const string SORT_ORDER = "sort_order";
            public const string ASCENDING = "ascending";
            public const string DESCENDING = "descending";

            /// <summary>Comma-separated list of the columns to be sorted by as
            /// well as the sort direction, e.g., 'timestamp asc, x desc'.  The
            /// default value is ''.</summary>
            public const string ORDER_BY = "order_by";

            /// <summary>If <i>true</i>, then WKT string columns will be
            /// returned as WKB bytes.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsByColumnRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see>.</summary>
            public const string CONVERT_WKTS_TO_WKBS = "convert_wkts_to_wkbs";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the table or view on which this operation will be
        /// performed, in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  An empty table name
        /// retrieves one record from a single-row virtual table, where columns
        /// specified should be constants or constant expressions.  </summary>
        public string table_name { get; set; }

        /// <summary>The list of column values to retrieve.  </summary>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>A positive integer indicating the number of initial
        /// results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </summary>
        public long offset { get; set; } = 0;

        /// <summary>A positive integer indicating the maximum number of
        /// results to be returned, or END_OF_SET (-9999) to indicate that the
        /// maximum number of results allowed by the server should be returned.
        /// The number of records returned will never exceed the server's own
        /// limit, defined by the <a href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsByColumnRequest.offset" /> & <paramref
        /// cref="GetRecordsByColumnRequest.limit" /> to request subsequent
        /// pages of results.  The default value is -9999.</summary>
        public long limit { get; set; } = -9999;

        /// <summary>Specifies the encoding for returned records; either
        /// <i>binary</i> or <i>json</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see>.
        /// </summary>
        public string encoding { get; set; } = Encoding.BINARY;

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Used in conjunction with <i>sort_order</i>. The <i>order_by</i>
        /// option can be used in lieu of <i>sort_by</i> / <i>sort_order</i>.
        /// The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - <i>ascending</i> or <i>descending</i>. If
        /// <i>sort_order</i> is provided, <i>sort_by</i> has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by as well as the sort direction, e.g., 'timestamp asc, x
        /// desc'.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.CONVERT_WKTS_TO_WKBS">CONVERT_WKTS_TO_WKBS</see>:</term>
        ///         <description>If <i>true</i>, then WKT string columns will
        /// be returned as WKB bytes.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a GetRecordsByColumnRequest object with default
        /// parameters.</summary>
        public GetRecordsByColumnRequest() { }

        /// <summary>Constructs a GetRecordsByColumnRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table or view on which this
        /// operation will be performed, in [schema_name.]table_name format,
        /// using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  An empty table name
        /// retrieves one record from a single-row virtual table, where columns
        /// specified should be constants or constant expressions.  </param>
        /// <param name="column_names">The list of column values to retrieve.
        /// </param>
        /// <param name="offset">A positive integer indicating the number of
        /// initial results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </param>
        /// <param name="limit">A positive integer indicating the maximum
        /// number of results to be returned, or END_OF_SET (-9999) to indicate
        /// that the maximum number of results allowed by the server should be
        /// returned.  The number of records returned will never exceed the
        /// server's own limit, defined by the <a
        /// href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsByColumnRequest.offset" /> & <paramref
        /// cref="GetRecordsByColumnRequest.limit" /> to request subsequent
        /// pages of results.  The default value is -9999.</param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Used in conjunction with <i>sort_order</i>. The <i>order_by</i>
        /// option can be used in lieu of <i>sort_by</i> / <i>sort_order</i>.
        /// The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - <i>ascending</i> or <i>descending</i>. If
        /// <i>sort_order</i> is provided, <i>sort_by</i> has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by as well as the sort direction, e.g., 'timestamp asc, x
        /// desc'.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.CONVERT_WKTS_TO_WKBS">CONVERT_WKTS_TO_WKBS</see>:</term>
        ///         <description>If <i>true</i>, then WKT string columns will
        /// be returned as WKB bytes.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public GetRecordsByColumnRequest( string table_name,
                                          IList<string> column_names,
                                          long? offset = null,
                                          long? limit = null,
                                          IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_names = column_names ?? new List<string>();
            this.offset = offset ?? 0;
            this.limit = limit ?? -9999;
            this.encoding = Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor


        /// <summary>Constructs a GetRecordsByColumnRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table or view on which this
        /// operation will be performed, in [schema_name.]table_name format,
        /// using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  An empty table name
        /// retrieves one record from a single-row virtual table, where columns
        /// specified should be constants or constant expressions.  </param>
        /// <param name="column_names">The list of column values to retrieve.
        /// </param>
        /// <param name="offset">A positive integer indicating the number of
        /// initial results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </param>
        /// <param name="limit">A positive integer indicating the maximum
        /// number of results to be returned, or END_OF_SET (-9999) to indicate
        /// that the maximum number of results allowed by the server should be
        /// returned.  The number of records returned will never exceed the
        /// server's own limit, defined by the <a
        /// href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsByColumnRequest.offset" /> & <paramref
        /// cref="GetRecordsByColumnRequest.limit" /> to request subsequent
        /// pages of results.  The default value is -9999.</param>
        /// <param name="encoding">Specifies the encoding for returned records;
        /// either <i>binary</i> or <i>json</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Encoding.BINARY">BINARY</see>.
        /// </param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Used in conjunction with <i>sort_order</i>. The <i>order_by</i>
        /// option can be used in lieu of <i>sort_by</i> / <i>sort_order</i>.
        /// The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - <i>ascending</i> or <i>descending</i>. If
        /// <i>sort_order</i> is provided, <i>sort_by</i> has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by as well as the sort direction, e.g., 'timestamp asc, x
        /// desc'.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.CONVERT_WKTS_TO_WKBS">CONVERT_WKTS_TO_WKBS</see>:</term>
        ///         <description>If <i>true</i>, then WKT string columns will
        /// be returned as WKB bytes.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsByColumnRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public GetRecordsByColumnRequest( string table_name,
                                          IList<string> column_names,
                                          long? offset = null,
                                          long? limit = null,
                                          string encoding = null,
                                          IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_names = column_names ?? new List<string>();
            this.offset = offset ?? 0;
            this.limit = limit ?? -9999;
            this.encoding = encoding ?? Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end full constructor

    } // end class GetRecordsByColumnRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.getRecordsByColumn(string,IList{string},long,long,IDictionary{string, string})"
    /// />.</summary>
    public class RawGetRecordsByColumnResponse : KineticaData
    {

        /// <summary>The same table name as was passed in the parameter list.
        /// </summary>
        public string table_name { get; set; }

        /// <summary>Avro schema of <member name="binary_encoded_response" />
        /// or <member name="json_encoded_response" />.  </summary>
        public string response_schema_str { get; set; }

        /// <summary>Avro binary encoded response.  </summary>
        public byte[] binary_encoded_response { get; set; }

        /// <summary>Avro JSON encoded response.  </summary>
        public string json_encoded_response { get; set; }

        /// <summary>Total/Filtered number of records.  </summary>
        public long total_number_of_records { get; set; }

        /// <summary>Too many records. Returned a partial set.  </summary>
        public bool has_more_records { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class RawGetRecordsByColumnResponse



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.getRecordsByColumn(string,IList{string},long,long,IDictionary{string, string})"
    /// />.</summary>
    public class GetRecordsByColumnResponse : KineticaData
    {

        /// <summary>The same table name as was passed in the parameter list.
        /// </summary>
        public string table_name { get; set; }

        /// <summary>Avro binary encoded response.  </summary>
        public IList<KineticaRecord> data { get; set; } = new List<KineticaRecord>();

        /// <summary>Total/Filtered number of records.  </summary>
        public long total_number_of_records { get; set; }

        /// <summary>Too many records. Returned a partial set.  </summary>
        public bool has_more_records { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class GetRecordsByColumnResponse




}  // end namespace kinetica
