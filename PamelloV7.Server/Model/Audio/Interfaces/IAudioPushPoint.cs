namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPushPoint : IAudioPoint
{
    public Task<bool> Push(byte[] audio, bool wait = true); //PI: on provide AO: push
}