/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createUnion(string,IList{string},IList{IList{string}},IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Merges data from one or more tables with comparable data types into a
    /// new table.
    /// <br />
    /// The following merges are supported:
    /// <br />
    /// UNION (DISTINCT/ALL) - For data set union details and examples, see <a
    /// href="../../concepts/unions.html" target="_top">Union</a>.  For
    /// limitations, see <a
    /// href="../../concepts/unions.html#limitations-and-cautions"
    /// target="_top">Union Limitations and Cautions</a>.
    /// <br />
    /// INTERSECT (DISTINCT/ALL) - For data set intersection details and
    /// examples, see <a href="../../concepts/intersect.html"
    /// target="_top">Intersect</a>.  For limitations, see <a
    /// href="../../concepts/intersect.html#limitations"
    /// target="_top">Intersect Limitations</a>.
    /// <br />
    /// EXCEPT (DISTINCT/ALL) - For data set subtraction details and examples,
    /// see <a href="../../concepts/except.html" target="_top">Except</a>.  For
    /// limitations, see <a href="../../concepts/except.html#limitations"
    /// target="_top">Except Limitations</a>.
    /// <br />
    /// MERGE VIEWS - For a given set of <a
    /// href="../../concepts/filtered_views.html" target="_top">filtered
    /// views</a> on a single table, creates a single filtered view containing
    /// all of the unique records across all of the given filtered data sets.
    /// <br />
    /// Non-charN 'string' and 'bytes' column types cannot be merged, nor can
    /// columns marked as <a href="../../concepts/types.html#data-handling"
    /// target="_top">store-only</a>.</summary>
    public class CreateUnionRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// output table. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, the output
        /// table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i>, then the columns of the output
        /// table will be cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MODE">MODE</see>:</term>
        ///         <description>If <i>merge_views</i>, then this operation
        /// will merge the provided views. All <paramref
        /// cref="CreateUnionRequest.table_names" /> must be views from the
        /// same underlying base table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>:</term>
        ///         <description>Retains all rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION">UNION</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables (synonym for <i>union_distinct</i>).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_DISTINCT">UNION_DISTINCT</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT">EXCEPT</see>:</term>
        ///         <description>Retains all unique rows from the first table
        /// that do not appear in the second table (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT_ALL">EXCEPT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) from
        /// the first table that do not appear in the second table (only works
        /// on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT">INTERSECT</see>:</term>
        ///         <description>Retains all unique rows that appear in both of
        /// the specified tables (only works on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT_ALL">INTERSECT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) that
        /// appear in both of the specified tables (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MERGE_VIEWS">MERGE_VIEWS</see>:</term>
        ///         <description>Merge two or more views (or views of views) of
        /// the same base data set into a new view. If this mode is selected
        /// <paramref cref="CreateUnionRequest.input_column_names" /> AND
        /// <paramref cref="CreateUnionRequest.output_column_names" /> must be
        /// empty. The resulting view would match the results of a SQL OR
        /// operation, e.g., if filter 1 creates a view using the expression 'x
        /// = 20' and filter 2 creates a view using the expression 'x <= 10',
        /// then the merge views operation creates a new view using the
        /// expression 'x = 20 OR x <= 10'.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CREATE_INDEXES">CREATE_INDEXES</see>:</term>
        ///         <description>Comma-separated list of columns on which to
        /// create indexes on the output table.  The columns specified must be
        /// present in <paramref cref="CreateUnionRequest.output_column_names"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in <paramref
        /// cref="CreateUnionRequest.table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.PERSIST">PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be persisted
        /// and will not expire unless a <i>ttl</i> is specified.   If
        /// <i>false</i>, then the table will be an in-memory table and will
        /// expire unless a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view the output table will be a part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FORCE_REPLICATED">FORCE_REPLICATED</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be
        /// replicated even if the source tables are not.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the output
            /// table. If the collection provided is non-existent, the
            /// collection will be automatically created. If empty, the output
            /// table will be a top-level table.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>If <i>true</i>, then the columns of the output table
            /// will be cached on the GPU.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</summary>
            public const string MATERIALIZE_ON_GPU = "materialize_on_gpu";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <i>merge_views</i>, then this operation will merge
            /// the provided views. All <see cref="table_names" /> must be
            /// views from the same underlying base table.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>:</term>
            ///         <description>Retains all rows from the specified
            /// tables.</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.UNION">UNION</see>:</term>
            ///         <description>Retains all unique rows from the specified
            /// tables (synonym for <i>union_distinct</i>).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.UNION_DISTINCT">UNION_DISTINCT</see>:</term>
            ///         <description>Retains all unique rows from the specified
            /// tables.</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.EXCEPT">EXCEPT</see>:</term>
            ///         <description>Retains all unique rows from the first
            /// table that do not appear in the second table (only works on 2
            /// tables).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.EXCEPT_ALL">EXCEPT_ALL</see>:</term>
            ///         <description>Retains all rows(including duplicates)
            /// from the first table that do not appear in the second table
            /// (only works on 2 tables).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.INTERSECT">INTERSECT</see>:</term>
            ///         <description>Retains all unique rows that appear in
            /// both of the specified tables (only works on 2
            /// tables).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.INTERSECT_ALL">INTERSECT_ALL</see>:</term>
            ///         <description>Retains all rows(including duplicates)
            /// that appear in both of the specified tables (only works on 2
            /// tables).</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.MERGE_VIEWS">MERGE_VIEWS</see>:</term>
            ///         <description>Merge two or more views (or views of
            /// views) of the same base data set into a new view. If this mode
            /// is selected <see cref="input_column_names" /> AND <see
            /// cref="output_column_names" /> must be empty. The resulting view
            /// would match the results of a SQL OR operation, e.g., if filter
            /// 1 creates a view using the expression 'x = 20' and filter 2
            /// creates a view using the expression 'x <= 10', then the merge
            /// views operation creates a new view using the expression 'x = 20
            /// OR x <= 10'.</description>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>.</summary>
            public const string MODE = "mode";

            /// <summary>Retains all rows from the specified tables.</summary>
            public const string UNION_ALL = "union_all";

            /// <summary>Retains all unique rows from the specified tables
            /// (synonym for <i>union_distinct</i>).</summary>
            public const string UNION = "union";

            /// <summary>Retains all unique rows from the specified
            /// tables.</summary>
            public const string UNION_DISTINCT = "union_distinct";

            /// <summary>Retains all unique rows from the first table that do
            /// not appear in the second table (only works on 2
            /// tables).</summary>
            public const string EXCEPT = "except";

            /// <summary>Retains all rows(including duplicates) from the first
            /// table that do not appear in the second table (only works on 2
            /// tables).</summary>
            public const string EXCEPT_ALL = "except_all";

            /// <summary>Retains all unique rows that appear in both of the
            /// specified tables (only works on 2 tables).</summary>
            public const string INTERSECT = "intersect";

            /// <summary>Retains all rows(including duplicates) that appear in
            /// both of the specified tables (only works on 2
            /// tables).</summary>
            public const string INTERSECT_ALL = "intersect_all";

            /// <summary>Merge two or more views (or views of views) of the
            /// same base data set into a new view. If this mode is selected
            /// <see cref="input_column_names" /> AND <see
            /// cref="output_column_names" /> must be empty. The resulting view
            /// would match the results of a SQL OR operation, e.g., if filter
            /// 1 creates a view using the expression 'x = 20' and filter 2
            /// creates a view using the expression 'x <= 10', then the merge
            /// views operation creates a new view using the expression 'x = 20
            /// OR x <= 10'.</summary>
            public const string MERGE_VIEWS = "merge_views";

            /// <summary>Indicates the chunk size to be used for this
            /// table.</summary>
            public const string CHUNK_SIZE = "chunk_size";

            /// <summary>Comma-separated list of columns on which to create
            /// indexes on the output table.  The columns specified must be
            /// present in <see cref="output_column_names" />.</summary>
            public const string CREATE_INDEXES = "create_indexes";

            /// <summary>Sets the <a href="../../concepts/ttl.html"
            /// target="_top">TTL</a> of the table specified in <see
            /// cref="table_name" />.</summary>
            public const string TTL = "ttl";

            /// <summary>If <i>true</i>, then the table specified in <see
            /// cref="table_name" /> will be persisted and will not expire
            /// unless a <i>ttl</i> is specified.   If <i>false</i>, then the
            /// table will be an in-memory table and will expire unless a
            /// <i>ttl</i> is specified otherwise.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</summary>
            public const string PERSIST = "persist";

            /// <summary>view the output table will be a part of</summary>
            public const string VIEW_ID = "view_id";

            /// <summary>If <i>true</i>, then the table specified in <see
            /// cref="table_name" /> will be replicated even if the source
            /// tables are not.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</summary>
            public const string FORCE_REPLICATED = "force_replicated";
        } // end struct Options


        /// <summary>Name of the table to be created. Has the same naming
        /// restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>.  </summary>
        public string table_name { get; set; }

        /// <summary>The list of table names to merge. Must contain the names
        /// of one or more existing tables.  </summary>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>The list of columns from each of the corresponding input
        /// tables.  </summary>
        public IList<IList<string>> input_column_names { get; set; } = new List<IList<string>>();

        /// <summary>The list of names of the columns to be stored in the
        /// output table.  </summary>
        public IList<string> output_column_names { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// output table. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, the output
        /// table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i>, then the columns of the output
        /// table will be cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MODE">MODE</see>:</term>
        ///         <description>If <i>merge_views</i>, then this operation
        /// will merge the provided views. All <paramref
        /// cref="CreateUnionRequest.table_names" /> must be views from the
        /// same underlying base table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>:</term>
        ///         <description>Retains all rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION">UNION</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables (synonym for <i>union_distinct</i>).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_DISTINCT">UNION_DISTINCT</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT">EXCEPT</see>:</term>
        ///         <description>Retains all unique rows from the first table
        /// that do not appear in the second table (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT_ALL">EXCEPT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) from
        /// the first table that do not appear in the second table (only works
        /// on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT">INTERSECT</see>:</term>
        ///         <description>Retains all unique rows that appear in both of
        /// the specified tables (only works on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT_ALL">INTERSECT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) that
        /// appear in both of the specified tables (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MERGE_VIEWS">MERGE_VIEWS</see>:</term>
        ///         <description>Merge two or more views (or views of views) of
        /// the same base data set into a new view. If this mode is selected
        /// <paramref cref="CreateUnionRequest.input_column_names" /> AND
        /// <paramref cref="CreateUnionRequest.output_column_names" /> must be
        /// empty. The resulting view would match the results of a SQL OR
        /// operation, e.g., if filter 1 creates a view using the expression 'x
        /// = 20' and filter 2 creates a view using the expression 'x <= 10',
        /// then the merge views operation creates a new view using the
        /// expression 'x = 20 OR x <= 10'.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CREATE_INDEXES">CREATE_INDEXES</see>:</term>
        ///         <description>Comma-separated list of columns on which to
        /// create indexes on the output table.  The columns specified must be
        /// present in <paramref cref="CreateUnionRequest.output_column_names"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in <paramref
        /// cref="CreateUnionRequest.table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.PERSIST">PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be persisted
        /// and will not expire unless a <i>ttl</i> is specified.   If
        /// <i>false</i>, then the table will be an in-memory table and will
        /// expire unless a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view the output table will be a part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FORCE_REPLICATED">FORCE_REPLICATED</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be
        /// replicated even if the source tables are not.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateUnionRequest object with default
        /// parameters.</summary>
        public CreateUnionRequest() { }

        /// <summary>Constructs a CreateUnionRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table to be created. Has the
        /// same naming restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>.  </param>
        /// <param name="table_names">The list of table names to merge. Must
        /// contain the names of one or more existing tables.  </param>
        /// <param name="input_column_names">The list of columns from each of
        /// the corresponding input tables.  </param>
        /// <param name="output_column_names">The list of names of the columns
        /// to be stored in the output table.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// output table. If the collection provided is non-existent, the
        /// collection will be automatically created. If empty, the output
        /// table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MATERIALIZE_ON_GPU">MATERIALIZE_ON_GPU</see>:</term>
        ///         <description>If <i>true</i>, then the columns of the output
        /// table will be cached on the GPU.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MODE">MODE</see>:</term>
        ///         <description>If <i>merge_views</i>, then this operation
        /// will merge the provided views. All <paramref
        /// cref="CreateUnionRequest.table_names" /> must be views from the
        /// same underlying base table.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>:</term>
        ///         <description>Retains all rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION">UNION</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables (synonym for <i>union_distinct</i>).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.UNION_DISTINCT">UNION_DISTINCT</see>:</term>
        ///         <description>Retains all unique rows from the specified
        /// tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT">EXCEPT</see>:</term>
        ///         <description>Retains all unique rows from the first table
        /// that do not appear in the second table (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.EXCEPT_ALL">EXCEPT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) from
        /// the first table that do not appear in the second table (only works
        /// on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT">INTERSECT</see>:</term>
        ///         <description>Retains all unique rows that appear in both of
        /// the specified tables (only works on 2 tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.INTERSECT_ALL">INTERSECT_ALL</see>:</term>
        ///         <description>Retains all rows(including duplicates) that
        /// appear in both of the specified tables (only works on 2
        /// tables).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.MERGE_VIEWS">MERGE_VIEWS</see>:</term>
        ///         <description>Merge two or more views (or views of views) of
        /// the same base data set into a new view. If this mode is selected
        /// <paramref cref="CreateUnionRequest.input_column_names" /> AND
        /// <paramref cref="CreateUnionRequest.output_column_names" /> must be
        /// empty. The resulting view would match the results of a SQL OR
        /// operation, e.g., if filter 1 creates a view using the expression 'x
        /// = 20' and filter 2 creates a view using the expression 'x <= 10',
        /// then the merge views operation creates a new view using the
        /// expression 'x = 20 OR x <= 10'.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.UNION_ALL">UNION_ALL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.CREATE_INDEXES">CREATE_INDEXES</see>:</term>
        ///         <description>Comma-separated list of columns on which to
        /// create indexes on the output table.  The columns specified must be
        /// present in <paramref cref="CreateUnionRequest.output_column_names"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the table specified in <paramref
        /// cref="CreateUnionRequest.table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.PERSIST">PERSIST</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be persisted
        /// and will not expire unless a <i>ttl</i> is specified.   If
        /// <i>false</i>, then the table will be an in-memory table and will
        /// expire unless a <i>ttl</i> is specified otherwise.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view the output table will be a part
        /// of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FORCE_REPLICATED">FORCE_REPLICATED</see>:</term>
        ///         <description>If <i>true</i>, then the table specified in
        /// <paramref cref="CreateUnionRequest.table_name" /> will be
        /// replicated even if the source tables are not.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateUnionRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public CreateUnionRequest( string table_name,
                                   IList<string> table_names,
                                   IList<IList<string>> input_column_names,
                                   IList<string> output_column_names,
                                   IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.table_names = table_names ?? new List<string>();
            this.input_column_names = input_column_names ?? new List<IList<string>>();
            this.output_column_names = output_column_names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateUnionRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createUnion(string,IList{string},IList{IList{string}},IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class CreateUnionResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateUnionRequest.table_name"
        /// />.  </summary>
        public string table_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateUnionResponse




}  // end namespace kinetica
