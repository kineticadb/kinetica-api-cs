using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace kinetica.FileSystem;

/// <summary>
/// Main class for handling file operations with Kinetica's KiFS (Kinetica File System).
/// Provides methods to upload, download, and manage files and directories in KiFS.
/// </summary>
/// <remarks>
/// <para>This class exposes the KiFS API for file operations including:</para>
/// <list type="bullet">
///     <item><description>Upload files to KiFS</description></item>
///     <item><description>Download files from KiFS</description></item>
///     <item><description>Create and delete directories</description></item>
///     <item><description>List files and directories</description></item>
///     <item><description>Check existence of files and directories</description></item>
/// </list>
/// <para>Supported file path patterns include standard Glob syntax (e.g., *.csv, data/**, file_?.txt).</para>
/// </remarks>
public class KineticaFileHandler
{
    /// <summary>
    /// Separator character between a KiFS directory and file name.
    /// </summary>
    public const string KifsPathSeparator = "/";

    /// <summary>
    /// Prefix to use when referencing KiFS files (e.g., for file ingest).
    /// </summary>
    public const string KifsPathPrefix = "kifs://";

    /// <summary>
    /// Alias for a user's home directory.
    /// </summary>
    public const string RemoteUserHomeDirPrefix = "~";

    /// <summary>
    /// Default file size threshold for multi-part uploads (60 MB).
    /// Files larger than this will be uploaded in parts.
    /// </summary>
    public const long DefaultFileSizeToSplit = 62914560; // 60 MB

    /// <summary>
    /// Default thread pool size for file operations.
    /// </summary>
    public const int DefaultThreadPoolSize = 5;

    private readonly Kinetica _db;
    private readonly Options _options;

    /// <summary>
    /// Constructs a KineticaFileHandler with the specified Kinetica connection.
    /// </summary>
    /// <param name="db">The Kinetica instance used to access KiFS.</param>
    public KineticaFileHandler(Kinetica db) : this(db, new Options())
    {
    }

