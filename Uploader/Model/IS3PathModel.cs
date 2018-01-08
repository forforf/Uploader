using System;
using System.IO;
using System.Reactive.Subjects;

namespace Uploader.Model
{
    public interface IS3PathModel : IDisposable
    {
        BehaviorSubject<String> S3PathSubject { get; }

        void UploadToS3(string localPath, string s3BucketPath);
    }
}