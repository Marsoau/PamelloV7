using Discord;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Difference;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song.Favorite;

[Modal("favorites-edit-modal")]
public class SongFavoriteEditModal : DiscordModal
{
    public static Modal Build(IPamelloUser user) {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Edit favorite songs")
            .WithCustomId($"favorites-edit-modal:{user.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput("New songs", new TextInputBuilder()
                        .WithCustomId("modal-input")
                        .WithValue(string.Join("\n", IPamelloEntity.GetIds(user.FavoriteSongs)))
                        .WithStyle(TextInputStyle.Paragraph)
                        .WithRequired(false)
                    , "Songs separated by newlines")
            );
        
        return modalBuilder.Build();
    }
    public override async Task Submit(string userQuery) {
        var user = _peql.GetSingleRequired<IPamelloUser>(userQuery, User);

        var newSongsStr = GetInputValue("modal-input");
        var newSongsQueries = newSongsStr.Replace('\n', ',');

        var newSongs = _peql.Get<IPamelloSong>(newSongsQueries, User);
        
        await ReleaseInteractionAsync();

        user.ReplaceFavoriteSongs(newSongs);
    }
}
