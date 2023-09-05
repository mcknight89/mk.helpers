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



        [TestMethod]
        public void IsValid_ValidIpv4Address_ReturnsTrue()
        {
            string ipAddress = "192.168.1.1";
            bool isValid = IpHelper.IsValid(ipAddress);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValid_ValidIpv6Address_ReturnsTrue()
        {
            string ipAddress = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            bool isValid = IpHelper.IsValid(ipAddress);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValid_InvalidIpAddress_ReturnsFalse()
        {
            string ipAddress = "invalid_ip_address";
            bool isValid = IpHelper.IsValid(ipAddress);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidIpv4_ValidIpv4Address_ReturnsTrue()
        {
            string ipAddress = "192.168.1.1";
            bool isValidIpv4 = IpHelper.IsValidIpv4(ipAddress);
            Assert.IsTrue(isValidIpv4);
        }

        [TestMethod]
        public void IsValidIpv4_ValidIpv6Address_ReturnsFalse()
        {
            string ipAddress = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            bool isValidIpv4 = IpHelper.IsValidIpv4(ipAddress);
            Assert.IsFalse(isValidIpv4);
        }

        [TestMethod]
        public void IsValidIpv6_ValidIpv6Address_ReturnsTrue()
        {
            string ipAddress = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            bool isValidIpv6 = IpHelper.IsValidIpv6(ipAddress);
            Assert.IsTrue(isValidIpv6);
        }

        [TestMethod]
        public void IsValidIpv6_ValidIpv4Address_ReturnsFalse()
        {
            string ipAddress = "192.168.1.1";
            bool isValidIpv6 = IpHelper.IsValidIpv6(ipAddress);
            Assert.IsFalse(isValidIpv6);
        }
    }
}
