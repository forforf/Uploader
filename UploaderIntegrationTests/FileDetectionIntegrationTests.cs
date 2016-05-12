using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uploader;


namespace UploaderIntegrationTests
{
    [TestClass]
    public class FileDetectionIntegrationTests
    {

        // Directory Helpers
        public class TestFileManager
        {
            private string testDirectoryPath;
            private string sandboxPath;
            private string moreContent;

            public string TestFile1Data;
            public string TestFile1Path;
            public string TestFile1Name;
            public DirectoryInfo TestDirectory;
            public DirectoryInfo SandboxDirectory;
            
            

            public TestFileManager()
            {
                this.sandboxPath = @"c:\temp\file_detection_integration_tests";
                this.SandboxDirectory = new DirectoryInfo(this.sandboxPath);
                this.testDirectoryPath = Path.Combine(this.sandboxPath, @"watch_directory");
                this.TestDirectory = new DirectoryInfo(this.testDirectoryPath);
                this.TestDirectory.Create();
                this.TestFile1Data = "Test 123";
                this.TestFile1Name = @"watch_directory";
                this.TestFile1Path = Path.Combine(this.TestDirectory.FullName, this.TestFile1Name);

            }

            public void CreateTestFile1()
            {
                File.WriteAllText(this.TestFile1Path, this.TestFile1Data);
            }

            public void AppendTestFile1(string appendedString )
            {
                File.AppendAllText(this.TestFile1Path, appendedString);
            }
        }

        public class FileWatcherHandler
        {
            public string Path;
            public WatcherChangeTypes ChangeType;
            
            //string fileChangedPath = "";
            //WatcherChangeTypes fileChangeType = WatcherChangeTypes.Renamed; // have to use one of the enum values
            // var fileChangeDetected = new System.Threading.ManualResetEvent(false);
            public FileWatcherHandler(FileSystemEventArgs e)
            {
                this.Path = e.FullPath;
                this.ChangeType = e.ChangeType;
            }

        }

        public DirectoryInfo GetSandboxDir()
        {
            return new DirectoryInfo(@"c:\temp\file_detection_integration_tests");
        }

        public DirectoryInfo GetWatcherDir()
        {
            var sandbox = GetSandboxDir().FullName;
            var testDirectoryPath = Path.Combine(sandbox, @"watch_directory");
            return new DirectoryInfo(@testDirectoryPath);
        }
        [TestMethod]
        public void FileCreated_FileDetection_Test()
        {
            // === Arrange ===
            // Sets up test directories and files
            var testFileManager = new TestFileManager();
            
            //   Setup FileDetector    
            var fileDetector = FileDetector.Factory.CreateFileDetector( testFileManager.TestDirectory.FullName );
            
            //   file change handler
            FileWatcherHandler fileWatcherHandler = null; 
            var fileChangeDetected = new System.Threading.ManualResetEvent(false);
            fileDetector.OnChanged += (s, e) =>
            {
                fileWatcherHandler = new FileWatcherHandler(e);
                fileChangeDetected.Set();
            };

            // === Act ===
            //   Create File
            testFileManager.CreateTestFile1();
            fileChangeDetected.WaitOne(500);

            // === Assert ===
            //   Detect that file was created
            Assert.IsNotNull(fileWatcherHandler);
            Assert.AreEqual(fileWatcherHandler.Path, testFileManager.TestFile1Path);
            var readText = File.ReadAllText( testFileManager.TestFile1Path );
            Assert.AreEqual(readText, testFileManager.TestFile1Data);

            //   Valid Change Types
            //  NOTE: Change Type is Changed NOT Created for some reason
            Assert.AreEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Changed);
            
            //   Invalid Change Types
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Created);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.All);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Deleted);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Renamed);
        }

        [TestMethod]
        public void FileChanged_FileDetection_Test()
        {
            // === Arrange ===
            // Sets up test directories and files
            var testFileManager = new TestFileManager();

            //   Setup FileDetector    
            var fileDetector = FileDetector.Factory.CreateFileDetector(testFileManager.TestDirectory.FullName);

            //   file change handler
            FileWatcherHandler fileWatcherHandler = null;
            var fileChangeDetected = new System.Threading.ManualResetEvent(false);
            fileDetector.OnChanged += (s, e) =>
            {
                fileWatcherHandler = new FileWatcherHandler(e);
                fileChangeDetected.Set();
            };

            // === Act ===
            //   Create File
            testFileManager.CreateTestFile1();
            
            //   Detect that file was created
            fileChangeDetected.WaitOne(500);
            Assert.IsNotNull(fileWatcherHandler);
            Assert.AreEqual(fileWatcherHandler.Path, testFileManager.TestFile1Path);

            // Reset handler until next event fired
            fileWatcherHandler = null;

            //   Content
            var readInitText = File.ReadAllText(testFileManager.TestFile1Path);
            Assert.AreEqual(readInitText, testFileManager.TestFile1Data);

            //   Modify the file
            var moreText = " moar text";
            testFileManager.AppendTestFile1(moreText);
            var fileText = testFileManager.TestFile1Data + moreText;

            fileChangeDetected.WaitOne(500);
            
            // === Assert ===
            Assert.IsNotNull(fileWatcherHandler);
            Assert.AreEqual(fileWatcherHandler.Path, testFileManager.TestFile1Path);
            var readText = File.ReadAllText(testFileManager.TestFile1Path);
            Assert.AreEqual(readText, fileText);

            //   Valid Change Types
            Assert.AreEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Changed);

            //   Invalid Change Types
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Created);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.All);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Deleted);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Renamed);
        }

        [TestMethod]
        public void FileDeleted_FileDetection_Test()
        {
            // === Arrange ===
            //   Create test directory
            // Sets up test directories and files
            var testFileManager = new TestFileManager();

            //   Setup FileDetector    
            var fileDetector = FileDetector.Factory.CreateFileDetector(testFileManager.TestDirectory.FullName);

            //   file change handler
            FileWatcherHandler fileWatcherHandler = null;
            var fileChangeDetected = new System.Threading.ManualResetEvent(false);
            fileDetector.OnChanged += (s, e) =>
            {
                fileWatcherHandler = new FileWatcherHandler(e);
                fileChangeDetected.Set();
            };

            // === Act ===
            //   Create File
            testFileManager.CreateTestFile1();

            //   Detect that file was created
            fileChangeDetected.WaitOne(500);
            Assert.IsNotNull(fileWatcherHandler);
            Assert.AreEqual(fileWatcherHandler.Path, testFileManager.TestFile1Path);

            // Reset handler until next event fired
            fileWatcherHandler = null;

            //   Delete the file
            File.Delete(testFileManager.TestFile1Path);

            fileChangeDetected.WaitOne(500);

            // === Assert ===
            Assert.IsNotNull(fileWatcherHandler);
            Assert.AreEqual(fileWatcherHandler.Path, testFileManager.TestFile1Path);
            Assert.IsFalse(File.Exists(testFileManager.TestFile1Path));


            //   Valid Change Types
            Assert.AreEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Changed);

            //   Invalid Change Types
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Created);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.All);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Deleted);
            Assert.AreNotEqual(fileWatcherHandler.ChangeType, WatcherChangeTypes.Renamed);
        }

        [TestCleanup]
        public void TestTeardown()
        {
            var testFileManager = new TestFileManager();
            testFileManager.SandboxDirectory.Delete(true);
        }

        // File deleted
        // File moved out of directory
        // File moved into directory
        // watch directory does not exist
        // watch directory removed (after setup)
        // File renamed
        // File renamed and modified


    }
}
