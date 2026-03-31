using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto
{
    public class PamelloEntityDto
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
    }
}
