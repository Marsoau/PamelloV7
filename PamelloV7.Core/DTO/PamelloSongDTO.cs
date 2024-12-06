using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloSongDTO : IPamelloDTO
    {
        public int Id { get; set; }
        public string YoutubeId { get; set; }
        public string Name { get; set; }
        public string CoverUrl { get; set; }
        public int PlayCount { get; set; }
        public int AddedById { get; set; }
        public DateTime AddedAt { get; set; }

        public IEnumerable<string> Associacions { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
        public IEnumerable<int> EpisodesIds { get; set; }
        public IEnumerable<int> PlaylistsIds { get; set; }
    }
}
