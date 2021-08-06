using System.IO;

namespace Sharp.Platform.Interfaces
{
    public interface ILibraryManager : IGamePlatformManager
    {
        bool OpenLibrary(string libraryName);
        bool IsLibraryOpened(string libraryName);
        bool CheckIfFileExistsInLibrary(string fileName);
        Stream OpenFileFromLibrary(string fileName);
    }
}
