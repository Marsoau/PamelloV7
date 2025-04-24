using PamelloV7.Core.DTO;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public abstract class RemoteRepository<TRemoteEntity, TPamelloDTO>
        where TRemoteEntity : RemoteEntity<TPamelloDTO>
        where TPamelloDTO : IPamelloDTO
    {
        protected readonly PamelloClient _client;

        public readonly List<TRemoteEntity> _loaded;

        protected abstract string ControllerName { get; }

        public RemoteRepository(PamelloClient client) {
            _client = client;

            _loaded = new List<TRemoteEntity>();
        }

        public async Task<TRemoteEntity> GetRequired(int id)
            => await Get(id) ?? throw new PamelloException($"Cant get user by id \"{id}\"");
        public async Task<TRemoteEntity> GetNewRequired(int id)
            => await GetNew(id) ?? throw new PamelloException($"Cant get new user by id \"{id}\"");
        public async Task<TRemoteEntity> GetNewRequired(string value)
            => await GetNew(value) ?? throw new PamelloException($"Cant get new user by value \"{value}\"");

        public async Task<TRemoteEntity?> Get(int id, bool requestNewIfNotFound = true) {
            var remoteEntity = _loaded.FirstOrDefault(entity => entity.Id == id);
            if (remoteEntity is not null) return remoteEntity;

            if (!requestNewIfNotFound) return null;

            return await GetNew(id);
        }

        public async Task<TRemoteEntity?> GetNew(int id, bool fullUpdateIfExists = true) {
            try {
                return GetFromDTO(await GetDTO(id), fullUpdateIfExists);
            }
            catch {
                return null;
            }
        }

        public async Task<TRemoteEntity?> GetNew(string value, bool fullUpdateIfExists = true) {
            try {
                return GetFromDTO(await GetDTO(value), fullUpdateIfExists);
            }
            catch {
                return null;
            }
        }

        protected TRemoteEntity? GetFromDTO(TPamelloDTO? dto, bool fullUpdateIfExists = true) {
            if (dto is null) return null;

            var remoteEntity = _loaded.FirstOrDefault(entity => entity.Id == dto.Id);
            if (remoteEntity is not null) {
                if (fullUpdateIfExists) remoteEntity.FullUpdate(dto);
                return remoteEntity;
            }

            remoteEntity = CreateRemoteEntity(dto);
            _loaded.Add(remoteEntity);

            return remoteEntity;
        }

        public async Task<IEnumerable<int>> Search(string querry = "") {
            return await GetSearch(querry, []);
        }
        protected async Task<IEnumerable<int>> GetSearch(string querry, Dictionary<string, string> atributes) {
            var sb = new StringBuilder();

            sb.Append($"Data/Search/{ControllerName}s/{querry}");

            var first = true;
            foreach (var attribute in atributes) {
                if (first) sb.Append("?");
                else sb.Append("&");

                sb.Append(attribute.Key);
                sb.Append("=");
                sb.Append(attribute.Value);
            }

            return await _client.HttpGetAsync<IEnumerable<int>>(sb.ToString()) ?? [];
        }

        protected abstract Task<TPamelloDTO?> GetDTO(int id);
        protected abstract Task<TPamelloDTO?> GetDTO(string value);
        protected abstract TRemoteEntity CreateRemoteEntity(TPamelloDTO dto);

        internal void Cleanup() {
            _loaded.Clear();
        }
    }
}
