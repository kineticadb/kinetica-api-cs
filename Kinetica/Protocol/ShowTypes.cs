/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showTypes(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Retrieves information for the specified data type ID or type label. For
    /// all data types that match the input criteria, the database returns the
    /// type ID, the type schema, the label (if available), and the type's
    /// column properties.</summary>
    public class ShowTypesRequest : KineticaData
    {

        /// <summary>Type Id returned in response to a call to /create/type.
        /// </summary>
        public string type_id { get; set; }

        /// <summary>Option string that was supplied by user in a call to
        /// /create/type.  </summary>
        public string label { get; set; }

        /// <summary>Optional parameters.  </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowTypesRequest object with default
        /// parameters.</summary>
        public ShowTypesRequest() { }

        /// <summary>Constructs a ShowTypesRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="type_id">Type Id returned in response to a call to
        /// /create/type.  </param>
        /// <param name="label">Option string that was supplied by user in a
        /// call to /create/type.  </param>
        /// <param name="options">Optional parameters.  </param>
        /// 
        public ShowTypesRequest( string type_id,
                                 string label,
                                 IDictionary<string, string> options = null)
        {
            this.type_id = type_id ?? "";
            this.label = label ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowTypesRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showTypes(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowTypesResponse : KineticaData
    {
        public IList<string> type_ids { get; set; } = new List<string>();
        public IList<string> type_schemas { get; set; } = new List<string>();
        public IList<string> labels { get; set; } = new List<string>();
        public IList<IDictionary<string, IList<string>>> properties { get; set; } = new List<IDictionary<string, IList<string>>>();

    } // end class ShowTypesResponse




}  // end namespace kinetica
