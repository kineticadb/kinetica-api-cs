/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showSystemStatus(IDictionary{string, string})" />.
    /// <br />
    /// Provides server configuration and health related status to the caller.
    /// The admin tool uses it to present server related information to the
    /// user.</summary>
    public class ShowSystemStatusRequest : KineticaData
    {

        /// <summary>Optional parameters, currently unused.  The default value
        /// is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowSystemStatusRequest object with default
        /// parameters.</summary>
        public ShowSystemStatusRequest() { }

        /// <summary>Constructs a ShowSystemStatusRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="options">Optional parameters, currently unused.  The
        /// default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowSystemStatusRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowSystemStatusRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showSystemStatus(IDictionary{string, string})"
    /// />.</summary>
    public class ShowSystemStatusResponse : KineticaData
    {

        /// <summary>A map of server configuration and health related status.
        /// </summary>
        public IDictionary<string, string> status_map { get; set; } = new Dictionary<string, string>();

    } // end class ShowSystemStatusResponse




}  // end namespace kinetica
