using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;
using SharpDescent2.Core.Loaders;

namespace SharpDescent2.Core.Systems;

public class SpecialEffectsSystem : IGamePlatformManager
{
    private readonly ILogger<SpecialEffectsSystem> logger;
    private readonly ILibraryManager library;

    public SpecialEffectsSystem(
        ILogger<SpecialEffectsSystem> logger,
        ILibraryManager libraryManager)
    {
        this.logger = logger;
        this.library = libraryManager;
    }

    public bool IsInitialized { get; }

    public eclip[] Effects { get; private set; } = new eclip[MAX.EFFECTS];

    public int Num_effects { get; private set; }

    public ValueTask<bool> Initialize()
    {
        var ham = (HAMArchive)this.library.GetLibrary("descent2.ham");
        this.Effects = ham.Effects;
        this.Num_effects = ham.Effects.Length;

        for (int i = 0; i < this.Num_effects; i++)
        {
            this.Effects[i].time_left = this.Effects[i].vc.frame_time;
        }

        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
