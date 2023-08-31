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
            var items = new List<string> { "abc", null, "def", null, "ghi" };
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
            var list = new List<string> { "a", null, "b", null, "c" };
            var result = list.Denullify().ToList();
            var expectedList = new List<string> { "a", "b", "c" };
            CollectionAssert.AreEqual(expectedList, result);
        }


   
    }
}
