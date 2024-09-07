/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createCredential(CreateCredentialRequest)">Kinetica.createCredential</see>.
    /// </summary>
    /// <remarks><para>Create a new <a href="../../../concepts/credentials/"
    /// target="_top">credential</a>.</para></remarks>
    public class CreateCredentialRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="type" />.</summary>
        /// <remarks><para>Type of the credential to be created.</para>
        /// </remarks>
        public struct Type
        {
            public const string AWS_ACCESS_KEY = "aws_access_key";
            public const string AWS_IAM_ROLE = "aws_iam_role";
            public const string AZURE_AD = "azure_ad";
            public const string AZURE_OAUTH = "azure_oauth";
            public const string AZURE_SAS = "azure_sas";
            public const string AZURE_STORAGE_KEY = "azure_storage_key";
            public const string CONFLUENT = "confluent";
            public const string DOCKER = "docker";
            public const string GCS_SERVICE_ACCOUNT_ID = "gcs_service_account_id";
            public const string GCS_SERVICE_ACCOUNT_KEYS = "gcs_service_account_keys";
            public const string HDFS = "hdfs";
            public const string JDBC = "jdbc";
            public const string KAFKA = "kafka";
            public const string NVIDIA_API_KEY = "nvidia_api_key";
            public const string OPENAI_API_KEY = "openai_api_key";
        } // end struct Type

        /// <summary>Name of the credential to be created.</summary>
        /// <remarks><para>Must contain only letters, digits, and underscores,
        /// and cannot begin with a digit. Must not match an existing
        /// credential name.</para></remarks>
        public string credential_name { get; set; }

        /// <summary>Type of the credential to be created.</summary>
        /// <remarks><para>Supported values:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Type.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AWS_IAM_ROLE">AWS_IAM_ROLE</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_AD">AZURE_AD</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_OAUTH">AZURE_OAUTH</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_SAS">AZURE_SAS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.CONFLUENT">CONFLUENT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.DOCKER">DOCKER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.GCS_SERVICE_ACCOUNT_ID">GCS_SERVICE_ACCOUNT_ID</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.GCS_SERVICE_ACCOUNT_KEYS">GCS_SERVICE_ACCOUNT_KEYS</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.HDFS">HDFS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.JDBC">JDBC</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.KAFKA">KAFKA</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.NVIDIA_API_KEY">NVIDIA_API_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.OPENAI_API_KEY">OPENAI_API_KEY</see>
        ///         </term>
        ///     </item>
        /// </list></remarks>
        public string type { get; set; }

        /// <summary>User of the credential to be created.</summary>
        public string identity { get; set; }

        /// <summary>Password of the credential to be created.</summary>
        public string secret { get; set; }

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a CreateCredentialRequest object with default
        /// parameters.</summary>
        public CreateCredentialRequest() { }

        /// <summary>Constructs a CreateCredentialRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="credential_name">Name of the credential to be created.
        /// Must contain only letters, digits, and underscores, and cannot
        /// begin with a digit. Must not match an existing credential name.
        /// </param>
        /// <param name="type">Type of the credential to be created.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Type.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AWS_IAM_ROLE">AWS_IAM_ROLE</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_AD">AZURE_AD</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_OAUTH">AZURE_OAUTH</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.AZURE_SAS">AZURE_SAS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.CONFLUENT">CONFLUENT</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.DOCKER">DOCKER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.GCS_SERVICE_ACCOUNT_ID">GCS_SERVICE_ACCOUNT_ID</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see
        ///         cref="Type.GCS_SERVICE_ACCOUNT_KEYS">GCS_SERVICE_ACCOUNT_KEYS</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.HDFS">HDFS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.JDBC">JDBC</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.KAFKA">KAFKA</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.NVIDIA_API_KEY">NVIDIA_API_KEY</see>
        ///         </term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Type.OPENAI_API_KEY">OPENAI_API_KEY</see>
        ///         </term>
        ///     </item>
        /// </list></param>
        /// <param name="identity">User of the credential to be created.
        /// </param>
        /// <param name="secret">Password of the credential to be created.
        /// </param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public CreateCredentialRequest( string credential_name,
                                        string type,
                                        string identity,
                                        string secret,
                                        IDictionary<string, string> options = null)
        {
            this.credential_name = credential_name ?? "";
            this.type = type ?? "";
            this.identity = identity ?? "";
            this.secret = secret ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class CreateCredentialRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createCredential(CreateCredentialRequest)">Kinetica.createCredential</see>.
    /// </summary>
    public class CreateCredentialResponse : KineticaData
    {
        /// <summary>Value of <see
        /// cref="CreateCredentialRequest.credential_name">credential_name</see>.
        /// </summary>
        public string credential_name { get; set; }

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class CreateCredentialResponse
} // end namespace kinetica
