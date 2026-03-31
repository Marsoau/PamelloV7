using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloSongDto : PamelloEntityDto
    {
        public required string CoverUrl { get; set; }
        public required int AddedBy { get; set; }
        public required DateTime AddedAt { get; set; }

        public required IEnumerable<string> Associations { get; set; }
        public required IEnumerable<int> FavoriteBy { get; set; }
        public required IEnumerable<int> Episodes { get; set; }
        public required IEnumerable<int> Playlists { get; set; }

        public required int SelectedSourceIndex { get; set; }
        public required IEnumerable<string> SourcesPlatformKeys { get; set; }
    }
}
