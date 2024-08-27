using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mk.helpers.tests
{
    [TestClass]
    public class ListExtensionsTests
    {

        [TestMethod]
        public void Batch_BatchItemsCorrectly()
        {
            var items = Enumerable.Range(1, 10);
            var maxItems = 3;
            var batches = items.Batch(maxItems).ToList();
            Assert.AreEqual(batches.Count(), 4);
        }


        [TestMethod]
        public void Denullify_RemoveNullItems()
        {
            var items = new List<string?> { "abc", null, "def", null, "ghi" };
            var result = items.Denullify();
            CollectionAssert.AreEqual(new[] { "abc", "def", "ghi" }, result.ToList());
        }


        [TestMethod]
        public void Permutations_GeneratePermutations()
        {
            var list = new List<int> { 1, 2, 3 };
            var permutations = list.Permutations();
            Assert.AreEqual(permutations.Count(), 6);
        }


        [TestMethod]
        public void Combinations_GenerateCombinations()
        {
            var list = new List<int> { 1, 2, 3 };
            var combinations = list.Combinations();
            Assert.AreEqual(7, combinations.Count);
        }




        [TestMethod]
        public void AddRange_AddMultipleItemsToList()
        {
            var list = new List<int> { 1, 2, 3 };
            var itemsToAdd = new[] { 4, 5, 6 };
            list.AddRange(itemsToAdd);
            var expectedList = new List<int> { 1, 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(expectedList, list);
        }


        [TestMethod]
        public void Denullify_RemoveNullItemsFromList()
        {
            var list = new List<string?> { "a", null, "b", null, "c" };
            var result = list.Denullify().ToList();
            var expectedList = new List<string> { "a", "b", "c" };
            CollectionAssert.AreEqual(expectedList, result);
        }


        [TestMethod]
        public void Median_WithDoubles_OddCount_ReturnsCorrectMedian()
        {
            // Arrange
            var numbers = new List<double> { 1.5, 2.2, 2.8, 3.0, 4.6 };

            // Act
            var result = numbers.Median();

            // Assert
            Assert.AreEqual(2.8, result);
        }

        [TestMethod]
        public void Median_WithDoubles_EvenCount_ReturnsCorrectMedian()
        {
            // Arrange
            var numbers = new List<double> { 1.5, 2.2, 2.8, 3.0 };

            // Act
            var result = numbers.Median();

            // Assert
            Assert.AreEqual(2.5, result); // (2.2 + 2.8) / 2 = 2.5
        }

        [TestMethod]
        public void Median_WithDecimals_OddCount_ReturnsCorrectMedian()
        {
            // Arrange
            var numbers = new List<decimal> { 1.5m, 2.2m, 2.8m, 3.0m, 4.6m };

            // Act
            var result = numbers.Median();

            // Assert
            Assert.AreEqual(2.8m, result);
        }

        [TestMethod]
        public void Median_WithDecimals_EvenCount_ReturnsCorrectMedian()
        {
            // Arrange
            var numbers = new List<decimal> { 1.5m, 2.2m, 2.8m, 3.0m };

            // Act
            var result = numbers.Median();

            // Assert
            Assert.AreEqual(2.5m, result); // (2.2m + 2.8m) / 2 = 2.5m
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Median_WithEmptySequence_ThrowsException()
        {
            // Arrange
            var numbers = new List<double>();

            // Act
            var result = numbers.Median();

            // Assert
            // Expect an exception
        }

        [TestMethod]
        public void Mode_WithDoubles_ReturnsCorrectMode()
        {
            // Arrange
            var numbers = new List<double> { 1.5, 2.2, 2.8, 3.0, 2.2, 4.6 };

            // Act
            var result = numbers.Mode().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2.2, result.First());
        }

        [TestMethod]
        public void Mode_WithDecimals_ReturnsCorrectMode()
        {
            // Arrange
            var numbers = new List<decimal> { 1.5m, 2.2m, 2.8m, 3.0m, 2.2m, 4.6m };

            // Act
            var result = numbers.Mode().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2.2m, result.First());
        }

        [TestMethod]
        public void Mode_WithMultipleModes_ReturnsAllModes()
        {
            // Arrange
            var numbers = new List<double> { 1.5, 2.2, 2.8, 3.0, 2.2, 4.6, 3.0 };

            // Act
            var result = numbers.Mode().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, 2.2);
            CollectionAssert.Contains(result, 3.0);
        }

        [TestMethod]
        public void Mode_WithNoRepetition_ReturnsAllElements()
        {
            // Arrange
            var numbers = new List<decimal> { 1.5m, 2.2m, 2.8m, 3.0m, 4.6m };

            // Act
            var result = numbers.Mode().ToList();

            // Assert
            Assert.AreEqual(numbers.Count, result.Count);
            CollectionAssert.AreEqual(numbers, result);
        }
    }
}

