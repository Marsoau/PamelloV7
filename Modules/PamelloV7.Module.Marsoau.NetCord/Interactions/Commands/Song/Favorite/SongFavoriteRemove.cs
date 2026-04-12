using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Favorites;

[DiscordCommand("song favorite remove", "Remove songs from favorites")]
public partial class SongFavoriteRemove
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IEnumerable<IPamelloSong> songs
    ) {
        var addedSongs = Command<Framework.Commands.SongFavoritesRemove>().Execute(songs);
        
        await RespondOneOrManyAsync(
            addedSongs,
            song => [
                BasicComponentsBuilder.Info("Removed from favorite", song.ToDiscordString())
            ],
            $"Removed {DiscordString.Code(addedSongs.Count)} favorite songs"
        );
    }
}
