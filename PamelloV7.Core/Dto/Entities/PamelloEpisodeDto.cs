using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloEpisodeDto : PamelloEntityDto
    {
        public int Start { get; set; }
        public bool AutoSkip { get; set; }
        public int Song { get; set; }
    }
}
