using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloEpisodeDto : PamelloEntityDto
    {
        public required int Start { get; set; }
        public required bool AutoSkip { get; set; }
        public required int Song { get; set; }
    }
}
