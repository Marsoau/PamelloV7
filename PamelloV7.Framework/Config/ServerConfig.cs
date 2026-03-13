using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Framework.Config;

[ConfigRoot]
public static partial class ServerConfig
{
    public static string Host { get; set; } = "http://*:51630";
    public static string HostName { get; set; } = "";
    public static string DataPath { get; set; } = $"{AppContext.BaseDirectory}Data";
    public static string[] DisabledModules { get; set; } = [];
}
