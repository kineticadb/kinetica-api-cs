/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// @cond NO_DOCS
    public class GetVectortileRequest : KineticaData
    {
        public IList<string> table_names { get; set; } = new List<string>();
        public IList<string> column_names { get; set; } = new List<string>();
        public IDictionary<string, IList<string>> layers { get; set; } = new Dictionary<string, IList<string>>();
        public int tile_x { get; set; }
        public int tile_y { get; set; }
        public int zoom { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        public GetVectortileRequest() { }

        public GetVectortileRequest( IList<string> table_names,
                                     IList<string> column_names,
                                     IDictionary<string, IList<string>> layers,
                                     int tile_x,
                                     int tile_y,
                                     int zoom,
                                     IDictionary<string, string> options = null)
        {
            this.table_names = table_names ?? new List<string>();
            this.column_names = column_names ?? new List<string>();
            this.layers = layers ?? new Dictionary<string, IList<string>>();
            this.tile_x = tile_x;
            this.tile_y = tile_y;
            this.zoom = zoom;
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class GetVectortileRequest
    /// @endcond

    /// @cond NO_DOCS
    public class GetVectortileResponse : KineticaData
    {
        public string encoded_data { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class GetVectortileResponse
    /// @endcond
} // end namespace kinetica
