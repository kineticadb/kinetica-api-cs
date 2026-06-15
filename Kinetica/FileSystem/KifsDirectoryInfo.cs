namespace kinetica.FileSystem;

/// <summary>
/// Contains information about a directory in KiFS.
    /// </summary>
    public class KifsDirectoryInfo
    {
        /// <summary>
        /// Gets or sets the full KiFS directory path.
        /// </summary>
        public string KifsPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the user who created the directory.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation time as a Unix timestamp in milliseconds.
        /// </summary>
        public long CreationTime { get; set; }

        /// <summary>
        /// Gets the creation time as a DateTime object.
        /// </summary>
        public DateTime CreationDateTime => DateTimeOffset.FromUnixTimeMilliseconds(CreationTime).DateTime;

        /// <summary>
        /// Gets or sets the permission string for the directory.
        /// </summary>
        public string Permission { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional information about the directory.
        /// </summary>
        public IDictionary<string, string> Info { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Returns a string representation of this directory info.
        /// </summary>
        public override string ToString()
        {
            return $"KifsDirectoryInfo {{ KifsPath='{KifsPath}', CreatedBy='{CreatedBy}', CreationTime={CreationTime}, Permission='{Permission}' }}";
        }
    }
