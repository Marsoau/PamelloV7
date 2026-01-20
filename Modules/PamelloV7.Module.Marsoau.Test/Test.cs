using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.Modules;
using PamelloV7.Core.Platforms;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Late;
    
    public void Startup(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var songs = services.GetRequiredService<IPamelloSongRepository>();
        var playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        var logger = services.GetRequiredService<IPamelloLogger>();

        var me = users.GetRequired(1);
        var list = playlists.GetRequired(1);
        
        var platforms = services.GetRequiredService<IPlatformService>();
        var discord = platforms.GetUserPlatform("discord");
        
        var commands = services.GetRequiredService<IPamelloCommandsService>();
        
        var events = services.GetRequiredService<IEventsService>();

        //var user = users.GetByPlatformKey(me, new PlatformKey("discord", "1422257871655145602"), true);
        //Console.WriteLine($"User: {user}");

        var query = "songs$4,5,6";

        events.Subscribe<SongNameUpdated>(async (e) => {
            Console.WriteLine($"Updated name of the song: {e.Song}");
        });
        
        logger.Log("G");
        var entities = peql.Get(query, me);
        logger.Log("G");

        Console.WriteLine($"Results of \"{query}\" query:");
        foreach (var entity in entities) {
            Console.WriteLine($"| {entity.GetType().Name} : {entity}");
            
            if (entity is not IPamelloSong song) continue;

            Console.WriteLine($"Before: {song.Name}");
            commands.Get<SongRename>(me).Execute(song, "test");
            
            Console.WriteLine($"Episodes: ({song.Episodes.Count} episodes)");
            foreach (var episode in song.Episodes) {
                Console.WriteLine($"| {episode}");
            }
        }


    }
}
