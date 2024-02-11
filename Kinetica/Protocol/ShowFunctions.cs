/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// @cond NO_DOCS
    public class ShowFunctionsRequest : KineticaData
    {
        public struct Options
        {
            public const string PROPERTIES = "properties";
            public const string SHOW_SCALAR_FUNCTIONS = "show_scalar_functions";
            public const string TRUE = "true";
            public const string FALSE = "false";
            public const string SHOW_AGGREGATE_FUNCTIONS = "show_aggregate_functions";
            public const string SHOW_SQL_PROCEDURES = "show_sql_procedures";
            public const string SHOW_USER_DEFINED_FUNCTIONS = "show_user_defined_functions";
            public const string SHOW_CAST_FUNCTIONS = "show_cast_functions";
        } // end struct Options

        public IList<string> names { get; set; } = new List<string>();
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        public ShowFunctionsRequest() { }

        public ShowFunctionsRequest( IList<string> names,
                                     IDictionary<string, string> options = null)
        {
            this.names = names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowFunctionsRequest
    /// @endcond

    /// @cond NO_DOCS
    public class ShowFunctionsResponse : KineticaData
    {
        public struct Flags
        {
            public const string SCALAR = "scalar";
            public const string AGGREGATE = "aggregate";
            public const string SQL = "sql";
            public const string UDF = "udf";
            public const string CAST = "cast";
            public const string NONDETERMINISTIC = "nondeterministic";
        } // end struct Flags

        public IList<string> function_names { get; set; } = new List<string>();
        public IList<string> return_types { get; set; } = new List<string>();
        public IList<IList<string>> parameters { get; set; } = new List<IList<string>>();
        public IList<int> optional_parameter_count { get; set; } = new List<int>();
        public IList<int> flags { get; set; } = new List<int>();
        public IList<string> type_schemas { get; set; } = new List<string>();
        public IList<IDictionary<string, IList<string>>> properties { get; set; } = new List<IDictionary<string, IList<string>>>();
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowFunctionsResponse
    /// @endcond
} // end namespace kinetica
