using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;

namespace PamelloV7.Module.Marsoau.YouTube;

public class YouTube : IPamelloModule
{
    public string Name => "YouTube";
    public string Author => "Marsoau";
    public string Description => "YouTube song platform implementation";
    public ELoadingStage Stage => ELoadingStage.Default;
}
