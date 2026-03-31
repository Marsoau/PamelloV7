using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Config;

[ConfigRoot]
public class NetCordNode
{
    public required string Token { get; set; }
}
