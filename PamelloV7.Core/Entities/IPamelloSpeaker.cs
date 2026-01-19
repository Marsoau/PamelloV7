using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Entities;

public interface IPamelloSpeaker : IPamelloEntity
{
    IPamelloPlayer Player { get; }
}
