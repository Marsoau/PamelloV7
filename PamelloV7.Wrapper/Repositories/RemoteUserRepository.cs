using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemoteUserRepository : RemoteRepository<RemoteUser, PamelloUserDTO>
    {
        public RemoteUser Current { get; protected set; }

        public RemoteUserRepository(PamelloClient client) : base(client) {
            Current = null;
        }

        internal async Task UpdateCurrentUser() {
            var dto = await GetDTO(_client.UserToken?.ToString() ?? "");
            if (dto is null) throw new Exception("Cant update curerent user, invalid token");

            Current = await Get(dto.Id) ?? throw new Exception($"Cant update current user, id \"{dto.Id}\" doesnt exist");
        }

        protected override Task<PamelloUserDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User?id={id}");
        protected override Task<PamelloUserDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User?value={value}");
        protected override RemoteUser CreateRemoteEntity(PamelloUserDTO dto)
            => new RemoteUser(dto, _client);
    }
}
