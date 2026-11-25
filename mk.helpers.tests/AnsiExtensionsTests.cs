using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mk.helpers.tests
{
    [TestClass]
    public class AnsiExtensionsTests
    {
        [TestMethod]
        public void StripAnsi_RemovesColoursCursorAndTitles()
        {
            Assert.AreEqual("warn: done", "\x1b[33;1mwarn\x1b[0m: done".StripAnsi());
            Assert.AreEqual("ab", "a\x1b[2Kb".StripAnsi());
            Assert.AreEqual("text", "\x1b]0;My Title\x07text".StripAnsi());
            Assert.AreEqual("plain", "plain".StripAnsi());
        }
    }
}
