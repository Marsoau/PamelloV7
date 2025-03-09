using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemotePlaylistRepository : RemoteRepository<RemotePlaylist, PamelloPlaylistDTO>
    {
        public RemotePlaylistRepository(PamelloClient client) : base(client) {
        }

        protected override Task<PamelloPlaylistDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloPlaylistDTO>($"Data/Playlist?id={id}");
        protected override Task<PamelloPlaylistDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloPlaylistDTO>($"Data/Playlist?value={value}");
        protected override RemotePlaylist CreateRemoteEntity(PamelloPlaylistDTO dto)
            => new RemotePlaylist(dto, _client);
    }
}
