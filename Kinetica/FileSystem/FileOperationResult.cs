using System;
using System.Collections.Generic;

namespace kinetica.FileSystem;

/// <summary>
/// Indicates the type of file operation.
    /// </summary>
    public enum OperationMode
    {
        /// <summary>Upload operation.</summary>
        Upload,
        /// <summary>Download operation.</summary>
        Download
    }

    /// <summary>
    /// Contains the result of a file upload or download operation.
    /// This is used for callback notifications during file transfers.
    /// </summary>
    public class FileOperationResult
    {
        /// <summary>
        /// Gets or sets the name of the file being uploaded/downloaded.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of all file names for full file operations.
        /// </summary>
        public IList<string> FileNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets whether the operation was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the exception that occurred, if any.
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the error message, if any.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the operation mode (upload or download).
        /// </summary>
        public OperationMode OpMode { get; set; }

        /// <summary>
        /// Gets or sets whether this is a multi-part operation.
        /// </summary>
        public bool IsMultiPart { get; set; }

        /// <summary>
        /// Gets or sets information about a multi-part upload operation.
        /// </summary>
        public MultiPartUploadInfo? UploadInfo { get; set; }

        /// <summary>
        /// Gets or sets information about a multi-part download operation.
        /// </summary>
        public MultiPartDownloadInfo? DownloadInfo { get; set; }

        /// <summary>
        /// Returns a string representation of this result.
        /// </summary>
        public override string ToString()
        {
            return $"FileOperationResult {{ FileName='{FileName}', IsSuccessful={IsSuccessful}, OpMode={OpMode}, IsMultiPart={IsMultiPart}, ErrorMessage='{ErrorMessage}' }}";
        }
    }

    /// <summary>
    /// Contains information about a multi-part upload operation.
    /// </summary>
    public class MultiPartUploadInfo
    {
        /// <summary>
        /// Gets or sets the UUID for the multi-part upload.
        /// </summary>
        public string UploadUuid { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current part number.
        /// </summary>
        public int PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of parts.
        /// </summary>
        public int TotalParts { get; set; }

        /// <summary>
        /// Gets or sets the size of the current part in bytes.
        /// </summary>
        public long PartSize { get; set; }
    }

    /// <summary>
    /// Contains information about a multi-part download operation.
    /// </summary>
    public class MultiPartDownloadInfo
    {
        /// <summary>
        /// Gets or sets the current part number.
        /// </summary>
        public int PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of parts.
        /// </summary>
        public int TotalParts { get; set; }

        /// <summary>
        /// Gets or sets the size of the current part in bytes.
        /// </summary>
        public long PartSize { get; set; }

        /// <summary>
        /// Gets or sets the offset in the file where this part starts.
        /// </summary>
        public long Offset { get; set; }
    }
