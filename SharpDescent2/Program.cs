using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharp.Platform;
using Sharp.Platform.Interfaces;
using Sharp.Platform.Managers;
using Sharp.Platform.Video;
using Sharp.Platform.Windows;
using SharpDescent2.Core;
using SharpDescent2.Core.Managers;
using SharpDescent2.Core.Systems;

CancellationTokenSource cts = new();

Console.CancelKeyPress += (o, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile($"SharpDescent2.json", optional: true)
    .AddJsonFile($"SharpDescent2.{Environment.MachineName}.json", optional: true)
    .AddCommandLine(args);

// pass configuration to GamePlatformBuilder...
IGamePlatformBuilder platformBuilder = new GamePlatformBuilder(configurationBuilder)
    .AddDependency<ITimerManager, TimerManager>()
    .AddDependency<IOSManager, WindowsOSManager>()
    .AddDependency<IVideoManager, VeldridVideoManager>()
    .AddDependency<IFileManager, DefaultFileManager>()
    .AddDependency<ILibraryManager, LibraryManager>()
    .AddGameLogic<SharpDescent2GameLogic>()
    .AddGameSystems();

// Initialize the game platform as a whole, which returns a game context
// containing platform components for core game logic to use.
using var context = platformBuilder.Build();

// The rest is up to game-specific logic, pass the context into a loop and go.
var result = await context.StartGameLoop(cts.Token);

return result;



public static class StandardSharpDescent2Extensions
{
    public static IGamePlatformBuilder AddGameSystems(this IGamePlatformBuilder builder)
    {
        builder.Services
            .AddSingleton<TextSystem>()
            .AddSingleton<ObjectSystem>()
            .AddSingleton<PolyObjSystem>()
            .AddSingleton<CollisionSystem>()
            .AddSingleton<SegmentSystem>()
            .AddSingleton<Player>()
            ;

        return builder;
    }
}
