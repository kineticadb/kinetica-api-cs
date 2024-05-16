/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.grantPermissionDatasource(string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Grants a <a href="../../../concepts/data_sources/" target="_top">data
    /// source</a> permission to a user or role.</summary>
    public class GrantPermissionDatasourceRequest : KineticaData
    {

        /// <summary>Permission to grant to the user or role
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Admin access on the given data
        /// source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data
        /// source</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <see cref="permission"
        /// />.</summary>
        public struct Permission
        {

            /// <summary>Admin access on the given data source</summary>
            public const string ADMIN = "admin";

            /// <summary>Connect access on the given data source</summary>
            public const string CONNECT = "connect";
        } // end struct Permission


        /// <summary>Name of the user or role to which the permission will be
        /// granted. Must be an existing user or role.  </summary>
        public string name { get; set; }

        /// <summary>Permission to grant to the user or role
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Admin access on the given data
        /// source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data
        /// source</description>
        ///     </item>
        /// </list>  </summary>
        public string permission { get; set; }

        /// <summary>Name of the data source on which the permission will be
        /// granted. Must be an existing data source, or an empty string to
        /// grant permission on all data sources.  </summary>
        public string datasource_name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a GrantPermissionDatasourceRequest object with
        /// default parameters.</summary>
        public GrantPermissionDatasourceRequest() { }

        /// <summary>Constructs a GrantPermissionDatasourceRequest object with
        /// the specified parameters.</summary>
        /// 
        /// <param name="name">Name of the user or role to which the permission
        /// will be granted. Must be an existing user or role.  </param>
        /// <param name="permission">Permission to grant to the user or role
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.ADMIN">ADMIN</see>:</term>
        ///         <description>Admin access on the given data
        /// source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="GrantPermissionDatasourceRequest.Permission.CONNECT">CONNECT</see>:</term>
        ///         <description>Connect access on the given data
        /// source</description>
        ///     </item>
        /// </list>  </param>
        /// <param name="datasource_name">Name of the data source on which the
        /// permission will be granted. Must be an existing data source, or an
        /// empty string to grant permission on all data sources.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
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
    /// cref="Kinetica.grantPermissionDatasource(string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class GrantPermissionDatasourceResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="GrantPermissionDatasourceRequest.name" />.  </summary>
        public string name { get; set; }

        /// <summary>Value of <paramref
        /// cref="GrantPermissionDatasourceRequest.permission" />.  </summary>
        public string permission { get; set; }

        /// <summary>Value of <paramref
        /// cref="GrantPermissionDatasourceRequest.datasource_name" />.
        /// </summary>
        public string datasource_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class GrantPermissionDatasourceResponse




}  // end namespace kinetica
