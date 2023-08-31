using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;

namespace mk.helpers.tests
{
    [TestClass]
    public class EnumHelperTests
    {
        [TestMethod]
        public void Parse_ValidEnumValue_ReturnsParsedValue()
        {
            string validValue = "OptionB";
            TestEnum result = EnumHelper.Parse<TestEnum>(validValue);
            Assert.AreEqual(TestEnum.OptionB, result);
        }

        [TestMethod]
        public void Parse_InvalidEnumValue_ReturnsDefaultValue()
        {
            string invalidValue = "InvalidValue";
            TestEnum result = EnumHelper.Parse<TestEnum>(invalidValue);
            Assert.AreEqual(default(TestEnum), result);
        }

        [TestMethod]
        public void TryParse_ValidEnumValue_ReturnsTrueAndParsedValue()
        {
            string validValue = "OptionC";
            TestEnum result;
            bool success = EnumHelper.TryParse(validValue, out result);
            Assert.IsTrue(success);
            Assert.AreEqual(TestEnum.OptionC, result);
        }

        [TestMethod]
        public void TryParse_InvalidEnumValue_ReturnsFalseAndDefault()
        {
            string invalidValue = "InvalidValue";
            TestEnum result;
            bool success = EnumHelper.TryParse(invalidValue, out result);
            Assert.IsFalse(success);
            Assert.AreEqual(default(TestEnum), result);
        }
    }

    // Example enum for testing
    public enum TestEnum
    {
        OptionA,
        OptionB,
        OptionC
    }
}
