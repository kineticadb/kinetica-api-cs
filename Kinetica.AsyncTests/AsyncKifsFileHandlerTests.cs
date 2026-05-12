using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Kinetica.AsyncTests.Common;
using kinetica;
using kinetica.FileSystem;

namespace Kinetica.AsyncTests
{
    /// <summary>
    /// Async integration tests for KiFS (Kinetica File System) operations.
    /// Tests the async UploadAsync and DownloadAsync methods.
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Category", "Async")]
    public class AsyncKifsFileHandlerTests : IAsyncLifetime
    {
        private KineticaFileHandler? _fileHandler;
        private string? _tempDir;
        private const string TestDirName = "async_kifs_test_dir";
        private TestContext? _ctx;

        public async Task InitializeAsync()
        {
            _ctx = new TestContext("async_kifs_tests");
            _fileHandler = new KineticaFileHandler(_ctx.Kinetica);
            _tempDir = Path.Combine(Path.GetTempPath(), $"async_kifs_test_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempDir);

            // Clean up any existing test directory
            try { _fileHandler.DeleteDirectory(TestDirName, true, true); } catch { }

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // Clean up test directory
            if (_fileHandler != null)
            {
                try { _fileHandler.DeleteDirectory(TestDirName, true, true); } catch { }
            }

            // Clean up temp directory
            if (_tempDir != null && Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, true); } catch { }
            }

            _ctx?.Dispose();

            await Task.CompletedTask;
        }

        #region Directory Tests

