using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    class UploaderModel
    {
        public const string WATCH_PATH_KEY = "WatchPath";
        public const string S3BUCKET_PATH_KEY = "S3BucketPath";
        public string localPath { get; set; }
        public string s3bucketPath { get; set; }
        public ReplaySubject<String> messagePasser = new ReplaySubject<String>();
        public BehaviorSubject<String> localPathSubject;
        public FileDropWatcher watcher;

        public UploaderModel()
        {
            // Set Default Paths
            this.localPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];
            this.s3bucketPath = (string)Properties.Settings.Default[S3BUCKET_PATH_KEY];
             this.localPathSubject = new BehaviorSubject<String>(localPath);
            this.watcher = new FileDropWatcher(localPath, "");

            this.messagePasser.OnNext("Uploader Model Initialized");
        }

        public void UpdateWatcher(String newPath)
        {
            this.localPathSubject.OnNext(newPath);
            Properties.Settings.Default[WATCH_PATH_KEY] = newPath;
            Properties.Settings.Default.Save();
            this.localPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
            this.watcher = new FileDropWatcher(localPath, "");
        }

        //private void SetupWatcher()
        //{
        //    string watchPath = this.localPath;
        //    Console.WriteLine("Watch Path: " + watchPath);
        //    StatusBoxUpdate("Watch path: " + watchPath);

        //    this.watcher = new FileDropWatcher(watchPath, "");

        //    var watcherObservable = watcher.Dropped
        //        // Emit Parent Directory
        //        .Select(fileDropped => (string)fileDropped.ParentPath);
        //    uploaderSubscription = watcherObservable.Subscribe(
        //            path =>
        //            {
        //                Console.WriteLine("OnNext: {0}", path);
        //                StatusBoxUpdate("Uploading " + path + " to " + textBoxS3Path.Text + " ...");
        //                UploadToS3(path, textBoxS3Path.Text);
        //            },
        //            ex => Console.WriteLine("OnError: {0}", ex.Message),
        //            () => Console.WriteLine("OnCompleted"));
        //}
    }
}
