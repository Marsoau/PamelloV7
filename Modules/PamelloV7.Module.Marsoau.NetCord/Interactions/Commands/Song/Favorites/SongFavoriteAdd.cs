using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Favorites;

[DiscordCommand("song favorite add", "Add songs to favorites")]
public partial class SongFavoriteAdd
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IEnumerable<IPamelloSong> songs
    ) {
        var addedSongs = Command<Framework.Commands.SongFavoritesAdd>().Execute(songs);
        
        await RespondOneOrManyAsync(
            addedSongs,
            song => [
                Builder<BasicComponentsBuilder>().Info("Added to favorite", song.ToDiscordString())
            ],
            $"Added {DiscordString.Code(addedSongs.Count)} favorite songs"
        );
    }
}
