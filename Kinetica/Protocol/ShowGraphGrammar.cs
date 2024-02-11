/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// @cond NO_DOCS
    public class ShowGraphGrammarRequest : KineticaData
    {
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        public ShowGraphGrammarRequest() { }

        public ShowGraphGrammarRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowGraphGrammarRequest
    /// @endcond

    /// @cond NO_DOCS
    public class ShowGraphGrammarResponse : KineticaData
    {
        public bool result { get; set; }
        public string components_json { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowGraphGrammarResponse
    /// @endcond
} // end namespace kinetica
