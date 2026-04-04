using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Config;

[ConfigRoot]
public partial class NetCordNode
{
    public partial class TokensNode {
        public required string Main { get; set; }
        public string[] SpeakerTokens { get; set; } = [];
    }
    public partial class CommandsNode {
        public bool AutoConnectOnAddition { get; set; } = true;
        public bool GlobalRegistration { get; set; } = true;
        public int UpdatableCommandsLifetime { get; set; } = 240;
        public ulong[] GuildsIds { get; set; } = [];
    }
}
