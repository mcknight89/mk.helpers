using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{

    [TestClass]
    public class RandomHelperTests
    {
        [TestMethod]
        public void RandomString_GeneratesRandomString()
        {
            string randomString = RandomHelper.RandomString(10, false);
            Assert.IsNotNull(randomString);
            Assert.AreEqual(10, randomString.Length);
        }

        [TestMethod]
        public void RandomInt_GeneratesRandomIntWithinRange()
        {
            int randomInt = RandomHelper.RandomInt(1, 100);
            Assert.IsTrue(randomInt >= 1 && randomInt <= 100);
        }

        [TestMethod]
        public void RandomDouble_GeneratesRandomDouble()
        {
            double randomDouble = RandomHelper.RandomDouble();
            Assert.IsTrue(randomDouble >= 0.0 && randomDouble < 1.0);
        }

        [TestMethod]
        public void RandomNumber_GeneratesRandomNumberWithinRangeAndDigits()
        {
            double randomNumber = RandomHelper.RandomNumber(10, 20, 2);
            Assert.IsTrue(randomNumber >= 10.0 && randomNumber <= 20.0);    
        }

        [TestMethod]
        public void RandomBool_GeneratesRandomBool()
        {
            bool randomBool = RandomHelper.RandomBool();
            Assert.IsTrue(randomBool || !randomBool);
        }

        [TestMethod]
        public void RandomDate_GeneratesRandomDateWithinRange()
        {
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime maxDate = new DateTime(2023, 1, 1);

            DateTime randomDate = RandomHelper.RandomDate(minDate, maxDate);
            Assert.IsTrue(randomDate >= minDate && randomDate < maxDate);
        }
    }
}
