﻿using System;
using System.IO;

namespace UploadWatchers
{
    public interface IFileSystemWatcher
    {
        bool EnableRaisingEvents { get; set; }
        string Filter { get; set; }
        string Path { get; set; }
        NotifyFilters NotifyFilter { get; set; }

        event EventHandler<FileSystemEventArgs> Changed;
        event EventHandler<FileSystemEventArgs> Created;
        event EventHandler<RenamedEventArgs> Renamed;

        void Dispose();

        // Is there a subset of watcher (i.e. FileSystemWatcher)
        // events we want to enforce? None are being enforced currently
    }
}
