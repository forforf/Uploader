using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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
        private MockFileSystem fileSystem;

        private String watchDirectoryName;
        private String newWatchDirectoryName;
        private FilePathModel filePathModel;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.watchDirectoryName = @"C:\watchDir\";
            this.newWatchDirectoryName = @"C:\newPath\";
            this.localPathSubject = new BehaviorSubject<String>(this.watchDirectoryName);
            this.watcherObservable = new Mock<IWatcherObservable>();

            this.fileSystem = new MockFileSystem();
            var watchDirectory = this.fileSystem.Directory
                .CreateDirectory(this.watchDirectoryName);
            this.fileSystem.AddDirectory(newWatchDirectoryName);
            //var dailyFtpDirectory = roootDirectory.CreateSubdirectory("DailyFtp");
            //var dataDirectory = dailyFtpDirectory.CreateSubdirectory("Data");
            //var outputDirectory = dailyFtpDirectory.CreateSubdirectory("Output");
            this.settings.Setup(x => x.WatchPath).Returns(this.watchDirectoryName);
            this.filePathModel = new FilePathModel(
                settings.Object,
                localPathSubject,
                watcherObservable.Object,
                this.fileSystem);
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
        [Ignore("We want to recreate the FileSystemWatcher if it goes away, but not working")]
        public void FilePathModel_ToggleWatch_WatchIsNull()
        {
            var _fileSystem = new MockFileSystem();
            String watchDirName = @"C:\temp";
            var _watchDirectory = _fileSystem.Directory.CreateDirectory(watchDirName);
            var _localPathSubject = new BehaviorSubject<String>(watchDirName);
            var _settingsMock = new Mock<ISettings>();
            _settingsMock.Setup(x => x.WatchPath).Returns(watchDirName);

            var _watcherMock = new Mock<IWatcherObservable>();
            var _filePathModel = new FilePathModel(
                 _settingsMock.Object,
                 _localPathSubject,
                 _watcherMock.Object,
                 _fileSystem);

            _filePathModel.watcher = null;
            Assert.That(_filePathModel.watcher, Is.Null);

            _filePathModel.ToggleWatch();

            //Verify we have a watcher
            Assert.That(_filePathModel.watcher, Is.Not.Null);

            //Verify we turn it on
            watcherObservable.Verify(x => x.Start());
        }

        [Test]
        public void FilePathModel_ChangeWatchPath()
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
    }
}


