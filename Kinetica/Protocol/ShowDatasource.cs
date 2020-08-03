/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showDatasource(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Shows information about a specified <a
    /// href="../../concepts/data_sources.html" target="_top">data source</a>
    /// or all data sources.</summary>
    public class ShowDatasourceRequest : KineticaData
    {

        /// <summary>Name of the data source for which to retrieve information.
        /// The name must refer to a currently existing data source. If '*' is
        /// specified, information about all data sources will be returned.
        /// </summary>
        public string name { get; set; }

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a ShowDatasourceRequest object with default
        /// parameters.</summary>
        public ShowDatasourceRequest() { }

        /// <summary>Constructs a ShowDatasourceRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="name">Name of the data source for which to retrieve
        /// information. The name must refer to a currently existing data
        /// source. If '*' is specified, information about all data sources
        /// will be returned.  </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public ShowDatasourceRequest( string name,
                                      IDictionary<string, string> options = null)
        {
            this.name = name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class ShowDatasourceRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showDatasource(string,IDictionary{string, string})"
    /// />.</summary>
    public class ShowDatasourceResponse : KineticaData
    {

        /// <summary>The storage provider type of the data sources named in
        /// <member name="datasource_names" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowDatasourceResponse.StorageProviderTypes.HDFS">HDFS</see>:</term>
        ///         <description>Apache Hadoop Distributed File
        /// System</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowDatasourceResponse.StorageProviderTypes.S3">S3</see>:</term>
        ///         <description>Amazon S3 bucket</description>
        ///     </item>
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="storage_provider_types" />.</summary>
        public struct StorageProviderTypes
        {

            /// <summary>Apache Hadoop Distributed File System</summary>
            public const string HDFS = "hdfs";

            /// <summary>Amazon S3 bucket</summary>
            public const string S3 = "s3";
        } // end struct StorageProviderTypes


        /// <summary>Additional information about the respective data sources
        /// in <member name="datasource_names" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>
        /// A set of string constants for the parameter <member
        /// name="additional_info" />.</summary>
        public struct AdditionalInfo
        {

            /// <summary>Location of the remote storage in
            /// 'storage_provider_type://[storage_path[:storage_port]]'
            /// format</summary>
            public const string LOCATION = "location";

            /// <summary>Name of the Amazon S3 bucket used as the data
            /// source</summary>
            public const string S3_BUCKET_NAME = "s3_bucket_name";

            /// <summary>Name of the Amazon S3 region where the bucket is
            /// located</summary>
            public const string S3_REGION = "s3_region";

            /// <summary>Kerberos key for the given HDFS user</summary>
            public const string HDFS_KERBEROS_KEYTAB = "hdfs_kerberos_keytab";

            /// <summary>Name of the remote system user</summary>
            public const string USER_NAME = "user_name";
        } // end struct AdditionalInfo


        /// <summary>The data source names.  </summary>
        public IList<string> datasource_names { get; set; } = new List<string>();

        /// <summary>The storage provider type of the data sources named in
        /// <member name="datasource_names" />.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="ShowDatasourceResponse.StorageProviderTypes.HDFS">HDFS</see>:</term>
        ///         <description>Apache Hadoop Distributed File
        /// System</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="ShowDatasourceResponse.StorageProviderTypes.S3">S3</see>:</term>
        ///         <description>Amazon S3 bucket</description>
        ///     </item>
        /// </list>  </summary>
        public IList<string> storage_provider_types { get; set; } = new List<string>();

        /// <summary>Additional information about the respective data sources
        /// in <member name="datasource_names" />.
        /// Supported values:
        /// <list type="bullet">
        /// </list>  </summary>
        public IList<IDictionary<string, string>> additional_info { get; set; } = new List<IDictionary<string, string>>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class ShowDatasourceResponse




}  // end namespace kinetica