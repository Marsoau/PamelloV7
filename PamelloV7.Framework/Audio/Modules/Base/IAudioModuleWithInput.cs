using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModuleWithInput : IAudioModule
{
    public AudioPoint Input { get; }
}
