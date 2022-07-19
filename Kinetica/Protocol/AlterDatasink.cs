/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterDatasink(string,IDictionary{string, string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Alters the properties of an existing <a
    /// href="../../../concepts/data_sinks/" target="_top">data
    /// sink</a></summary>
    public class AlterDatasinkRequest : KineticaData
    {

        /// <summary>Map containing the properties of the data sink to be
        /// updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Destination for the output data in format
        /// 'destination_type://path[:port]'.
        /// <br />
        /// Supported destination types are 'http', 'https' and
        /// 'kafka'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to use for this data
        /// sink, if it references a Kafka broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// <br />
        /// If <i>nested</i>, records are returned as an array.
        /// Otherwise, only a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:</term>
        ///         <description>Updates the schema name.  If
        /// <i>schema_name</i>
        /// doesn't exist, an error will be thrown. If <i>schema_name</i> is
        /// empty, then the user's
        /// default schema will be used.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see
        /// cref="datasink_updates_map" />.</summary>
        public struct DatasinkUpdatesMap
        {

            /// <summary>Destination for the output data in format
            /// 'destination_type://path[:port]'.
            /// <br />
            /// Supported destination types are 'http', 'https' and
            /// 'kafka'.</summary>
            public const string DESTINATION = "destination";

            /// <summary>Timeout in seconds for connecting to this
            /// sink</summary>
            public const string CONNECTION_TIMEOUT = "connection_timeout";

            /// <summary>Timeout in seconds for waiting for a response from
            /// this sink</summary>
            public const string WAIT_TIMEOUT = "wait_timeout";

            /// <summary>Name of the <a href="../../../concepts/credentials/"
            /// target="_top">credential</a> object to be used in this data
            /// sink</summary>
            public const string CREDENTIAL = "credential";

            /// <summary>Name of the Kafka topic to use for this data sink, if
            /// it references a Kafka broker</summary>
            public const string KAFKA_TOPIC_NAME = "kafka_topic_name";

            /// <summary>Maximum number of records per notification message.
            /// The default value is '1'.</summary>
            public const string MAX_BATCH_SIZE = "max_batch_size";

            /// <summary>Maximum size in bytes of each notification message.
            /// The default value is '1000000'.</summary>
            public const string MAX_MESSAGE_SIZE = "max_message_size";

            /// <summary>The desired format of JSON encoded notifications
            /// message.
            /// <br />
            /// If <i>nested</i>, records are returned as an array.
            /// Otherwise, only a single record per messages is returned.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.NESTED">NESTED</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see>.</summary>
            public const string JSON_FORMAT = "json_format";
            public const string FLAT = "flat";
            public const string NESTED = "nested";

            /// <summary>Bypass validation of connection to this data sink.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see>.</summary>
            public const string SKIP_VALIDATION = "skip_validation";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Updates the schema name.  If <i>schema_name</i>
            /// doesn't exist, an error will be thrown. If <i>schema_name</i>
            /// is empty, then the user's
            /// default schema will be used.</summary>
            public const string SCHEMA_NAME = "schema_name";
        } // end struct DatasinkUpdatesMap


        /// <summary>Name of the data sink to be altered. Must be an existing
        /// data sink.  </summary>
        public string name { get; set; }

        /// <summary>Map containing the properties of the data sink to be
        /// updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Destination for the output data in format
        /// 'destination_type://path[:port]'.
        /// <br />
        /// Supported destination types are 'http', 'https' and
        /// 'kafka'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to use for this data
        /// sink, if it references a Kafka broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// <br />
        /// If <i>nested</i>, records are returned as an array.
        /// Otherwise, only a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:</term>
        ///         <description>Updates the schema name.  If
        /// <i>schema_name</i>
        /// doesn't exist, an error will be thrown. If <i>schema_name</i> is
        /// empty, then the user's
        /// default schema will be used.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> datasink_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.  </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterDatasinkRequest object with default
        /// parameters.</summary>
        public AlterDatasinkRequest() { }

        /// <summary>Constructs an AlterDatasinkRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the data sink to be altered. Must be an
        /// existing data sink.  </param>
        /// <param name="datasink_updates_map">Map containing the properties of
        /// the data sink to be updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Destination for the output data in format
        /// 'destination_type://path[:port]'.
        /// Supported destination types are 'http', 'https' and
        /// 'kafka'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to use for this data
        /// sink, if it references a Kafka broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// If <i>nested</i>, records are returned as an array.
        /// Otherwise, only a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterDatasinkRequest.DatasinkUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:</term>
        ///         <description>Updates the schema name.  If
        /// <i>schema_name</i>
        /// doesn't exist, an error will be thrown. If <i>schema_name</i> is
        /// empty, then the user's
        /// default schema will be used.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// <param name="options">Optional parameters.  </param>
        /// 
        public AlterDatasinkRequest( string name,
                                     IDictionary<string, string> datasink_updates_map,
                                     IDictionary<string, string> options)
        {
            this.name = name ?? "";
            this.datasink_updates_map = datasink_updates_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterDatasinkRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterDatasink(string,IDictionary{string, string},IDictionary{string, string})"
    /// />.</summary>
    public class AlterDatasinkResponse : KineticaData
    {

        /// <summary>Map of values updated  </summary>
        public IDictionary<string, string> updated_properties_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterDatasinkResponse




}  // end namespace kinetica
