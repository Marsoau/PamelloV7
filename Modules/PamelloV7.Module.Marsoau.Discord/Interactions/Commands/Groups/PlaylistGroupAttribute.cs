using PamelloV7.Module.Marsoau.Discord.Attributes;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class PlaylistGroupAttribute : DiscordGroupAttribute {
    public PlaylistGroupAttribute() : base("playlist", "Actions with playlists") { }
}

public class PlaylistFavoriteGroupAttribute : DiscordGroupAttribute {
    public PlaylistFavoriteGroupAttribute() : base("playlist favorite", "Actions with favorite playlists") { }
}
