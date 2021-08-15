using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;

namespace SharpDescent2.Core.Managers;

public class LibraryManager : ILibraryManager
{
    private readonly ILogger<LibraryManager> logger;
    private readonly IFileManager files;
    private readonly IConfiguration config;
    private HAMArchive mainHam;
    private HOGArchive mainHog;

    public LibraryManager(
        ILogger<LibraryManager> logger,
        IFileManager fileManager,
        IConfiguration configuration)
    {
        this.logger = logger;
        this.files = fileManager;
        this.config = configuration;
    }

    public bool IsInitialized { get; }

    public ValueTask<bool> Initialize()
    {
        var dataDir = this.config["DataDirectory"];
        var hogPath = Path.Combine(dataDir, "descent2.hog");
        var hamPath = Path.Combine(dataDir, "descent2.ham");

        this.mainHog = HOGArchive.LoadFile(hogPath);
        this.mainHam = HAMArchive.LoadFile(hamPath);

        return ValueTask.FromResult(true);
    }

    public object GetLibrary(string libraryName)
        => libraryName switch
        {
            "descent2.ham" => this.mainHam,
            "descent2.hog" => this.mainHog,
        };

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
        var ext = Path.GetExtension(libraryName);

        switch (ext)
        {
            case ".ham":
                break;
            case ".hog":
                break;
        }

        return true;
    }

    public void Dispose()
    {
    }
}
