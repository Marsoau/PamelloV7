using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemoteEpisodeRepository : RemoteRepository<RemoteEpisode, PamelloEpisodeDTO>
    {
        public RemoteEpisodeRepository(PamelloClient client) : base(client) {
        }

        protected override Task<PamelloEpisodeDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloEpisodeDTO>($"Data/Episode?id={id}");
        protected override Task<PamelloEpisodeDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloEpisodeDTO>($"Data/Episode?value={value}");
        protected override RemoteEpisode CreateRemoteEntity(PamelloEpisodeDTO dto)
            => new RemoteEpisode(dto, _client);
    }
}