    /// <summary>
    /// Constructs a KineticaFileHandler with the specified Kinetica connection and options.
    /// </summary>
    /// <param name="db">The Kinetica instance used to access KiFS.</param>
    /// <param name="options">Options for configuring file transfer behavior.</param>
    public KineticaFileHandler(Kinetica db, Options options)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _options = options ?? new Options();
    }

    /// <summary>
    /// Gets the options for this file handler.
    /// </summary>
    public Options FileHandlerOptions => _options;

    #region Upload Methods

    /// <summary>
    /// Uploads a single file to a KiFS directory using default options.
    /// </summary>
    /// <param name="fileName">Name and path of the local file (or glob pattern) to upload.</param>
    /// <param name="remoteDirName">Name of the KiFS directory to upload to.</param>
    public void Upload(string fileName, string remoteDirName)
    {
        Upload(fileName, remoteDirName, UploadOptions.Default, null);
    }

    /// <summary>
    /// Uploads a single file to a KiFS directory.
    /// </summary>
    /// <param name="fileName">Name and path of the local file (or glob pattern) to upload.</param>
    /// <param name="remoteDirName">Name of the KiFS directory to upload to.</param>
    /// <param name="uploadOptions">Options for the upload operation.</param>
    /// <param name="callback">Optional callback for upload progress notifications.</param>
    public void Upload(string fileName, string remoteDirName, UploadOptions? uploadOptions, IFileUploadListener? callback)
    {
        Upload(new List<string> { fileName }, remoteDirName, uploadOptions, callback);
    }

    /// <summary>
    /// Uploads multiple files to a KiFS directory using default options.
    /// </summary>
    /// <param name="fileNames">Names and paths of the local files (or glob patterns) to upload.</param>
    /// <param name="remoteDirName">Name of the KiFS directory to upload to.</param>
    public void Upload(IList<string> fileNames, string remoteDirName)
    {
        Upload(fileNames, remoteDirName, UploadOptions.Default, null);
    }

    /// <summary>
    /// Uploads multiple files to a KiFS directory.
    /// </summary>
    /// <param name="fileNames">Names and paths of the local files (or glob patterns) to upload.</param>
    /// <param name="remoteDirName">Name of the KiFS directory to upload to.</param>
    /// <param name="uploadOptions">Options for the upload operation.</param>
    /// <param name="callback">Optional callback for upload progress notifications.</param>
    public void Upload(IList<string> fileNames, string remoteDirName, UploadOptions? uploadOptions, IFileUploadListener? callback)
    {
        if (fileNames == null || fileNames.Count == 0)
            throw new KineticaException("List of local files to upload cannot be null or empty");

        if (string.IsNullOrEmpty(remoteDirName))
            throw new KineticaException("Remote directory name cannot be null or empty");

        if (!KifsDirectoryExists(remoteDirName))
            throw new KineticaException($"Remote directory '{remoteDirName}' does not exist");

        uploadOptions ??= UploadOptions.Default;

        // Resolve glob patterns and validate files exist
        var resolvedFiles = new List<string>();
        foreach (var fileName in fileNames)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new KineticaException("File name cannot be null or empty");

            var files = ResolveLocalFiles(fileName, uploadOptions.Recursive);
            if (files.Count == 0)
                throw new KineticaException($"No files found matching pattern: {fileName}");

            resolvedFiles.AddRange(files);
        }

        // Separate files into full-file uploads and multi-part uploads based on size
        var fullFiles = new List<(string localPath, string remotePath)>();
        var multiPartFiles = new List<(string localPath, string remotePath)>();

        foreach (var localPath in resolvedFiles)
        {
            var fileInfo = new FileInfo(localPath);
            var remotePath = BuildRemotePath(remoteDirName, fileInfo.Name);

            if (fileInfo.Length <= _options.FileSizeToSplit)
            {
                fullFiles.Add((localPath, remotePath));
            }
            else
            {
                multiPartFiles.Add((localPath, remotePath));
            }
        }

        // Upload full files in batches
        if (fullFiles.Count > 0)
        {
            UploadFullFiles(fullFiles, uploadOptions, callback);
        }

        // Upload multi-part files
        foreach (var (localPath, remotePath) in multiPartFiles)
        {
            UploadMultiPartFile(localPath, remotePath, uploadOptions, callback);
        }
    }

    /// <summary>
    /// Uploads files asynchronously to a KiFS directory.
    /// </summary>
    /// <param name="fileNames">Names and paths of the local files (or glob patterns) to upload.</param>
    /// <param name="remoteDirName">Name of the KiFS directory to upload to.</param>
    /// <param name="uploadOptions">Options for the upload operation.</param>
    /// <param name="callback">Optional callback for upload progress notifications.</param>
    public async Task UploadAsync(IList<string> fileNames, string remoteDirName, UploadOptions? uploadOptions = null, IFileUploadListener? callback = null)
    {
        await Task.Run(() => Upload(fileNames, remoteDirName, uploadOptions, callback));
    }

    private void UploadFullFiles(IList<(string localPath, string remotePath)> files, UploadOptions options, IFileUploadListener? callback)
    {
        // Batch files for upload
        var batch = new List<string>();
        var batchData = new List<byte[]>();

        foreach (var (localPath, remotePath) in files)
        {
            try
            {
                var data = File.ReadAllBytes(localPath);
                batch.Add(remotePath);
                batchData.Add(data);

                // Upload batch when it reaches the threshold or total size limit
                if (batch.Count >= _options.MaxFilesPerBatch)
                {
                    ExecuteUploadBatch(batch, batchData, options, callback);
                    batch.Clear();
                    batchData.Clear();
                }
            }
            catch (IOException ex)
            {
                throw new KineticaException($"Error reading file '{localPath}': {ex.Message}", ex);
            }
        }

        // Upload remaining files
        if (batch.Count > 0)
        {
            ExecuteUploadBatch(batch, batchData, options, callback);
        }
    }

    private void ExecuteUploadBatch(IList<string> fileNames, IList<byte[]> fileData, UploadOptions options, IFileUploadListener? callback)
    {
        var requestOptions = new Dictionary<string, string>();

        if (options.Ttl > 0)
        {
            requestOptions["ttl"] = options.Ttl.ToString();
        }

        if (options.DeleteIfExists)
        {
            requestOptions[UploadFilesRequest.Options.DELETE_IF_EXISTS] = UploadFilesRequest.Options.TRUE;
        }

        var request = new UploadFilesRequest(fileNames, fileData, requestOptions);
        _db.uploadFiles(request);

        // Notify callback
        if (callback != null)
        {
            var result = new FileOperationResult
            {
                FileNames = fileNames,
                IsSuccessful = true,
                OpMode = OperationMode.Upload,
                IsMultiPart = false
            };
            callback.OnFullFileUpload(result);
        }
    }

    private void UploadMultiPartFile(string localPath, string remotePath, UploadOptions options, IFileUploadListener? callback)
    {
        var fileInfo = new FileInfo(localPath);
        var uuid = Guid.NewGuid().ToString();
        var partSize = _options.FileSizeToSplit;
        var totalParts = (int)Math.Ceiling((double)fileInfo.Length / partSize);
        var results = new List<FileOperationResult>();

        try
        {
            // Initialize multi-part upload
            var initOptions = new Dictionary<string, string>
            {
                [UploadFilesRequest.Options.MULTIPART_OPERATION] = UploadFilesRequest.Options.INIT,
                [UploadFilesRequest.Options.MULTIPART_UPLOAD_UUID] = uuid
            };

            if (options.DeleteIfExists)
            {
                initOptions[UploadFilesRequest.Options.DELETE_IF_EXISTS] = UploadFilesRequest.Options.TRUE;
            }

            var initRequest = new UploadFilesRequest(
                new List<string> { remotePath },
                new List<byte[]>(),
                initOptions);
            _db.uploadFiles(initRequest);

            // Upload parts
            using var stream = File.OpenRead(localPath);
            var buffer = new byte[partSize];

            for (int partNumber = 1; partNumber <= totalParts; partNumber++)
            {
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                var partData = new byte[bytesRead];
                Array.Copy(buffer, partData, bytesRead);

                var partOptions = new Dictionary<string, string>
                {
                    [UploadFilesRequest.Options.MULTIPART_OPERATION] = UploadFilesRequest.Options.UPLOAD_PART,
                    [UploadFilesRequest.Options.MULTIPART_UPLOAD_UUID] = uuid,
                    [UploadFilesRequest.Options.MULTIPART_UPLOAD_PART_NUMBER] = partNumber.ToString()
                };

                var partRequest = new UploadFilesRequest(
                    new List<string> { remotePath },
                    new List<byte[]> { partData },
                    partOptions);
                _db.uploadFiles(partRequest);

                // Notify callback
                var partResult = new FileOperationResult
                {
                    FileName = remotePath,
                    IsSuccessful = true,
                    OpMode = OperationMode.Upload,
                    IsMultiPart = true,
                    UploadInfo = new MultiPartUploadInfo
                    {
                        UploadUuid = uuid,
                        PartNumber = partNumber,
                        TotalParts = totalParts,
                        PartSize = bytesRead
                    }
                };
                results.Add(partResult);
                callback?.OnPartUpload(partResult);
            }

            // Complete multi-part upload
            var completeOptions = new Dictionary<string, string>
            {
                [UploadFilesRequest.Options.MULTIPART_OPERATION] = UploadFilesRequest.Options.COMPLETE,
                [UploadFilesRequest.Options.MULTIPART_UPLOAD_UUID] = uuid
            };

            var completeRequest = new UploadFilesRequest(
                new List<string> { remotePath },
                new List<byte[]>(),
                completeOptions);
            _db.uploadFiles(completeRequest);

            callback?.OnMultiPartUploadComplete(results);
        }
        catch (Exception ex)
        {
            // Cancel multi-part upload on error
            try
            {
                var cancelOptions = new Dictionary<string, string>
                {
                    [UploadFilesRequest.Options.MULTIPART_OPERATION] = UploadFilesRequest.Options.CANCEL,
                    [UploadFilesRequest.Options.MULTIPART_UPLOAD_UUID] = uuid
                };

                var cancelRequest = new UploadFilesRequest(
                    new List<string> { remotePath },
                    new List<byte[]>(),
                    cancelOptions);
                _db.uploadFiles(cancelRequest);
            }
            catch
            {
                // Ignore cancel errors
            }

            throw new KineticaException($"Failed to upload file '{localPath}': {ex.Message}", ex);
        }
    }

    #endregion

    #region Download Methods

    /// <summary>
    /// Downloads a single file from KiFS to a local directory using default options.
    /// </summary>
    /// <param name="fileName">Path of the KiFS file to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    public void Download(string fileName, string localDirName)
    {
        Download(fileName, localDirName, DownloadOptions.Default, null);
    }

    /// <summary>
    /// Downloads a single file from KiFS to a local directory.
    /// </summary>
    /// <param name="fileName">Path of the KiFS file to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    /// <param name="downloadOptions">Options for the download operation.</param>
    /// <param name="callback">Optional callback for download progress notifications.</param>
    public void Download(string fileName, string localDirName, DownloadOptions? downloadOptions, IFileDownloadListener? callback)
    {
        Download(new List<string> { fileName }, localDirName, downloadOptions, callback);
    }

    /// <summary>
    /// Downloads multiple files from KiFS to a local directory using default options.
    /// </summary>
    /// <param name="fileNames">Paths of the KiFS files to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    public void Download(IList<string> fileNames, string localDirName)
    {
        Download(fileNames, localDirName, DownloadOptions.Default, null);
    }

    /// <summary>
    /// Downloads multiple files from KiFS to a local directory.
    /// </summary>
    /// <param name="fileNames">Paths of the KiFS files to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    /// <param name="downloadOptions">Options for the download operation.</param>
    /// <param name="callback">Optional callback for download progress notifications.</param>
    public void Download(IList<string> fileNames, string localDirName, DownloadOptions? downloadOptions, IFileDownloadListener? callback)
    {
        if (fileNames == null || fileNames.Count == 0)
            throw new KineticaException("List of KiFS file names cannot be null or empty");

        if (string.IsNullOrEmpty(localDirName))
            throw new KineticaException("Local directory name cannot be null or empty");

        if (!Directory.Exists(localDirName))
            throw new KineticaException($"Local directory '{localDirName}' does not exist");

        downloadOptions ??= DownloadOptions.Default;

        // Download files
        var request = new DownloadFilesRequest(fileNames, new List<long>(), new List<long>());
        var response = _db.downloadFiles(request);

        // Write files to local directory
        var downloadedFiles = new List<string>();
        for (int i = 0; i < response.file_names.Count; i++)
        {
            var remoteName = response.file_names[i];
            var localFileName = Path.GetFileName(remoteName);
            var localPath = Path.Combine(localDirName, localFileName);

            if (File.Exists(localPath) && !downloadOptions.OverwriteExisting)
            {
                throw new KineticaException($"File '{localPath}' already exists and OverwriteExisting is false");
            }

            File.WriteAllBytes(localPath, response.file_data[i]);
            downloadedFiles.Add(localPath);
        }

        callback?.OnFullFileDownload(downloadedFiles);
    }

    /// <summary>
    /// Downloads all files in a KiFS directory to a local directory.
    /// </summary>
    /// <param name="remoteDirName">Path of the KiFS directory to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    /// <param name="downloadOptions">Options for the download operation.</param>
    /// <param name="callback">Optional callback for download progress notifications.</param>
    public void DownloadDirectory(string remoteDirName, string localDirName, DownloadOptions? downloadOptions = null, IFileDownloadListener? callback = null)
    {
        if (!Directory.Exists(localDirName))
            throw new KineticaException($"Local directory '{localDirName}' does not exist");

        var showFilesResponse = _db.showFiles(new List<string> { remoteDirName }, new Dictionary<string, string>());

        if (showFilesResponse.file_names.Count > 0)
        {
            Download(showFilesResponse.file_names, localDirName, downloadOptions, callback);
        }
    }

    /// <summary>
    /// Downloads files asynchronously from KiFS to a local directory.
    /// </summary>
    /// <param name="fileNames">Paths of the KiFS files to download.</param>
    /// <param name="localDirName">Path of the local directory to download to.</param>
    /// <param name="downloadOptions">Options for the download operation.</param>
    /// <param name="callback">Optional callback for download progress notifications.</param>
    public async Task DownloadAsync(IList<string> fileNames, string localDirName, DownloadOptions? downloadOptions = null, IFileDownloadListener? callback = null)
    {
        await Task.Run(() => Download(fileNames, localDirName, downloadOptions, callback));
    }

    #endregion

    #region Directory Operations

    /// <summary>
    /// Creates a KiFS directory, suppressing error if it already exists.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory to create.</param>
    public void CreateDirectory(string remoteDirName)
    {
        CreateDirectory(remoteDirName, true);
    }

    /// <summary>
    /// Creates a KiFS directory.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory to create.</param>
    /// <param name="noErrorIfExists">Whether to suppress error if directory already exists.</param>
    public void CreateDirectory(string remoteDirName, bool noErrorIfExists)
    {
        var options = new Dictionary<string, string>
        {
            [CreateDirectoryRequest.Options.NO_ERROR_IF_EXISTS] = noErrorIfExists
                ? CreateDirectoryRequest.Options.TRUE
                : CreateDirectoryRequest.Options.FALSE
        };

        _db.createDirectory(remoteDirName, options);
    }

    /// <summary>
    /// Deletes a KiFS directory and all files under it, suppressing error if it doesn't exist.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory to delete.</param>
    public void DeleteDirectory(string remoteDirName)
    {
        DeleteDirectory(remoteDirName, true, true);
    }

    /// <summary>
    /// Deletes a KiFS directory.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory to delete.</param>
    /// <param name="recursive">Whether to delete all files and subdirectories.</param>
    /// <param name="noErrorIfNotExists">Whether to suppress error if directory doesn't exist.</param>
    public void DeleteDirectory(string remoteDirName, bool recursive, bool noErrorIfNotExists)
    {
        var options = new Dictionary<string, string>
        {
            [DeleteDirectoryRequest.Options.RECURSIVE] = recursive
                ? DeleteDirectoryRequest.Options.TRUE
                : DeleteDirectoryRequest.Options.FALSE,
            [DeleteDirectoryRequest.Options.NO_ERROR_IF_NOT_EXISTS] = noErrorIfNotExists
                ? DeleteDirectoryRequest.Options.TRUE
                : DeleteDirectoryRequest.Options.FALSE
        };

        _db.deleteDirectory(remoteDirName, options);
    }

    /// <summary>
    /// Returns statistics about the given KiFS directories.
    /// </summary>
    /// <param name="remoteDirNames">List of KiFS directory names.</param>
    /// <returns>List of KifsDirectoryInfo objects.</returns>
    public IList<KifsDirectoryInfo> ShowDirectories(IList<string> remoteDirNames)
    {
        if (remoteDirNames == null || remoteDirNames.Count == 0)
            throw new KineticaException("List of KiFS directory names cannot be null or empty");

        var result = new List<KifsDirectoryInfo>();

        foreach (var dirName in remoteDirNames)
        {
            if (string.IsNullOrEmpty(dirName?.Trim()))
                throw new KineticaException("Directory name cannot be null or empty");

            var response = _db.showDirectories(dirName, new Dictionary<string, string>());

            for (int i = 0; i < response.directories.Count; i++)
            {
                result.Add(new KifsDirectoryInfo
                {
                    KifsPath = response.directories[i],
                    CreatedBy = response.users[i],
                    Permission = response.permissions[i],
                    CreationTime = response.creation_times[i]
                });
            }
        }

        return result;
    }

    /// <summary>
    /// Returns statistics about a single KiFS directory.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory.</param>
    /// <returns>List of KifsDirectoryInfo objects.</returns>
    public IList<KifsDirectoryInfo> ShowDirectory(string remoteDirName)
    {
        if (string.IsNullOrEmpty(remoteDirName))
            throw new KineticaException("KiFS directory name cannot be null or empty");

        var response = _db.showDirectories(remoteDirName, new Dictionary<string, string>());
        var result = new List<KifsDirectoryInfo>();

        for (int i = 0; i < response.directories.Count; i++)
        {
            result.Add(new KifsDirectoryInfo
            {
                KifsPath = response.directories[i],
                CreatedBy = response.users[i],
                Permission = response.permissions[i],
                CreationTime = response.creation_times[i]
            });
        }

        return result;
    }

    /// <summary>
    /// Returns statistics about all KiFS directories.
    /// </summary>
    /// <returns>List of KifsDirectoryInfo objects.</returns>
    public IList<KifsDirectoryInfo> ShowAllDirectories()
    {
        var response = _db.showDirectories("", new Dictionary<string, string>());
        var result = new List<KifsDirectoryInfo>();

        for (int i = 0; i < response.directories.Count; i++)
        {
            result.Add(new KifsDirectoryInfo
            {
                KifsPath = response.directories[i],
                CreatedBy = response.users[i],
                Permission = response.permissions[i],
                CreationTime = response.creation_times[i]
            });
        }

        return result;
    }

    #endregion

    #region File Operations

    /// <summary>
    /// Deletes files from KiFS, suppressing error if files don't exist.
    /// </summary>
    /// <param name="fileNames">List of KiFS file paths to delete.</param>
    public void DeleteFiles(IList<string> fileNames)
    {
        DeleteFiles(fileNames, true);
    }

    /// <summary>
    /// Deletes files from KiFS.
    /// </summary>
    /// <param name="fileNames">List of KiFS file paths to delete.</param>
    /// <param name="noErrorIfNotExists">Whether to suppress error if files don't exist.</param>
    public void DeleteFiles(IList<string> fileNames, bool noErrorIfNotExists)
    {
        var options = new Dictionary<string, string>
        {
            [DeleteFilesRequest.Options.NO_ERROR_IF_NOT_EXISTS] = noErrorIfNotExists
                ? DeleteFilesRequest.Options.TRUE
                : DeleteFilesRequest.Options.FALSE
        };

        _db.deleteFiles(fileNames, options);
    }

    /// <summary>
    /// Deletes all files in a KiFS directory.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory containing files to delete.</param>
    public void DeleteFilesInDirectory(string remoteDirName)
    {
        DeleteFilesInDirectory(remoteDirName, true);
    }

    /// <summary>
    /// Deletes all files in a KiFS directory.
    /// </summary>
    /// <param name="remoteDirName">Name of the KiFS directory containing files to delete.</param>
    /// <param name="noErrorIfNotExists">Whether to suppress error if directory doesn't exist.</param>
    public void DeleteFilesInDirectory(string remoteDirName, bool noErrorIfNotExists)
    {
        var showFilesResponse = _db.showFiles(new List<string> { remoteDirName }, new Dictionary<string, string>());
        DeleteFiles(showFilesResponse.file_names, noErrorIfNotExists);
    }

    /// <summary>
    /// Returns statistics about the given KiFS files.
    /// </summary>
    /// <param name="remotePaths">List of KiFS file or directory paths.</param>
    /// <returns>List of KifsFileInfo objects.</returns>
    public IList<KifsFileInfo> ShowFiles(IList<string> remotePaths)
    {
        if (remotePaths == null || remotePaths.Count == 0)
            throw new KineticaException("List of KiFS paths cannot be null or empty");

        var response = _db.showFiles(remotePaths, new Dictionary<string, string>());
        var result = new List<KifsFileInfo>();

        for (int i = 0; i < response.file_names.Count; i++)
        {
            result.Add(new KifsFileInfo
            {
                FileName = response.file_names[i],
                FileSize = response.sizes[i],
                CreatedBy = response.users[i],
                CreationTime = response.creation_times[i],
                Info = response.info
            });
        }

        return result;
    }

    #endregion

    #region Existence Checks

    /// <summary>
    /// Checks whether a KiFS directory exists.
    /// </summary>
    /// <param name="dirName">Full KiFS directory path to check.</param>
    /// <returns>True if the directory exists, false otherwise.</returns>
    public bool KifsDirectoryExists(string dirName)
    {
        if (string.IsNullOrEmpty(dirName))
            return false;

        try
        {
            var response = _db.showDirectories(dirName, new Dictionary<string, string>());
            return response.directories.Contains(dirName);
        }
        catch (KineticaException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether multiple KiFS directories exist.
    /// </summary>
    /// <param name="dirNames">Set of full KiFS directory paths to check.</param>
    /// <returns>True if all directories exist, false otherwise.</returns>
    public bool KifsDirectoriesExist(ISet<string> dirNames)
    {
        if (dirNames == null || dirNames.Count == 0)
            return false;

        try
        {
            var response = _db.showDirectories("", new Dictionary<string, string>());
            var existingDirs = new HashSet<string>(response.directories);
            return dirNames.All(d => existingDirs.Contains(d));
        }
        catch (KineticaException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether a KiFS file exists.
    /// </summary>
    /// <param name="fileName">Full KiFS file path to check.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public bool KifsFileExists(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        try
        {
            var response = _db.showFiles(new List<string> { fileName }, new Dictionary<string, string>());
            return response.file_names.Contains(fileName);
        }
        catch (KineticaException)
        {
            return false;
        }
    }

    #endregion

    #region Helper Methods

    private IList<string> ResolveLocalFiles(string pattern, bool recursive)
    {
        // Check if it's a glob pattern or a direct file path
        if (!ContainsGlobCharacters(pattern))
        {
            if (File.Exists(pattern))
            {
                return new List<string> { Path.GetFullPath(pattern) };
            }
            return new List<string>();
        }

        // Use glob matching
        var matcher = new Matcher();

        // Get the base directory and the pattern
        var (baseDir, globPattern) = SplitPathAndPattern(pattern);

        if (recursive)
        {
            matcher.AddInclude("**/" + globPattern);
        }
        else
        {
            matcher.AddInclude(globPattern);
        }

        var directoryInfo = new DirectoryInfo(baseDir);
        if (!directoryInfo.Exists)
        {
            return new List<string>();
        }

        var result = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));
        return result.Files.Select(f => Path.GetFullPath(Path.Combine(baseDir, f.Path))).ToList();
    }

    private static bool ContainsGlobCharacters(string path)
    {
        return path.Contains('*') || path.Contains('?') || path.Contains('[');
    }

    private static (string baseDir, string pattern) SplitPathAndPattern(string path)
    {
        var normalizedPath = path.Replace('\\', '/');
        var lastSeparatorBeforeGlob = -1;

        for (int i = 0; i < normalizedPath.Length; i++)
        {
            char c = normalizedPath[i];
            if (c == '*' || c == '?' || c == '[')
                break;
            if (c == '/')
                lastSeparatorBeforeGlob = i;
        }

        if (lastSeparatorBeforeGlob < 0)
        {
            return (Directory.GetCurrentDirectory(), normalizedPath);
        }

        return (normalizedPath.Substring(0, lastSeparatorBeforeGlob), normalizedPath.Substring(lastSeparatorBeforeGlob + 1));
    }

    private static string BuildRemotePath(string remoteDirName, string fileName)
    {
        if (remoteDirName.EndsWith(KifsPathSeparator))
        {
            return remoteDirName + fileName;
        }
        return remoteDirName + KifsPathSeparator + fileName;
    }

    #endregion

    #region Options Class

    /// <summary>
    /// Options for configuring the KineticaFileHandler behavior.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the file size threshold for multi-part uploads in bytes.
        /// Files larger than this will be uploaded in parts.
        /// Default is 60 MB.
        /// </summary>
        public long FileSizeToSplit { get; set; } = DefaultFileSizeToSplit;

        /// <summary>
        /// Gets or sets the thread pool size for concurrent file operations.
        /// Default is 5.
        /// </summary>
        public int ThreadPoolSize { get; set; } = DefaultThreadPoolSize;

        /// <summary>
        /// Gets or sets the maximum number of files to upload in a single batch.
        /// Default is 100.
        /// </summary>
        public int MaxFilesPerBatch { get; set; } = 100;

        /// <summary>
        /// Creates a new Options instance with default values.
        /// </summary>
        public Options()
        {
        }

        /// <summary>
        /// Creates a copy of the specified options.
        /// </summary>
        /// <param name="other">The options to copy.</param>
        public Options(Options other)
        {
            FileSizeToSplit = other.FileSizeToSplit;
            ThreadPoolSize = other.ThreadPoolSize;
            MaxFilesPerBatch = other.MaxFilesPerBatch;
        }

        /// <summary>
        /// Sets the file size threshold for multi-part uploads.
        /// </summary>
        /// <param name="size">The file size in bytes.</param>
        /// <returns>This options instance for chaining.</returns>
        public Options SetFileSizeToSplit(long size)
        {
            if (size <= 0 || size > DefaultFileSizeToSplit)
                throw new KineticaException($"FileSizeToSplit must be between 1 and {DefaultFileSizeToSplit}");
            FileSizeToSplit = size;
            return this;
        }

        /// <summary>
        /// Sets the thread pool size for concurrent file operations.
        /// </summary>
        /// <param name="size">The number of threads.</param>
        /// <returns>This options instance for chaining.</returns>
        public Options SetThreadPoolSize(int size)
        {
            if (size <= 0 || size > Environment.ProcessorCount)
                throw new KineticaException($"ThreadPoolSize must be between 1 and {Environment.ProcessorCount}");
            ThreadPoolSize = size;
            return this;
        }
    }

    #endregion
}
