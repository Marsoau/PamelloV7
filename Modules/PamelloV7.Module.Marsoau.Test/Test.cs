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
        var logger = services.GetRequiredService<IPamelloLogger>();
        var platforms = services.GetRequiredService<IPlatformService>();
        var commands = services.GetRequiredService<IPamelloCommandsService>();
        var events = services.GetRequiredService<IEventsService>();
        var files = services.GetRequiredService<IFileAccessService>();
        var downloaders = services.GetRequiredService<IDownloadService>();

        var me = users.GetRequired(1);
        var song = await peql.GetSingleRequiredAsync<IPamelloSong>("16", me);

        Console.WriteLine($"File: {song.Sources[0].GetFile().FullName}");

        try {
            var downloader = song.Sources[0].GetDownloader();
            Console.WriteLine("Got downloader");
            var resultTask = downloader.DownloadAsync();
            if (!resultTask.IsCompleted) {
                Console.WriteLine("Waiting for download");
            }
            var result = await resultTask;
            Console.WriteLine($"result: {result}");
        }
        catch (Exception x) {
            Console.WriteLine($"No downloader: {x}");
        }
    }
}
