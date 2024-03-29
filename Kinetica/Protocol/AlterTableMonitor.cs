/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterTableMonitor(AlterTableMonitorRequest)">Kinetica.alterTableMonitor</see>.
    /// </summary>
    /// <remarks><para>Alters a table monitor previously created with <see
    /// cref="Kinetica.createTableMonitor(CreateTableMonitorRequest)">Kinetica.createTableMonitor</see>.
    /// </para></remarks>
    public class AlterTableMonitorRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="monitor_updates_map" />.</summary>
        /// <remarks><para>Map containing the properties of the table monitor
        /// to be updated. Error if empty.</para></remarks>
        public struct MonitorUpdatesMap
        {
            /// <summary>Updates the schema name.</summary>
            /// <remarks><para> If <see
            /// cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see> doesn't
            /// exist, an error will be thrown. If <see
            /// cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see> is
            /// empty, then the user's default schema will be used.</para>
            /// </remarks>
            public const string SCHEMA_NAME = "schema_name";
        } // end struct MonitorUpdatesMap

        /// <summary>The topic ID returned by <see
        /// cref="Kinetica.createTableMonitor(CreateTableMonitorRequest)">Kinetica.createTableMonitor</see>.
        /// </summary>
        public string topic_id { get; set; }

        /// <summary>Map containing the properties of the table monitor to be
        /// updated.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:
        ///         </term>
        ///         <description>Updates the schema name.  If <see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         doesn't exist, an error will be thrown. If <see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see> is
        ///         empty, then the user's default schema will be used.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public IDictionary<string, string> monitor_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AlterTableMonitorRequest object with default
        /// parameters.</summary>
        public AlterTableMonitorRequest() { }

        /// <summary>Constructs an AlterTableMonitorRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="topic_id">The topic ID returned by <see
        /// cref="Kinetica.createTableMonitor(CreateTableMonitorRequest)">Kinetica.createTableMonitor</see>.
        /// </param>
        /// <param name="monitor_updates_map">Map containing the properties of
        /// the table monitor to be updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:
        ///         </term>
        ///         <description>Updates the schema name.  If <see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         doesn't exist, an error will be thrown. If <see
        ///         cref="MonitorUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see> is
        ///         empty, then the user's default schema will be used.
        ///         </description>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters.</param>
        public AlterTableMonitorRequest( string topic_id,
                                         IDictionary<string, string> monitor_updates_map,
                                         IDictionary<string, string> options)
        {
            this.topic_id = topic_id ?? "";
            this.monitor_updates_map = monitor_updates_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AlterTableMonitorRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterTableMonitor(AlterTableMonitorRequest)">Kinetica.alterTableMonitor</see>.
    /// </summary>
    public class AlterTableMonitorResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="AlterTableMonitorRequest.topic_id">topic_id</see>.</summary>
        public string topic_id { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AlterTableMonitorResponse
} // end namespace kinetica
