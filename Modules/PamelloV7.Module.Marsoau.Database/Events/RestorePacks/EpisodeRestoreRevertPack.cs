using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class EpisodeRestoreRevertPack : RevertPack<EpisodeRestored>
{
    protected override void RevertInternal(IPamelloUser scopeUser) {
        if (Event.Episode is null) throw new PamelloException("Cannot revert episode restore, episode is null");
        
        var episodes = (PamelloEpisodeRepository)Services.GetRequiredService<IPamelloEpisodeRepository>();
        
        episodes.Delete(Event.Episode, scopeUser);
        
        Debug.WriteLine($"Reverted episode restore, {Event.Episode} is deleted");
    }
}
