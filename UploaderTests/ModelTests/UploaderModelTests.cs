using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Uploader;
using Uploader.Model;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class UploaderModelTests
    {
        private Mock<ISettings> settings;
        private Mock<IFilePathModel> filePathModel;
        private Mock<IS3PathModel> s3PathModel;
        private ReplaySubject<String> messagePasser;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.filePathModel = new Mock<IFilePathModel>();
            this.s3PathModel = new Mock<IS3PathModel>();
            this.messagePasser = new ReplaySubject<String>();


            // Initialize subscription
            // Observable never ends, but will emit a fake path initially
            this.filePathModel.Setup(x => x.GetObservable())
                .Returns(Observable.Never<String>().StartWith(@"c:\foo.test"));
        }


        [Test]
        public void UploaderModel_Dispose()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);
            
            uploaderModel.Dispose();

            filePathModel.Verify(x => x.Dispose());
            s3PathModel.Verify(x => x.Dispose());
            // Not sure how to elegantly test private fileWatcherSubscription is disposed
        }

        [Test]
        public void UploaderModel_ToggleWatch()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            uploaderModel.ToggleWatch();

            filePathModel.Verify(x => x.ToggleWatch());
        }

        [Test]
        public void UploaderModel_UpdateWatcher()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            String newPath = @"new_path";
            uploaderModel.UpdateWatcher(newPath);

            filePathModel.Verify(x => x.ChangeWatchPath(newPath));
        }

        [Test]
        public void UploaderModel_UploadToS3()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            String localPath = @"local_path";
            String s3Bucket = @"S3_bucket";
            uploaderModel.UploadToS3(localPath, s3Bucket);

            s3PathModel.Verify(x => x.UploadToS3(localPath, s3Bucket));
        }

        [Test]
        public void UploaderModel_IsWatching()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            uploaderModel.IsWatching();

            filePathModel.Verify(x => x.IsWatching());
        }
    }
}


