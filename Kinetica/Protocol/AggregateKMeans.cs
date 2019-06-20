/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateKMeans(string,IList{string},int,double,IDictionary{string, string})"
    /// />.
    /// <br />
    /// This endpoint runs the k-means algorithm - a heuristic algorithm that
    /// attempts to do k-means clustering.  An ideal k-means clustering
    /// algorithm selects k points such that the sum of the mean squared
    /// distances of each member of the set to the nearest of the k points is
    /// minimized.  The k-means algorithm however does not necessarily produce
    /// such an ideal cluster.   It begins with a randomly selected set of k
    /// points and then refines the location of the points iteratively and
    /// settles to a local minimum.  Various parameters and options are
    /// provided to control the heuristic search.
    /// <br />
    /// NOTE:  The Kinetica instance being accessed must be running a CUDA
    /// (GPU-based) build to service this request.</summary>
    public class AggregateKMeansRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.WHITEN">WHITEN</see>:</term>
        ///         <description>When set to 1 each of the columns is first
        /// normalized by its stdv - default is not to whiten.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.MAX_ITERS">MAX_ITERS</see>:</term>
        ///         <description>Number of times to try to hit the tolerance
        /// limit before giving up - default is 10.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.NUM_TRIES">NUM_TRIES</see>:</term>
        ///         <description>Number of times to run the k-means algorithm
        /// with a different randomly selected starting points - helps avoid
        /// local minimum. Default is 1.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>When set to 1 each of the columns is first normalized
            /// by its stdv - default is not to whiten.</summary>
            public const string WHITEN = "whiten";

            /// <summary>Number of times to try to hit the tolerance limit
            /// before giving up - default is 10.</summary>
            public const string MAX_ITERS = "max_iters";

            /// <summary>Number of times to run the k-means algorithm with a
            /// different randomly selected starting points - helps avoid local
            /// minimum. Default is 1.</summary>
            public const string NUM_TRIES = "num_tries";
        } // end struct Options


        /// <summary>Name of the table on which the operation will be
        /// performed. Must be an existing table or collection.  </summary>
        public string table_name { get; set; }

        /// <summary>List of column names on which the operation would be
        /// performed. If n columns are provided then each of the k result
        /// points will have n dimensions corresponding to the n columns.
        /// </summary>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>The number of mean points to be determined by the
        /// algorithm.  </summary>
        public int k { get; set; }

        /// <summary>Stop iterating when the distances between successive
        /// points is less than the given tolerance.  </summary>
        public double tolerance { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.WHITEN">WHITEN</see>:</term>
        ///         <description>When set to 1 each of the columns is first
        /// normalized by its stdv - default is not to whiten.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.MAX_ITERS">MAX_ITERS</see>:</term>
        ///         <description>Number of times to try to hit the tolerance
        /// limit before giving up - default is 10.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.NUM_TRIES">NUM_TRIES</see>:</term>
        ///         <description>Number of times to run the k-means algorithm
        /// with a different randomly selected starting points - helps avoid
        /// local minimum. Default is 1.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AggregateKMeansRequest object with default
        /// parameters.</summary>
        public AggregateKMeansRequest() { }

        /// <summary>Constructs an AggregateKMeansRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table or collection.
        /// </param>
        /// <param name="column_names">List of column names on which the
        /// operation would be performed. If n columns are provided then each
        /// of the k result points will have n dimensions corresponding to the
        /// n columns.  </param>
        /// <param name="k">The number of mean points to be determined by the
        /// algorithm.  </param>
        /// <param name="tolerance">Stop iterating when the distances between
        /// successive points is less than the given tolerance.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.WHITEN">WHITEN</see>:</term>
        ///         <description>When set to 1 each of the columns is first
        /// normalized by its stdv - default is not to whiten.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.MAX_ITERS">MAX_ITERS</see>:</term>
        ///         <description>Number of times to try to hit the tolerance
        /// limit before giving up - default is 10.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateKMeansRequest.Options.NUM_TRIES">NUM_TRIES</see>:</term>
        ///         <description>Number of times to run the k-means algorithm
        /// with a different randomly selected starting points - helps avoid
        /// local minimum. Default is 1.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public AggregateKMeansRequest( string table_name,
                                       IList<string> column_names,
                                       int k,
                                       double tolerance,
                                       IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_names = column_names ?? new List<string>();
            this.k = k;
            this.tolerance = tolerance;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AggregateKMeansRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateKMeans(string,IList{string},int,double,IDictionary{string, string})"
    /// />.</summary>
    public class AggregateKMeansResponse : KineticaData
    {

        /// <summary>The k-mean values found.  </summary>
        public IList<IList<double>> means { get; set; } = new List<IList<double>>();

        /// <summary>The number of elements in the cluster closest the
        /// corresponding k-means values.  </summary>
        public IList<long> counts { get; set; } = new List<long>();

        /// <summary>The root mean squared distance of the elements in the
        /// cluster for each of the k-means values.  </summary>
        public IList<double> rms_dists { get; set; } = new List<double>();

        /// <summary>The total count of all the clusters - will be the size of
        /// the input table.  </summary>
        public long count { get; set; }

        /// <summary>The sum of all the rms_dists - the value the k-means
        /// algorithm is attempting to minimize.  </summary>
        public double rms_dist { get; set; }

        /// <summary>The distance between the last two iterations of the
        /// algorithm before it quit.  </summary>
        public double tolerance { get; set; }

        /// <summary>The number of iterations the algorithm executed before it
        /// quit.  </summary>
        public int num_iters { get; set; }

    } // end class AggregateKMeansResponse




}  // end namespace kinetica
