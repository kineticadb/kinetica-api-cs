/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminShowJobs(IDictionary{string, string})" />.
    /// <br />
    /// Get a list of the current jobs in GPUdb.</summary>
    public class AdminShowJobsRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.SHOW_ASYNC_JOBS">SHOW_ASYNC_JOBS</see>:</term>
        ///         <description>If <i>true</i>, then the completed async jobs
        /// are also included in the response. By default, once the async jobs
        /// are completed they are no longer included in the jobs list.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, then the completed async jobs are also
            /// included in the response. By default, once the async jobs are
            /// completed they are no longer included in the jobs list.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AdminShowJobsRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see>.</summary>
            public const string SHOW_ASYNC_JOBS = "show_async_jobs";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.SHOW_ASYNC_JOBS">SHOW_ASYNC_JOBS</see>:</term>
        ///         <description>If <i>true</i>, then the completed async jobs
        /// are also included in the response. By default, once the async jobs
        /// are completed they are no longer included in the jobs list.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminShowJobsRequest object with default
        /// parameters.</summary>
        public AdminShowJobsRequest() { }

        /// <summary>Constructs an AdminShowJobsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.SHOW_ASYNC_JOBS">SHOW_ASYNC_JOBS</see>:</term>
        ///         <description>If <i>true</i>, then the completed async jobs
        /// are also included in the response. By default, once the async jobs
        /// are completed they are no longer included in the jobs list.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="AdminShowJobsRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AdminShowJobsRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminShowJobsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminShowJobs(IDictionary{string, string})"
    /// />.</summary>
    public class AdminShowJobsResponse : KineticaData
    {

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsResponse.Info.JOB_TAG">JOB_TAG</see>:</term>
        ///         <description>The job tag specified by the user or if
        /// unspecified by user, an internally generated unique identifier for
        /// the job across clusters.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <member name="info"
        /// />.</summary>
        public struct Info
        {

            /// <summary>The job tag specified by the user or if unspecified by
            /// user, an internally generated unique identifier for the job
            /// across clusters.</summary>
            public const string JOB_TAG = "job_tag";
        } // end struct Info

        public IList<long> job_id { get; set; } = new List<long>();
        public IList<string> status { get; set; } = new List<string>();
        public IList<string> endpoint_name { get; set; } = new List<string>();
        public IList<long> time_received { get; set; } = new List<long>();
        public IList<string> auth_id { get; set; } = new List<string>();
        public IList<string> source_ip { get; set; } = new List<string>();
        public IList<string> user_data { get; set; } = new List<string>();

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AdminShowJobsResponse.Info.JOB_TAG">JOB_TAG</see>:</term>
        ///         <description>The job tag specified by the user or if
        /// unspecified by user, an internally generated unique identifier for
        /// the job across clusters.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AdminShowJobsResponse




}  // end namespace kinetica
