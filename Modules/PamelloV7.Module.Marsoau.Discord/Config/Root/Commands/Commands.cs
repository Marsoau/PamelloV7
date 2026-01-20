using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Discord.Config.Root.Commands;

public class Commands : IConfigNode
{
    public bool GlobalRegistration { get; set; } = true;
    public ulong[] GuildsIds { get; set; } = [];
    
    public void EnsureRight() { }
}
