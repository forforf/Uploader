using System;
using NUnit.Framework;
using Uploader.UploadWatchers;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

namespace UploadeWatcherTests
{
    [TestFixture]
    public class WatcherObservableTests
    {
        private WatcherComponentMock mock;
        private WatcherObservable watcherObservable;

        [SetUp]
        public void BeforeEachTest()
        {
            mock = new WatcherComponentMock();
            SetupChangedEvent(mock);
            SetupCreatedEvent(mock);
            SetupRenamedEvent(mock);
            watcherObservable = new WatcherObservable(mock);
        }

        [Test]
        public void WatcherObservable_Path()
        {
            String myPath = "/Some/Path";
            mock.Path = myPath;

            Assert.That(watcherObservable.Path, Is.EqualTo(myPath));
        }

        [Test]
        public void WatcherObservable_StartStop()
        {
            watcherObservable.Start();

            Assert.True(mock.EnableRaisingEvents);

            watcherObservable.Stop();
            Assert.False(mock.EnableRaisingEvents);

            watcherObservable.Start();
            Assert.True(mock.EnableRaisingEvents);
        }

        [Test]
        public void WatcherObservable_IsWatching()
        {
            mock.EnableRaisingEvents = true;
            Assert.True(watcherObservable.IsWatching());

            mock.EnableRaisingEvents = false;
            Assert.False(watcherObservable.IsWatching());
        }

        [Test]
        public void WatcherObservable_Dispose()
        {
            Assert.False(mock.IsDisposed);
            watcherObservable.Dispose();
            Assert.True(mock.IsDisposed);
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherObservable_GetObservable_CreateEvent()
        {
            String monitorPath = "";
            // Set monitorPath to the path that is updated when it is changed
            var asyncObs = watcherObservable.GetObservable()
            .Select(eventPath => monitorPath = eventPath)
            .FirstAsync().ToTask();

            //Invoke Created Event on Mock
            String newRoot = "SomeRoot";
            String newPath = "SomePath";
            String newFullPath = String.Format("{0}\\{1}", newRoot, newPath);
            var args = new FileSystemEventArgs(WatcherChangeTypes.Created, newRoot, newPath);
            mock.OnCreated(args);

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(newFullPath));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherObservable_GetObservable_ChangeEvent()
        {
            String monitorPath = "";
            // Set monitorPath to the path that is updated when it is changed
            var asyncObs = watcherObservable.GetObservable()
            .Select(eventPath => monitorPath = eventPath)
            .FirstAsync().ToTask();

            //Invoke Change Event on Mock
            String newRoot = "SomeRoot";
            String newPath = "SomePath";
            String newFullPath = String.Format("{0}\\{1}", newRoot, newPath);
            var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, newRoot, newPath);
            mock.OnChanged(args);

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(newFullPath));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherObservable_GetObservable_RenameEvent()
        {
            String monitorPath = "";
            // Set monitorPath to the path that is updated when it is changed
            var asyncObs = watcherObservable.GetObservable()
            .Select(eventPath => monitorPath = eventPath)
            .FirstAsync().ToTask();

            //Invoke Rename Event on Mock
            String newRoot = "SomeRoot";
            String newPath = "SomePath";
            String oldName = "NotImportant";
            String newFullPath = String.Format("{0}\\{1}", newRoot, newPath);
            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, newRoot, newPath, oldName);
            mock.OnRenamed(args);

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(newFullPath));
        }

        private void SetupChangedEvent(WatcherComponentMock mock)
        {
            String path = String.Empty;
            EventHandler<FileSystemEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };
            // Assign event
            mock.Changed += myEvent;
        }

        private void SetupCreatedEvent(WatcherComponentMock mock)
        {
            String path = String.Empty;
            EventHandler<FileSystemEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };
            // Assign event
            mock.Created += myEvent;
        }

        private void SetupRenamedEvent(WatcherComponentMock mock)
        {
            String path = String.Empty;
            EventHandler<RenamedEventArgs> myEvent = (_, fseArgs) => { path = fseArgs.FullPath; };
            // Assign event
            mock.Renamed += myEvent;
        }
    }
 }
