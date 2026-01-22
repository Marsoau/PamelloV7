using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song;
using SongNameUpdated = PamelloV7.Core.Events.SongNameUpdated;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song;

public partial class Song
{
    [SlashCommand("info", "Get info about a song", runMode: RunMode.Async)]
    public async Task Info(
        [Summary("song", "Single song query")] string songQuery = "12"
    ) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await RespondAsync("Nema tokogo");
            return;
        }

        await RespondUpdatableAsync(message => {
            message.Components = PamelloComponentBuilders.SongInfo(song, Context.User, Services).Build();
        }, song);
    }
}

public partial class SongInteractions
{
    [ComponentInteraction("song-info-edit-name:*")]
    public async Task EditNameButton(string songQuery) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await EndInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongRenameModal.Build(song));
    }

    [ComponentInteraction("song-info-associations-edit:*")]
    public async Task EditAssociationsButton(string songQuery) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await EndInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongEditAssociationsModal.Build(song));
    }
    
    [ComponentInteraction("song-info-reset:*")]
    public async Task SongInfoResetButton(string songQuery) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await EndInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(SongResetModal.Build(song, Services));
    }
    

    [ComponentInteraction("song-info-favorite:*")]
    public async Task FavoriteButton(string songQuery) {
        var song = _peql.GetSingle<IPamelloSong>(songQuery, Context.User);
        if (song is null) {
            await EndInteractionAsync();
            return;
        }
        
        if (Context.User.FavoriteSongs.Contains(song))
            Command<SongFavoritesRemove>().Execute([song]);
        else
            Command<SongFavoritesAdd>().Execute([song]);

        await EndInteractionAsync();
    }
}
