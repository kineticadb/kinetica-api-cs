namespace kinetica.FileSystem;

/// <summary>
/// Options for downloading files from KiFS.
/// </summary>
public class DownloadOptions
{
    /// <summary>
    /// Gets or sets whether to overwrite existing files on the local file system.
    /// Default is true.
    /// </summary>
    public bool OverwriteExisting { get; set; } = true;

    /// <summary>
    /// Creates a new DownloadOptions with default values.
    /// </summary>
    public DownloadOptions()
    {
    }

    /// <summary>
    /// Creates a new DownloadOptions with the specified values.
    /// </summary>
    /// <param name="overwriteExisting">Whether to overwrite existing files.</param>
    public DownloadOptions(bool overwriteExisting)
    {
        OverwriteExisting = overwriteExisting;
    }

    /// <summary>
    /// Returns the default download options.
    /// </summary>
    public static DownloadOptions Default => new DownloadOptions(true);
}
