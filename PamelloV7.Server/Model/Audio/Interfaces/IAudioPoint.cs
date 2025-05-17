namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPoint
{
    IAudioPoint? BackPoint { get; }
    IAudioPoint? FrontPoint { get; }

    public void ConnectBack(IAudioPoint point);
    public void ConnectFront(IAudioPoint point);
    
    public void DisconnectBack();
    public void DisconnectFront();
}