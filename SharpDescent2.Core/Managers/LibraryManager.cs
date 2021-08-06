using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Managers
{
    public class LibraryManager : ILibraryManager
    {
        private readonly ILogger<LibraryManager> logger;
        private readonly IFileManager files;

        public LibraryManager(ILogger<LibraryManager> logger, IFileManager fileManager)
        {
            this.logger = logger;
            this.files = fileManager;
        }

        public bool IsInitialized { get; }

        public ValueTask<bool> Initialize()
        {

            return ValueTask.FromResult(true);
        }

        public bool CheckIfFileExistsInLibrary(string fileName)
        {
            return false;
        }

        public bool IsLibraryOpened(string libraryName)
        {
            return false;
        }

        public Stream OpenFileFromLibrary(string fileName)
        {
            return Stream.Null;
        }

        public bool OpenLibrary(string libraryName)
        {
            return false;
        }

        public void Dispose()
        {
        }
    }
}
