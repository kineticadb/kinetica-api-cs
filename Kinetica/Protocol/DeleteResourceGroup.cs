/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.deleteResourceGroup(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Deletes a resource group.</summary>
    public class DeleteResourceGroupRequest : KineticaData
    {

        /// <summary>Name of the resource group to be deleted.  </summary>
        public string name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a DeleteResourceGroupRequest object with
        /// default parameters.</summary>
        public DeleteResourceGroupRequest() { }

        /// <summary>Constructs a DeleteResourceGroupRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the resource group to be deleted.
        /// </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public DeleteResourceGroupRequest( string name,
                                           IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class DeleteResourceGroupRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.deleteResourceGroup(string,IDictionary{string, string})"
    /// />.</summary>
    public class DeleteResourceGroupResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="DeleteResourceGroupRequest.name"
        /// />.  </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class DeleteResourceGroupResponse




}  // end namespace kinetica
