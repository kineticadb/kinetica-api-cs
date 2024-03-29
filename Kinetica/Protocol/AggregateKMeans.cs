/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateKMeans(AggregateKMeansRequest)">Kinetica.aggregateKMeans</see>.
    /// </summary>
    /// <remarks><para>This endpoint runs the k-means algorithm - a heuristic
    /// algorithm that attempts to do k-means clustering.  An ideal k-means
    /// clustering algorithm selects k points such that the sum of the mean
    /// squared distances of each member of the set to the nearest of the k
    /// points is minimized.  The k-means algorithm however does not
    /// necessarily produce such an ideal cluster.   It begins with a randomly
    /// selected set of k points and then refines the location of the points
    /// iteratively and settles to a local minimum.  Various parameters and
    /// options are provided to control the heuristic search.</para>
    /// <para>NOTE:  The Kinetica instance being accessed must be running a
    /// CUDA (GPU-based) build to service this request.</para></remarks>
    public class AggregateKMeansRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
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
            /// minimum.</summary>
            /// <remarks><para>Default is 1.</para></remarks>
            public const string NUM_TRIES = "num_tries";

            /// <summary>If <see cref="Options.TRUE">TRUE</see>, a unique
            /// temporary table name will be generated in the sys_temp schema
            /// and used in place of <see
            /// cref="Options.RESULT_TABLE">RESULT_TABLE</see>.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string CREATE_TEMP_TABLE = "create_temp_table";

            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>The name of a table used to store the results, in
            /// [schema_name.]table_name format, using standard <a
            /// href="../../../concepts/tables/#table-name-resolution"
            /// target="_top">name resolution rules</a> and meeting <a
            /// href="../../../concepts/tables/#table-naming-criteria"
            /// target="_top">table naming criteria</a>.</summary>
            /// <remarks><para> If this option is specified, the results are
            /// not returned in the response.</para></remarks>
            public const string RESULT_TABLE = "result_table";

            /// <summary>If <see cref="Options.TRUE">TRUE</see>, then the
            /// result table specified in <see
            /// cref="Options.RESULT_TABLE">RESULT_TABLE</see> will be
            /// persisted and will not expire unless a <see
            /// cref="Options.TTL">TTL</see> is specified.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string RESULT_TABLE_PERSIST = "result_table_persist";

            /// <summary>Sets the <a href="../../../concepts/ttl/"
            /// target="_top">TTL</a> of the table specified in <see
            /// cref="Options.RESULT_TABLE">RESULT_TABLE</see>.</summary>
            public const string TTL = "ttl";
        } // end struct Options

        /// <summary>Name of the table on which the operation will be
        /// performed.</summary>
        /// <remarks><para>Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</para></remarks>
        public string table_name { get; set; }

        /// <summary>List of column names on which the operation would be
        /// performed.</summary>
        /// <remarks><para>If n columns are provided then each of the k result
        /// points will have n dimensions corresponding to the n columns.
        /// </para></remarks>
        public IList<string> column_names { get; set; } = new List<string>();

        /// <summary>The number of mean points to be determined by the
        /// algorithm.</summary>
        public int k { get; set; }

        /// <summary>Stop iterating when the distances between successive
        /// points is less than the given tolerance.</summary>
        public double tolerance { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.WHITEN">WHITEN</see>:</term>
        ///         <description>When set to 1 each of the columns is first
        ///         normalized by its stdv - default is not to whiten.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.MAX_ITERS">MAX_ITERS</see>:</term>
        ///         <description>Number of times to try to hit the tolerance
        ///         limit before giving up - default is 10.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.NUM_TRIES">NUM_TRIES</see>:</term>
        ///         <description>Number of times to run the k-means algorithm
        ///         with a different randomly selected starting points - helps
        ///         avoid local minimum. Default is 1.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, a
        ///         unique temporary table name will be generated in the
        ///         sys_temp schema and used in place of <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see>. If <see
        ///         cref="Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>
        ///         is <see cref="Options.FALSE">FALSE</see> (or unspecified),
        ///         then this is always allowed even if the caller does not
        ///         have permission to create tables. The generated name is
        ///         returned in <see
        ///         cref="AggregateKMeansResponse.Info.QUALIFIED_RESULT_TABLE_NAME">QUALIFIED_RESULT_TABLE_NAME</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.RESULT_TABLE">RESULT_TABLE</see>:
        ///         </term>
        ///         <description>The name of a table used to store the results,
        ///         in [schema_name.]table_name format, using standard <a
        ///         href="../../../concepts/tables/#table-name-resolution"
        ///         target="_top">name resolution rules</a> and meeting <a
        ///         href="../../../concepts/tables/#table-naming-criteria"
        ///         target="_top">table naming criteria</a>.  If this option is
        ///         specified, the results are not returned in the response.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, then
        ///         the result table specified in <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see> will be
        ///         persisted and will not expire unless a <see
        ///         cref="Options.TTL">TTL</see> is specified.   If <see
        ///         cref="Options.FALSE">FALSE</see>, then the result table
        ///         will be an in-memory table and will expire unless a <see
        ///         cref="Options.TTL">TTL</see> is specified otherwise.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        ///         target="_top">TTL</a> of the table specified in <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AggregateKMeansRequest object with default
        /// parameters.</summary>
        public AggregateKMeansRequest() { }

        /// <summary>Constructs an AggregateKMeansRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</param>
        /// <param name="column_names">List of column names on which the
        /// operation would be performed. If n columns are provided then each
        /// of the k result points will have n dimensions corresponding to the
        /// n columns.</param>
        /// <param name="k">The number of mean points to be determined by the
        /// algorithm.</param>
        /// <param name="tolerance">Stop iterating when the distances between
        /// successive points is less than the given tolerance.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.WHITEN">WHITEN</see>:</term>
        ///         <description>When set to 1 each of the columns is first
        ///         normalized by its stdv - default is not to whiten.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.MAX_ITERS">MAX_ITERS</see>:</term>
        ///         <description>Number of times to try to hit the tolerance
        ///         limit before giving up - default is 10.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.NUM_TRIES">NUM_TRIES</see>:</term>
        ///         <description>Number of times to run the k-means algorithm
        ///         with a different randomly selected starting points - helps
        ///         avoid local minimum. Default is 1.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.CREATE_TEMP_TABLE">CREATE_TEMP_TABLE</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, a
        ///         unique temporary table name will be generated in the
        ///         sys_temp schema and used in place of <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see>. If <see
        ///         cref="Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>
        ///         is <see cref="Options.FALSE">FALSE</see> (or unspecified),
        ///         then this is always allowed even if the caller does not
        ///         have permission to create tables. The generated name is
        ///         returned in <see
        ///         cref="AggregateKMeansResponse.Info.QUALIFIED_RESULT_TABLE_NAME">QUALIFIED_RESULT_TABLE_NAME</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.RESULT_TABLE">RESULT_TABLE</see>:
        ///         </term>
        ///         <description>The name of a table used to store the results,
        ///         in [schema_name.]table_name format, using standard <a
        ///         href="../../../concepts/tables/#table-name-resolution"
        ///         target="_top">name resolution rules</a> and meeting <a
        ///         href="../../../concepts/tables/#table-naming-criteria"
        ///         target="_top">table naming criteria</a>.  If this option is
        ///         specified, the results are not returned in the response.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Options.RESULT_TABLE_PERSIST">RESULT_TABLE_PERSIST</see>:
        ///         </term>
        ///         <description>If <see cref="Options.TRUE">TRUE</see>, then
        ///         the result table specified in <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see> will be
        ///         persisted and will not expire unless a <see
        ///         cref="Options.TTL">TTL</see> is specified.   If <see
        ///         cref="Options.FALSE">FALSE</see>, then the result table
        ///         will be an in-memory table and will expire unless a <see
        ///         cref="Options.TTL">TTL</see> is specified otherwise.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Options.TTL">TTL</see>:</term>
        ///         <description>Sets the <a href="../../../concepts/ttl/"
        ///         target="_top">TTL</a> of the table specified in <see
        ///         cref="Options.RESULT_TABLE">RESULT_TABLE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
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
    /// cref="Kinetica.aggregateKMeans(AggregateKMeansRequest)">Kinetica.aggregateKMeans</see>.
    /// </summary>
    public class AggregateKMeansResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="info" />.</summary>
        /// <remarks><para>Additional information.</para></remarks>
        public struct Info
        {
            /// <summary>The fully qualified name of the result table (i.e.
            /// including the schema) used to store the results.</summary>
            public const string QUALIFIED_RESULT_TABLE_NAME = "qualified_result_table_name";
        } // end struct Info

        /// <summary>The k-mean values found.</summary>
        public IList<IList<double>> means { get; set; } = new List<IList<double>>();

        /// <summary>The number of elements in the cluster closest the
        /// corresponding k-means values.</summary>
        public IList<long> counts { get; set; } = new List<long>();

        /// <summary>The root mean squared distance of the elements in the
        /// cluster for each of the k-means values.</summary>
        public IList<double> rms_dists { get; set; } = new List<double>();

        /// <summary>The total count of all the clusters - will be the size of
        /// the input table.</summary>
        public long count { get; set; }

        /// <summary>The sum of all the rms_dists - the value the k-means
        /// algorithm is attempting to minimize.</summary>
        public double rms_dist { get; set; }

        /// <summary>The distance between the last two iterations of the
        /// algorithm before it quit.</summary>
        public double tolerance { get; set; }

        /// <summary>The number of iterations the algorithm executed before it
        /// quit.</summary>
        public int num_iters { get; set; }

        /// <summary>Additional information.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Info.QUALIFIED_RESULT_TABLE_NAME">QUALIFIED_RESULT_TABLE_NAME</see>:
        ///         </term>
        ///         <description>The fully qualified name of the result table
        ///         (i.e. including the schema) used to store the results.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AggregateKMeansResponse
} // end namespace kinetica
