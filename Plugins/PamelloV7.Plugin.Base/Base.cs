using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services;
using PamelloV7.Plugin.Base.Services;

namespace PamelloV7.Plugin.Base;

public class Base : IPamelloPlugin
{
    public string Name => "Base";
    public string Description => "Base functionality of PamelloV7";

    public void Startup(IServiceProvider services) {
        Console.WriteLine($"Base plugin startup");
    }
}
