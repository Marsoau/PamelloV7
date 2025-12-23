using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
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
    }

    private static void RepositoryOnOnLoadingStart(IPamelloDatabaseRepository repository) {
        Console.Write($"\rLoading {repository.CollectionName}                                 ");
    }
    private static void RepositoryOnOnLoadingProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        Console.Write($"\rLoading {repository.CollectionName} [{loaded}/{from}]               ");
    }
    private static void RepositoryOnOnLoadingEnd(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"\nDone");
    }

    private static void RepositoryOnOnInitStart(IPamelloDatabaseRepository repository) {
        Console.Write($"\rInitializing {repository.CollectionName}                            ");
    }
    private static void RepositoryOnOnInitProgress(IPamelloDatabaseRepository repository, int loaded, int from) {
        Console.Write($"\rInitializing {repository.CollectionName} [{loaded}/{from}]          ");
    }
    private static void RepositoryOnOnInitEnd(IPamelloDatabaseRepository repository) {
        Console.WriteLine($"\nDone");
    }
}
