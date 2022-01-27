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
    /// For join details and examples see: <a href="../../../concepts/joins/"
    /// target="_top">Joins</a>.  For limitations, see <a
    /// href="../../../concepts/joins/#limitations-cautions" target="_top">Join
    /// Limitations and Cautions</a>.</summary>
    public class CreateJoinTableRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:</term>
        ///         <description>If <i>true</i>, a unique temporary table name
        /// will be generated in the sys_temp schema and used in place of
        /// <paramref cref="CreateJoinTableRequest.join_table_name" />. This is
        /// always allowed even if the caller does not have permission to
        /// create tables. The generated name is returned in
        /// <i>qualified_join_table_name</i>.
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
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the join as part of <paramref
        /// cref="CreateJoinTableRequest.join_table_name" /> and use
        /// /create/schema to create the schema if non-existent]  Name of a
        /// schema for the join. If the schema is non-existent, it will be
        /// automatically created.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>No longer used.</description>
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
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of.  The default
        /// value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils.  The default value is
        /// 'false'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum number of records per joined-chunk for
        /// this table. Defaults to the gpudb.conf file chunk
        /// size</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, a unique temporary table name will be
            /// generated in the sys_temp schema and used in place of <see
            /// cref="join_table_name" />. This is always allowed even if the
            /// caller does not have permission to create tables. The generated
            /// name is returned in <i>qualified_join_table_name</i>.
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
            public const string CREATE_TEMP_TABLE = "create_temp_table";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>[DEPRECATED--please specify the containing schema for
            /// the join as part of <see cref="join_table_name" /> and use <see
            /// cref="Kinetica.createSchema(string,IDictionary{string, string})"
            /// /> to create the schema if non-existent]  Name of a schema for
            /// the join. If the schema is non-existent, it will be
            /// automatically created.  The default value is ''.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>No longer used.</summary>
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

            /// <summary>Sets the <a href="../../../concepts/ttl/"
            /// target="_top">TTL</a> of the join table specified in <see
            /// cref="join_table_name" />.</summary>
            public const string TTL = "ttl";

            /// <summary>view this projection is part of.  The default value is
            /// ''.</summary>
            public const string VIEW_ID = "view_id";

            /// <summary>return a count of 0 for the join table for logging and
            /// for show_table. optimization needed for large overlapped
            /// equi-join stencils.  The default value is 'false'.</summary>
            public const string NO_COUNT = "no_count";

            /// <summary>Maximum number of records per joined-chunk for this
            /// table. Defaults to the gpudb.conf file chunk size</summary>
            public const string CHUNK_SIZE = "chunk_size";
        } // end struct Options


        /// <summary>Name of the join table to be created, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  </summary>
        public string join_table_name { get; set; }

        /// <summary>The list of table names composing the join, each in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Corresponds to a SQL
        /// statement FROM clause.  </summary>
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
        /// details see: <a href="../../../concepts/expressions/"
        /// target="_top">expressions</a>.  The default value is an empty
        /// {@link List}.</summary>
        public IList<string> expressions { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:</term>
        ///         <description>If <i>true</i>, a unique temporary table name
        /// will be generated in the sys_temp schema and used in place of
        /// <paramref cref="CreateJoinTableRequest.join_table_name" />. This is
        /// always allowed even if the caller does not have permission to
        /// create tables. The generated name is returned in
        /// <i>qualified_join_table_name</i>.
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
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the join as part of <paramref
        /// cref="CreateJoinTableRequest.join_table_name" /> and use
        /// /create/schema to create the schema if non-existent]  Name of a
        /// schema for the join. If the schema is non-existent, it will be
        /// automatically created.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>No longer used.</description>
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
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of.  The default
        /// value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils.  The default value is
        /// 'false'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum number of records per joined-chunk for
        /// this table. Defaults to the gpudb.conf file chunk
        /// size</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateJoinTableRequest object with default
        /// parameters.</summary>
        public CreateJoinTableRequest() { }

        /// <summary>Constructs a CreateJoinTableRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="join_table_name">Name of the join table to be created,
        /// in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a> and meeting <a
        /// href="../../../concepts/tables/#table-naming-criteria"
        /// target="_top">table naming criteria</a>.  </param>
        /// <param name="table_names">The list of table names composing the
        /// join, each in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Corresponds to a SQL
        /// statement FROM clause.  </param>
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
        /// href="../../../concepts/expressions/"
        /// target="_top">expressions</a>.  The default value is an empty
        /// {@link List}.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:</term>
        ///         <description>If <i>true</i>, a unique temporary table name
        /// will be generated in the sys_temp schema and used in place of
        /// <paramref cref="CreateJoinTableRequest.join_table_name" />. This is
        /// always allowed even if the caller does not have permission to
        /// create tables. The generated name is returned in
        /// <i>qualified_join_table_name</i>.
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
        /// cref="CreateJoinTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>[DEPRECATED--please specify the containing
        /// schema for the join as part of <paramref
        /// cref="CreateJoinTableRequest.join_table_name" /> and use
        /// /create/schema to create the schema if non-existent]  Name of a
        /// schema for the join. If the schema is non-existent, it will be
        /// automatically created.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.MAX_QUERY_DIMENSIONS">MAX_QUERY_DIMENSIONS</see>:</term>
        ///         <description>No longer used.</description>
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
        /// cref="CreateJoinTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        /// target="_top">TTL</a> of the join table specified in <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.VIEW_ID">VIEW_ID</see>:</term>
        ///         <description>view this projection is part of.  The default
        /// value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.NO_COUNT">NO_COUNT</see>:</term>
        ///         <description>return a count of 0 for the join table for
        /// logging and for show_table. optimization needed for large
        /// overlapped equi-join stencils.  The default value is
        /// 'false'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Maximum number of records per joined-chunk for
        /// this table. Defaults to the gpudb.conf file chunk
        /// size</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
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

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableResponse.Info.QUALIFIED_JOIN_TABLE_NAME">QUALIFIED_JOIN_TABLE_NAME</see>:</term>
        ///         <description>The fully qualified name of the join table
        /// (i.e. including the schema)</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <member name="info"
        /// />.</summary>
        public struct Info
        {

            /// <summary>The fully qualified name of the join table (i.e.
            /// including the schema)</summary>
            public const string QUALIFIED_JOIN_TABLE_NAME = "qualified_join_table_name";
        } // end struct Info


        /// <summary>Value of <paramref
        /// cref="CreateJoinTableRequest.join_table_name" />.  </summary>
        public string join_table_name { get; set; }

        /// <summary>The number of records in the join table filtered by the
        /// given select expression.  </summary>
        public long count { get; set; }

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateJoinTableResponse.Info.QUALIFIED_JOIN_TABLE_NAME">QUALIFIED_JOIN_TABLE_NAME</see>:</term>
        ///         <description>The fully qualified name of the join table
        /// (i.e. including the schema)</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateJoinTableResponse




}  // end namespace kinetica
