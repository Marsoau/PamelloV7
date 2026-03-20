using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloSongDto : PamelloEntityDto
    {
        public string CoverUrl { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedAt { get; set; }


        public IEnumerable<string> Associations { get; set; }
        public IEnumerable<int> FavoriteBy { get; set; }
        public IEnumerable<int> Episodes { get; set; }
        public IEnumerable<int> Playlists { get; set; }

        public int SelectedSourceIndex { get; set; }
        public IEnumerable<string> SourcesPlatformKeys { get; set; }
    }
}
