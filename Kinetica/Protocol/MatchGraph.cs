/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.matchGraph(string,IList{string},string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Matches a directed route implied by a given set of latitude/longitude
    /// points to an existing underlying road network graph using a given
    /// solution type. See <a
    /// href="../../graph_solver/network_graph_solver.html"
    /// target="_top">Network Graph Solvers</a> for more information.</summary>
    public class MatchGraphRequest : KineticaData
    {

        /// <summary>The type of solver to use for graph matching.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using the
        /// Hidden Markov Model (HMM)-based method, which conducts a range-tree
        /// closest-edge search to find the best combinations of possible road
        /// segments (<i>num_segments</i>) for each sample point to create the
        /// best route. The route is secured one point at a time while looking
        /// ahead <i>chain_width</i> number of points, so the prediction is
        /// corrected after each point. This solution type is the most accurate
        /// but also the most computationally intensive.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.INCREMENTAL_WEIGHTED">INCREMENTAL_WEIGHTED</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using time
        /// and/or distance between points to influence one or more shortest
        /// paths across the sample points.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MATCH_OD_PAIRS">MATCH_OD_PAIRS</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to find the most probable
        /// path between origin and destination pairs with cost
        /// constraints</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>.
        /// A set of string constants for the parameter <see
        /// cref="solve_method" />.</summary>
        public struct SolveMethod
        {

            /// <summary>Matches <see cref="sample_points" /> to the graph
            /// using the Hidden Markov Model (HMM)-based method, which
            /// conducts a range-tree closest-edge search to find the best
            /// combinations of possible road segments (<i>num_segments</i>)
            /// for each sample point to create the best route. The route is
            /// secured one point at a time while looking ahead
            /// <i>chain_width</i> number of points, so the prediction is
            /// corrected after each point. This solution type is the most
            /// accurate but also the most computationally intensive.</summary>
            public const string MARKOV_CHAIN = "markov_chain";

            /// <summary>Matches <see cref="sample_points" /> to the graph
            /// using time and/or distance between points to influence one or
            /// more shortest paths across the sample points.</summary>
            public const string INCREMENTAL_WEIGHTED = "incremental_weighted";

            /// <summary>Matches <see cref="sample_points" /> to find the most
            /// probable path between origin and destination pairs with cost
            /// constraints</summary>
            public const string MATCH_OD_PAIRS = "match_od_pairs";
        } // end struct SolveMethod


        /// <summary>Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.GPS_NOISE">GPS_NOISE</see>:</term>
        ///         <description>GPS noise value (in meters) to remove
        /// redundant sample points. Use -1 to disable noise reduction. The
        /// default value accounts for 95% of point variation (+ or -5 meters).
        /// The default value is '5.0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.NUM_SEGMENTS">NUM_SEGMENTS</see>:</term>
        ///         <description>Maximum number of potentially matching road
        /// segments for each sample point. For the <i>markov_chain</i> solver,
        /// the default is 3; for the <i>incremental_weighted</i>, the default
        /// is 5.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SEARCH_RADIUS">SEARCH_RADIUS</see>:</term>
        ///         <description>Maximum search radius used when snapping
        /// sample points onto potentially matching surrounding segments. The
        /// default value corresponds to approximately 100 meters.  The default
        /// value is '0.001'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.CHAIN_WIDTH">CHAIN_WIDTH</see>:</term>
        ///         <description>For the <i>markov_chain</i> solver only.
        /// Length of the sample points lookahead window within the Markov
        /// kernel; the larger the number, the more accurate the solution.  The
        /// default value is '9'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.MAX_SOLVE_LENGTH">MAX_SOLVE_LENGTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Maximum number of samples along the path on which to solve.
        /// The default value is '200'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TIME_WINDOW_WIDTH">TIME_WINDOW_WIDTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Time window, also known as sampling period, in which points
        /// are favored. To determine the raw window value, the
        /// <i>time_window_width</i> value is multiplied by the mean sample
        /// time (in seconds) across all points, e.g., if
        /// <i>time_window_width</i> is 30 and the mean sample time is 2
        /// seconds, points that are sampled greater than 60 seconds after the
        /// previous point are no longer favored in the solution.  The default
        /// value is '30'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DETECT_LOOPS">DETECT_LOOPS</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. If <i>true</i>, a loop will be detected and traversed even if
        /// it would make a shorter path to ignore the loop.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SOURCE">SOURCE</see>:</term>
        ///         <description>Optional WKT starting point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// starting point.  The default value is 'POINT NULL'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Optional WKT ending point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// destination point.  The default value is 'POINT
        /// NULL'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>GPS noise value (in meters) to remove redundant sample
            /// points. Use -1 to disable noise reduction. The default value
            /// accounts for 95% of point variation (+ or -5 meters).  The
            /// default value is '5.0'.</summary>
            public const string GPS_NOISE = "gps_noise";

            /// <summary>Maximum number of potentially matching road segments
            /// for each sample point. For the <i>markov_chain</i> solver, the
            /// default is 3; for the <i>incremental_weighted</i>, the default
            /// is 5.  The default value is ''.</summary>
            public const string NUM_SEGMENTS = "num_segments";

            /// <summary>Maximum search radius used when snapping sample points
            /// onto potentially matching surrounding segments. The default
            /// value corresponds to approximately 100 meters.  The default
            /// value is '0.001'.</summary>
            public const string SEARCH_RADIUS = "search_radius";

            /// <summary>For the <i>markov_chain</i> solver only. Length of the
            /// sample points lookahead window within the Markov kernel; the
            /// larger the number, the more accurate the solution.  The default
            /// value is '9'.</summary>
            public const string CHAIN_WIDTH = "chain_width";

            /// <summary>For the <i>incremental_weighted</i> solver only.
            /// Maximum number of samples along the path on which to solve.
            /// The default value is '200'.</summary>
            public const string MAX_SOLVE_LENGTH = "max_solve_length";

            /// <summary>For the <i>incremental_weighted</i> solver only. Time
            /// window, also known as sampling period, in which points are
            /// favored. To determine the raw window value, the
            /// <i>time_window_width</i> value is multiplied by the mean sample
            /// time (in seconds) across all points, e.g., if
            /// <i>time_window_width</i> is 30 and the mean sample time is 2
            /// seconds, points that are sampled greater than 60 seconds after
            /// the previous point are no longer favored in the solution.  The
            /// default value is '30'.</summary>
            public const string TIME_WINDOW_WIDTH = "time_window_width";

            /// <summary>For the <i>incremental_weighted</i> solver only. If
            /// <i>true</i>, a loop will be detected and traversed even if it
            /// would make a shorter path to ignore the loop.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="MatchGraphRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="MatchGraphRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="MatchGraphRequest.Options.TRUE">TRUE</see>.</summary>
            public const string DETECT_LOOPS = "detect_loops";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Optional WKT starting point from <see
            /// cref="sample_points" /> for the solver. The default behavior
            /// for the endpoint is to use time to determine the starting
            /// point.  The default value is 'POINT NULL'.</summary>
            public const string SOURCE = "source";

            /// <summary>Optional WKT ending point from <see
            /// cref="sample_points" /> for the solver. The default behavior
            /// for the endpoint is to use time to determine the destination
            /// point.  The default value is 'POINT NULL'.</summary>
            public const string DESTINATION = "destination";
        } // end struct Options


        /// <summary>Name of the underlying geospatial graph resource to match
        /// to using <paramref cref="MatchGraphRequest.sample_points" />.
        /// </summary>
        public string graph_name { get; set; }

        /// <summary>Sample points used to match to an underlying geospatial
        /// graph. Sample points must be specified using <a
        /// href="../../graph_solver/network_graph_solver.html#match-identifiers"
        /// target="_top">identifiers</a>; identifiers are grouped as <a
        /// href="../../graph_solver/network_graph_solver.html#match-combinations"
        /// target="_top">combinations</a>. Identifiers can be used with:
        /// existing column names, e.g., 'table.column AS SAMPLE_X';
        /// expressions, e.g., 'ST_MAKEPOINT(table.x, table.y) AS
        /// SAMPLE_WKTPOINT'; or raw values, e.g., '{1, 2, 10} AS
        /// SAMPLE_TRIPID'.  </summary>
        public IList<string> sample_points { get; set; } = new List<string>();

        /// <summary>The type of solver to use for graph matching.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using the
        /// Hidden Markov Model (HMM)-based method, which conducts a range-tree
        /// closest-edge search to find the best combinations of possible road
        /// segments (<i>num_segments</i>) for each sample point to create the
        /// best route. The route is secured one point at a time while looking
        /// ahead <i>chain_width</i> number of points, so the prediction is
        /// corrected after each point. This solution type is the most accurate
        /// but also the most computationally intensive.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.INCREMENTAL_WEIGHTED">INCREMENTAL_WEIGHTED</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using time
        /// and/or distance between points to influence one or more shortest
        /// paths across the sample points.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MATCH_OD_PAIRS">MATCH_OD_PAIRS</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to find the most probable
        /// path between origin and destination pairs with cost
        /// constraints</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>.
        /// </summary>
        public string solve_method { get; set; } = SolveMethod.MARKOV_CHAIN;

        /// <summary>The name of the table used to store the results; this
        /// table contains a <a
        /// href="../../geospatial/geo_objects.html#geospatial-tracks"
        /// target="_top">track</a> of geospatial points for the matched
        /// portion of the graph, a track ID, and a score value. Also outputs a
        /// details table containing a trip ID (that matches the track ID), the
        /// latitude/longitude pair, the timestamp the point was recorded at,
        /// and an edge ID corresponding to the matched road segment. Has the
        /// same naming restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>. Must not be an existing table of the same
        /// name.  The default value is ''.</summary>
        public string solution_table { get; set; } = "";

        /// <summary>Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.GPS_NOISE">GPS_NOISE</see>:</term>
        ///         <description>GPS noise value (in meters) to remove
        /// redundant sample points. Use -1 to disable noise reduction. The
        /// default value accounts for 95% of point variation (+ or -5 meters).
        /// The default value is '5.0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.NUM_SEGMENTS">NUM_SEGMENTS</see>:</term>
        ///         <description>Maximum number of potentially matching road
        /// segments for each sample point. For the <i>markov_chain</i> solver,
        /// the default is 3; for the <i>incremental_weighted</i>, the default
        /// is 5.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SEARCH_RADIUS">SEARCH_RADIUS</see>:</term>
        ///         <description>Maximum search radius used when snapping
        /// sample points onto potentially matching surrounding segments. The
        /// default value corresponds to approximately 100 meters.  The default
        /// value is '0.001'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.CHAIN_WIDTH">CHAIN_WIDTH</see>:</term>
        ///         <description>For the <i>markov_chain</i> solver only.
        /// Length of the sample points lookahead window within the Markov
        /// kernel; the larger the number, the more accurate the solution.  The
        /// default value is '9'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.MAX_SOLVE_LENGTH">MAX_SOLVE_LENGTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Maximum number of samples along the path on which to solve.
        /// The default value is '200'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TIME_WINDOW_WIDTH">TIME_WINDOW_WIDTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Time window, also known as sampling period, in which points
        /// are favored. To determine the raw window value, the
        /// <i>time_window_width</i> value is multiplied by the mean sample
        /// time (in seconds) across all points, e.g., if
        /// <i>time_window_width</i> is 30 and the mean sample time is 2
        /// seconds, points that are sampled greater than 60 seconds after the
        /// previous point are no longer favored in the solution.  The default
        /// value is '30'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DETECT_LOOPS">DETECT_LOOPS</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. If <i>true</i>, a loop will be detected and traversed even if
        /// it would make a shorter path to ignore the loop.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SOURCE">SOURCE</see>:</term>
        ///         <description>Optional WKT starting point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// starting point.  The default value is 'POINT NULL'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Optional WKT ending point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// destination point.  The default value is 'POINT
        /// NULL'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a MatchGraphRequest object with default
        /// parameters.</summary>
        public MatchGraphRequest() { }

        /// <summary>Constructs a MatchGraphRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="graph_name">Name of the underlying geospatial graph
        /// resource to match to using <paramref
        /// cref="MatchGraphRequest.sample_points" />.  </param>
        /// <param name="sample_points">Sample points used to match to an
        /// underlying geospatial graph. Sample points must be specified using
        /// <a
        /// href="../../graph_solver/network_graph_solver.html#match-identifiers"
        /// target="_top">identifiers</a>; identifiers are grouped as <a
        /// href="../../graph_solver/network_graph_solver.html#match-combinations"
        /// target="_top">combinations</a>. Identifiers can be used with:
        /// existing column names, e.g., 'table.column AS SAMPLE_X';
        /// expressions, e.g., 'ST_MAKEPOINT(table.x, table.y) AS
        /// SAMPLE_WKTPOINT'; or raw values, e.g., '{1, 2, 10} AS
        /// SAMPLE_TRIPID'.  </param>
        /// <param name="solve_method">The type of solver to use for graph
        /// matching.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using the
        /// Hidden Markov Model (HMM)-based method, which conducts a range-tree
        /// closest-edge search to find the best combinations of possible road
        /// segments (<i>num_segments</i>) for each sample point to create the
        /// best route. The route is secured one point at a time while looking
        /// ahead <i>chain_width</i> number of points, so the prediction is
        /// corrected after each point. This solution type is the most accurate
        /// but also the most computationally intensive.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.INCREMENTAL_WEIGHTED">INCREMENTAL_WEIGHTED</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to the graph using time
        /// and/or distance between points to influence one or more shortest
        /// paths across the sample points.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.SolveMethod.MATCH_OD_PAIRS">MATCH_OD_PAIRS</see>:</term>
        ///         <description>Matches <paramref
        /// cref="MatchGraphRequest.sample_points" /> to find the most probable
        /// path between origin and destination pairs with cost
        /// constraints</description>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.SolveMethod.MARKOV_CHAIN">MARKOV_CHAIN</see>.
        /// </param>
        /// <param name="solution_table">The name of the table used to store
        /// the results; this table contains a <a
        /// href="../../geospatial/geo_objects.html#geospatial-tracks"
        /// target="_top">track</a> of geospatial points for the matched
        /// portion of the graph, a track ID, and a score value. Also outputs a
        /// details table containing a trip ID (that matches the track ID), the
        /// latitude/longitude pair, the timestamp the point was recorded at,
        /// and an edge ID corresponding to the matched road segment. Has the
        /// same naming restrictions as <a href="../../concepts/tables.html"
        /// target="_top">tables</a>. Must not be an existing table of the same
        /// name.  The default value is ''.</param>
        /// <param name="options">Additional parameters
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.GPS_NOISE">GPS_NOISE</see>:</term>
        ///         <description>GPS noise value (in meters) to remove
        /// redundant sample points. Use -1 to disable noise reduction. The
        /// default value accounts for 95% of point variation (+ or -5 meters).
        /// The default value is '5.0'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.NUM_SEGMENTS">NUM_SEGMENTS</see>:</term>
        ///         <description>Maximum number of potentially matching road
        /// segments for each sample point. For the <i>markov_chain</i> solver,
        /// the default is 3; for the <i>incremental_weighted</i>, the default
        /// is 5.  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SEARCH_RADIUS">SEARCH_RADIUS</see>:</term>
        ///         <description>Maximum search radius used when snapping
        /// sample points onto potentially matching surrounding segments. The
        /// default value corresponds to approximately 100 meters.  The default
        /// value is '0.001'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.CHAIN_WIDTH">CHAIN_WIDTH</see>:</term>
        ///         <description>For the <i>markov_chain</i> solver only.
        /// Length of the sample points lookahead window within the Markov
        /// kernel; the larger the number, the more accurate the solution.  The
        /// default value is '9'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.MAX_SOLVE_LENGTH">MAX_SOLVE_LENGTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Maximum number of samples along the path on which to solve.
        /// The default value is '200'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TIME_WINDOW_WIDTH">TIME_WINDOW_WIDTH</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. Time window, also known as sampling period, in which points
        /// are favored. To determine the raw window value, the
        /// <i>time_window_width</i> value is multiplied by the mean sample
        /// time (in seconds) across all points, e.g., if
        /// <i>time_window_width</i> is 30 and the mean sample time is 2
        /// seconds, points that are sampled greater than 60 seconds after the
        /// previous point are no longer favored in the solution.  The default
        /// value is '30'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DETECT_LOOPS">DETECT_LOOPS</see>:</term>
        ///         <description>For the <i>incremental_weighted</i> solver
        /// only. If <i>true</i>, a loop will be detected and traversed even if
        /// it would make a shorter path to ignore the loop.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="MatchGraphRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.SOURCE">SOURCE</see>:</term>
        ///         <description>Optional WKT starting point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// starting point.  The default value is 'POINT NULL'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="MatchGraphRequest.Options.DESTINATION">DESTINATION</see>:</term>
        ///         <description>Optional WKT ending point from <paramref
        /// cref="MatchGraphRequest.sample_points" /> for the solver. The
        /// default behavior for the endpoint is to use time to determine the
        /// destination point.  The default value is 'POINT
        /// NULL'.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public MatchGraphRequest( string graph_name,
                                  IList<string> sample_points,
                                  string solve_method = null,
                                  string solution_table = null,
                                  IDictionary<string, string> options = null)
        {
            this.graph_name = graph_name ?? "";
            this.sample_points = sample_points ?? new List<string>();
            this.solve_method = solve_method ?? SolveMethod.MARKOV_CHAIN;
            this.solution_table = solution_table ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class MatchGraphRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.matchGraph(string,IList{string},string,string,IDictionary{string, string})"
    /// />.</summary>
    public class MatchGraphResponse : KineticaData
    {

        /// <summary>Indicates a successful solution.  </summary>
        public bool result { get; set; }

        /// <summary>The mean square error calculation representing the map
        /// matching score. Values closer to zero are better.  </summary>
        public float match_score { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class MatchGraphResponse




}  // end namespace kinetica
