using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.DTO.Other;
using PamelloV7.Core.Entities;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Core.DTO
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
