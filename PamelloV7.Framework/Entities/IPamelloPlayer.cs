using PamelloV7.Framework.Audio.Attributes;
using PamelloV7.Framework.AudioOld;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;

namespace PamelloV7.Framework.Entities;

[ValueEntity("players")]
public interface IPamelloPlayer : IPamelloEntity
{
    public IPamelloUser Owner { get; }
    
    public bool IsProtected { get; }
    public bool IsPaused { get; }
    public bool SetPause(bool state, IPamelloUser? scopeUser);
    
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
