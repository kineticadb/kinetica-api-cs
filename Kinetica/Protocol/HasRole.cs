/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.hasRole(string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Checks if the specified user has the specified role.</summary>
    public class HasRoleRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="HasRoleRequest.role" /> does not exist or
        /// is blank. If <i>true</i> then it will return <i>false</i> for
        /// <member name="has_role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.ONLY_DIRECT">ONLY_DIRECT</see>:</term>
        ///         <description>If <i>false</i> will search recursively if the
        /// <paramref cref="HasRoleRequest.principal" /> is a member of
        /// <paramref cref="HasRoleRequest.role" />.  If <i>true</i> then
        /// <paramref cref="HasRoleRequest.principal" /> must directly be a
        /// member of <paramref cref="HasRoleRequest.role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>false</i> will return an error if the provided
            /// <see cref="role" /> does not exist or is blank. If <i>true</i>
            /// then it will return <i>false</i> for <member name="has_role"
            /// />.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>If <i>false</i> will search recursively if the <see
            /// cref="principal" /> is a member of <see cref="role" />.  If
            /// <i>true</i> then <see cref="principal" /> must directly be a
            /// member of <see cref="role" />.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</summary>
            public const string ONLY_DIRECT = "only_direct";
        } // end struct Options


        /// <summary>Name of the user for which role membersih is being
        /// checked. Must be an existing user. If blank, will use the current
        /// user.  The default value is ''.</summary>
        public string principal { get; set; } = "";

        /// <summary>Name of role to check for membership.  </summary>
        public string role { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="HasRoleRequest.role" /> does not exist or
        /// is blank. If <i>true</i> then it will return <i>false</i> for
        /// <member name="has_role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.ONLY_DIRECT">ONLY_DIRECT</see>:</term>
        ///         <description>If <i>false</i> will search recursively if the
        /// <paramref cref="HasRoleRequest.principal" /> is a member of
        /// <paramref cref="HasRoleRequest.role" />.  If <i>true</i> then
        /// <paramref cref="HasRoleRequest.principal" /> must directly be a
        /// member of <paramref cref="HasRoleRequest.role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a HasRoleRequest object with default
        /// parameters.</summary>
        public HasRoleRequest() { }

        /// <summary>Constructs a HasRoleRequest object with the specified
        /// parameters.</summary>
        /// 
        /// <param name="principal">Name of the user for which role membersih
        /// is being checked. Must be an existing user. If blank, will use the
        /// current user.  The default value is ''.</param>
        /// <param name="role">Name of role to check for membership.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:</term>
        ///         <description>If <i>false</i> will return an error if the
        /// provided <paramref cref="HasRoleRequest.role" /> does not exist or
        /// is blank. If <i>true</i> then it will return <i>false</i> for
        /// <member name="has_role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.ONLY_DIRECT">ONLY_DIRECT</see>:</term>
        ///         <description>If <i>false</i> will search recursively if the
        /// <paramref cref="HasRoleRequest.principal" /> is a member of
        /// <paramref cref="HasRoleRequest.role" />.  If <i>true</i> then
        /// <paramref cref="HasRoleRequest.principal" /> must directly be a
        /// member of <paramref cref="HasRoleRequest.role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public HasRoleRequest( string principal,
                               string role,
                               IDictionary<string, string> options = null)
        {
            this.principal = principal ?? "";
            this.role = role ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class HasRoleRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.hasRole(string,string,IDictionary{string, string})"
    /// />.</summary>
    public class HasRoleResponse : KineticaData
    {

        /// <summary>Indicates whether the specified user has membership in the
        /// specified target <paramref cref="HasRoleRequest.role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.HasRole.TRUE">TRUE</see>:</term>
        ///         <description>User has membership in the role</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.HasRole.FALSE">FALSE</see>:</term>
        ///         <description>User does not have membership in the
        /// role</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <member name="has_role"
        /// />.</summary>
        public struct HasRole
        {

            /// <summary>User has membership in the role</summary>
            public const string TRUE = "true";

            /// <summary>User does not have membership in the role</summary>
            public const string FALSE = "false";
        } // end struct HasRole


        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.DIRECT">DIRECT</see>:</term>
        ///         <description><i>true</i> when principal is directly a
        /// member of the role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleResponse.Info.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <member name="info"
        /// />.</summary>
        public struct Info
        {

            /// <summary><i>true</i> when principal is directly a member of the
            /// role.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="HasRoleResponse.Info.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="HasRoleResponse.Info.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="HasRoleResponse.Info.FALSE">FALSE</see>.</summary>
            public const string DIRECT = "direct";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Info


        /// <summary>Value of <paramref cref="HasRoleRequest.principal" />
        /// </summary>
        public string principal { get; set; }

        /// <summary><paramref cref="HasRoleRequest.role" /> for which
        /// membership is being checked  </summary>
        public string role { get; set; }

        /// <summary>Indicates whether the specified user has membership in the
        /// specified target <paramref cref="HasRoleRequest.role" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.HasRole.TRUE">TRUE</see>:</term>
        ///         <description>User has membership in the role</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.HasRole.FALSE">FALSE</see>:</term>
        ///         <description>User does not have membership in the
        /// role</description>
        ///     </item>
        /// </list>  </summary>
        public bool has_role { get; set; }

        /// <summary>Additional information.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.DIRECT">DIRECT</see>:</term>
        ///         <description><i>true</i> when principal is directly a
        /// member of the role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="HasRoleResponse.Info.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="HasRoleResponse.Info.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class HasRoleResponse




}  // end namespace kinetica