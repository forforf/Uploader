using System;
using NUnit.Framework;
using Moq;
using Uploader;
using UploadWatchers;

namespace UploaderTests.ModelTests
{
    [TestFixture]
    public class UploaderModelTests
    {
        private UploaderModel uploaderModel;
        
        [SetUp]
        public void BeforeEachTest()
        {
            var mock = new Mock<IWatcherObservable>();
            //mock = new WatcherComponentMock();
        }


        [Test]
        public void UploaderModel_FOO()
        {
            //mock.EnableRaisingEvents = true;
            //Assert.True(mock.EnableRaisingEvents);

            //mock.EnableRaisingEvents = false;
            //Assert.False(mock.EnableRaisingEvents);
        }
    }
}


