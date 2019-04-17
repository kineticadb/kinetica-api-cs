/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterTableMetadata(IList{string},IDictionary{string, string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Updates (adds or changes) metadata for tables. The metadata key and
    /// values must both be strings. This is an easy way to annotate whole
    /// tables rather than single records within tables.  Some examples of
    /// metadata are owner of the table, table creation timestamp
    /// etc.</summary>
    public class AlterTableMetadataRequest : KineticaData
    {

        /// <summary>Names of the tables whose metadata will be updated. All
        /// specified tables must exist, or an error will be returned.
        /// </summary>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>A map which contains the metadata of the tables that are
        /// to be updated. Note that only one map is provided for all the
        /// tables; so the change will be applied to every table. If the
        /// provided map is empty, then all existing metadata for the table(s)
        /// will be cleared.  </summary>
        public IDictionary<string, string> metadata_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterTableMetadataRequest object with
        /// default parameters.</summary>
        public AlterTableMetadataRequest() { }

        /// <summary>Constructs an AlterTableMetadataRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_names">Names of the tables whose metadata will
        /// be updated. All specified tables must exist, or an error will be
        /// returned.  </param>
        /// <param name="metadata_map">A map which contains the metadata of the
        /// tables that are to be updated. Note that only one map is provided
        /// for all the tables; so the change will be applied to every table.
        /// If the provided map is empty, then all existing metadata for the
        /// table(s) will be cleared.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public AlterTableMetadataRequest( IList<string> table_names,
                                          IDictionary<string, string> metadata_map,
                                          IDictionary<string, string> options = null)
        {
            this.table_names = table_names ?? new List<string>();
            this.metadata_map = metadata_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterTableMetadataRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterTableMetadata(IList{string},IDictionary{string, string},IDictionary{string, string})"
    /// />.</summary>
    public class AlterTableMetadataResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="AlterTableMetadataRequest.table_names" />.  </summary>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>Value of <paramref
        /// cref="AlterTableMetadataRequest.metadata_map" />.  </summary>
        public IDictionary<string, string> metadata_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterTableMetadataResponse




}  // end namespace kinetica
