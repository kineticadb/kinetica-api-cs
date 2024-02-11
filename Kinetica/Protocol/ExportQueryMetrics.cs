/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.exportQueryMetrics(ExportQueryMetricsRequest)">Kinetica.exportQueryMetrics</see>.
    /// </summary>
    /// <remarks><para>Export query metrics to a given destination.
    /// Returns query metrics.</para></remarks>
    public class ExportQueryMetricsRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Export query metrics for the currently running job
            /// </summary>
            public const string JOB_ID = "job_id";

            /// <summary>Specifies which format to export the metrics.
            /// </summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.JSON">JSON</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see cref="Options.JSON">JSON</see>.
            /// </para></remarks>
            public const string FORMAT = "format";

            public const string JSON = "json";
        } // end struct Options

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.JOB_ID">JOB_ID</see>:</term>
        ///         <description>Export query metrics for the currently running
        ///         job</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.FORMAT">FORMAT</see>:</term>
        ///         <description>Specifies which format to export the metrics.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.JSON">JSON</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.JSON">JSON</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an ExportQueryMetricsRequest object with
        /// default parameters.</summary>
        public ExportQueryMetricsRequest() { }

        /// <summary>Constructs an ExportQueryMetricsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.JOB_ID">JOB_ID</see>:</term>
        ///         <description>Export query metrics for the currently running
        ///         job</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.FORMAT">FORMAT</see>:</term>
        ///         <description>Specifies which format to export the metrics.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.JSON">JSON</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.JSON">JSON</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public ExportQueryMetricsRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ExportQueryMetricsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.exportQueryMetrics(ExportQueryMetricsRequest)">Kinetica.exportQueryMetrics</see>.
    /// </summary>
    public class ExportQueryMetricsResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="info" />.</summary>
        /// <remarks><para>Additional information.</para></remarks>
        public struct Info
        {
            /// <summary>Exported metrics if no other destination specified
            /// </summary>
            public const string OUTPUT = "output";
        } // end struct Info

        /// <summary>Additional information.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Info.OUTPUT">OUTPUT</see>:</term>
        ///         <description>Exported metrics if no other destination
        ///         specified</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ExportQueryMetricsResponse
} // end namespace kinetica