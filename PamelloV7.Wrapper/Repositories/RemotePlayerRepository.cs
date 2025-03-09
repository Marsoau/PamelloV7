using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemotePlayerRepository : RemoteRepository<RemotePlayer, PamelloPlayerDTO>
    {
        public RemotePlayerRepository(PamelloClient client) : base(client) {
        }

        protected override Task<PamelloPlayerDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloPlayerDTO>($"Data/Player?id={id}");
        protected override Task<PamelloPlayerDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloPlayerDTO>($"Data/Player?value={value}");
        protected override RemotePlayer CreateRemoteEntity(PamelloPlayerDTO dto)
            => new RemotePlayer(dto, _client);
    }
}
