using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloSongDto : PamelloEntityDto
    {
        public string CoverUrl { get; set; }
        public int AddedById { get; set; }
        public DateTime AddedAt { get; set; }


        public IEnumerable<string> Associations { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
        public IEnumerable<int> EpisodesIds { get; set; }
        public IEnumerable<int> PlaylistsIds { get; set; }

        public int SelectedSourceIndex { get; set; }
        public IEnumerable<string> SourcesPlatformKeys { get; set; }
    }
}
