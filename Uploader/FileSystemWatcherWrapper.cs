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

        public event FileSystemEventHandler Changed;

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            //Defaults
            this.watcher = watcher;
            this.EnableRaisingEvents = false;
            this.IncludeSubdirectories = true;
            this.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.Security;
            //this.Path = "";
            System.Diagnostics.Debug.WriteLine("Intializing watcher wrapper");
            this.Changed += (watchSender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Something changed FW");
            };

            watcher.Changed += this.Changed;
            this.watcher = watcher;
            //watcher.EnableRaisingEvents = this.EnableRaisingEvents;
            //watcher.IncludeSubdirectories = this.IncludeSubdirectories;
            //watcher.NotifyFilter = this.NotifyFilter;
            //watcher.Path = this.Path;
        }

        public bool EnableRaisingEvents
        {
            get { return watcher.EnableRaisingEvents; }
            set { watcher.EnableRaisingEvents = value; }
        }

        public bool IncludeSubdirectories
        {
            get { return watcher.IncludeSubdirectories; }
            set { watcher.IncludeSubdirectories = value; }
        }

        public NotifyFilters NotifyFilter
        {
            get { return watcher.NotifyFilter; }
            set { watcher.NotifyFilter = value; }
        }

        public String Path
        {
            get { return watcher.Path; }
            set { watcher.Path = value; }
        }
    }
}
