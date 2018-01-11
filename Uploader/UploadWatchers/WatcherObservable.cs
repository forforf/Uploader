using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using UploadWatchers;

namespace Uploader.UploadWatchers
{
    public class WatcherObservable : IWatcherObservable
    {
        private IFileSystemWatcher watcher;

        public String Path
        {
            get
            {
                return watcher.Path;
            }
            set
            {
                watcher.Path = value;
            }
        }

        public WatcherObservable(IFileSystemWatcher fileSystemWatcherAdapter)
        {
            this.watcher = fileSystemWatcherAdapter;
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        public bool IsWatching()
        {
            return watcher.EnableRaisingEvents;
        }

        public void Dispose()
        {
            watcher.Dispose();
        }

        public IObservable<String> GetObservable()
        {
            // Observables that return File Path
            var changed = this.observableFromFileEvent("Changed");
            var creates = this.observableFromFileEvent("Created");
            var renames = this.observableFromRenameEvent("Renamed");

            return creates
                .Merge(changed)
                .Merge(renames)
                .Distinct();
        }

        private IObservable<String> observableFromFileEvent(String eventName)
        {
            return EventHandlerFactory
                .GetEventObservable(watcher, eventName)
                .Select(o => ((FileSystemEventArgs)o.EventArgs).FullPath);
        }

        private IObservable<String> observableFromRenameEvent(String eventName)
        {
            return EventHandlerFactory
                .GetEventObservable(watcher, eventName)
                .Select(o => ((RenamedEventArgs)o.EventArgs).FullPath);
        }
    }
}
