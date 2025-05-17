namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPushPoint : IAudioPoint
{
    public Task Push(byte[] audio); //PI: on provide AO: push
}