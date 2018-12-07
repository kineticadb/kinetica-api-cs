/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showResourceGroups(IList{string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Shows resource group properties.</summary>
    public class ShowResourceGroupsRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_VALUES">SHOW_DEFAULT_VALUES</see>:</term>
        ///         <description>If true include values of fields that are
        /// based on the default resource group.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_GROUP">SHOW_DEFAULT_GROUP</see>:</term>
        ///         <description>If true include the default resource group in
        /// the response.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If true include values of fields that are based on the
            /// default resource group.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</summary>
            public const string SHOW_DEFAULT_VALUES = "show_default_values";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If true include the default resource group in the
            /// response.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</summary>
            public const string SHOW_DEFAULT_GROUP = "show_default_group";
        } // end struct Options


        /// <summary>List of names of groups to be shown. A single entry with
        /// an empty string returns all groups.  </summary>
        public IList<string> names { get; set; } = new List<string>();

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_VALUES">SHOW_DEFAULT_VALUES</see>:</term>
        ///         <description>If true include values of fields that are
        /// based on the default resource group.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_GROUP">SHOW_DEFAULT_GROUP</see>:</term>
        ///         <description>If true include the default resource group in
        /// the response.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowResourceGroupsRequest object with default
        /// parameters.</summary>
        public ShowResourceGroupsRequest() { }

        /// <summary>Constructs a ShowResourceGroupsRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="names">List of names of groups to be shown. A single
        /// entry with an empty string returns all groups.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_VALUES">SHOW_DEFAULT_VALUES</see>:</term>
        ///         <description>If true include values of fields that are
        /// based on the default resource group.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.SHOW_DEFAULT_GROUP">SHOW_DEFAULT_GROUP</see>:</term>
        ///         <description>If true include the default resource group in
        /// the response.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowResourceGroupsRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="ShowResourceGroupsRequest.Options.TRUE">TRUE</see>.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// 
        public ShowResourceGroupsRequest( IList<string> names,
                                          IDictionary<string, string> options = null)
        {
            this.names = names ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowResourceGroupsRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showResourceGroups(IList{string},IDictionary{string, string})"
    /// />.</summary>
    public class ShowResourceGroupsResponse : KineticaData
    {

        /// <summary>Map of resource group information.  </summary>
        public IDictionary<string, IDictionary<string, string>> groups { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowResourceGroupsResponse




}  // end namespace kinetica
