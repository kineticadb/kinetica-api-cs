/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showTableMonitors(IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Show table monitors and their properties. Table monitors are created
    /// using <see
    /// cref="Kinetica.createTableMonitor(string,IDictionary{string, string})"
    /// />.
    /// Returns detailed information about existing table monitors.</summary>
    public class ShowTableMonitorsRequest : KineticaData
    {

        /// <summary>List of monitors to be shown. An empty list or a single
        /// entry with an empty string returns all table monitors.  </summary>
        public IList<string> monitor_ids { get; set; } = new List<string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowTableMonitorsRequest object with default
        /// parameters.</summary>
        public ShowTableMonitorsRequest() { }

        /// <summary>Constructs a ShowTableMonitorsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="monitor_ids">List of monitors to be shown. An empty
        /// list or a single entry with an empty string returns all table
        /// monitors.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public ShowTableMonitorsRequest( IList<string> monitor_ids,
                                         IDictionary<string, string> options = null)
        {
            this.monitor_ids = monitor_ids ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowTableMonitorsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showTableMonitors(IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class ShowTableMonitorsResponse : KineticaData
    {

        /// <summary>Additional information about the respective monitors in
        /// <member name="monitor_ids" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="additional_info" />.</summary>
        public struct AdditionalInfo
        {

            /// <summary>Notification type for the respective <member
            /// name="monitor_ids" /> and <member name="table_names" />.  The
            /// default value is ''.</summary>
            public const string MONITOR_TYPE = "monitor_type";

            /// <summary>Notification type schemas for the respective <member
            /// name="monitor_ids" /> and <member name="table_names" />.  The
            /// default value is ''.</summary>
            public const string TYPE_SCHEMA = "type_schema";

            /// <summary>Materialized view that implements the change
            /// detector</summary>
            public const string MATERIALIZED_VIEW_FOR_CHANGE_DETECTOR = "materialized_view_for_change_detector";

            /// <summary>Materialized views created for the <member
            /// name="filter_expressions" />.  The default value is
            /// ''.</summary>
            public const string MATERIALIZED_VIEW_FOR_FILTER = "materialized_view_for_filter";

            /// <summary>Reference count on the respective <member
            /// name="monitor_ids" />.  The default value is ''.</summary>
            public const string REFERENCES = "references";

            /// <summary>Datasink info in JSON format for the respective
            /// <member name="monitor_ids" /> if one is defined.  The default
            /// value is ''.</summary>
            public const string DATASINK_JSON = "datasink_json";
        } // end struct AdditionalInfo


        /// <summary>List of monitor IDs.  </summary>
        public IList<string> monitor_ids { get; set; } = new List<string>();

        /// <summary>List of source tables being monitored for the respective
        /// <member name="monitor_ids" />.  </summary>
        public IList<string> table_names { get; set; } = new List<string>();

        /// <summary>List of notification events for the respective <member
        /// name="monitor_ids" />.  </summary>
        public IList<string> events { get; set; } = new List<string>();

        /// <summary>List of columns used on the respective tables in <member
        /// name="table_names" /> that will increase for new records.
        /// </summary>
        public IList<string> increasing_columns { get; set; } = new List<string>();

        /// <summary>List of filter expressions used on the respective tables
        /// in <member name="table_names" /> to limit records for
        /// notifications.  </summary>
        public IList<string> filter_expressions { get; set; } = new List<string>();

        /// <summary>List of refresh methods used on the respective tables in
        /// <member name="table_names" />.  </summary>
        public IList<string> refresh_method { get; set; } = new List<string>();

        /// <summary>List of refresh periods used on the respective tables in
        /// <member name="table_names" />.  </summary>
        public IList<string> refresh_period { get; set; } = new List<string>();

        /// <summary>List of refresh start times used on the respective tables
        /// in <member name="table_names" />.  </summary>
        public IList<string> refresh_start_time { get; set; } = new List<string>();

        /// <summary>List of datasink names for the respective <member
        /// name="monitor_ids" /> if one is defined.  </summary>
        public IList<string> datasink_names { get; set; } = new List<string>();

        /// <summary>Additional information about the respective monitors in
        /// <member name="monitor_ids" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>  </summary>
        public IList<IDictionary<string, string>> additional_info { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowTableMonitorsResponse




}  // end namespace kinetica
