using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Model
{
    class UploaderPath
    {
        public string FullPath { get; }
        private IFileSystem fileSystem;

        public UploaderPath(string _fullPath, IFileSystem fileSystem)
        {
            this.FullPath = _fullPath;
            this.fileSystem = fileSystem;
        }

        public Boolean IsFile()
        {
            return this.fileSystem.File.Exists(this.FullPath);
        }

        public Boolean IsDirectory()
        {
            return this.fileSystem.Directory.Exists(this.FullPath);
        }
    }
}
