/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{
    /// @cond NO_DOCS
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showModel(IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class ShowModelRequest : KineticaData
    {
        public IList<string> model_names { get; set; } = new List<string>();
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowModelRequest object with default
        /// parameters.</summary>
        public ShowModelRequest() { }

        /// <summary>Constructs a ShowModelRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="model_names"></param>
        /// <param name="options"></param>
        /// 
        public ShowModelRequest( IList<string> model_names = null,
                                 IDictionary<string, string> options = null)
        {
            this.model_names = model_names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowModelRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showModel(IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class ShowModelResponse : KineticaData
    {

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowModelResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <member name="info"
        /// />.</summary>
        public struct Info
        {
            public const string KML_RESPONSE = "kml_response";
        } // end struct Info

        public IList<string> model_names { get; set; } = new List<string>();
        public IList<int> entity_ids { get; set; } = new List<int>();
        public IList<string> input_schemas { get; set; } = new List<string>();
        public IList<string> output_schemas { get; set; } = new List<string>();
        public IList<string> registry_list { get; set; } = new List<string>();
        public IList<string> container_list { get; set; } = new List<string>();
        public IList<string> run_function_list { get; set; } = new List<string>();

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowModelResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowModelResponse
    /// @endcond





}  // end namespace kinetica
