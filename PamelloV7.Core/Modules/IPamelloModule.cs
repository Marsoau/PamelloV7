using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Modules;

public interface IPamelloModule
{
    public string Name { get; }
    public string Author { get; }
    public string Description { get; }
    
    public ELoadingStage Stage { get; }
    
    public void Configure(IServiceCollection services) { }

    public Task StartupAsync(IServiceProvider services) { return Task.CompletedTask; }
    public Task ShoutdownAsync() { return Task.CompletedTask; }
}
