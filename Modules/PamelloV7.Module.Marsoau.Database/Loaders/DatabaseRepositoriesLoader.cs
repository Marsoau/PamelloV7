using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Messages;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;

namespace PamelloV7.Server.Loaders;

internal static class DatabaseRepositoriesLoader
{
    private static RefreshableLogMessage? _currentMessage;
    
    public static async Task Load(IServiceCollection collection, IServiceProvider services) {
        var repositories = new List<IPamelloDatabaseRepository>();

        foreach (var descriptor in collection) {
            if (!descriptor.ServiceType.IsAssignableTo(typeof(IPamelloDatabaseRepository))) continue;
            repositories.Add((IPamelloDatabaseRepository)services.GetRequiredService(descriptor.ServiceType));;
        }
        
        foreach (var repository in repositories) {
            repository.OnLoadingStart += () => RepositoryOnOnLoadingStart(repository, "Loading");
            repository.OnLoadingProgress += (int loaded, int from) => RepositoryOnOnLoadingProgress(repository, loaded, from, "Loading");
            repository.OnLoadingEnd += () => RepositoryOnOnLoadingEnd(repository);
            
            repository.OnInitStart += () => RepositoryOnOnLoadingStart(repository, "Initializing");
            repository.OnInitProgress += (int loaded, int from) => RepositoryOnOnLoadingProgress(repository, loaded, from, "Initializing");
            repository.OnInitEnd += () => RepositoryOnOnLoadingEnd(repository);
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

    private static void RepositoryOnOnLoadingStart(IPamelloDatabaseRepository repository, string message) {
        _currentMessage = Output.Write($"{message} {repository.CollectionName}");
        //Output.Write($"Loading {repository.CollectionName}                                 ");
    }
    private static void RepositoryOnOnLoadingProgress(IPamelloDatabaseRepository repository, int loaded, int from, string message) {
        _currentMessage?.ContentBuilder.Clear().Append($"{message} {repository.CollectionName} [{loaded}/{from}]");
        _currentMessage?.Refresh();
        //Output.Write($"Loading {repository.CollectionName} [{loaded}/{from}]");
    }
    private static void RepositoryOnOnLoadingEnd(IPamelloDatabaseRepository repository) {
        _currentMessage?.ContentBuilder.Append(" Done");
        _currentMessage?.Refresh();
    }
}
