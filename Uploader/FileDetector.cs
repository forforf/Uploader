using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    class FileDetector
    {
        private readonly IFileSystemWatcherWrapper fileSystemWatcher;

        public FileDetector(IFileSystemWatcherWrapper fileSystemWatcher)
        {
            System.Diagnostics.Debug.WriteLine("Intializing detector");
            this.fileSystemWatcher = fileSystemWatcher;


            fileSystemWatcher.Changed += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Something changed");
            };
        }
    }
}
