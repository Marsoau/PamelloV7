using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config;

namespace PamelloV7.Module.Marsoau.Discord.Config;

[StaticConfigPart]
public static class DiscordConfig
{
    public static Root.Root Root { get; set; }
}
