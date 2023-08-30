using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;

namespace mk.helpers.tests
{
    [TestClass]
    public class StaticDataTests
    {
        [TestMethod]
        public void Add_And_Get_String_Data()
        {
            string key = "Name";
            string value = "John";

            StaticData.Add(key, value);

            Assert.AreEqual(value, StaticData.Get(key));
        }

        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }


        [TestMethod]
        public void Add_And_Get_Object_Data()
        {
            string key = "Person";
            var person = new Person  { FirstName = "Alice", LastName = "Smith", Age = 30 };

            StaticData.AddObject(key, person);

            var retrievedPerson = StaticData.GetObject<Person>(key);
            Assert.IsNotNull(retrievedPerson);

            Assert.AreEqual("Alice", retrievedPerson.FirstName);
            Assert.AreEqual("Smith", retrievedPerson.LastName);
            Assert.AreEqual(30, retrievedPerson.Age);
        }

        [TestMethod]
        public void Add_Multiple_KeyValuePairs()
        {
            var keyValuePairs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Key1", "Value1"),
                new KeyValuePair<string, string>("Key2", "Value2"),
                new KeyValuePair<string, string>("Key3", "Value3")
            };

            StaticData.Add(keyValuePairs);

            Assert.AreEqual("Value1", StaticData.Get("Key1"));
            Assert.AreEqual("Value2", StaticData.Get("Key2"));
            Assert.AreEqual("Value3", StaticData.Get("Key3"));
        }

        [TestMethod]
        public void Get_Nonexistent_Key_Returns_Null()
        {
            Assert.IsNull(StaticData.Get("NonexistentKey"));
        }

        [TestMethod]
        public void GetInt_Valid_Key_Returns_Integer_Value()
        {
            string key = "Age";
            string value = "25";

            StaticData.Add(key, value);

            Assert.AreEqual(25, StaticData.GetInt(key));
        }

        [TestMethod]
        public void GetInt_Invalid_Key_Returns_Null()
        {
            string key = "InvalidAge";
            string value = "InvalidValue";

            StaticData.Add(key, value);

            Assert.IsNull(StaticData.GetInt(key));
        }

        [TestMethod]
        public void GetBoolean_Valid_Key_Returns_Boolean_Value()
        {
            string key = "IsEnabled";
            string value = "true";

            StaticData.Add(key, value);

            Assert.IsTrue(StaticData.GetBoolean(key));
        }

        [TestMethod]
        public void GetBoolean_Invalid_Key_Returns_False()
        {
            string key = "InvalidBool";
            string value = "InvalidValue";

            StaticData.Add(key, value);

            Assert.IsFalse(StaticData.GetBoolean(key));
        }
    }
}
