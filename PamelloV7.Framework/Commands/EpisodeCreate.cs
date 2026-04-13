using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class EpisodeCreate
{
    public IPamelloEpisode Execute(IPamelloSong song, AudioTime start, string name, bool autoSkip) {
        var episodes = Services.GetRequiredService<IPamelloEpisodeRepository>();
        var episode = episodes.Add(start, name, autoSkip, song, ScopeUser);
        
        return episode;
    }
}
