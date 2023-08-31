using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;
using System;
using System.Collections.Generic;

namespace mk.helpers.tests
{

    [TestClass]
    public class CsvHelperTests
    {
        [TestMethod]
        public void ToCsv_WithListOfObjects_ReturnsCorrectCsvString()
        {
            // Arrange
            var items = new List<Person>
            {
                new Person { Name = "John", Age = 30, Email = "john@example.com", IsMarried=true, IsAByte=1 },
                new Person { Name = "Alice", Age = 25, Email = "alice@example.com", IsMarried=false, IsAByte=0  }
            };

            // Act
            var csv = items.ToCsv(addHeader: true);

            // Assert
            var expectedCsv = "Name,Age,Email,IsMarried,IsAByte\r\n\"John\",\"30\",\"john@example.com\",\"true\",\"1\"\r\n\"Alice\",\"25\",\"alice@example.com\",\"false\",\"0\"\r\n";
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
        public bool IsMarried { get; set; }
        public byte IsAByte { get; set; }
    }

}
