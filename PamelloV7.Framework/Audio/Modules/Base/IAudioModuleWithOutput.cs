using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModuleWithOutput : IAudioModuleWithOutputs
{
    int IAudioModuleWithOutputs.MinOutputs => 1;
    
    public IAudioPoint Output { get; }
}
