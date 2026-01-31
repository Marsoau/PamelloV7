using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Audio.Services;

public interface IPamelloAudioSystem : IPamelloService
{
    public TAudioModule Register<TAudioModule>(TAudioModule module);
}
