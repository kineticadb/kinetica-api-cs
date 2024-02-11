/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.updateRecordsBySeries(UpdateRecordsBySeriesRequest)">Kinetica.updateRecordsBySeries</see>.
    /// </summary>
    /// <remarks><para>Updates the view specified by <see cref="table_name" />
    /// to include full series (track) information from the <see
    /// cref="world_table_name" /> for the series (tracks) present in the <see
    /// cref="view_name" />.</para></remarks>
    public class UpdateRecordsBySeriesRequest : KineticaData
    {
        /// <summary>Name of the view on which the update operation will be
        /// performed, in [schema_name.]view_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</summary>
        /// <remarks><para> Must be an existing view.</para></remarks>
        public string table_name { get; set; }

        /// <summary>Name of the table containing the complete series (track)
        /// information, in [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</summary>
        public string world_table_name { get; set; }

        /// <summary>Name of the view containing the series (tracks) which have
        /// to be updated, in [schema_name.]view_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</summary>
        /// <remarks><para>The default value is ''.</para></remarks>
        public string view_name { get; set; } = "";

        /// <summary>The default value is an empty List.</summary>
        public IList<string> reserved { get; set; } = new List<string>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an UpdateRecordsBySeriesRequest object with
        /// default parameters.</summary>
        public UpdateRecordsBySeriesRequest() { }

        /// <summary>Constructs an UpdateRecordsBySeriesRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Name of the view on which the update
        /// operation will be performed, in [schema_name.]view_name format,
        /// using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  Must be an existing view.
        /// </param>
        /// <param name="world_table_name">Name of the table containing the
        /// complete series (track) information, in [schema_name.]table_name
        /// format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</param>
        /// <param name="view_name">Name of the view containing the series
        /// (tracks) which have to be updated, in [schema_name.]view_name
        /// format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>. The default value is ''.
        /// </param>
        /// <param name="reserved">The default value is an empty List.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
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
    /// cref="Kinetica.updateRecordsBySeries(UpdateRecordsBySeriesRequest)">Kinetica.updateRecordsBySeries</see>.
    /// </summary>
    public class UpdateRecordsBySeriesResponse : KineticaData
    {
        public int count { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class UpdateRecordsBySeriesResponse
} // end namespace kinetica
