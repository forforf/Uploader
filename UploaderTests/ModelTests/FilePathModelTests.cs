using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Uploader;
using Uploader.Model;
using UploadWatchers;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class FilePathModelTests
    {
        private Mock<ISettings> settings;
        private BehaviorSubject<String> localPathSubject;
        private Mock<IWatcherObservable> watcherObservable;

        private String watchDirectoryName;
        private String newWatchDirectoryName;
        private FilePathModel filePathModel;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.watchDirectoryName = GetTemporaryDirectory(@"c:\temp\uploader_tests_temp");
            this.newWatchDirectoryName =GetTemporaryDirectory(@"C:\temp\uploader_tests_temp2\");
            this.localPathSubject = new BehaviorSubject<String>(this.watchDirectoryName);
            this.watcherObservable = new Mock<IWatcherObservable>();

            this.settings.Setup(x => x.WatchPath).Returns(this.watchDirectoryName);
            this.filePathModel = new FilePathModel(
                settings.Object,
                localPathSubject,
                watcherObservable.Object);
        }

        [TearDown]
        public void AfterEachTest()
        {
            if (!Directory.Exists(this.watchDirectoryName))
            {
                return;
            }
            Directory.Delete(watchDirectoryName, true);
        }

        [Test]
        public void FilePathModel_Dispose()
        {
            this.filePathModel.Dispose();

            watcherObservable.Verify(x => x.Dispose());
        }

        [Test]
        public void FilePathModel_ToggleWatch_OnToOff()
        {
            //Watcher is On
            this.watcherObservable.Setup(x => x.IsWatching()).Returns(true);

            this.filePathModel.ToggleWatch();

            //Verify we turn it off
            watcherObservable.Verify(x => x.Stop());
        }

        [Test]
        public void FilePathModel_ToggleWatch_OffToOn()
        {
            //Watcher is On
            this.watcherObservable.Setup(x => x.IsWatching()).Returns(false);

            this.filePathModel.ToggleWatch();

            //Verify we turn it on
            watcherObservable.Verify(x => x.Start());
        }

        [Test]
        public void FilePathModel_ToggleWatch_WatchIsNull()
        {
            String watchDirName = GetTemporaryDirectory(@"c:\temp\uploader_tests_temp");
            var _localPathSubject = new BehaviorSubject<String>(watchDirName);
            var _settingsMock = new Mock<ISettings>();
            _settingsMock.Setup(x => x.WatchPath).Returns(watchDirName);

            var _watcherMock = new Mock<IWatcherObservable>();
            var _filePathModel = new FilePathModel(
                 _settingsMock.Object,
                 _localPathSubject,
                 _watcherMock.Object)
            {
                watcher = null
            };
            Assert.That(_filePathModel.watcher, Is.Null);

            _filePathModel.ToggleWatch();

            //Verify we have a watcher
            Assert.That(_filePathModel.watcher, Is.Not.Null);
            Assert.True(_filePathModel.watcher.IsWatching());
        }

        [Test]
        public void FilePathModel_ChangeWatchPath_Directory()
        {
            
            String subjectPath = "";

            this.localPathSubject.Subscribe(x => subjectPath = x);


            this.filePathModel.ChangeWatchPath(this.newWatchDirectoryName);

            this.watcherObservable.VerifySet(x => x.Path = this.newWatchDirectoryName, Times.Once());
            this.settings.VerifySet(x => x.WatchPath = this.newWatchDirectoryName, Times.Once());
            Assert.That(subjectPath, Is.EqualTo(this.newWatchDirectoryName));

        }

        [Test]
        public void FilePathModel_IsWatching()
        {
            this.filePathModel.IsWatching();

            watcherObservable.Verify(x => x.IsWatching());
        }

        [Test]
        public void FilePathModel_GetObservable()
        {
            this.filePathModel.GetObservable();

            watcherObservable.Verify(x => x.GetObservable());
        }

        // Yes, I know I'm creating files for a unit test, the complexity
        // involved in injecting a file system interface is just not worth
        // the hassle.
        private String GetTemporaryDirectory(String dirName)
        {
            Directory.CreateDirectory(dirName);
            return dirName;
        }
    }
}


