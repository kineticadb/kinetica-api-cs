/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{
    /// @cond NO_DOCS
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createMonitorTable(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class CreateMonitorTableRequest : KineticaData
    {
        public string monitor_table_name { get; set; }
        public string table_name { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateMonitorTableRequest object with default
        /// parameters.</summary>
        public CreateMonitorTableRequest() { }

        /// <summary>Constructs a CreateMonitorTableRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="monitor_table_name"></param>
        /// <param name="table_name"></param>
        /// <param name="options"></param>
        /// 
        public CreateMonitorTableRequest( string monitor_table_name,
                                          string table_name,
                                          IDictionary<string, string> options = null)
        {
            this.monitor_table_name = monitor_table_name ?? "";
            this.table_name = table_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateMonitorTableRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createMonitorTable(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateMonitorTableResponse : KineticaData
    {
        public string monitor_table_name { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateMonitorTableResponse
    /// @endcond





}  // end namespace kinetica