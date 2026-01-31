namespace PamelloV7.Core.Audio;

public interface IAudioPullPoint : IAudioPoint
{
    public new IAudioPullPoint? ConnectedPoint { get; }

    public bool Pull(byte[] audio);
}
