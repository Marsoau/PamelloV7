using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

public class EpisodeDelete : PamelloCommand
{
    public void Execute(IPamelloEpisode episode) {
        var episodes = Services.GetRequiredService<IPamelloEpisodeRepository>();
        episodes.Delete(episode, ScopeUser);
    }
}
