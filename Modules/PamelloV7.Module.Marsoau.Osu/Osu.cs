using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;
using PamelloV7.Module.Marsoau.Osu.Services;

namespace PamelloV7.Module.Marsoau.Osu;

public class Osu : IPamelloModule
{
    public string Name => "Osu";
    public string Author => "Marsoau";
    public string Description => "Osu! user & song platform integration";
    public ELoadingStage Stage => ELoadingStage.Default;
}
