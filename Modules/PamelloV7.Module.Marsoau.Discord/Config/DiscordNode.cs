using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.Discord.Config;

//usage example
//Config.Discord.Tokens.Main
//Config.Discord.Commands.AutoConnectOnAddition

[ConfigRoot]
public partial class DiscordNode
{
    public string TestProperty { get; set; } = "test";
    
    public partial class TokensNode {
        public string Main { get; set; } = "";
        public string[] SpeakerTokens { get; set; } = [];
    }
    
    public partial class CommandsNode {
        public bool AutoConnectOnAddition { get; set; } = true;
        public bool GlobalRegistration { get; set; } = true;
        public int UpdatableCommandsLifetime { get; set; } = 100;
        public ulong[] GuildsIds { get; set; } = [];
    }
}

[StaticConfigPart]
public static class DiscordConfigOld
{
    public static Root.Root Root { get; set; }
}