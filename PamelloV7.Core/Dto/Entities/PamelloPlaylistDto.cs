using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlaylistDto : PamelloEntityDto
    {
        public required int Owner { get; set; }
        public required bool IsProtected { get; set; }

        public required IEnumerable<int> Songs { get; set; }
        public required IEnumerable<int> FavoriteBy { get; set; }
    }
}
