using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto.Other;

public class RemoteQueueDto
{
    public required SafeStoredEntity<RemoteSong> CurrentSong { get; set; }
    public required int CurrentSongTimePassed { get; set; }
    public required int CurrentSongTimeTotal { get; set; }
    public required IEnumerable<RemoteQueueEntryDto> Entries { get; set; }
    public required int Position { get; set; }
    public required int? NextPositionRequest { get; set; }
    public required int? CurrentEpisodePosition { get; set; }
    public required bool IsRandom { get; set; }
    public required bool IsReversed { get; set; }
    public required bool IsNoLeftovers { get; set; }
    public required bool IsFeedRandom { get; set; }
}
