/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */


namespace kinetica
{

    /// <summary>Column properties used for Kinetica types.</summary>
    public struct ColumnProperty
    {

        /// <summary>Default property for all numeric and string type columns;
        /// makes the column available for GPU queries.</summary>
        public const string DATA = "data";

        /// <summary>Valid only for 'string' columns. Enables full text search
        /// for string columns. Can be set independently of <i>data</i> and
        /// <i>store_only</i>.</summary>
        public const string TEXT_SEARCH = "text_search";

        /// <summary>Persist the column value but do not make it available to
        /// queries (e.g. <see
        /// cref="Kinetica.filter(string,string,string,IDictionary{string, string})"
        /// />)-i.e. it is mutually exclusive to the <i>data</i> property. Any
        /// 'bytes' type column must have a <i>store_only</i> property. This
        /// property reduces system memory usage.</summary>
        public const string STORE_ONLY = "store_only";

        /// <summary>Works in conjunction with the <i>data</i> property for
        /// string columns. This property reduces system disk usage by
        /// disabling reverse string lookups. Queries like <see
        /// cref="Kinetica.filter(string,string,string,IDictionary{string, string})"
        /// />, <see
        /// cref="Kinetica.filterByList(string,string,IDictionary{string, IList{string}},IDictionary{string, string})"
        /// />, and <see
        /// cref="Kinetica.filterByValue(string,string,bool,double,string,string,IDictionary{string, string})"
        /// /> work as usual but <see
        /// cref="Kinetica.aggregateUnique(string,string,long,long,IDictionary{string, string})"
        /// /> and <see
        /// cref="Kinetica.aggregateGroupBy(string,IList{string},long,long,IDictionary{string, string})"
        /// /> are not allowed on columns with this property.</summary>
        public const string DISK_OPTIMIZED = "disk_optimized";

        /// <summary>Valid only for 'long' columns. Indicates that this field
        /// represents a timestamp and will be provided in milliseconds since
        /// the Unix epoch: 00:00:00 Jan 1 1970.  Dates represented by a
        /// timestamp must fall between the year 1000 and the year
        /// 2900.</summary>
        public const string TIMESTAMP = "timestamp";

        /// <summary>Valid only for 'string' columns.  It represents an
        /// unsigned long integer data type. The string can only be interpreted
        /// as an unsigned long data type with minimum value of zero, and
        /// maximum value of 18446744073709551615.</summary>
        public const string ULONG = "ulong";

        /// <summary>Valid only for 'string' columns.  It represents an uuid
        /// data type. Internally, it is stored as a 128-bit integer.</summary>
        public const string UUID = "uuid";

        /// <summary>Valid only for 'string' columns.  It represents a SQL type
        /// NUMERIC(19, 4) data type.  There can be up to 15 digits before the
        /// decimal point and up to four digits in the fractional part.  The
        /// value can be positive or negative (indicated by a minus sign at the
        /// beginning).  This property is mutually exclusive with the
        /// <i>text_search</i> property.</summary>
        public const string DECIMAL = "decimal";

        /// <summary>Valid only for 'string' columns.  Indicates that this
        /// field represents a date and will be provided in the format
        /// 'YYYY-MM-DD'.  The allowable range is 1000-01-01 through
        /// 2900-01-01.  This property is mutually exclusive with the
        /// <i>text_search</i> property.</summary>
        public const string DATE = "date";

        /// <summary>Valid only for 'string' columns.  Indicates that this
        /// field represents a time-of-day and will be provided in the format
        /// 'HH:MM:SS.mmm'.  The allowable range is 00:00:00.000 through
        /// 23:59:59.999.  This property is mutually exclusive with the
        /// <i>text_search</i> property.</summary>
        public const string TIME = "time";

