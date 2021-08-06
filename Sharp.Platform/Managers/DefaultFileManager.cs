using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.Managers
{
    public class DefaultFileManager : IFileManager
    {
        private readonly Dictionary<string, Stream> files = new Dictionary<string, Stream>();
        private readonly ILogger<DefaultFileManager> logger;
        private readonly IConfiguration config;
        private readonly string baseDirectory;

        public DefaultFileManager(
            ILogger<DefaultFileManager> logger,
            IConfiguration configuration)
        {
            this.logger = logger;

            // TODO: strongly type this.
            this.config = configuration;
            this.baseDirectory = this.config["BaseDirectory"];
        }

        public bool IsInitialized { get; }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void FileClose(Stream stream)
        {
        }

        public bool FileExists(string pFilename)
        {
            throw new NotImplementedException();
        }

        public Stream FileOpen(string pFileName, FileAccess read, bool fDeleteOnClose = false)
        {
            throw new NotImplementedException();
        }

        public bool FileRead(Stream stream, Span<byte> buffer, out uint bytesRead)
        {
            throw new NotImplementedException();
        }

        public bool FileRead(Stream stream, ref byte[] buffer, uint count, out uint uiBytesRead)
        {
            throw new NotImplementedException();
        }

        public bool FileRead<T>(Stream stream, ref T[] fillArray, uint uiFileSectionSize, out uint uiBytesRead) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public bool FileSeek(Stream stream, ref uint uiStoredSize, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
