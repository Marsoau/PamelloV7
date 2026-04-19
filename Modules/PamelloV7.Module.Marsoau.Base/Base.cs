using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Data;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.History.Services;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.Base.History;
using PamelloV7.Module.Marsoau.Base.Services;

namespace PamelloV7.Module.Marsoau.Base;

public class Base : IPamelloModule
{
    public string Name => "Base";
    public string Author => "Marsoau";
    public string Description => "Base functionality of PamelloV7";
    public ELoadingStage Stage => ELoadingStage.Early;
    public int Color => 0x8479D9;

    public async Task StartupAsync(IServiceProvider services) {
        var dependencies = services.GetRequiredService<IDependenciesService>().GetAll();

        foreach (var dependency in dependencies) {
            Output.Write($"--- {dependency} ---");
            Output.Write($"Is installed: {dependency.IsInstalled}");
            
            if (dependency.IsInstalled) continue;
            
            Output.Write("Downloading...");
            await dependency.DownloadOrUpdateAsync();
            Output.Write($"Done, Now installed: {dependency.IsInstalled}");
        }
    }
}
