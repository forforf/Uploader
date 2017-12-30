using System;
using NUnit.Framework;
using System.IO;

namespace UploadeWatcherTests
{
    [TestFixture]
    public class WatcherComponentTests
    {

        private WatcherComponentMock mock;

        [SetUp]
        public void BeforeEachTest()
        {
            mock = new WatcherComponentMock();
        }


        [Test]
        public void WatcherComponent_EnableRaisingEvents()
        {
            mock.EnableRaisingEvents = true;
            Assert.True(mock.EnableRaisingEvents);

            mock.EnableRaisingEvents = false;
            Assert.False(mock.EnableRaisingEvents);
        }

        [Test]
        public void WatcherComponent_Filter()
        {
            mock.Filter = "backing filter";
            Assert.That(mock.Filter, Is.EqualTo("backing filter"));
        }

        [Test]
        public void WatcherComponent_Path()
        {
            mock.Path = "backing path";
            Assert.That(mock.Path, Is.EqualTo("backing path"));
        }

        [Test]
        public void WatcherComponent_Dispose()
        {
            Assert.False(mock.IsDisposed);
            mock.Dispose();
            Assert.True(mock.IsDisposed);
        }

        [Test]
        public void WatcherComponent_ChangedEvent()
        {
            String path = String.Empty;

            EventHandler<FileSystemEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };

            // Assign event
            mock.Changed += myEvent;

            var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, "SomeRootDir", "SomePath");

            Assert.That(path, Is.EqualTo(String.Empty));

            // Call event on mock object 
            mock.OnChanged(args);
            Assert.That(path, Is.EqualTo(@"SomeRootDir\SomePath"));

            // Reset path
            path = String.Empty;
            Assert.That(path, Is.EqualTo(String.Empty));

            //// Remove the event
            mock.Changed -= myEvent;

            mock.OnChanged(args);
            Assert.That(path, Is.EqualTo(String.Empty));
        }

        [Test]
        public void WatcherComponent_CreatedEvent()
        {
            String path = String.Empty;

            EventHandler<FileSystemEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };

            // Assign event
            mock.Created += myEvent;

            var args = new FileSystemEventArgs(WatcherChangeTypes.Created, "SomeRootDir", "SomePath");

            Assert.That(path, Is.EqualTo(String.Empty));

            // Call event on mock object 
            mock.OnCreated(args);
            Assert.That(path, Is.EqualTo(@"SomeRootDir\SomePath"));

            // Reset path
            path = String.Empty;
            Assert.That(path, Is.EqualTo(String.Empty));

            // Remove the event
            mock.Created -= myEvent;

            mock.OnCreated(args);
            Assert.That(path, Is.EqualTo(String.Empty));
        }

        [Test]
        public void WatcherComponent_RenamedEvent()
        {
            String path = String.Empty;

            EventHandler<RenamedEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };

            // Assign event
            mock.Renamed += myEvent;

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, "SomeRootDir", "SomePath", "OldPathName");

            Assert.That(path, Is.EqualTo(String.Empty));

            // Call event on mock object 
            mock.OnRenamed(args);
            Assert.That(path, Is.EqualTo(@"SomeRootDir\SomePath"));

            // Reset path
            path = String.Empty;
            Assert.That(path, Is.EqualTo(String.Empty));

            // Remove the event
            mock.Renamed -= myEvent;

            mock.OnChanged(args);
            Assert.That(path, Is.EqualTo(String.Empty));
        }
    }
}
