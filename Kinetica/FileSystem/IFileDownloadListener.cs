namespace kinetica.FileSystem;

/// <summary>
/// Interface for receiving callbacks during file download operations.
/// Implement this interface to receive notifications about download progress.
/// </summary>
public interface IFileDownloadListener
{
    /// <summary>
    /// Called when all parts of a multi-part download have been completed.
    /// </summary>
    /// <param name="results">List of results for each part of the download.</param>
    void OnMultiPartDownloadComplete(IList<FileOperationResult> results);

    /// <summary>
    /// Called when a single part of a multi-part download has been completed.
    /// This is useful for tracking progress of large file downloads.
    /// </summary>
    /// <param name="result">Result of the part download.</param>
    void OnPartDownload(FileOperationResult result);

    /// <summary>
    /// Called when one or more full file downloads (non-multi-part) have been completed.
    /// </summary>
    /// <param name="fileNames">List of file names that were downloaded.</param>
    void OnFullFileDownload(IList<string> fileNames);
}
