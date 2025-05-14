namespace PamelloV7.Server.Config.Root.Discord.Commands.Aliases;

public class Alias : IConfigNode
{
    public string Name { get; set; }
    public string Command { get; set; }
    
    public void EnsureRight() {
        if (Name is null || Name.Length == 0) throw new Exception("Name is required for alias");
        if (Command is null || Command.Length == 0) throw new Exception("Command is required for alias");
    }
}