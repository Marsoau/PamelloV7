using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Core.Extensions;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Default;
    
    public void Startup(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var songs = services.GetRequiredService<IPamelloSongRepository>();
        var playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        var logger = services.GetRequiredService<IPamelloLogger>();

        var me = users.GetRequired(1);
        var list = playlists.GetRequired(1);

        var query = "songs$https://www.youtube.com/watch?v=NKGE2bSSQmI";
        
        logger.Log("G");
        var entities = peql.Get(query, me);
        logger.Log("G");

        Console.WriteLine($"Results of \"{query}\" query:");
        foreach (var entity in entities) {
            Console.WriteLine($"| {entity.GetType().Name} : {entity}");
        }
        
        var song = entities.FirstOrDefault() as IPamelloSong;
        if (song is null) {
            Console.WriteLine("No song");
            return;
        }

        Console.WriteLine($"Episodes: ({song.Episodes.Count} episodes)");
        foreach (var episode in song.Episodes) {
            Console.WriteLine($"| {episode}");
        }
    }
}
