/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;

namespace kinetica
{
    /// <summary>A set of parameters for <see
    /// cref="Kinetica.uploadFilesFromurl(UploadFilesFromurlRequest)">Kinetica.uploadFilesFromurl</see>.
    /// </summary>
    /// <remarks><para>Uploads one or more files to <a
    /// href="../../../tools/kifs/" target="_top">KiFS</a>.</para>
    /// <para>Each file path must reside in a top-level KiFS directory, i.e.
    /// one of the directories listed in <see
    /// cref="Kinetica.showDirectories(ShowDirectoriesRequest)">Kinetica.showDirectories</see>.
    /// The user must have write permission on the directory. Nested
    /// directories are permitted in file name paths. Directories are
    /// deliniated with the directory separator of '/'.  For example, given the
    /// file path '/a/b/c/d.txt', 'a' must be a KiFS directory.</para>
    /// <para>These characters are allowed in file name paths: letters,
    /// numbers, spaces, the path delimiter of '/', and the characters: '.' '-'
    /// ':' '[' ']' '(' ')' '#' '='.</para></remarks>
    public class UploadFilesFromurlRequest : KineticaData
    {
        /// <summary>An array of full file name paths to be used for the files
        /// uploaded to KiFS.</summary>
        /// <remarks><para>File names may have any number of nested directories
        /// in their paths, but the top-level directory must be an existing
        /// KiFS directory. Each file must reside in or under a top-level
        /// directory. A full file name path cannot be larger than 1024
        /// characters.</para></remarks>
        public IList<string> file_names { get; set; } = new List<string>();

        /// <summary>List of URLs to upload, for each respective file in <see
        /// cref="file_names" />.</summary>
        public IList<string> urls { get; set; } = new List<string>();

        /// <summary>Optional parameters.</summary>
        /// <remarks><para>The default value is an empty Dictionary.</para>
        /// </remarks>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        /// <summary>Constructs an UploadFilesFromurlRequest object with
        /// default parameters.</summary>
        public UploadFilesFromurlRequest() { }

        /// <summary>Constructs an UploadFilesFromurlRequest object with the
        /// specified parameters.</summary>
        ///
        /// <param name="file_names">An array of full file name paths to be
        /// used for the files uploaded to KiFS. File names may have any number
        /// of nested directories in their paths, but the top-level directory
        /// must be an existing KiFS directory. Each file must reside in or
        /// under a top-level directory. A full file name path cannot be larger
        /// than 1024 characters.</param>
        /// <param name="urls">List of URLs to upload, for each respective file
        /// in <paramref name="file_names" />.</param>
        /// <param name="options">Optional parameters. The default value is an
        /// empty Dictionary.</param>
        public UploadFilesFromurlRequest( IList<string> file_names,
                                          IList<string> urls,
                                          IDictionary<string, string> options = null)
        {
            this.file_names = file_names ?? new List<string>();
            this.urls = urls ?? new List<string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor
    } // end class UploadFilesFromurlRequest

    /// <summary>A set of results returned by <see
    /// cref="Kinetica.uploadFilesFromurl(UploadFilesFromurlRequest)">Kinetica.uploadFilesFromurl</see>.
    /// </summary>
    public class UploadFilesFromurlResponse : KineticaData
    {
        /// <summary>List of <see
        /// cref="UploadFilesFromurlRequest.file_names">file_names</see> that
        /// were successfully uploaded.</summary>
        public IList<string> successful_file_names { get; set; } = new List<string>();

        /// <summary>List of <see
        /// cref="UploadFilesFromurlRequest.urls">urls</see> that were
        /// successfully uploaded.</summary>
        public IList<string> successful_urls { get; set; } = new List<string>();

        /// <summary>Additional information.</summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();
    } // end class UploadFilesFromurlResponse
} // end namespace kinetica
