using PamelloV7.Core.Attributes;
using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Discord.Config;

[StaticConfigPart]
public static class DiscordConfig
{
    public static Root.Root Root { get; set; }
}
