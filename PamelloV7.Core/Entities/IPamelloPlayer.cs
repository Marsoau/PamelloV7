using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Entities;

public interface IPamelloPlayer : IPamelloEntity
{
    public IPamelloUser Owner { get; }
    
    public bool IsProtected { get; set; }
    public bool IsPaused { get; set; }
}
