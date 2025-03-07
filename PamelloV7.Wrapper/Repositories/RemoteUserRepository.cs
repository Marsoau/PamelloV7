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
        public RemoteUser CurrentUser { get; protected set; }

        public RemoteUserRepository(PamelloClient client) : base(client) {
            CurrentUser = null;
        }

        protected override async Task<RemoteUser?> GetNew(int id) {
            var dto = await _client.HttpGetAsync<PamelloUserDTO>($"Data/User?id={id}");
            if (dto is null) return null;

            var remoteUser = _loaded.FirstOrDefault(entity => entity.Id == id);
            if (remoteUser is not null) return remoteUser;

            remoteUser = new RemoteUser(dto, _client);
            _loaded.Add(remoteUser);

            return remoteUser;
        }

        internal async Task UpdateCurrentUser() {
            var dto = await _client.HttpGetAsync<PamelloUserDTO>($"Data/User?value={_client.UserToken}");
            if (dto is null) throw new Exception("Cant update curerent user, invalid token");

            CurrentUser = await Get(dto.Id) ?? throw new Exception($"Cant update current user, id \"{dto.Id}\" doesnt exist");
        }
    }
}
