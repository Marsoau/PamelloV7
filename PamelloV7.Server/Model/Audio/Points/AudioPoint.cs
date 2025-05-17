using PamelloV7.Server.Model.Audio.Interfaces;

namespace PamelloV7.Server.Model.Audio.Points;

public abstract class AudioPoint : IAudioPoint
{
    private IAudioPoint? _backPoint;
    private IAudioPoint? _frontPoint;
    
    public IAudioPoint? BackPoint => _backPoint;
    public IAudioPoint? FrontPoint => _frontPoint;
    
    public void ConnectBack(IAudioPoint point) {
        if (_backPoint is not null) return;
        
        _backPoint = point;
        
        point.ConnectFront(this);
    }

    public void ConnectFront(IAudioPoint point) {
        if (_frontPoint is not null) return;
        
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
}