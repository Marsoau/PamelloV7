using System.Reflection;
using PamelloV7.Core.Config;

namespace PamelloV7.Server.Config.Root;

public class Root : IConfigNode
{
    public string Host { get; set; } = "http://*:51630";
    public string HostName { get; set; } = "";
    public string DataPath { get; set; } = $"{AppContext.BaseDirectory}Data";
    public string[] DisabledModules { get; set; } = [];
    public Discord.Discord Discord { get; set; } = null;
    public Modules.Modules Modules { get; set; } = new();
    
    public void EnsureRight()
    {
        try {
            DataPath = Path.GetFullPath(DataPath);
        }
        catch {
            throw new Exception("Data path directory is invalid");
        }
    }
}