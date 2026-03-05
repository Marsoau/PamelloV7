using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlaylistDto : PamelloEntityDto
    {
        public int OwnerId { get; set; }
        public bool IsProtected { get; set; }

        public IEnumerable<int> SongsIds { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}
