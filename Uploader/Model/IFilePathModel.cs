﻿using System;
using System.Reactive.Subjects;

namespace Uploader.Model
{
    public interface IFilePathModel : IDisposable
    {
        BehaviorSubject<String> LocalPathSubject { get; }

        void ChangeWatchPath(string newPath);
        IObservable<string> GetObservable();
        bool IsWatching();
        void ToggleWatch();
        //FileStream WaitForFile(string fullPath);
    }
}