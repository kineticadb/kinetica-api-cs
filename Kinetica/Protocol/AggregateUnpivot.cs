/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateUnpivot(string,string,string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Rotate the column values into rows values.
    /// <br />
    /// For unpivot details and examples, see <a
    /// href="../../concepts/unpivot.html" target="_top">Unpivot</a>.  For
    /// limitations, see <a href="../../concepts/unpivot.html#limitations"
    /// target="_top">Unpivot Limitations</a>.
    /// <br />
    /// Unpivot is used to normalize tables that are built for cross tabular
    /// reporting purposes. The unpivot operator rotates the column values for
    /// all the pivoted columns. A variable column, value column and all
    /// columns from the source table except the unpivot columns are projected
    /// into the result table. The variable column and value columns in the
    /// result table indicate the pivoted column name and values respectively.
    /// <br />
    /// The response is returned as a dynamic schema. For details see: <a
    /// href="../../api/index.html#dynamic-schemas" target="_top">dynamic
    /// schemas documentation</a>.</summary>
    public class AggregateUnpivotRequest : KineticaData
    {

        /// <summary>Specifies the encoding for returned records.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// binary encoded.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.JSON">JSON</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// json encoded.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>.
        /// A set of string constants for the parameter <see cref="encoding"
        /// />.</summary>
        public struct Encoding
        {

            /// <summary>Indicates that the returned records should be binary
            /// encoded.</summary>
            public const string BINARY = "binary";

            /// <summary>Indicates that the returned records should be json
            /// encoded.</summary>
            public const string JSON = "json";
        } // end struct Encoding


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// table specified in <i>result_table</i>. If the collection provided
        /// is non-existent, the collection will be automatically created. If
        /// empty, then the table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE">RESULT_TABLE</see>:</term>
        ///         <description>The name of the table used to store the
        /// results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>. If
        /// present, no results are returned in the response.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the result table
        /// specified in <i>result_table</i> will be persisted and will not
        /// expire unless a <i>ttl</i> is specified.   If <i>false</i>, then
        /// the result table will be an in-memory table and will expire unless
        /// a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Filter expression to apply to the table prior
        /// to unpivot processing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by; e.g. 'timestamp asc, x desc'.  The columns specified
        /// must be present in input table.  If any alias is given for any
        /// column name, the alias must be used, rather than the original
        /// column name.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for the
        /// result table. Must be used in combination with the
        /// <i>result_table</i> option.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>The number of records to keep.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in
        /// <i>result_table</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this result table is part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i> then the output columns will be
        /// cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the table
            /// specified in <i>result_table</i>. If the collection provided is
            /// non-existent, the collection will be automatically created. If
            /// empty, then the table will be a top-level table.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>The name of the table used to store the results. Has
            /// the same naming restrictions as <a
            /// href="../../concepts/tables.html" target="_top">tables</a>. If
            /// present, no results are returned in the response.</summary>
            public const string RESULT_TABLE = "result_table";

            /// <summary>If <i>true</i>, then the result table specified in
            /// <i>result_table</i> will be persisted and will not expire
            /// unless a <i>ttl</i> is specified.   If <i>false</i>, then the
            /// result table will be an in-memory table and will expire unless
            /// a <i>ttl</i> is specified otherwise.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</summary>
            public const string RESULT_TABLE_PERSIST = "result_table_persist";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Filter expression to apply to the table prior to
            /// unpivot processing.</summary>
            public const string EXPRESSION = "expression";

            /// <summary>Comma-separated list of the columns to be sorted by;
            /// e.g. 'timestamp asc, x desc'.  The columns specified must be
            /// present in input table.  If any alias is given for any column
            /// name, the alias must be used, rather than the original column
            /// name.</summary>
            public const string ORDER_BY = "order_by";

            /// <summary>Indicates the chunk size to be used for the result
            /// table. Must be used in combination with the <i>result_table</i>
            /// option.</summary>
            public const string CHUNK_SIZE = "chunk_size";

            /// <summary>The number of records to keep.</summary>
            public const string LIMIT = "limit";

            /// <summary>Sets the <a href="../../concepts/ttl.html"
            /// target="_top">TTL</a> of the table specified in
            /// <i>result_table</i>.</summary>
            public const string TTL = "ttl";

            /// <summary>view this result table is part of</summary>
            public const string VIEW_ID = "view_id";

            /// <summary>If <i>true</i> then the output columns will be cached
            /// on the GPU.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</summary>
            public const string MATERIALIZE_ON_GPU = "materialize_on_gpu";
        } // end struct Options


        /// <summary>Name of the table on which the operation will be
        /// performed. Must be an existing table/view.  </summary>
        public string table_name { get; set; }

        /// <summary>Specifies the variable/parameter column name.  </summary>
        public string variable_column_name { get; set; } = "";

        /// <summary>Specifies the value column name.  </summary>
        public string value_column_name { get; set; } = "";

        /// <summary>List of one or more values typically the column names of
        /// the input table. All the columns in the source table must have the
        /// same data type.  </summary>
        public IList<string> pivoted_columns { get; set; } = new List<string>();

        /// <summary>Specifies the encoding for returned records.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// binary encoded.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.JSON">JSON</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// json encoded.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>.
        /// </summary>
        public string encoding { get; set; } = Encoding.BINARY;

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// table specified in <i>result_table</i>. If the collection provided
        /// is non-existent, the collection will be automatically created. If
        /// empty, then the table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE">RESULT_TABLE</see>:</term>
        ///         <description>The name of the table used to store the
        /// results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>. If
        /// present, no results are returned in the response.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the result table
        /// specified in <i>result_table</i> will be persisted and will not
        /// expire unless a <i>ttl</i> is specified.   If <i>false</i>, then
        /// the result table will be an in-memory table and will expire unless
        /// a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Filter expression to apply to the table prior
        /// to unpivot processing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by; e.g. 'timestamp asc, x desc'.  The columns specified
        /// must be present in input table.  If any alias is given for any
        /// column name, the alias must be used, rather than the original
        /// column name.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for the
        /// result table. Must be used in combination with the
        /// <i>result_table</i> option.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>The number of records to keep.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in
        /// <i>result_table</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this result table is part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i> then the output columns will be
        /// cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AggregateUnpivotRequest object with default
        /// parameters.</summary>
        public AggregateUnpivotRequest() { }

        /// <summary>Constructs an AggregateUnpivotRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table/view.  </param>
        /// <param name="variable_column_name">Specifies the variable/parameter
        /// column name.  </param>
        /// <param name="value_column_name">Specifies the value column name.
        /// </param>
        /// <param name="pivoted_columns">List of one or more values typically
        /// the column names of the input table. All the columns in the source
        /// table must have the same data type.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// table specified in <i>result_table</i>. If the collection provided
        /// is non-existent, the collection will be automatically created. If
        /// empty, then the table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE">RESULT_TABLE</see>:</term>
        ///         <description>The name of the table used to store the
        /// results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>. If
        /// present, no results are returned in the response.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the result table
        /// specified in <i>result_table</i> will be persisted and will not
        /// expire unless a <i>ttl</i> is specified.   If <i>false</i>, then
        /// the result table will be an in-memory table and will expire unless
        /// a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Filter expression to apply to the table prior
        /// to unpivot processing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by; e.g. 'timestamp asc, x desc'.  The columns specified
        /// must be present in input table.  If any alias is given for any
        /// column name, the alias must be used, rather than the original
        /// column name.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for the
        /// result table. Must be used in combination with the
        /// <i>result_table</i> option.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>The number of records to keep.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in
        /// <i>result_table</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this result table is part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i> then the output columns will be
        /// cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public AggregateUnpivotRequest( string table_name,
                                        string variable_column_name,
                                        string value_column_name,
                                        IList<string> pivoted_columns,
                                        IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.variable_column_name = variable_column_name ?? "";
            this.value_column_name = value_column_name ?? "";
            this.pivoted_columns = pivoted_columns ?? new List<string>();
            this.encoding = Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor


        /// <summary>Constructs an AggregateUnpivotRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table/view.  </param>
        /// <param name="variable_column_name">Specifies the variable/parameter
        /// column name.  </param>
        /// <param name="value_column_name">Specifies the value column name.
        /// </param>
        /// <param name="pivoted_columns">List of one or more values typically
        /// the column names of the input table. All the columns in the source
        /// table must have the same data type.  </param>
        /// <param name="encoding">Specifies the encoding for returned records.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// binary encoded.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Encoding.JSON">JSON</see>:</term>
        ///         <description>Indicates that the returned records should be
        /// json encoded.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Encoding.BINARY">BINARY</see>.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// table specified in <i>result_table</i>. If the collection provided
        /// is non-existent, the collection will be automatically created. If
        /// empty, then the table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE">RESULT_TABLE</see>:</term>
        ///         <description>The name of the table used to store the
        /// results. Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>. If
        /// present, no results are returned in the response.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the result table
        /// specified in <i>result_table</i> will be persisted and will not
        /// expire unless a <i>ttl</i> is specified.   If <i>false</i>, then
        /// the result table will be an in-memory table and will expire unless
        /// a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.EXPRESSION">EXPRESSION</see>:</term>
        ///         <description>Filter expression to apply to the table prior
        /// to unpivot processing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.ORDER_BY">ORDER_BY</see>:</term>
        ///         <description>Comma-separated list of the columns to be
        /// sorted by; e.g. 'timestamp asc, x desc'.  The columns specified
        /// must be present in input table.  If any alias is given for any
        /// column name, the alias must be used, rather than the original
        /// column name.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for the
        /// result table. Must be used in combination with the
        /// <i>result_table</i> option.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.LIMIT">LIMIT</see>:</term>
        ///         <description>The number of records to keep.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in
        /// <i>result_table</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this result table is part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i> then the output columns will be
        /// cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AggregateUnpivotRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public AggregateUnpivotRequest( string table_name,
                                        string variable_column_name,
                                        string value_column_name,
                                        IList<string> pivoted_columns,
                                        string encoding = null,
                                        IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.variable_column_name = variable_column_name ?? "";
            this.value_column_name = value_column_name ?? "";
            this.pivoted_columns = pivoted_columns ?? new List<string>();
            this.encoding = encoding ?? Encoding.BINARY;
            this.options = options ?? new Dictionary<string, string>();
        } // end full constructor

    } // end class AggregateUnpivotRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateUnpivot(string,string,string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class RawAggregateUnpivotResponse : KineticaData
    {

        /// <summary>Typically shows the result-table name if provided in the
        /// request (Ignore otherwise).  </summary>
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

    } // end class RawAggregateUnpivotResponse



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateUnpivot(string,string,string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class AggregateUnpivotResponse : KineticaData
    {

        /// <summary>Typically shows the result-table name if provided in the
        /// request (Ignore otherwise).  </summary>
        public string table_name { get; set; }

        /// <summary>Avro binary encoded response.  </summary>
        public IList<KineticaRecord> data { get; set; } = new List<KineticaRecord>();

        /// <summary>Total/Filtered number of records.  </summary>
        public long total_number_of_records { get; set; }

        /// <summary>Too many records. Returned a partial set.  </summary>
        public bool has_more_records { get; set; }

    } // end class AggregateUnpivotResponse




}  // end namespace kinetica
