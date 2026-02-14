using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Core.Extensions;
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

    public async Task StartupAsync(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var songs = services.GetRequiredService<IPamelloSongRepository>();
        var playlists = services.GetRequiredService<IPamelloPlaylistRepository>();
        var episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
        var players = services.GetRequiredService<IPamelloPlayerRepository>();
        var logger = services.GetRequiredService<IPamelloLogger>();
        var platforms = services.GetRequiredService<IPlatformService>();
        var commands = services.GetRequiredService<IPamelloCommandsService>();
        var events = services.GetRequiredService<IEventsService>();
        var files = services.GetRequiredService<IFileAccessService>();
        var downloaders = services.GetRequiredService<IDownloadService>();

        var me = users.GetRequired(1);

        events.Subscribe<Jombis>(async e => {
            events.Invoke(new PlayerNameUpdated());
        });
        events.Subscribe<TestNestedEvent>(async e => {
            events.Invoke(new Jombis());
        });

        events.Invoke(new TestNestedEvent());
        events.Invoke(new Jombis());
    }
}
