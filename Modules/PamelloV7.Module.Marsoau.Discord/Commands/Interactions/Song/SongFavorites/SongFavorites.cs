using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.SongFavorites;

[Group("song", "Actions with songs")]
public partial class Song
{
    [Group("favorites", "Actions with favorites")]
    public partial class SongFavorites : DiscordCommand
    { }
}
