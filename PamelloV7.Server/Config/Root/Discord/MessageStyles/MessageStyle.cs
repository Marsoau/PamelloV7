namespace PamelloV7.Server.Config.Root.Discord.MessageStyles;

public class MessageStyle : IConfigNode
{
    public string Color { get; set; } = "";
    
    public void EnsureRight() {
        if (Color is null || Color.Length == 0) throw new Exception("Color is required for message style");
    }
}