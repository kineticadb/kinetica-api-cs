/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.clearStatistics(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Clears statistics (cardinality, mean value, etc.) for a column in a
    /// specified table.</summary>
    public class ClearStatisticsRequest : KineticaData
    {

        /// <summary>Name of a table. Must be an existing table.  The default
        /// value is ''.</summary>
        public string table_name { get; set; } = "";

        /// <summary>Name of the column in <paramref
        /// cref="ClearStatisticsRequest.table_name" /> for which to clear
        /// statistics. The column must be from an existing table. An empty
        /// string clears statistics for all columns in the table.  The default
        /// value is ''.</summary>
        public string column_name { get; set; } = "";

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ClearStatisticsRequest object with default
        /// parameters.</summary>
        public ClearStatisticsRequest() { }

        /// <summary>Constructs a ClearStatisticsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of a table. Must be an existing
        /// table.  The default value is ''.</param>
        /// <param name="column_name">Name of the column in <paramref
        /// cref="ClearStatisticsRequest.table_name" /> for which to clear
        /// statistics. The column must be from an existing table. An empty
        /// string clears statistics for all columns in the table.  The default
        /// value is ''.</param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public ClearStatisticsRequest( string table_name = null,
                                       string column_name = null,
                                       IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_name = column_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ClearStatisticsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.clearStatistics(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class ClearStatisticsResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="ClearStatisticsRequest.table_name" />.  </summary>
        public string table_name { get; set; }

        /// <summary>Value of <paramref
        /// cref="ClearStatisticsRequest.column_name" />.  </summary>
        public string column_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ClearStatisticsResponse




}  // end namespace kinetica