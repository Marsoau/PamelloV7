namespace PamelloV7.Core.Audio;

public interface IAudioPushPoint : IAudioPoint
{
    public new IAudioPushPoint? ConnectedPoint { get; }

    public bool Push(byte[] audio);
}
