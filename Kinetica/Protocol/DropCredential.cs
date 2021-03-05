/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.dropCredential(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Drop an existing <a href="../../../concepts/credentials/"
    /// target="_top">credential</a>.</summary>
    public class DropCredentialRequest : KineticaData
    {

        /// <summary>Name of the credential to be dropped. Must be an existing
        /// credential.  </summary>
        public string credential_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a DropCredentialRequest object with default
        /// parameters.</summary>
        public DropCredentialRequest() { }

        /// <summary>Constructs a DropCredentialRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="credential_name">Name of the credential to be dropped.
        /// Must be an existing credential.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public DropCredentialRequest( string credential_name,
                                      IDictionary<string, string> options = null)
        {
            this.credential_name = credential_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class DropCredentialRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.dropCredential(string,IDictionary{string, string})"
    /// />.</summary>
    public class DropCredentialResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="DropCredentialRequest.credential_name" />.  </summary>
        public string credential_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class DropCredentialResponse




}  // end namespace kinetica
