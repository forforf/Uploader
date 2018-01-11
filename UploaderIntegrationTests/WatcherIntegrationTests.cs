using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uploader.UploadWatchers;
using UploadWatchers;

namespace UploadWatcherIntegrationTests
{
    [TestFixture]
    public class WatcherIntegrationTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private WatcherObservable watcherObservable;
        protected string TempPath;

        [SetUp]
        public void BeforeEachTest()
        {
            TempPath = Path.Combine(@"c:\temp", Guid.NewGuid().ToString());
            logger.Debug("Setting up temporary directory: {0}", TempPath);

            Directory.CreateDirectory(TempPath);

            FileSystemWatcher fsw = new FileSystemWatcher();
            FileSystemWatcherAdapter fswAdapter = new FileSystemWatcherAdapter(fsw);

            watcherObservable = new WatcherObservable(fswAdapter)
            {
                Path = TempPath
            };
            watcherObservable.Start();
        }


        [TearDown]
        public void AfterEachTest()
        {
            logger.Debug("Tearing down temporary directory: {0}", TempPath);
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
            List<String> monitoredPaths = new List<String>();

            var timer = Observable.Timer(TimeSpan.FromMilliseconds(500));

            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => {
                    monitoredPaths.Add(eventPath);
                    return monitoredPaths;
                    })
                .TakeUntil(timer) // Force the observable to complete on timer expiration
                .ToTask();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var obs = await asyncObs;

            Assert.That(monitoredPaths.Count, Is.EqualTo(1));
            Assert.That(monitoredPaths[0], Is.EqualTo(monitoredFile));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_WhileFileInUse()
        {

            List<String> monitoredPaths = new List<String>();

            var timer = Observable.Timer(TimeSpan.FromMilliseconds(250));


            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => {
                    monitoredPaths.Add(eventPath);
                    return monitoredPaths;
                })
                .TakeUntil(timer) // Force the observable to complete on timer expiration
                .ToTask();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");

            var fs1 = new FileStream(monitoredFile, FileMode.CreateNew);
            using (StreamWriter writer = new StreamWriter(fs1, Encoding.UTF8, 512, false))
            {
                writer.Write("FoofieBarista");
                Thread.Sleep(500);
                writer.Close();

            }

            var obs = await asyncObs;

            Assert.That(monitoredPaths.Count, Is.EqualTo(1));
            Assert.That(monitoredPaths[0], Is.EqualTo(monitoredFile));
        }

        [Test]
        [Timeout(20000)]
        public async Task WatcherIntegrationTest_FileAccessibleAfterNotification()
        {
            List<String> monitoredPaths = new List<String>();

            var timer = Observable.Timer(TimeSpan.FromMilliseconds(1800));
            var asyncObs = watcherObservable.GetObservable()

                .Select(eventPath =>
                {
                    monitoredPaths.Add(eventPath);
                    return monitoredPaths;
                })
                .TakeUntil(timer) // Force the observable to complete on timer expiration
                .ToTask();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");

            var fs1 = new FileStream(monitoredFile, FileMode.CreateNew);
            string textToWrite1 = "Initial String";
            string textToWrite2 = "Final String";
            using (StreamWriter writer = new StreamWriter(fs1, Encoding.UTF8, 512, false))
            {

                writer.Write(textToWrite1);
                Thread.Sleep(100);
                writer.Write(textToWrite2);
                var obs = await asyncObs;
                writer.Close();

            }
            string writtenText = File.ReadAllText(monitoredFile);

            Assert.That(monitoredPaths.Count, Is.EqualTo(1));
            Assert.That(monitoredPaths[0], Is.EqualTo(monitoredFile));
            Assert.That(writtenText, Is.EqualTo(textToWrite1 + textToWrite2));
        }

        [Test]
        [Timeout(2000)]
        public async Task WatcherIntegrationTest_WritingMultiipleFiles()
        {

            List<String> monitoredPaths = new List<String>();

            var timer = Observable.Timer(TimeSpan.FromMilliseconds(500));


            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => {
                    monitoredPaths.Add(eventPath);
                    return monitoredPaths;
                })
                .TakeUntil(timer) // Force the observable to complete on timer expiration
                .ToTask();

            var monitoredFile1 = Path.Combine(TempPath, "Monitored1.Txt");
            var monitoredFile2 = Path.Combine(TempPath, "Monitored2.Txt");
            File.WriteAllText(monitoredFile1, "foo");
            File.WriteAllText(monitoredFile2, "bar");

            var obs = await asyncObs;

            Assert.That(monitoredPaths.Count, Is.EqualTo(2));
            Assert.That(monitoredPaths[0], Is.EqualTo(monitoredFile1));
            Assert.That(monitoredPaths[1], Is.EqualTo(monitoredFile2));
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

            var originalFile = Path.Combine(TempPath, "BeforeRename.Txt");
            var renamedFile = Path.Combine(TempPath, "Renamed.Txt");
            File.WriteAllText(originalFile, "foo");

            var asyncObs = watcherObservable.GetObservable()
                .Select(eventPath => monitorPath = eventPath)
                .FirstAsync().ToTask();

            //File.AppendAllText(monitoredFile, "bar");
            File.Move(originalFile, renamedFile);

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
