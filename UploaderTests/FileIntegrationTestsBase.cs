using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace UploaderTests
{
    public class FileIntegrationTestsBase : AssertionHelper
    {
        protected string TempPath;

        [SetUp]
        public void BeforeEachTest()
        {
            var BasePath = @"c:\temp";
            TempPath = BasePath + Guid.NewGuid().ToString();
            Directory.CreateDirectory(TempPath);
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
    }
}
