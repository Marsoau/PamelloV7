using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.WrapperOld.Model
{
    public class RemoteSong : RemoteEntity<PamelloSongDTO>
    {
        public RemoteSong(PamelloSongDTO dto, PamelloClient client) : base(dto, client) {

        }

        public string YoutubeId {
            get => _dto.YoutubeId;
        }
        public string CoverUrl {
            get => _dto.CoverUrl;
        }
        public int PlayCount {
            get => _dto.PlayCount;
        }
        public int AddedById {
            get => _dto.AddedById;
        }
        public DateTime AddedAt {
            get => _dto.AddedAt;
        }

        public IEnumerable<string> Associacions {
            get => _dto.Associations;
        }
        public IEnumerable<int> FavoriteByIds {
            get => _dto.FavoriteByIds;
        }
        public IEnumerable<int> EpisodesIds {
            get => _dto.EpisodesIds;
        }
        public IEnumerable<int> PlaylistsIds {
            get => _dto.PlaylistsIds;
        }

        public bool IsDownloading {
            get => _dto.IsDownloading;
        }
        public double DownloadProgress {
            get => _dto.DownloadProgress;
        }

        internal override void FullUpdate(PamelloSongDTO dto) {
            _dto = dto;
        }
    }
}
