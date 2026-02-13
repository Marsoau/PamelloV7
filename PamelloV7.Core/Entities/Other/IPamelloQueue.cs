using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Other;

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
    public int? EpisodePosition { get; }
    
    public IPamelloSong? CurrentSong { get; }
    public IPamelloEpisode? CurrentEpisode { get; }
    
    public AudioTime CurrentSongTimePosition { get; }
    public AudioTime CurrentSongTimeTotal { get; }
    
    public int Count { get; }
    
    public IReadOnlyList<PamelloQueueEntry> Entries { get; }
    public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs { get; }
    public IReadOnlyList<IPamelloSong> Songs { get; }
    
    public void SetCurrent(PamelloQueueEntry? entry);
    public Task RewindCurrent(AudioTime toTime);
    
    public int? RequestNextPosition(string? positionValue);

    public IPamelloSong? SongAt(int position);
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? adder);
    public IEnumerable<IPamelloPlaylist> AddPlaylist(IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? adder);
    public IEnumerable<IPamelloSong> InsertSongs(string positionValue, IEnumerable<IPamelloSong> songs, IPamelloUser? adder);
    public IEnumerable<IPamelloPlaylist> InsertPlaylist(string positionValue, IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? adder);
    public IPamelloSong RemoveSong(string songPositionValue);
    public bool MoveSong(string fromPositionValue, string toPositionValue);
    public bool SwapSongs(string inPositionValue, string withPositionValue);
    public IPamelloSong? GoToSong(string songPositionValue, bool returnBack = false);
    public IPamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false);
    public Task<IPamelloEpisode?> GoToEpisode(string episodePositionValue);
    public void Clear();

    public PamelloQueueDTO GetDto();
}
