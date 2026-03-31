using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities.Other;

public class PamelloQueueDto
{
    public required int CurrentSong { get; set; }
    public required int CurrentSongTimePassed { get; set; }
    public required int CurrentSongTimeTotal { get; set; }
    public required IEnumerable<PamelloQueueEntryDto> Entries { get; set; }
    public required int Position { get; set; }
    public required int NextPositionRequest { get; set; }
    public required int CurrentEpisodePosition { get; set; }
    public required bool IsRandom { get; set; }
    public required bool IsReversed { get; set; }
    public required bool IsNoLeftovers { get; set; }
    public required bool IsFeedRandom { get; set; }
}
