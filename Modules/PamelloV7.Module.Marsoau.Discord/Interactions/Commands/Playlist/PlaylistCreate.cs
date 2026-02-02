using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Playlist;

public partial class Playlist
{
    [SlashCommand("create", "Create a playlist")]
    public async Task Create(
        [Summary("name", "Playlist name")] string playlistName, 
        [Summary("songs", "Songs to fill the playlist with")] string? songsQuery = null
    ) {
        var playlists = Services.GetRequiredService<IPamelloPlaylistRepository>();
        
        var playlist = playlists.Add(playlistName, Context.User);
        
        var songs = songsQuery is null ? [] : await GetAsync<IPamelloSong>(songsQuery);
        playlist.AddSongs(songs);

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.Info("Playlist Created", $"{playlist.ToDiscordString()}").Build()
        , playlist);
    }
}
