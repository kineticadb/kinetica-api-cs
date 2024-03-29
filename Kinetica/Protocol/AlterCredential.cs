/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterCredential(AlterCredentialRequest)">Kinetica.alterCredential</see>.
    /// </summary>
    /// <remarks><para>Alter the properties of an existing <a
    /// href="../../../concepts/credentials/" target="_top">credential</a>.
    /// </para></remarks>
    public class AlterCredentialRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="credential_updates_map" />.</summary>
        /// <remarks><para>Map containing the properties of the credential to
        /// be updated. Error if empty.</para></remarks>
        public struct CredentialUpdatesMap
        {
            /// <summary>New type for the credential.</summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AZURE_AD">AZURE_AD</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.DOCKER">DOCKER</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_ID">GCS_SERVICE_ACCOUNT_ID</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_KEYS">GCS_SERVICE_ACCOUNT_KEYS</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see cref="CredentialUpdatesMap.HDFS">HDFS</see>
            ///         </term>
            ///     </item>
            ///     <item>
            ///         <term><see
            ///         cref="CredentialUpdatesMap.KAFKA">KAFKA</see></term>
            ///     </item>
            /// </list></remarks>
            public const string TYPE = "type";

            public const string AWS_ACCESS_KEY = "aws_access_key";
            public const string AWS_IAM_ROLE = "aws_iam_role";
            public const string AZURE_AD = "azure_ad";
            public const string AZURE_OAUTH = "azure_oauth";
            public const string AZURE_SAS = "azure_sas";
            public const string AZURE_STORAGE_KEY = "azure_storage_key";
            public const string DOCKER = "docker";
            public const string GCS_SERVICE_ACCOUNT_ID = "gcs_service_account_id";
            public const string GCS_SERVICE_ACCOUNT_KEYS = "gcs_service_account_keys";
            public const string HDFS = "hdfs";
            public const string KAFKA = "kafka";

            /// <summary>New user for the credential</summary>
            public const string IDENTITY = "identity";

            /// <summary>New password for the credential</summary>
            public const string SECRET = "secret";

            /// <summary>Updates the schema name.</summary>
            /// <remarks><para> If <see
            /// cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
            /// doesn't exist, an error will be thrown. If <see
            /// cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see> is
            /// empty, then the user's default schema will be used.</para>
            /// </remarks>
            public const string SCHEMA_NAME = "schema_name";
        } // end struct CredentialUpdatesMap

        /// <summary>Name of the credential to be altered.</summary>
        /// <remarks><para>Must be an existing credential.</para></remarks>
        public string credential_name { get; set; }

        /// <summary>Map containing the properties of the credential to be
        /// updated.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see cref="CredentialUpdatesMap.TYPE">TYPE</see>:
        ///         </term>
        ///         <description>New type for the credential.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_AD">AZURE_AD</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.DOCKER">DOCKER</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_ID">GCS_SERVICE_ACCOUNT_ID</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_KEYS">GCS_SERVICE_ACCOUNT_KEYS</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.HDFS">HDFS</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.KAFKA">KAFKA</see>
        ///                 </term>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="CredentialUpdatesMap.IDENTITY">IDENTITY</see>:</term>
        ///         <description>New user for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CredentialUpdatesMap.SECRET">SECRET</see>:
        ///         </term>
        ///         <description>New password for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:
        ///         </term>
        ///         <description>Updates the schema name.  If <see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         doesn't exist, an error will be thrown. If <see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         is empty, then the user's default schema will be used.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        public IDictionary<string, string> credential_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an AlterCredentialRequest object with default
        /// parameters.</summary>
        public AlterCredentialRequest() { }

        /// <summary>Constructs an AlterCredentialRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="credential_name">Name of the credential to be altered.
        /// Must be an existing credential.</param>
        /// <param name="credential_updates_map">Map containing the properties
        /// of the credential to be updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="CredentialUpdatesMap.TYPE">TYPE</see>:
        ///         </term>
        ///         <description>New type for the credential.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_AD">AZURE_AD</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.DOCKER">DOCKER</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_ID">GCS_SERVICE_ACCOUNT_ID</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.GCS_SERVICE_ACCOUNT_KEYS">GCS_SERVICE_ACCOUNT_KEYS</see>
        ///                 </term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.HDFS">HDFS</see></term>
        ///             </item>
        ///             <item>
        ///                 <term><see
        ///                 cref="CredentialUpdatesMap.KAFKA">KAFKA</see>
        ///                 </term>
        ///             </item>
        ///         </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="CredentialUpdatesMap.IDENTITY">IDENTITY</see>:</term>
        ///         <description>New user for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CredentialUpdatesMap.SECRET">SECRET</see>:
        ///         </term>
        ///         <description>New password for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>:
        ///         </term>
        ///         <description>Updates the schema name.  If <see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         doesn't exist, an error will be thrown. If <see
        ///         cref="CredentialUpdatesMap.SCHEMA_NAME">SCHEMA_NAME</see>
        ///         is empty, then the user's default schema will be used.
        ///         </description>
        ///     </item>
        /// </list></param>
        /// <param name="options">Optional parameters.</param>
        public AlterCredentialRequest( string credential_name,
                                       IDictionary<string, string> credential_updates_map,
                                       IDictionary<string, string> options)
        {
            this.credential_name = credential_name ?? "";
            this.credential_updates_map = credential_updates_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class AlterCredentialRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterCredential(AlterCredentialRequest)">Kinetica.alterCredential</see>.
    /// </summary>
    public class AlterCredentialResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="AlterCredentialRequest.credential_name">credential_name</see>.
        /// </summary>
        public string credential_name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class AlterCredentialResponse
} // end namespace kinetica
