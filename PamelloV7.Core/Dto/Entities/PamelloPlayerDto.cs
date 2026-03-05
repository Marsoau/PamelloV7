using System.Text.Json.Serialization;
using PamelloV7.Core.Dto.Entities.Other;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlayerDto : PamelloEntityDto
    {
        public int OwnerId { get; set; }
        public bool IsProtected { get; set; }
        public bool IsPaused { get; set; }
        public PamelloQueueDto Queue { get; set; }
    }
}
