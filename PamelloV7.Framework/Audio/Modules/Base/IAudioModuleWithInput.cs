using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModuleWithInput : IAudioModuleWithInputs
{
    int IAudioModuleWithInputs.MinInputs => 1;

    public IAudioPoint Input { get; }
}
