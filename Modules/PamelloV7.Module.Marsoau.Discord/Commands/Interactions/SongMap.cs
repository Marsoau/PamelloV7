using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Associations;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Episodes;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Favorites;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions;

[Group("song", "Actions with songs")]
public class SongMap : Song.Song
{
    [Group("favorites", "Actions with favorite songs")]
    public class SongFavoritesMap : SongFavorites
    { }
    [Group("associations", "Actions with songs associations")]
    public class SongAssociationsMap : SongAssociations
    { }
    [Group("episodes", "Actions with songs episodes")]
    public class SongEpisodesMap : SongEpisodes
    { }
}
