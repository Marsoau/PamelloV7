using Discord.Interactions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song;

[SongGroup]
public class SongInfoCommand : DiscordCommand
{
    [SlashCommand("info", "Get info about a song", runMode: RunMode.Async)]
    public async Task Info(
        [Summary("song", "Single song query")] string songQuery = "current"
    ) {
        Output.Write($"BSQ: {DateTime.Now.TimeOfDay}");
        var songs = await GetAsync<IPamelloSong>(songQuery);

        switch (songs.Count) {
            case 1: {
                var song = songs.First();
            
                await RespondUpdatableAsync(() =>
                    Builder<SongInfoBuilder>().Component(song).Build()
                , () => [song, ..song.FavoriteBy, ..song.Playlists]);
            } break;
            case >= 1: {
                await RespondUpdatablePageAsync(page =>
                    Builder<BasicComponentsBuilder>().EntitiesList($"{DiscordString.Code(songs.Count)} Songs", songs, page).Build()
                , () => [.. songs]);
            } break;
            default:
                await RespondComponentAsync(Builder<BasicComponentsBuilder>().Info("Song Info", $"No song found by query `{songQuery}`").Build());
            break;
        }
    }
    
}

public class SongInfoInteractions : DiscordCommand
{
    [ComponentInteraction("song-info-edit-name:*")]
    public async Task EditNameButton(string songQuery) {
        var song = await GetSingleAsync<IPamelloSong>(songQuery);
        if (song is null) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongRenameModal.Build(song));
    }

    [ComponentInteraction("song-info-associations-edit:*")]
    public async Task EditAssociationsButton(string songQuery) {
        var song = await GetSingleAsync<IPamelloSong>(songQuery);
        if (song is null) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongEditAssociationsModal.Build(song));
    }
    
    [ComponentInteraction("song-info-reset:*")]
    public async Task SongInfoResetButton(string songQuery) {
        var song = await GetSingleAsync<IPamelloSong>(songQuery);
        if (song is null) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongResetModal.Build(song, Services));
    }
    

    [ComponentInteraction("song-info-favorite:*")]
    public async Task FavoriteButton(string songQuery) {
        var song = await GetSingleAsync<IPamelloSong>(songQuery);
        if (song is null) {
            await ReleaseInteractionAsync();
            return;
        }
        
        if (Context.User.FavoriteSongs.Contains(song))
            Command<SongFavoritesRemove>().Execute([song]);
        else
            Command<SongFavoritesAdd>().Execute([song]);

        await ReleaseInteractionAsync();
    }
}
