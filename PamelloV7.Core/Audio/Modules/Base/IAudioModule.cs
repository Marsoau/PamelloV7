using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Core.Audio.Modules.Base;

public interface IAudioModule
{
    public bool HasOutputs => false;
    public bool HasInputs => false;
    
    public void InitAudio() { }
}
