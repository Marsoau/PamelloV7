using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongAdd
{
    public IEnumerable<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs, string position = "last") {
        var addedSongs = RequiredQueue.InsertSongs(position, songs, ScopeUser).ToList();

        foreach (var song in addedSongs) {
            if (song.SelectedSource is null || song.SelectedSource.IsDownloaded()) continue;

            var downloader = song.SelectedSource.GetDownloader();
            if (downloader is null) continue;

            _ = downloader.DownloadAsync();
        }
        
        return addedSongs;
    }
}
