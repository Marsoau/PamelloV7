using PamelloV7.Core.Config;

namespace PamelloV7.Server.Config.Root.Modules;

public class Modules : IConfigNode
{
    public bool UseDiscord { get; set; } = true;
    
    public void EnsureRight() {
    }
}