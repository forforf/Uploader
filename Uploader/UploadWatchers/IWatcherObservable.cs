using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UploadWatchers
{
    public interface IWatcherObservable
    {
        string Path { get; set; }
        
        void Dispose();
        IObservable<String> GetObservable();
        bool IsWatching();
        void Start();
        void Stop();
    }
}
