using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Uploader;
using Moq;

namespace UploaderTests
{
    [TestClass]
    public class FileDetectorTests
    {
        [TestMethod]
        public void Test_OnChanged()
        {
            // Arrange
            var mock = new Mock<IFileDetector>();

            // Act
            mock.Raise(e => e.OnChanged += null, EventArgs.Empty);

            // Assert
            mock.VerifyAll();
        }
    }
}
