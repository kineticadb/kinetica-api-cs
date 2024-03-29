/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showDatasink(ShowDatasinkRequest)">Kinetica.showDatasink</see>.
    /// </summary>
    /// <remarks><para>Shows information about a specified <a
    /// href="../../../concepts/data_sinks/" target="_top">data sink</a> or all
    /// data sinks.</para></remarks>
    public class ShowDatasinkRequest : KineticaData
    {
        /// <summary>Name of the data sink for which to retrieve information.
        /// </summary>
        /// <remarks><para>The name must refer to a currently existing data
        /// sink. If '*' is specified, information about all data sinks will be
        /// returned.</para></remarks>
        public string name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a ShowDatasinkRequest object with default
        /// parameters.</summary>
        public ShowDatasinkRequest() { }

        /// <summary>Constructs a ShowDatasinkRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="name">Name of the data sink for which to retrieve
        /// information. The name must refer to a currently existing data sink.
        /// If '*' is specified, information about all data sinks will be
        /// returned.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public ShowDatasinkRequest( string name,
                                    IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowDatasinkRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showDatasink(ShowDatasinkRequest)">Kinetica.showDatasink</see>.
    /// </summary>
    public class ShowDatasinkResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="additional_info" />.</summary>
        /// <remarks><para>Additional information about the respective data
        /// sinks in <see cref="datasink_names" />.</para></remarks>
        public struct AdditionalInfo
        {
            /// <summary>Destination for the output data in
            /// 'destination_type://path[:port]' format</summary>
            public const string DESTINATION = "destination";

            /// <summary>Kafka topic if the data sink type is a Kafka broker
            /// </summary>
            public const string KAFKA_TOPIC_NAME = "kafka_topic_name";

            /// <summary>Name of the remote system user</summary>
            public const string USER_NAME = "user_name";
        } // end struct AdditionalInfo

        /// <summary>The data sink names.</summary>
        public IList<string> datasink_names { get; set; } = new List<string>();

        /// <summary>The destination type of the data sinks named in <see
        /// cref="datasink_names" />.</summary>
        public IList<string> destination_types { get; set; } = new List<string>();

        /// <summary>Additional information about the respective data sinks in
        /// <see cref="datasink_names" />.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="AdditionalInfo.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Destination for the output data in
        ///         'destination_type://path[:port]' format</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="AdditionalInfo.KAFKA_TOPIC_NAME">KAFKA_TOPIC_NAME</see>:
        ///         </term>
        ///         <description>Kafka topic if the data sink type is a Kafka
        ///         broker</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="AdditionalInfo.USER_NAME">USER_NAME</see>:
        ///         </term>
        ///         <description>Name of the remote system user</description>
        ///     </item>
        /// </list></remarks>
        public IList<IDictionary<string, string>> additional_info { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowDatasinkResponse
} // end namespace kinetica
