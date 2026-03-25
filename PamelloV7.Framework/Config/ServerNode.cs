using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Framework.Config;

[ConfigRoot]
public partial class ServerNode
{
    public string Host { get; set; } = "http://*:51630";
    public string HostName { get; set; } = "";
    public string DataPath { get; set; } = $"{AppContext.BaseDirectory}Data";
    public string[] DisabledModules { get; set; } = [];
    public bool UseConsolonia { get; set; } = true;
}
