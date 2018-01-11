using NLog;
using System;
using System.IO;
using System.Reactive.Subjects;
using Uploader.UploadWatchers;
using UploadWatchers;

namespace Uploader.Model
{
    public class FilePathModel : IFilePathModel, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public BehaviorSubject<String> LocalPathSubject { get; }
        public IWatcherObservable watcher;
        private ISettings settings;

        public FilePathModel(
            ISettings _settings,
            BehaviorSubject<string> _localPathSubject,
            IWatcherObservable _watcher
            )

        {
            this.settings = _settings;
            this.LocalPathSubject = _localPathSubject;

            // Set up File Watcher
            this.watcher = _watcher;
            this.ChangeWatchPath(this.settings.WatchPath);

            logger.Debug("FilePathModel Initialized");
        }

        public void Dispose()
        {
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
        }

        public void ToggleWatch()
        {
            // This code will recreate the file watcher if it went away
            // but haven't figured out a way to test it yet
            if (this.watcher == null)
            {
                logger.Warn("watcher was null, restrting ...");
                SetupWatcher();
            }

            bool watchIsOn = this.watcher.IsWatching();
            if (watchIsOn)
            {

                this.watcher.Stop();
                logger.Info("Watcher Stopped");
            }
            else
            {
                this.watcher.Start();
                logger.Info("Watching.");
            };
        }

        public void ChangeWatchPath(String newPath)
        {
            logger.Debug($"Changing path from {this?.watcher?.Path} to {newPath}");
            logger.Debug($"New path is Directory? {Directory.Exists(newPath.ToString())}");
            if (Directory.Exists(newPath) ||  File.Exists(newPath))
            {
                if (this.watcher != null)
                {
                    this.watcher.Path = newPath;
                }
                
                this.settings.WatchPath = newPath;
                this.LocalPathSubject.OnNext(newPath);
            } else
            {
                throw new FileNotFoundException($"Unable to find file or directory: {newPath}");
            }
        }

        public Boolean IsWatching()
        {
            return this.watcher.IsWatching();
        }

        public IObservable<String> GetObservable()
        {
            return this.watcher.GetObservable();
        }

        private void SetupWatcher()
        {
            if (this.watcher == null)
            {
                FileSystemWatcher fsw = new FileSystemWatcher();
                FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);

                this.watcher = new WatcherObservable(fswAdapter)
                {
                    Path = this.settings.WatchPath
                };
                this.watcher.Stop();
            }
        }
    }
}
