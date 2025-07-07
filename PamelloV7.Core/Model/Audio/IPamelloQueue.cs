using PamelloV7.Core.DTO;
using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Model.Audio;

public interface IPamelloQueue
{
    public IPamelloPlayer Player { get; }
    
    public bool IsRandom { get; set; }
    public bool IsReversed { get; set; }
    public bool IsNoLeftovers { get; set; }
    public bool IsFeedRandom { get; set; }
    
    public int? NextPositionRequest { get; set; }
    public int Position { get; set; }
    
    public IAudioSong Audio { get; }
    
    public int Count { get; }
    
    public IReadOnlyList<PamelloQueueEntry> Entries { get; }
    public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs { get; }
    public IReadOnlyList<IPamelloSong> Songs { get; }
    
    public void SetCurrent(PamelloQueueEntry? entry);

    public IPamelloSong? SongAt(int position);
    public IPamelloSong AddSong(IPamelloSong song, IPamelloUser? adder);
    public IPamelloPlaylist AddPlaylist(IPamelloPlaylist playlist, IPamelloUser? adder);
    public IPamelloSong InsertSong(int position, IPamelloSong song, IPamelloUser? adder);
    public IPamelloPlaylist InsertPlaylist(int position, IPamelloPlaylist playlist, IPamelloUser? adder);
    public IPamelloSong RemoveSong(int songPosition);
    public bool MoveSong(int fromPosition, int toPosition);
    public bool SwapSongs(int inPosition, int withPosition);
    public IPamelloSong GoToSong(int songPosition, bool returnBack = false);
    public IPamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false);
    public void Clear();
}
