using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Audio.Services;

public interface IPamelloAudioSystem : IPamelloService
{
    public TAudioModule RegisterModule<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule;
    
    public TAudioModule DeleteModule<TAudioModule>(TAudioModule module)
    
        where TAudioModule : class, IAudioModule;
    public TAudioDependant RegisterDependant<TAudioDependant>(TAudioDependant dependant)
        where TAudioDependant : class, IAudioDependant;
}
