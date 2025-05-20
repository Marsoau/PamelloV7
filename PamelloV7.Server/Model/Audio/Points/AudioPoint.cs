using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public abstract class AudioPoint : IAudioPoint
{
    public int Id { get; }
    
    private IAudioPoint? _backPoint;
    private IAudioPoint? _frontPoint;
    
    public IAudioPoint? BackPoint => _backPoint;
    public IAudioPoint? FrontPoint => _frontPoint;

    private static int _idCounter = 1;
    public AudioPoint()
    {
        Id = _idCounter++;
    }
    
    public void ConnectBack(IAudioPoint point) {
        if (_backPoint is not null) return;
        if (point is null) {
            Console.WriteLine("WARNING: Attempted to connect to a null back point");
            return;
        }
        
        _backPoint = point;
        
        point.ConnectFront(this);
    }

    public void ConnectFront(IAudioPoint point) {
        if (_frontPoint is not null) return;
        if (point is null) {
            Console.WriteLine("WARNING: Attempted to connect to a null front point");
            return;
        }
        
        _frontPoint = point;
        
        point.ConnectBack(this);
    }

    public void DisconnectBack() {
        if (_backPoint is null) return;
        
        var oldPoint = _backPoint;
        _backPoint = null;
        
        oldPoint.DisconnectFront();
    }

    public void DisconnectFront() {
        if (_frontPoint is null) return;
        
        var oldPoint = _frontPoint;
        _frontPoint = null;
        
        oldPoint.DisconnectBack();
    }

    public override string ToString()
    {
        return $"<{(
                BackPoint is not null ?
                BackPoint.Id :
                "none"
            )}>{Id}<{(
                FrontPoint is not null ?
                FrontPoint.Id :
                "none"
            )}>";
    }
}