/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateHistogram(AggregateHistogramRequest)">Kinetica.aggregateHistogram</see>.
    /// </summary>
    /// <remarks><para>Performs a histogram calculation given a table, a
    /// column, and an interval function. The <see cref="interval" /> is used
    /// to produce bins of that size and the result, computed over the records
    /// falling within each bin, is returned. For each bin, the start value is
    /// inclusive, but the end value is exclusive--except for the very last bin
    /// for which the end value is also inclusive.  The value returned for each
    /// bin is the number of records in it, except when a column name is
    /// provided as a <see cref="Options.VALUE_COLUMN">VALUE_COLUMN</see>.  In
    /// this latter case the sum of the values corresponding to the <see
    /// cref="Options.VALUE_COLUMN">VALUE_COLUMN</see> is used as the result
    /// instead.  The total number of bins requested cannot exceed
    /// 10,000.</para>
    /// <para>NOTE:  The Kinetica instance being accessed must be running a
    /// CUDA (GPU-based) build to service a request that specifies a <see
    /// cref="Options.VALUE_COLUMN">VALUE_COLUMN</see>.</para></remarks>
    public class AggregateHistogramRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>The name of the column to use when calculating the bin
            /// values (values are summed).</summary>
            /// <remarks><para> The column must be a numerical type (int,
            /// double, long, float).</para></remarks>
            public const string VALUE_COLUMN = "value_column";
        } // end struct Options

        /// <summary>Name of the table on which the operation will be
        /// performed.</summary>
        /// <remarks><para>Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</para></remarks>
        public string table_name { get; set; }

        /// <summary>Name of a column or an expression of one or more column
        /// names over which the histogram will be calculated.</summary>
        public string column_name { get; set; }

        /// <summary>Lower end value of the histogram interval, inclusive.
        /// </summary>
        public double start { get; set; }

        /// <summary>Upper end value of the histogram interval, inclusive.
        /// </summary>
        public double end { get; set; }

        /// <summary>The size of each bin within the start and end parameters.
        /// </summary>
        public double interval { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.VALUE_COLUMN">VALUE_COLUMN</see>:
        ///         </term>
        ///         <description>The name of the column to use when calculating
        ///         the bin values (values are summed).  The column must be a
        ///         numerical type (int, double, long, float).</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AggregateHistogramRequest object with
        /// default parameters.</summary>
        public AggregateHistogramRequest() { }

        /// <summary>Constructs an AggregateHistogramRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</param>
        /// <param name="column_name">Name of a column or an expression of one
        /// or more column names over which the histogram will be calculated.
        /// </param>
        /// <param name="start">Lower end value of the histogram interval,
        /// inclusive.</param>
        /// <param name="end">Upper end value of the histogram interval,
        /// inclusive.</param>
        /// <param name="interval">The size of each bin within the start and
        /// end parameters.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.VALUE_COLUMN">VALUE_COLUMN</see>:
        ///         </term>
        ///         <description>The name of the column to use when calculating
        ///         the bin values (values are summed).  The column must be a
        ///         numerical type (int, double, long, float).</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public AggregateHistogramRequest( string table_name,
                                          string column_name,
                                          double start,
                                          double end,
                                          double interval,
                                          IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_name = column_name ?? "";
            this.start = start;
            this.end = end;
            this.interval = interval;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AggregateHistogramRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateHistogram(AggregateHistogramRequest)">Kinetica.aggregateHistogram</see>.
    /// </summary>
    public class AggregateHistogramResponse : KineticaData
    {
        /// <summary>The array of calculated values that represents the
        /// histogram data points.</summary>
        public IList<double> counts { get; set; } = new List<double>();

        /// <summary>Value of <see
        /// cref="AggregateHistogramRequest.start">start</see>.</summary>
        public double start { get; set; }

        /// <summary>Value of <see
        /// cref="AggregateHistogramRequest.end">end</see>.</summary>
        public double end { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AggregateHistogramResponse
} // end namespace kinetica
