/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createTable(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a new table or collection. If a new table is being created, the
    /// type of the table is given by <see cref="type_id" />, which must the be
    /// the ID of a currently registered type (i.e. one created via <see
    /// cref="Kinetica.createType(string,string,IDictionary{string, IList{string}},IDictionary{string, string})"
    /// />). The table will be created inside a collection if the option
    /// <i>collection_name</i> is specified. If that collection does not
    /// already exist, it will be created.
    /// <br />
    /// To create a new collection, specify the name of the collection in <see
    /// cref="table_name" /> and set the <i>is_collection</i> option to
    /// <i>true</i>; <see cref="type_id" /> will be ignored.</summary>
    public class CreateTableRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the table already exists and is of the given type.  If
        /// a table with the same ID but a different type exists, it is still
        /// an error.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created table. If the collection provided is non-existent,
        /// the collection will be automatically created. If empty, then the
        /// newly created table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_COLLECTION">IS_COLLECTION</see>:</term>
        ///         <description>Indicates whether the new table to be created
        /// will be a collection.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.DISALLOW_HOMOGENEOUS_TABLES">DISALLOW_HOMOGENEOUS_TABLES</see>:</term>
        ///         <description>For a collection, indicates whether the
        /// collection prohibits containment of multiple tables of exactly the
        /// same data type.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_REPLICATED">IS_REPLICATED</see>:</term>
        ///         <description>For a table, indicates the <a
        /// href="../../../../concepts/tables.html#distribution"
        /// target="_top">distribution scheme</a> for the table's data.  If
        /// true, the table will be <a
        /// href="../../../../concepts/tables.html#replication"
        /// target="_top">replicated</a>.  If false, the table will be <a
        /// href="../../../../concepts/tables.html#sharding"
        /// target="_top">sharded</a> according to the <a
        /// href="../../../../concepts/tables.html#shard-keys"
        /// target="_top">shard key</a> specified in the given <paramref
        /// cref="CreateTableRequest.type_id" />, or <a
        /// href="../../../../concepts/tables.html#random-sharding"
        /// target="_top">randomly sharded</a>, if no shard key is specified.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_KEYS">FOREIGN_KEYS</see>:</term>
        ///         <description>Semicolon-separated list of <a
        /// href="../../../../concepts/tables.html#foreign-keys"
        /// target="_top">foreign keys</a>, of the format 'source_column
        /// references target_table(primary_key_column) [ as <foreign_key_name>
        /// ]'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_SHARD_KEY">FOREIGN_SHARD_KEY</see>:</term>
        ///         <description>Foreign shard key of the format 'source_column
        /// references shard_by_column from
        /// target_table(primary_key_column)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>For a table, sets the <a
        /// href="../../../../concepts/ttl.html" target="_top">TTL</a> of the
        /// table specified in <paramref cref="CreateTableRequest.table_name"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_RESULT_TABLE">IS_RESULT_TABLE</see>:</term>
        ///         <description>For a table, indicates whether the table is an
        /// in-memory table. A result table cannot contain store_only,
        /// text_search, or string columns (charN columns are acceptable), and
        /// it will not be retained if the server is restarted.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, prevents an error from occurring if
            /// the table already exists and is of the given type.  If a table
            /// with the same ID but a different type exists, it is still an
            /// error.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_EXISTS = "no_error_if_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Name of a collection which is to contain the newly
            /// created table. If the collection provided is non-existent, the
            /// collection will be automatically created. If empty, then the
            /// newly created table will be a top-level table.</summary>
            public const string COLLECTION_NAME = "collection_name";

            /// <summary>Indicates whether the new table to be created will be
            /// a collection.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string IS_COLLECTION = "is_collection";

            /// <summary>For a collection, indicates whether the collection
            /// prohibits containment of multiple tables of exactly the same
            /// data type.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string DISALLOW_HOMOGENEOUS_TABLES = "disallow_homogeneous_tables";

            /// <summary>For a table, indicates the <a
            /// href="../../../../../concepts/tables.html#distribution"
            /// target="_top">distribution scheme</a> for the table's data.  If
            /// true, the table will be <a
            /// href="../../../../../concepts/tables.html#replication"
            /// target="_top">replicated</a>.  If false, the table will be <a
            /// href="../../../../../concepts/tables.html#sharding"
            /// target="_top">sharded</a> according to the <a
            /// href="../../../../../concepts/tables.html#shard-keys"
            /// target="_top">shard key</a> specified in the given <see
            /// cref="type_id" />, or <a
            /// href="../../../../../concepts/tables.html#random-sharding"
            /// target="_top">randomly sharded</a>, if no shard key is
            /// specified.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string IS_REPLICATED = "is_replicated";

            /// <summary>Semicolon-separated list of <a
            /// href="../../../../../concepts/tables.html#foreign-keys"
            /// target="_top">foreign keys</a>, of the format 'source_column
            /// references target_table(primary_key_column) [ as
            /// <foreign_key_name> ]'.</summary>
            public const string FOREIGN_KEYS = "foreign_keys";

            /// <summary>Foreign shard key of the format 'source_column
            /// references shard_by_column from
            /// target_table(primary_key_column)'</summary>
            public const string FOREIGN_SHARD_KEY = "foreign_shard_key";

            /// <summary>For a table, sets the <a
            /// href="../../../../../concepts/ttl.html" target="_top">TTL</a>
            /// of the table specified in <see cref="table_name" />.</summary>
            public const string TTL = "ttl";

            /// <summary>Indicates the chunk size to be used for this
            /// table.</summary>
            public const string CHUNK_SIZE = "chunk_size";

            /// <summary>For a table, indicates whether the table is an
            /// in-memory table. A result table cannot contain store_only,
            /// text_search, or string columns (charN columns are acceptable),
            /// and it will not be retained if the server is restarted.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</summary>
            public const string IS_RESULT_TABLE = "is_result_table";
        } // end struct Options


        /// <summary>Name of the table to be created. Error for requests with
        /// existing table of the same name and type id may be suppressed by
        /// using the <i>no_error_if_exists</i> option.  See <a
        /// href="../../../../concepts/tables.html" target="_top">Tables</a>
        /// for naming restrictions.  </summary>
        public string table_name { get; set; }

        /// <summary>ID of a currently registered type. All objects added to
        /// the newly created table will be of this type.  Ignored if
        /// <i>is_collection</i> is <i>true</i>.  </summary>
        public string type_id { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the table already exists and is of the given type.  If
        /// a table with the same ID but a different type exists, it is still
        /// an error.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created table. If the collection provided is non-existent,
        /// the collection will be automatically created. If empty, then the
        /// newly created table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_COLLECTION">IS_COLLECTION</see>:</term>
        ///         <description>Indicates whether the new table to be created
        /// will be a collection.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.DISALLOW_HOMOGENEOUS_TABLES">DISALLOW_HOMOGENEOUS_TABLES</see>:</term>
        ///         <description>For a collection, indicates whether the
        /// collection prohibits containment of multiple tables of exactly the
        /// same data type.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_REPLICATED">IS_REPLICATED</see>:</term>
        ///         <description>For a table, indicates the <a
        /// href="../../../../concepts/tables.html#distribution"
        /// target="_top">distribution scheme</a> for the table's data.  If
        /// true, the table will be <a
        /// href="../../../../concepts/tables.html#replication"
        /// target="_top">replicated</a>.  If false, the table will be <a
        /// href="../../../../concepts/tables.html#sharding"
        /// target="_top">sharded</a> according to the <a
        /// href="../../../../concepts/tables.html#shard-keys"
        /// target="_top">shard key</a> specified in the given <paramref
        /// cref="CreateTableRequest.type_id" />, or <a
        /// href="../../../../concepts/tables.html#random-sharding"
        /// target="_top">randomly sharded</a>, if no shard key is specified.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_KEYS">FOREIGN_KEYS</see>:</term>
        ///         <description>Semicolon-separated list of <a
        /// href="../../../../concepts/tables.html#foreign-keys"
        /// target="_top">foreign keys</a>, of the format 'source_column
        /// references target_table(primary_key_column) [ as <foreign_key_name>
        /// ]'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_SHARD_KEY">FOREIGN_SHARD_KEY</see>:</term>
        ///         <description>Foreign shard key of the format 'source_column
        /// references shard_by_column from
        /// target_table(primary_key_column)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>For a table, sets the <a
        /// href="../../../../concepts/ttl.html" target="_top">TTL</a> of the
        /// table specified in <paramref cref="CreateTableRequest.table_name"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_RESULT_TABLE">IS_RESULT_TABLE</see>:</term>
        ///         <description>For a table, indicates whether the table is an
        /// in-memory table. A result table cannot contain store_only,
        /// text_search, or string columns (charN columns are acceptable), and
        /// it will not be retained if the server is restarted.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateTableRequest object with default
        /// parameters.</summary>
        public CreateTableRequest() { }

        /// <summary>Constructs a CreateTableRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table to be created. Error for
        /// requests with existing table of the same name and type id may be
        /// suppressed by using the <i>no_error_if_exists</i> option.  See <a
        /// href="../../../../concepts/tables.html" target="_top">Tables</a>
        /// for naming restrictions.  </param>
        /// <param name="type_id">ID of a currently registered type. All
        /// objects added to the newly created table will be of this type.
        /// Ignored if <i>is_collection</i> is <i>true</i>.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, prevents an error from
        /// occurring if the table already exists and is of the given type.  If
        /// a table with the same ID but a different type exists, it is still
        /// an error.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.COLLECTION_NAME">COLLECTION_NAME</see>:</term>
        ///         <description>Name of a collection which is to contain the
        /// newly created table. If the collection provided is non-existent,
        /// the collection will be automatically created. If empty, then the
        /// newly created table will be a top-level table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_COLLECTION">IS_COLLECTION</see>:</term>
        ///         <description>Indicates whether the new table to be created
        /// will be a collection.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.DISALLOW_HOMOGENEOUS_TABLES">DISALLOW_HOMOGENEOUS_TABLES</see>:</term>
        ///         <description>For a collection, indicates whether the
        /// collection prohibits containment of multiple tables of exactly the
        /// same data type.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_REPLICATED">IS_REPLICATED</see>:</term>
        ///         <description>For a table, indicates the <a
        /// href="../../../../concepts/tables.html#distribution"
        /// target="_top">distribution scheme</a> for the table's data.  If
        /// true, the table will be <a
        /// href="../../../../concepts/tables.html#replication"
        /// target="_top">replicated</a>.  If false, the table will be <a
        /// href="../../../../concepts/tables.html#sharding"
        /// target="_top">sharded</a> according to the <a
        /// href="../../../../concepts/tables.html#shard-keys"
        /// target="_top">shard key</a> specified in the given <paramref
        /// cref="CreateTableRequest.type_id" />, or <a
        /// href="../../../../concepts/tables.html#random-sharding"
        /// target="_top">randomly sharded</a>, if no shard key is specified.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_KEYS">FOREIGN_KEYS</see>:</term>
        ///         <description>Semicolon-separated list of <a
        /// href="../../../../concepts/tables.html#foreign-keys"
        /// target="_top">foreign keys</a>, of the format 'source_column
        /// references target_table(primary_key_column) [ as <foreign_key_name>
        /// ]'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FOREIGN_SHARD_KEY">FOREIGN_SHARD_KEY</see>:</term>
        ///         <description>Foreign shard key of the format 'source_column
        /// references shard_by_column from
        /// target_table(primary_key_column)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TTL">TTL</see>:</term>
        ///         <description>For a table, sets the <a
        /// href="../../../../concepts/ttl.html" target="_top">TTL</a> of the
        /// table specified in <paramref cref="CreateTableRequest.table_name"
        /// />.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Indicates the chunk size to be used for this
        /// table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.IS_RESULT_TABLE">IS_RESULT_TABLE</see>:</term>
        ///         <description>For a table, indicates whether the table is an
        /// in-memory table. A result table cannot contain store_only,
        /// text_search, or string columns (charN columns are acceptable), and
        /// it will not be retained if the server is restarted.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateTableRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public CreateTableRequest( string table_name,
                                   string type_id,
                                   IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.type_id = type_id ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateTableRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createTable(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateTableResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateTableRequest.table_name"
        /// />.  </summary>
        public string table_name { get; set; }

        /// <summary>Value of <paramref cref="CreateTableRequest.type_id" />.
        /// </summary>
        public string type_id { get; set; }

        /// <summary>Indicates if the created entity is a collection.
        /// </summary>
        public bool is_collection { get; set; }

    } // end class CreateTableResponse




}  // end namespace kinetica
