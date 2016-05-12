using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Uploader;
using Moq;

namespace UploaderTests
{
    [TestClass]
    public class FileSystemWatcherWrapperTests
    {
        [TestMethod]
        public void Test_OnChanged()
        {
            // Arrange
            var mock = new Mock<IFileSystemWatcherWrapper>();

            // Act
            mock.Raise(e => e.OnChanged += null, EventArgs.Empty);

            // Assert
            mock.VerifyAll();
        }

        [TestMethod]
        public void Test_EnableRaisingEvents()
        {
            // Arrange
            var mock = new Mock<IFileSystemWatcherWrapper>();
            mock.SetupProperty(f => f.EnableRaisingEvents);
            IFileSystemWatcherWrapper fsw = mock.Object;

            // Act
            fsw.EnableRaisingEvents = true;

            // Assert
            Assert.AreEqual(true, fsw.EnableRaisingEvents);
        }

        [TestMethod]
        public void Test_IncludeSubdirectories()
        {
            // Arrange
            var mock = new Mock<IFileSystemWatcherWrapper>();
            mock.SetupProperty(f => f.IncludeSubdirectories);
            IFileSystemWatcherWrapper fsw = mock.Object;

            // Act
            fsw.IncludeSubdirectories = true;

            // Assert
            Assert.AreEqual(true, fsw.IncludeSubdirectories);
        }

        [TestMethod]
        public void Test_NotifyFilter()
        {
            // Arrange
            var mock = new Mock<IFileSystemWatcherWrapper>();
            mock.SetupProperty(f => f.NotifyFilter);
            IFileSystemWatcherWrapper fsw = mock.Object;

            // Act
            var filters = NotifyFilters.Attributes |
                NotifyFilters.CreationTime;
            fsw.NotifyFilter = filters; 

            // Assert
            Assert.AreEqual(filters, fsw.NotifyFilter);
        }

        [TestMethod]
        public void Test_Path()
        {
            // Arrange
            var mock = new Mock<IFileSystemWatcherWrapper>();
            mock.SetupProperty(f => f.Path);
            IFileSystemWatcherWrapper fsw = mock.Object;

            // Act
            fsw.Path = @"c:\some\path";

            // Assert
            Assert.AreEqual(@"c:\some\path", fsw.Path);
        }
    }
}
