using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.WrapperOld.Model
{
    public class RemoteEpisode : RemoteEntity<PamelloEpisodeDTO>
    {
        public RemoteEpisode(PamelloEpisodeDTO dto, PamelloClient client) : base(dto, client) {

        }

        public int Start {
            get => _dto.Start;
        }
        public bool Skip {
            get => _dto.Skip;
        }
        public int SongId {
            get => _dto.SongId;
        }
        public Task<RemoteSong?> Song {
            get => _client.Songs.Get(SongId);
        }

        internal override void FullUpdate(PamelloEpisodeDTO dto) {
            _dto = dto;
        }
    }
}
