using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit.Framework;
using Uploader;
using FileDropWatcher;

[TestFixture]
public class FileDropWatcherTests : FileIntegrationTestsBase
{
    [Test]
    [Timeout(2000)]
    public async Task FileDropped_NoExistingFile_StreamsDropped()
    {
        using (var watcher = new Watcher(TempPath, "Monitored.Txt"))
        {
            var firstDropped = watcher.Dropped.FirstAsync().ToTask();
            watcher.Start();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            var dropped = await firstDropped;
            Expect(dropped.Name, Is.EqualTo("Monitored.Txt"));
            Expect(dropped.FullPath, Is.EqualTo(monitoredFile));
            Expect(dropped.ParentPath, Is.EqualTo(TempPath));
        }
    }

    [Test]
    [Timeout(2000)]
    public async Task FileRenamed_NoExistingFile_StreamsDropped()
    {
        using (var watcher = new Watcher(TempPath, "Monitored.Txt"))
        {
            var firstDropped = watcher.Dropped.FirstAsync().ToTask();
            var otherFile = Path.Combine(TempPath, "Other.Txt");
            File.WriteAllText(otherFile, "foo");
            watcher.Start();

            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.Move(otherFile, monitoredFile);

            var dropped = await firstDropped;
            Expect(dropped.Name, Is.EqualTo("Monitored.Txt"));
            Expect(dropped.FullPath, Is.EqualTo(monitoredFile));
            Expect(dropped.ParentPath, Is.EqualTo(TempPath));
        }
    }

    [Test]
    [Timeout(2000)]
    public async Task Overwrite_ExistingFile_StreamsDropped()
    {
        using (var watcher = new Watcher(TempPath, "Monitored.Txt"))
        {
            var firstDropped = watcher.Dropped.FirstAsync().ToTask();
            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");
            watcher.Start();

            File.WriteAllText(monitoredFile, "bar");

            var dropped = await firstDropped;
            Expect(dropped.Name, Is.EqualTo("Monitored.Txt"));
            Expect(dropped.FullPath, Is.EqualTo(monitoredFile));
            Expect(dropped.ParentPath, Is.EqualTo(TempPath));
        }
    }

    [Test]
    [Timeout(2000)]
    public async Task PollExisting_FileBeforeStart_StreamsDropped()
    {
        using (var watcher = new Watcher(TempPath, "Monitored.Txt"))
        {
            var firstDropped = watcher.Dropped.FirstAsync().ToTask();
            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            watcher.PollExisting();

            var dropped = await firstDropped;
            Expect(dropped.Name, Is.EqualTo("Monitored.Txt"));
            Expect(dropped.FullPath, Is.EqualTo(monitoredFile));
            Expect(dropped.ParentPath, Is.EqualTo(TempPath));
        }
    }

    [Test]
    [Timeout(2000)]
    public async Task PollExisting_SecondTime_StreamsSecondTime()
    {
        using (var watcher = new Watcher(TempPath, "Monitored.Txt"))
        {
            var secondDropped = watcher.Dropped.Skip(1).FirstAsync().ToTask();
            var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
            File.WriteAllText(monitoredFile, "foo");

            watcher.PollExisting();
            watcher.PollExisting();

            var dropped = await secondDropped;
            Expect(dropped.Name, Is.EqualTo("Monitored.Txt"));
            Expect(dropped.FullPath, Is.EqualTo(monitoredFile));
            Expect(dropped.ParentPath, Is.EqualTo(TempPath));
        }
    }
}
