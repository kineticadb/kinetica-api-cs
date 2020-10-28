/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createDatasource(string,string,string,string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a <a href="../../concepts/data_sources.html" target="_top">data
    /// source</a>, which contains the
    /// location and connection information for a data store that is external
    /// to the database.</summary>
    public class CreateDatasourceRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to remote
        /// source.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for reading from this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_BUCKET_NAME">S3_BUCKET_NAME</see>:</term>
        ///         <description>Name of the Amazon S3 bucket to use as the
        /// data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_REGION">S3_REGION</see>:</term>
        ///         <description>Name of the Amazon S3 region where the given
        /// bucket is located</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_KERBEROS_KEYTAB">HDFS_KERBEROS_KEYTAB</see>:</term>
        ///         <description>Kerberos keytab file location for the given
        /// HDFS user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_DELEGATION_TOKEN">HDFS_DELEGATION_TOKEN</see>:</term>
        ///         <description>Delegation token for the given HDFS
        /// user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_USE_KERBEROS">HDFS_USE_KERBEROS</see>:</term>
        ///         <description>Use kerberos authentication for the given HDFS
        /// cluster
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_STORAGE_ACCOUNT_NAME">AZURE_STORAGE_ACCOUNT_NAME</see>:</term>
        ///         <description>Name of the Azure storage account to use as
        /// the data source, this is valid only if tenant_id is
        /// specified</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_CONTAINER_NAME">AZURE_CONTAINER_NAME</see>:</term>
        ///         <description>Name of the Azure storage container to use as
        /// the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_TENANT_ID">AZURE_TENANT_ID</see>:</term>
        ///         <description>Active Directory tenant ID (or directory
        /// ID)</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_SAS_TOKEN">AZURE_SAS_TOKEN</see>:</term>
        ///         <description>Shared access signature token for Azure
        /// storage account to use as the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_OAUTH_TOKEN">AZURE_OAUTH_TOKEN</see>:</term>
        ///         <description>Oauth token to access given storage
        /// container</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>Bypass validation of connection to remote source.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</summary>
            public const string SKIP_VALIDATION = "skip_validation";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Timeout in seconds for connecting to this storage
            /// provider</summary>
            public const string CONNECTION_TIMEOUT = "connection_timeout";

            /// <summary>Timeout in seconds for reading from this storage
            /// provider</summary>
            public const string WAIT_TIMEOUT = "wait_timeout";

            /// <summary>Name of the Amazon S3 bucket to use as the data
            /// source</summary>
            public const string S3_BUCKET_NAME = "s3_bucket_name";

            /// <summary>Name of the Amazon S3 region where the given bucket is
            /// located</summary>
            public const string S3_REGION = "s3_region";

            /// <summary>Kerberos keytab file location for the given HDFS
            /// user</summary>
            public const string HDFS_KERBEROS_KEYTAB = "hdfs_kerberos_keytab";

            /// <summary>Delegation token for the given HDFS user</summary>
            public const string HDFS_DELEGATION_TOKEN = "hdfs_delegation_token";

            /// <summary>Use kerberos authentication for the given HDFS cluster
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</summary>
            public const string HDFS_USE_KERBEROS = "hdfs_use_kerberos";

            /// <summary>Name of the Azure storage account to use as the data
            /// source, this is valid only if tenant_id is specified</summary>
            public const string AZURE_STORAGE_ACCOUNT_NAME = "azure_storage_account_name";

            /// <summary>Name of the Azure storage container to use as the data
            /// source</summary>
            public const string AZURE_CONTAINER_NAME = "azure_container_name";

            /// <summary>Active Directory tenant ID (or directory ID)</summary>
            public const string AZURE_TENANT_ID = "azure_tenant_id";

            /// <summary>Shared access signature token for Azure storage
            /// account to use as the data source</summary>
            public const string AZURE_SAS_TOKEN = "azure_sas_token";

            /// <summary>Oauth token to access given storage
            /// container</summary>
            public const string AZURE_OAUTH_TOKEN = "azure_oauth_token";
        } // end struct Options


        /// <summary>Name of the data source to be created.  </summary>
        public string name { get; set; }

        /// <summary>Location of the remote storage in
        /// 'storage_provider_type://[storage_path[:storage_port]]' format.
        /// <br />
        /// Supported storage provider types are 'hdfs' and 's3'.  </summary>
        public string location { get; set; }

        /// <summary>Name of the remote system user; may be an empty string
        /// </summary>
        public string user_name { get; set; }

        /// <summary>Password for the remote system user; may be an empty
        /// string  </summary>
        public string password { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to remote
        /// source.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for reading from this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_BUCKET_NAME">S3_BUCKET_NAME</see>:</term>
        ///         <description>Name of the Amazon S3 bucket to use as the
        /// data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_REGION">S3_REGION</see>:</term>
        ///         <description>Name of the Amazon S3 region where the given
        /// bucket is located</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_KERBEROS_KEYTAB">HDFS_KERBEROS_KEYTAB</see>:</term>
        ///         <description>Kerberos keytab file location for the given
        /// HDFS user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_DELEGATION_TOKEN">HDFS_DELEGATION_TOKEN</see>:</term>
        ///         <description>Delegation token for the given HDFS
        /// user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_USE_KERBEROS">HDFS_USE_KERBEROS</see>:</term>
        ///         <description>Use kerberos authentication for the given HDFS
        /// cluster
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_STORAGE_ACCOUNT_NAME">AZURE_STORAGE_ACCOUNT_NAME</see>:</term>
        ///         <description>Name of the Azure storage account to use as
        /// the data source, this is valid only if tenant_id is
        /// specified</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_CONTAINER_NAME">AZURE_CONTAINER_NAME</see>:</term>
        ///         <description>Name of the Azure storage container to use as
        /// the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_TENANT_ID">AZURE_TENANT_ID</see>:</term>
        ///         <description>Active Directory tenant ID (or directory
        /// ID)</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_SAS_TOKEN">AZURE_SAS_TOKEN</see>:</term>
        ///         <description>Shared access signature token for Azure
        /// storage account to use as the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_OAUTH_TOKEN">AZURE_OAUTH_TOKEN</see>:</term>
        ///         <description>Oauth token to access given storage
        /// container</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateDatasourceRequest object with default
        /// parameters.</summary>
        public CreateDatasourceRequest() { }

        /// <summary>Constructs a CreateDatasourceRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the data source to be created.  </param>
        /// <param name="location">Location of the remote storage in
        /// 'storage_provider_type://[storage_path[:storage_port]]' format.
        /// Supported storage provider types are 'hdfs' and 's3'.  </param>
        /// <param name="user_name">Name of the remote system user; may be an
        /// empty string  </param>
        /// <param name="password">Password for the remote system user; may be
        /// an empty string  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.SKIP_VALIDATION">SKIP_VALIDATION</see>:</term>
        ///         <description>Bypass validation of connection to remote
        /// source.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.CONNECTION_TIMEOUT">CONNECTION_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for connecting to this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.WAIT_TIMEOUT">WAIT_TIMEOUT</see>:</term>
        ///         <description>Timeout in seconds for reading from this
        /// storage provider</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_BUCKET_NAME">S3_BUCKET_NAME</see>:</term>
        ///         <description>Name of the Amazon S3 bucket to use as the
        /// data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.S3_REGION">S3_REGION</see>:</term>
        ///         <description>Name of the Amazon S3 region where the given
        /// bucket is located</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_KERBEROS_KEYTAB">HDFS_KERBEROS_KEYTAB</see>:</term>
        ///         <description>Kerberos keytab file location for the given
        /// HDFS user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_DELEGATION_TOKEN">HDFS_DELEGATION_TOKEN</see>:</term>
        ///         <description>Delegation token for the given HDFS
        /// user</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.HDFS_USE_KERBEROS">HDFS_USE_KERBEROS</see>:</term>
        ///         <description>Use kerberos authentication for the given HDFS
        /// cluster
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDatasourceRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_STORAGE_ACCOUNT_NAME">AZURE_STORAGE_ACCOUNT_NAME</see>:</term>
        ///         <description>Name of the Azure storage account to use as
        /// the data source, this is valid only if tenant_id is
        /// specified</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_CONTAINER_NAME">AZURE_CONTAINER_NAME</see>:</term>
        ///         <description>Name of the Azure storage container to use as
        /// the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_TENANT_ID">AZURE_TENANT_ID</see>:</term>
        ///         <description>Active Directory tenant ID (or directory
        /// ID)</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_SAS_TOKEN">AZURE_SAS_TOKEN</see>:</term>
        ///         <description>Shared access signature token for Azure
        /// storage account to use as the data source</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDatasourceRequest.Options.AZURE_OAUTH_TOKEN">AZURE_OAUTH_TOKEN</see>:</term>
        ///         <description>Oauth token to access given storage
        /// container</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public CreateDatasourceRequest( string name,
                                        string location,
                                        string user_name,
                                        string password,
                                        IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.location = location ?? "";
            this.user_name = user_name ?? "";
            this.password = password ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateDatasourceRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createDatasource(string,string,string,string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateDatasourceResponse : KineticaData
    {

        /// <summary>Value of <paramref cref="CreateDatasourceRequest.name" />.
        /// </summary>
        public string name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateDatasourceResponse




}  // end namespace kinetica
