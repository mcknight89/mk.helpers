using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void ToConcurrentDictionary_SourceAndSelectors_ReturnsCorrectConcurrentDictionary()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3 };
            Func<int, int> keySelector = x => x;
            Func<int, int> elementSelector = x => x * 2;

            // Act
            var result = source.ToConcurrentDictionary(keySelector, elementSelector);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
        }

        [TestMethod]
        public void ToConcurrentDictionary_SourceSelectorsAndComparer_ReturnsCorrectConcurrentDictionary()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3 };
            Func<int, int> keySelector = x => x;
            Func<int, int> elementSelector = x => x * 2;
            var comparer = EqualityComparer<int>.Default;

            // Act
            var result = source.ToConcurrentDictionary(keySelector, elementSelector, comparer);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(4, result[2]);
            Assert.AreEqual(6, result[3]);
        }

        [TestMethod]
        public void ToConcurrentBag_Source_ReturnsCorrectConcurrentBag()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3 };

            // Act
            var result = source.ToConcurrentBag();

            // Assert
            Assert.AreEqual(3, result.Count);
            CollectionAssert.Contains(result, 1);
            CollectionAssert.Contains(result, 2);
            CollectionAssert.Contains(result, 3);
        }

        [TestMethod]
        public void IncrementAt_Dictionary_KeyExists_ValueIncremented()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { { "A", 5 } };
            var key = "A";

            // Act
            dictionary.IncrementAt(key);

            // Assert
            Assert.AreEqual(6, dictionary[key]);
        }

        [TestMethod]
        public void IncrementAt_ConcurrentDictionary_KeyExists_ValueIncremented()
        {
            // Arrange
            var dictionary = new ConcurrentDictionary<string, int> { ["A"] = 5 };
            var key = "A";

            // Act
            dictionary.IncrementAt(key);

            // Assert
            Assert.AreEqual(6, dictionary[key]);
        }

        [TestMethod]
        public void TryGet_KeyExistsInDictionary_ReturnsDefaultValue()
        {
            // Arrange
            var dictionary = new ConcurrentDictionary<int, string> { [1] = "One", [2] = "Two" };
            // Act
            var result = dictionary.TryGet(100, "Cake");

            // Assert
            Assert.AreEqual("Cake", result);
        }


        [TestMethod]
        public void TryGet_KeyExistsInDictionary_ReturnsValue()
        {
            // Arrange
            var dictionary = new ConcurrentDictionary<int, string> { [1] = "One", [2] = "Two" };
            var key = 2;

            // Act
            var result = dictionary.TryGet(key);

            // Assert
            Assert.AreEqual("Two", result);
        }

        [TestMethod]
        public void TryGet_KeyDoesNotExistInDictionary_ReturnsDefault()
        {
            // Arrange
            var dictionary = new ConcurrentDictionary<int, string> { [1] = "One", [2] = "Two" };
            var key = 3;

            // Act
            var result = dictionary.TryGet(key);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetFloat_ReadsInvariantNumber_AndFallsBack()
        {
            var meta = new Dictionary<string, string> { ["speed"] = "1.5", ["bad"] = "fast" };

            Assert.AreEqual(1.5f, meta.GetFloat("speed", 0));
            Assert.AreEqual(2f, meta.GetFloat("bad", 2));
            Assert.AreEqual(3f, meta.GetFloat("missing", 3));
        }

        [TestMethod]
        public void GetInt_GetBool_GetFloats_ReadValues()
        {
            var meta = new Dictionary<string, string> { ["frames"] = "4", ["loop"] = "True", ["angles"] = "22 22  13", ["empty"] = "" };

            Assert.AreEqual(4, meta.GetInt("frames", 1));
            Assert.AreEqual(1, meta.GetInt("missing", 1));
            Assert.IsTrue(meta.GetBool("loop"));
            Assert.IsFalse(meta.GetBool("missing"));
            CollectionAssert.AreEqual(new[] { 22f, 22f, 13f }, meta.GetFloats("angles"));
            CollectionAssert.AreEqual(new[] { 1f, 2f }, meta.GetFloats("empty", 1, 2));
        }

        [TestMethod]
        public void SameAs_ComparesKeysAndValues()
        {
            var a = new Dictionary<string, string> { ["A"] = "1", ["B"] = "2" };

            Assert.IsTrue(a.SameAs(new Dictionary<string, string> { ["B"] = "2", ["A"] = "1" }));
            Assert.IsFalse(a.SameAs(new Dictionary<string, string> { ["A"] = "1", ["B"] = "3" }));
            Assert.IsFalse(a.SameAs(new Dictionary<string, string> { ["A"] = "1" }));
            Assert.IsFalse(a.SameAs(null!));
        }
    }
}
