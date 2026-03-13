using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.Osu.Config;

[ConfigRoot]
public partial class OsuNode
{
    public partial class AuthorizationNode
    {
        public string ApplicationId { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
