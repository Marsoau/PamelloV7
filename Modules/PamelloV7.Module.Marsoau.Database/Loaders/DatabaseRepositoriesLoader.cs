using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;

namespace PamelloV7.Server.Loaders;

internal static class DatabaseRepositoriesLoader
{
    public static async Task Load(IServiceCollection collection, IServiceProvider services) {
        var repositories = new List<IPamelloDatabaseRepository>();

        foreach (var descriptor in collection) {
            if (!descriptor.ServiceType.IsAssignableTo(typeof(IPamelloDatabaseRepository))) continue;
            repositories.Add((IPamelloDatabaseRepository)services.GetRequiredService(descriptor.ServiceType));;
        }
        
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

        foreach (var repository in repositories) {
            (repository as IPamelloService)!.Startup(services);
        }
    }

    private static void RepositoryOnOnLoadingStart(IPamelloDatabaseRepository repository) {
        StaticLogger.Log($"Loading {repository.CollectionName}                                 ");
    }
    private static void RepositoryOnOnLoadingProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        StaticLogger.Log($"Loading {repository.CollectionName} [{loaded}/{from}]");
    }
    private static void RepositoryOnOnLoadingEnd(IPamelloDatabaseRepository repository) {
        StaticLogger.Log($"Done");
    }

    private static void RepositoryOnOnInitStart(IPamelloDatabaseRepository repository) {
        StaticLogger.Log($"Initializing {repository.CollectionName}                            ");
    }
    private static void RepositoryOnOnInitProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        StaticLogger.Log($"Initializing {repository.CollectionName} [{loaded}/{from}]");
    }
    private static void RepositoryOnOnInitEnd(IPamelloDatabaseRepository repository) {
        StaticLogger.Log($"Done");
    }
}
