using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace mk.helpers.tests
{
    [TestClass]
    public class CommandArgsTests
    {
        [TestMethod]
        public void Split_RespectsQuotes()
        {
            CollectionAssert.AreEqual(new[] { "run", "--project", "C:\\My App\\api.csproj", "" }, CommandArgs.Split("run  --project \"C:\\My App\\api.csproj\" \"\""));
            CollectionAssert.AreEqual(new[] { "say", "a \"b\"" }, CommandArgs.Split("say \"a \\\"b\\\"\""));
            Assert.AreEqual(0, CommandArgs.Split("   ").Count);
        }

        [TestMethod]
        public void Split_KeepsBackslashesNotBeforeQuote()
        {
            CollectionAssert.AreEqual(new[] { "C:\\a\\b" }, CommandArgs.Split("C:\\a\\b"));
            CollectionAssert.AreEqual(new[] { "C:\\dir\\" }, CommandArgs.Split("\"C:\\dir\\\\\""));
        }

        [TestMethod]
        public void Join_ThenSplit_GivesSameArgs()
        {
            var args = new List<string> { "plain", "with space", "", "quote\"inside", "C:\\dir\\", "C:\\dir with space\\", "a&b", "back\\\\\"slash" };

            var line = CommandArgs.Join(args);

            CollectionAssert.AreEqual(args, CommandArgs.Split(line));
        }

        [TestMethod]
        public void Quote_LeavesSimpleArgsAlone()
        {
            Assert.AreEqual("--port=5000", CommandArgs.Quote("--port=5000"));
            Assert.AreEqual("\"C:\\dir with space\\\\\"", CommandArgs.Quote("C:\\dir with space\\"));
        }
    }
}
