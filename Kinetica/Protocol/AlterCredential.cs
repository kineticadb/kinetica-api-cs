/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterCredential(string,IDictionary{string, string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// Alter the properties of an existing <a
    /// href="../../../concepts/credentials/"
    /// target="_top">credential</a>.</summary>
    public class AlterCredentialRequest : KineticaData
    {

        /// <summary>Map containing the properties of the credential to be
        /// updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.TYPE">TYPE</see>:</term>
        ///         <description>New type for the credential.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_AD">AZURE_AD</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.DOCKER">DOCKER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.HDFS">HDFS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.KAFKA">KAFKA</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.IDENTITY">IDENTITY</see>:</term>
        ///         <description>New user for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.SECRET">SECRET</see>:</term>
        ///         <description>New password for the credential</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see
        /// cref="credential_updates_map" />.</summary>
        public struct CredentialUpdatesMap
        {

            /// <summary>New type for the credential.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_AD">AZURE_AD</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.DOCKER">DOCKER</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.HDFS">HDFS</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterCredentialRequest.CredentialUpdatesMap.KAFKA">KAFKA</see></term>
            ///     </item>
            /// </list></summary>
            public const string TYPE = "type";
            public const string AWS_ACCESS_KEY = "aws_access_key";
            public const string AWS_IAM_ROLE = "aws_iam_role";
            public const string AZURE_AD = "azure_ad";
            public const string AZURE_OAUTH = "azure_oauth";
            public const string AZURE_SAS = "azure_sas";
            public const string AZURE_STORAGE_KEY = "azure_storage_key";
            public const string DOCKER = "docker";
            public const string HDFS = "hdfs";
            public const string KAFKA = "kafka";

            /// <summary>New user for the credential</summary>
            public const string IDENTITY = "identity";

            /// <summary>New password for the credential</summary>
            public const string SECRET = "secret";
        } // end struct CredentialUpdatesMap


        /// <summary>Name of the credential to be altered. Must be an existing
        /// credential.  </summary>
        public string credential_name { get; set; }

        /// <summary>Map containing the properties of the credential to be
        /// updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.TYPE">TYPE</see>:</term>
        ///         <description>New type for the credential.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_AD">AZURE_AD</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.DOCKER">DOCKER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.HDFS">HDFS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.KAFKA">KAFKA</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.IDENTITY">IDENTITY</see>:</term>
        ///         <description>New user for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.SECRET">SECRET</see>:</term>
        ///         <description>New password for the credential</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> credential_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.  </summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterCredentialRequest object with default
        /// parameters.</summary>
        public AlterCredentialRequest() { }

        /// <summary>Constructs an AlterCredentialRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="credential_name">Name of the credential to be altered.
        /// Must be an existing credential.  </param>
        /// <param name="credential_updates_map">Map containing the properties
        /// of the credential to be updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.TYPE">TYPE</see>:</term>
        ///         <description>New type for the credential.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_ACCESS_KEY">AWS_ACCESS_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AWS_IAM_ROLE">AWS_IAM_ROLE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_AD">AZURE_AD</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_OAUTH">AZURE_OAUTH</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_SAS">AZURE_SAS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.AZURE_STORAGE_KEY">AZURE_STORAGE_KEY</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.DOCKER">DOCKER</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.HDFS">HDFS</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.KAFKA">KAFKA</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.IDENTITY">IDENTITY</see>:</term>
        ///         <description>New user for the credential</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterCredentialRequest.CredentialUpdatesMap.SECRET">SECRET</see>:</term>
        ///         <description>New password for the credential</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// <param name="options">Optional parameters.  </param>
        /// 
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
    /// cref="Kinetica.alterCredential(string,IDictionary{string, string},IDictionary{string, string})"
    /// />.</summary>
    public class AlterCredentialResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="AlterCredentialRequest.credential_name" />.  </summary>
        public string credential_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterCredentialResponse




}  // end namespace kinetica
