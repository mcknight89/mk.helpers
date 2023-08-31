using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class IpHelperTests
    {
        [TestMethod]
        public void IpAddressToNumber_ConvertsCorrectly()
        {
            string ipAddress = "192.168.0.1";
            long expectedNumber = 3232235521;
            long actualNumber = IpHelper.IpAddressToNumber(ipAddress);

            Assert.AreEqual(expectedNumber, actualNumber, "Conversion from IP address to number is incorrect.");
        }

        [TestMethod]
        public void NumberToIpAddress_ConvertsCorrectly()
        {
            long ipAddressNumber = 3232235521;
            string expectedIpAddress = "192.168.0.1";

            string actualIpAddress = IpHelper.NumberToIpAddress(ipAddressNumber);

            Assert.AreEqual(expectedIpAddress, actualIpAddress, "Conversion from number to IP address is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void IpAddressToNumber_InvalidIpAddress_ThrowsException()
        {
            string invalidIpAddress = "invalid";

            long number = IpHelper.IpAddressToNumber(invalidIpAddress);
        }
    }
}
