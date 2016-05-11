using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    interface IFileSystemWatcherWrapper
    {
        event FileSystemEventHandler Changed;
        bool EnableRaisingEvents { get; set; }
    }
}
