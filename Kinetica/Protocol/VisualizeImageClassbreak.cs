/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// @cond NO_DOCS
    public class VisualizeImageClassbreakRequest : KineticaData
    {
        public struct Projection
        {
            public const string EPSG_4326 = "EPSG:4326";
            public const string PLATE_CARREE = "PLATE_CARREE";
            public const string _900913 = "900913";
            public const string EPSG_900913 = "EPSG:900913";
            public const string _102100 = "102100";
            public const string EPSG_102100 = "EPSG:102100";
            public const string _3857 = "3857";
            public const string EPSG_3857 = "EPSG:3857";
            public const string WEB_MERCATOR = "WEB_MERCATOR";
        } // end struct Projection

        public struct StyleOptions
        {
            public const string DO_POINTS = "do_points";
            public const string TRUE = "true";
            public const string FALSE = "false";
            public const string DO_SHAPES = "do_shapes";
            public const string DO_TRACKS = "do_tracks";
            public const string DO_SYMBOLOGY = "do_symbology";
            public const string POINTCOLORS = "pointcolors";
            public const string CB_POINTALPHAS = "cb_pointalphas";
            public const string POINTSIZES = "pointsizes";
            public const string POINTOFFSET_X = "pointoffset_x";
            public const string POINTOFFSET_Y = "pointoffset_y";
            public const string POINTSHAPES = "pointshapes";
            public const string NONE = "none";
            public const string CIRCLE = "circle";
            public const string SQUARE = "square";
            public const string DIAMOND = "diamond";
            public const string HOLLOWCIRCLE = "hollowcircle";
            public const string HOLLOWSQUARE = "hollowsquare";
            public const string HOLLOWDIAMOND = "hollowdiamond";
            public const string SYMBOLCODE = "symbolcode";
            public const string DASH = "dash";
            public const string PIPE = "pipe";
            public const string PLUS = "plus";
            public const string HOLLOWSQUAREWITHPLUS = "hollowsquarewithplus";
            public const string DOT = "dot";
            public const string SYMBOLROTATIONS = "symbolrotations";
            public const string SHAPELINEWIDTHS = "shapelinewidths";
            public const string SHAPELINECOLORS = "shapelinecolors";
            public const string SHAPELINEPATTERNS = "shapelinepatterns";
            public const string SHAPELINEPATTERNLEN = "shapelinepatternlen";
            public const string SHAPEFILLCOLORS = "shapefillcolors";
            public const string HASHLINEINTERVALS = "hashlineintervals";
            public const string HASHLINECOLORS = "hashlinecolors";
            public const string HASHLINEANGLES = "hashlineangles";
            public const string HASHLINELENS = "hashlinelens";
            public const string HASHLINEWIDTHS = "hashlinewidths";
            public const string TRACKLINEWIDTHS = "tracklinewidths";
            public const string TRACKLINECOLORS = "tracklinecolors";
            public const string TRACKMARKERSIZES = "trackmarkersizes";
            public const string TRACKMARKERCOLORS = "trackmarkercolors";
            public const string TRACKMARKERSHAPES = "trackmarkershapes";
            public const string ORIENTED_ARROW = "oriented_arrow";
            public const string ORIENTED_TRIANGLE = "oriented_triangle";
            public const string TRACKHEADCOLORS = "trackheadcolors";
            public const string TRACKHEADSIZES = "trackheadsizes";
            public const string TRACKHEADSHAPES = "trackheadshapes";
        } // end struct StyleOptions

        public struct Options
        {
            public const string TRACK_ID_COLUMN_NAME = "track_id_column_name";
            public const string TRACK_ORDER_COLUMN_NAME = "track_order_column_name";
        } // end struct Options

        public IList<string> table_names { get; set; } = new List<string>();
        public IList<string> world_table_names { get; set; } = new List<string>();
        public string x_column_name { get; set; }
        public string y_column_name { get; set; }
        public string symbol_column_name { get; set; }
        public string geometry_column_name { get; set; }
        public IList<IList<string>> track_ids { get; set; } = new List<IList<string>>();
        public string cb_attr { get; set; }
        public IList<string> cb_vals { get; set; } = new List<string>();
        public string cb_pointcolor_attr { get; set; }
        public IList<string> cb_pointcolor_vals { get; set; } = new List<string>();
        public string cb_pointalpha_attr { get; set; }
        public IList<string> cb_pointalpha_vals { get; set; } = new List<string>();
        public string cb_pointsize_attr { get; set; }
        public IList<string> cb_pointsize_vals { get; set; } = new List<string>();
        public string cb_pointshape_attr { get; set; }
        public IList<string> cb_pointshape_vals { get; set; } = new List<string>();
        public double min_x { get; set; }
        public double max_x { get; set; }
        public double min_y { get; set; }
        public double max_y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string projection { get; set; } = Projection.PLATE_CARREE;
        public long bg_color { get; set; }
        public IDictionary<string, IList<string>> style_options { get; set; } = new Dictionary<string, IList<string>>();
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();
        public IList<int> cb_transparency_vec { get; set; } = new List<int>();

        public VisualizeImageClassbreakRequest() { }

        public VisualizeImageClassbreakRequest( IList<string> table_names,
                                                IList<string> world_table_names,
                                                string x_column_name,
                                                string y_column_name,
                                                string symbol_column_name,
                                                string geometry_column_name,
                                                IList<IList<string>> track_ids,
                                                string cb_attr,
                                                IList<string> cb_vals,
                                                string cb_pointcolor_attr,
                                                IList<string> cb_pointcolor_vals,
                                                string cb_pointalpha_attr,
                                                IList<string> cb_pointalpha_vals,
                                                string cb_pointsize_attr,
                                                IList<string> cb_pointsize_vals,
                                                string cb_pointshape_attr,
                                                IList<string> cb_pointshape_vals,
                                                double min_x,
                                                double max_x,
                                                double min_y,
                                                double max_y,
                                                int width,
                                                int height,
                                                string projection,
                                                long bg_color,
                                                IDictionary<string, IList<string>> style_options,
                                                IDictionary<string, string> options,
                                                IList<int> cb_transparency_vec)
        {
            this.table_names = table_names ?? new List<string>();
            this.world_table_names = world_table_names ?? new List<string>();
            this.x_column_name = x_column_name ?? "";
            this.y_column_name = y_column_name ?? "";
            this.symbol_column_name = symbol_column_name ?? "";
            this.geometry_column_name = geometry_column_name ?? "";
            this.track_ids = track_ids ?? new List<IList<string>>();
            this.cb_attr = cb_attr ?? "";
            this.cb_vals = cb_vals ?? new List<string>();
            this.cb_pointcolor_attr = cb_pointcolor_attr ?? "";
            this.cb_pointcolor_vals = cb_pointcolor_vals ?? new List<string>();
            this.cb_pointalpha_attr = cb_pointalpha_attr ?? "";
            this.cb_pointalpha_vals = cb_pointalpha_vals ?? new List<string>();
            this.cb_pointsize_attr = cb_pointsize_attr ?? "";
            this.cb_pointsize_vals = cb_pointsize_vals ?? new List<string>();
            this.cb_pointshape_attr = cb_pointshape_attr ?? "";
            this.cb_pointshape_vals = cb_pointshape_vals ?? new List<string>();
            this.min_x = min_x;
            this.max_x = max_x;
            this.min_y = min_y;
            this.max_y = max_y;
            this.width = width;
            this.height = height;
            this.projection = projection ?? Projection.PLATE_CARREE;
            this.bg_color = bg_color;
            this.style_options = style_options ?? new Dictionary<string, IList<string>>();
            this.options = options ?? new Dictionary<string, string>();
            this.cb_transparency_vec = cb_transparency_vec ?? new List<int>();
        } // end constructor
    } // end class VisualizeImageClassbreakRequest
    /// @endcond

    /// @cond NO_DOCS
    public class VisualizeImageClassbreakResponse : KineticaData
    {
        public double width { get; set; }
        public double height { get; set; }
        public long bg_color { get; set; }
        public byte[] image_data { get; set; }
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class VisualizeImageClassbreakResponse
    /// @endcond
} // end namespace kinetica
