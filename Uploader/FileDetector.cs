using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{


    public class FileDetector : IFileDetector
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

        // Factory class automatically injects a FileSystemWatcher
        public class Factory
        {
            public static IFileDetector CreateFileDetector(string directory)
            {
                var fsw = new FileSystemWatcher();
                var fsww = new FileSystemWatcherWrapper(fsw);
                return new FileDetector( fsww, directory);
            }
        }

        public event FileSystemEventHandler OnChanged
        {
            add { this.fileSystemWatcher.OnChanged += value; }
            remove { this.fileSystemWatcher.OnChanged -= value; }
        }

        public event FileSystemEventHandler OnCreated
        {
            add { this.fileSystemWatcher.OnCreated += value; }
            remove { this.fileSystemWatcher.OnCreated -= value; }
        }
    }
}
