/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.insertSymbol(string,string,byte[],IDictionary{string, string})"
    /// />.
    /// <br />
    /// Adds a symbol or icon (i.e. an image) to represent data points when
    /// data is rendered visually. Users must provide the symbol identifier
    /// (string), a format (currently supported: 'svg' and 'svg_path'), the
    /// data for the symbol, and any additional optional parameter (e.g.
    /// color). To have a symbol used for rendering create a table with a
    /// string column named 'SYMBOLCODE' (along with 'x' or 'y' for example).
    /// Then when the table is rendered (via <a href="../rest/wms_rest.html"
    /// target="_top">WMS</a>) if the 'dosymbology' parameter is 'true' then
    /// the value of the 'SYMBOLCODE' column is used to pick the symbol
    /// displayed for each point.</summary>
    public class InsertSymbolRequest : KineticaData
    {

        /// <summary>Specifies the symbol format. Must be either 'svg' or
        /// 'svg_path'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG">SVG</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG_PATH">SVG_PATH</see></term>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see
        /// cref="symbol_format" />.</summary>
        public struct SymbolFormat
        {
            public const string SVG = "svg";
            public const string SVG_PATH = "svg_path";
        } // end struct SymbolFormat


        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.Options.COLOR">COLOR</see>:</term>
        ///         <description>If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is 'svg' this is
        /// ignored. If <paramref cref="InsertSymbolRequest.symbol_format" />
        /// is 'svg_path' then this option specifies the color (in RRGGBB hex
        /// format) of the path. For example, to have the path rendered in red,
        /// used 'FF0000'. If 'color' is not provided then '00FF00' (i.e.
        /// green) is used by default.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <see cref="symbol_format" /> is 'svg' this is
            /// ignored. If <see cref="symbol_format" /> is 'svg_path' then
            /// this option specifies the color (in RRGGBB hex format) of the
            /// path. For example, to have the path rendered in red, used
            /// 'FF0000'. If 'color' is not provided then '00FF00' (i.e. green)
            /// is used by default.</summary>
            public const string COLOR = "color";
        } // end struct Options


        /// <summary>The id of the symbol being added. This is the same id that
        /// should be in the 'SYMBOLCODE' column for objects using this symbol
        /// </summary>
        public string symbol_id { get; set; }

        /// <summary>Specifies the symbol format. Must be either 'svg' or
        /// 'svg_path'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG">SVG</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG_PATH">SVG_PATH</see></term>
        ///     </item>
        /// </list>  </summary>
        public string symbol_format { get; set; }

        /// <summary>The actual symbol data. If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is 'svg' then this
        /// should be the raw bytes representing an svg file. If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is svg path then this
        /// should be an svg path string, for example:
        /// 'M25.979,12.896,5.979,12.896,5.979,19.562,25.979,19.562z'
        /// </summary>
        public byte[] symbol_data { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.Options.COLOR">COLOR</see>:</term>
        ///         <description>If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is 'svg' this is
        /// ignored. If <paramref cref="InsertSymbolRequest.symbol_format" />
        /// is 'svg_path' then this option specifies the color (in RRGGBB hex
        /// format) of the path. For example, to have the path rendered in red,
        /// used 'FF0000'. If 'color' is not provided then '00FF00' (i.e.
        /// green) is used by default.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an InsertSymbolRequest object with default
        /// parameters.</summary>
        public InsertSymbolRequest() { }

        /// <summary>Constructs an InsertSymbolRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="symbol_id">The id of the symbol being added. This is
        /// the same id that should be in the 'SYMBOLCODE' column for objects
        /// using this symbol  </param>
        /// <param name="symbol_format">Specifies the symbol format. Must be
        /// either 'svg' or 'svg_path'.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG">SVG</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.SymbolFormat.SVG_PATH">SVG_PATH</see></term>
        ///     </item>
        /// </list>  </param>
        /// <param name="symbol_data">The actual symbol data. If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is 'svg' then this
        /// should be the raw bytes representing an svg file. If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is svg path then this
        /// should be an svg path string, for example:
        /// 'M25.979,12.896,5.979,12.896,5.979,19.562,25.979,19.562z'  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="InsertSymbolRequest.Options.COLOR">COLOR</see>:</term>
        ///         <description>If <paramref
        /// cref="InsertSymbolRequest.symbol_format" /> is 'svg' this is
        /// ignored. If <paramref cref="InsertSymbolRequest.symbol_format" />
        /// is 'svg_path' then this option specifies the color (in RRGGBB hex
        /// format) of the path. For example, to have the path rendered in red,
        /// used 'FF0000'. If 'color' is not provided then '00FF00' (i.e.
        /// green) is used by default.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public InsertSymbolRequest( string symbol_id,
                                    string symbol_format,
                                    byte[] symbol_data,
                                    IDictionary<string, string> options = null)
        {
            this.symbol_id = symbol_id ?? "";
            this.symbol_format = symbol_format ?? "";
            this.symbol_data = symbol_data ?? new byte[] { };
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class InsertSymbolRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.insertSymbol(string,string,byte[],IDictionary{string, string})"
    /// />.</summary>
    public class InsertSymbolResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="InsertSymbolRequest.symbol_id"
        /// />.  </summary>
        public string symbol_id { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class InsertSymbolResponse




}  // end namespace kinetica
