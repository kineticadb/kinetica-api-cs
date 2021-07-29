/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showProcStatus(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Shows the statuses of running or completed proc instances. Results are
    /// grouped by run ID (as returned from <see
    /// cref="Kinetica.executeProc(string,IDictionary{string, string},IDictionary{string, byte[]},IList{string},IDictionary{string, IList{string}},IList{string},IDictionary{string, string})"
    /// />) and data segment ID (each invocation of the proc command on a data
    /// segment is assigned a data segment ID).</summary>
    public class ShowProcStatusRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.CLEAR_COMPLETE">CLEAR_COMPLETE</see>:</term>
        ///         <description>If set to <i>true</i>, if a proc instance has
        /// completed (either successfully or unsuccessfully) then its status
        /// will be cleared and no longer returned in subsequent calls.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.RUN_TAG">RUN_TAG</see>:</term>
        ///         <description>If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is specified, return the
        /// status for a proc instance that has a matching run ID and a
        /// matching run tag that was provided to /execute/proc. If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is not specified, return
        /// statuses for all proc instances where a matching run tag was
        /// provided to /execute/proc.  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If set to <i>true</i>, if a proc instance has
            /// completed (either successfully or unsuccessfully) then its
            /// status will be cleared and no longer returned in subsequent
            /// calls.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowProcStatusRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see>.</summary>
            public const string CLEAR_COMPLETE = "clear_complete";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <see cref="run_id" /> is specified, return the
            /// status for a proc instance that has a matching run ID and a
            /// matching run tag that was provided to <see
            /// cref="Kinetica.executeProc(string,IDictionary{string, string},IDictionary{string, byte[]},IList{string},IDictionary{string, IList{string}},IList{string},IDictionary{string, string})"
            /// />. If <see cref="run_id" /> is not specified, return statuses
            /// for all proc instances where a matching run tag was provided to
            /// <see
            /// cref="Kinetica.executeProc(string,IDictionary{string, string},IDictionary{string, byte[]},IList{string},IDictionary{string, IList{string}},IList{string},IDictionary{string, string})"
            /// />.  The default value is ''.</summary>
            public const string RUN_TAG = "run_tag";
        } // end struct Options


        /// <summary>The run ID of a specific proc instance for which the
        /// status will be returned. If a proc with a matching run ID is not
        /// found, the response will be empty. If not specified, the statuses
        /// of all executed proc instances will be returned.  The default value
        /// is ''.</summary>
        public string run_id { get; set; } = "";

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.CLEAR_COMPLETE">CLEAR_COMPLETE</see>:</term>
        ///         <description>If set to <i>true</i>, if a proc instance has
        /// completed (either successfully or unsuccessfully) then its status
        /// will be cleared and no longer returned in subsequent calls.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.RUN_TAG">RUN_TAG</see>:</term>
        ///         <description>If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is specified, return the
        /// status for a proc instance that has a matching run ID and a
        /// matching run tag that was provided to /execute/proc. If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is not specified, return
        /// statuses for all proc instances where a matching run tag was
        /// provided to /execute/proc.  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowProcStatusRequest object with default
        /// parameters.</summary>
        public ShowProcStatusRequest() { }

        /// <summary>Constructs a ShowProcStatusRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="run_id">The run ID of a specific proc instance for
        /// which the status will be returned. If a proc with a matching run ID
        /// is not found, the response will be empty. If not specified, the
        /// statuses of all executed proc instances will be returned.  The
        /// default value is ''.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.CLEAR_COMPLETE">CLEAR_COMPLETE</see>:</term>
        ///         <description>If set to <i>true</i>, if a proc instance has
        /// completed (either successfully or unsuccessfully) then its status
        /// will be cleared and no longer returned in subsequent calls.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowProcStatusRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusRequest.Options.RUN_TAG">RUN_TAG</see>:</term>
        ///         <description>If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is specified, return the
        /// status for a proc instance that has a matching run ID and a
        /// matching run tag that was provided to /execute/proc. If <paramref
        /// cref="ShowProcStatusRequest.run_id" /> is not specified, return
        /// statuses for all proc instances where a matching run tag was
        /// provided to /execute/proc.  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public ShowProcStatusRequest( string run_id = null,
                                      IDictionary<string, string> options = null)
        {
            this.run_id = run_id ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowProcStatusRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showProcStatus(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowProcStatusResponse : KineticaData
    {

        /// <summary>Overall statuses for the returned run IDs. Note that these
        /// are rollups and individual statuses may differ between data
        /// segments for the same run ID; see <member name="statuses" /> and
        /// <member name="messages" /> for statuses from individual data
        /// segments.
        /// Valid values are:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.RUNNING">RUNNING</see>:</term>
        ///         <description>The proc instance is currently
        /// running.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.COMPLETE">COMPLETE</see>:</term>
        ///         <description>The proc instance completed with no
        /// errors.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.KILLED">KILLED</see>:</term>
        ///         <description>The proc instance was killed before
        /// completion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.ERROR">ERROR</see>:</term>
        ///         <description>The proc instance failed with an
        /// error.</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="overall_statuses" />.</summary>
        public struct OverallStatuses
        {

            /// <summary>The proc instance is currently running.</summary>
            public const string RUNNING = "running";

            /// <summary>The proc instance completed with no errors.</summary>
            public const string COMPLETE = "complete";

            /// <summary>The proc instance was killed before
            /// completion.</summary>
            public const string KILLED = "killed";

            /// <summary>The proc instance failed with an error.</summary>
            public const string ERROR = "error";
        } // end struct OverallStatuses


        /// <summary>Statuses for the returned run IDs, grouped by data segment
        /// ID.
        /// Valid values are:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member name="statuses"
        /// />.</summary>
        public struct Statuses
        {

            /// <summary>The proc instance is currently running.</summary>
            public const string RUNNING = "running";

            /// <summary>The proc instance completed with no errors.</summary>
            public const string COMPLETE = "complete";

            /// <summary>The proc instance was killed before
            /// completion.</summary>
            public const string KILLED = "killed";

            /// <summary>The proc instance failed with an error.</summary>
            public const string ERROR = "error";
        } // end struct Statuses


        /// <summary>Output lines for the returned run IDs, grouped by data
        /// segment ID.
        /// Valid values are:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member name="output"
        /// />.</summary>
        public struct Output
        {

            /// <summary>Output lines from stdout.</summary>
            public const string STDOUT = "stdout";

            /// <summary>Output lines from stderr.</summary>
            public const string STDERR = "stderr";
        } // end struct Output


        /// <summary>The proc names corresponding to the returned run IDs.
        /// </summary>
        public IDictionary<string, string> proc_names { get; set; } = new Dictionary<string, string>();

        /// <summary>The string params passed to /execute/proc for the returned
        /// run IDs.  </summary>
        public IDictionary<string, IDictionary<string, string>> _params { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>The binary params passed to /execute/proc for the returned
        /// run IDs.  </summary>
        public IDictionary<string, IDictionary<string, byte[]>> bin_params { get; set; } = new Dictionary<string, IDictionary<string, byte[]>>();

        /// <summary>The input table names passed to /execute/proc for the
        /// returned run IDs.  </summary>
        public IDictionary<string, IList<string>> input_table_names { get; set; } = new Dictionary<string, IList<string>>();

        /// <summary>The input column names passed to /execute/proc for the
        /// returned run IDs, supplemented with the column names for input
        /// tables not included in the input column name map.  </summary>
        public IDictionary<string, IDictionary<string, IList<string>>> input_column_names { get; set; } = new Dictionary<string, IDictionary<string, IList<string>>>();

        /// <summary>The output table names passed to /execute/proc for the
        /// returned run IDs.  </summary>
        public IDictionary<string, IList<string>> output_table_names { get; set; } = new Dictionary<string, IList<string>>();

        /// <summary>The optional parameters passed to /execute/proc for the
        /// returned run IDs.  </summary>
        public IDictionary<string, IDictionary<string, string>> options { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>Overall statuses for the returned run IDs. Note that these
        /// are rollups and individual statuses may differ between data
        /// segments for the same run ID; see <member name="statuses" /> and
        /// <member name="messages" /> for statuses from individual data
        /// segments.
        /// Valid values are:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.RUNNING">RUNNING</see>:</term>
        ///         <description>The proc instance is currently
        /// running.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.COMPLETE">COMPLETE</see>:</term>
        ///         <description>The proc instance completed with no
        /// errors.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.KILLED">KILLED</see>:</term>
        ///         <description>The proc instance was killed before
        /// completion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowProcStatusResponse.OverallStatuses.ERROR">ERROR</see>:</term>
        ///         <description>The proc instance failed with an
        /// error.</description>
        ///     </item>
        /// </list>  </summary>
        public IDictionary<string, string> overall_statuses { get; set; } = new Dictionary<string, string>();

        /// <summary>Statuses for the returned run IDs, grouped by data segment
        /// ID.
        /// Valid values are:
        /// <list type="bullet">
        /// </list>  </summary>
        public IDictionary<string, IDictionary<string, string>> statuses { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>Messages containing additional status information for the
        /// returned run IDs, grouped by data segment ID.  </summary>
        public IDictionary<string, IDictionary<string, string>> messages { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>String results for the returned run IDs, grouped by data
        /// segment ID.  </summary>
        public IDictionary<string, IDictionary<string, IDictionary<string, string>>> results { get; set; } = new Dictionary<string, IDictionary<string, IDictionary<string, string>>>();

        /// <summary>Binary results for the returned run IDs, grouped by data
        /// segment ID.  </summary>
        public IDictionary<string, IDictionary<string, IDictionary<string, byte[]>>> bin_results { get; set; } = new Dictionary<string, IDictionary<string, IDictionary<string, byte[]>>>();

        /// <summary>Output lines for the returned run IDs, grouped by data
        /// segment ID.
        /// Valid values are:
        /// <list type="bullet">
        /// </list>  </summary>
        public IDictionary<string, IDictionary<string, IDictionary<string, IList<string>>>> output { get; set; } = new Dictionary<string, IDictionary<string, IDictionary<string, IList<string>>>>();

        /// <summary>Timing information for the returned run IDs, grouped by
        /// data segment ID.  </summary>
        public IDictionary<string, IDictionary<string, IDictionary<string, long>>> timings { get; set; } = new Dictionary<string, IDictionary<string, IDictionary<string, long>>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowProcStatusResponse




}  // end namespace kinetica
