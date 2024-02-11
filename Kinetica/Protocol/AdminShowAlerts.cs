/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminShowAlerts(AdminShowAlertsRequest)">Kinetica.adminShowAlerts</see>.
    /// </summary>
    /// <remarks><para>Requests a list of the most recent alerts.
    /// Returns lists of alert data, including timestamp and type.</para>
    /// </remarks>
    public class AdminShowAlertsRequest : KineticaData
    {
        /// <summary>Number of most recent alerts to request.</summary>
        /// <remarks><para>The response will include up to <see
        /// cref="num_alerts" /> depending on how many alerts there are in the
        /// system. A value of 0 returns all stored alerts.</para></remarks>
        public int num_alerts { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AdminShowAlertsRequest object with default
        /// parameters.</summary>
        public AdminShowAlertsRequest() { }

        /// <summary>Constructs an AdminShowAlertsRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="num_alerts">Number of most recent alerts to request.
        /// The response will include up to <paramref name="num_alerts" />
        /// depending on how many alerts there are in the system. A value of 0
        /// returns all stored alerts.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public AdminShowAlertsRequest( int num_alerts,
                                       IDictionary<string, string> options = null)
        {
            this.num_alerts = num_alerts;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AdminShowAlertsRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminShowAlerts(AdminShowAlertsRequest)">Kinetica.adminShowAlerts</see>.
    /// </summary>
    public class AdminShowAlertsResponse : KineticaData
    {
        /// <summary>Timestamp for when the alert occurred, sorted from most
        /// recent to least recent.</summary>
        /// <remarks><para>Each array entry corresponds with the entries at the
        /// same index in <see cref="types" /> and <see cref="_params" />.
        /// </para></remarks>
        public IList<string> timestamps { get; set; } = new List<string>();

        /// <summary>Type of system alert, sorted from most recent to least
        /// recent.</summary>
        /// <remarks><para>Each array entry corresponds with the entries at the
        /// same index in <see cref="timestamps" /> and <see cref="_params" />.
        /// </para></remarks>
        public IList<string> types { get; set; } = new List<string>();

        /// <summary>Parameters for each alert, sorted from most recent to
        /// least recent.</summary>
        /// <remarks><para>Each array entry corresponds with the entries at the
        /// same index in <see cref="timestamps" /> and <see cref="types" />.
        /// </para></remarks>
        public IList<IDictionary<string, string>> _params { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AdminShowAlertsResponse
} // end namespace kinetica
