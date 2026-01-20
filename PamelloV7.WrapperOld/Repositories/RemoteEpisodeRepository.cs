using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.WrapperOld.Model;

namespace PamelloV7.WrapperOld.Repositories
{
    public class RemoteEpisodeRepository : RemoteRepository<RemoteEpisode, PamelloEpisodeDTO>
    {
        protected override string ControllerName => "Episode";


        public RemoteEpisodeRepository(PamelloClient client) : base(client) {
            SubscribeToEventsDataUpdates();
        }

        internal void SubscribeToEventsDataUpdates() {
            _client.Events.OnEpisodeNameUpdated += async (e) => {
                var episode = await Get(e.EpisodeId, false);
                if (episode is not null) episode._dto.Name = e.Name;
            };
            _client.Events.OnEpisodeSkipUpdated += async (e) => {
                var episode = await Get(e.EpisodeId, false);
                if (episode is not null) episode._dto.Skip = e.Skip;
            };
            _client.Events.OnEpisodeStartUpdated += async (e) => {
                var episode = await Get(e.EpisodeId, false);
                if (episode is not null) episode._dto.Start = e.Start;
            };
        }

        protected override RemoteEpisode CreateRemoteEntity(PamelloEpisodeDTO dto)
            => new RemoteEpisode(dto, _client);
    }
}
