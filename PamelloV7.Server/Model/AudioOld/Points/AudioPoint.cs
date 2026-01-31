using PamelloV7.Server.Model.AudioOld.Interfaces;

namespace PamelloV7.Server.Model.AudioOld.Points;

public abstract class AudioPoint : IAudioPoint
{
    public int Id { get; }
    
    private IAudioPoint? _backPoint;
    private IAudioPoint? _frontPoint;
    private IAudioModule _parentModule;
    
    public IAudioPoint? BackPoint => _backPoint;
    public IAudioPoint? FrontPoint => _frontPoint;
    public IAudioModule ParentModule => _parentModule;
    
    public bool IsDisposed { get; private set; }

    private static int _idCounter = 1;
    public AudioPoint(IAudioModule parentModule)
    {
        _parentModule = parentModule;
        Id = _idCounter++;
    }
    
    public void ConnectBack(IAudioPoint point) {
        if (_backPoint is not null) DisconnectBack();
        if (point is null) {
            Console.WriteLine("WARNING: Attempted to connect to a null back point");
            return;
        }
        
        _backPoint = point;
        
        if (point.FrontPoint != this) point.ConnectFront(this);
    }

    public void ConnectFront(IAudioPoint point) {
        if (_frontPoint is not null) DisconnectFront();
        if (point is null) {
            Console.WriteLine("WARNING: Attempted to connect to a null front point");
            return;
        }
        
        _frontPoint = point;
        
        if (point.BackPoint != this) point.ConnectBack(this);
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

    protected string OneSideString(bool front)
    {
        return front ?
            $"{Id}>{FrontPoint?.Id.ToString() ?? "none"}" :
            $"{Id}<{BackPoint?.Id.ToString() ?? "none"}";
    }
    public override string ToString()
    {
        return $"{(
                BackPoint is not null ?
                ((AudioPoint)BackPoint).OneSideString(false) :
                "none"
            )}|{Id}|{(
                FrontPoint is not null ?
                ((AudioPoint)FrontPoint).OneSideString(true) :
                "none"
            )}";
    }

    public void Dispose()
    {
        IsDisposed = true;
        
        DisconnectFront();
        DisconnectBack();
    }
}