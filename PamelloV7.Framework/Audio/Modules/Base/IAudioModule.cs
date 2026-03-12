using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModule
{
    public bool HasOutputs => false;
    public bool HasInputs => false;
    
    public void InitAudio(IServiceProvider serviceProvider) { }
}
