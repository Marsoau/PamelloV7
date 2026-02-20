using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.DTO.Other;

namespace PamelloV7.Framework.DTO
{
    public record HistoryRecordDTO : IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("performerId")]
        public int? PerformerId { get; set; }
        
        [JsonPropertyName("nested")]
        public NestedEventDTO Nested { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
