/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.dropDatasource(DropDatasourceRequest)">Kinetica.dropDatasource</see>.
    /// </summary>
    /// <remarks><para>Drops an existing <a
    /// href="../../../concepts/data_sources/" target="_top">data source</a>.
    /// Any external tables that depend on the data source must be dropped
    /// before it can be dropped.</para></remarks>
    public class DropDatasourceRequest : KineticaData
    {
        /// <summary>Name of the data source to be dropped.</summary>
        /// <remarks><para>Must be an existing data source.</para></remarks>
        public string name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a DropDatasourceRequest object with default
        /// parameters.</summary>
        public DropDatasourceRequest() { }

        /// <summary>Constructs a DropDatasourceRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="name">Name of the data source to be dropped. Must be
        /// an existing data source.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public DropDatasourceRequest( string name,
                                      IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class DropDatasourceRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.dropDatasource(DropDatasourceRequest)">Kinetica.dropDatasource</see>.
    /// </summary>
    public class DropDatasourceResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="DropDatasourceRequest.name">name</see>.</summary>
        public string name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class DropDatasourceResponse
} // end namespace kinetica
