using PamelloV7.Core.Config;

namespace PamelloV7.Server.Config
{
    public class PamelloServerConfig : PamelloConfig
    {
        public string MainBotToken;
        public string Speaker1Token;

        public PamelloServerConfig() : base("Config/config.yaml") {

        }
    }
}
