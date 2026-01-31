namespace PamelloV7.Server.Model.AudioOld.Interfaces;

public interface IAudioPoint : IDisposable
{
    public int Id { get; }
    
    IAudioPoint? BackPoint { get; }
    IAudioPoint? FrontPoint { get; }
    IAudioModule? ParentModule { get; }
    
    public bool IsDisposed { get; }

    public void ConnectBack(IAudioPoint point);
    public void ConnectFront(IAudioPoint point);
    
    public void DisconnectBack();
    public void DisconnectFront();
}