using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Plugins;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Default;
}
