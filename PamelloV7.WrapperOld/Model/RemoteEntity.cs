using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.WrapperOld.Interfaces;

namespace PamelloV7.WrapperOld.Model
{
    public abstract class RemoteEntity<TPamelloDTO> : IRemoteEntity where TPamelloDTO : IPamelloDTO
    {
        protected readonly PamelloClient _client;

        internal TPamelloDTO _dto;

        public int Id {
            get => _dto.Id;
        }
        public string Name {
            get => _dto.Name;
        }

        public RemoteEntity(TPamelloDTO dto, PamelloClient client) {
            _client = client;

            _dto = dto;
        }

        internal abstract void FullUpdate(TPamelloDTO dto);
    }
}
