using NLog;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Uploader.Model;

namespace Uploader
{
    public class UploaderModel : IDisposable
    {
        public ReplaySubject<String> messagePasser;
        public BehaviorSubject<String> LocalPathSubject { get; }
        public BehaviorSubject<String> S3PathSubject { get; }
        private IFilePathModel filePathModel;
        private IS3PathModel s3PathModel;
        private IDisposable fileWatcherSubscription; //used to keep subscription from being GC'd
        private ISettings settings;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        // TODO migrate to the DI constructor
        public UploaderModel(
            ISettings _settings,
            IFilePathModel _filePathModel,
            IS3PathModel _s3PathModel,
            ReplaySubject<String> _messagePasser)
        {
            this.settings = _settings;
            this.messagePasser = _messagePasser;
            this.filePathModel = _filePathModel;
            this.s3PathModel = _s3PathModel;
            this.LocalPathSubject = this.filePathModel.LocalPathSubject;
            this.S3PathSubject = this.s3PathModel.S3PathSubject;

            // Set up File Watcher
            this.UpdateWatcher(this.settings.WatchPath);
            this.fileWatcherSubscription = this.GetSubscription();
     
            logger.Debug("Uploader Model Initialized");
        }

        public void Dispose()
        {
            if (fileWatcherSubscription != null)
            {
                fileWatcherSubscription.Dispose();
            }
            if (this.filePathModel != null)
            {
                this.filePathModel.Dispose();
            }
            if (this.s3PathModel != null)
            {
                this.s3PathModel.Dispose();
            }
        }

        public void ToggleWatch()
        {
            this.filePathModel.ToggleWatch();
        }

        public void UpdateWatcher(String newPath)
        {
            this.filePathModel.ChangeWatchPath(newPath);
        }

        public void UploadToS3(string localPath, string s3BucketPath)
        {
            this.messagePasser.OnNext($"Uploading from: {localPath} to: {s3BucketPath}");

            this.s3PathModel.UploadToS3(localPath, s3BucketPath);       
        }

        public Boolean IsWatching()
        {
           return this.filePathModel.IsWatching();
        }

        private IDisposable GetSubscription()
        {

            // TODO: Files to ignore should be user configurable
            return  this.filePathModel.GetObservable()
                .Where(path => (!Path.GetExtension(path).EndsWith("tm")))
                .Subscribe(
                    path =>
                    {
                        string s3Path = this.settings.S3Path;
                        this.messagePasser.OnNext("Uploading " + path + " to " + s3Path + " ...");
                        UploadToS3(path, s3Path);
                    },
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("OnCompleted"));
        }
    }
}
