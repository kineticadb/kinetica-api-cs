/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.grantPermissionDatasource(GrantPermissionDatasourceRequest)">Kinetica.grantPermissionDatasource</see>.
    /// </summary>
    /// <remarks><para>Grants a <a href="../../../concepts/data_sources/"
    /// target="_top">data source</a> permission to a user or role.</para>
    /// </remarks>
    public class GrantPermissionDatasourceRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="permission" />.</summary>
        /// <remarks><para>Permission to grant to the user or role</para>
        /// </remarks>
        public struct Permission
        {
            /// <summary>Admin access on the given data source</summary>
            public const string ADMIN = "admin";

            /// <summary>Connect access on the given data source</summary>
            public const string CONNECT = "connect";
        } // end struct Permission

        /// <summary>Name of the user or role to which the permission will be
        /// granted.</summary>
        /// <remarks><para>Must be an existing user or role.</para></remarks>
        public string name { get; set; }

        /// <summary>Permission to grant to the user or role.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Admin access on the given data source
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data source
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public string permission { get; set; }

        /// <summary>Name of the data source on which the permission will be
        /// granted.</summary>
        /// <remarks><para>Must be an existing data source, or an empty string
        /// to grant permission on all data sources.</para></remarks>
        public string datasource_name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a GrantPermissionDatasourceRequest object with
        /// default parameters.</summary>
        public GrantPermissionDatasourceRequest() { }

        /// <summary>Constructs a GrantPermissionDatasourceRequest object with
        /// the specified parameters.</summary>
        ///
        /// <param name="name">Name of the user or role to which the permission
        /// will be granted. Must be an existing user or role.</param>
        /// <param name="permission">Permission to grant to the user or role.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Admin access on the given data source
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data source
        ///         </description>
        ///     </item>
        /// </list></param>
        /// <param name="datasource_name">Name of the data source on which the
        /// permission will be granted. Must be an existing data source, or an
        /// empty string to grant permission on all data sources.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public GrantPermissionDatasourceRequest( string name,
                                                 string permission,
                                                 string datasource_name,
                                                 IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.permission = permission ?? "";
            this.datasource_name = datasource_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class GrantPermissionDatasourceRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.grantPermissionDatasource(GrantPermissionDatasourceRequest)">Kinetica.grantPermissionDatasource</see>.
    /// </summary>
    public class GrantPermissionDatasourceResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="GrantPermissionDatasourceRequest.name">name</see>.</summary>
        public string name { get; set; }

        /// <summary>Value of <see
        /// cref="GrantPermissionDatasourceRequest.permission">permission</see>.
        /// </summary>
        public string permission { get; set; }

        /// <summary>Value of <see
        /// cref="GrantPermissionDatasourceRequest.datasource_name">datasource_name</see>.
        /// </summary>
        public string datasource_name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class GrantPermissionDatasourceResponse
} // end namespace kinetica
