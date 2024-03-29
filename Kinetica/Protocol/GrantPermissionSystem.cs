/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.grantPermissionSystem(GrantPermissionSystemRequest)">Kinetica.grantPermissionSystem</see>.
    /// </summary>
    /// <remarks><para>Grants a system-level permission to a user or role.
    /// </para></remarks>
    public class GrantPermissionSystemRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="permission" />.</summary>
        /// <remarks><para>Permission to grant to the user or role.</para>
        /// </remarks>
        public struct Permission
        {
            /// <summary>Full access to all data and system functions.
            /// </summary>
            public const string SYSTEM_ADMIN = "system_admin";

            /// <summary>Access to administer users and roles that do not have
            /// system_admin permission.</summary>
            public const string SYSTEM_USER_ADMIN = "system_user_admin";

            /// <summary>Read and write access to all tables.</summary>
            public const string SYSTEM_WRITE = "system_write";

            /// <summary>Read-only access to all tables.</summary>
            public const string SYSTEM_READ = "system_read";
        } // end struct Permission

        /// <summary>Name of the user or role to which the permission will be
        /// granted.</summary>
        /// <remarks><para>Must be an existing user or role.</para></remarks>
        public string name { get; set; }

        /// <summary>Permission to grant to the user or role.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_ADMIN">SYSTEM_ADMIN</see>:</term>
        ///         <description>Full access to all data and system functions.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_USER_ADMIN">SYSTEM_USER_ADMIN</see>:
        ///         </term>
        ///         <description>Access to administer users and roles that do
        ///         not have system_admin permission.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_WRITE">SYSTEM_WRITE</see>:</term>
        ///         <description>Read and write access to all tables.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.SYSTEM_READ">SYSTEM_READ</see>:
        ///         </term>
        ///         <description>Read-only access to all tables.</description>
        ///     </item>
        /// </list></remarks>
        public string permission { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a GrantPermissionSystemRequest object with
        /// default parameters.</summary>
        public GrantPermissionSystemRequest() { }

        /// <summary>Constructs a GrantPermissionSystemRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="name">Name of the user or role to which the permission
        /// will be granted. Must be an existing user or role.</param>
        /// <param name="permission">Permission to grant to the user or role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_ADMIN">SYSTEM_ADMIN</see>:</term>
        ///         <description>Full access to all data and system functions.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_USER_ADMIN">SYSTEM_USER_ADMIN</see>:
        ///         </term>
        ///         <description>Access to administer users and roles that do
        ///         not have system_admin permission.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Permission.SYSTEM_WRITE">SYSTEM_WRITE</see>:</term>
        ///         <description>Read and write access to all tables.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.SYSTEM_READ">SYSTEM_READ</see>:
        ///         </term>
        ///         <description>Read-only access to all tables.</description>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public GrantPermissionSystemRequest( string name,
                                             string permission,
                                             IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.permission = permission ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class GrantPermissionSystemRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.grantPermissionSystem(GrantPermissionSystemRequest)">Kinetica.grantPermissionSystem</see>.
    /// </summary>
    public class GrantPermissionSystemResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="GrantPermissionSystemRequest.name">name</see>.</summary>
        public string name { get; set; }

        /// <summary>Value of <see
        /// cref="GrantPermissionSystemRequest.permission">permission</see>.
        /// </summary>
        public string permission { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class GrantPermissionSystemResponse
} // end namespace kinetica
