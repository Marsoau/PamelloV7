namespace PamelloV7.Core.Model.Audio;

public interface IPamelloListener
{
    public int Id { get; }
    public TaskCompletionSource Completion { get; }
}
