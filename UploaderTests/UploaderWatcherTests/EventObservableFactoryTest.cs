using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using UploadWatchers;
using UploadWatcherTests;

namespace UploadeWatcherTests
{
    [TestFixture]
    public class EventObservableFactoryTests
    {

        [SetUp]
        public void BeforeEachTest()
        {
            // Do any Setup Required
        }

        [Test]
        public void EventObservableFactoryTest_SingleEvent()
        {
            String eventMessage = "Hey, do something";
            String retrievedMessage = "";

            EventMock eventMock = new EventMock();
            // TestA is the event name we mocked out in EventMock 
            IObservable<EventPattern<EventArgs>> evtSource =
                EventHandlerFactory.GetEventObservable<EventMock>(eventMock, "TestA");

            // Select the Message passed to the event from the event source observable
            IObservable<String> source = evtSource.Select(x => ((TestAEventArgs)x.EventArgs).Message);

            // Observer to subscribe to the event observable
            // and retrieve  message
            IObserver<String> observer = Observer.Create<String>(x => retrievedMessage = x);

            // Wire up the source Event to the observer
            source.Subscribe(observer);

            // Trigger Event
            eventMock.OnTestA(new TestAEventArgs(eventMessage));

            Assert.That(eventMessage, Is.EqualTo(retrievedMessage));
        }

        [Test]
        public void EventObservableFactoryTest_NonExistingEvent()
        {
            String retrievedMessage = "";
            String noEvent = "No Event, So no message";

            EventMock eventMock = new EventMock();
            // TestNullEvent does not exist 
            IObservable<EventPattern<EventArgs>> evtSource =
                EventHandlerFactory.GetEventObservable<EventMock>(eventMock, "TestNullEvent");

            // Select the Message passed to the event from the event source observable
            IObservable<String> source = evtSource.Select(x => ((TestAEventArgs)x.EventArgs).Message);

            // Wire up the source Event to the observer
            source.Subscribe(
                x => Assert.Fail("Unexpected subscription event"),
                ex => Assert.Fail("Error subscribing to source"),
                () => retrievedMessage = noEvent
            );

            // Trigger Event
            eventMock.OnTestA(new TestAEventArgs("We are not listening to this event"));

            Assert.That(noEvent, Is.EqualTo(retrievedMessage));
        }

        [Test]
        public void EventObservableFactoryTest_EventList()
        {
            List<String> eventNames = new List<String>(){ "TestA", "TestB" };
            List<String> eventMessages = new List<String>(){ "Hey, do something", "Now do something else" };
            List<String> retrievedMessages = new List<String>();

            EventMock eventMock = new EventMock();
            // TestA is the event name we mocked out in EventMock 
            List<IObservable<EventPattern<EventArgs>>> evtSources =
                EventHandlerFactory.GetEventObservable<EventMock>(eventMock, eventNames);

            // Select the Message passed to the event from the event source observable
            IObservable<String> sourceA = evtSources[0].Select(x => ((TestAEventArgs)x.EventArgs).Message);
            IObservable<String> sourceB = evtSources[1].Select(x => ((TestBEventArgs)x.EventArgs).Message);


            // Observer to subscribe to the event observable
            // and retrieve  message
            // Note that the order of events determines the order of retrieved messages
            IObserver<String> observer = Observer.Create<String>(x => retrievedMessages.Add(x));

            // Wire up the source Event to the observer
            sourceA.Subscribe(observer);
            sourceB.Subscribe(observer);

            // Trigger EventA then Event B
            eventMock.OnTestA(new TestAEventArgs(eventMessages[0]));
            eventMock.OnTestB(new TestBEventArgs(eventMessages[1]));

            Assert.That(eventMessages[0], Is.EqualTo(retrievedMessages[0]));
            Assert.That(eventMessages[1], Is.EqualTo(retrievedMessages[1]));
        }
    }
}
