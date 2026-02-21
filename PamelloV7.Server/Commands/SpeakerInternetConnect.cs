using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Repositories;
using PamelloV7.Server.Speakers;

namespace PamelloV7.Server.Commands;

public class SpeakerInternetConnect : PamelloCommand
{
    public PamelloInternetSpeaker Execute(string? name = null) {
        name ??= Guid.NewGuid().ToString();
        
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloInternetSpeaker(speakers.NextId, name, ScopeUser.GuaranteedSelectedPlayer, Services);
        
        return speakers.Add(speaker);
    }
}

