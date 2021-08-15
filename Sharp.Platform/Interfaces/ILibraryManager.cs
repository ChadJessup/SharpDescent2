namespace Sharp.Platform.Interfaces
{
    public interface ILibraryManager : IGamePlatformManager
    {
        object GetLibrary(string libraryName);
        bool OpenLibrary(string libraryName);
        bool IsLibraryOpened(string libraryName);
        bool CheckIfFileExistsInLibrary(string fileName);
        Stream OpenFileFromLibrary(string fileName);
    }
}