        [Fact]
        public void CreateDirectory_ValidName_CreatesSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void DeleteDirectory_ValidName_DeletesSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));

            _fileHandler.DeleteDirectory(TestDirName);
            Assert.False(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void ShowAllDirectories_ReturnsDirectories()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var directories = _fileHandler.ShowAllDirectories();

            Assert.NotNull(directories);
            Assert.Contains(directories, d => d.KifsPath == TestDirName);
        }

        [Fact]
        public void KifsDirectoryExists_ExistingDirectory_ReturnsTrue()
        {
            _fileHandler!.CreateDirectory(TestDirName);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void KifsDirectoryExists_NonExistingDirectory_ReturnsFalse()
        {
            Assert.False(_fileHandler!.KifsDirectoryExists("non_existing_dir_" + Guid.NewGuid().ToString("N")));
        }

        [Fact]
        public void CreateDirectory_ExistsNoError_NoException()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Should not throw when noErrorIfExists is true
            _fileHandler.CreateDirectory(TestDirName, noErrorIfExists: true);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void ShowDirectory_ValidName_ReturnsInfo()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var directories = _fileHandler.ShowDirectory(TestDirName);

            Assert.NotNull(directories);
            Assert.Single(directories);
            Assert.Equal(TestDirName, directories[0].KifsPath);
            Assert.NotEmpty(directories[0].CreatedBy);
        }

        #endregion

        #region Upload Tests (Async)

        [Fact]
        public async Task UploadAsync_SingleFile_UploadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Create test file
            var testFile = Path.Combine(_tempDir!, "test_upload.txt");
            await File.WriteAllTextAsync(testFile, "Test content for upload");

            // Upload using async method
            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName);

            // Verify
            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Single(files);
            Assert.Contains(files, f => f.FileName.EndsWith("test_upload.txt"));
        }

        [Fact]
        public async Task UploadAsync_MultipleFiles_UploadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Create multiple test files
            var file1 = Path.Combine(_tempDir!, "file1.txt");
            var file2 = Path.Combine(_tempDir!, "file2.txt");
            var file3 = Path.Combine(_tempDir!, "file3.txt");

            await File.WriteAllTextAsync(file1, "Content 1");
            await File.WriteAllTextAsync(file2, "Content 2");
            await File.WriteAllTextAsync(file3, "Content 3");

            // Upload all files using async method
            await _fileHandler.UploadAsync(new List<string> { file1, file2, file3 }, TestDirName);

            // Verify
            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Equal(3, files.Count);
        }

        [Fact]
        public async Task UploadAsync_LargeFile_UploadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Create a 5MB test file
            var largeFile = Path.Combine(_tempDir!, "large_file.bin");
            var content = new byte[5 * 1024 * 1024]; // 5MB
            new Random().NextBytes(content);
            await File.WriteAllBytesAsync(largeFile, content);

            // Upload using async method
            await _fileHandler.UploadAsync(new List<string> { largeFile }, TestDirName);

            // Verify
            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Single(files);
            Assert.True(files[0].FileSize > 0);
        }

        [Fact]
        public async Task UploadAsync_WithOptions_UploadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFile = Path.Combine(_tempDir!, "test_with_options.txt");
            await File.WriteAllTextAsync(testFile, "Test content with options");

            var options = new UploadOptions
            {
                DeleteIfExists = false
            };

            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName, options);

            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Single(files);
        }

        [Fact]
        public async Task UploadAsync_WithCallback_CallsCallback()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFile = Path.Combine(_tempDir!, "test_callback.txt");
            await File.WriteAllTextAsync(testFile, "Hello, Callback!");

            var callback = new TestUploadListener();

            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName, UploadOptions.Default, callback);

            Assert.True(callback.FullFileUploadCalled);
        }

        [Fact]
        public async Task UploadAsync_NonExistentFile_ThrowsException()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            await Assert.ThrowsAsync<KineticaException>(async () =>
                await _fileHandler.UploadAsync(new List<string> { "/non/existent/file.txt" }, TestDirName));
        }

        [Fact]
        public async Task UploadAsync_NonExistentDirectory_ThrowsException()
        {
            var testFile = Path.Combine(_tempDir!, "test.txt");
            await File.WriteAllTextAsync(testFile, "Test");

            await Assert.ThrowsAsync<KineticaException>(async () =>
                await _fileHandler!.UploadAsync(new List<string> { testFile }, "non_existent_dir_" + Guid.NewGuid().ToString("N")));
        }

        #endregion

        #region Download Tests (Async)

        [Fact]
        public async Task DownloadAsync_SingleFile_DownloadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Upload a file first
            var uploadFile = Path.Combine(_tempDir!, "upload_for_download.txt");
            var testContent = "Test content for download";
            await File.WriteAllTextAsync(uploadFile, testContent);
            await _fileHandler.UploadAsync(new List<string> { uploadFile }, TestDirName);

            // Download it
            var downloadDir = Path.Combine(_tempDir!, "downloads");
            Directory.CreateDirectory(downloadDir);

            await _fileHandler.DownloadAsync(
                new List<string> { $"{TestDirName}/upload_for_download.txt" },
                downloadDir);

            // Verify
            var downloadedFile = Path.Combine(downloadDir, "upload_for_download.txt");
            Assert.True(File.Exists(downloadedFile));
            var downloadedContent = await File.ReadAllTextAsync(downloadedFile);
            Assert.Equal(testContent, downloadedContent);
        }

        [Fact]
        public async Task DownloadAsync_MultipleFiles_DownloadsSuccessfully()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Upload multiple files
            var file1 = Path.Combine(_tempDir!, "download1.txt");
            var file2 = Path.Combine(_tempDir!, "download2.txt");
            await File.WriteAllTextAsync(file1, "Content 1");
            await File.WriteAllTextAsync(file2, "Content 2");
            await _fileHandler.UploadAsync(new List<string> { file1, file2 }, TestDirName);

            // Download them
            var downloadDir = Path.Combine(_tempDir!, "downloads_multi");
            Directory.CreateDirectory(downloadDir);

            await _fileHandler.DownloadAsync(
                new List<string> {
                    $"{TestDirName}/download1.txt",
                    $"{TestDirName}/download2.txt"
                },
                downloadDir);

            // Verify
            Assert.True(File.Exists(Path.Combine(downloadDir, "download1.txt")));
            Assert.True(File.Exists(Path.Combine(downloadDir, "download2.txt")));
        }

        [Fact]
        public async Task DownloadAsync_WithGlobPattern_DownloadsMatchingFiles()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            // Upload files with pattern
            var csvFile1 = Path.Combine(_tempDir!, "data1.csv");
            var csvFile2 = Path.Combine(_tempDir!, "data2.csv");
            var txtFile = Path.Combine(_tempDir!, "readme.txt");

            await File.WriteAllTextAsync(csvFile1, "csv1");
            await File.WriteAllTextAsync(csvFile2, "csv2");
            await File.WriteAllTextAsync(txtFile, "txt");

            await _fileHandler.UploadAsync(new List<string> { csvFile1, csvFile2, txtFile }, TestDirName);

            // Download only CSV files using glob pattern
            var downloadDir = Path.Combine(_tempDir!, "downloads_glob");
            Directory.CreateDirectory(downloadDir);

            await _fileHandler.DownloadAsync(
                new List<string> { $"{TestDirName}/*.csv" },
                downloadDir);

            // Verify only CSV files were downloaded
            Assert.True(File.Exists(Path.Combine(downloadDir, "data1.csv")));
            Assert.True(File.Exists(Path.Combine(downloadDir, "data2.csv")));
            Assert.False(File.Exists(Path.Combine(downloadDir, "readme.txt")));
        }

        [Fact]
        public async Task DownloadAsync_OverwriteExistingFalse_ThrowsException()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFileName = "test_overwrite.txt";
            var uploadPath = Path.Combine(_tempDir!, testFileName);
            await File.WriteAllTextAsync(uploadPath, "Original content");
            await _fileHandler.UploadAsync(new List<string> { uploadPath }, TestDirName);

            var downloadDir = Path.Combine(_tempDir!, "downloads_overwrite");
            Directory.CreateDirectory(downloadDir);
            var existingFile = Path.Combine(downloadDir, testFileName);
            await File.WriteAllTextAsync(existingFile, "Existing content");

            var remoteFilePath = $"{TestDirName}/{testFileName}";
            var options = new DownloadOptions(overwriteExisting: false);

            await Assert.ThrowsAsync<KineticaException>(async () =>
                await _fileHandler.DownloadAsync(new List<string> { remoteFilePath }, downloadDir, options, null));
        }

        #endregion

        #region File Management Tests

        [Fact]
        public async Task ShowFiles_AfterUpload_ReturnsFileInfo()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFile = Path.Combine(_tempDir!, "show_file_test.txt");
            await File.WriteAllTextAsync(testFile, "Test content");
            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName);

            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });

            Assert.Single(files);
            Assert.EndsWith("show_file_test.txt", files[0].FileName);
            Assert.True(files[0].FileSize > 0);
        }

        [Fact]
        public async Task DeleteFiles_RemovesFiles()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFile = Path.Combine(_tempDir!, "delete_test.txt");
            await File.WriteAllTextAsync(testFile, "Delete me");
            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName);

            // Verify file exists
            var filesBefore = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Single(filesBefore);

            // Delete file
            _fileHandler.DeleteFiles(new List<string> { $"{TestDirName}/delete_test.txt" });

            // Verify file is gone
            var filesAfter = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Empty(filesAfter);
        }

        [Fact]
        public async Task KifsFileExists_ExistingFile_ReturnsTrue()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            var testFile = Path.Combine(_tempDir!, "exists_test.txt");
            await File.WriteAllTextAsync(testFile, "I exist");
            await _fileHandler.UploadAsync(new List<string> { testFile }, TestDirName);

            Assert.True(_fileHandler.KifsFileExists($"{TestDirName}/exists_test.txt"));
        }

        [Fact]
        public void KifsFileExists_NonExistingFile_ReturnsFalse()
        {
            Assert.False(_fileHandler!.KifsFileExists($"{TestDirName}/nonexistent.txt"));
        }

        [Fact]
        public async Task DeleteFilesInDirectory_ValidDirectory_DeletesAllFiles()
        {
            _fileHandler!.CreateDirectory(TestDirName);

            for (int i = 1; i <= 3; i++)
            {
                var fileName = $"test_delete_all_{i}.txt";
                var filePath = Path.Combine(_tempDir!, fileName);
                await File.WriteAllTextAsync(filePath, $"Content {i}");
                await _fileHandler.UploadAsync(new List<string> { filePath }, TestDirName);
            }

            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Equal(3, files.Count);

            _fileHandler.DeleteFilesInDirectory(TestDirName);

            files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Empty(files);
        }

        #endregion

        #region Helper Classes

        private class TestUploadListener : IFileUploadListener
        {
            public bool MultiPartUploadCompleteCalled { get; private set; }
            public bool PartUploadCalled { get; private set; }
            public bool FullFileUploadCalled { get; private set; }
            public List<FileOperationResult> Results { get; } = new List<FileOperationResult>();

            public void OnMultiPartUploadComplete(IList<FileOperationResult> results)
            {
                MultiPartUploadCompleteCalled = true;
                Results.AddRange(results);
            }

            public void OnPartUpload(FileOperationResult result)
            {
                PartUploadCalled = true;
                Results.Add(result);
            }

            public void OnFullFileUpload(FileOperationResult result)
            {
                FullFileUploadCalled = true;
                Results.Add(result);
            }
        }

        #endregion
    }
}
