using Discord.Interactions;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Base;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Playlist.Favorites;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Associations;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Episodes;
using PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Song.Favorites;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.Playlist;

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
