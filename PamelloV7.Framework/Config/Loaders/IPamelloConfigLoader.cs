using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Config.Loaders;

public interface IPamelloConfigLoader
{
    public List<PamelloConfigPart> Parts { get; }
}
