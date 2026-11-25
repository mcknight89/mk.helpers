using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace mk.helpers.tests
{
    [TestClass]
    public class JsonNodeExtensionsTests
    {
        [TestMethod]
        public void ParseJsonObject_AllowsCommentsAndTrailingCommas()
        {
            var obj = "{ // note\n \"Name\": \"api\", /* x */ \"port\": 5000, }".ParseJsonObject();

            Assert.AreEqual("api", obj["name"].ReadString());
            Assert.AreEqual(5000, obj["port"].ReadInt());
            Assert.AreEqual(0, "  ".ParseJsonObject().Count);
            Assert.ThrowsException<JsonException>(() => "[1, 2]".ParseJsonObject());
        }

        [TestMethod]
        public void HasJsonComments_FindsOnlyRealComments()
        {
            Assert.IsTrue("{ \"a\": 1 // note\n }".HasJsonComments());
            Assert.IsTrue("{ /* x */ \"a\": 1 }".HasJsonComments());
            Assert.IsFalse("{ \"url\": \"http://localhost\" }".HasJsonComments());
            Assert.IsFalse("".HasJsonComments());
        }

        [TestMethod]
        public void ReadValues_ReturnNullForWrongType()
        {
            var obj = "{ \"on\": true, \"name\": 5 }".ParseJsonObject();

            Assert.AreEqual(true, obj["on"].ReadBool());
            Assert.IsNull(obj["name"].ReadString());
            Assert.IsNull(obj["missing"]!.ReadInt());
        }

        [TestMethod]
        public void GetOrCreateObject_ReusesOrCreates()
        {
            var obj = "{ \"env\": { \"A\": \"1\" }, \"bad\": 3 }".ParseJsonObject();

            Assert.AreEqual("1", obj.GetOrCreateObject("env")["A"].ReadString());
            obj.GetOrCreateObject("bad")["x"] = 1;
            obj.GetOrCreateObject("new")["y"] = 2;

            Assert.AreEqual(1, obj["bad"]!["x"].ReadInt());
            Assert.AreEqual(2, obj["new"]!["y"].ReadInt());
        }

        [TestMethod]
        public void DeepMerge_MergesChildObjects()
        {
            var baseObject = "{ \"a\": 1, \"env\": { \"X\": \"1\", \"Y\": \"2\" } }".ParseJsonObject();
            var over = "{ \"b\": 2, \"env\": { \"Y\": \"3\" } }".ParseJsonObject();

            var merged = baseObject.DeepMerge(over);

            Assert.AreEqual("{\"a\":1,\"env\":{\"X\":\"1\",\"Y\":\"3\"},\"b\":2}", merged.ToJsonString());
            Assert.AreEqual("2", baseObject["env"]!["Y"].ReadString());
        }

        [TestMethod]
        public void StringDictionary_RoundTrips()
        {
            var obj = "{ \"A\": \"x\", \"B\": 5, \"C\": null }".ParseJsonObject();

            var map = obj.ToStringDictionary();

            Assert.AreEqual("x", map["a"]);
            Assert.AreEqual("5", map["B"]);
            Assert.AreEqual("", map["C"]);
            Assert.AreEqual("{\"K\":\"v\"}", new Dictionary<string, string> { ["K"] = "v" }.ToJsonObject().ToJsonString());
        }
    }
}
