using PamelloV7.Core.Audio.Points;

namespace PamelloV7.Core.Audio.Modules.Base;

public interface IAudioModuleWithInput : IAudioModuleWithInputs
{
    int IAudioModuleWithInputs.MinInputs => 1;

    public IAudioPoint Input { get; }
}
