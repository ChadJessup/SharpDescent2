using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullFileManager : IFileManager
    {
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public void FileClose(Stream fptr)
        {
        }

        public bool FileExists(string pFilename)
        {
            return true;
        }

        public Stream FileOpen(string pFileName, FileAccess read, bool fDeleteOnClose = false)
        {
            return new NullStream();
        }

        public bool FileRead(Stream stream, Span<byte> buffer, out uint bytesRead)
        {
            bytesRead = 0;
            return true;
        }

        public bool FileRead(Stream stream, ref byte[] buffer, uint count, out uint uiBytesRead)
        {
            uiBytesRead = 0;
            return true;
        }

        public bool FileRead<T>(Stream stream, ref T[] fillArray, uint uiFileSectionSize, out uint uiBytesRead) where T : unmanaged
        {
            uiBytesRead = 0;
            return true;
        }

        public bool FileSeek(Stream stream, ref uint uiStoredSize, SeekOrigin current)
        {
            return true;
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        private class NullStream : Stream
        {
            public override bool CanRead { get; }
            public override bool CanSeek { get; }
            public override bool CanWrite { get; }
            public override long Length { get; }
            public override long Position { get; set; }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return 0;
            }

            public override void SetLength(long value)
            {
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
            }
        }
    }
}
