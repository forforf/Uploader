using System;
using System.IO;
using System.ComponentModel;
using UploadWatchers;

namespace UploadeWatcherTests
{

    public class WatcherComponentMock : IFileSystemWatcher
    {
  
        public bool EnableRaisingEvents { get; set; }
        public EventHandlerList Events { get; }
        public string Filter { get; set; }
        public string Path { get; set; }
        public ISite Site { get; set; }

        public bool IsDisposed { get; set; }
        public bool IsRunning { get; set; }  //Public for Testing
        public string EventString { get; set; } // Public for Testing
        public NotifyFilters NotifyFilter { get; set; }

        public event EventHandler<FileSystemEventArgs> Changed;
        public event EventHandler<FileSystemEventArgs> Created;
        public event EventHandler<RenamedEventArgs> Renamed;
        public event EventHandler Disposed;

        // Triggers for event mocking
        // Calling these methods will trigger an event that should map to the
        // Events exposed throught the IFileSystemWatcher interface.

        public virtual void OnChanged(FileSystemEventArgs a)
        {
            if (Changed != null)
            {
                Changed(this, a);
            }
        }

        public virtual void OnCreated(FileSystemEventArgs a)
        {
            if (Created != null)
            {
                Created(this, a);
            }
        }

        protected virtual void OnDisposed(FileSystemEventArgs a)
        {
            if (Disposed != null)
            {
                Disposed(this, a);
                this.Dispose();
            }
        }

        public virtual void OnRenamed(RenamedEventArgs a)
        {
            if (Renamed != null)
            {
                Renamed(this, a);
            }
        }

        public void Dispose()
        {
            this.IsDisposed = true;
        }

        // Maybe verify EventObservableFactory works on this class too?
        public WatcherComponentMock()
        {
            Site = null; //Not used
            EnableRaisingEvents = false;
            Events = new EventHandlerList();
            Filter = "";
            Path = "";
            IsDisposed = false;
        }
    }
}
