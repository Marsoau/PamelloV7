using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Associations;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Episodes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song.Favorites;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Song;

[Map]
[Group("song", "Actions with songs")]
public partial class Song : DiscordCommand
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

[Map]
public partial class SongInteractions : DiscordCommand
{ }
