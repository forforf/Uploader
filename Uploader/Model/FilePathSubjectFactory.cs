using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Uploader.Model
{

    public delegate string PathNotExistHandler(string path);

    public class FilePathSubjectFactory
    {
        private PathNotExistHandler pathNotExistHandler;

        public static BehaviorSubject<string> Make(string _path, PathNotExistHandler _pathNotExistHandler = null)
        {
            var pathNotExistHandler = _pathNotExistHandler == null ? DefaultPathNotExistHandler : _pathNotExistHandler;

            string path = Directory.Exists(_path) ? _path : pathNotExistHandler(_path);

            return new BehaviorSubject<String>(path);
        }

        private static string DefaultPathNotExistHandler(string badPath)
        {
            throw new FileNotFoundException($"Can not create FilePathSubject with non-existent path: {badPath}");
        }

        // Helper that will return current directory that can be used for PathNotExistHandler 
        public static string GetCurrentDirectory(string path)
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
