using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;
using SharpDescent2.Core.Systems;

namespace SharpDescent2.Core;

public class SharpDescent2GameLogic : IGameLogic
{
    private readonly ILogger<SharpDescent2GameLogic> logger;
    private readonly IConfiguration configuration;
    private readonly ILibraryManager library;
    private readonly ObjectSystem objects;
    private readonly TextSystem text;
    private readonly AISystem ai;
    private readonly SpecialEffectsSystem specialEffects;
    private readonly GaugeSystem gauges;
    private readonly FireballSystem fireball;

    private grs_bitmap background_bitmap;

    public bool IsInitialized { get; private set; }

    // TODO: not sure what this is yet, move to Globals?
    public bool john_head_on { get; set; }

    public SharpDescent2GameLogic(
        ILogger<SharpDescent2GameLogic> logger,
        IConfiguration configuration,
        ILibraryManager libraryManager,
        TextSystem textSystem,
        ObjectSystem objectSystem,
        SpecialEffectsSystem specialEffectsSystem,
        AISystem aISystem,
        GaugeSystem gaugeSystem,
        FireballSystem fireballSystem)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.library = libraryManager;
        this.objects = objectSystem;
        this.text = textSystem;
        this.ai = aISystem;
        this.specialEffects = specialEffectsSystem;
        this.gauges = gaugeSystem;
        this.fireball = fireballSystem;
    }

    public async Task<int> GameLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100.0), CancellationToken.None);
        }

        return 0;
    }

    public async ValueTask<bool> Initialize()
    {
        await this.library.Initialize();
        this.IsInitialized = await init_game();

        return this.IsInitialized;
    }

    private async Task<bool> init_game()
    {
        var result = await this.objects.Initialize();
        result |= await this.specialEffects.Initialize();
        result |= await this.ai.Initialize();
        result |= await this.gauges.Initialize();
        result |= await this.fireball.Initialize();
        result |= await this.load_background_bitmap();


        return result;
    }

    private const string BACKGROUND_NAME = "statback.pcx";

    private async Task<bool> load_background_bitmap()
    {
        var pal = new byte[256 * 3];
        var hog = (HOGArchive)this.library.GetLibrary("descent2.hog");

        var fileName = john_head_on ? "johnhead.pcx" : BACKGROUND_NAME;

        var header = hog.FileHeaders.FirstOrDefault(fh => fh.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        var contents = await hog.ReadFile(header);

        var pcx_error = PCX.pcx_read_bitmap(contents.Span, ref background_bitmap, BM.LINEAR, pal);
        if (pcx_error != PCX_ERROR.NONE)
        {
            // Error("File %s - PCX error: %s", BACKGROUND_NAME, pcx_errormsg(pcx_error));
        }

        //gr_remap_bitmap_good(&background_bitmap, pal, -1, -1);

        return true;
    }

    public void Dispose()
    {
    }
}
