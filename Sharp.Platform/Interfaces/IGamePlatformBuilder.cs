using Microsoft.Extensions.DependencyInjection;

namespace Sharp.Platform.Interfaces
{
    public interface IGamePlatformBuilder
    {
        IServiceCollection Services { get; }
        GameContext Build();
        IGamePlatformBuilder AddDependency<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}
