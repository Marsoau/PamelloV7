using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlaylistDto : PamelloEntityDto
    {
        public int Owner { get; set; }
        public bool IsProtected { get; set; }

        public IEnumerable<int> Songs { get; set; }
        public IEnumerable<int> FavoriteBy { get; set; }
    }
}
