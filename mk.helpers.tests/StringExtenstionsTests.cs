using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;
using System;
using System.Globalization;
using System.Text;


namespace mk.helpers.tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void TestToKMB_String()
        {
            string data = "1234567890";
            string result = data.ToKMB();
            Assert.AreEqual("1.235B", result);
        }

        [TestMethod]
        public void TestToKMB_Decimal()
        {
            decimal num = 1234567890.0m;
            string result = num.ToKMB();
            Assert.AreEqual("1.235B", result);
        }

        [TestMethod]
        public void TestKiloFormat_Int()
        {
            int num = 1230000;
            string result = num.KiloFormat();
            Assert.AreEqual("1.23M", result);
        }

        [TestMethod]
        public void TestKiloFormat_Decimal()
        {
            decimal num = 1230000m;
            string result = num.KiloFormat();
            Assert.AreEqual("1.23M", result);
        }

        [TestMethod]
        public void TestGetDaySuffix()
        {
            int day = 21;
            string result = day.GetDaySuffix();
            Assert.AreEqual("st", result);
        }

        [TestMethod]
        public void TestToHtml()
        {
            string text = "Hello\nWorld";
            string result = text.ToHtml();
            Assert.AreEqual("Hello<br>\r\nWorld", result);
        }

        [TestMethod]
        public void TestBytesToString()
        {
            long byteCount = 1024;
            string result = byteCount.BytesToString();
            Assert.AreEqual("1 KB", result);
        }

        [TestMethod]
        public void TestIsNullOrEmpty()
        {
            string str = null;
            bool result = str.IsNullOrEmpty();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsNullOrWhiteSpace()
        {
            string str = "   ";
            bool result = str.IsNullOrWhiteSpace();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestToSlug()
        {
            string phrase = "This is a test";
            string result = phrase.ToSlug();
            Assert.AreEqual("this-is-a-test", result);
        }

        [TestMethod]
        public void TestToBytesUtf8()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBytesUtf8();
            CollectionAssert.AreEqual(new byte[] { 72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33 }, result);
        }

        [TestMethod]
        public void TestToBytesUtf32()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBytesUtf32();
            CollectionAssert.AreEqual(new byte[] { 72, 0, 0, 0, 101, 0, 0, 0, 108, 0, 0, 0, 108, 0, 0, 0, 111, 0, 0, 0, 44, 0, 0, 0, 32, 0, 0, 0, 119, 0, 0, 0, 111, 0, 0, 0, 114, 0, 0, 0, 108, 0, 0, 0, 100, 0, 0, 0, 33, 0, 0, 0 }, result);
        }

        [TestMethod]
        public void TestToTitleCase()
        {
            string str = "hello world";
            string result = str.ToTitleCase();
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void TestToTitleCase_CultureInfoName()
        {
            string str = "hello world";
            string result = str.ToTitleCase("en-US");
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void TestToTitleCase_CultureInfo()
        {
            string str = "hello world";
            CultureInfo cultureInfo = new CultureInfo("en-US");
            string result = str.ToTitleCase(cultureInfo);
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void TestToUpperCase()
        {
            string str = "hello world";
            string result = str.ToUpperCase();
            Assert.AreEqual("HELLO WORLD", result);
        }

        [TestMethod]
        public void TestToBase64()
        {
            string str = "Hello, world!";
            string result = str.ToBase64();
            Assert.AreEqual("SGVsbG8sIHdvcmxkIQ==", result);
        }


        [TestMethod]
        public void TestToBase64Bytes()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBase64Bytes();
            CollectionAssert.AreEqual(new byte[] { 72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33 }, result);
        }


        [TestMethod]
        public void TestConvertCapsToWords()
        {
            string str = "HelloWorld";
            string result = str.ConvertCapsToWords();
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void TestNormalizePostcode()
        {
            string postcode = "ab12cd";
            string result = postcode.NormalizePostcode();
            Assert.AreEqual("AB1 2CD", result);
        }

        [TestMethod]
        public void TestReplaceStart()
        {
            string str = "12345";
            string start = "12";
            string replacement = "99";
            string result = str.ReplaceStart(start, replacement);
            Assert.AreEqual("99345", result);
        }

        [TestMethod]
        public void TestComputeSimilarityLevenshtein()
        {
            string source = "kitten";
            string target = "sitting";
            double result = source.ComputeSimilarityLevenshtein(target);
            Assert.AreEqual(0.5714285714285714, result, 0.0001);
        }

        [TestMethod]
        public void TestToBytesUtf7()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBytesUtf7();
            CollectionAssert.AreEqual(new byte[] { 72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 43,65, 67,69,45 }, result);
        }

        [TestMethod]
        public void TestToBytesAscii()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBytesAscii();
            CollectionAssert.AreEqual(new byte[] { 72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33 }, result);
        }

        [TestMethod]
        public void TestToBytesUnicode()
        {
            string str = "Hello, world!";
            byte[] result = str.ToBytesUnicode();
            CollectionAssert.AreEqual(new byte[] { 72, 0, 101, 0, 108, 0, 108, 0, 111, 0, 44, 0, 32, 0, 119, 0, 111, 0, 114, 0, 108, 0, 100, 0, 33, 0 }, result);
        }

        [TestMethod]
        public void TestRemoveAccent()
        {
            string txt = "Café";
            string result = txt.RemoveAccent();
            Assert.AreEqual("Cafe", result);
        }

        [TestMethod]
        public void TestTrimStart()
        {
            string target = "   Hello, world!";
            string trimString = "   ";
            string result = target.TrimStart(trimString);
            Assert.AreEqual("Hello, world!", result);
        }

        [TestMethod]
        public void TestTrimEnd()
        {
            string target = "Hello, world!   ";
            string trimString = "   ";
            string result = target.TrimEnd(trimString);
            Assert.AreEqual("Hello, world!", result);
        }

        [TestMethod]
        public void TestToTitleCaseWithCulture()
        {
            string str = "hello world";
            string result = str.ToTitleCase(new CultureInfo("en-US"));
            Assert.AreEqual("Hello World", result);
        }


    }
}
