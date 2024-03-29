/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.showFiles(ShowFilesRequest)">Kinetica.showFiles</see>.
    /// </summary>
    /// <remarks><para>Shows information about files in <a
    /// href="../../../tools/kifs/" target="_top">KiFS</a>. Can be used for
    /// individual files, or to show all files in a given directory.</para>
    /// </remarks>
    public class ShowFilesRequest : KineticaData
    {
        /// <summary>File paths to show.</summary>
        /// <remarks><para>Each path can be a KiFS directory name, or a full
        /// path to a KiFS file. File paths may contain wildcard characters
        /// after the KiFS directory delimeter.</para>
        /// <para>Accepted wildcard characters are asterisk (*) to represent
        /// any string of zero or more characters, and question mark (?) to
        /// indicate a single character.</para></remarks>
        public IList<string> paths { get; set; } = new List<string>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a ShowFilesRequest object with default
        /// parameters.</summary>
        public ShowFilesRequest() { }

        /// <summary>Constructs a ShowFilesRequest object with the specified
        /// parameters.</summary>
        ///
        /// <param name="paths">File paths to show. Each path can be a KiFS
        /// directory name, or a full path to a KiFS file. File paths may
        /// contain wildcard characters after the KiFS directory delimeter.
        /// Accepted wildcard characters are asterisk (*) to represent any
        /// string of zero or more characters, and question mark (?) to
        /// indicate a single character.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public ShowFilesRequest( IList<string> paths,
                                 IDictionary<string, string> options = null)
        {
            this.paths = paths ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class ShowFilesRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.showFiles(ShowFilesRequest)">Kinetica.showFiles</see>.
    /// </summary>
    public class ShowFilesResponse : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="info" />.</summary>
        /// <remarks><para>Additional information.</para></remarks>
        public struct Info
        {
            /// <summary>JSON-encoded information about multipart uploads in
            /// progress</summary>
            public const string MULTIPART_UPLOADS = "multipart_uploads";
        } // end struct Info

        /// <summary>A listing of files in the paths specified</summary>
        public IList<string> file_names { get; set; } = new List<string>();

        /// <summary>Size of each file, in bytes</summary>
        public IList<long> sizes { get; set; } = new List<long>();

        /// <summary>User that created the file</summary>
        public IList<string> users { get; set; } = new List<string>();

        /// <summary>Creation time for each file, in milliseconds since epoch
        /// </summary>
        public IList<long> creation_times { get; set; } = new List<long>();

        /// <summary>Additional information.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Info.MULTIPART_UPLOADS">MULTIPART_UPLOADS</see>:
        ///         </term>
        ///         <description>JSON-encoded information about multipart
        ///         uploads in progress</description>
        ///     </item>
        /// </list></remarks>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class ShowFilesResponse
} // end namespace kinetica
