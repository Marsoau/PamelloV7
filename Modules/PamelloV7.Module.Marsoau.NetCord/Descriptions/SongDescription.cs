using PamelloV7.Module.Marsoau.NetCord.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Descriptions;

public class SongDescription : DescriptionAttribute {
    public SongDescription() : base("song", "Query for a single song (id/url/name/other..)") { }
}

public class SongsDescription : DescriptionAttribute {
    public SongsDescription() : base("songs", "Query for songs (id/url/name/other..)") { }
}
