using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Uploader;
using Uploader.Model;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class UploaderModelTests
    {
        private Mock<ISettings> settings;
        private Mock<IFilePathModel> filePathModel;
        private Mock<IS3PathModel> s3PathModel;
        private ReplaySubject<String> messagePasser;

        [SetUp]
        public void BeforeEachTest()
        {
            this.settings = new Mock<ISettings>();
            this.filePathModel = new Mock<IFilePathModel>();
            this.s3PathModel = new Mock<IS3PathModel>();
            this.messagePasser = new ReplaySubject<String>();


            // Initialize subscription
            // Observable never ends, but will emit a fake path initially
            this.filePathModel.Setup(x => x.GetObservable())
                .Returns(Observable.Never<String>().StartWith(@"c:\foo.test"));
        }


        [Test]
        public void UploaderModel_Dispose()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);
            
            uploaderModel.Dispose();

            filePathModel.Verify(x => x.Dispose());
            s3PathModel.Verify(x => x.Dispose());
            // Not sure how to elegantly test private fileWatcherSubscription is disposed
        }

        [Test]
        public void UploaderModel_ToggleWatch()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            uploaderModel.ToggleWatch();

            filePathModel.Verify(x => x.ToggleWatch());
        }

        [Test]
        public void UploaderModel_UpdateWatcher()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            String newPath = @"new_path";
            uploaderModel.UpdateWatcher(newPath);

            filePathModel.Verify(x => x.ChangeWatchPath(newPath));
        }

        [Test]
        public void UploaderModel_UploadToS3()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            String localPath = @"local_path";
            String s3Bucket = @"S3_bucket";
            uploaderModel.UploadToS3(localPath, s3Bucket);

            s3PathModel.Verify(x => x.UploadToS3(localPath, s3Bucket));
        }

        [Test]
        public void UploaderModel_IsWatching()
        {
            var uploaderModel = new UploaderModel(
                    settings.Object,
                    filePathModel.Object,
                    s3PathModel.Object,
                    messagePasser);

            uploaderModel.IsWatching();

            filePathModel.Verify(x => x.IsWatching());
        }

        [Test]
        public void Select_Complete()

        {
            //// Working Now Finally
            //var scheduler = new TestScheduler();
            //var wasExecuted = false;
            //scheduler.Schedule(() => wasExecuted = true);
            //Assert.False(wasExecuted);
            //scheduler.AdvanceBy(1); //execute 1 tick of queued actions
            //Assert.True(wasExecuted);

            //***********
            //Working too
            var scheduler = new TestScheduler();
            var expected = new[] { 'a', 'b', 'c' };
            var actual = new List<List<char>>();
            var test = new List<char>();

            var subject = new Subject<char>();

            subject.AsObservable().
               //Throttle(TimeSpan.FromMilliseconds(500), scheduler).
               //Subscribe(i => actual.Add(new List<char>(i)));
               Subscribe(i => FooZee(i, test));

            scheduler.Schedule(TimeSpan.FromMilliseconds(100), () => subject.OnNext('a'));
            scheduler.Schedule(TimeSpan.FromMilliseconds(200), () => subject.OnNext('b'));
            scheduler.Schedule(TimeSpan.FromMilliseconds(300), () => subject.OnNext('c'));
            scheduler.Start();

            Assert.That(expected, Is.EqualTo(test));


            //*******************************

            //var scheduler = new TestScheduler();

            //var input = scheduler.CreateHotObservable(
            //    OnNext(100, "abc"),
            //    OnNext(200, "def"),
            //    OnNext(250, "ghi"),
            //    OnNext(300, "pqr"),
            //    OnNext(450, "xyz"),
            //    OnCompleted<string>(500)
            //    );

            //var results = scheduler.Start(
            //    () => input.Buffer(() => input.Throttle(TimeSpan.FromTicks(100), scheduler))
            //               .Select(b => string.Join(",", b)),
            //    created: 50,
            //    subscribed: 150,
            //    disposed: 600);

            //ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<string>>[] {
            //        OnNext(400, "def,ghi,pqr"),
            //        OnNext(500, "xyz"),
            //        OnCompleted<string>(500)
            //    });

            //ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
            //        Subscribe(150, 500),
            //        Subscribe(150, 400),
            //        Subscribe(400, 500)
            //    });

        }

        private char FooZee(char x, List<char> test )
        {
            test.Add(x);
            Console.WriteLine("Added: " + x.ToString());
            return x;
        }

        //public Recorded<Notification<T>> OnNext<T>(long ticks, T value)

        //{

        //    return new Recorded<Notification<T>>(ticks, (new Notification<T>.OnNext(value));

        //}



        //public Recorded<Notification<T>> OnCompleted<T>(long ticks)

        //{

        //    return new Recorded<Notification<T>>(ticks, new Notification<T>.OnCompleted());

        //}



        //public Recorded<Notification<T>> OnError<T>(long ticks, Exception exception)

        //{

        //    return new Recorded<Notification<T>>(ticks,
        //        new Notification<T>.OnError(exception));

        //}



        //public Subscription Subscribe(long start, long end)

        //{

        //    return new Subscription(start, end);

        //}



        //public Subscription Subscribe(long start)

        //{

        //    return new Subscription(start);

        //}
    }
}


