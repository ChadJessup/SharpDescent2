using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Sharp.Platform;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core;

CancellationTokenSource cts = new();

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile($"SharpDescent2.json", optional: true)
    .AddJsonFile($"SharpDescent2.{Environment.MachineName}.json", optional: true)
    .AddCommandLine(args);

// pass configuration to GamePlatformBuilder...
IGamePlatformBuilder platformBuilder = new GamePlatformBuilder(configurationBuilder);

// Adding components that as closely match Jagged Alliance 2's internals as possible.
// These components can have other components injected in when instantiated.
platformBuilder
    .AddGameLogic<SharpDescent2GameLogic>()
    .AddOtherComponents();

// Initialize the game platform as a whole, which returns a game context
// containing platform components for core game logic to use.
using var context = platformBuilder.Build();

// The rest is up to game-specific logic, pass the context into a loop and go.
var result = await context.StartGameLoop(cts.Token);

return result;



public static class StandardSharpDescent2Extensions
{
    public static IGamePlatformBuilder AddOtherComponents(this IGamePlatformBuilder builder)
    {
        return builder;
    }
}
