﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void Paged_ReturnsCorrectPage()
        {
            var source = new[] {
                new { Name = "Alice" },
                new { Name = "Bob" },
                new { Name = "Frank" },
                new { Name = "Alice" }
            };


            var pagedResult = source.Paged(2, 3);
            var pagedResult2 = source.Paged(2, 10);

            Assert.AreEqual(pagedResult.Results[0].Name, "Alice");
            Assert.AreEqual(2, pagedResult.CurrentPage);
            Assert.AreEqual(4, pagedResult.RowCount);
            Assert.AreEqual(2, pagedResult.PageCount);
            Assert.AreEqual(1, pagedResult.PageSize);

            Assert.AreEqual(0, pagedResult2.PageSize);
        }

        [TestMethod]
        public void Yield_CreatesSequenceWithSingleItem()
        {
            // Arrange
            var item = 42;

            // Act
            var sequence = item.Yield().ToList();

            // Assert
            CollectionAssert.AreEqual(new[] { 42 }, sequence);
        }

        [TestMethod]
        public void IsAnyOf_ReturnsTrueForMatchingItem()
        {
            // Arrange
            var item = "apple";
            var list = new[] { "banana", "apple", "orange" };

            // Act
            var result = item.IsAnyOf(list);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAnyOf_ReturnsFalseForNonMatchingItem()
        {
            // Arrange
            var item = "pear";
            var list = new[] { "banana", "apple", "orange" };

            // Act
            var result = item.IsAnyOf(list);

            // Assert
            Assert.IsFalse(result);
        }


        public class Item
        {
            public string Name { get; set; }
        }


        [TestMethod]
        public void SortByReferenceOrder_SortsItemsCorrectly()
        {
            List<Item> source = new List<Item>
        {
            new Item { Name = "Frank" },
            new Item { Name = "Bob" },
            new Item { Name = "Rob" },
        };

            List<Item> orderReference = new List<Item>
        {
            new Item { Name = "Rob" },
            new Item { Name = "Bob" },
            new Item { Name = "Frank" },
        };
            var sortedResult = source.SortByReference(orderReference, (a, b) => a.Name == b.Name).ToList();

            Assert.AreEqual(3, sortedResult.Count);
            Assert.AreEqual("Rob", sortedResult[0].Name);
            Assert.AreEqual("Bob", sortedResult[1].Name);
            Assert.AreEqual("Frank", sortedResult[2].Name);
        }

        [TestMethod]
        public void SortByReferenceOrder_EmptyItemSource_ReturnsEmpty()
        {
            List<Item> source = new List<Item>();
            List<Item> orderReference = new List<Item>
        {
            new Item { Name = "Rob" },
            new Item { Name = "Bob" },
        };

            var sortedResult = source.SortByReference(orderReference, (a, b) => a.Name == b.Name).ToList();

            Assert.AreEqual(0, sortedResult.Count);
        }

        [TestMethod]
        public void SortByReferenceOrder_SortsStringsCorrectly()
        {
            List<string> listToSort = new List<string> { "Bob", "Frank", "Rob" };
            List<string> orderReference = new List<string> { "Rob", "Bob" };

            var sortedResult = listToSort.SortByReference(orderReference).ToList();

            CollectionAssert.AreEqual(new List<string> { "Rob", "Bob", "Frank" }, sortedResult);
        }

        [TestMethod]
        public void SortByReferenceOrder_EmptyStringSource_ReturnsEmpty()
        {
            List<string> listToSort = new List<string>();
            List<string> orderReference = new List<string> { "Rob", "Bob" };

            var sortedResult = listToSort.SortByReference(orderReference).ToList();

            CollectionAssert.AreEqual(new List<string>(), sortedResult);
        }

        [TestMethod]
        public void SortListByReferenceOrder_SortsStringsCorrectly()
        {
            List<string> listToSort = new List<string> { "Bob", "Frank", "Rob" };
            List<string> orderReference = new List<string> { "Rob", "Bob" };

            var sortedResult = listToSort.SortByReference(orderReference);

            CollectionAssert.AreEqual(new List<string> { "Rob", "Bob", "Frank" }, sortedResult);
        }

        [TestMethod]
        public void SortListByReferenceOrder_EmptyStringSource_ReturnsEmpty()
        {
            List<string> listToSort = new List<string>();
            List<string> orderReference = new List<string> { "Rob", "Bob" };

            var sortedResult = listToSort.SortByReference(orderReference);

            CollectionAssert.AreEqual(new List<string>(), sortedResult);
        }



        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class PersonComparer : IComparer<Person>
        {
            public int Compare(Person x, Person y)
            {
                if (x == null || y == null)
                {
                    throw new InvalidOperationException("Cannot compare null objects.");
                }

                return x.Name.CompareTo(y.Name);
            }
        }

        [TestMethod]
        public void FilterItemsUsingReferenceKeys_RemovesMissingItems()
        {
            var source = new List<string> { "Apple", "Banana", "Cherry", "Date" };
            var orderReference = new List<string> { "Cherry", "Apple", "Banana" };

            var filteredSource = source.FilterByReference(
                orderReference,
                (a, b) => a == b
            ).ToList();

            CollectionAssert.AreEqual(new List<string> { "Apple", "Banana", "Cherry" }, filteredSource);
        }

        [TestMethod]
        public void CombindFilterItemsUsingReferenceKeysWithSortByReferenceOrder()
        {
            var source = new List<Person>
            {
                new Person { Name = "Alice", Age = 30 },
                new Person { Name = "Bob", Age = 25 },
                new Person { Name = "Carol", Age = 35 },
                new Person { Name = "David", Age = 40 }
            };

            var filter = new List<Person>
            {
                new Person { Name = "Alice", Age = 30 },
                new Person { Name = "Carol", Age = 35 },
                new Person { Name = "Bob", Age = 25 }
            };

            var expectedAfterFilter = new[] { "Alice", "Bob", "Carol" };
            var expectedAfterSort = new[] { "Alice", "Carol", "Bob" };

            var filtered = source.FilterByReference(filter, (a, b) => a.Name == b.Name);

            CollectionAssert.AreEqual(expectedAfterFilter, filtered.Select(x => x.Name).ToList());

            var sorted = filtered.SortByReference(filter, (a, b) => a.Name == b.Name);

            CollectionAssert.AreEqual(expectedAfterSort, sorted.Select(x => x.Name).ToList());
        }



        [TestMethod]
        public void RemoveDuplicates_RemovesDuplicates_WithCustomComparison()
        {
            List<int> source = new List<int> { 1, 2, 2, 3, 4, 4, 5 };
            var result = source.RemoveDuplicates((x, y) => x == y).ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5 }, result);
        }

        [TestMethod]
        public void RemoveDuplicates_PreservesOrder_WithCustomComparison()
        {
            List<int> source = new List<int> { 5, 4, 3, 2, 1 };
            var result = source.RemoveDuplicates((x, y) => x == y).ToList();
            CollectionAssert.AreEqual(new List<int> { 5, 4, 3, 2, 1 }, result);
        }

        [TestMethod]
        public void RemoveDuplicates_RemovesNoDuplicates_WithAlwaysFalseComparison()
        {
            List<int> source = new List<int> { 1, 2, 3, 4, 5 };
            var result = source.RemoveDuplicates((x, y) => false).ToList();
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5 }, result);
        }

        [TestMethod]
        public void RemoveDuplicates_RemovesAllDuplicates_WithAlwaysTrueComparison()
        {
            List<int> source = new List<int> { 1, 1, 1, 1, 1 };
            var result = source.RemoveDuplicates((x, y) => true).ToList();
            Assert.AreEqual(1, result.Count); // There should be only one element left.
        }

        [TestMethod]
        public void RemoveDuplicates_RemovesDuplicates_WithCustomComparisonForComplexObject()
        {
            var source = new List<Person>
            {
                new Person { Name = "Alice", Age = 30 },
                new Person { Name = "Bob", Age = 25 },
                new Person { Name = "Carol", Age = 35 },
                new Person { Name = "David", Age = 40 },
                new Person { Name = "Alice", Age = 30 }, // Duplicate
                new Person { Name = "Carol", Age = 35 }, // Duplicate
            };

            var result = source.RemoveDuplicates((x, y) => x.Name == y.Name).ToList();

            Assert.AreEqual("Alice", result[0].Name);
            Assert.AreEqual(30, result[0].Age);

            Assert.AreEqual("Bob", result[1].Name);
            Assert.AreEqual(25, result[1].Age);

            Assert.AreEqual("Carol", result[2].Name);
            Assert.AreEqual(35, result[2].Age);

            Assert.AreEqual("David", result[3].Name);
            Assert.AreEqual(40, result[3].Age);
        }
    }

}
