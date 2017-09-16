using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UploadWatchers;
using System.ComponentModel;
using Uploader.UploadWatchers;
using System.Threading;

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
        public WatcherObservable watcher;
        private IDisposable fileWatcherSubscription;
        private TransferUtility directoryTransferUtility;

        public UploaderModel()
        {
            // Set Default Paths and set the initial value to emit on the Subject
            string localPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];
            if (!Directory.Exists(localPath))
            {
                localPath = Directory.GetCurrentDirectory();
                Properties.Settings.Default[WATCH_PATH_KEY] = localPath;
                Properties.Settings.Default.Save();
            }
            string s3bucketPath = (string)Properties.Settings.Default[S3BUCKET_PATH_KEY];
            this.localPathSubject = new BehaviorSubject<String>(localPath);
            this.s3BucketPathSubject = new BehaviorSubject<String>(s3bucketPath);
            

            // Keep default S3 Path in real-time sync with control
            // ToDo: Don't use subject for this
            s3BucketPathSubject.Subscribe(
                s3Path => Properties.Settings.Default[S3BUCKET_PATH_KEY] = s3Path);

            // Set up File Watcher
            this.SetupWatcher();

            // Set up S3
            try
            {
                var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                this.directoryTransferUtility = new TransferUtility(s3Client);
            } catch (Exception ex)
            {
                this.messagePasser.OnNext("Failed to connect to AWS");
                System.Windows.Forms.MessageBox
                    .Show("Unable to connect to AWS. This program uses an aws credential file profile named 'Uploader'. See http://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/net-dg-config-creds.html.  " + ex);
            }
            
            

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
            FileSystemWatcher fsw = new FileSystemWatcher();
            FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);
             
            this.watcher = new WatcherObservable(fswAdapter);
            this.watcher.Path = newPath;
        }

        public void UploadToS3(string localPath, string s3BucketPath)
        {
            FileAttributes attr = File.GetAttributes(localPath);
            this.messagePasser.OnNext("Uploading from: " + localPath + " to: " + s3BucketPath);

            // We can get notified of changes to the file before the file has been unlocked
            // So we wait until the file is readable
            // Note there is still a chance another process can sneak in and grab the file, but this should work for most cases

            var fs = WaitForFile(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            if (fs == null)
            {
                throw new ArgumentNullException("Timed out trying to open " + localPath);
            }
            fs.Dispose();

            if (attr.HasFlag(FileAttributes.Directory))
                directoryTransferUtility.UploadDirectory(localPath,
                                 s3BucketPath,
                                 "*.*",
                                 SearchOption.AllDirectories);
            else // it's a file
                directoryTransferUtility.Upload(localPath, s3BucketPath);


            this.messagePasser.OnNext("Upload completed");
        }


    // Private Methods

        private void SetupWatcher()
        {
            string watchPath = (string)Properties.Settings.Default[WATCH_PATH_KEY];

            this.UpdateWatcher(watchPath);

            // var watcherObservable = this.watcher.Dropped
            //    // Emit Parent Directory
            //    .Select(fileDropped => (string)fileDropped.ParentPath);

            // TODO: Files to ignore should be user configurable
            this.fileWatcherSubscription = this.watcher.GetObservable()
                .Where(path => (!Path.GetExtension(path).EndsWith("tm")))
                .Subscribe(
                    path =>
                    {
                        string s3Path = (string)Properties.Settings.Default[S3BUCKET_PATH_KEY];
                        this.messagePasser.OnNext("Uploading " + path + " to " + s3Path + " ...");
                        UploadToS3(path, s3Path);
                    },
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("OnCompleted"));
        }


        private FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(50);
                }
            }

            return null;
        }
    }
}
