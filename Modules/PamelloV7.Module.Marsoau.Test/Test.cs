using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Converters;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Framework.Extensions;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.History.Services;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.Test.Config;
using PamelloV7.Module.Marsoau.Test.Events;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Late;
    public int Color => 0x00FF7F;

    private IEntityQueryService _peql = null!;
    private IPamelloUserRepository _users = null!;
    private IPamelloSongRepository _songs = null!;
    private IPamelloPlaylistRepository _playlists = null!;
    private IPamelloEpisodeRepository _episodes = null!;
    private IPamelloPlayerRepository _players = null!;
    private IPlatformService _platforms = null!;
    private IPamelloCommandsService _commands = null!;
    private IEventsService _events = null!;
    private IFileAccessService _files = null!;
    private IDownloadService _downloaders = null!;
    private IHistoryService _history = null!;
    private IDependenciesService _dependencies = null!;
    
    private IPamelloUser Me => _users.GetRequired(1);
    private IPamelloUser Ferrout => _users.GetRequired(3);
    private IPamelloUser Pivozavr => _users.GetRequired(2);
    private IPamelloUser Jombis => _users.GetRequired(4);

    public async Task StartupAsync(IServiceProvider services) {
        _peql = services.GetRequiredService<IEntityQueryService>();
        _users = services.GetRequiredService<IPamelloUserRepository>();
        _songs = services.GetRequiredService<IPamelloSongRepository>();
        _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        _players = services.GetRequiredService<IPamelloPlayerRepository>();
        _platforms = services.GetRequiredService<IPlatformService>();
        _commands = services.GetRequiredService<IPamelloCommandsService>();
        _events = services.GetRequiredService<IEventsService>();
        _files = services.GetRequiredService<IFileAccessService>();
        _downloaders = services.GetRequiredService<IDownloadService>();
        _history = services.GetRequiredService<IHistoryService>();
        _dependencies = services.GetRequiredService<IDependenciesService>();
        
        var soundcloud = _platforms.GetSongPlatform("soundcloud")!;

        var key = soundcloud.ValueToKey("https://soundcloud.com/rorynearly20s/shihandu-takenu-des?in=kot3-694254000/sets/loly-in-early-20s&utm_source=direct&utm_content=download_button_header&utm_medium=mobi&utm_campaign=no_campaign");
        var song = await soundcloud.GetSongInfoAsync(key);
        
        Output.Write($"Key: {song!.Key}");
    }

    public void WriteSongs(IEnumerable<IDeletableEntity> songs) {
        var counter = 0;
        foreach (var song in songs) {
            Output.Write($"{counter++}: {song}");
        }
    }
}
