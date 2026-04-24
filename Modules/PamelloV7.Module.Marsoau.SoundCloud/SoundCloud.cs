using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;

namespace PamelloV7.Module.Marsoau.SoundCloud;

public class SoundCloud : IPamelloModule
{
    public string Name => "SoundCloud";
    public string Author => "Marsoau";
    public string Description => "SoundCloud song platform integration";
    public ELoadingStage Stage => ELoadingStage.Default;
    public int Color => 0xFF7700;
}
