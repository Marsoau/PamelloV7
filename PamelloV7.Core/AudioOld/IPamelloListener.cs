namespace PamelloV7.Core.AudioOld;

public interface IPamelloListener
{
    public int Id { get; }
    public TaskCompletionSource Completion { get; }
}
