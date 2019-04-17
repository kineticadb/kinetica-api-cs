/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.deleteRole(string,IDictionary{string, string})" />.
    /// <br />
    /// Deletes an existing role.</summary>
    public class DeleteRoleRequest : KineticaData
    {

        /// <summary>Name of the role to be deleted. Must be an existing role.
        /// </summary>
        public string name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a DeleteRoleRequest object with default
        /// parameters.</summary>
        public DeleteRoleRequest() { }

        /// <summary>Constructs a DeleteRoleRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="name">Name of the role to be deleted. Must be an
        /// existing role.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public DeleteRoleRequest( string name,
                                  IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class DeleteRoleRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.deleteRole(string,IDictionary{string, string})"
    /// />.</summary>
    public class DeleteRoleResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="DeleteRoleRequest.name" />.
        /// </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class DeleteRoleResponse




}  // end namespace kinetica
