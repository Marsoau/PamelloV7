namespace PamelloV7.Server.Config.Root.Discord.Tokens;

public class Tokens : IConfigNode
{
    public string MainBotToken { get; set; }
    public string[] SpeakerTokens { get; set; } = [];
    
    public void EnsureRight() {
        if (MainBotToken is null || MainBotToken.Length == 0) throw new Exception("Main bot token is required for discord config");
    }
}