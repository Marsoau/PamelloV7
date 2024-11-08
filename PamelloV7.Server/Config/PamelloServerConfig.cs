using PamelloV7.Core.Config;

namespace PamelloV7.Server.Config
{
    public class PamelloServerConfig : PamelloConfig
    {
        public string MainBotToken;

        public PamelloServerConfig() : base("Config/config.yaml") {

        }
    }
}
