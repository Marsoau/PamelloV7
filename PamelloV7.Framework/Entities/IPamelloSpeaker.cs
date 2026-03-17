using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.DTO.Speakers;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;

namespace PamelloV7.Framework.Entities;

[PamelloEntity("speakers", typeof(PamelloSpeakerDto))]
public interface IPamelloSpeaker : IPamelloEntity
{
    IPamelloPlayer Player { get; }
    
    IEnumerable<IPamelloListener> Listeners { get; }
    
    IAudioModuleWithInput InputModule { get; }
    
    public bool IsAvailableFor(IPamelloUser user);
}
