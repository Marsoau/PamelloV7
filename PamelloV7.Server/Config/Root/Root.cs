using System.Reflection;

namespace PamelloV7.Server.Config.Root;

public class Root : IConfigNode
{
    public string Host { get; set; } = "*:51630";
    public string DataPath { get; set; } = $"{AppContext.BaseDirectory}Data";
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
        
        Modules.EnsureRight();
        
        if (Modules.UseDiscord) {
            if (Discord is null) throw new Exception("UseDiscord module is true but discord config not provided");
            Discord.EnsureRight();
        }
    }
}