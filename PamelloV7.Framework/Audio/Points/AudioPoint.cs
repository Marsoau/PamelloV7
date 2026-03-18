using PamelloV7.Framework.Audio.Modules.Base;

namespace PamelloV7.Framework.Audio.Points;

public class AudioPoint
{
    public int Id { get; }

    private AudioPoint? _connectedPoint;
    public AudioPoint? ConnectedPoint {
        get => _connectedPoint;
        set {
            if (value is not null) {
                if (value.ProcessAudio is null == ProcessAudio is null) throw new Exception("Cannot connect two points with same ProcessAudio is null result");
                
                if (_connectedPoint is not null) ConnectedPoint = null;
        
                _connectedPoint = value;
        
                if (_connectedPoint.ConnectedPoint != this) _connectedPoint.ConnectedPoint = this;
            }
            else {
                if (_connectedPoint is null) return;
        
                var oldPoint = _connectedPoint;
                _connectedPoint = null;
        
                if (oldPoint is not null)
                    oldPoint.ConnectedPoint = null;
            }
        }
    }
    public IAudioModule? ParentModule { get; }

    public Func<byte[], bool, CancellationToken, bool>? ProcessAudio { get; set; }
    
    private static int _idCounter = 1;
    public AudioPoint(IAudioModule parentModule) {
        Id = Interlocked.Increment(ref _idCounter);
        
        ParentModule = parentModule;
    }

    public bool Pass(byte[] audio, bool wait, CancellationToken token) {
        if (ProcessAudio is not null) {
            return ProcessAudio(audio, wait, token);
        }
        
        return ConnectedPoint?.Pass(audio, wait, token) ?? false;
    }
}
