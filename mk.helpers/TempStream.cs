using System;
using System.IO;

namespace mk.helpers
{
    /// <summary>
    /// A temporary file stream that tries to use a file stream in the temp folder, but falls back to a memory stream if that fails. Will delete the file on dispose unless specified otherwise.
    /// </summary>
    public class TempStream : Stream
    {
        /// <summary>
        /// Gets the inner stream.
        /// </summary>
        public readonly Stream InnerStream;
        private readonly bool keepFileOnDispose;
        /// <summary>
        // Creates a new instance of the <see cref="TempStream"/> class.
        /// </summary>
        /// <param name="keepFileOnDispose">keep the temporary when its disposed</param>
        public TempStream(bool keepFileOnDispose = false)
        {
            this.keepFileOnDispose = keepFileOnDispose;

            try
            {
                // Create a temporary file stream in the temp folder
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                InnerStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // If creating the file stream fails, fall back to using a memory stream
                InnerStream = new MemoryStream();
            }
        }
        /// <summary>
        /// Gets a value indicating whether the stream is a file stream.
        /// </summary>
        public bool IsFileStream => InnerStream is FileStream;
        /// <summary>
        /// Gets a value indicating whether the stream is a memory stream.
        /// </summary>
        public bool IsMemoryStream => InnerStream is MemoryStream;

        /// <inheritdoc/>
        public override bool CanRead => InnerStream.CanRead;
        /// <inheritdoc/>
        public override bool CanSeek => InnerStream.CanSeek;
        /// <inheritdoc/>
        public override bool CanWrite => InnerStream.CanWrite;
        /// <inheritdoc/>
        public override long Length => InnerStream.Length;
        /// <inheritdoc/>
        public override long Position
        {
            get => InnerStream.Position;
            set => InnerStream.Position = value;
        }
        /// <inheritdoc/>
        public override void Flush() => InnerStream.Flush();
        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => InnerStream.Read(buffer, offset, count);
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => InnerStream.Seek(offset, origin);
        /// <inheritdoc/>
        public override void SetLength(long value) => InnerStream.SetLength(value);
        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => InnerStream.Write(buffer, offset, count);
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InnerStream.Dispose();

                if (!keepFileOnDispose && InnerStream is FileStream fileStream)
                {
                    TryDeleteFile(fileStream.Name);
                }
            }

            base.Dispose(disposing);
        }

        private void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                // Handle any exceptions if needed
            }
        }
    }

}
