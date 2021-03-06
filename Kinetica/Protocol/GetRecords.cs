/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.getRecords{T}(string,long,long,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Retrieves records from a given table, optionally filtered by an
    /// expression and/or sorted by a column. This operation can be performed
    /// on tables
    /// and views. Records can be returned encoded as binary, json, or geojson.
    /// <br />
    /// This operation supports paging through the data via the <see
    /// cref="offset" /> and
    /// <see cref="limit" /> parameters.  Note that when paging through a
    /// table, if the table
    /// (or the underlying table in case of a view) is updated (records are
    /// inserted,
    /// deleted or modified) the records retrieved may differ between calls
    /// based on the
    /// updates applied.</summary>
    public class GetRecordsRequest : KineticaData
    {

        /// <summary>Specifies the encoding for returned records; one of
        /// <i>binary</i>, <i>json</i>, or <i>geojson</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.GEOJSON">GEOJSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see>.
        /// A set of string constants for the parameter <see cref="encoding"
        /// />.</summary>
        public struct Encoding
        {
            public const string BINARY = "binary";
            public const string JSON = "json";
            public const string GEOJSON = "geojson";
        } // end struct Encoding


        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FAST_INDEX_LOOKUP">FAST_INDEX_LOOKUP</see>:</term>
        ///         <description>Indicates if indexes should be used to perform
        /// the lookup for a given expression if possible. Only applicable if
        /// there is no sorting, the expression contains only equivalence
        /// comparisons based on existing tables indexes and the range of
        /// requested values is from [0 to END_OF_SET].
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Empty by default (i.e. no sorting is applied).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - ascending or descending. If sort_order is
        /// provided, sort_by has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see>.</description>
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

            /// <summary>Indicates if indexes should be used to perform the
            /// lookup for a given expression if possible. Only applicable if
            /// there is no sorting, the expression contains only equivalence
            /// comparisons based on existing tables indexes and the range of
            /// requested values is from [0 to END_OF_SET].
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="GetRecordsRequest.Options.TRUE">TRUE</see>.</summary>
            public const string FAST_INDEX_LOOKUP = "fast_index_lookup";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Optional column that the data should be sorted by.
            /// Empty by default (i.e. no sorting is applied).</summary>
            public const string SORT_BY = "sort_by";

            /// <summary>String indicating how the returned values should be
            /// sorted - ascending or descending. If sort_order is provided,
            /// sort_by has to be provided.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="GetRecordsRequest.Options.DESCENDING">DESCENDING</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see>.</summary>
            public const string SORT_ORDER = "sort_order";
            public const string ASCENDING = "ascending";
            public const string DESCENDING = "descending";
        } // end struct Options


        /// <summary>Name of the table or view from which the records will be
        /// fetched, in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  </summary>
        public string table_name { get; set; }

        /// <summary>A positive integer indicating the number of initial
        /// results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </summary>
        public long offset { get; set; } = 0;

        /// <summary>A positive integer indicating the maximum number of
        /// results to be returned. Or END_OF_SET (-9999) to indicate that the
        /// max number of results should be returned.  The number of records
        /// returned will never exceed the server's own limit, defined by the
        /// <a href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsRequest.offset" /> & <paramref
        /// cref="GetRecordsRequest.limit" /> to request subsequent pages of
        /// results.  The default value is -9999.</summary>
        public long limit { get; set; } = -9999;

        /// <summary>Specifies the encoding for returned records; one of
        /// <i>binary</i>, <i>json</i>, or <i>geojson</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.GEOJSON">GEOJSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see>.  </summary>
        public string encoding { get; set; } = Encoding.BINARY;

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FAST_INDEX_LOOKUP">FAST_INDEX_LOOKUP</see>:</term>
        ///         <description>Indicates if indexes should be used to perform
        /// the lookup for a given expression if possible. Only applicable if
        /// there is no sorting, the expression contains only equivalence
        /// comparisons based on existing tables indexes and the range of
        /// requested values is from [0 to END_OF_SET].
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Empty by default (i.e. no sorting is applied).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - ascending or descending. If sort_order is
        /// provided, sort_by has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a GetRecordsRequest object with default
        /// parameters.</summary>
        public GetRecordsRequest() { }

        /// <summary>Constructs a GetRecordsRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table or view from which the
        /// records will be fetched, in [schema_name.]table_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  </param>
        /// <param name="offset">A positive integer indicating the number of
        /// initial results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </param>
        /// <param name="limit">A positive integer indicating the maximum
        /// number of results to be returned. Or END_OF_SET (-9999) to indicate
        /// that the max number of results should be returned.  The number of
        /// records returned will never exceed the server's own limit, defined
        /// by the <a href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsRequest.offset" /> & <paramref
        /// cref="GetRecordsRequest.limit" /> to request subsequent pages of
        /// results.  The default value is -9999.</param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FAST_INDEX_LOOKUP">FAST_INDEX_LOOKUP</see>:</term>
        ///         <description>Indicates if indexes should be used to perform
        /// the lookup for a given expression if possible. Only applicable if
        /// there is no sorting, the expression contains only equivalence
        /// comparisons based on existing tables indexes and the range of
        /// requested values is from [0 to END_OF_SET].
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Empty by default (i.e. no sorting is applied).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - ascending or descending. If sort_order is
        /// provided, sort_by has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public GetRecordsRequest( string table_name,
                                  long? offset = null,
                                  long? limit = null,
                                  IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.offset = offset ?? 0;
            this.limit = limit ?? -9999;
            this.encoding = Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor


        /// <summary>Constructs a GetRecordsRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table or view from which the
        /// records will be fetched, in [schema_name.]table_name format, using
        /// standard <a href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  </param>
        /// <param name="offset">A positive integer indicating the number of
        /// initial results to skip (this can be useful for paging through the
        /// results).  The default value is 0.The minimum allowed value is 0.
        /// The maximum allowed value is MAX_INT. </param>
        /// <param name="limit">A positive integer indicating the maximum
        /// number of results to be returned. Or END_OF_SET (-9999) to indicate
        /// that the max number of results should be returned.  The number of
        /// records returned will never exceed the server's own limit, defined
        /// by the <a href="../../../config/#general"
        /// target="_top">max_get_records_size</a> parameter in the server
        /// configuration.  Use <member name="has_more_records" /> to see if
        /// more records exist in the result to be fetched, and <paramref
        /// cref="GetRecordsRequest.offset" /> & <paramref
        /// cref="GetRecordsRequest.limit" /> to request subsequent pages of
        /// results.  The default value is -9999.</param>
        /// <param name="encoding">Specifies the encoding for returned records;
        /// one of <i>binary</i>, <i>json</i>, or <i>geojson</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.JSON">JSON</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Encoding.GEOJSON">GEOJSON</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Encoding.BINARY">BINARY</see>.  </param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Optional filter expression to apply to the
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FAST_INDEX_LOOKUP">FAST_INDEX_LOOKUP</see>:</term>
        ///         <description>Indicates if indexes should be used to perform
        /// the lookup for a given expression if possible. Only applicable if
        /// there is no sorting, the expression contains only equivalence
        /// comparisons based on existing tables indexes and the range of
        /// requested values is from [0 to END_OF_SET].
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_BY">SORT_BY</see>:</term>
        ///         <description>Optional column that the data should be sorted
        /// by. Empty by default (i.e. no sorting is applied).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.SORT_ORDER">SORT_ORDER</see>:</term>
        ///         <description>String indicating how the returned values
        /// should be sorted - ascending or descending. If sort_order is
        /// provided, sort_by has to be provided.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GetRecordsRequest.Options.DESCENDING">DESCENDING</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="GetRecordsRequest.Options.ASCENDING">ASCENDING</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public GetRecordsRequest( string table_name,
                                  long? offset = null,
                                  long? limit = null,
                                  string encoding = null,
                                  IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.offset = offset ?? 0;
            this.limit = limit ?? -9999;
            this.encoding = encoding ?? Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end full constructor

    } // end class GetRecordsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.getRecords{T}(string,long,long,IDictionary{string, string})"
    /// />.</summary>
    public class RawGetRecordsResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="GetRecordsRequest.table_name" />.
        /// </summary>
        public string table_name { get; set; }
        public string type_name { get; set; }

        /// <summary>Avro schema of <member name="records_binary" /> or <member
        /// name="records_json" />  </summary>
        public string type_schema { get; set; }

        /// <summary>If the <paramref cref="GetRecordsRequest.encoding" /> was
        /// 'binary', then this list contains the binary encoded records
        /// retrieved from the table, otherwise not populated.  </summary>
        public IList<byte[]> records_binary { get; set; } = new List<byte[]>();

        /// <summary>If the <paramref cref="GetRecordsRequest.encoding" /> was
        /// 'json', then this list contains the JSON encoded records retrieved
        /// from the table. If the <paramref cref="GetRecordsRequest.encoding"
        /// /> was 'geojson' this list contains a single entry consisting of a
        /// GeoJSON FeatureCollection containing a feature per record.
        /// Otherwise not populated.  </summary>
        public IList<string> records_json { get; set; } = new List<string>();

        /// <summary>Total/Filtered number of records.  </summary>
        public long total_number_of_records { get; set; }

        /// <summary>Too many records. Returned a partial set.  </summary>
        public bool has_more_records { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class RawGetRecordsResponse



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.getRecords{T}(string,long,long,IDictionary{string, string})"
    /// />.</summary>
    /// 
    /// <typeparam name="T">The type of object being processed.</typeparam>
    /// 
    public class GetRecordsResponse<T> : KineticaData
    {

        /// <summary>Value of <paramref cref="GetRecordsRequest.table_name" />.
        /// </summary>
        public string table_name { get; set; }
        public string type_name { get; set; }

        /// <summary>Avro schema of <member name="records_binary" /> or <member
        /// name="records_json" />  </summary>
        public string type_schema { get; set; }

        /// <summary>If the <paramref cref="GetRecordsRequest.encoding" /> was
        /// 'binary', then this list contains the binary encoded records
        /// retrieved from the table, otherwise not populated.  </summary>
        public IList<T> data { get; set; } = new List<T>();

        /// <summary>Total/Filtered number of records.  </summary>
        public long total_number_of_records { get; set; }

        /// <summary>Too many records. Returned a partial set.  </summary>
        public bool has_more_records { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class GetRecordsResponse




}  // end namespace kinetica
