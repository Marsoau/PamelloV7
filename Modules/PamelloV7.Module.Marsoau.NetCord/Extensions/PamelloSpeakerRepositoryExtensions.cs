using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Speakers;

namespace PamelloV7.Module.Marsoau.NetCord.Extensions;

public static class PamelloSpeakerRepositoryExtensions
{
    extension (IPamelloSpeakerRepository speakers)
    {
        public List<IPamelloSpeaker> GetByVcId(ulong vcId, IPamelloUser scopeUser) {
            if (vcId == 0) return [];
            
            return speakers.GetAll(scopeUser)
                .OfType<PamelloDiscordSpeaker>()
                .Where(s => s.VoiceClient is not null && s.VoiceClient.ChannelId == vcId)
                .OfType<IPamelloSpeaker>()
                .ToList();
        }
    }
}
