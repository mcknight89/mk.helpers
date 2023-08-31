using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class Crc32HashTests
    {
        [TestMethod]
        public void Compute_SameBuffer_ReturnsSameHash()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            var hash1 = Crc32Hash.Compute(buffer);
            var hash2 = Crc32Hash.Compute(buffer);

            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void Compute_DifferentBuffers_ReturnsDifferentHashes()
        {
            var buffer1 = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var buffer2 = new byte[] { 0x10, 0x20, 0x30, 0x40 };

            var hash1 = Crc32Hash.Compute(buffer1);
            var hash2 = Crc32Hash.Compute(buffer2);

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void Compute_WithSeed_SameBuffer_ReturnsSameHash()
        {
            var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            uint seed = 0x12345678;

            var hash1 = Crc32Hash.Compute(seed, buffer);
            var hash2 = Crc32Hash.Compute(seed, buffer);

            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void Compute_WithSeed_DifferentBuffers_ReturnsDifferentHashes()
        {
            // Arrange
            var buffer1 = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var buffer2 = new byte[] { 0x10, 0x20, 0x30, 0x40 };
            uint seed = 0x12345678;

            var hash1 = Crc32Hash.Compute(seed, buffer1);
            var hash2 = Crc32Hash.Compute(seed, buffer2);
            Assert.AreNotEqual(hash1, hash2);
        }

    }
}
