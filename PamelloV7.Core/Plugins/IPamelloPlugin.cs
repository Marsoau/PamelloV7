using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Core.Plugins;

public interface IPamelloPlugin
{
    public string Name { get; }
    public string Description { get; }
    
    public void Configure(IServiceCollection services) { }
    public void PreStartup(IServiceProvider services) { }
    public void Startup(IServiceProvider services) { }
    public void Shoutdown() { }
}
