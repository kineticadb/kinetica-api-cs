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
    /// cref="Kinetica.createStateTable(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class CreateStateTableRequest : KineticaData
    {
        public string table_name { get; set; }
        public string input_table_name { get; set; }
        public string init_table_name { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateStateTableRequest object with default
        /// parameters.</summary>
        public CreateStateTableRequest() { }

        /// <summary>Constructs a CreateStateTableRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name"></param>
        /// <param name="input_table_name"></param>
        /// <param name="init_table_name"></param>
        /// <param name="options"></param>
        /// 
        public CreateStateTableRequest( string table_name,
                                        string input_table_name,
                                        string init_table_name,
                                        IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.input_table_name = input_table_name ?? "";
            this.init_table_name = init_table_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateStateTableRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createStateTable(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateStateTableResponse : KineticaData
    {
        public string table_name { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateStateTableResponse
    /// @endcond





}  // end namespace kinetica