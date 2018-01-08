using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uploader.UploadWatchers;
using UploadWatchers;

namespace Uploader.Model
{
    public class FilePathModel : IFilePathModel, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        readonly IFileSystem fileSystem;

        public BehaviorSubject<String> LocalPathSubject { get; }
        private IWatcherObservable watcher;
        private ISettings settings;

        public FilePathModel(ISettings _settings,
            BehaviorSubject<string> _localPathSubject,
            IWatcherObservable _watcher) : this(
                _settings,
                _localPathSubject,
                _watcher,
                _fileSystem: new FileSystem() //use default implementation which calls System.IO
            )
        {
        }

        public FilePathModel(ISettings _settings,
            BehaviorSubject<string> _localPathSubject,
            IWatcherObservable _watcher,
            IFileSystem _fileSystem)

        {
            this.settings = _settings;
            this.LocalPathSubject = _localPathSubject;
            this.fileSystem = _fileSystem;

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
            logger.Debug($"New path is Directory? {fileSystem.Directory.Exists(newPath.ToString())}");
            if (fileSystem.Directory.Exists(newPath) || fileSystem.File.Exists(newPath))
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

        public FileStream WaitForFile(string fullPath)
        {
            const int TIMEOUT_MS = 20000;
            const int RETRY_DELAY_MS = 100;
            const int TRIES = TIMEOUT_MS / RETRY_DELAY_MS;
            const FileMode FILEMODE = FileMode.Open;
            const FileAccess FILEACCESS = FileAccess.ReadWrite;
            const FileShare FILESHARE = FileShare.None;

            for (int numTries = 0; numTries < TRIES; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, FILEMODE, FILEACCESS, FILESHARE);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(RETRY_DELAY_MS);
                }
            }

            return null;
        }

        private void SetupWatcher()
        {
            if (this.watcher == null)
            {
                FileSystemWatcher fsw = new FileSystemWatcher();
                FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);

                this.watcher = new WatcherObservable(fswAdapter);
            }
        }
    }
}
