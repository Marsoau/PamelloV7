using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Osu.Config.Root.Authorization;

public class Authorization : IConfigNode
{
    public string ApplicationId { get; set; }
    public string Token { get; set; }
    
    public void EnsureRight() {
    }
}
