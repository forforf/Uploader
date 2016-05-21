using System;
using System.IO;

namespace FileDropWatcher
{
    public class FileDropped : IEquatable<FileDropped>
    {
        public FileDropped()
        {
        }

        public FileDropped(FileSystemEventArgs fileEvent)
        {
            Name = fileEvent.Name;
            FullPath = fileEvent.FullPath;
            ParentPath = Path.GetDirectoryName(FullPath);
        }

        public FileDropped(string filePath)
        {
            Name = Path.GetFileName(filePath);
            FullPath = filePath;
            ParentPath = new FileInfo(filePath).Directory.FullName;
        }

        public string Name { get; set; }
        public string FullPath { get; set; }
        public string ParentPath { get; set; }

        public bool Equals(FileDropped other)
        {
            return this.FullPath == other.FullPath;
        }
    }
}
