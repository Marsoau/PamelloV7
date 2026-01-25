using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Modules;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Services;

namespace PamelloV7.Module.Marsoau.Base;

public class Base : IPamelloModule
{
    public string Name => "Base";
    public string Author => "Marsoau";
    public string Description => "Base functionality of PamelloV7";
    public ELoadingStage Stage => ELoadingStage.Early;

    public void Configure(IServiceCollection services) {
    }
    public async Task StartupAsync(IServiceProvider services) {
        var platforms = services.GetRequiredService<IPlatformService>() as PlatformService;
        platforms?.Load();

        var commands = services.GetRequiredService<IPamelloCommandsService>() as PamelloCommandsService;
        commands?.Load();
        
        var downloaders = services.GetRequiredService<IDownloadService>() as DownloadService;
        downloaders?.Load();

        /*
        var songs = services.GetRequiredService<IPamelloSongRepository>();
        var users = services.GetRequiredService<IPamelloUserRepository>();

        var user = users.Get(1)!;
        var song = songs.Get(1)!;

        Console.WriteLine($"User: {user}");
        Console.WriteLine($"Song: {song} ({song.AddedBy})");

        //song.AddEpisode("0:00", "cat", false);
        //song.AddEpisode("0:30", "jam", false);
        //song.AddEpisode("1:21", "guess", false);
        //song.AddEpisode("1:04", "color", false);
        //song.AddEpisode("0:54", "may", false);

        foreach (var association in song.Associations) {
            Console.WriteLine($"Association: {association}");
        }
        foreach (var episode in song.Episodes) {
            Console.WriteLine($"Episode: {episode}");
        }
        */
    }
}
