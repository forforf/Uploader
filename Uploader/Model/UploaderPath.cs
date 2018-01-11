using System;
using System.IO;

namespace Uploader.Model
{
    class UploaderPath
    {
        public string FullPath { get; }

        public UploaderPath(string _fullPath)
        {
            this.FullPath = _fullPath;
        }

        public Boolean IsFile()
        {
            return File.Exists(this.FullPath);
        }

        public Boolean IsDirectory()
        {
            return Directory.Exists(this.FullPath);
        }
    }
}
