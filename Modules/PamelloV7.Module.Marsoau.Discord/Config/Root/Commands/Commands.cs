using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Discord.Config.Root.Commands;

public class Commands : IConfigNode
{
    public bool AutoConnectOnAddition { get; set; } = true;
    public bool GlobalRegistration { get; set; } = true;
    public int UpdatableCommandsLifetime { get; set; } = 100;
    public ulong[] GuildsIds { get; set; } = [];
    
    public void EnsureRight() { }
}
