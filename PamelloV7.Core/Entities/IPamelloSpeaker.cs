using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.Entities;

[ValueEntity("speakers")]
public interface IPamelloSpeaker : IPamelloEntity
{
    IPamelloPlayer? Player { get; }
    
    IEnumerable<IPamelloListener> Listeners { get; }
    
    IAudioModule Output { get; }

    public void Connect(ulong vcId) {
        
    }
    
    public bool IsAvailableFor(IPamelloUser user) {
        return Listeners.Any(listener => listener.User == user);
    }
}
