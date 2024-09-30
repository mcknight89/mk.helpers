using Microsoft.VisualStudio.TestTools.UnitTesting;
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




        [TestMethod]
        public async Task PagedAsync_Returns_CorrectPage()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();
            int page = 2;
            int pageSize = 3;

            // Act
            var result = await source.PagedAsync(page, pageSize);

            // Assert
            Assert.AreEqual(3, result.Results.Count);
            Assert.AreEqual(4, result.Results.First().Value);
            Assert.AreEqual(6, result.Results.Last().Value);
            Assert.AreEqual(page, result.CurrentPage);
            Assert.AreEqual(10, result.RowCount);
            Assert.AreEqual(4, result.PageCount); // 10 items with pageSize 3 => 4 pages
        }



        [TestMethod]
        public async Task SkipAsync_Skips_CorrectNumber_OfElements()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();
            int skipCount = 5;

            // Act
            var result = await source.SkipAsync(skipCount).ToListAsync();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result.First().Value);
        }

        [TestMethod]
        public async Task TakeAsync_Takes_CorrectNumber_OfElements()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();
            int takeCount = 4;

            // Act
            var result = await source.TakeAsync(takeCount).ToListAsync();

            // Assert
            Assert.AreEqual(takeCount, result.Count);
            Assert.AreEqual(1, result.First().Value);
            Assert.AreEqual(4, result.Last().Value);
        }

        [TestMethod]
        public async Task ToListAsync_Converts_To_List()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();

            // Act
            var result = await source.ToListAsync();

            // Assert
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(1, result.First().Value);
            Assert.AreEqual(10, result.Last().Value);
        }

        [TestMethod]
        public async Task CountAsync_Returns_CorrectCount()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();

            // Act
            var count = await source.CountAsync();

            // Assert
            Assert.AreEqual(10, count);
        }

        [TestMethod]
        public async Task SkipAsync_WithZero_Returns_FullList()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();

            // Act
            var result = await source.SkipAsync(0).ToListAsync();

            // Assert
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(1, result.First().Value);
        }

        [TestMethod]
        public async Task TakeAsync_WithZero_Returns_EmptyList()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();

            // Act
            var result = await source.TakeAsync(0).ToListAsync();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task PagedAsync_Returns_Empty_When_PageExceedsCount()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();
            int page = 5; // With a pageSize of 3 and 10 items, page 5 should be empty
            int pageSize = 3;

            // Act
            var result = await source.PagedAsync(page, pageSize);

            // Assert
            Assert.AreEqual(0, result.Results.Count);
            Assert.AreEqual(page, result.CurrentPage);
            Assert.AreEqual(10, result.RowCount);
            Assert.AreEqual(4, result.PageCount);
        }

        [TestMethod]
        public async Task PagedAsync_Maintains_Order()
        {
            // Arrange
            var source = GetSampleData().ToAsyncEnumerable();
            int page = 1;
            int pageSize = 5;

            // Act
            var result = await source.PagedAsync(page, pageSize);

            // Assert
            Assert.AreEqual(5, result.Results.Count);
            Assert.AreEqual(1, result.Results.First().Value);
            Assert.AreEqual(5, result.Results.Last().Value);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5 }, result.Results.Select(d => d.Value).ToList());
        }

        private List<IntWrapper> GetSampleData()
        {
            return Enumerable.Range(1, 10).Select(d => new IntWrapper(d)).ToList();
        }



        // Test for integers with odd length
        [TestMethod]
        public void Median_OddNumberOfIntegers_ReturnsMiddleValue()
        {
            // Arrange
            var data = new List<int> { 1, 3, 5, 7, 9 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(5, result);
        }

        // Test for integers with even length
        [TestMethod]
        public void Median_EvenNumberOfIntegers_ReturnsAverageOfMiddleValues()
        {
            // Arrange
            var data = new List<int> { 1, 3, 5, 7 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(4, result);
        }

        // Test for doubles with odd length
        [TestMethod]
        public void Median_OddNumberOfDoubles_ReturnsMiddleValue()
        {
            // Arrange
            var data = new List<double> { 1.1, 2.2, 3.3, 4.4, 5.5 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(3.3, result);
        }

        // Test for doubles with even length
        [TestMethod]
        public void Median_EvenNumberOfDoubles_ReturnsAverageOfMiddleValues()
        {
            // Arrange
            var data = new List<double> { 1.1, 2.2, 3.3, 4.4 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(2.75, result);
        }

        // Test for decimals with odd length
        [TestMethod]
        public void Median_OddNumberOfDecimals_ReturnsMiddleValue()
        {
            // Arrange
            var data = new List<decimal> { 1.5m, 3.5m, 5.5m };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(3.5m, result);
        }

        // Test for decimals with even length
        [TestMethod]
        public void Median_EvenNumberOfDecimals_ReturnsAverageOfMiddleValues()
        {
            // Arrange
            var data = new List<decimal> { 1.5m, 3.5m, 5.5m, 7.5m };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(4.5m, result);
        }

        // Test for longs with odd length
        [TestMethod]
        public void Median_OddNumberOfLongs_ReturnsMiddleValue()
        {
            // Arrange
            var data = new List<long> { 10000000000, 30000000000, 50000000000 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(30000000000, result);
        }

        // Test for longs with even length
        [TestMethod]
        public void Median_EvenNumberOfLongs_ReturnsAverageOfMiddleValues()
        {
            // Arrange
            var data = new List<long> { 10000000000, 20000000000, 30000000000, 40000000000 };

            // Act
            var result = data.Median();

            // Assert
            Assert.AreEqual(25000000000, result);
        }




        [TestMethod]
        public void Median_LambdaSelector_ReturnsCorrectMedian()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product A", Price = 10.5m },
                new Product { Name = "Product B", Price = 20.0m },
                new Product { Name = "Product C", Price = 15.0m }
            };

            // Act
            var result = products.Median(p => p.Price);

            // Assert
            Assert.AreEqual(15.0m, result);
        }










    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class IntWrapper
    {
        public int Value { get; set; }

        public IntWrapper(int value)
        {
            Value = value;
        }
        public IntWrapper()
        {
        }
    }


    public static class TestHelpers
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                yield return item;
                await Task.Yield();
            }
        }
    }
}
