namespace PamelloV7.Framework.AudioOld;

public interface IPamelloListener
{
    public int Id { get; }
    public TaskCompletionSource Completion { get; }
}
