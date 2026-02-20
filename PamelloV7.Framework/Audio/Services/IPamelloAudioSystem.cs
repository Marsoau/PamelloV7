using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Audio.Services;

public interface IPamelloAudioSystem : IPamelloService
{
    public TAudioModule RegisterModule<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule;
    
    public void DeleteModule<TAudioModule>(TAudioModule module)
        where TAudioModule : class, IAudioModule;
    
    public TAudioDependant RegisterDependant<TAudioDependant>(TAudioDependant dependant)
        where TAudioDependant : class, IAudioDependant;
}
