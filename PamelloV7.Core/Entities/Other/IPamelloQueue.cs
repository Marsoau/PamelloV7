using PamelloV7.Core.DTO;

namespace PamelloV7.Core.Entities.Other;

public interface IPamelloQueue
{
    public IPamelloPlayer Player { get; }
    
    public bool IsRandom { get; set; }
    public bool IsReversed { get; set; }
    public bool IsNoLeftovers { get; set; }
    public bool IsFeedRandom { get; set; }
    
    public int? NextPositionRequest { get; set; }
    public int Position { get; set; }
    
    public IPamelloSong? CurrentSong { get; }
    public IPamelloEpisode? CurrentEpisode { get; }
    
    public int Count { get; }
    
    public IReadOnlyList<PamelloQueueEntry> Entries { get; }
    public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs { get; }
    public IReadOnlyList<IPamelloSong> Songs { get; }
    
    public void SetCurrent(PamelloQueueEntry? entry);

    public IPamelloSong? SongAt(int position);
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? adder);
    public IPamelloPlaylist AddPlaylist(IPamelloPlaylist playlist, IPamelloUser? adder);
    public IEnumerable<IPamelloSong> InsertSongs(string positionValue, IEnumerable<IPamelloSong> songs, IPamelloUser? adder);
    public IPamelloPlaylist InsertPlaylist(string positionValue, IPamelloPlaylist playlist, IPamelloUser? adder);
    public IPamelloSong RemoveSong(string songPositionValue);
    public bool MoveSong(string fromPositionValue, string toPositionValue);
    public bool SwapSongs(string inPositionValue, string withPositionValue);
    public IPamelloSong GoToSong(string songPosition, bool returnBack = false);
    public IPamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false);
    public void Clear();
}
