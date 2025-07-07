using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloSpeaker : IPamelloEntity
{
    IPamelloPlayer Player { get; }
}
