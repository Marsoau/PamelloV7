using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities.Other;

public class PamelloQueueDto
{
    public int CurrentSong { get; set; }
    public int CurrentSongTimePassed { get; set; }
    public int CurrentSongTimeTotal { get; set; }
    public IEnumerable<PamelloQueueEntryDto> Entries { get; set; }
    public int Position { get; set; }
    public int NextPositionRequest { get; set; }
    public int CurrentEpisodePosition { get; set; }
    public bool IsRandom { get; set; }
    public bool IsReversed { get; set; }
    public bool IsNoLeftovers { get; set; }
    public bool IsFeedRandom { get; set; }
}
