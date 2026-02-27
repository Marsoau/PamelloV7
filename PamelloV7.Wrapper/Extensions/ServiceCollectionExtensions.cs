using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Wrapper.Query;
using PamelloV7.Wrapper.Repositories;
using PamelloV7.Wrapper.Requests;

namespace PamelloV7.Wrapper.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPamello(this IServiceCollection services) {
        services.AddSingleton<PamelloClient>();
        
        return services;
    }
}
