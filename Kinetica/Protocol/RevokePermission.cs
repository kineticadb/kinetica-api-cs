/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.revokePermission(RevokePermissionRequest)">Kinetica.revokePermission</see>.
    /// </summary>
    /// <remarks><para>Revoke user or role the specified permission on the
    /// specified object.</para></remarks>
    public class RevokePermissionRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="object_type" />.</summary>
        /// <remarks><para>The type of object being revoked</para></remarks>
        public struct ObjectType
        {
            /// <summary>Context</summary>
            public const string CONTEXT = "context";

            /// <summary>Credential</summary>
            public const string CREDENTIAL = "credential";

            /// <summary>Data Sink</summary>
            public const string DATASINK = "datasink";

            /// <summary>Data Source</summary>
            public const string DATASOURCE = "datasource";

            /// <summary>KIFS File Directory</summary>
            public const string DIRECTORY = "directory";

            /// <summary>A Graph object</summary>
            public const string GRAPH = "graph";

            /// <summary>UDF Procedure</summary>
            public const string PROC = "proc";

            /// <summary>Schema</summary>
            public const string SCHEMA = "schema";

            /// <summary>SQL Procedure</summary>
            public const string SQL_PROC = "sql_proc";

            /// <summary>System-level access</summary>
            public const string SYSTEM = "system";

            /// <summary>Database Table</summary>
            public const string TABLE = "table";

            /// <summary>Table monitor</summary>
            public const string TABLE_MONITOR = "table_monitor";
        } // end struct ObjectType

        /// <summary>A set of string constants for the parameter <see
        /// cref="permission" />.</summary>
        /// <remarks><para>Permission being revoked.</para></remarks>
        public struct Permission
        {
            /// <summary>Full read/write and administrative access on the
            /// object.</summary>
            public const string ADMIN = "admin";

            /// <summary>Connect access on the given data source or data sink.
            /// </summary>
            public const string CONNECT = "connect";

            /// <summary>Delete rows from tables.</summary>
            public const string DELETE = "delete";

            /// <summary>Ability to Execute the Procedure object.</summary>
            public const string EXECUTE = "execute";

            /// <summary>Insert access to tables.</summary>
            public const string INSERT = "insert";

            /// <summary>Ability to read, list and use the object.</summary>
            public const string READ = "read";

            /// <summary>Update access to the table.</summary>
            public const string UPDATE = "update";

            /// <summary>Access to administer users and roles that do not have
            /// system_admin permission.</summary>
            public const string USER_ADMIN = "user_admin";

            /// <summary>Access to write, change and delete objects.</summary>
            public const string WRITE = "write";
        } // end struct Permission

        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Revoke table security from these columns,
            /// comma-separated.</summary>
            /// <remarks><para>The default value is ''.</para></remarks>
            public const string COLUMNS = "columns";
        } // end struct Options

        /// <summary>Name of the user or role for which the permission is being
        /// revoked.</summary>
        /// <remarks><para> Must be an existing user or role. The default value
        /// is ''.</para></remarks>
        public string principal { get; set; } = "";

        /// <summary>Name of object permission is being revoked from.</summary>
        /// <remarks><para> It is recommended to use a fully-qualified name
        /// when possible.</para></remarks>
        public string _object { get; set; }

        /// <summary>The type of object being revoked.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="ObjectType.CONTEXT">CONTEXT</see>:</term>
        ///         <description>Context</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.CREDENTIAL">CREDENTIAL</see>:
        ///         </term>
        ///         <description>Credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DATASINK">DATASINK</see>:
        ///         </term>
        ///         <description>Data Sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DATASOURCE">DATASOURCE</see>:
        ///         </term>
        ///         <description>Data Source</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DIRECTORY">DIRECTORY</see>:
        ///         </term>
        ///         <description>KIFS File Directory</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.GRAPH">GRAPH</see>:</term>
        ///         <description>A Graph object</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.PROC">PROC</see>:</term>
        ///         <description>UDF Procedure</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SCHEMA">SCHEMA</see>:</term>
        ///         <description>Schema</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SQL_PROC">SQL_PROC</see>:
        ///         </term>
        ///         <description>SQL Procedure</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SYSTEM">SYSTEM</see>:</term>
        ///         <description>System-level access</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.TABLE">TABLE</see>:</term>
        ///         <description>Database Table</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="ObjectType.TABLE_MONITOR">TABLE_MONITOR</see>:</term>
        ///         <description>Table monitor</description>
        ///     </item>
        /// </list></remarks>
        public string object_type { get; set; }

        /// <summary>Permission being revoked.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Full read/write and administrative access on
        ///         the object.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data source or
        ///         data sink.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.DELETE">DELETE</see>:</term>
        ///         <description>Delete rows from tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.EXECUTE">EXECUTE</see>:</term>
        ///         <description>Ability to Execute the Procedure object.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.INSERT">INSERT</see>:</term>
        ///         <description>Insert access to tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.READ">READ</see>:</term>
        ///         <description>Ability to read, list and use the object.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.UPDATE">UPDATE</see>:</term>
        ///         <description>Update access to the table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.USER_ADMIN">USER_ADMIN</see>:
        ///         </term>
        ///         <description>Access to administer users and roles that do
        ///         not have system_admin permission.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.WRITE">WRITE</see>:</term>
        ///         <description>Access to write, change and delete objects.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public string permission { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.COLUMNS">COLUMNS</see>:</term>
        ///         <description>Revoke table security from these columns,
        ///         comma-separated. The default value is ''.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a RevokePermissionRequest object with default
        /// parameters.</summary>
        public RevokePermissionRequest() { }

        /// <summary>Constructs a RevokePermissionRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="principal">Name of the user or role for which the
        /// permission is being revoked.  Must be an existing user or role. The
        /// default value is ''.</param>
        /// <param name="_object">Name of object permission is being revoked
        /// from.  It is recommended to use a fully-qualified name when
        /// possible.</param>
        /// <param name="object_type">The type of object being revoked.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="ObjectType.CONTEXT">CONTEXT</see>:</term>
        ///         <description>Context</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.CREDENTIAL">CREDENTIAL</see>:
        ///         </term>
        ///         <description>Credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DATASINK">DATASINK</see>:
        ///         </term>
        ///         <description>Data Sink</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DATASOURCE">DATASOURCE</see>:
        ///         </term>
        ///         <description>Data Source</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.DIRECTORY">DIRECTORY</see>:
        ///         </term>
        ///         <description>KIFS File Directory</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.GRAPH">GRAPH</see>:</term>
        ///         <description>A Graph object</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.PROC">PROC</see>:</term>
        ///         <description>UDF Procedure</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SCHEMA">SCHEMA</see>:</term>
        ///         <description>Schema</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SQL_PROC">SQL_PROC</see>:
        ///         </term>
        ///         <description>SQL Procedure</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.SYSTEM">SYSTEM</see>:</term>
        ///         <description>System-level access</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ObjectType.TABLE">TABLE</see>:</term>
        ///         <description>Database Table</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="ObjectType.TABLE_MONITOR">TABLE_MONITOR</see>:</term>
        ///         <description>Table monitor</description>
        ///     </item>
        /// </list></param>
        /// <param name="permission">Permission being revoked.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Full read/write and administrative access on
        ///         the object.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data source or
        ///         data sink.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.DELETE">DELETE</see>:</term>
        ///         <description>Delete rows from tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.EXECUTE">EXECUTE</see>:</term>
        ///         <description>Ability to Execute the Procedure object.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.INSERT">INSERT</see>:</term>
        ///         <description>Insert access to tables.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.READ">READ</see>:</term>
        ///         <description>Ability to read, list and use the object.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.UPDATE">UPDATE</see>:</term>
        ///         <description>Update access to the table.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.USER_ADMIN">USER_ADMIN</see>:
        ///         </term>
        ///         <description>Access to administer users and roles that do
        ///         not have system_admin permission.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.WRITE">WRITE</see>:</term>
        ///         <description>Access to write, change and delete objects.
        ///         </description>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Options.COLUMNS">COLUMNS</see>:</term>
        ///         <description>Revoke table security from these columns,
        ///         comma-separated. The default value is ''.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public RevokePermissionRequest( string principal,
                                        string _object,
                                        string object_type,
                                        string permission,
                                        IDictionary<string, string> options = null)
        {
            this.principal = principal ?? "";
            this._object = _object ?? "";
            this.object_type = object_type ?? "";
            this.permission = permission ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class RevokePermissionRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.revokePermission(RevokePermissionRequest)">Kinetica.revokePermission</see>.
    /// </summary>
    public class RevokePermissionResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="RevokePermissionRequest.principal">principal</see>.</summary>
        public string principal { get; set; }

        /// <summary>Value of <see
        /// cref="RevokePermissionRequest._object">_object</see>.</summary>
        public string _object { get; set; }

        /// <summary>Value of <see
        /// cref="RevokePermissionRequest.object_type">object_type</see>.
        /// </summary>
        public string object_type { get; set; }

        /// <summary>Value of <see
        /// cref="RevokePermissionRequest.permission">permission</see>.
        /// </summary>
        public string permission { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class RevokePermissionResponse
} // end namespace kinetica
