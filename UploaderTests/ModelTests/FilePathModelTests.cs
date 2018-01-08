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

        private String watchDirectory;
        private FilePathModel filePathModel;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.watchDirectory = @"C:\watchDir\";
            this.localPathSubject = new BehaviorSubject<String>(this.watchDirectory);
            this.watcherObservable = new Mock<IWatcherObservable>();

            this.fileSystem = new MockFileSystem();
            var watchDirectory = this.fileSystem.Directory.CreateDirectory(this.watchDirectory);
            //var dailyFtpDirectory = roootDirectory.CreateSubdirectory("DailyFtp");
            //var dataDirectory = dailyFtpDirectory.CreateSubdirectory("Data");
            //var outputDirectory = dailyFtpDirectory.CreateSubdirectory("Output");
            this.settings.Setup(x => x.WatchPath).Returns(this.watchDirectory);
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
        public void FilePathModel_ToggleWatch_WatchIsNull()
        {

            IWatcherObservable watcher = null;
            this.filePathModel = new FilePathModel(
                 settings.Object,
                 localPathSubject,
                 watcher,
                 this.fileSystem);

            this.filePathModel.ToggleWatch();

            //Verify we have a watcher
            Assert.That(watcher, Is.Not.Null);

            //Verify we turn it on
            watcherObservable.Verify(x => x.Start());
        }
    }
}


