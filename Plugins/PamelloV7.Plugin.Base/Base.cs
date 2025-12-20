using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Plugin.Base.Services;

namespace PamelloV7.Plugin.Base;

public class Base : IPamelloPlugin
{
    public string Name => "Base";
    public string Description => "Base functionality of PamelloV7";

    public void PreStartup(IServiceProvider services) {
        
    }

    public void Startup(IServiceProvider services) {
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var songs = services.GetRequiredService<IPamelloSongRepository>();

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
    }
}
