using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config.Attributes;

namespace PamelloV7.Module.Marsoau.Osu.Config;

[ConfigRoot]
public partial class OsuNode
{
    public partial class AuthorizationNode
    {
        public required string ApplicationId { get; set; } = null!;
        public required string Token { get; set; } = null!;
    }
}
