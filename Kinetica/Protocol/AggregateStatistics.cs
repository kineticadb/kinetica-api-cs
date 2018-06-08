/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateStatistics(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates the requested statistics of the given column(s) in a given
    /// table.
    /// <br />
    /// The available statistics are <i>count</i> (number of total objects),
    /// <i>mean</i>, <i>stdv</i> (standard deviation), <i>variance</i>,
    /// <i>skew</i>, <i>kurtosis</i>, <i>sum</i>, <i>min</i>, <i>max</i>,
    /// <i>weighted_average</i>, <i>cardinality</i> (unique count),
    /// <i>estimated_cardinality</i>, <i>percentile</i> and
    /// <i>percentile_rank</i>.
    /// <br />
    /// Estimated cardinality is calculated by using the hyperloglog
    /// approximation technique.
    /// <br />
    /// Percentiles and percentile ranks are approximate and are calculated
    /// using the t-digest algorithm. They must include the desired
    /// <i>percentile</i>/<i>percentile_rank</i>. To compute multiple
    /// percentiles each value must be specified separately (i.e.
    /// 'percentile(75.0),percentile(99.0),percentile_rank(1234.56),percentile_rank(-5)').
    /// <br />
    /// A second, comma-separated value can be added to the <i>percentile</i>
    /// statistic to calculate percentile resolution, e.g., a 50th percentile
    /// with 200 resolution would be 'percentile(50,200)'.
    /// <br />
    /// The weighted average statistic requires a <i>weight_column_name</i> to
    /// be specified in <see cref="options" />. The weighted average is then
    /// defined as the sum of the products of <see cref="column_name" /> times
    /// the <i>weight_column_name</i> values divided by the sum of the
    /// <i>weight_column_name</i> values.
    /// <br />
    /// Additional columns can be used in the calculation of statistics via the
    /// <i>additional_column_names</i> option.  Values in these columns will be
    /// included in the overall aggregate calculation--individual aggregates
    /// will not be calculated per additional column.  For instance, requesting
    /// the <i>count</i> & <i>mean</i> of <see cref="column_name" /> x and
    /// <i>additional_column_names</i> y & z, where x holds the numbers 1-10, y
    /// holds 11-20, and z holds 21-30, would return the total number of x, y,
    /// & z values (30), and the single average value across all x, y, & z
    /// values (15.5).
    /// <br />
    /// The response includes a list of key/value pairs of each statistic
    /// requested and its corresponding value.</summary>
    public class AggregateStatisticsRequest : KineticaData
    {

        /// <summary>Comma separated list of the statistics to calculate, e.g.
        /// "sum,mean".
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.COUNT">COUNT</see>:</term>
        ///         <description>Number of objects (independent of the given
        /// column(s)).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MEAN">MEAN</see>:</term>
        ///         <description>Arithmetic mean (average), equivalent to
        /// sum/count.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.STDV">STDV</see>:</term>
        ///         <description>Sample standard deviation (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.VARIANCE">VARIANCE</see>:</term>
        ///         <description>Unbiased sample variance (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SKEW">SKEW</see>:</term>
        ///         <description>Skewness (third standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.KURTOSIS">KURTOSIS</see>:</term>
        ///         <description>Kurtosis (fourth standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SUM">SUM</see>:</term>
        ///         <description>Sum of all values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MIN">MIN</see>:</term>
        ///         <description>Minimum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MAX">MAX</see>:</term>
        ///         <description>Maximum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.WEIGHTED_AVERAGE">WEIGHTED_AVERAGE</see>:</term>
        ///         <description>Weighted arithmetic mean (using the option
        /// <i>weight_column_name</i> as the weighting column).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.CARDINALITY">CARDINALITY</see>:</term>
        ///         <description>Number of unique values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.ESTIMATED_CARDINALITY">ESTIMATED_CARDINALITY</see>:</term>
        ///         <description>Estimate (via hyperloglog technique) of the
        /// number of unique values in the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE">PERCENTILE</see>:</term>
        ///         <description>Estimate (via t-digest) of the given
        /// percentile of the column(s) (percentile(50.0) will be an
        /// approximation of the median). Add a second, comma-separated value
        /// to calculate percentile resolution, e.g.,
        /// 'percentile(75,150)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE_RANK">PERCENTILE_RANK</see>:</term>
        ///         <description>Estimate (via t-digest) of the percentile rank
        /// of the given value in the column(s) (if the given value is the
        /// median of the column(s), percentile_rank(<median>) will return
        /// approximately 50.0).</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="stats"
        /// />.</summary>
        public struct Stats
        {

            /// <summary>Number of objects (independent of the given
            /// column(s)).</summary>
            public const string COUNT = "count";

            /// <summary>Arithmetic mean (average), equivalent to
            /// sum/count.</summary>
            public const string MEAN = "mean";

            /// <summary>Sample standard deviation (denominator is
            /// count-1).</summary>
            public const string STDV = "stdv";

            /// <summary>Unbiased sample variance (denominator is
            /// count-1).</summary>
            public const string VARIANCE = "variance";

            /// <summary>Skewness (third standardized moment).</summary>
            public const string SKEW = "skew";

            /// <summary>Kurtosis (fourth standardized moment).</summary>
            public const string KURTOSIS = "kurtosis";

            /// <summary>Sum of all values in the column(s).</summary>
            public const string SUM = "sum";

            /// <summary>Minimum value of the column(s).</summary>
            public const string MIN = "min";

            /// <summary>Maximum value of the column(s).</summary>
            public const string MAX = "max";

            /// <summary>Weighted arithmetic mean (using the option
            /// <i>weight_column_name</i> as the weighting column).</summary>
            public const string WEIGHTED_AVERAGE = "weighted_average";

            /// <summary>Number of unique values in the column(s).</summary>
            public const string CARDINALITY = "cardinality";

            /// <summary>Estimate (via hyperloglog technique) of the number of
            /// unique values in the column(s).</summary>
            public const string ESTIMATED_CARDINALITY = "estimated_cardinality";

            /// <summary>Estimate (via t-digest) of the given percentile of the
            /// column(s) (percentile(50.0) will be an approximation of the
            /// median). Add a second, comma-separated value to calculate
            /// percentile resolution, e.g., 'percentile(75,150)'</summary>
            public const string PERCENTILE = "percentile";

            /// <summary>Estimate (via t-digest) of the percentile rank of the
            /// given value in the column(s) (if the given value is the median
            /// of the column(s), percentile_rank(<median>) will return
            /// approximately 50.0).</summary>
            public const string PERCENTILE_RANK = "percentile_rank";
        } // end struct Stats


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.ADDITIONAL_COLUMN_NAMES">ADDITIONAL_COLUMN_NAMES</see>:</term>
        ///         <description>A list of comma separated column names over
        /// which statistics can be accumulated along with the primary column.
        /// All columns listed and <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> must be of the
        /// same type.  Must not include the column specified in <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> and no column can
        /// be listed twice.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.WEIGHT_COLUMN_NAME">WEIGHT_COLUMN_NAME</see>:</term>
        ///         <description>Name of column used as weighting attribute for
        /// the weighted average statistic.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>A list of comma separated column names over which
            /// statistics can be accumulated along with the primary column.
            /// All columns listed and <see cref="column_name" /> must be of
            /// the same type.  Must not include the column specified in <see
            /// cref="column_name" /> and no column can be listed
            /// twice.</summary>
            public const string ADDITIONAL_COLUMN_NAMES = "additional_column_names";

            /// <summary>Name of column used as weighting attribute for the
            /// weighted average statistic.</summary>
            public const string WEIGHT_COLUMN_NAME = "weight_column_name";
        } // end struct Options


        /// <summary>Name of the table on which the statistics operation will
        /// be performed.  </summary>
        public string table_name { get; set; }

        /// <summary>Name of the primary column for which the statistics are to
        /// be calculated.  </summary>
        public string column_name { get; set; }

        /// <summary>Comma separated list of the statistics to calculate, e.g.
        /// "sum,mean".
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.COUNT">COUNT</see>:</term>
        ///         <description>Number of objects (independent of the given
        /// column(s)).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MEAN">MEAN</see>:</term>
        ///         <description>Arithmetic mean (average), equivalent to
        /// sum/count.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.STDV">STDV</see>:</term>
        ///         <description>Sample standard deviation (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.VARIANCE">VARIANCE</see>:</term>
        ///         <description>Unbiased sample variance (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SKEW">SKEW</see>:</term>
        ///         <description>Skewness (third standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.KURTOSIS">KURTOSIS</see>:</term>
        ///         <description>Kurtosis (fourth standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SUM">SUM</see>:</term>
        ///         <description>Sum of all values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MIN">MIN</see>:</term>
        ///         <description>Minimum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MAX">MAX</see>:</term>
        ///         <description>Maximum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.WEIGHTED_AVERAGE">WEIGHTED_AVERAGE</see>:</term>
        ///         <description>Weighted arithmetic mean (using the option
        /// <i>weight_column_name</i> as the weighting column).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.CARDINALITY">CARDINALITY</see>:</term>
        ///         <description>Number of unique values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.ESTIMATED_CARDINALITY">ESTIMATED_CARDINALITY</see>:</term>
        ///         <description>Estimate (via hyperloglog technique) of the
        /// number of unique values in the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE">PERCENTILE</see>:</term>
        ///         <description>Estimate (via t-digest) of the given
        /// percentile of the column(s) (percentile(50.0) will be an
        /// approximation of the median). Add a second, comma-separated value
        /// to calculate percentile resolution, e.g.,
        /// 'percentile(75,150)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE_RANK">PERCENTILE_RANK</see>:</term>
        ///         <description>Estimate (via t-digest) of the percentile rank
        /// of the given value in the column(s) (if the given value is the
        /// median of the column(s), percentile_rank(<median>) will return
        /// approximately 50.0).</description>
        ///     </item>
        /// </list>  </summary>
        public string stats { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.ADDITIONAL_COLUMN_NAMES">ADDITIONAL_COLUMN_NAMES</see>:</term>
        ///         <description>A list of comma separated column names over
        /// which statistics can be accumulated along with the primary column.
        /// All columns listed and <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> must be of the
        /// same type.  Must not include the column specified in <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> and no column can
        /// be listed twice.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.WEIGHT_COLUMN_NAME">WEIGHT_COLUMN_NAME</see>:</term>
        ///         <description>Name of column used as weighting attribute for
        /// the weighted average statistic.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AggregateStatisticsRequest object with
        /// default parameters.</summary>
        public AggregateStatisticsRequest() { }

        /// <summary>Constructs an AggregateStatisticsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the statistics
        /// operation will be performed.  </param>
        /// <param name="column_name">Name of the primary column for which the
        /// statistics are to be calculated.  </param>
        /// <param name="stats">Comma separated list of the statistics to
        /// calculate, e.g. "sum,mean".
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.COUNT">COUNT</see>:</term>
        ///         <description>Number of objects (independent of the given
        /// column(s)).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MEAN">MEAN</see>:</term>
        ///         <description>Arithmetic mean (average), equivalent to
        /// sum/count.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.STDV">STDV</see>:</term>
        ///         <description>Sample standard deviation (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.VARIANCE">VARIANCE</see>:</term>
        ///         <description>Unbiased sample variance (denominator is
        /// count-1).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SKEW">SKEW</see>:</term>
        ///         <description>Skewness (third standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.KURTOSIS">KURTOSIS</see>:</term>
        ///         <description>Kurtosis (fourth standardized
        /// moment).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.SUM">SUM</see>:</term>
        ///         <description>Sum of all values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MIN">MIN</see>:</term>
        ///         <description>Minimum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.MAX">MAX</see>:</term>
        ///         <description>Maximum value of the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.WEIGHTED_AVERAGE">WEIGHTED_AVERAGE</see>:</term>
        ///         <description>Weighted arithmetic mean (using the option
        /// <i>weight_column_name</i> as the weighting column).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.CARDINALITY">CARDINALITY</see>:</term>
        ///         <description>Number of unique values in the
        /// column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.ESTIMATED_CARDINALITY">ESTIMATED_CARDINALITY</see>:</term>
        ///         <description>Estimate (via hyperloglog technique) of the
        /// number of unique values in the column(s).</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE">PERCENTILE</see>:</term>
        ///         <description>Estimate (via t-digest) of the given
        /// percentile of the column(s) (percentile(50.0) will be an
        /// approximation of the median). Add a second, comma-separated value
        /// to calculate percentile resolution, e.g.,
        /// 'percentile(75,150)'</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Stats.PERCENTILE_RANK">PERCENTILE_RANK</see>:</term>
        ///         <description>Estimate (via t-digest) of the percentile rank
        /// of the given value in the column(s) (if the given value is the
        /// median of the column(s), percentile_rank(<median>) will return
        /// approximately 50.0).</description>
        ///     </item>
        /// </list>  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.ADDITIONAL_COLUMN_NAMES">ADDITIONAL_COLUMN_NAMES</see>:</term>
        ///         <description>A list of comma separated column names over
        /// which statistics can be accumulated along with the primary column.
        /// All columns listed and <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> must be of the
        /// same type.  Must not include the column specified in <paramref
        /// cref="AggregateStatisticsRequest.column_name" /> and no column can
        /// be listed twice.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AggregateStatisticsRequest.Options.WEIGHT_COLUMN_NAME">WEIGHT_COLUMN_NAME</see>:</term>
        ///         <description>Name of column used as weighting attribute for
        /// the weighted average statistic.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public AggregateStatisticsRequest( string table_name,
                                           string column_name,
                                           string stats,
                                           IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_name = column_name ?? "";
            this.stats = stats ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AggregateStatisticsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateStatistics(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AggregateStatisticsResponse : KineticaData
    {

        /// <summary>(statistic name, double value) pairs of the requested
        /// statistics, including the total count by default.  </summary>
        public IDictionary<string, double> stats { get; set; } = new Dictionary<string, double>();

    } // end class AggregateStatisticsResponse




}  // end namespace kinetica
