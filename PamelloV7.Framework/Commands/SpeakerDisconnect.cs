using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SpeakerDisconnect
{
    public void Execute(IPamelloSpeaker speaker) {
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        
        speakers.Delete(speaker, ScopeUser);
    }
}

