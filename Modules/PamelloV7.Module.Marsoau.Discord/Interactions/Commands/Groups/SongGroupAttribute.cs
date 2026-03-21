using PamelloV7.Module.Marsoau.Discord.Attributes;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class SongGroupAttribute : DiscordGroupAttribute {
    public SongGroupAttribute() : base("song", "Actions with songs") { }
}

public class SongFavoriteGroupAttribute : DiscordGroupAttribute {
    public SongFavoriteGroupAttribute() : base("song favorite", "Actions with favorite songs") { }
}

public class SongAssociationsGroupAttribute : DiscordGroupAttribute {
    public SongAssociationsGroupAttribute() : base("song associations", "Actions with songs associations") { }
}

public class SongEpisodesGroupAttribute : DiscordGroupAttribute {
    public SongEpisodesGroupAttribute() : base("song episodes", "Actions with songs episodes") { }
}
