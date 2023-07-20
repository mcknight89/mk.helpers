using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mk.helpers
{
    public static class StreamExtensions
    {
        private static Dictionary<ImageFormat, byte[][]> _ImageHeaders = new Dictionary<ImageFormat, byte[][]>
            {
                 {ImageFormat.BMP, new [] {Encoding.ASCII.GetBytes("BM") }},     // BMP
                 {ImageFormat.GIF,new [] {Encoding.ASCII.GetBytes("GIF") }},     // GIF
                 {ImageFormat.PNG, new [] {new byte[] { 137, 80, 78, 71 } }},    // PNG
                 {ImageFormat.TIFF,new [] {new byte[] { 73, 73, 42 },new byte[] { 77, 77, 42 } }}, // TIFF
                 {ImageFormat.JPEG,new [] { new byte[] { 255, 216, 255, } } },  // JPEG
                 {ImageFormat.WEBP, new [] {new byte[]  {82, 73, 70, 70} }},    
            };

        public static byte[] ReadExactly(this Stream stream, int count, int pos = 0)
        {
            stream.Position = pos;
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                    throw new EndOfStreamException();
                offset += read;
            }
            System.Diagnostics.Debug.Assert(offset == count);
            return buffer;
        }

        public static ImageFormat ReadImageFormatFromHeader(this Stream input)
        {
            input.Position = 0;
            var result = _ImageHeaders?.FirstOrDefault(x => x.Value.Any(e => e.SequenceEqual(input.ReadExactly(e.Length))));
            return result?.Key ?? ImageFormat.None;
        }
    }
}

