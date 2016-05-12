using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader
{
    public interface IFileSystemWatcherWrapper
    {
        event FileSystemEventHandler OnChanged;
        bool EnableRaisingEvents { get; set; }
        bool IncludeSubdirectories { get; set; }
        NotifyFilters NotifyFilter { get; set; }
        string Path { get; set; }
    }
}
