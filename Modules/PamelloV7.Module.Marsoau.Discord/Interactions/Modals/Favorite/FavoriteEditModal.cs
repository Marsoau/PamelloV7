using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Enumerators;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Favorite;

public class FavoriteEditModal : DiscordModal
{
    public static Modal Build(IPamelloUser user, ESongOrPlaylist songOrPlaylist) {
        IReadOnlyList<IPamelloEntity> items = songOrPlaylist switch {
            ESongOrPlaylist.Song => user.FavoriteSongs,
            ESongOrPlaylist.Playlist => user.FavoritePlaylists,
        };
        
        var modalBuilder = new ModalBuilder()
            .WithTitle($"Edit favorite {songOrPlaylist.ToShortString()}s")
            .WithCustomId($"favorite-{songOrPlaylist.ToShortString()}-edit-modal:{user.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput($"New {songOrPlaylist.ToShortString()}s", new TextInputBuilder()
                        .WithCustomId("modal-input")
                        .WithValue(string.Join("\n", IPamelloEntity.GetIds(items)))
                        .WithStyle(TextInputStyle.Paragraph)
                        .WithRequired(false)
                    , "Queries separated by newlines")
            );
        
        return modalBuilder.Build();
    }

    [ModalSubmission("favorite-playlist-edit-modal")]
    public Task SubmitPlaylists(string userQuery)
        => Submit(userQuery, ESongOrPlaylist.Playlist);
    
    [ModalSubmission("favorite-song-edit-modal")]
    public Task SubmitSongs(string userQuery)
        => Submit(userQuery, ESongOrPlaylist.Song);
    
    public async Task Submit(string userQuery, ESongOrPlaylist songOrPlaylist) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);

        var newItemsStr = GetInputValue("modal-input");
        var newItemsQueries = newItemsStr.Replace('\n', ',');

        switch (songOrPlaylist) {
            case ESongOrPlaylist.Song: {
                var newSongs = await GetAsync<IPamelloSong>(newItemsQueries);
                user.ReplaceFavoriteSongs(newSongs);
            } break;
            case ESongOrPlaylist.Playlist: {
                var newPlaylists = await GetAsync<IPamelloPlaylist>(newItemsQueries);
                user.ReplaceFavoritePlaylists(newPlaylists);
            } break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(songOrPlaylist), songOrPlaylist, null);
        }
        
        await ReleaseInteractionAsync();
    }
}
