using Amazon.S3;
using Amazon.S3.Transfer;
using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Reactive.Subjects;
using System.Windows.Forms;
using Uploader.Model;
using Uploader.UploadWatchers;
using UploadWatchers;

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

            //LogManager.ThrowExceptions = true;

            // Custom NLog Logger that publishes logs to a Subject
            // We then inject that subject into the App
            // Voila, the App has access to log data.
            // see also App.config
            Target.Register<LoggerSubject>("LoggerSubject");
            Logger logger = LogManager.GetLogger("Main");
            var subjectLoggerTarget = (LoggerSubject)LogManager.Configuration.FindTargetByName("subject");

            // Set up Dependencies
            Settings settings = new Settings();
            string watchPath = settings.WatchPath;
            var localPathSubject = Model.FilePathSubjectFactory.Make(watchPath, Model.FilePathSubjectFactory.GetCurrentDirectory);
            var s3PathSubject = new BehaviorSubject<String>(settings.S3Path);
            var messagePasser = subjectLoggerTarget.Subject;
            AmazonS3Client s3Client = null;
            TransferUtility directoryTransferUtility = null;
            UploaderModel uploaderModel = null;

            FileSystemWatcher fsw = new FileSystemWatcher();
            FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);
            WatcherObservable watcherObservable = new WatcherObservable(fswAdapter);

            logger.Info("Setting up S3");
            try
            {
                s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                directoryTransferUtility = new TransferUtility(s3Client);
            }
            catch (Exception ex)
            {
                MessageBox
                    .Show($"Unable to connect to AWS. This program uses an aws credential file profile named 'Uploader'. See http://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/net-dg-config-creds.html.\n{ex}");
                logger.Info("Failed to connect to AWS");
            }
            logger.Info("AWS S3Client initialized");

            logger.Info("Setting Up Uploader Models");
            try
            {
                // Set up Underlying Models
                var filePathModel = new Model.FilePathModel(settings, localPathSubject, watcherObservable);
                var s3PathModel = new Model.S3PathModel(settings, s3PathSubject, directoryTransferUtility);

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
                logger.Info("Failed setting up Uploader Models");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (uploaderModel == null)
            {
                MessageBox
                    .Show("Unable to initialize Uploader Model");
            } else {
                Application.Run(new Uploader(uploaderModel));
            }
        }
    }
}
