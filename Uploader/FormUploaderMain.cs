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
using System.Reactive.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Security.Principal;


namespace Uploader
{
    public partial class Uploader : Form
    {
        private IDisposable uploaderSubscription;
        private TransferUtility directoryTransferUtility;

        public Uploader()
        {
            InitializeComponent();
        }

        private void Uploader_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Loading Form");

            WindowsIdentity id = WindowsIdentity.GetCurrent();
            string defaultPassword = id.User.AccountDomainSid.Value;
            Console.WriteLine("PW: " + defaultPassword);

            var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            directoryTransferUtility = new TransferUtility(s3Client);

            textBoxLocalPath.Text = (string)Properties.Settings.Default["WatchPath"];
            textBoxS3Path.Text = (string)Properties.Settings.Default["S3BucketPath"];


        }

        private void Uploader_FormClosed(object sender, FormClosedEventArgs e)
        {
            uploaderSubscription.Dispose();
        }

        private void StartWatch()
        {
            Console.WriteLine("Watch Path: " + Properties.Settings.Default["WatchPath"]);
            string watchPath = (string)Properties.Settings.Default["WatchPath"];

            var watcher = new FileDropWatcher(watchPath, "");

            var watcherObservable = watcher.Dropped
                // Emit Parent Directory
                .Select(fileDropped => (string)fileDropped.ParentPath);
            uploaderSubscription = watcherObservable.Subscribe(
                    x =>
                    {
                        Console.WriteLine("OnNext: {0}", x);
                        UploadToS3(x);
                    },
                    ex => Console.WriteLine("OnError: {0}", ex.Message),
                    () => Console.WriteLine("OnCompleted"));

            watcher.Start();
        }
        
        private void UploadToS3(string directory)
        {
            directoryTransferUtility.UploadDirectory(directory,
                                                     @"forforf-uploader");
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
                
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                string[] files = Directory.GetFiles(selectedPath);
                MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }

    }
}
