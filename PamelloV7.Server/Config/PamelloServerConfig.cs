using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace PamelloV7.Server.Config
{
    public static class PamelloServerConfig
    {
        public static Root.Root Root { get; private set; }
        
        static PamelloServerConfig()
        {
            var configFile = File.OpenRead("Config/config.json");
            Root = JsonSerializer.Deserialize<Root.Root>(configFile) ?? throw new Exception("Failed to load config.json");
            Root.EnsureRight();
        }
    }
}
