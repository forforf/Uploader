using Amazon.S3.Transfer;
using NLog;
using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;

namespace Uploader.Model
{
    public class S3PathModel : IS3PathModel
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public BehaviorSubject<String> S3PathSubject { get; }
        private ITransferUtility directoryTransferUtility;
        private ISettings settings;

        public S3PathModel(
            ISettings _settings,
            BehaviorSubject<String> _s3PathSubject,
            ITransferUtility _directoryTransferUtility
            )
        {
            this.settings = _settings;
            this.S3PathSubject = _s3PathSubject;
            this.directoryTransferUtility = _directoryTransferUtility;

            // Keep default S3 Path in real-time sync with control
            // ToDo: Don't use subject for this
            S3PathSubject.Subscribe(
                s3Path => this.settings.S3Path = s3Path);

            logger.Info("S3PathModel Initialized");
        }

        public void Dispose()
        {
            // TODO: Figure out how to do real clean up.
            Thread.Sleep(500);
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

                logger.Debug("Uploaded all files in directory");
            }
            if (pathObj.IsFile())
            {
                this.UploadFile(pathObj.FullPath, s3BucketPath);
                logger.Debug($"Uploaded file {pathObj.FullPath}");
            }

            if (!pathObj.IsDirectory() && !pathObj.IsFile())
            {
                String msg = $"Unable to locate file/directory: {pathObj.FullPath}";
                logger.Error(msg);
                throw new FileNotFoundException($"Unable to locate file/directory: {msg}");
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
                // We only want to wait for files that exist. If a file doesn't exist, that'a a problem.
                if (!File.Exists(fileName))
                {
                    String msg = $"Could not find file: {fileName}";
                    logger.Error(msg);
                    throw new FileNotFoundException(msg);
                }
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

