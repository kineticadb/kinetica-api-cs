/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.adminShowAlerts(int,IDictionary{string, string})" />.
    /// <br />
    /// Retrieves a list of the most recent alerts generated.  The number of
    /// alerts to retrieve is specified in this request.
    /// <br />
    /// Important: This endpoint is accessed via the host manager port rather
    /// than the primary database port; the default ports for host manager and
    /// the primary database can be found <a
    /// href="../../install/index.html#default-ports" target="_top">here</a>.
    /// If you are invoking this endpoint via a GPUdb API object, you must
    /// instantiate that object using the host manager port instead of the
    /// database port. The same IP address is used for both ports.
    /// <br />
    /// Returns lists of alert data, earliest to latest</summary>
    public class AdminShowAlertsRequest : KineticaData
    {

        /// <summary>Number of most recent alerts to request. The response will
        /// return <paramref cref="AdminShowAlertsRequest.num_alerts" />
        /// alerts, or less if there are less in the system. A value of 0
        /// returns all stored alerts.  </summary>
        public int num_alerts { get; set; }

        /// <summary>Optional parameters.  </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AdminShowAlertsRequest object with default
        /// parameters.</summary>
        public AdminShowAlertsRequest() { }

        /// <summary>Constructs an AdminShowAlertsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="num_alerts">Number of most recent alerts to request.
        /// The response will return <paramref
        /// cref="AdminShowAlertsRequest.num_alerts" /> alerts, or less if
        /// there are less in the system. A value of 0 returns all stored
        /// alerts.  </param>
        /// <param name="options">Optional parameters.  </param>
        /// 
        public AdminShowAlertsRequest( int num_alerts,
                                       IDictionary<string, string> options)
        {
            this.num_alerts = num_alerts;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AdminShowAlertsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.adminShowAlerts(int,IDictionary{string, string})"
    /// />.</summary>
    public class AdminShowAlertsResponse : KineticaData
    {

        /// <summary>System alert timestamps.  The array is sorted from
        /// earliest to latest.  Each array entry corresponds with the entries
        /// at the same index in <member name="types" /> and <member
        /// name="_params" />.  </summary>
        public IList<string> timestamps { get; set; } = new List<string>();

        /// <summary>System alert types.  The array is sorted from earliest to
        /// latest. Each array entry corresponds with the entries at the same
        /// index in <member name="timestamps" /> and <member name="_params"
        /// />.  </summary>
        public IList<string> types { get; set; } = new List<string>();

        /// <summary>Parameters for each alert.  The array is sorted from
        /// earliest to latest. Each array entry corresponds with the entries
        /// at the same index in <member name="timestamps" /> and <member
        /// name="types" />.  </summary>
        public IList<IDictionary<string, string>> _params { get; set; } = new List<IDictionary<string, string>>();

    } // end class AdminShowAlertsResponse




}  // end namespace kinetica
