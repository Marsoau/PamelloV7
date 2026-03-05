using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto.Entities.Other;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.DTO.Other;

namespace PamelloV7.Framework.Entities.Other;

public interface IPamelloQueue
{
    public IPamelloPlayer Player { get; }
    
    public bool IsRandom { get; }
    public bool IsReversed { get; }
    public bool IsNoLeftovers { get; }
    public bool IsFeedRandom { get; }
    
    void SetIsRandom(bool state, IPamelloUser? scopeUser);
    void SetIsReversed(bool state, IPamelloUser? scopeUser);
    void SetIsNoLeftovers(bool state, IPamelloUser? scopeUser);
    void SetIsFeedRandom(bool state, IPamelloUser? scopeUser);
    
    public int? NextPositionRequest { get; }
    public int Position { get; }
    
    void SetNextPositionRequest(int? position, IPamelloUser? scopeUser);
    void SetPosition(int position, IPamelloUser? scopeUser);
    
    
    public int? EpisodePosition { get; }
    
    public IPamelloSong? CurrentSong { get; }
    public IPamelloEpisode? CurrentEpisode { get; }
    
    public AudioTime CurrentSongTimePosition { get; }
    public AudioTime CurrentSongTimeTotal { get; }
    
    public int Count { get; }
    
    public IReadOnlyList<PamelloQueueEntry> Entries { get; }
    public IReadOnlyList<IPamelloSong> Songs { get; }
    
    public void SetCurrent(PamelloQueueEntry? entry, IPamelloUser? scopeUser);
    public Task RewindCurrent(AudioTime toTime, IPamelloUser? scopeUser);
    
    public int? RequestNextPosition(string? positionValue, IPamelloUser? scopeUser);

    public IPamelloSong? SongAt(int position);
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloPlaylist> AddPlaylist(IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloSong> InsertSongs(string positionValue, IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloPlaylist> InsertPlaylist(string positionValue, IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? scopeUser);
    public IPamelloSong RemoveSong(string songPositionValue, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloSong> RemoveSongsRange(string fromPositionValue, string toPositionValue, IPamelloUser? scopeUser);
    public bool MoveSong(string fromPositionValue, string toPositionValue, IPamelloUser? scopeUser);
    public bool SwapSongs(string inPositionValue, string withPositionValue, IPamelloUser? scopeUser);
    public IPamelloSong? GoToSong(string songPositionValue, IPamelloUser scopeUser, bool returnBack = false);
    public IPamelloSong? GoToNextSong(IPamelloUser scopeUser, bool forceRemoveCurrentSong = false);
    public Task<IPamelloEpisode?> GoToEpisode(string episodePositionValue, IPamelloUser? scopeUser);
    public void Clear(IPamelloUser? scopeUser);

    public PamelloQueueDto GetDto();
}
