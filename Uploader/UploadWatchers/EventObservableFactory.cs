using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace UploadWatchers
{
    public static class EventHandlerFactory
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Info($"Creating empty event handler after catching exception: {e}");
                obs = Observable.Empty<EventPattern<EventArgs>>();
            }
            return obs;
           
        }
    }
}
