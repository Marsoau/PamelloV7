using System.Text.Json.Serialization;
using PamelloV7.Core.Dto.Entities.Other;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlayerDto : PamelloEntityDto
    {
        public required int Owner { get; set; }
        public required bool IsProtected { get; set; }
        public required bool IsPaused { get; set; }
        public required PamelloQueueDto? Queue { get; set; }
    }
}
