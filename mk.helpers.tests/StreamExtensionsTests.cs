using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers.Types;
using System.IO;


namespace mk.helpers.tests
{

    [TestClass]
    public class StreamExtensionsTests
    {
        [TestMethod]
        public void ReadExactly_ShouldReadCorrectNumberOfBytes()
        {
            // Arrange
            byte[] testData = { 1, 2, 3, 4, 5 };
            MemoryStream stream = new MemoryStream(testData);

            // Act
            byte[] result = stream.ReadExactly(3);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, result);
        }

        [TestMethod]
        public void ReadImageFormatFromHeader_ShouldReturnCorrectImageFormat()
        {
            // Arrange
            byte[] jpegHeader = { 255, 216, 255 };
            MemoryStream jpegStream = new MemoryStream(jpegHeader);

            // Act
            ImageFormat format = jpegStream.ReadImageFormatFromHeader();

            // Assert
            Assert.AreEqual(ImageFormat.JPEG, format);
        }

        [TestMethod]
        public void ReadImageFormatFromHeader_ShouldReturnNoneForUnknownFormat()
        {
            // Arrange
            byte[] unknownHeader = { 0, 0, 0 };
            MemoryStream unknownStream = new MemoryStream(unknownHeader);

            // Act
            ImageFormat format = unknownStream.ReadImageFormatFromHeader();

            // Assert
            Assert.AreEqual(ImageFormat.None, format);
        }
    }


}