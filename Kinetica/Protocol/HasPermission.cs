/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.hasPermission(HasPermissionRequest)">Kinetica.hasPermission</see>.
    /// </summary>
    /// <remarks><para>Checks if the specified user has the specified
    /// permission on the specified object.</para></remarks>
    public class HasPermissionRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="object_type" />.</summary>
        /// <remarks><para>The type of object being checked</para></remarks>
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

            /// <summary>KiFS File Directory</summary>
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
        /// <remarks><para>Permission to check for.</para></remarks>
        public struct Permission
        {
            /// <summary>Full read/write and administrative access on the
            /// object.</summary>
            public const string ADMIN = "admin";

            /// <summary>Connect access on the given data source or data sink.
            /// </summary>
            public const string CONNECT = "connect";

            /// <summary>Ability to create new objects of this type.</summary>
            public const string CREATE = "create";

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
            /// <summary>If <see cref="Options.FALSE">FALSE</see> will return
            /// an error if the provided <see cref="_object" /> does not exist
            /// or is blank.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// <para>The default value is <see
            /// cref="Options.FALSE">FALSE</see>.</para></remarks>
            public const string NO_ERROR_IF_NOT_EXISTS = "no_error_if_not_exists";

            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options

        /// <summary>Name of the user for which the permission is being
        /// checked.</summary>
        /// <remarks><para>Must be an existing user. If blank, will use the
        /// current user. The default value is ''.</para></remarks>
        public string principal { get; set; } = "";

        /// <summary>Name of object to check for the requested permission.
        /// </summary>
        /// <remarks><para> It is recommended to use a fully-qualified name
        /// when possible.</para></remarks>
        public string _object { get; set; }

        /// <summary>The type of object being checked.</summary>
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
        ///         <description>KiFS File Directory</description>
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

        /// <summary>Permission to check for.</summary>
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
        ///         <term><see cref="Permission.CREATE">CREATE</see>:</term>
        ///         <description>Ability to create new objects of this type.
        ///         </description>
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
        ///         <term><see
        ///         cref="Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.FALSE">FALSE</see> will
        ///         return an error if the provided <see cref="_object" /> does
        ///         not exist or is blank. If <see
        ///         cref="Options.TRUE">TRUE</see> then it will return <see
        ///         cref="HasPermissionResponse.HasPermission.FALSE">FALSE</see>
        ///         for <see
        ///         cref="HasPermissionResponse.has_permission">has_permission</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a HasPermissionRequest object with default
        /// parameters.</summary>
        public HasPermissionRequest() { }

        /// <summary>Constructs a HasPermissionRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="principal">Name of the user for which the permission
        /// is being checked. Must be an existing user. If blank, will use the
        /// current user. The default value is ''.</param>
        /// <param name="_object">Name of object to check for the requested
        /// permission.  It is recommended to use a fully-qualified name when
        /// possible.</param>
        /// <param name="object_type">The type of object being checked.
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
        ///         <description>KiFS File Directory</description>
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
        /// <param name="permission">Permission to check for.
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
        ///         <term><see cref="Permission.CREATE">CREATE</see>:</term>
        ///         <description>Ability to create new objects of this type.
        ///         </description>
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
        ///         <term><see
        ///         cref="Options.NO_ERROR_IF_NOT_EXISTS">NO_ERROR_IF_NOT_EXISTS</see>:
        ///         </term>
        ///         <description>If <see cref="Options.FALSE">FALSE</see> will
        ///         return an error if the provided <paramref name="_object" />
        ///         does not exist or is blank. If <see
        ///         cref="Options.TRUE">TRUE</see> then it will return <see
        ///         cref="HasPermissionResponse.HasPermission.FALSE">FALSE</see>
        ///         for <see
        ///         cref="HasPermissionResponse.has_permission">has_permission</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.TRUE">TRUE</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.FALSE">FALSE</see></term>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.FALSE">FALSE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public HasPermissionRequest( string principal,
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
    } // end class HasPermissionRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.hasPermission(HasPermissionRequest)">Kinetica.hasPermission</see>.
    /// </summary>
    public class HasPermissionResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="has_permission" />.</summary>
        /// <remarks><para>Indicates whether the specified user has the
        /// specified permission on the specified target.</para></remarks>
        public struct HasPermission
        {
            /// <summary>User has the effective queried permission</summary>
            public const string TRUE = "true";

            /// <summary>User does not have the queried permission</summary>
            public const string FALSE = "false";
        } // end struct HasPermission

        /// <summary>Value of <see
        /// cref="HasPermissionRequest.principal">principal</see></summary>
        public string principal { get; set; }

        /// <summary>Fully-qualified value of <see
        /// cref="HasPermissionRequest._object">_object</see></summary>
        public string _object { get; set; }

        /// <summary>Value of <see
        /// cref="HasPermissionRequest.object_type">object_type</see></summary>
        public string object_type { get; set; }

        /// <summary>Value of <see
        /// cref="HasPermissionRequest.permission">permission</see></summary>
        public string permission { get; set; }

        /// <summary>Indicates whether the specified user has the specified
        /// permission on the specified target.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>true:</term>
        ///         <description>User has the effective queried permission
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>false:</term>
        ///         <description>User does not have the queried permission
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public bool has_permission { get; set; }

        /// <summary>Map of column/filters that have been granted.</summary>
        public IDictionary<string, string> filters { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class HasPermissionResponse
} // end namespace kinetica
