namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPushPoint : IAudioPoint
{
    public Task<bool> TryPush(byte[] audio); //PI: on provide AO: push
    public Task Push(byte[] audio); //PI: on provide AO: push
}