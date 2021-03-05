/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.aggregateMinMaxGeometry(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Calculates and returns the minimum and maximum x- and y-coordinates
    /// of a particular geospatial geometry column in a table.</summary>
    public class AggregateMinMaxGeometryRequest : KineticaData
    {

        /// <summary>Name of the table on which the operation will be
        /// performed. Must be an existing table, in [schema_name.]table_name
        /// format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  </summary>
        public string table_name { get; set; }

        /// <summary>Name of a geospatial geometry column on which the min-max
        /// will be calculated.  </summary>
        public string column_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AggregateMinMaxGeometryRequest object with
        /// default parameters.</summary>
        public AggregateMinMaxGeometryRequest() { }

        /// <summary>Constructs an AggregateMinMaxGeometryRequest object with
        /// the specified parameters.</summary>
        /// 
        /// <param name="table_name">Name of the table on which the operation
        /// will be performed. Must be an existing table, in
        /// [schema_name.]table_name format, using standard <a
        /// href="../../../concepts/tables/#table-name-resolution"
        /// target="_top">name resolution rules</a>.  </param>
        /// <param name="column_name">Name of a geospatial geometry column on
        /// which the min-max will be calculated.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public AggregateMinMaxGeometryRequest( string table_name,
                                               string column_name,
                                               IDictionary<string, string> options = null)
        {
            this.table_name = table_name ?? "";
            this.column_name = column_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AggregateMinMaxGeometryRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.aggregateMinMaxGeometry(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class AggregateMinMaxGeometryResponse : KineticaData
    {

        /// <summary>Minimum x-coordinate value of the <paramref
        /// cref="AggregateMinMaxGeometryRequest.column_name" />.  </summary>
        public double min_x { get; set; }

        /// <summary>Maximum x-coordinate value of the <paramref
        /// cref="AggregateMinMaxGeometryRequest.column_name" />.  </summary>
        public double max_x { get; set; }

        /// <summary>Minimum y-coordinate value of the <paramref
        /// cref="AggregateMinMaxGeometryRequest.column_name" />.  </summary>
        public double min_y { get; set; }

        /// <summary>Maximum y-coordinate value of the <paramref
        /// cref="AggregateMinMaxGeometryRequest.column_name" />.  </summary>
        public double max_y { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AggregateMinMaxGeometryResponse




}  // end namespace kinetica
