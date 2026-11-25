using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace mk.helpers.tests
{
    [TestClass]
    public class NumberExtensionsTests
    {
        [TestMethod]
        public void ToFloat_IgnoresCurrentCulture()
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

                Assert.AreEqual(1.5f, "1.5".ToFloat());
                Assert.AreEqual("1.5", 1.5f.ToInvariant());
                Assert.AreEqual("2.25", 2.25.ToInvariant());
                Assert.AreEqual("0.50", 0.5f.ToInvariant("0.00"));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [TestMethod]
        public void ToFloat_ReturnsFallbackWhenNotANumber()
        {
            Assert.AreEqual(7f, ((string)null!).ToFloat(7));
            Assert.AreEqual(7f, "".ToFloat(7));
            Assert.AreEqual(7f, "abc".ToFloat(7));
            Assert.IsFalse("abc".TryToFloat(out _));
            Assert.IsTrue(" -3.25 ".TryToFloat(out var value));
            Assert.AreEqual(-3.25f, value);
        }

        [TestMethod]
        public void ToDouble_ToInt_ReadValues()
        {
            Assert.AreEqual(0.125, "0.125".ToDouble());
            Assert.AreEqual(4, "4".ToInt());
            Assert.AreEqual(9, "4.5".ToInt(9));
            Assert.IsTrue("12".TryToInt(out var number));
            Assert.AreEqual(12, number);
            Assert.IsTrue("1e3".TryToDouble(out var big));
            Assert.AreEqual(1000, big);
        }

        [TestMethod]
        public void ToFloats_ToInts_SkipEmptyParts()
        {
            CollectionAssert.AreEqual(new[] { 22f, 22f, 13f }, "22 22  13".ToFloats());
            CollectionAssert.AreEqual(new[] { 1.5f, 2f }, "1.5,,2".ToFloats(','));
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, " 1 2 3 ".ToInts());
            Assert.AreEqual(0, ((string)null!).ToFloats().Length);
        }

        [TestMethod]
        public void ToFloats_ToInts_ThrowOnTypo()
        {
            Assert.ThrowsException<FormatException>(() => "22 2x 13".ToFloats());
            Assert.ThrowsException<FormatException>(() => "1 2.5".ToInts());
        }
    }
}
