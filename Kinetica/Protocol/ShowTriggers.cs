/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showTriggers(IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Retrieves information regarding the specified triggers or all existing
    /// triggers currently active.</summary>
    public class ShowTriggersRequest : KineticaData
    {

        /// <summary>List of IDs of the triggers whose information is to be
        /// retrieved. An empty list means information will be retrieved on all
        /// active triggers.  </summary>
        public IList<string> trigger_ids { get; set; } = new List<string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowTriggersRequest object with default
        /// parameters.</summary>
        public ShowTriggersRequest() { }

        /// <summary>Constructs a ShowTriggersRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="trigger_ids">List of IDs of the triggers whose
        /// information is to be retrieved. An empty list means information
        /// will be retrieved on all active triggers.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public ShowTriggersRequest( IList<string> trigger_ids,
                                    IDictionary<string, string> options = null)
        {
            this.trigger_ids = trigger_ids ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowTriggersRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showTriggers(IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class ShowTriggersResponse : KineticaData
    {

        /// <summary>This dictionary contains (key, value) pairs of (trigger
        /// ID, information map/dictionary) where the key is a Unicode string
        /// representing a Trigger ID. The value is another embedded dictionary
        /// containing (key, value) pairs where the keys consist of
        /// 'table_name', 'type' and the parameter names relating to the
        /// trigger type, e.g. *nai*, *min*, *max*. The values are unicode
        /// strings (numeric values are also converted to strings) representing
        /// the value of the respective parameter. If a trigger is associated
        /// with multiple tables, then the string value for *table_name*
        /// contains a comma separated list of table names.  </summary>
        public IDictionary<string, IDictionary<string, string>> trigger_map { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowTriggersResponse




}  // end namespace kinetica
