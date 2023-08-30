using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers.Types;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers.tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeAndDeserialize_Json()
        {
            var data = new TestData { Name = "John", Age = 30 };
            byte[] serializedData = Serialization.Serialize(data, SerializationType.Json);

            TestData deserializedData = Serialization.Deserialize<TestData>(serializedData, SerializationType.Json);

            Assert.AreEqual(data.Name, deserializedData.Name);
            Assert.AreEqual(data.Age, deserializedData.Age);
        }

        [TestMethod]
        public void SerializeAndDeserialize_Bson()
        {
            var data = new TestData { Name = "Jane", Age = 25 };
            byte[] serializedData = Serialization.Serialize(data, SerializationType.Bson);

            TestData deserializedData = Serialization.Deserialize<TestData>(serializedData, SerializationType.Bson);

            Assert.AreEqual(data.Name, deserializedData.Name);
            Assert.AreEqual(data.Age, deserializedData.Age);
        }

        [TestMethod]
        public void ToJsonBytes_WithEncoding()
        {
            Serialization.SetEncoding(Encoding.UTF8);

            var data = new TestData { Name = "Alice", Age = 28 };
            byte[] jsonBytes = Serialization.ToJsonBytes(data);

            string jsonString = Encoding.UTF8.GetString(jsonBytes);
            TestData deserializedData = Serialization.FromJson<TestData>(jsonString);

            Assert.AreEqual(data.Name, deserializedData.Name);
            Assert.AreEqual(data.Age, deserializedData.Age);
        }

        [TestMethod]
        public void FromJsonBytes_WithEncoding()
        {
            Serialization.SetEncoding(Encoding.UTF8);

            var data = new TestData { Name = "Bob", Age = 35 };
            string jsonString = Serialization.ToJson(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            TestData deserializedData = Serialization.FromJsonBytes<TestData>(jsonBytes);

            Assert.AreEqual(data.Name, deserializedData.Name);
            Assert.AreEqual(data.Age, deserializedData.Age);
        }


        private class TestData
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
