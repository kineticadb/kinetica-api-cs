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
    /// cref="Kinetica.createContainerRegistry(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// </summary>
    public class CreateContainerRegistryRequest : KineticaData
    {

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.USER_NAME">USER_NAME</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.PASSWORD">PASSWORD</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.EMAIL">EMAIL</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>  The default value is ''.</summary>
            public const string USER_NAME = "user_name";

            /// <summary>  The default value is ''.</summary>
            public const string PASSWORD = "password";

            /// <summary>  The default value is ''.</summary>
            public const string EMAIL = "email";
        } // end struct Options

        public string registry_name { get; set; }
        public string uri { get; set; }
        public string credential { get; set; }

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.USER_NAME">USER_NAME</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.PASSWORD">PASSWORD</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.EMAIL">EMAIL</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateContainerRegistryRequest object with
        /// default parameters.</summary>
        public CreateContainerRegistryRequest() { }

        /// <summary>Constructs a CreateContainerRegistryRequest object with
        /// the specified parameters.</summary>
        /// 
        /// <param name="registry_name"></param>
        /// <param name="uri"></param>
        /// <param name="credential"></param>
        /// <param name="options">
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.USER_NAME">USER_NAME</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.PASSWORD">PASSWORD</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryRequest.Options.EMAIL">EMAIL</see>:</term>
        ///         <description>  The default value is ''.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// 
        public CreateContainerRegistryRequest( string registry_name,
                                               string uri,
                                               string credential,
                                               IDictionary<string, string> options = null)
        {
            this.registry_name = registry_name ?? "";
            this.uri = uri ?? "";
            this.credential = credential ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateContainerRegistryRequest
    /// @endcond



    /// @cond NO_DOCS
    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createContainerRegistry(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateContainerRegistryResponse : KineticaData
    {

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
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
        public int entity_id { get; set; }

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateContainerRegistryResponse.Info.KML_RESPONSE">KML_RESPONSE</see>:</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateContainerRegistryResponse
    /// @endcond





}  // end namespace kinetica
