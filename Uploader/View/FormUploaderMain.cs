using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Uploader
{
    public partial class Uploader : Form
    {
        private List<string> statusItems = new List<string>();
        private UploaderModel uploaderModel; 

        public Uploader()
        {
            InitializeComponent();
            this.uploaderModel = new UploaderModel();

            //var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            //this.directoryTransferUtility = new TransferUtility(s3Client);
        }

        private void Uploader_Load(object sender, EventArgs e)
        {
            this.StatusBoxUpdate("Starting up ...");

            // Sync Model to text box controls
            this.uploaderModel.localPathSubject.Subscribe(
                changedLocalPath => textBoxLocalPath.Text = changedLocalPath);

            this.uploaderModel.s3BucketPathSubject.Subscribe(
                changedS3BucketPath => textBoxS3Path.Text = changedS3BucketPath);

            StatusBoxUpdate("Ready.");

            // Set up observer to handle messages from model that should go on
            // the status box control
            this.uploaderModel.messagePasser.Subscribe(
                message =>
                {
                    StatusBoxUpdate(message);
                },
                ex => StatusBoxUpdate("Error: " + ex.Message),
                () => StatusBoxUpdate("Uploader Model messaging stopped."));
        }

        private void Uploader_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.uploaderModel.Dispose();
        }

        private void StatusBoxUpdate(String statusText)
        {   
            this.BeginInvoke(new Action(delegate() 
                {
                    this.statusItems.Add(statusText);
                    this.listBoxStatus.DataSource = null;
                    this.listBoxStatus.DataSource = statusItems;
                    // scroll list box
                    listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
                }));  
        }

        private void ToggleWatch()
        {
            this.uploaderModel.ToggleWatch();

            // TODO: Make into observable
            btnWatchStop.Visible = this.uploaderModel.watcher.IsWatching();
            btnWatchStart.Visible = !btnWatchStop.Visible;
        }
        

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var selectedPath = folderBrowserDialog1.SelectedPath;
                this.uploaderModel.UpdateWatcher(selectedPath);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            this.uploaderModel.UploadToS3(textBoxLocalPath.Text, textBoxS3Path.Text);
        }

        private void btnWatchStart_Click(object sender, EventArgs e)
        {
            ToggleWatch();
        }

        private void btnWatchStop_Click(object sender, EventArgs e)
        {
            ToggleWatch();
        }

        private void textBoxS3Path_TextChanged(object sender, EventArgs e)
        {
            this.uploaderModel.s3BucketPathSubject.OnNext(textBoxS3Path.Text);
        }
        
    }
}
