#nullable enable
using System;
using System.IO;

namespace BinarySerializer.PlayStation.PS2.MemoryCard
{
    /// <summary>
    /// A stream wrapper for a PS2 memory card. This allows automatically
    /// handling the error correction codes stored after each page.
    /// </summary>
    public class MemoryCardStream : Stream
    {
        public MemoryCardStream(Stream baseStream)
        {
            BaseStream = baseStream;

            using (Reader reader = new(baseStream, leaveOpen: true))
            {
                // Get the page size
                baseStream.Position = 0x28;
                PageDataSize = reader.ReadInt16();

                // Check if ECC is used
                baseStream.Position = 0x151;
                MemoryCardFlags flags = (MemoryCardFlags)reader.ReadByte();
                HasECC = (flags & MemoryCardFlags.ECC) != 0;
            }

            if (HasECC)
            {
                // Calculate the spare size
                SpareSize = PageDataSize / 128 * 4;

                // Set the total page size
                TotalPageSize = PageDataSize + SpareSize;
            }
            else
            {
                SpareSize = 0;
                TotalPageSize = PageDataSize;
            }
        }

        public Stream BaseStream { get; }

        public int PageDataSize { get; }
        public bool HasECC { get; }
        public int SpareSize { get; }
        public int TotalPageSize { get; }

        public override bool CanRead => BaseStream.CanRead;
        public override bool CanSeek => BaseStream.CanSeek;
        public override bool CanWrite => BaseStream.CanWrite;
        public override long Length => BaseStream.Length;

        public override long Position
        {
            get
            {
                if (HasECC)
                {
                    long pageIndex = BaseStream.Position / TotalPageSize;
                    return BaseStream.Position - pageIndex * SpareSize;
                }
                else
                {
                    return BaseStream.Position;
                }
            }
            set
            {
                if (HasECC)
                {
                    long pageIndex = value / PageDataSize;
                    BaseStream.Position = value + pageIndex * SpareSize;
                }
                else
                {
                    BaseStream.Position = value;
                }
            }
        }

        // TODO: Implement ReadByte and WriteByte

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (HasECC)
            {
                int totalReadBytes = 0;

                while (count > 0)
                {
                    int remainingPageLength = (int)(PageDataSize - (BaseStream.Position % TotalPageSize));

                    if (remainingPageLength == 0)
                    {
                        BaseStream.Position += SpareSize;
                        remainingPageLength = PageDataSize;
                    }

                    int readLength = Math.Min(count, remainingPageLength);

                    readLength = BaseStream.Read(buffer, offset, readLength);
                    totalReadBytes += readLength;
                    offset += readLength;
                    count -= readLength;

                    if (readLength == 0)
                        return totalReadBytes;
                }

                return totalReadBytes;
            }
            else
            {
                return BaseStream.Read(buffer, offset, count);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (HasECC)
            {
                throw new NotImplementedException();
            }
            else
            {
                BaseStream.Write(buffer, offset, count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => BaseStream.SetLength(value);
        public override void Flush() => BaseStream.Flush();
    }
}