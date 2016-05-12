using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Uploader
{
    class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private FileSystemWatcher watcher;

        private event FileSystemEventHandler changed;

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            
            this.watcher = watcher;
            //Defaults
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = true;
            this.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.Security;

            System.Diagnostics.Debug.WriteLine("Intializing watcher wrapper");

            watcher.Changed += this.changed;
            this.watcher = watcher;
        }

        public event FileSystemEventHandler OnChanged
        {
            add { this.watcher.Changed += value; }
            remove { this.watcher.Changed -= value; }
        }

        public bool EnableRaisingEvents
        {
            get { return watcher.EnableRaisingEvents; }
            set { this.watcher.EnableRaisingEvents = value; }
        }

        public bool IncludeSubdirectories
        {
            get { return watcher.IncludeSubdirectories; }
            set { this.watcher.IncludeSubdirectories = value; }
        }

        public NotifyFilters NotifyFilter
        {
            get { return watcher.NotifyFilter; }
            set { this.watcher.NotifyFilter = value; }
        }

        public String Path
        {
            get { return watcher.Path; }
            set { this.watcher.Path = value; }
        }
    }
}
