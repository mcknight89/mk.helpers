using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;
namespace mk.helpers.tests
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        private class SimpleObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class ComplexObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public SimpleObject SubObject { get; set; }
            public List<SimpleObject> SubObjects { get; set; }
        }

        [TestMethod]
        public void ToExpandoObject_SimpleObject_ShouldConvertCorrectly()
        {
            var simpleObject = new SimpleObject
            {
                Id = 1,
                Name = "Test"
            };

            dynamic expando = simpleObject.ToExpandoObject();

            Assert.AreEqual(simpleObject.Id, expando.Id);
            Assert.AreEqual(simpleObject.Name, expando.Name);
        }

        [TestMethod]
        public void ToExpandoObject_ComplexObject_ShouldConvertCorrectly()
        {
            var complexObject = new ComplexObject
            {
                Id = 1,
                Name = "Complex Test",
                SubObject = new SimpleObject
                {
                    Id = 2,
                    Name = "Sub Test"
                },
                SubObjects = new List<SimpleObject>
            {
                new SimpleObject { Id = 3, Name = "Sub List Test 1" },
                new SimpleObject { Id = 4, Name = "Sub List Test 2" }
            }
            };

            dynamic expando = complexObject.ToExpandoObject();

            Assert.AreEqual(complexObject.Id, expando.Id);
            Assert.AreEqual(complexObject.Name, expando.Name);

            // Verify nested object
            Assert.IsNotNull(expando.SubObject);
            Assert.AreEqual(complexObject.SubObject.Id, expando.SubObject.Id);
            Assert.AreEqual(complexObject.SubObject.Name, expando.SubObject.Name);

            // Verify list of nested objects
            Assert.IsNotNull(expando.SubObjects);
            Assert.AreEqual(complexObject.SubObjects.Count, ((List<object>)expando.SubObjects).Count);

            for (int i = 0; i < complexObject.SubObjects.Count; i++)
            {
                var subObject = complexObject.SubObjects[i];
                var expandoSubObject = ((List<object>)expando.SubObjects)[i] as IDictionary<string, object>;

                Assert.AreEqual(subObject.Id, expandoSubObject["Id"]);
                Assert.AreEqual(subObject.Name, expandoSubObject["Name"]);
            }
        }

        [TestMethod]
        public void ToExpandoObject_NullObject_ShouldReturnNull()
        {
            SimpleObject nullObject = null;

            var expando = nullObject.ToExpandoObject();

            Assert.IsNull(expando);
        }

        [TestMethod]
        public void IsIn_ShouldReturnTrueIfInValues()
        {
            int value = 2;
            bool result = value.IsIn(1, 2, 3);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsIn_ShouldReturnFalseIfNotInValues()
        {
            int value = 4;
            bool result = value.IsIn(1, 2, 3);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AsDictionary_ShouldConvertObjectToDictionary()
        {
            var simpleObject = new SimpleObject
            {
                Id = 1,
                Name = "Test"
            };

            var dictionary = simpleObject.AsDictionary();

            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(simpleObject.Id, dictionary["Id"]);
            Assert.AreEqual(simpleObject.Name, dictionary["Name"]);
        }

        [TestMethod]
        public void IsDefault_ShouldReturnTrueForDefaultValue()
        {
            int defaultInt = 0;
            bool result = defaultInt.IsDefault();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsDefault_ShouldReturnFalseForNonDefaultValue()
        {
            int nonDefaultInt = 1;
            bool result = nonDefaultInt.IsDefault();
            Assert.IsFalse(result);
        }


    }
}