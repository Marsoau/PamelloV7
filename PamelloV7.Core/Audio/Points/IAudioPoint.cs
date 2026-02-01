using PamelloV7.Core.Audio.Modules.Base;

namespace PamelloV7.Core.Audio.Points;

public interface IAudioPoint
{
    public int Id { get; }
    
    public IAudioPoint? ConnectedPoint { get; set; }
    public IAudioModule? ParentModule { get; }
    
    public Func<byte[], bool>? ProcessAudio { get; set; }
    public bool Pass(byte[] audio, bool wait, CancellationToken token);
}
