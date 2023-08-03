using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using mk.helpers;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class CsvHelperTests
    {
        [TestMethod]
        public void ToCsv_WithListOfObjects_ReturnsCorrectCsvString()
        {
            // Arrange
            var items = new List<Person>
            {
                new Person { Name = "John", Age = 30, Email = "john@example.com" },
                new Person { Name = "Alice", Age = 25, Email = "alice@example.com" }
            };

            // Act
            var csv = items.ToCsv(addHeader: true);

            // Assert
            var expectedCsv = "Name,Age,Email\r\n\"John\",30,\"john@example.com\"\r\n\"Alice\",25,\"alice@example.com\"\r\n";
            Assert.AreEqual(expectedCsv, csv);
        }

        [TestMethod]
        public void ToCsv_WithArrayOfObjects_ReturnsCorrectCsvString()
        {
            // Arrange
            var items = new[]
            {
            new Person { Name = "John", Age = 30, Email = "john@example.com" },
            new Person { Name = "Alice", Age = 25, Email = "alice@example.com" }
        };

            // Act
            var csv = items.ToCsv(addHeader: true);

            // Assert
            var expectedCsv = "Name,Age,Email\r\n\"John\",30,\"john@example.com\"\r\n\"Alice\",25,\"alice@example.com\"\r\n";
            Assert.AreEqual(expectedCsv, csv);
        }

        [TestMethod]
        public void ToCsv_WithEmptyCollection_ReturnsEmptyString()
        {
            // Arrange
            var items = new List<Person>();

            // Act
            var csv = items.ToCsv();

            // Assert
            Assert.AreEqual(string.Empty, csv);
        }

        // You can add more test cases to cover different scenarios.
    }

    // Sample class for testing
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }

}
