/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.createDirectory(string,IDictionary{string, string})"
    /// />.
    /// <br />
    /// Creates a new directory in <a href="../../../tools/kifs/"
    /// target="_top">KiFS</a>. The new
    /// directory serves as a location in which the user can upload files using
    /// <see
    /// cref="Kinetica.uploadFiles(IList{string},IList{byte[]},IDictionary{string, string})"
    /// />.</summary>
    public class CreateDirectoryRequest : KineticaData
    {

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, does not return an error if
        /// the directory already exists
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.
        /// A set of string constants for the parameter <see cref="options"
        /// />.</summary>
        public struct Options
        {

            /// <summary>If <i>true</i>, does not return an error if the
            /// directory already exists
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="CreateDirectoryRequest.Options.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see></term>
            ///     </item>
            /// </list>
            /// The default value is <see
            /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see>.</summary>
            public const string NO_ERROR_IF_EXISTS = "no_error_if_exists";
            public const string TRUE = "true";
            public const string FALSE = "false";
        } // end struct Options


        /// <summary>Name of the directory in KiFS to be created.  </summary>
        public string directory_name { get; set; }

        /// <summary>Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, does not return an error if
        /// the directory already exists
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs a CreateDirectoryRequest object with default
        /// parameters.</summary>
        public CreateDirectoryRequest() { }

        /// <summary>Constructs a CreateDirectoryRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="directory_name">Name of the directory in KiFS to be
        /// created.  </param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.NO_ERROR_IF_EXISTS">NO_ERROR_IF_EXISTS</see>:</term>
        ///         <description>If <i>true</i>, does not return an error if
        /// the directory already exists
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see></term>
        ///     </item>
        /// </list>
        /// The default value is <see
        /// cref="CreateDirectoryRequest.Options.FALSE">FALSE</see>.</description>
        ///     </item>
        /// </list>
        /// The default value is an empty {@link Dictionary}.</param>
        /// 
        public CreateDirectoryRequest( string directory_name,
                                       IDictionary<string, string> options = null)
        {
            this.directory_name = directory_name ?? "";
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class CreateDirectoryRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.createDirectory(string,IDictionary{string, string})"
    /// />.</summary>
    public class CreateDirectoryResponse : KineticaData
    {

        /// <summary>Value of <paramref
        /// cref="CreateDirectoryRequest.directory_name" />.  </summary>
        public string directory_name { get; set; }

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class CreateDirectoryResponse




}  // end namespace kinetica
