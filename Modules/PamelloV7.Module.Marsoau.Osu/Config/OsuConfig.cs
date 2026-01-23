using PamelloV7.Core.Attributes;

namespace PamelloV7.Module.Marsoau.Osu.Config;

[StaticConfigPart("Osu")]
public static class OsuConfig
{
    public static Root.Root Root { get; set; }
}
