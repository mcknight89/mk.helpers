using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class IntExtensionsTests
    {
        [TestMethod]
        public void ZeroToNull_NonNullableValue_Zero_ReturnsNull()
        {
            int value = 0;
            int? result = value.ZeroToNull();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ZeroToNull_NonNullableValue_NonZero_ReturnsOriginalValue()
        {
            int value = 42;
            int? result = value.ZeroToNull();
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void ZeroToNull_NullableValue_Null_ReturnsNull()
        {
            int? value = null;
            int? result = value.ZeroToNull();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ZeroToNull_NullableValue_Zero_ReturnsNull()
        {
            int? value = 0;
            int? result = value.ZeroToNull();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ZeroToNull_NullableValue_NonZero_ReturnsOriginalValue()
        {
            int? value = 42;
            int? result = value.ZeroToNull();
            Assert.AreEqual(value, result);
        }
    }
}
