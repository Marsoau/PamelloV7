namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPullPoint : IAudioPoint
{
    public Task<bool> Pull(byte[] buffer, bool wait, CancellationToken token); //AI: pull PO: on request
}