using System.Text;
using System.Text.Json;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Converters;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.DTO;
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
    public IBrush Color => Brushes.LightGreen;

    private IEntityQueryService _peql;
    private IPamelloUserRepository _users;
    private IPamelloSongRepository _songs;
    private IPamelloPlaylistRepository _playlists;
    private IPamelloEpisodeRepository _episodes;
    private IPamelloPlayerRepository _players;
    private IPlatformService _platforms;
    private IPamelloCommandsService _commands;
    private IEventsService _events;
    private IFileAccessService _files;
    private IDownloadService _downloaders;
    private IHistoryService _history;
    private IDependenciesService _dependencies;
    
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
        _platforms = services.GetRequiredService<IPlatformService>();
        _commands = services.GetRequiredService<IPamelloCommandsService>();
        _events = services.GetRequiredService<IEventsService>();
        _files = services.GetRequiredService<IFileAccessService>();
        _downloaders = services.GetRequiredService<IDownloadService>();
        _history = services.GetRequiredService<IHistoryService>();
        _dependencies = services.GetRequiredService<IDependenciesService>();
    }

    public void WriteSongs(IEnumerable<IDeletableEntity> songs) {
        var counter = 0;
        foreach (var song in songs) {
            Output.Write($"{counter++}: {song}");
        }
    }
}
