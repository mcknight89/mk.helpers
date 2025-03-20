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
    }
}
