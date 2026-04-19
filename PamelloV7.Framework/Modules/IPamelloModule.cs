using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.Modules;

public interface IPamelloModule
{
    public string Name { get; }
    public string Author { get; }
    public string Description { get; }
    
    public ELoadingStage Stage { get; }
    
    public int Color => 0xFFFFFF;
    
    public void Configure(IServiceCollection services) { }

    public Task StartupAsync(IServiceProvider services) { return Task.CompletedTask; }
    public Task StartedAsync(IServiceProvider services) { return Task.CompletedTask; }
    public Task ShoutdownAsync() { return Task.CompletedTask; }
}
