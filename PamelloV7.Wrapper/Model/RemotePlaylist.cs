using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Model
{
    public class RemotePlaylist : RemoteEntity<PamelloPlaylistDTO>
    {
        public RemotePlaylist(PamelloPlaylistDTO dto, PamelloClient client) : base(dto, client) {

        }

        public int AddedById {
            get => _dto.AddedById;
        }
        public bool IsProtected {
            get => _dto.IsProtected;
        }

        public IEnumerable<int> SongsIds {
            get => _dto.SongsIds;
        }
        public IEnumerable<int> FavoriteByIds {
            get => _dto.FavoriteByIds;
        }

        internal override void FullUpdate(PamelloPlaylistDTO dto) {

        }
    }
}
