using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Uploader.Model;

namespace Uploader
{
    public class UploaderModel : IDisposable
    {
        public ReplaySubject<string> messagePasser;
        public BehaviorSubject<string> localPathSubject;
        public BehaviorSubject<string> s3PathSubject;
        private FilePathModel filePathModel;
        private S3PathModel s3PathModel;
        private IDisposable fileWatcherSubscription; //used to keep subscription from being GC'd
        private ISettings settings;

        // TODO migrate to the DI constructor
        public UploaderModel(ISettings _settings,
            //BehaviorSubject<string> _localPathSubject, 
            FilePathModel _filePathModel,
            S3PathModel _s3PathModel,
            ReplaySubject<string> _messagePasser)
        {
            this.settings = _settings;
            this.messagePasser = _messagePasser;
            this.filePathModel = _filePathModel;
            this.s3PathModel = _s3PathModel;
            this.localPathSubject = this.filePathModel.localPathSubject;
            this.s3PathSubject = this.s3PathModel.s3PathSubject;

            // Set up File Watcher
            this.UpdateWatcher(this.settings.WatchPath);
            this.SetupWatcher();
     
            this.messagePasser.OnNext("Uploader Model Initialized");
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
        }

        public void ToggleWatch()
        {
            this.filePathModel.ToggleWatch();
        }

        public void UpdateWatcher(String newPath)
        {
            this.filePathModel.UpdateWatcher(newPath);
        }

        public void UploadToS3(string localPath, string s3BucketPath)
        {

            var pathObj = new UploaderPath(localPath);

            this.messagePasser.OnNext("Uploading from: " + pathObj.FullPath + " to: " + s3BucketPath);

            if (pathObj.IsDirectory())
            {
                this.s3PathModel.UploadDirectory(pathObj.FullPath,
                                 s3BucketPath,
                                 "*.*",
                                 SearchOption.AllDirectories);

                this.messagePasser.OnNext("Uploaded all files in directory");
            }
            if (pathObj.IsFile())
            {
                WaitForFile(pathObj.FullPath);
                this.s3PathModel.UploadFile(pathObj.FullPath, s3BucketPath);
                this.messagePasser.OnNext("Uploaded file");
            }

            if (!pathObj.IsDirectory() && !pathObj.IsFile())
            {
                this.messagePasser.OnNext($"Unable to locate file/directory: {pathObj.FullPath}");
            }            
        }

        public Boolean IsWatching()
        {
           return this.filePathModel.IsWatching();
        }

        private void SetupWatcher()
        {

            // TODO: Files to ignore should be user configurable
            this.fileWatcherSubscription = this.filePathModel.GetObservable()
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

        private void WaitForFile(string localPath)
        {
            // We can get notified of changes to the file before the file has been unlocked
            // So we wait until the file is readable
            // Note there is still a chance another process can sneak in and grab the file, but this should work for most cases

            var fs = this.filePathModel.WaitForFile(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            if (fs == null)
            {
                throw new ArgumentNullException("Timed out trying to open " + localPath);
            }
            fs.Dispose();
        }
    }
}
