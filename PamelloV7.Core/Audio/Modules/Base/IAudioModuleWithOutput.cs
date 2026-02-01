using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Core.Audio.Modules.Base;

public interface IAudioModuleWithOutput : IAudioModuleWithOutputs
{
    int IAudioModuleWithOutputs.MinOutputs => 1;
    
    public IAudioPoint Output { get; }
}
