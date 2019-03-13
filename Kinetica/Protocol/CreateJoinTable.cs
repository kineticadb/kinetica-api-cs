/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createJoinTable(string,IList{string},IList{string},IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a table that is the result of a SQL JOIN.
    /// <br />
    /// For join details and examples see: <a href="../../concepts/joins.html"
    /// target="_top">Joins</a>.  For limitations, see <a
    /// href="../../concepts/joins.html#limitations-cautions"
    /// target="_top">Join Limitations and Cautions</a>.</summary>
    public class CreateJoinTableRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// join. If the collection provided is non-existent, the collection
        /// will be automatically created. If empty, then the join will be at
        /// the top level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>Obsolete in GPUdb v7.0</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.OPTIMIZE_LOOKUPS">OPTIMIZE_LOOKUPS</see>:</term>
        ///         <description>Use more memory to speed up the joining of
        /// tables.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH_METHOD">REFRESH_METHOD</see>:</term>
        ///         <description>Method by which the join can be refreshed when
        /// the data in underlying member tables have changed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>:</term>
        ///         <description>refresh only occurs when manually requested by
        /// calling this endpoint with refresh option set to <i>refresh</i> or
        /// <i>full_refresh</i></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_QUERY">ON_QUERY</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever a new query is issued and new data is
        /// inserted into the base table.  A full refresh of all the records
        /// occurs when a new query is issued and there have been inserts to
        /// any non-base-tables since the last query.  <a
        /// href="../../concepts/ttl.html" target="_top">TTL</a> will be set to
        /// not expire; any <i>ttl</i> specified will be ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_INSERT">ON_INSERT</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever new data is inserted into a base table.  A
        /// full refresh of all the records occurs when a new query is issued
        /// and there have been inserts to any non-base-tables since the last
        /// query.  <a href="../../concepts/ttl.html" target="_top">TTL</a>
        /// will be set to not expire; any <i>ttl</i> specified will be
        /// ignored.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>Do a manual refresh of the join if it exists -
        /// throws an error otherwise
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>:</term>
        ///         <description>don't refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) if new data has been inserted into the base table.
        /// A full refresh of all the records occurs if there have been inserts
        /// to any non-base-tables since the last refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FULL_REFRESH">FULL_REFRESH</see>:</term>
        ///         <description>always refresh even if no new records have
        /// been added.  Only refresh method guaranteed to do a full refresh
        /// (refresh all the records) if a delete or update has occurred since
        /// the last refresh.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.  Ignored if
        /// <i>refresh_method</i> is either <i>on_insert</i> or
        /// <i>on_query</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum size of a joined-chunk for this table.
        /// Defaults to the gpudb.conf file chunk size</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Name of a collection which is to contain the join. If
            /// the collection provided is non-existent, the collection will be
            /// automatically created. If empty, then the join will be at the
            /// top level.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>Obsolete in GPUdb v7.0</summary>
            public const string MAX_QUERY_DIMENSIONS = "max_query_dimensions";

            /// <summary>Use more memory to speed up the joining of tables.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateJoinTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string OPTIMIZE_LOOKUPS = "optimize_lookups";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Method by which the join can be refreshed when the
            /// data in underlying member tables have changed.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>:</term>
            ///         <description>refresh only occurs when manually
            /// requested by calling this endpoint with refresh option set to
            /// <i>refresh</i> or <i>full_refresh</i></description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateJoinTableRequest.Options.ON_QUERY">ON_QUERY</see>:</term>
            ///         <description>incrementally refresh (refresh just those
            /// records added) whenever a new query is issued and new data is
            /// inserted into the base table.  A full refresh of all the
            /// records occurs when a new query is issued and there have been
            /// inserts to any non-base-tables since the last query.  <a
            /// href="../../concepts/ttl.html" target="_top">TTL</a> will be
            /// set to not expire; any <i>ttl</i> specified will be
            /// ignored.</description>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateJoinTableRequest.Options.ON_INSERT">ON_INSERT</see>:</term>
            ///         <description>incrementally refresh (refresh just those
            /// records added) whenever new data is inserted into a base table.
            /// A full refresh of all the records occurs when a new query is
            /// issued and there have been inserts to any non-base-tables since
            /// the last query.  <a href="../../concepts/ttl.html"
            /// target="_top">TTL</a> will be set to not expire; any <i>ttl</i>
            /// specified will be ignored.</description>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>.</summary>
            public const string REFRESH_METHOD = "refresh_method";

            /// <summary>refresh only occurs when manually requested by calling
            /// this endpoint with refresh option set to <i>refresh</i> or
            /// <i>full_refresh</i></summary>
            public const string MANUAL = "manual";

            /// <summary>incrementally refresh (refresh just those records
            /// added) whenever a new query is issued and new data is inserted
            /// into the base table.  A full refresh of all the records occurs
            /// when a new query is issued and there have been inserts to any
            /// non-base-tables since the last query.  <a
            /// href="../../concepts/ttl.html" target="_top">TTL</a> will be
            /// set to not expire; any <i>ttl</i> specified will be
            /// ignored.</summary>
            public const string ON_QUERY = "on_query";

            /// <summary>incrementally refresh (refresh just those records
            /// added) whenever new data is inserted into a base table.  A full
            /// refresh of all the records occurs when a new query is issued
            /// and there have been inserts to any non-base-tables since the
            /// last query.  <a href="../../concepts/ttl.html"
            /// target="_top">TTL</a> will be set to not expire; any <i>ttl</i>
            /// specified will be ignored.</summary>
            public const string ON_INSERT = "on_insert";

            /// <summary>incrementally refresh (refresh just those records
            /// added) if new data has been inserted into the base table.  A
            /// full refresh of all the records occurs if there have been
            /// inserts to any non-base-tables since the last refresh</summary>
            public const string REFRESH = "refresh";

            /// <summary>don't refresh</summary>
            public const string NO_REFRESH = "no_refresh";

            /// <summary>always refresh even if no new records have been added.
            /// Only refresh method guaranteed to do a full refresh (refresh
            /// all the records) if a delete or update has occurred since the
            /// last refresh.</summary>
            public const string FULL_REFRESH = "full_refresh";

            /// <summary>Sets the <a href="../../concepts/ttl.html"
            /// target="_top">TTL</a> of the join table specified in <see
            /// cref="join_table_name" />.  Ignored if <i>refresh_method</i> is
            /// either <i>on_insert</i> or <i>on_query</i>.</summary>
            public const string TTL = "ttl";

            /// <summary>view this projection is part of</summary>
            public const string VIEW_ID = "view_id";

            /// <summary>return a count of 0 for the join table for logging and
            /// for show_table. optimization needed for large overlapped
            /// equi-join stencils</summary>
            public const string NO_COUNT = "no_count";

            /// <summary>Maximum size of a joined-chunk for this table.
            /// Defaults to the gpudb.conf file chunk size</summary>
            public const string CHUNK_SIZE = "chunk_size";
        } // end struct Options


        /// <summary>Name of the join table to be created.  Has the same naming
        /// restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>.  </summary>
        public string join_table_name { get; set; }

        /// <summary>The list of table names composing the join.  Corresponds
        /// to a SQL statement FROM clause.  </summary>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>List of member table columns or column expressions to be
        /// included in the join. Columns can be prefixed with
        /// 'table_id.column_name', where 'table_id' is the table name or
        /// alias.  Columns can be aliased via the syntax 'column_name as
        /// alias'. Wild cards '*' can be used to include all columns across
        /// member tables or 'table_id.*' for all of a single table's columns.
        /// Columns and column expressions composing the join must be uniquely
        /// named or aliased--therefore, the '*' wild card cannot be used if
        /// column names aren't unique across all tables.  </summary>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>An optional list of expressions to combine and filter the
        /// joined tables.  Corresponds to a SQL statement WHERE clause. For
        /// details see: <a href="../../concepts/expressions.html"
        /// target="_top">expressions</a>.  </summary>
        public IList<string> expressions { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// join. If the collection provided is non-existent, the collection
        /// will be automatically created. If empty, then the join will be at
        /// the top level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>Obsolete in GPUdb v7.0</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.OPTIMIZE_LOOKUPS">OPTIMIZE_LOOKUPS</see>:</term>
        ///         <description>Use more memory to speed up the joining of
        /// tables.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH_METHOD">REFRESH_METHOD</see>:</term>
        ///         <description>Method by which the join can be refreshed when
        /// the data in underlying member tables have changed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>:</term>
        ///         <description>refresh only occurs when manually requested by
        /// calling this endpoint with refresh option set to <i>refresh</i> or
        /// <i>full_refresh</i></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_QUERY">ON_QUERY</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever a new query is issued and new data is
        /// inserted into the base table.  A full refresh of all the records
        /// occurs when a new query is issued and there have been inserts to
        /// any non-base-tables since the last query.  <a
        /// href="../../concepts/ttl.html" target="_top">TTL</a> will be set to
        /// not expire; any <i>ttl</i> specified will be ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_INSERT">ON_INSERT</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever new data is inserted into a base table.  A
        /// full refresh of all the records occurs when a new query is issued
        /// and there have been inserts to any non-base-tables since the last
        /// query.  <a href="../../concepts/ttl.html" target="_top">TTL</a>
        /// will be set to not expire; any <i>ttl</i> specified will be
        /// ignored.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>Do a manual refresh of the join if it exists -
        /// throws an error otherwise
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>:</term>
        ///         <description>don't refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) if new data has been inserted into the base table.
        /// A full refresh of all the records occurs if there have been inserts
        /// to any non-base-tables since the last refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FULL_REFRESH">FULL_REFRESH</see>:</term>
        ///         <description>always refresh even if no new records have
        /// been added.  Only refresh method guaranteed to do a full refresh
        /// (refresh all the records) if a delete or update has occurred since
        /// the last refresh.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.  Ignored if
        /// <i>refresh_method</i> is either <i>on_insert</i> or
        /// <i>on_query</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum size of a joined-chunk for this table.
        /// Defaults to the gpudb.conf file chunk size</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateJoinTableRequest object with default
        /// parameters.</summary>
        public CreateJoinTableRequest() { }

        /// <summary>Constructs a CreateJoinTableRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="join_table_name">Name of the join table to be created.
        /// Has the same naming restrictions as <a
        /// href="../../concepts/tables.html" target="_top">tables</a>.
        /// </param>
        /// <param name="table_names">The list of table names composing the
        /// join.  Corresponds to a SQL statement FROM clause.  </param>
        /// <param name="column_names">List of member table columns or column
        /// expressions to be included in the join. Columns can be prefixed
        /// with 'table_id.column_name', where 'table_id' is the table name or
        /// alias.  Columns can be aliased via the syntax 'column_name as
        /// alias'. Wild cards '*' can be used to include all columns across
        /// member tables or 'table_id.*' for all of a single table's columns.
        /// Columns and column expressions composing the join must be uniquely
        /// named or aliased--therefore, the '*' wild card cannot be used if
        /// column names aren't unique across all tables.  </param>
        /// <param name="expressions">An optional list of expressions to
        /// combine and filter the joined tables.  Corresponds to a SQL
        /// statement WHERE clause. For details see: <a
        /// href="../../concepts/expressions.html"
        /// target="_top">expressions</a>.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// join. If the collection provided is non-existent, the collection
        /// will be automatically created. If empty, then the join will be at
        /// the top level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>Obsolete in GPUdb v7.0</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.OPTIMIZE_LOOKUPS">OPTIMIZE_LOOKUPS</see>:</term>
        ///         <description>Use more memory to speed up the joining of
        /// tables.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH_METHOD">REFRESH_METHOD</see>:</term>
        ///         <description>Method by which the join can be refreshed when
        /// the data in underlying member tables have changed.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>:</term>
        ///         <description>refresh only occurs when manually requested by
        /// calling this endpoint with refresh option set to <i>refresh</i> or
        /// <i>full_refresh</i></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_QUERY">ON_QUERY</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever a new query is issued and new data is
        /// inserted into the base table.  A full refresh of all the records
        /// occurs when a new query is issued and there have been inserts to
        /// any non-base-tables since the last query.  <a
        /// href="../../concepts/ttl.html" target="_top">TTL</a> will be set to
        /// not expire; any <i>ttl</i> specified will be ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.ON_INSERT">ON_INSERT</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) whenever new data is inserted into a base table.  A
        /// full refresh of all the records occurs when a new query is issued
        /// and there have been inserts to any non-base-tables since the last
        /// query.  <a href="../../concepts/ttl.html" target="_top">TTL</a>
        /// will be set to not expire; any <i>ttl</i> specified will be
        /// ignored.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.MANUAL">MANUAL</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>Do a manual refresh of the join if it exists -
        /// throws an error otherwise
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>:</term>
        ///         <description>don't refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.REFRESH">REFRESH</see>:</term>
        ///         <description>incrementally refresh (refresh just those
        /// records added) if new data has been inserted into the base table.
        /// A full refresh of all the records occurs if there have been inserts
        /// to any non-base-tables since the last refresh</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.FULL_REFRESH">FULL_REFRESH</see>:</term>
        ///         <description>always refresh even if no new records have
        /// been added.  Only refresh method guaranteed to do a full refresh
        /// (refresh all the records) if a delete or update has occurred since
        /// the last refresh.</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateJoinTableRequest.Options.NO_REFRESH">NO_REFRESH</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../concepts/ttl.html"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.  Ignored if
        /// <i>refresh_method</i> is either <i>on_insert</i> or
        /// <i>on_query</i>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum size of a joined-chunk for this table.
        /// Defaults to the gpudb.conf file chunk size</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public CreateJoinTableRequest( string join_table_name,
                                       IList<string> table_names,
                                       IList<string> column_names,
                                       IList<string> expressions = null,
                                       IDictionary<string, string> options = null)
        {
            this.join_table_name = join_table_name ?? "";
            this.table_names = table_names ?? new List<string>();
            this.column_names = column_names ?? new List<string>();
            this.expressions = expressions ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateJoinTableRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createJoinTable(string,IList{string},IList{string},IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class CreateJoinTableResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.  </summary>
        public string join_table_name { get; set; }

        /// <summary>The number of records in the join table filtered by the
        /// given select expression.  </summary>
        public long count { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateJoinTableResponse




}  // end namespace kinetica
