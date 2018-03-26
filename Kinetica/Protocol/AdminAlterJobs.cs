/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminAlterJobs(IList{int},string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Perform the requested action on a list of one or more job(s). Based on
    /// the type of job and the current state of execution, the action may not
    /// be successfully executed. The final result of the attempted actions for
    /// each specified job is returned in the status array of the response. See
    /// <a href="../../gpudbAdmin/job_manager.html" target="_top">Job
    /// Manager</a> for more information.</summary>
    public class AdminAlterJobsRequest : KineticaData
    {

        /// <summary>Action to be performed on the jobs specified by job_ids.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAlterJobsRequest.Action.CANCEL">CANCEL</see></term>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="action"
        /// />.</summary>
        public struct Action
        {
            public const string CANCEL = "cancel";
        } // end struct Action


        /// <summary>Jobs to be modified.  </summary>
        public IList<int> job_ids { get; set; } = new List<int>();

        /// <summary>Action to be performed on the jobs specified by job_ids.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAlterJobsRequest.Action.CANCEL">CANCEL</see></term>
        ///     </item>
        /// </list>  </summary>
        public string action { get; set; }

        /// <summary>Optional parameters.  </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminAlterJobsRequest object with default
        /// parameters.</summary>
        public AdminAlterJobsRequest() { }

        /// <summary>Constructs an AdminAlterJobsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="job_ids">Jobs to be modified.  </param>
        /// <param name="action">Action to be performed on the jobs specified
        /// by job_ids.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminAlterJobsRequest.Action.CANCEL">CANCEL</see></term>
        ///     </item>
        /// </list>  </param>
        /// <param name="options">Optional parameters.  </param>
        /// 
        public AdminAlterJobsRequest( IList<int> job_ids,
                                      string action,
                                      IDictionary<string, string> options = null)
        {
            this.job_ids = job_ids ?? new List<int>();
            this.action = action ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminAlterJobsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminAlterJobs(IList{int},string,IDictionary{string, string})"
    /// />.</summary>
    public class AdminAlterJobsResponse : KineticaData
    {

        /// <summary>Jobs on which the action was performed.  </summary>
        public IList<int> job_ids { get; set; } = new List<int>();

        /// <summary>Action requested on the jobs.  </summary>
        public string action { get; set; }

        /// <summary>Status of the requested action for each job.  </summary>
        public IList<string> status { get; set; } = new List<string>();

    } // end class AdminAlterJobsResponse




}  // end namespace kinetica
