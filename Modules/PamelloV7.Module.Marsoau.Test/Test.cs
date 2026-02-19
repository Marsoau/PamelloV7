using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.History.Services;
using PamelloV7.Core.Modules;
using PamelloV7.Core.Platforms;
using PamelloV7.Module.Marsoau.Test.Events;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Late;
    
    private IEntityQueryService _peql;
    private IPamelloUserRepository _users;
    private IPamelloSongRepository _songs;
    private IPamelloPlaylistRepository _playlists;
    private IPamelloEpisodeRepository _episodes;
    private IPamelloPlayerRepository _players;
    private IPamelloLogger _logger;
    private IPlatformService _platforms;
    private IPamelloCommandsService _commands;
    private IEventsService _events;
    private IFileAccessService _files;
    private IDownloadService _downloaders;
    private IHistoryService _history;
    
    private IPamelloUser _me => _users.GetRequired(1);
    private IPamelloUser _ferrout => _users.GetRequired(3);
    private IPamelloUser _pivozavr => _users.GetRequired(2);
    private IPamelloUser _jombis => _users.GetRequired(4);

    public async Task StartupAsync(IServiceProvider services) {
        _peql = services.GetRequiredService<IEntityQueryService>();
        _users = services.GetRequiredService<IPamelloUserRepository>();
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _players = services.GetRequiredService<IPamelloPlayerRepository>();
        _logger = services.GetRequiredService<IPamelloLogger>();
        _platforms = services.GetRequiredService<IPlatformService>();
        _commands = services.GetRequiredService<IPamelloCommandsService>();
        _events = services.GetRequiredService<IEventsService>();
        _files = services.GetRequiredService<IFileAccessService>();
        _downloaders = services.GetRequiredService<IDownloadService>();
        _history = services.GetRequiredService<IHistoryService>();
        
        _history.FullReset();
        _history.WriteAll();
        
        _events.Subscribe<SongDeleted>((user, e) => {
            Console.WriteLine($"Song {e.Song} deleted by {user}");
            //e.RevertPack.Revert();
        });
        _events.Subscribe<SongRestored>(e => {
            Console.WriteLine($"Song {e.Song} restored");
            //e.RevertPack.Revert();
        });
        
        var song = _songs.GetRequired(11);

        Console.WriteLine($"Got song: <{song.Episodes.Count}> {song}");

        var record = _songs.Delete(song, _me);

        Console.WriteLine($"Get record: {record}, waiting to revert");
        Console.ReadKey(true);
        
        record.Revert(_me);
        
        _history.WriteAll();
    }
}
