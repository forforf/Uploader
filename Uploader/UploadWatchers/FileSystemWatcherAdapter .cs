using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace UploadWatchers
{
    // Very thin adapter around FileSystemWatcher
    public class FileSystemWatcherAdapter : IFileSystemWatcher
    {

        private FileSystemWatcher fsw;

        public bool EnableRaisingEvents
        {
            get { return this.fsw.EnableRaisingEvents; }
            set { this.fsw.EnableRaisingEvents = value; }
        }

        public string Filter
        {
            get { return this.fsw.Filter; }
            set { this.fsw.Filter = value; }
        }

        public NotifyFilters NotifyFilter
        {
            get { return this.fsw.NotifyFilter; }
            set { this.fsw.NotifyFilter = value; }
        }

        public string Path
        {
            get { return this.fsw.Path; }
            set { this.fsw.Path = value; }
        }


        public event EventHandler<FileSystemEventArgs> Changed;
        public event EventHandler<FileSystemEventArgs> Created;
        public event EventHandler<RenamedEventArgs> Renamed;

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            EventHandler<FileSystemEventArgs> handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            };
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            EventHandler<FileSystemEventArgs> handler = Created;
            if (handler != null)
            {
                handler(this, e);
            };
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            EventHandler<RenamedEventArgs> handler = Renamed;
            if (handler != null)
            {
                handler(this, e);
            };
        }

        // Constructor
        public FileSystemWatcherAdapter(FileSystemWatcher fsw)
        {

            this.fsw = fsw;
            // Register the events of the Adapter with the underlying FileSystemWatcher instance
            this.fsw.Changed += GetChanged();
            this.fsw.Created += new FileSystemEventHandler(WatcherCreated);
            this.fsw.Renamed += new RenamedEventHandler(WatcherRenamed);
        }

        public void Dispose()
        {
           this.fsw.Dispose();
        }
        
        private FileSystemEventHandler GetChanged()
        {
            return new FileSystemEventHandler(WatcherChanged);
        }
    }
}
