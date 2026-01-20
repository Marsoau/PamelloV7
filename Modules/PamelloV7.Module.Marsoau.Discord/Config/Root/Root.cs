using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Discord.Config.Root;

public class Root : IConfigNode
{
    public Tokens.Tokens Tokens { get; set; }
    public Commands.Commands Commands { get; set; }
    
    public void EnsureRight() {
        Tokens.EnsureRight();
        Commands.EnsureRight();
    }
}