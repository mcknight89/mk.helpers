using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using mk.helpers;

namespace mk.helpers.tests
{
    [TestClass]
    public class HttpClientExtensionsTests
    {
        [TestMethod]
        public async Task GetFileSizeAsync_ValidUrl_ReturnsFileSize()
        {
            using var client = new HttpClient();
            var url = "http://testfiles.ic.net.uk/100MB.zip";
            // Act
            var fileSize = await client.GetFileSizeAsync(url);

            // Assert
            Assert.AreEqual(104857600, fileSize);
        }


        [TestMethod]
        public async Task GetHeadersAsync()
        {
            using var client = new HttpClient();
            var url = "http://testfiles.ic.net.uk/100MB.zip";

            // Act
            var headers = await client.GetHeadersAsync(url);

            Assert.IsTrue(headers.Any());
        }

    }
}
