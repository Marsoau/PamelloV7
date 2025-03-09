using Microsoft.Extensions.DependencyInjection;
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

        public async Task<TRemoteEntity?> Get(int id) {
            var remoteEntity = _loaded.FirstOrDefault(entity => entity.Id == id);
            if (remoteEntity is not null) return remoteEntity;

            return await GetNew(id);
        }

        public async Task<TRemoteEntity?> GetNew(int id, bool fullUpdateIfExists = true) {
            var dto = await GetDTO(id);
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

        public async Task<TRemoteEntity?> GetNew(string value, bool fullUpdateIfExists = true) {
            var dto = await GetDTO(value);
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

        protected abstract Task<TPamelloDTO?> GetDTO(int id);
        protected abstract Task<TPamelloDTO?> GetDTO(string value);
        protected abstract TRemoteEntity CreateRemoteEntity(TPamelloDTO dto);
    }
}
