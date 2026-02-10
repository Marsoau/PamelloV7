namespace PamelloV7.Core.Entities.Other;

public interface IPamelloListener
{
    public IPamelloUser User { get; }
    public IPamelloSpeaker Speaker { get; }
}
