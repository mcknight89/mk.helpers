using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers.Store;
using System;
using System.IO;


namespace mk.helpers.tests
{
    [TestClass]
    public class InMemoryStoreTests
    {
        [TestMethod]
        public void TempStream_CreateAndWriteToFile_Success()
        {
            byte[] testData = { 1, 2, 3, 4, 5 };

            using (TempStream tempStream = new TempStream())
            {
                tempStream.Write(testData, 0, testData.Length);

                Assert.IsTrue(tempStream.IsFileStream, "Expected FileStream");
                Assert.IsFalse(tempStream.IsMemoryStream, "Expected not a MemoryStream");
                Assert.AreEqual(testData.Length, tempStream.Length, "Length does not match");
            }
        }

        [TestMethod]
        public void TempStream_CreateAndDispose_DeletesFile()
        {
            byte[] testData = { 1, 2, 3, 4, 5 };
            string? filePath;

            using (TempStream tempStream = new TempStream())
            {
                filePath = (tempStream.InnerStream as FileStream)?.Name;
                tempStream.Write(testData, 0, testData.Length);
            }
            Assert.IsFalse(File.Exists(filePath), "File should not exist after disposal");
        }

        [TestMethod]
        public void TempStream_CreateAndDispose_KeepsFile()
        {
            byte[] testData = { 1, 2, 3, 4, 5 };
            string? filePath;

            using (TempStream tempStream = new TempStream(keepFileOnDispose: true))
            {
                filePath = (tempStream.InnerStream as FileStream)?.Name;
                tempStream.Write(testData, 0, testData.Length);
            }
            Assert.IsTrue(File.Exists(filePath), "File should exist after disposal");
        }
    }
}