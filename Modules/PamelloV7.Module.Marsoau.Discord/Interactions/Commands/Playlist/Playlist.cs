using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Playlist.Favorites;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Playlist;

[Map]
[Group("playlist", "Actions with playlists")]
public partial class Playlist : DiscordCommand
{
    [Group("favorite", "Actions with favorite playlists")]
    public class PlaylistFavoriteMap : PlaylistFavorite
    { }
}

[Map]
public partial class PlaylistInteractions : DiscordCommand
{ }
