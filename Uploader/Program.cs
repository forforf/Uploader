using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set up Dependencies
            Settings settings = new Settings();
            string watchPath = settings.WatchPath;
            var localPathSubject = Model.FilePathSubjectFactory.Make(watchPath, Model.FilePathSubjectFactory.GetCurrentDirectory);
            var s3PathSubject = new BehaviorSubject<String>(settings.S3Path);
            var messagePasser = new ReplaySubject<string>();
            AmazonS3Client s3Client;
            TransferUtility directoryTransferUtility;
            UploaderModel uploaderModel = null;

            try
            {
                // Set up Underlying Models
                var filePathModel = new Model.FilePathModel(settings, localPathSubject, messagePasser);
                s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                directoryTransferUtility = new TransferUtility(s3Client);
                var s3PathModel = new Model.S3PathModel(settings, s3PathSubject, messagePasser, directoryTransferUtility);
                messagePasser.OnNext("AWS S3Client initialized");
                uploaderModel = new UploaderModel(
                        _settings: settings,
                        _filePathModel: filePathModel,
                        _s3PathModel: s3PathModel,
                        _messagePasser: messagePasser);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox
                    .Show("Unable to connect to AWS. This program uses an aws credential file profile named 'Uploader'. See http://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/net-dg-config-creds.html.  " + ex);
                messagePasser.OnNext("Failed to connect to AWS");
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (uploaderModel == null)
            {
                System.Windows.Forms.MessageBox
                    .Show("Unable to initialize Uploader Model");
            } else {
                Application.Run(new Uploader(uploaderModel));
            }
        }
    }
}
