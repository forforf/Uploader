using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Uploader
{
    class UploaderModel : IDisposable
    {
        public const string WATCH_PATH_KEY = "WatchPath";
        public const string S3BUCKET_PATH_KEY = "S3BucketPath";
        //public string s3bucketPath { get; set; }
        public ReplaySubject<string> messagePasser = new ReplaySubject<string>();
        public BehaviorSubject<string> localPathSubject;
        public BehaviorSubject<string> s3BucketPathSubject;
        public FileDropWatcher watcher;
        private IDisposable fileWatcherSubscription;
        private TransferUtility directoryTransferUtility;

        public UploaderModel()
        {
            // Set Default Paths and set the initial value to emit on the Subject
            string localPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];
            string s3bucketPath = (string)Properties.Settings.Default[S3BUCKET_PATH_KEY];
            this.localPathSubject = new BehaviorSubject<String>(localPath);
            this.s3BucketPathSubject = new BehaviorSubject<String>(s3bucketPath);
            this.watcher = new FileDropWatcher(localPath, "");

            // Keep default S3 Path in real-time sync with control
            // ToDo: Don't use subject for this
            s3BucketPathSubject.Subscribe(
                s3Path => Properties.Settings.Default[S3BUCKET_PATH_KEY] = s3Path);

            // Set up File Watcher
            this.SetupWatcher();

            // Set up S3
            var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            this.directoryTransferUtility = new TransferUtility(s3Client);

            this.messagePasser.OnNext("Uploader Model Initialized");
        }

        public void Dispose()
        {
            if (fileWatcherSubscription != null)
            {
                fileWatcherSubscription.Dispose();
            }
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
        }

        public void ToggleWatch()
        {
            if (this.watcher == null)
            {
                this.messagePasser.OnNext("Setting up watcher ...");
                SetupWatcher();
                this.messagePasser.OnNext("Setup complete.");
            }

            bool watchIsOn = this.watcher.IsWatching();
            if (watchIsOn)
            {

                this.watcher.Stop();
                this.messagePasser.OnNext("Watcher Stopped");
            }
            else
            {
                this.watcher.Start();
                this.messagePasser.OnNext("Watching.");
            };
        }

        public void UpdateWatcher(String newPath)
        {
            this.localPathSubject.OnNext(newPath);
            Properties.Settings.Default[WATCH_PATH_KEY] = newPath;
            Properties.Settings.Default.Save();
         
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
            this.watcher = new FileDropWatcher(newPath, "");
        }

        public void UploadToS3(string localPath, string s3BucketPath)
        {
            this.messagePasser.OnNext("Uploading from: " + localPath + " to: " + s3BucketPath);
            //directoryTransferUtility.UploadDirectory(localPath, 
            //    s3BucketPath);
            directoryTransferUtility.UploadDirectory(localPath,
                                             s3BucketPath,
                                             "*.*",
                                             SearchOption.AllDirectories);

            this.messagePasser.OnNext("Upload completed");
        }


    // Private Methods

        private void SetupWatcher()
        {
            string watchPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];

            this.UpdateWatcher(watchPath);

            var watcherObservable = this.watcher.Dropped
                // Emit Parent Directory
                .Select(fileDropped => (string)fileDropped.ParentPath);

            this.fileWatcherSubscription = watcherObservable.Subscribe(
                    path =>
                    {
                        string s3Path = (string)Properties.Settings.Default[S3BUCKET_PATH_KEY];
                        this.messagePasser.OnNext("Uploading " + path + " to " + s3Path + " ...");
                        UploadToS3(path, s3Path);
                    },
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("OnCompleted"));
        }
    }
}
