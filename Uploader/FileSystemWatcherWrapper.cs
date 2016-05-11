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
        private readonly FileSystemWatcher watcher;

        public event FileSystemEventHandler Changed;

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            System.Diagnostics.Debug.WriteLine("Intializing watcher wrapper");
            watcher.Changed += this.Changed;
        }

        public bool EnableRaisingEvents
        {
            get { return watcher.EnableRaisingEvents; }
            set { watcher.EnableRaisingEvents = value; }
        }
    }
}
