using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class EpisodeDeletionRevertPack : RevertPack<EpisodeDeleted>
{
    public DatabaseEpisode DatabaseEpisode { get; set; }

    protected override void RevertInternal(IPamelloUser scopeUser) {
        var episodes = (PamelloEpisodeRepository)Services.GetRequiredService<IPamelloEpisodeRepository>();
        
        Debug.WriteLine($"Reverted episode deletion, episode is restored: {DatabaseEpisode.Name}");
        
        episodes.Restore(scopeUser, DatabaseEpisode);
    }
}
