using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit.Framework;
using UploadWatchers;
using System;
using Uploader.UploadWatchers;

namespace UploadWatcherIntegrationTests
{
    [TestFixture]
    public class WatcherIntegrationTests 
    {

        private WatcherObservable watcherObservable;
        protected string TempPath;

        [SetUp]
        public void BeforeEachTest()
        {
            TempPath = Path.Combine(@"c:\temp", Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempPath);

            FileSystemWatcher fsw = new FileSystemWatcher();
            FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);

            watcherObservable = new WatcherObservable(fswAdapter);
            watcherObservable.Path = TempPath;
            watcherObservable.Start();
        }


        [TearDown]
        public void AfterEachTest()
        {
            if (!Directory.Exists(TempPath))
            {
                return;
            }
            Directory.Delete(TempPath, true);
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_WritingNewFile()
        {
            String monitorPath = "";

            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => monitorPath = eventPath)
                .FirstAsync().ToTask();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(monitoredFile));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_ChangeExistingFile()
        {
            String monitorPath = "";

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => monitorPath = eventPath)
                .FirstAsync().ToTask();

            File.AppendAllText(monitoredFile, "bar");

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(monitoredFile));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_RenameFile()
        {
            String monitorPath = "";

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            var renamedFile = Path.Combine(TempPath, "Renamed.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => monitorPath = eventPath)
                .FirstAsync().ToTask();

            //File.AppendAllText(monitoredFile, "bar");
            File.Move(monitoredFile, renamedFile);

            var obs = await asyncObs;

            Assert.That(monitorPath, Is.EqualTo(renamedFile));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_DeletedFileDoesNotTrigger()
        {
            const string TIMEOUT = "TIMEOUT";
            String monitor = "";

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var timer = Observable.Timer(TimeSpan.FromMilliseconds(500))
                .Select(t => TIMEOUT);

            // Amb: Listen only to the Observable that is first to emit
            var asyncObs = Observable.Amb(timer, watcherObservable.GetObservable())
                .Select(emittedString => monitor = emittedString)
                .FirstAsync().ToTask();

            File.Delete(monitoredFile);

            var obs = await asyncObs;

            Assert.That(monitor, Is.EqualTo(TIMEOUT));
        }
    }
}
