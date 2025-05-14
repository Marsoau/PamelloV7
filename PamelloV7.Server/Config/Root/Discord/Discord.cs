using PamelloV7.Server.Config.Root.Discord.MessageStyles;

namespace PamelloV7.Server.Config.Root.Discord;

public class Discord : IConfigNode
{
    public Tokens.Tokens Tokens { get; set; }
    public MessageStyles.MessageStyles MessageStyles { get; set; } = new();
    public Commands.Commands Commands { get; set; } = new();
    
    public void EnsureRight() {
        Tokens.EnsureRight();
        MessageStyles.EnsureRight();
        Commands.EnsureRight();
    }
}