/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createEnvironment(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a new environment which can be used by <a
    /// href="../../../concepts/udf/" target="_top">user-defined functions</a>
    /// (UDF).</summary>
    public class CreateEnvironmentRequest : KineticaData
    {

        /// <summary>Name of the environment to be created.  </summary>
        public string environment_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateEnvironmentRequest object with default
        /// parameters.</summary>
        public CreateEnvironmentRequest() { }

        /// <summary>Constructs a CreateEnvironmentRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="environment_name">Name of the environment to be
        /// created.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public CreateEnvironmentRequest( string environment_name,
                                         IDictionary<string, string> options = null)
        {
            this.environment_name = environment_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateEnvironmentRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createEnvironment(string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateEnvironmentResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="CreateEnvironmentRequest.environment_name" />.  </summary>
        public string environment_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateEnvironmentResponse




}  // end namespace kinetica
