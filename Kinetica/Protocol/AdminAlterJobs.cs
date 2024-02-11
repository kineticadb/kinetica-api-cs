/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminAlterJobs(AdminAlterJobsRequest)">Kinetica.adminAlterJobs</see>.
    /// </summary>
    /// <remarks><para>Perform the requested action on a list of one or more
    /// job(s). Based on the type of job and the current state of execution,
    /// the action may not be successfully executed. The final result of the
    /// attempted actions for each specified job is returned in the status
    /// array of the response. See <a href="../../../admin/job_manager/"
    /// target="_top">Job Manager</a> for more information.</para></remarks>
    public class AdminAlterJobsRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="action" />.</summary>
        /// <remarks><para>Action to be performed on the jobs specified by
        /// job_ids.</para></remarks>
        public struct Action
        {
            public const string CANCEL = "cancel";
        } // end struct Action

        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Job tag returned in call to create the job</summary>
            public const string JOB_TAG = "job_tag";
        } // end struct Options

        /// <summary>Jobs to be modified.</summary>
        public IList<long> job_ids { get; set; } = new List<long>();

        /// <summary>Action to be performed on the jobs specified by job_ids.
        /// </summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Action.CANCEL">CANCEL</see></term>
        ///     </item>
        /// </list></remarks>
        public string action { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.JOB_TAG">JOB_TAG</see>:</term>
        ///         <description>Job tag returned in call to create the job
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AdminAlterJobsRequest object with default
        /// parameters.</summary>
        public AdminAlterJobsRequest() { }

        /// <summary>Constructs an AdminAlterJobsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="job_ids">Jobs to be modified.</param>
        /// <param name="action">Action to be performed on the jobs specified
        /// by job_ids.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Action.CANCEL">CANCEL</see></term>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.JOB_TAG">JOB_TAG</see>:</term>
        ///         <description>Job tag returned in call to create the job
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public AdminAlterJobsRequest( IList<long> job_ids,
                                      string action,
                                      IDictionary<string, string> options = null)
        {
            this.job_ids = job_ids ?? new List<long>();
            this.action = action ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AdminAlterJobsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminAlterJobs(AdminAlterJobsRequest)">Kinetica.adminAlterJobs</see>.
    /// </summary>
    public class AdminAlterJobsResponse : KineticaData
    {
        /// <summary>Jobs on which the action was performed.</summary>
        public IList<long> job_ids { get; set; } = new List<long>();

        /// <summary>Action requested on the jobs.</summary>
        public string action { get; set; }

        /// <summary>Status of the requested action for each job.</summary>
        public IList<string> status { get; set; } = new List<string>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AdminAlterJobsResponse
} // end namespace kinetica
