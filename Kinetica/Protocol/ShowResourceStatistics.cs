/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showResourceStatistics(ShowResourceStatisticsRequest)">Kinetica.showResourceStatistics</see>.
    /// </summary>
    /// <remarks><para>Requests various statistics for storage/memory tiers and
    /// resource groups.
    /// Returns statistics on a per-rank basis.</para></remarks>
    public class ShowResourceStatisticsRequest : KineticaData
    {
        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a ShowResourceStatisticsRequest object with
        /// default parameters.</summary>
        public ShowResourceStatisticsRequest() { }

        /// <summary>Constructs a ShowResourceStatisticsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public ShowResourceStatisticsRequest( IDictionary<string, string> options = null)
        {
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowResourceStatisticsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showResourceStatistics(ShowResourceStatisticsRequest)">Kinetica.showResourceStatistics</see>.
    /// </summary>
    public class ShowResourceStatisticsResponse : KineticaData
    {
        /// <summary>Map of resource statistics</summary>
        public IDictionary<string, string> statistics_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowResourceStatisticsResponse
} // end namespace kinetica
