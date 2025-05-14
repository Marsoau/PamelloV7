namespace PamelloV7.Server.Config.Root.Discord.Commands;

public class Commands : IConfigNode
{
    public bool GlobalRegistration { get; set; } = false;
    public int[] GuildsIds { get; set; } = [];
    public Aliases.Alias[] Aliases { get; set; } = [];
    
    public void EnsureRight() {
        foreach (var alias in Aliases) {
            alias.EnsureRight();
        }
    }
}