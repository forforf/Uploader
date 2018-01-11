using System;
using System.IO;
using System.Reactive.Subjects;

namespace Uploader.Model
{

    public delegate string PathNotExistHandler(string path);

    public class FilePathSubjectFactory
    {
        public static BehaviorSubject<string> Make(string _path, PathNotExistHandler _pathNotExistHandler = null)
        {
            var pathNotExistHandler = _pathNotExistHandler ?? DefaultPathNotExistHandler;

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
