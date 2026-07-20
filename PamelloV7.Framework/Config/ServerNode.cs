using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Config.Loaders;

namespace PamelloV7.Framework.Config;

[ConfigRoot]
public partial class ServerNode
{
    public string Host { get; set; } = "http://*:51630";
    public string HostName { get; set; } = "";
    public bool AllowUserCreation { get; set; } = true;
    public bool FullResetHistory { get; set; } = false;
    public int MaximumQueryComplexity { get; set; } = 100;
}
