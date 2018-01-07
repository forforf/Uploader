using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uploader.Model
{
    public class S3PathModel : IS3PathModel
    {
        public ReplaySubject<String> MessagePasser { get; }
        public BehaviorSubject<String> S3PathSubject { get; }
        private TransferUtility directoryTransferUtility;
        private ISettings settings;

        // TODO migrate to the DI constructor
        public S3PathModel(ISettings _settings,
            BehaviorSubject<String> _s3PathSubject,
            ReplaySubject<String> _messagePasser,
            TransferUtility _directoryTransferUtility)
        {
            this.settings = _settings;
            this.MessagePasser = _messagePasser;
            this.S3PathSubject = _s3PathSubject;
            this.directoryTransferUtility = _directoryTransferUtility;


            // Keep default S3 Path in real-time sync with control
            // ToDo: Don't use subject for this
            S3PathSubject.Subscribe(
                s3Path => this.settings.S3Path = s3Path);


            this.MessagePasser.OnNext("S3PathModel Initialized");
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        public void UploadToS3(string localPath, string s3BucketPath)
        {
            var pathObj = new UploaderPath(localPath);

            if (pathObj.IsDirectory())
            {
                this.UploadDirectory(pathObj.FullPath,
                                    s3BucketPath,
                                    "*.*",
                                    SearchOption.AllDirectories);

                //this.messagePasser.OnNext("Uploaded all files in directory");
            }
            if (pathObj.IsFile())
            {
                this.UploadFile(pathObj.FullPath, s3BucketPath);
                //this.messagePasser.OnNext("Uploaded file");
            }

            if (!pathObj.IsDirectory() && !pathObj.IsFile())
            {
                throw new FileNotFoundException($"Unable to locate file/directory: {pathObj.FullPath}");
                //this.messagePasser.OnNext($"Unable to locate file/directory: {pathObj.FullPath}");
            }
        }

        private void UploadDirectory(string directory, string bucketName, string searchPattern, SearchOption searchOption)
        {
            this.directoryTransferUtility.UploadDirectory(directory, bucketName, searchPattern, searchOption);
        }

        private void UploadFile(string filePath, string bucketName)
        {

            // If File is not available, wait for it to become available
            WaitForFile(filePath);
            this.directoryTransferUtility.Upload(filePath, bucketName);
        }

        private void WaitForFile(String fileName)
        {
            const int RETRY_INTERVAL = 100;
            while (true)
            {
                //// We only want to wait for files that exist. If a file doesn't exist, that'a a problem.
                //if (!File.Exists(fileName)) {
                //    Console.WriteLine("Improperly handled error");
                //    throw new FileNotFoundException($"Could not find file: {fileName}");
                //}
                try
                {
                    using (FileStream Fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 100))
                    {
                        //the file is accessible and closed
                        break;
                    }
                }
                catch (IOException)
                {
                    //wait and retry
                    Thread.Sleep(RETRY_INTERVAL);
                }
            }
        }
    }
}

