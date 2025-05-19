namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPullPoint : IAudioPoint
{
    public Task<bool> TryPull(byte[] buffer); //AI: pull PO: on request
    public Task Pull(byte[] buffer); //AI: pull PO: on request
}