using Amazon.S3.Transfer;
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
    public class S3PathModelTests
    {
        private Mock<ISettings> settings;
        private BehaviorSubject<String> s3PathSubject; 
        private Mock<ITransferUtility> directoryTranferUtility;
        private MockFileSystem fileSystem;

        private String directoryName;
        private String filePath;
        private S3PathModel s3PathModel;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.s3PathSubject = new BehaviorSubject<String>("S3_Path");
            this.directoryTranferUtility = new Mock<ITransferUtility>();
            this.directoryName = @"C:\thisIsADir\";
            this.filePath = $"{this.directoryName}thisIsAFile.txt";

            this.settings.Setup(x => x.S3Path).Returns("S3_Path");


            this.fileSystem = new MockFileSystem();
            this.fileSystem.AddDirectory(this.directoryName);
            this.fileSystem.AddFile(filePath, "Dont care about file data");
            this.s3PathModel = new S3PathModel(settings.Object, s3PathSubject, directoryTranferUtility.Object, fileSystem);

        }


        [Test]
        public void FilePathModel_UploadToS3_Directory()
        {
            this.s3PathModel.UploadToS3(this.directoryName, "AnyBucket");

            this.directoryTranferUtility.Verify(x => x.UploadDirectory(
                directoryName, @"AnyBucket", @"*.*", System.IO.SearchOption.AllDirectories ));
        }

        [Test]
        [Ignore("Need a way to mock FileStream to run as a unit test")]
        public void FilePathModel_UploadToS3_File()
        {
            this.s3PathModel.UploadToS3(this.filePath, @"AnyBucket");

            this.directoryTranferUtility.Verify(x => x.Upload(
                filePath, @"AnyBucket"));
        }
    }
}


