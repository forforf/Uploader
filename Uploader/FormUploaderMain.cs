using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Security.Principal;


namespace Uploader
{
    public partial class Uploader : Form
    {
        private IDisposable uploaderSubscription;
        private TransferUtility directoryTransferUtility;
        private IObservable<EventPattern<EventArgs>> browseClickObservable;
        private FileDropWatcher watcher;
        private List<string> statusItems = new List<string>();

        public Uploader()
        {
            InitializeComponent();

            var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            directoryTransferUtility = new TransferUtility(s3Client);
        }

        private void Uploader_Load(object sender, EventArgs e)
        {
            StatusBoxUpdate("Starting up ...");

            WindowsIdentity id = WindowsIdentity.GetCurrent();
            string defaultPassword = id.User.AccountDomainSid.Value;
            Console.WriteLine("PW: " + defaultPassword);

            textBoxLocalPath.Text = (string)Properties.Settings.Default["WatchPath"];
            textBoxS3Path.Text = (string)Properties.Settings.Default["S3BucketPath"];
            StatusBoxUpdate("Ready.");
        }

        private void Uploader_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(uploaderSubscription != null)
            {
                uploaderSubscription.Dispose();
            }
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
        }

        private void SetupWatcher()
        {
            string watchPath = textBoxLocalPath.Text;
            Console.WriteLine("Watch Path: " + watchPath);
            StatusBoxUpdate("Watch path: " + watchPath);

            this.watcher = new FileDropWatcher(watchPath, "");

            var watcherObservable = watcher.Dropped
                // Emit Parent Directory
                .Select(fileDropped => (string)fileDropped.ParentPath);
            uploaderSubscription = watcherObservable.Subscribe(
                    path =>
                    {
                        Console.WriteLine("OnNext: {0}", path);
                        StatusBoxUpdate("Uploading " + path + " to " + textBoxS3Path.Text + " ...");
                        UploadToS3(path, textBoxS3Path.Text);
                    },
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("OnCompleted"));
        }

        //private void StartWatch()
        //{
        //    Console.WriteLine("Watch Path: " + Properties.Settings.Default["WatchPath"]);
        //    var defaultPath = (string)Properties.Settings.Default["WatchPath"];
        //    StatusBoxUpdate("Starting Watch on Default Path: " + defaultPath);
        //    string watchPath = defaultPath;

        //    var watcher = new FileDropWatcher(watchPath, "");

        //    var watcherObservable = watcher.Dropped
        //        // Emit Parent Directory
        //        .Select(fileDropped => (string)fileDropped.ParentPath);
        //    uploaderSubscription = watcherObservable.Subscribe(
        //            path =>
        //            {
        //                Console.WriteLine("OnNext: {0}", path);
        //                UploadToS3(path, textBoxS3Path.Text);
        //            },
        //            ex => Console.WriteLine("OnError: {0}", ex.Message),
        //            () => Console.WriteLine("OnCompleted"));

        //    watcher.Start();
        //}

        private void StatusBoxUpdate(String statusText)
        {   
            this.BeginInvoke(new Action(delegate() 
                {
                    this.statusItems.Add(statusText);
                    this.listBoxStatus.DataSource = null;
                    //this.statusItems.Reverse();
                    this.listBoxStatus.DataSource = statusItems;
                    // scroll list box
                    listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
                }));  

        }

        private void ToggleWatch()
        {
            if (this.watcher == null)
            {
                //string watchPath = (string)Properties.Settings.Default["WatchPath"];
                //this.watcher = new FileDropWatcher(watchPath, "");
                StatusBoxUpdate("Setting up watcher ...");
                SetupWatcher();
                StatusBoxUpdate("Setup complete.");
            }

            bool watchIsOn = this.watcher.IsWatching();
            if (watchIsOn) 
            {

                this.watcher.Stop();
                StatusBoxUpdate("Watcher Stopped");
            }
            else
            {
                this.watcher.Start();
                StatusBoxUpdate("Watching: " + textBoxLocalPath.Text);
            };

            watchIsOn = this.watcher.IsWatching();
            Console.WriteLine("Watcher Running?: " + this.watcher.IsWatching());
            btnWatchStop.Visible = watchIsOn;
            btnWatchStart.Visible = !btnWatchStop.Visible;
        }
        
        private void UploadToS3(string localPath, string s3BucketPath)
        {
            Console.WriteLine("Uploading from: " + localPath + " to: " + s3BucketPath);
            //directoryTransferUtility.UploadDirectory(localPath, 
            //    s3BucketPath);
            directoryTransferUtility.UploadDirectory(localPath,
                                             s3BucketPath,
                                             "*.*",
                                             SearchOption.AllDirectories);

            Console.WriteLine("Upload completed");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {

                var selectedPath = folderBrowserDialog1.SelectedPath;
                // This really should be an observable that can be subscribed to
                // rather than pushing to the text box from here
                // ... sigh ... just a bit beyond me at the moement.
                textBoxLocalPath.Text = selectedPath;
                Properties.Settings.Default["WatchPath"] = selectedPath;
                Properties.Settings.Default.Save();

                // We changed paths to watch
                if (this.watcher != null)
                {
                    this.watcher.Dispose();
                }
                this.watcher = new FileDropWatcher(selectedPath, "");
                
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                string[] files = Directory.GetFiles(selectedPath);
                MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            UploadToS3(textBoxLocalPath.Text, textBoxS3Path.Text);
        }

        private void btnWatchStart_Click(object sender, EventArgs e)
        {
            ToggleWatch();
        }

        private void btnWatchStop_Click(object sender, EventArgs e)
        {
            ToggleWatch();
        }
    }
}
