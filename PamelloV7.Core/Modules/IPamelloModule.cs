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
    public void Startup(IServiceProvider services) { }
    public void Shoutdown() { }
}
