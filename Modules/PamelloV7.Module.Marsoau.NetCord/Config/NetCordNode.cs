using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Config;

[ConfigRoot]
public partial class NetCordNode
{
    public partial class TokensNode {
        public required string Main { get; set; }
        public string[] SpeakerTokens { get; set; } = [];
    }
}
