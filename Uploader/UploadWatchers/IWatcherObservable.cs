using System;

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
