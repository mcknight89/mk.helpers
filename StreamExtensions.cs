using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with streams.
    /// </summary>
    public static class StreamExtensions
    {
        private static Dictionary<ImageFormat, byte[][]> _ImageHeaders = new Dictionary<ImageFormat, byte[][]>
        {
            // Image format headers and corresponding byte sequences
            {ImageFormat.BMP, new [] {Encoding.ASCII.GetBytes("BM") }},     // BMP
            {ImageFormat.GIF,new [] {Encoding.ASCII.GetBytes("GIF") }},     // GIF
            {ImageFormat.PNG, new [] {new byte[] { 137, 80, 78, 71 } }},    // PNG
            {ImageFormat.TIFF,new [] {new byte[] { 73, 73, 42 },new byte[] { 77, 77, 42 } }}, // TIFF
            {ImageFormat.JPEG,new [] { new byte[] { 255, 216, 255, } } },  // JPEG
            {ImageFormat.WEBP, new [] {new byte[]  {82, 73, 70, 70} }},    // WEBP
        };

        /// <summary>
        /// Reads exactly the specified number of bytes from the stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="pos">The position from which to start reading. Default is 0.</param>
        /// <returns>The byte array containing the read bytes.</returns>
        public static byte[] ReadExactly(this Stream stream, int count, int pos = 0)
        {
            stream.Position = pos;
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                    return buffer;
                offset += read;
            }
            return buffer;
        }

        /// <summary>
        /// Reads the image format from the header of the input stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>The detected image format or ImageFormat.None if no match is found.</returns>
        public static ImageFormat ReadImageFormatFromHeader(this Stream input)
        {
            

            input.Position = 0;
            var result = _ImageHeaders?.FirstOrDefault(x => x.Value.Any(e => e.SequenceEqual(input.ReadExactly(e.Length))));
            return result?.Key ?? ImageFormat.None;
        }
    }
}
