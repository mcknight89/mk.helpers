using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class ReflectionHelperTests
    {
        [TestMethod]
        public void IsSimpleType_ReturnsTrueForPrimitiveTypes()
        {
            Assert.IsTrue(typeof(int).IsSimpleType());
            Assert.IsTrue(typeof(string).IsSimpleType());
            Assert.IsTrue(typeof(decimal).IsSimpleType());
        }

        [TestMethod]
        public void IsSimpleType_ReturnsFalseForComplexTypes()
        {
            Assert.IsFalse(typeof(List<int>).IsSimpleType());
            Assert.IsTrue(typeof(DateTime).IsSimpleType());
        }

        [TestMethod]
        public void GetUnderlyingType_ReturnsCorrectUnderlyingType()
        {
            var eventInfo = typeof(EventClass).GetEvent("Event");
            var fieldInfo = typeof(FieldClass).GetField("Field");
            var methodInfo = typeof(MethodClass).GetMethod("Method");
            var propertyInfo = typeof(PropertyClass).GetProperty("Property");

            Assert.AreEqual(typeof(EventHandler), eventInfo.GetUnderlyingType());
            Assert.AreEqual(typeof(int), fieldInfo.GetUnderlyingType());
            Assert.AreEqual(typeof(string), methodInfo.GetUnderlyingType());
            Assert.AreEqual(typeof(int), propertyInfo.GetUnderlyingType());
        }

        [TestMethod]
        public void PublicPropertiesEqual_ReturnsTrueForEqualObjects()
        {
            var obj1 = new SampleObject { Id = 1, Name = "Alice" };
            var obj2 = new SampleObject { Id = 1, Name = "Alice" };

            Assert.IsTrue(obj1.PublicPropertiesEqual(obj2));
        }

        [TestMethod]
        public void PublicPropertiesEqual_ReturnsFalseForDifferentObjects()
        {
            var obj1 = new SampleObject { Id = 1, Name = "Alice" };
            var obj2 = new SampleObject { Id = 2, Name = "Bob" };

            Assert.IsFalse(obj1.PublicPropertiesEqual(obj2));
        }


    }
#pragma warning disable CS0067
    public class EventClass { public event EventHandler? Event; }
    public class FieldClass { public int Field; }
    public class MethodClass { public string Method() => ""; }
    public class PropertyClass { public int Property { get; set; } }
#pragma warning restore CS0067 
    public class SampleObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

}
