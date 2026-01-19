using PamelloV7.Core.Config;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Module.Marsoau.Discord.Config.Root.Tokens;

public class Tokens : IConfigNode
{
    public string Main { get; set; }
    public string[] SpeakerTokens { get; set; } = [];
    
    public void EnsureRight() {
        if (Main is null || Main.Length == 0) throw new PamelloConfigException("Main bot token is required for discord config");
    }
}