using PamelloV7.Module.Marsoau.NetCord.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Descriptions;

public class PlaylistDescription : DescriptionAttribute {
    public PlaylistDescription() : base("playlist", "Query for a single playlist") { }
}

public class PlaylistsDescription : DescriptionAttribute {
    public PlaylistsDescription() : base("playlists", "Query for many playlists") { }
}
