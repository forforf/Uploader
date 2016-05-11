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
            //this.fileSystemWatcher.IncludeSubdirectories = true;
            //this.fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes |
            //    NotifyFilters.CreationTime |
            //    NotifyFilters.FileName |
            //    NotifyFilters.LastAccess |
            //    NotifyFilters.LastWrite |
            //    NotifyFilters.Size |
            //    NotifyFilters.Security;
            

            this.fileSystemWatcher.Changed += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Something changed");
            };

            this.fileSystemWatcher.EnableRaisingEvents = true;
        }
    }
}
