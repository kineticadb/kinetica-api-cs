/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// @cond NO_DOCS
    public class DropModelRequest : KineticaData
    {
        public string model_name { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        public DropModelRequest() { }

        public DropModelRequest( string model_name,
                                 IDictionary<string, string> options = null)
        {
            this.model_name = model_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class DropModelRequest
    /// @endcond

    /// @cond NO_DOCS
    public class DropModelResponse : KineticaData
    {
        public struct Info
        {
            public const string KML_RESPONSE = "kml_response";
        } // end struct Info

        public string model_name { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class DropModelResponse
    /// @endcond
} // end namespace kinetica