        /// <summary>Valid only for 'string' columns.  Indicates that this
        /// field represents a datetime and will be provided in the format
        /// 'YYYY-MM-DD HH:MM:SS.mmm'.  The allowable range is 1000-01-01
        /// 00:00:00.000 through 2900-01-01 23:59:59.999.  This property is
        /// mutually exclusive with the <i>text_search</i> property.</summary>
        public const string DATETIME = "datetime";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 1 character.</summary>
        public const string CHAR1 = "char1";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 2 characters.</summary>
        public const string CHAR2 = "char2";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 4 characters.</summary>
        public const string CHAR4 = "char4";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 8 characters.</summary>
        public const string CHAR8 = "char8";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 16 characters.</summary>
        public const string CHAR16 = "char16";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 32 characters.</summary>
        public const string CHAR32 = "char32";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 64 characters.</summary>
        public const string CHAR64 = "char64";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 128 characters.</summary>
        public const string CHAR128 = "char128";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns. Strings with this property must be
        /// no longer than 256 characters.</summary>
        public const string CHAR256 = "char256";

        /// <summary>This property provides optimized memory and query
        /// performance for int columns. Ints with this property must be
        /// between -128 and +127 (inclusive)</summary>
        public const string INT8 = "int8";

        /// <summary>This property provides optimized memory and query
        /// performance for int columns. Ints with this property must be
        /// between -32768 and +32767 (inclusive)</summary>
        public const string INT16 = "int16";

        /// <summary>This property provides optimized memory, disk and query
        /// performance for string columns representing IPv4 addresses (i.e.
        /// 192.168.1.1). Strings with this property must be of the form:
        /// A.B.C.D where A, B, C and D are in the range of 0-255.</summary>
        public const string IPV4 = "ipv4";

        /// <summary>Valid only for 'string' and 'bytes' columns. Indicates
        /// that this field contains geospatial geometry objects in Well-Known
        /// Text (WKT) or Well-Known Binary (WKB) format.</summary>
        public const string WKT = "wkt";

        /// <summary>This property indicates that this column will be part of
        /// (or the entire) <a href="../../../concepts/tables/#primary-keys"
        /// target="_top">primary key</a>.</summary>
        public const string PRIMARY_KEY = "primary_key";

        /// <summary>This property indicates that this column will be part of
        /// (or the entire) <a href="../../../concepts/tables/#shard-keys"
        /// target="_top">shard key</a>.</summary>
        public const string SHARD_KEY = "shard_key";

        /// <summary>This property indicates that this column is nullable.
        /// However, setting this property is insufficient for making the
        /// column nullable.  The user must declare the type of the column as a
        /// union between its regular type and 'null' in the avro schema for
        /// the record type in <see cref="type_definition" />.  For example, if
        /// a column is of type integer and is nullable, then the entry for the
        /// column in the avro schema must be: ['int', 'null'].
        /// <br />
        /// The C++, C#, Java, and Python APIs have built-in convenience for
        /// bypassing setting the avro schema by hand.  For those languages,
        /// one can use this property as usual and not have to worry about the
        /// avro schema for the record.</summary>
        public const string NULLABLE = "nullable";

        /// <summary>This property indicates that this column should be <a
        /// href="../../../concepts/dictionary_encoding/"
        /// target="_top">dictionary encoded</a>. It can only be used in
        /// conjunction with restricted string (charN), int, long or date
        /// columns. Dictionary encoding is best for columns where the
        /// cardinality (the number of unique values) is expected to be low.
        /// This property can save a large amount of memory.</summary>
        public const string DICT = "dict";

        /// <summary>For 'date', 'time', 'datetime', or 'timestamp' column
        /// types, replace empty strings and invalid timestamps with 'NOW()'
        /// upon insert.</summary>
        public const string INIT_WITH_NOW = "init_with_now";

        /// <summary>For 'uuid' type,  repalce empty strings and invalid uuid
        /// values with new_uuid()' upon insert.</summary>
        public const string INIT_WITH_UUID = "init_with_uuid";
    } // end struct ColumnProperty



}  // end namespace kinetica
