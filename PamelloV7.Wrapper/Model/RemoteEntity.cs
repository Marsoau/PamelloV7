using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Model
{
    public abstract class RemoteEntity<TDTO> where TDTO : IPamelloDTO
    {
        protected readonly PamelloClient _client;

        internal readonly TDTO _dto;

        public int Id {
            get => _dto.Id;
        }
        public string Name {
            get => _dto.Name;
        }

        public RemoteEntity(TDTO dto, PamelloClient client) {
            _client = client;

            _dto = dto;
        }
    }
}
