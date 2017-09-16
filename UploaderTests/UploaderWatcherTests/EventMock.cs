using System;

namespace UploadWatcherTests
{

    // A delegate type for hooking up change notifications.
    public delegate void EventHandler(object sender, EventArgs e);

    public class EventMock
    {
        public event EventHandler<TestAEventArgs> TestA;
        public event EventHandler<TestBEventArgs> TestB;

        // Event
        public void OnTestA(TestAEventArgs e)
        {
            assignEventHandler<TestAEventArgs>(e, TestA);
        }

        // Event
        public void OnTestB(TestBEventArgs e)
        {
            assignEventHandler<TestBEventArgs>(e, TestB);
        }

        private void assignEventHandler<T>(T e, EventHandler<T> eh)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<T> handler = eh;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

    }

    public class TestAEventArgs : EventArgs
    {
        public string Message { get; }

        public TestAEventArgs(string s)
        {
            Message = s;
        }
    }

    public class TestBEventArgs : EventArgs
    {
        public string Message { get; }

        public TestBEventArgs(string s)
        {
            Message = s;
        }
    }
}
