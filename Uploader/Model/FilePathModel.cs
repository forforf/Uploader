using System;
using System.Collections.Generic;
using System.IO;
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

        public ReplaySubject<String> MessagePasser { get; }
        public BehaviorSubject<String> LocalPathSubject { get; }
        private IWatcherObservable watcher;
        private ISettings settings;

        public FilePathModel(ISettings _settings,
            BehaviorSubject<string> _localPathSubject,
            ReplaySubject<string> _messagePasser)
        {
            this.settings = _settings;
            this.MessagePasser = _messagePasser;

            this.LocalPathSubject = _localPathSubject;


            // Set up File Watcher
            this.SetupWatcher();
            this.ChangeWatchPath(this.settings.WatchPath);

            this.MessagePasser.OnNext("FilePathModel Initialized");
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
                this.MessagePasser.OnNext("Setting up watcher ...");
                //SetupWatcher();
                this.MessagePasser.OnNext("Setup complete.");
            }

            bool watchIsOn = this.watcher.IsWatching();
            if (watchIsOn)
            {

                this.watcher.Stop();
                this.MessagePasser.OnNext("Watcher Stopped");
            }
            else
            {
                this.watcher.Start();
                this.MessagePasser.OnNext("Watching.");
            };
        }

        public void ChangeWatchPath(String newPath)
        {
            
            if (Directory.Exists(newPath) || File.Exists(newPath))
            {
                this.watcher.Path = newPath;
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
