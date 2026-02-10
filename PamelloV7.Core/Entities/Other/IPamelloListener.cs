namespace PamelloV7.Core.Entities.Other;

public interface IPamelloListener
{
    public IPamelloSpeaker Speaker { get; }
    public IPamelloUser? User { get; }
}
