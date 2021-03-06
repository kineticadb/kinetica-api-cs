/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.revokePermissionCredential(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Revokes a <a
    /// href="../../../security/sec_concepts/#security-concepts-permissions-credential"
    /// target="_top">credential-level permission</a> from a user or
    /// role.</summary>
    public class RevokePermissionCredentialRequest : KineticaData
    {

        /// <summary>Permission to revoke from the user or role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_ADMIN">CREDENTIAL_ADMIN</see>:</term>
        ///         <description>Full read/write and administrative access on
        /// the credential.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_READ">CREDENTIAL_READ</see>:</term>
        ///         <description>Ability to read and use the
        /// credential.</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="permission"
        /// />.</summary>
        public struct Permission
        {

            /// <summary>Full read/write and administrative access on the
            /// credential.</summary>
            public const string CREDENTIAL_ADMIN = "credential_admin";

            /// <summary>Ability to read and use the credential.</summary>
            public const string CREDENTIAL_READ = "credential_read";
        } // end struct Permission


        /// <summary>Name of the user or role from which the permission will be
        /// revoked. Must be an existing user or role.  </summary>
        public string name { get; set; }

        /// <summary>Permission to revoke from the user or role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_ADMIN">CREDENTIAL_ADMIN</see>:</term>
        ///         <description>Full read/write and administrative access on
        /// the credential.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_READ">CREDENTIAL_READ</see>:</term>
        ///         <description>Ability to read and use the
        /// credential.</description>
        ///     </item>
        /// </list>  </summary>
        public string permission { get; set; }

        /// <summary>Name of the credential on which the permission will be
        /// revoked. Must be an existing credential, or an empty string to
        /// revoke access on all credentials.  </summary>
        public string credential_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a RevokePermissionCredentialRequest object with
        /// default parameters.</summary>
        public RevokePermissionCredentialRequest() { }

        /// <summary>Constructs a RevokePermissionCredentialRequest object with
        /// the specified parameters.</summary>
        /// 
        /// <param name="name">Name of the user or role from which the
        /// permission will be revoked. Must be an existing user or role.
        /// </param>
        /// <param name="permission">Permission to revoke from the user or
        /// role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_ADMIN">CREDENTIAL_ADMIN</see>:</term>
        ///         <description>Full read/write and administrative access on
        /// the credential.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="RevokePermissionCredentialRequest.Permission.CREDENTIAL_READ">CREDENTIAL_READ</see>:</term>
        ///         <description>Ability to read and use the
        /// credential.</description>
        ///     </item>
        /// </list>  </param>
        /// <param name="credential_name">Name of the credential on which the
        /// permission will be revoked. Must be an existing credential, or an
        /// empty string to revoke access on all credentials.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public RevokePermissionCredentialRequest( string name,
                                                  string permission,
                                                  string credential_name,
                                                  IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.permission = permission ?? "";
            this.credential_name = credential_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class RevokePermissionCredentialRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.revokePermissionCredential(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class RevokePermissionCredentialResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="RevokePermissionCredentialRequest.name" />.  </summary>
        public string name { get; set; }

        /// <summary>Value of <paramref
        /// cref="RevokePermissionCredentialRequest.permission" />.  </summary>
        public string permission { get; set; }

        /// <summary>Value of <paramref
        /// cref="RevokePermissionCredentialRequest.credential_name" />.
        /// </summary>
        public string credential_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class RevokePermissionCredentialResponse




}  // end namespace kinetica
