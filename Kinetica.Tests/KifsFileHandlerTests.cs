using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using kinetica;
using kinetica.FileSystem;

namespace Kinetica.Tests
{
    /// <summary>
    /// Integration tests for KiFS (Kinetica File System) operations.
    /// </summary>
    [Trait("Category", "Integration")]
    public class KifsFileHandlerTests : IDisposable
    {
        private const string ConnectionUrl = "http://localhost:9191";
        private const string Username = "admin";
        private const string Password = "secret";
        private const string TestDirName = "kifs_test_dir";

        private readonly kinetica.Kinetica _kdb;
        private readonly KineticaFileHandler _fileHandler;
        private readonly string _tempDir;

        public KifsFileHandlerTests()
        {
            _kdb = new kinetica.Kinetica(ConnectionUrl, new kinetica.Kinetica.Options()
            {
                Username = Username,
                Password = Password
            });
            _fileHandler = new KineticaFileHandler(_kdb);
            _tempDir = Path.Combine(Path.GetTempPath(), $"kifs_test_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            // Clean up test directory
            try { _fileHandler.DeleteDirectory(TestDirName, true, true); } catch { }

            // Clean up temp directory
            if (Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, true); } catch { }
            }
        }

        #region Directory Tests

        [Fact]
        public void CreateDirectory_ValidName_CreatesSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }

            // Act
            _fileHandler.CreateDirectory(TestDirName);

            // Assert
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void CreateDirectory_ExistsNoError_NoException()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act & Assert - should not throw
            _fileHandler.CreateDirectory(TestDirName, noErrorIfExists: true);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void DeleteDirectory_ValidName_DeletesSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));

            // Act
            _fileHandler.DeleteDirectory(TestDirName);

            // Assert
            Assert.False(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void ShowAllDirectories_ReturnsDirectories()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act
            var directories = _fileHandler.ShowAllDirectories();

            // Assert
            Assert.NotNull(directories);
            Assert.Contains(directories, d => d.KifsPath == TestDirName);
        }

        [Fact]
        public void ShowDirectory_ValidName_ReturnsInfo()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act
            var directories = _fileHandler.ShowDirectory(TestDirName);

            // Assert
            Assert.NotNull(directories);
            Assert.Single(directories);
            Assert.Equal(TestDirName, directories[0].KifsPath);
            Assert.NotEmpty(directories[0].CreatedBy);
        }

        [Fact]
        public void KifsDirectoryExists_ExistingDirectory_ReturnsTrue()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act & Assert
            Assert.True(_fileHandler.KifsDirectoryExists(TestDirName));
        }

        [Fact]
        public void KifsDirectoryExists_NonExistingDirectory_ReturnsFalse()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }

            // Act & Assert
            Assert.False(_fileHandler.KifsDirectoryExists("non_existing_dir_" + Guid.NewGuid().ToString("N")));
        }

        #endregion

        #region Upload Tests

        [Fact]
        public void Upload_SingleFile_UploadsSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_upload.txt";
            var testFilePath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(testFilePath, "Hello, KiFS!");

            // Act
            _fileHandler.Upload(testFilePath, TestDirName);

            // Assert
            var remoteFilePath = $"{TestDirName}/{testFileName}";
            Assert.True(_fileHandler.KifsFileExists(remoteFilePath));
        }

        [Fact]
        public void Upload_MultipleFiles_UploadsSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFiles = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                var fileName = $"test_upload_{i}.txt";
                var filePath = Path.Combine(_tempDir, fileName);
                File.WriteAllText(filePath, $"Content for file {i}");
                testFiles.Add(filePath);
            }

            // Act
            _fileHandler.Upload(testFiles, TestDirName);

            // Assert
            foreach (var filePath in testFiles)
            {
                var fileName = Path.GetFileName(filePath);
                var remoteFilePath = $"{TestDirName}/{fileName}";
                Assert.True(_fileHandler.KifsFileExists(remoteFilePath));
            }
        }

        [Fact]
        public void Upload_WithCallback_CallsCallback()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_callback.txt";
            var testFilePath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(testFilePath, "Hello, Callback!");

            var callback = new TestUploadListener();

            // Act
            _fileHandler.Upload(testFilePath, TestDirName, UploadOptions.Default, callback);

            // Assert
            Assert.True(callback.FullFileUploadCalled);
        }

        [Fact]
        public void Upload_NonExistentFile_ThrowsException()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act & Assert
            Assert.Throws<KineticaException>(() =>
                _fileHandler.Upload("/non/existent/file.txt", TestDirName));
        }

        [Fact]
        public void Upload_NonExistentDirectory_ThrowsException()
        {
            // Arrange
            var testFilePath = Path.Combine(_tempDir, "test.txt");
            File.WriteAllText(testFilePath, "Test");

            // Act & Assert
            Assert.Throws<KineticaException>(() =>
                _fileHandler.Upload(testFilePath, "non_existent_dir_" + Guid.NewGuid().ToString("N")));
        }

        #endregion

        #region Download Tests

        [Fact]
        public void Download_SingleFile_DownloadsSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_download.txt";
            var uploadPath = Path.Combine(_tempDir, testFileName);
            var expectedContent = "Hello, Download!";
            File.WriteAllText(uploadPath, expectedContent);
            _fileHandler.Upload(uploadPath, TestDirName);

            var downloadDir = Path.Combine(_tempDir, "downloads");
            Directory.CreateDirectory(downloadDir);

            // Act
            var remoteFilePath = $"{TestDirName}/{testFileName}";
            _fileHandler.Download(remoteFilePath, downloadDir);

            // Assert
            var downloadedPath = Path.Combine(downloadDir, testFileName);
            Assert.True(File.Exists(downloadedPath));
            Assert.Equal(expectedContent, File.ReadAllText(downloadedPath));
        }

        [Fact]
        public void Download_MultipleFiles_DownloadsSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFiles = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                var fileName = $"test_download_{i}.txt";
                var filePath = Path.Combine(_tempDir, fileName);
                File.WriteAllText(filePath, $"Content for file {i}");
                _fileHandler.Upload(filePath, TestDirName);
                testFiles.Add($"{TestDirName}/{fileName}");
            }

            var downloadDir = Path.Combine(_tempDir, "downloads_multi");
            Directory.CreateDirectory(downloadDir);

            // Act
            _fileHandler.Download(testFiles, downloadDir);

            // Assert
            foreach (var remoteFile in testFiles)
            {
                var fileName = Path.GetFileName(remoteFile);
                var downloadedPath = Path.Combine(downloadDir, fileName);
                Assert.True(File.Exists(downloadedPath));
            }
        }

        [Fact]
        public void Download_OverwriteExistingFalse_ThrowsException()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_overwrite.txt";
            var uploadPath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(uploadPath, "Original content");
            _fileHandler.Upload(uploadPath, TestDirName);

            var downloadDir = Path.Combine(_tempDir, "downloads_overwrite");
            Directory.CreateDirectory(downloadDir);
            var existingFile = Path.Combine(downloadDir, testFileName);
            File.WriteAllText(existingFile, "Existing content");

            // Act & Assert
            var remoteFilePath = $"{TestDirName}/{testFileName}";
            var options = new DownloadOptions(overwriteExisting: false);

            Assert.Throws<KineticaException>(() =>
                _fileHandler.Download(remoteFilePath, downloadDir, options, null));
        }

        #endregion

        #region File Operations Tests

        [Fact]
        public void ShowFiles_ValidDirectory_ReturnsFileInfo()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_show.txt";
            var testContent = "Show files test content";
            var uploadPath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(uploadPath, testContent);
            _fileHandler.Upload(uploadPath, TestDirName);

            // Act
            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });

            // Assert
            Assert.NotNull(files);
            Assert.NotEmpty(files);
            Assert.Contains(files, f => f.FileName.EndsWith(testFileName));

            var testFile = files[0];
            Assert.True(testFile.FileSize > 0);
            Assert.NotEmpty(testFile.CreatedBy);
        }

        [Fact]
        public void DeleteFiles_ValidFile_DeletesSuccessfully()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_delete.txt";
            var uploadPath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(uploadPath, "Delete test");
            _fileHandler.Upload(uploadPath, TestDirName);

            var remoteFilePath = $"{TestDirName}/{testFileName}";
            Assert.True(_fileHandler.KifsFileExists(remoteFilePath));

            // Act
            _fileHandler.DeleteFiles(new List<string> { remoteFilePath });

            // Assert
            Assert.False(_fileHandler.KifsFileExists(remoteFilePath));
        }

        [Fact]
        public void DeleteFilesInDirectory_ValidDirectory_DeletesAllFiles()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            for (int i = 1; i <= 3; i++)
            {
                var fileName = $"test_delete_all_{i}.txt";
                var filePath = Path.Combine(_tempDir, fileName);
                File.WriteAllText(filePath, $"Content {i}");
                _fileHandler.Upload(filePath, TestDirName);
            }

            var files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Equal(3, files.Count);

            // Act
            _fileHandler.DeleteFilesInDirectory(TestDirName);

            // Assert
            files = _fileHandler.ShowFiles(new List<string> { TestDirName });
            Assert.Empty(files);
        }

        [Fact]
        public void KifsFileExists_ExistingFile_ReturnsTrue()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            var testFileName = "test_exists.txt";
            var uploadPath = Path.Combine(_tempDir, testFileName);
            File.WriteAllText(uploadPath, "Exists test");
            _fileHandler.Upload(uploadPath, TestDirName);

            // Act & Assert
            var remoteFilePath = $"{TestDirName}/{testFileName}";
            Assert.True(_fileHandler.KifsFileExists(remoteFilePath));
        }

        [Fact]
        public void KifsFileExists_NonExistingFile_ReturnsFalse()
        {
            // Arrange
            try { _fileHandler.DeleteDirectory(TestDirName); } catch { }
            _fileHandler.CreateDirectory(TestDirName);

            // Act & Assert
            Assert.False(_fileHandler.KifsFileExists($"{TestDirName}/non_existing_file.txt"));
        }

        #endregion

        #region Options Tests

        [Fact]
        public void Options_DefaultValues_AreCorrect()
        {
            // Act
            var options = new KineticaFileHandler.Options();

            // Assert
            Assert.Equal(KineticaFileHandler.DefaultFileSizeToSplit, options.FileSizeToSplit);
            Assert.Equal(KineticaFileHandler.DefaultThreadPoolSize, options.ThreadPoolSize);
            Assert.Equal(100, options.MaxFilesPerBatch);
        }

        [Fact]
        public void UploadOptions_DefaultValues_AreCorrect()
        {
            // Act
            var options = UploadOptions.Default;

            // Assert
            Assert.True(options.Recursive);
            Assert.Equal(-1, options.Ttl);
            Assert.False(options.DeleteIfExists);
        }

        [Fact]
        public void DownloadOptions_DefaultValues_AreCorrect()
        {
            // Act
            var options = DownloadOptions.Default;

            // Assert
            Assert.True(options.OverwriteExisting);
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
