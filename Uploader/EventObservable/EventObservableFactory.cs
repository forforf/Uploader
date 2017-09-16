using System;
using System.IO;
using FileDropWatcher;

namespace FileDropWatcher
{
    public static class EventObservableFactory
    {
        public static Observable GetEventObservable<T>(ref T obj, String eventName)
        {
            return IObservable < EventPattern < EventArgs >> Observable.FromEventPattern<EventArgs>(obj, eventName);
        }
    }
}