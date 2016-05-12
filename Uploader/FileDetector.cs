using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    class FileDetector
    {
        private IFileSystemWatcherWrapper fileSystemWatcher;

        public FileDetector(IFileSystemWatcherWrapper fileSystemWatcher, String directory)
        {
            System.Diagnostics.Debug.WriteLine("Intializing detector");
            this.fileSystemWatcher = fileSystemWatcher;

            this.fileSystemWatcher = fileSystemWatcher;
            this.fileSystemWatcher.Path = directory;

            this.fileSystemWatcher.EnableRaisingEvents = true;
        }

        public event FileSystemEventHandler OnChanged
        {
            add { this.fileSystemWatcher.OnChanged += value; }
            remove { this.fileSystemWatcher.OnChanged -= value; }
        }
    }
}
