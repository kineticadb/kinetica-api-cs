/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.collectStatistics(string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Collect statistics for a column(s) in a specified table.</summary>
    public class CollectStatisticsRequest : KineticaData
    {

        /// <summary>Name of a table, in [schema_name.]table_name format, using
        /// standard <a href="../../concepts/tables.html#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Must be an existing
        /// table.  </summary>
        public string table_name { get; set; }

        /// <summary>List of one or more column names in <paramref
        /// cref="CollectStatisticsRequest.table_name" /> for which to collect
        /// statistics (cardinality, mean value, etc.).  </summary>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CollectStatisticsRequest object with default
        /// parameters.</summary>
        public CollectStatisticsRequest() { }

        /// <summary>Constructs a CollectStatisticsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of a table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../concepts/tables.html#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Must be an existing
        /// table.  </param>
        /// <param name="column_names">List of one or more column names in
        /// <paramref cref="CollectStatisticsRequest.table_name" /> for which
        /// to collect statistics (cardinality, mean value, etc.).  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public CollectStatisticsRequest( string table_name,
                                         IList<string> column_names,
                                         IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_names = column_names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CollectStatisticsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.collectStatistics(string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class CollectStatisticsResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="CollectStatisticsRequest.table_name" />.  </summary>
        public string table_name { get; set; }

        /// <summary>Value of <paramref
        /// cref="CollectStatisticsRequest.column_names" />.  </summary>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CollectStatisticsResponse




}  // end namespace kinetica
