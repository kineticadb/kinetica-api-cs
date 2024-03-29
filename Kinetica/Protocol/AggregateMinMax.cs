/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateMinMax(AggregateMinMaxRequest)">Kinetica.aggregateMinMax</see>.
    /// </summary>
    /// <remarks><para>Calculates and returns the minimum and maximum values of
    /// a particular column in a table.</para></remarks>
    public class AggregateMinMaxRequest : KineticaData
    {
        /// <summary>Name of the table on which the operation will be
        /// performed.</summary>
        /// <remarks><para>Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</para></remarks>
        public string table_name { get; set; }

        /// <summary>Name of a column or an expression of one or more column on
        /// which the min-max will be calculated.</summary>
        public string column_name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AggregateMinMaxRequest object with default
        /// parameters.</summary>
        public AggregateMinMaxRequest() { }

        /// <summary>Constructs an AggregateMinMaxRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.</param>
        /// <param name="column_name">Name of a column or an expression of one
        /// or more column on which the min-max will be calculated.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public AggregateMinMaxRequest( string table_name,
                                       string column_name,
                                       IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_name = column_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AggregateMinMaxRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateMinMax(AggregateMinMaxRequest)">Kinetica.aggregateMinMax</see>.
    /// </summary>
    public class AggregateMinMaxResponse : KineticaData
    {
        /// <summary>Minimum value of the <see
        /// cref="AggregateMinMaxRequest.column_name">column_name</see>.
        /// </summary>
        public double min { get; set; }

        /// <summary>Maximum value of the <see
        /// cref="AggregateMinMaxRequest.column_name">column_name</see>.
        /// </summary>
        public double max { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AggregateMinMaxResponse
} // end namespace kinetica
