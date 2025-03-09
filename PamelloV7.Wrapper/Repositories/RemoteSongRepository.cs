using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemoteSongRepository : RemoteRepository<RemoteSong, PamelloSongDTO>
    {
        public RemoteSongRepository(PamelloClient client) : base(client) {
        }

        protected override Task<PamelloSongDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloSongDTO>($"Data/Song?id={id}");
        protected override Task<PamelloSongDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloSongDTO>($"Data/Song?value={value}");
        protected override RemoteSong CreateRemoteEntity(PamelloSongDTO dto)
            => new RemoteSong(dto, _client);
    }
}
