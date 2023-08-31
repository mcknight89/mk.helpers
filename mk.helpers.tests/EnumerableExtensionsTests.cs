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
    }
}
