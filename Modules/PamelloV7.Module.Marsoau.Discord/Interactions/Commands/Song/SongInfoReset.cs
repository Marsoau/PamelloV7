using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song;

public partial class Song
{
    [SlashCommand("info-reset", "Reset song info from a source")]
    public async Task InfoReset(
        [Summary("songs", "Songs query")] string songsQuery,
        [Summary("source-index", "Source index to reset from")] int sourcePos = 0
    ) {
        var songs = await GetAsync<IPamelloSong>(songsQuery);
        var processedSongs = new List<IPamelloSong>();

        if (songs.Count == 0) {
            await RespondComponentAsync(PamelloComponentBuilders.Info("Songs Reset", $"No songs found by query `{songsQuery}`").Build());
            return;
        }

        var message = await RespondUpdatableAsync(message => {
            var title = "";
            var content = "";
            
            if (processedSongs.Count < songs.Count) {
                var lastSong = processedSongs.LastOrDefault();
                
                title = $"Resetting... `[{processedSongs.Count}/{songs.Count}]`";
                content = lastSong is null ? "No songs reset yet" : $"`[{lastSong.Id}]` {lastSong.Name}";
            }
            else {
                title = $"{DiscordString.Code(processedSongs.Count)} Songs Were Reset";
                content = string.Join("\n", processedSongs.Skip(processedSongs.Count - 5).Select(song => $"`[{song.Id}]` {song.Name}"));
                if (processedSongs.Count > 5) content += $"\n{DiscordString.Italic($"... And {processedSongs.Count - 5} more")}";
            }

            message.Components = PamelloComponentBuilders.Info(title, content).Build();
        }, () => [.. processedSongs.Skip(processedSongs.Count - 5)]);

        foreach (var song in songs) {
            await Command<SongInfoReset>().Execute(song, song.SelectedSourceIndex);
            
            processedSongs.Add(song);
            
            message.Touch();
            await message.Refresh();
        }
    }
}
