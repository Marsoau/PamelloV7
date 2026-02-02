using Discord.Interactions;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Builders.Components;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player.Queue;

public partial class PlayerQueue
{
    [SlashCommand("song-add", "Add songs to the queue")]
    public async Task SongAdd(
        [Summary("songs", "Songs query")] string songsQuery
    ) {
        var songs = await GetAsync<IPamelloSong>(songsQuery);
        
        var addedSongs = Command<PlayerQueueSongAdd>().Execute(songs).ToList();

        switch (addedSongs.Count) {
            case 1: {
                var song = addedSongs.First();
            
                await RespondUpdatableAsync(() =>
                    PamelloComponentBuilders.RefreshButton(AddedSongsComponent.GetForOne(song)).Build()
                , song);
            } break;
            case >= 1: {
                await RespondUpdatablePageAsync(page =>
                        PamelloComponentBuilders.EntitiesList($"Added {DiscordString.Code(addedSongs.Count)} Songs", addedSongs, page).Build()
                    , () => [.. addedSongs]);
            } break;
            default:
                await RespondComponentAsync(PamelloComponentBuilders.Info("No Songs Added", $"No songs found by query `{songsQuery}`").Build());
            break;
        }
    }
}
