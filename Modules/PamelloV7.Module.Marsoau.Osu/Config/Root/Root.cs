using PamelloV7.Core.Config;

namespace PamelloV7.Module.Marsoau.Osu.Config.Root;

public class Root : IConfigNode
{
    public Authorization.Authorization Authorization { get; set; }
    
    public void EnsureRight() {
    }
}
