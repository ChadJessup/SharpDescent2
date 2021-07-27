using System;
using Microsoft.Extensions.Configuration;

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile($"SharpDescent2.json", optional: true)
    .AddJsonFile($"SharpDescent2.{Environment.MachineName}.json", optional: true)
    .AddCommandLine(args);

// pass configuration to GamePlatformBuilder...
IGamePlatformBuilder platformBuilder = new GamePlatformBuilder(configurationBuilder);
