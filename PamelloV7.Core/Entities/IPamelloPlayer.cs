using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.Entities;

[ValueEntity("players")]
public interface IPamelloPlayer : IPamelloEntity
{
    public IPamelloUser Owner { get; }
    
    public bool IsProtected { get; set; }
    public bool IsPaused { get; set; }
    
    public IAudioModule Pump { get; }
    
    public IPamelloQueue? Queue { get; }
    public IPamelloQueue RequiredQueue { get; }
    
    public IEnumerable<IPamelloSpeaker> ConnectedSpeakers { get; }

    public bool IsAvailableFor(IPamelloUser user) {
        if (Owner == user) return true;
        
        return ConnectedSpeakers.Any(speaker => speaker.IsAvailableFor(user));
    }

    public IPamelloSpeaker AddSpeaker(IPamelloSpeaker speaker);
}
