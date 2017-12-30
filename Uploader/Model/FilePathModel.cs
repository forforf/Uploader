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
    public class FilePathModel : IDisposable
    {

        private ISettings settings;
        public ReplaySubject<string> messagePasser;
        public BehaviorSubject<string> localPathSubject;
        private IWatcherObservable watcher;

        public FilePathModel(ISettings _settings,
            BehaviorSubject<string> _localPathSubject,
            ReplaySubject<string> _messagePasser)
        {
            this.settings = _settings;
            this.messagePasser = _messagePasser;

            this.localPathSubject = _localPathSubject;


            // Set up File Watcher
            this.UpdateWatcher(this.settings.WatchPath);
            //this.SetupWatcher();

            this.messagePasser.OnNext("FilePathModel Initialized");
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
                this.messagePasser.OnNext("Setting up watcher ...");
                //SetupWatcher();
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
            this.settings.WatchPath = newPath;

            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
            FileSystemWatcher fsw = new FileSystemWatcher();
            FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);

            this.watcher = new WatcherObservable(fswAdapter);
            this.watcher.Path = newPath;
        }

        public Boolean IsWatching()
        {
            return this.watcher.IsWatching();
        }

        public IObservable<String> GetObservable()
        {
            return this.watcher.GetObservable();
        }

        public FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
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
