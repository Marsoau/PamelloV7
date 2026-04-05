using PamelloV7.Module.Marsoau.NetCord.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Descriptions;

public class PlayerDescription : DescriptionAttribute {
    public PlayerDescription() : base("player", "Query for a single player") { }
}

public class PlayersDescription : DescriptionAttribute {
    public PlayersDescription() : base("players", "Query for many players") { }
}
