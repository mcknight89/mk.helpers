using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mk.helpers.tests
{
    [TestClass]
    public class TempStreamTests
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
                filePath = (tempStream as TempStream)?.IsFileStream == true ? (tempStream.InnerStream as FileStream)?.Name : null;
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
                filePath = (tempStream as TempStream)?.IsFileStream == true ? (tempStream.InnerStream as FileStream)?.Name : null;
                tempStream.Write(testData, 0, testData.Length);
            }

            Assert.IsNotNull(filePath, "File should exist after disposal");
            Assert.IsTrue(File.Exists(filePath), "File should exist after disposal");
        }
    }

}
