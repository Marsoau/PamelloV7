using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Audio.Points;

public class AudioPoint : IAudioPoint
{
    public int Id { get; }

    public IAudioPoint? ConnectedPoint {
        get; set {
            if (value is not null) {
                if (value.ProcessAudio is null == ProcessAudio is null) throw new Exception("Cannot connect two points with same ProcessAudio is null result");
                
                if (field is not null) ConnectedPoint = null;
        
                field = value;
        
                if (field.ConnectedPoint != this) field.ConnectedPoint = this;
            }
            else {
                if (field is null) return;
        
                var oldPoint = field;
                field = null;
        
                oldPoint?.ConnectedPoint = null;
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
