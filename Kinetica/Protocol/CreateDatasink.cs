/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createDatasink(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a <a href="../../../concepts/data_sinks/" target="_top">data
    /// sink</a>, which contains the
    /// destination information for a data sink that is external to the
    /// database.</summary>
    public class CreateDatasinkRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this data
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to publish to if
        /// <paramref cref="CreateDatasinkRequest.destination" /> is a Kafka
        /// broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// <br />
        /// If <i>nested</i>, records are returned as an array. Otherwise, only
        /// a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_JAR_PATH">JDBC_DRIVER_JAR_PATH</see>:</term>
        ///         <description>JDBC driver jar file location</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_CLASS_NAME">JDBC_DRIVER_CLASS_NAME</see>:</term>
        ///         <description>Name of the JDBC driver class</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Timeout in seconds for connecting to this data
            /// sink</summary>
            public const string CONNECTION_TIMEOUT = "connection_timeout";

            /// <summary>Timeout in seconds for waiting for a response from
            /// this data sink</summary>
            public const string WAIT_TIMEOUT = "wait_timeout";

            /// <summary>Name of the <a href="../../../concepts/credentials/"
            /// target="_top">credential</a> object to be used in this data
            /// sink</summary>
            public const string CREDENTIAL = "credential";

            /// <summary>Name of the Kafka topic to publish to if <see
            /// cref="destination" /> is a Kafka broker</summary>
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
            /// If <i>nested</i>, records are returned as an array. Otherwise,
            /// only a single record per messages is returned.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasinkRequest.Options.NESTED">NESTED</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see>.</summary>
            public const string JSON_FORMAT = "json_format";
            public const string FLAT = "flat";
            public const string NESTED = "nested";

            /// <summary>JDBC driver jar file location</summary>
            public const string JDBC_DRIVER_JAR_PATH = "jdbc_driver_jar_path";

            /// <summary>Name of the JDBC driver class</summary>
            public const string JDBC_DRIVER_CLASS_NAME = "jdbc_driver_class_name";

            /// <summary>Bypass validation of connection to this data sink.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasinkRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see>.</summary>
            public const string SKIP_VALIDATION = "skip_validation";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the data sink to be created.  </summary>
        public string name { get; set; }

        /// <summary>Destination for the output data in format
        /// 'destination_type://path[:port]'.
        /// <br />
        /// Supported destination types are 'http', 'https' and 'kafka'.
        /// </summary>
        public string destination { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this data
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to publish to if
        /// <paramref cref="CreateDatasinkRequest.destination" /> is a Kafka
        /// broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// <br />
        /// If <i>nested</i>, records are returned as an array. Otherwise, only
        /// a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_JAR_PATH">JDBC_DRIVER_JAR_PATH</see>:</term>
        ///         <description>JDBC driver jar file location</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_CLASS_NAME">JDBC_DRIVER_CLASS_NAME</see>:</term>
        ///         <description>Name of the JDBC driver class</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateDatasinkRequest object with default
        /// parameters.</summary>
        public CreateDatasinkRequest() { }

        /// <summary>Constructs a CreateDatasinkRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the data sink to be created.  </param>
        /// <param name="destination">Destination for the output data in format
        /// 'destination_type://path[:port]'.
        /// Supported destination types are 'http', 'https' and 'kafka'.
        /// </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this data
        /// sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for waiting for a response
        /// from this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.CREDENTIAL">CREDENTIAL</see>:</term>
        ///         <description>Name of the <a
        /// href="../../../concepts/credentials/" target="_top">credential</a>
        /// object to be used in this data sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:</term>
        ///         <description>Name of the Kafka topic to publish to if
        /// <paramref cref="CreateDatasinkRequest.destination" /> is a Kafka
        /// broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_BATCH_SIZE">MAX_BATCH_SIZE</see>:</term>
        ///         <description>Maximum number of records per notification
        /// message.  The default value is '1'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.MAX_MESSAGE_SIZE">MAX_MESSAGE_SIZE</see>:</term>
        ///         <description>Maximum size in bytes of each notification
        /// message.  The default value is '1000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JSON_FORMAT">JSON_FORMAT</see>:</term>
        ///         <description>The desired format of JSON encoded
        /// notifications message.
        /// If <i>nested</i>, records are returned as an array. Otherwise, only
        /// a single record per messages is returned.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.NESTED">NESTED</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FLAT">FLAT</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_JAR_PATH">JDBC_DRIVER_JAR_PATH</see>:</term>
        ///         <description>JDBC driver jar file location</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.JDBC_DRIVER_CLASS_NAME">JDBC_DRIVER_CLASS_NAME</see>:</term>
        ///         <description>Name of the JDBC driver class</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to this data
        /// sink.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasinkRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public CreateDatasinkRequest( string name,
                                      string destination,
                                      IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.destination = destination ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateDatasinkRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createDatasink(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateDatasinkResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateDatasinkRequest.name" />.
        /// </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateDatasinkResponse




}  // end namespace kinetica
