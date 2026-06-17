namespace kinetica.FileSystem;

/// <summary>
/// Contains information about a file stored in KiFS.
/// </summary>
public class KifsFileInfo
{
    /// <summary>
    /// Gets or sets the full KiFS file name path.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the username of the user who created the file.
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
    /// Gets or sets additional information about the file.
    /// </summary>
    public IDictionary<string, string> Info { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Returns a string representation of this file info.
    /// </summary>
    public override string ToString()
    {
        return $"KifsFileInfo {{ FileName='{FileName}', FileSize={FileSize}, CreatedBy='{CreatedBy}', CreationTime={CreationTime} }}";
    }
}
