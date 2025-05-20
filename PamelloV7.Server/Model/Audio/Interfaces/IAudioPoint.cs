namespace PamelloV7.Server.Model.Audio.Interfaces;

public interface IAudioPoint
{
    public int Id { get; }
    
    IAudioPoint? BackPoint { get; }
    IAudioPoint? FrontPoint { get; }

    public void ConnectBack(IAudioPoint point);
    public void ConnectFront(IAudioPoint point);
    
    public void DisconnectBack();
    public void DisconnectFront();
}