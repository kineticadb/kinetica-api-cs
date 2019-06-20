/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createTableMonitor(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a monitor that watches for new records inserted into a
    /// particular table (identified by <see cref="table_name" />) and forwards
    /// copies to subscribers via ZMQ. After this call completes, subscribe to
    /// the returned <member name="topic_id" /> on the ZMQ table monitor port
    /// (default 9002). Each time an insert operation on the table completes, a
    /// multipart message is published for that topic; the first part contains
    /// only the topic ID, and each subsequent part contains one binary-encoded
    /// Avro object that was inserted. The monitor will continue to run
    /// (regardless of whether or not there are any subscribers) until
    /// deactivated with <see
    /// cref="Kinetica.clearTableMonitor(string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateTableMonitorRequest : KineticaData
    {

        /// <summary>Name of the table to monitor. Must not refer to a
        /// collection.  </summary>
        public string table_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateTableMonitorRequest object with default
        /// parameters.</summary>
        public CreateTableMonitorRequest() { }

        /// <summary>Constructs a CreateTableMonitorRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table to monitor. Must not
        /// refer to a collection.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public CreateTableMonitorRequest( string table_name,
                                          IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateTableMonitorRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createTableMonitor(string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateTableMonitorResponse : KineticaData
    {

        /// <summary>The ZMQ topic ID to subscribe to for inserted records.
        /// </summary>
        public string topic_id { get; set; }

        /// <summary>Value of <paramref
        /// cref="CreateTableMonitorRequest.table_name" />.  </summary>
        public string table_name { get; set; }

        /// <summary>JSON Avro schema of the table, for use in decoding
        /// published records.  </summary>
        public string type_schema { get; set; }

    } // end class CreateTableMonitorResponse




}  // end namespace kinetica
