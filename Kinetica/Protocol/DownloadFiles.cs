/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.downloadFiles(DownloadFilesRequest)">Kinetica.downloadFiles</see>.
    /// </summary>
    /// <remarks><para>Downloads one or more files from <a
    /// href="../../../tools/kifs/" target="_top">KiFS</a>.</para></remarks>
    public class DownloadFilesRequest : KineticaData
    {
        /// <summary>A set of string constants for the parameter <see
        /// cref="options" />.</summary>
        /// <remarks><para>Optional parameters.</para></remarks>
        public struct Options
        {
            /// <summary>Encoding to be applied to the output file data.
            /// </summary>
            /// <remarks><para>Supported values:</para>
            /// <list type="bullet">
            ///     <item>
            ///         <term><see cref="Options.BASE64">BASE64</see>:</term>
            ///         <description>Apply base64 encoding to the output file
            ///         data.</description>
            ///     </item>
            ///     <item>
            ///         <term><see cref="Options.NONE">NONE</see>:</term>
            ///         <description>Do not apply any encoding to the output
            ///         file data.</description>
            ///     </item>
            /// </list>
            /// <para>The default value is <see cref="Options.NONE">NONE</see>.
            /// </para></remarks>
            public const string FILE_ENCODING = "file_encoding";

            /// <summary>Apply base64 encoding to the output file data.
            /// </summary>
            public const string BASE64 = "base64";

            /// <summary>Do not apply any encoding to the output file data.
            /// </summary>
            public const string NONE = "none";
        } // end struct Options

        /// <summary>An array of the file names to download from KiFS.
        /// </summary>
        /// <remarks><para>File paths may contain wildcard characters after the
        /// KiFS directory delimeter.</para>
        /// <para>Accepted wildcard characters are asterisk (*) to represent
        /// any string of zero or more characters, and question mark (?) to
        /// indicate a single character.</para></remarks>
        public IList<string> file_names { get; set; } = new List<string>();

        /// <summary>An array of starting byte offsets from which to read each
        /// respective file in <see cref="file_names" />.</summary>
        /// <remarks><para>Must either be empty or the same length as <see
        /// cref="file_names" />. If empty, files are downloaded in their
        /// entirety. If not empty, <see cref="read_lengths" /> must also not
        /// be empty.</para></remarks>
        public IList<long> read_offsets { get; set; } = new List<long>();

        /// <summary>Array of number of bytes to read from each respective file
        /// in <see cref="file_names" />.</summary>
        /// <remarks><para>Must either be empty or the same length as <see
        /// cref="file_names" />. If empty, files are downloaded in their
        /// entirety. If not empty, <see cref="read_offsets" /> must also not
        /// be empty.</para></remarks>
        public IList<long> read_lengths { get; set; } = new List<long>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.FILE_ENCODING">FILE_ENCODING</see>:</term>
        ///         <description>Encoding to be applied to the output file
        ///         data. When using JSON serialization it is recommended to
        ///         specify this as <see cref="Options.BASE64">BASE64</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.BASE64">BASE64</see>:
        ///                 </term>
        ///                 <description>Apply base64 encoding to the output
        ///                 file data.</description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.NONE">NONE</see>:</term>
        ///                 <description>Do not apply any encoding to the
        ///                 output file data.</description>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.NONE">NONE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>The default value is an empty Dictionary.</para></remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs a DownloadFilesRequest object with default
        /// parameters.</summary>
        public DownloadFilesRequest() { }

        /// <summary>Constructs a DownloadFilesRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="file_names">An array of the file names to download
        /// from KiFS. File paths may contain wildcard characters after the
        /// KiFS directory delimeter.
        /// Accepted wildcard characters are asterisk (*) to represent any
        /// string of zero or more characters, and question mark (?) to
        /// indicate a single character.</param>
        /// <param name="read_offsets">An array of starting byte offsets from
        /// which to read each respective file in <paramref name="file_names"
        /// />. Must either be empty or the same length as <paramref
        /// name="file_names" />. If empty, files are downloaded in their
        /// entirety. If not empty, <paramref name="read_lengths" /> must also
        /// not be empty.</param>
        /// <param name="read_lengths">Array of number of bytes to read from
        /// each respective file in <paramref name="file_names" />. Must either
        /// be empty or the same length as <paramref name="file_names" />. If
        /// empty, files are downloaded in their entirety. If not empty,
        /// <paramref name="read_offsets" /> must also not be empty.</param>
        /// <param name="options">Optional parameters.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        ///         cref="Options.FILE_ENCODING">FILE_ENCODING</see>:</term>
        ///         <description>Encoding to be applied to the output file
        ///         data. When using JSON serialization it is recommended to
        ///         specify this as <see cref="Options.BASE64">BASE64</see>.
        ///         Supported values:
        ///         <list type="bullet">
        ///             <item>
        ///                 <term><see cref="Options.BASE64">BASE64</see>:
        ///                 </term>
        ///                 <description>Apply base64 encoding to the output
        ///                 file data.</description>
        ///             </item>
        ///             <item>
        ///                 <term><see cref="Options.NONE">NONE</see>:</term>
        ///                 <description>Do not apply any encoding to the
        ///                 output file data.</description>
        ///             </item>
        ///         </list>
        ///         The default value is <see cref="Options.NONE">NONE</see>.
        ///         </description>
        ///     </item>
        /// </list>
        /// The default value is an empty Dictionary.</param>
        public DownloadFilesRequest( IList<string> file_names,
                                     IList<long> read_offsets,
                                     IList<long> read_lengths,
                                     IDictionary<string, string> options = null)
        {
            this.file_names = file_names ?? new List<string>();
            this.read_offsets = read_offsets ?? new List<long>();
            this.read_lengths = read_lengths ?? new List<long>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class DownloadFilesRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.downloadFiles(DownloadFilesRequest)">Kinetica.downloadFiles</see>.
    /// </summary>
    public class DownloadFilesResponse : KineticaData
    {
        /// <summary>Names of the files downloaded from KiFS</summary>
        public IList<string> file_names { get; set; } = new List<string>();

        /// <summary>Data for the respective downloaded files listed in <see
        /// cref="file_names" /></summary>
        public IList<byte[]> file_data { get; set; } = new List<byte[]>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class DownloadFilesResponse
} // end namespace kinetica
