using System;
using System.IO;
using UploadWatchers;
using System.Reactive.Linq;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;

namespace UploadWatchers
{
    public static class EventHandlerFactory
    {
        public static IObservable<EventPattern<EventArgs>> GetEventObservable<T>(T eventObj, String eventName)
        {
            return EventHandlerFactory.GetObservable<T>(eventObj, eventName);
        }

        // Same as above, just accept List of events and return List of Observables
        // TODO Handle non existent event
        public static List<IObservable<EventPattern<EventArgs>>> GetEventObservable<T>(T eventObj, List<String> eventNames)
        {
            return eventNames.Select(eventName =>
            {
                return EventHandlerFactory.GetObservable<T>(eventObj, eventName);
            }).ToList();
        }

        // Return Observable from Event, if Event does not exist, return Completed Observable
        private static IObservable<EventPattern<EventArgs>> GetObservable<T>(T eventObj, String eventName)
        {
            IObservable<EventPattern<EventArgs>> obs;
            try
            {
                obs = Observable.FromEventPattern<EventArgs>(eventObj, eventName);
            } catch (System.InvalidOperationException e)
            {
                obs = Observable.Empty<EventPattern<EventArgs>>();
            }
            return obs;
           
        }
    }
}
