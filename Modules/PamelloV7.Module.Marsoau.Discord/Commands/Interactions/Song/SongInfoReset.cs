using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song;

public partial class Song
{
    [SlashCommand("info-reset", "Reset song info from a source")]
    public async Task InfoReset(
        [Summary("songs", "Songs query")] string songsQuery,
        [Summary("source-index", "Source index to reset from")] int sourcePos = 0
    ) {
        var songs = _peql.Get<IPamelloSong>(songsQuery, Context.User);
        var processedSongs = new List<IPamelloSong>();

        var message = await RespondUpdatableAsync(message => {
            var title = "";
            var content = "";
            
            if (processedSongs.Count < songs.Count) {
                var lastSong = processedSongs.LastOrDefault();
                
                title = $"Resetting... `[{processedSongs.Count}/{songs.Count}]`";
                content = lastSong is null ? "No songs reset yet" : $"`[{lastSong.Id}]` {lastSong.Name}";
            }
            else {
                title = $"{processedSongs.Count} Songs Were Reset";
                content = string.Join("\n", processedSongs.Skip(processedSongs.Count - 5).Select(song => $"`[{song.Id}]` {song.Name}"));
                if (processedSongs.Count > 5) content += $"\n_... And {processedSongs.Count - 5} more_";
            }

            message.Components = PamelloComponentBuilders.Info(title, content).Build();
        }, () => [.. processedSongs]);

        foreach (var song in songs) {
            processedSongs.Add(song);
            Command<SongInfoReset>().Execute(song, song.SelectedSourceIndex);
            song.Sources.ElementAtOrDefault(sourcePos)?.SetInfoToSong();
            await message.Refresh();
        }
    }
}
