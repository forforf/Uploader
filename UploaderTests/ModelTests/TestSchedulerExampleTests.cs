using Microsoft.Reactive.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class TestSchedulerExampleTests
    {
        [SetUp]
        public void BeforeEachTest()
        {
        }

        [Test]
        public void TestSchedulerExample_Select_Complete()

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
    }
}


