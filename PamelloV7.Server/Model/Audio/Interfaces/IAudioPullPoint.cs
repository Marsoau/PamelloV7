namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPullPoint : IAudioPoint
{
    public Task<bool> Pull(byte[] buffer, bool wait = true); //AI: pull PO: on request
}