namespace PamelloV7.Framework.Entities.Other;

public interface IPamelloListener
{
    public IPamelloSpeaker Speaker { get; }
    public IPamelloUser? User { get; }
    
    public bool IsListening { get; }
}
