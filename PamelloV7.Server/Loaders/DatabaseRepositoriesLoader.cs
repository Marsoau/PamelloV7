using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Server.Loaders;

public static class DatabaseRepositoriesLoader
{
    public static void Configure(IServiceCollection services) {
        services.AddSingleton<IPamelloDatabaseRepository>(s => s.GetRequiredService<IPamelloUserRepository>());
        services.AddSingleton<IPamelloDatabaseRepository>(s => s.GetRequiredService<IPamelloSongRepository>());
        services.AddSingleton<IPamelloDatabaseRepository>(s => s.GetRequiredService<IPamelloEpisodeRepository>());
        services.AddSingleton<IPamelloDatabaseRepository>(s => s.GetRequiredService<IPamelloPlaylistRepository>());
    }
    
    public static async Task Load(IServiceProvider services) {
        var repositories = services.GetServices<IPamelloDatabaseRepository>().ToArray();
        
        foreach (var repository in repositories) {
            repository.OnLoadingStart += () => RepositoryOnOnLoadingStart(repository);
            repository.OnLoadingProgress += (int loaded, int from) => RepositoryOnOnLoadingProgress(repository, loaded, from);
            repository.OnLoadingEnd += () => RepositoryOnOnLoadingEnd(repository);
            
            repository.OnInitStart += () => RepositoryOnOnInitStart(repository);
            repository.OnInitProgress += (int loaded, int from) => RepositoryOnOnInitProgress(repository, loaded, from);
            repository.OnInitEnd += () => RepositoryOnOnInitEnd(repository);
        }
        
        foreach (var repository in repositories) {
            await repository.LoadAllAsync();
        }
        foreach (var repository in repositories) {
            await repository.InitAllAsync();
        }
    }

    private static void RepositoryOnOnLoadingStart(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"Loading {repository.CollectionName}");
    }
    private static void RepositoryOnOnLoadingProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        Console.WriteLine($"Loading {repository.CollectionName} [{loaded}/{from}]");
    }
    private static void RepositoryOnOnLoadingEnd(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"Loaded {repository.CollectionName}");
    }

    private static void RepositoryOnOnInitStart(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"Initializing {repository.CollectionName}");
    }
    private static void RepositoryOnOnInitProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        Console.WriteLine($"Initializing {repository.CollectionName} [{loaded}/{from}]");
    }
    private static void RepositoryOnOnInitEnd(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"Initialized {repository.CollectionName}");
    }
}
