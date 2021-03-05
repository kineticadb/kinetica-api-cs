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
    /// cref="Kinetica.showContainerRegistry(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class ShowContainerRegistryRequest : KineticaData
    {
        public string registry_name { get; set; }
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowContainerRegistryRequest object with
        /// default parameters.</summary>
        public ShowContainerRegistryRequest() { }

        /// <summary>Constructs a ShowContainerRegistryRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="registry_name"></param>
        /// <param name="options"></param>
        /// 
        public ShowContainerRegistryRequest( string registry_name,
                                             IDictionary<string, string> options = null)
        {
            this.registry_name = registry_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowContainerRegistryRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showContainerRegistry(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowContainerRegistryResponse : KineticaData
    {

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowContainerRegistryResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
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

        public string registry_name { get; set; }
        public IList<string> registry_names { get; set; } = new List<string>();
        public IList<int> entity_ids { get; set; } = new List<int>();
        public IList<string> uri_list { get; set; } = new List<string>();
        public IList<string> credential_list { get; set; } = new List<string>();

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowContainerRegistryResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowContainerRegistryResponse
    /// @endcond





}  // end namespace kinetica
