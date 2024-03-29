/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.revokePermissionDatasource(RevokePermissionDatasourceRequest)">Kinetica.revokePermissionDatasource</see>.
    /// </summary>
    /// <remarks><para>Revokes a <a href="../../../concepts/data_sources/"
    /// target="_top">data source</a> permission from a user or role.</para>
    /// </remarks>
    public class RevokePermissionDatasourceRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="permission" />.</summary>
        /// <remarks><para>Permission to revoke from the user or role</para>
        /// </remarks>
        public struct Permission
        {
            /// <summary>Admin access on the given data source</summary>
            public const string ADMIN = "admin";

            /// <summary>Connect access on the given data source</summary>
            public const string CONNECT = "connect";
        } // end struct Permission

        /// <summary>Name of the user or role from which the permission will be
        /// revoked.</summary>
        /// <remarks><para>Must be an existing user or role.</para></remarks>
        public string name { get; set; }

        /// <summary>Permission to revoke from the user or role.</summary>
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
        /// revoked.</summary>
        /// <remarks><para>Must be an existing data source, or an empty string
        /// to revoke permission from all data sources.</para></remarks>
        public string datasource_name { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a RevokePermissionDatasourceRequest object with
        /// default parameters.</summary>
        public RevokePermissionDatasourceRequest() { }

        /// <summary>Constructs a RevokePermissionDatasourceRequest object with
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
        /// permission will be revoked. Must be an existing data source, or an
        /// empty string to revoke permission from all data sources.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public RevokePermissionDatasourceRequest( string name,
                                                  string permission,
                                                  string datasource_name,
                                                  IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.permission = permission ?? "";
            this.datasource_name = datasource_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class RevokePermissionDatasourceRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.revokePermissionDatasource(RevokePermissionDatasourceRequest)">Kinetica.revokePermissionDatasource</see>.
    /// </summary>
    public class RevokePermissionDatasourceResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="RevokePermissionDatasourceRequest.name">name</see>.</summary>
        public string name { get; set; }

        /// <summary>Value of <see
        /// cref="RevokePermissionDatasourceRequest.permission">permission</see>.
        /// </summary>
        public string permission { get; set; }

        /// <summary>Value of <see
        /// cref="RevokePermissionDatasourceRequest.datasource_name">datasource_name</see>.
        /// </summary>
        public string datasource_name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class RevokePermissionDatasourceResponse
} // end namespace kinetica
