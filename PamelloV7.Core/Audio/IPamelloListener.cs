namespace PamelloV7.Core.Audio;

public interface IPamelloListener
{
    public int Id { get; }
    public TaskCompletionSource Completion { get; }
}
