using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Framework.Audio.Modules.Base;

public interface IAudioModuleWithOutput : IAudioModule
{
    public AudioPoint Output { get; }
}
