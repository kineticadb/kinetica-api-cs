namespace kinetica.FileSystem;

/// <summary>
/// Interface for receiving callbacks during file upload operations.
/// Implement this interface to receive notifications about upload progress.
/// </summary>
public interface IFileUploadListener
{
    /// <summary>
    /// Called when all parts of a multi-part upload have been completed.
    /// </summary>
    /// <param name="results">List of results for each part of the upload.</param>
    void OnMultiPartUploadComplete(IList<FileOperationResult> results);

    /// <summary>
    /// Called when a single part of a multi-part upload has been completed.
    /// This is useful for tracking progress of large file uploads.
    /// </summary>
    /// <param name="result">Result of the part upload.</param>
    void OnPartUpload(FileOperationResult result);

    /// <summary>
    /// Called when a full file upload (non-multi-part) has been completed.
    /// </summary>
    /// <param name="result">Result of the file upload.</param>
    void OnFullFileUpload(FileOperationResult result);
}
