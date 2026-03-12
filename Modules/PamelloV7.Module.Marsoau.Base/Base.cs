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

    public async Task StartupAsync(IServiceProvider services) {
        var dependencies = services.GetRequiredService<IDependenciesService>();
        
        var ffmpeg = dependencies.ResolveRequired("ffmpeg");
        var ytDlp = dependencies.ResolveRequired("yt-dlp");

        var d1 = ffmpeg.DownloadOrUpdateAsync();
        var d2 = ytDlp.DownloadOrUpdateAsync();
        
        await Task.WhenAll(d1, d2);

        Console.WriteLine($"FFMpeg: {ffmpeg.IsInstalled}");
        Console.WriteLine($"YtDlp: {ytDlp.IsInstalled}");
    }
}
