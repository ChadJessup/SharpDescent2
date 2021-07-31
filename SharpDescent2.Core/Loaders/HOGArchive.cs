using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpDescent2.Core.Loaders
{
    public class HOGArchive
    {
        public string FilePath { get; init; }
        public int NumberOfFiles => this.FileHeaders.Count;
        public List<HOGFileHeader> FileHeaders { get; init; } = new();
        public Dictionary<HOGFileHeader, Memory<byte>> OpenedFiles { get; } = new();
        public Stream Stream { get; init; }

        public async Task WriteFile(HOGFileHeader hogFile, string directory)
        {
            Directory.CreateDirectory(directory);

            if (!this.OpenedFiles.TryGetValue(hogFile, out Memory<byte> buffer))
            {
                buffer = await this.ReadFile(hogFile);
            }

            var fp = Path.Combine(directory, hogFile.FileName);
            var fs = File.OpenWrite(fp);

            await fs.WriteAsync(buffer);
        }

        public async Task<Memory<byte>> ReadFile(HOGFileHeader hogFile)
        {
            if (this.OpenedFiles.TryGetValue(hogFile, out var foundBuffer))
            {
                return foundBuffer;
            }

            if (this.Stream is null)
            {
                throw new NullReferenceException("HOGFile needs to be opened via static LoadFile method.");
            }

            this.Stream.Seek(hogFile.Offset, SeekOrigin.Begin);
            Memory<byte> buffer = new byte[hogFile.Length];
            await this.Stream.ReadAsync(buffer);

            this.OpenedFiles.Add(hogFile, buffer);
            return buffer;
        }

        public static HOGArchive LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var file = File.OpenRead(path);

            // verify header of "DHF"
            Span<byte> hogHeader = stackalloc byte[3];
            file.Read(hogHeader);
            if (hogHeader[0] != 'D' && hogHeader[1] != 'H' && hogHeader[2] != 'F')
            {
                throw new InvalidOperationException("Hog file doesn't start with DHF: " + path);
            }

            Span<byte> fileNameBytes = stackalloc byte[13];
            Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
            bool processing = true;

            var headers = new List<HOGFileHeader>();

            while (processing)
            {
                int readCount = file.Read(fileNameBytes);

                if (readCount == 0)
                {
                    processing = false;
                    continue;
                }

                var fileName = Encoding.ASCII.GetString(fileNameBytes)
                    .Split("\0", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];

                file.Read(lengthBytes);

                var length = MemoryMarshal.Cast<byte, int>(lengthBytes);
                var offset = file.Position;

                file.Seek(length[0], SeekOrigin.Current);

                var header = new HOGFileHeader
                {
                    FileName = fileName,
                    Length = length[0],
                    Offset = offset,
                };

                headers.Add(header);
            }

            return new HOGArchive
            {
                FileHeaders = headers,
                FilePath = path,
                Stream = file,
            };
        }
    }
}
