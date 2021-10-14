/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.clearTableMonitor(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Deactivates a table monitor previously created with <see
    /// cref="Kinetica.createTableMonitor(string,IDictionary{string, string})"
    /// />.</summary>
    public class ClearTableMonitorRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.KEEP_AUTOGENERATED_SINK">KEEP_AUTOGENERATED_SINK</see>:</term>
        ///         <description>If <i>true</i>, the auto-generated <a
        /// href="../../../concepts/data_sinks/" target="_top">datasink</a>
        /// associated with this monitor, if there is one, will be retained for
        /// further use. If <i>false</i>, then the auto-generated sink will be
        /// dropped if there are no other monitors referencing it.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.CLEAR_ALL_REFERENCES">CLEAR_ALL_REFERENCES</see>:</term>
        ///         <description>If <i>true</i>, all references that share the
        /// same {@input topic_id} will be cleared.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, the auto-generated <a
            /// href="../../../concepts/data_sinks/" target="_top">datasink</a>
            /// associated with this monitor, if there is one, will be retained
            /// for further use. If <i>false</i>, then the auto-generated sink
            /// will be dropped if there are no other monitors referencing it.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</summary>
            public const string KEEP_AUTOGENERATED_SINK = "keep_autogenerated_sink";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <i>true</i>, all references that share the same
            /// {@input topic_id} will be cleared.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</summary>
            public const string CLEAR_ALL_REFERENCES = "clear_all_references";
        } // end struct Options


        /// <summary>The topic ID returned by /create/tablemonitor.  </summary>
        public string topic_id { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.KEEP_AUTOGENERATED_SINK">KEEP_AUTOGENERATED_SINK</see>:</term>
        ///         <description>If <i>true</i>, the auto-generated <a
        /// href="../../../concepts/data_sinks/" target="_top">datasink</a>
        /// associated with this monitor, if there is one, will be retained for
        /// further use. If <i>false</i>, then the auto-generated sink will be
        /// dropped if there are no other monitors referencing it.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.CLEAR_ALL_REFERENCES">CLEAR_ALL_REFERENCES</see>:</term>
        ///         <description>If <i>true</i>, all references that share the
        /// same {@input topic_id} will be cleared.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ClearTableMonitorRequest object with default
        /// parameters.</summary>
        public ClearTableMonitorRequest() { }

        /// <summary>Constructs a ClearTableMonitorRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="topic_id">The topic ID returned by
        /// /create/tablemonitor.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.KEEP_AUTOGENERATED_SINK">KEEP_AUTOGENERATED_SINK</see>:</term>
        ///         <description>If <i>true</i>, the auto-generated <a
        /// href="../../../concepts/data_sinks/" target="_top">datasink</a>
        /// associated with this monitor, if there is one, will be retained for
        /// further use. If <i>false</i>, then the auto-generated sink will be
        /// dropped if there are no other monitors referencing it.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.CLEAR_ALL_REFERENCES">CLEAR_ALL_REFERENCES</see>:</term>
        ///         <description>If <i>true</i>, all references that share the
        /// same {@input topic_id} will be cleared.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ClearTableMonitorRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ClearTableMonitorRequest( string topic_id,
                                         IDictionary<string, string> options = null)
        {
            this.topic_id = topic_id ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ClearTableMonitorRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.clearTableMonitor(string,IDictionary{string, string})"
    /// />.</summary>
    public class ClearTableMonitorResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="ClearTableMonitorRequest.topic_id" />.  </summary>
        public string topic_id { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ClearTableMonitorResponse




}  // end namespace kinetica
