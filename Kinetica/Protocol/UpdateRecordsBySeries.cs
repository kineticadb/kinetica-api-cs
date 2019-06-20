/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.updateRecordsBySeries(string,string,string,IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Updates the view specified by <see cref="table_name" /> to include full
    /// series (track) information from the <see cref="world_table_name" /> for
    /// the series (tracks) present in the <see cref="view_name" />.</summary>
    public class UpdateRecordsBySeriesRequest : KineticaData
    {

        /// <summary>Name of the view on which the update operation will be
        /// performed. Must be an existing view.  </summary>
        public string table_name { get; set; }

        /// <summary>Name of the table containing the complete series (track)
        /// information.  </summary>
        public string world_table_name { get; set; }

        /// <summary>Optional name of the view containing the series (tracks)
        /// which have to be updated.  The default value is ''.</summary>
        public string view_name { get; set; } = "";

        /// <summary>The default value is an empty {@link List}.</summary>
        public IList<string> reserved { get; set; } = new List<string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an UpdateRecordsBySeriesRequest object with
        /// default parameters.</summary>
        public UpdateRecordsBySeriesRequest() { }

        /// <summary>Constructs an UpdateRecordsBySeriesRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the view on which the update
        /// operation will be performed. Must be an existing view.  </param>
        /// <param name="world_table_name">Name of the table containing the
        /// complete series (track) information.  </param>
        /// <param name="view_name">Optional name of the view containing the
        /// series (tracks) which have to be updated.  The default value is
        /// ''.</param>
        /// <param name="reserved">The default value is an empty {@link
        /// List}.</param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public UpdateRecordsBySeriesRequest( string table_name,
                                             string world_table_name,
                                             string view_name = null,
                                             IList<string> reserved = null,
                                             IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.world_table_name = world_table_name ?? "";
            this.view_name = view_name ?? "";
            this.reserved = reserved ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class UpdateRecordsBySeriesRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.updateRecordsBySeries(string,string,string,IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class UpdateRecordsBySeriesResponse : KineticaData
    {
        public int count { get; set; }

    } // end class UpdateRecordsBySeriesResponse




}  // end namespace kinetica
