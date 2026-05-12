namespace kinetica.FileSystem;

/// <summary>
/// Options for uploading files to KiFS.
    /// </summary>
    public class UploadOptions
    {
        /// <summary>
        /// Gets or sets whether file search should be recursive along a directory hierarchy.
        /// This is applicable when file names contain glob patterns like '*.txt' or '**/*.csv'.
        /// Default is true.
        /// </summary>
        public bool Recursive { get; set; } = true;

        /// <summary>
        /// Gets or sets the time-to-live (TTL) value for uploaded files in minutes.
        /// A value of -1 means no expiration.
        /// Default is -1.
        /// </summary>
        public int Ttl { get; set; } = -1;

        /// <summary>
        /// Gets or sets whether to delete existing files before uploading.
        /// Default is false.
        /// </summary>
        public bool DeleteIfExists { get; set; } = false;

        /// <summary>
        /// Creates a new UploadOptions with default values.
        /// </summary>
        public UploadOptions()
        {
        }

        /// <summary>
        /// Creates a new UploadOptions with the specified values.
        /// </summary>
        /// <param name="recursive">Whether file search should be recursive.</param>
        /// <param name="ttl">TTL value for uploaded files in minutes.</param>
        public UploadOptions(bool recursive, int ttl)
        {
            Recursive = recursive;
            Ttl = ttl;
        }

        /// <summary>
        /// Returns the default upload options.
        /// </summary>
        public static UploadOptions Default => new UploadOptions(true, -1);
    }
