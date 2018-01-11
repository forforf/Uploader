using Amazon.S3.Transfer;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Reactive.Subjects;
using Uploader;
using Uploader.Model;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class S3PathModelTests
    {
        private Mock<ISettings> settings;
        private BehaviorSubject<String> s3PathSubject; 
        private Mock<ITransferUtility> directoryTranferUtility;

        private String directoryName;
        private String filePath;
        private S3PathModel s3PathModel;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.s3PathSubject = new BehaviorSubject<String>("S3_Path");
            this.directoryTranferUtility = new Mock<ITransferUtility>();
            this.directoryName = GetTemporaryDirectory();
            this.filePath = GetTemporaryFile(directoryName);

            this.settings.Setup(x => x.S3Path).Returns("S3_Path");

            this.s3PathModel = new S3PathModel(settings.Object, s3PathSubject, directoryTranferUtility.Object);
        }

        [TearDown]
        public void AfterEachTest()
        {
            this.s3PathModel.Dispose();
            if (!Directory.Exists(this.directoryName))
            {
                return;
            }
            Directory.Delete(this.directoryName, true);
        }


        [Test]
        public void S3PathModel_UploadToS3_Directory()
        {
            this.s3PathModel.UploadToS3(this.directoryName, "AnyBucket");

            this.directoryTranferUtility.Verify(x => x.UploadDirectory(
                directoryName, @"AnyBucket", @"*.*", System.IO.SearchOption.AllDirectories ));
        }

        [Test]
        public void FilePathModel_UploadToS3_File()
        {
            this.s3PathModel.UploadToS3(this.filePath, @"AnyBucket");

            this.directoryTranferUtility.Verify(x => x.Upload(
                filePath, @"AnyBucket"));
        }

        // Yes, I know I'm creating files for a unit test, the complexity
        // involved in injecting a file system interface is just not worth
        // the hassle.
        private String GetTemporaryDirectory()
        {
            String tempDirectory = @"c:\temp\uploader_tests_temp";
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private String GetTemporaryFile(String directory)
        {
            Directory.CreateDirectory(directory);
            String tempFile = Path.Combine(directory, Path.GetRandomFileName());
            File.Create(tempFile);

            return tempFile;
        }
    }
}


