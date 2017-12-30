using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Model
{
    public class S3PathModel : IDisposable
    {
        public ReplaySubject<string> messagePasser;
        public BehaviorSubject<string> s3PathSubject;
        private TransferUtility directoryTransferUtility;
        private ISettings settings;

        // TODO migrate to the DI constructor
        public S3PathModel(ISettings _settings,
            BehaviorSubject<string> _s3PathSubject,
            ReplaySubject<string> _messagePasser,
            TransferUtility _directoryTransferUtility)
        {
            this.settings = _settings;
            this.messagePasser = _messagePasser;
            this.s3PathSubject = _s3PathSubject;
            this.directoryTransferUtility = _directoryTransferUtility;


            // Keep default S3 Path in real-time sync with control
            // ToDo: Don't use subject for this
            s3PathSubject.Subscribe(
                s3Path => this.settings.S3Path = s3Path);


            this.messagePasser.OnNext("S3PathModel Initialized");
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        public void UploadDirectory(string directory, string bucketName, string searchPattern, SearchOption searchOption)
        {
            this.directoryTransferUtility.UploadDirectory(directory, bucketName, searchPattern, searchOption);
        }

        public void UploadFile(string filePath, string bucketName)
        {
            this.directoryTransferUtility.Upload(filePath, bucketName);
        }
    }
}

