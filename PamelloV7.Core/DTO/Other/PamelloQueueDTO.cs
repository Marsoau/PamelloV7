using System.Text.Json.Serialization;

namespace PamelloV7.Core.DTO.Other;

public class PamelloQueueDTO
{
    [JsonPropertyName("currentSongId")]
    public int? CurrentSongId { get; set; }
    
    [JsonPropertyName("currentSongTinePassed")]
    public int CurrentSongTimePassed { get; set; }

    [JsonPropertyName("currentSongTimeTotal")]
    public int CurrentSongTimeTotal { get; set; }
    
    [JsonPropertyName("entriesDTOs")]
    public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs { get; set; }
    
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("nextPositionRequest")]
    public int? NextPositionRequest { get; set; }
    
    [JsonPropertyName("currentEpisodePosition")]
    public int? CurrentEpisodePosition { get; set; }
    
    [JsonPropertyName("isRandom")]
    public bool IsRandom { get; set; }

    [JsonPropertyName("isReversed")]
    public bool IsReversed { get; set; }

    [JsonPropertyName("isNoLeftovers")]
    public bool IsNoLeftovers { get; set; }

    [JsonPropertyName("isFeedRandom")]
    public bool IsFeedRandom { get; set; }
}
