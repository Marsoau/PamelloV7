using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Model.Audio;

public interface IAudioSong : IDisposable
{
    public IPamelloSong Song { get; }
    
    public bool IsInitialized { get; }
    public bool IsEnded { get; }
    
    public AudioTime Position { get; }
    public AudioTime Duration { get; }
    
    public event Action OnEnded;

    public void Clean();
    public Task<bool> TryInitialize(CancellationToken token = default);
    public void UpdatePlaybackPoints(bool excludeCurrent = false);
    public Task RewindTo(AudioTime time, bool forceEpisodePlayback, CancellationToken token);
    public Task<IPamelloEpisode?> RewindToEpisode(int episodePosition, bool forceEpisodePlayback = true);
    public int? GetCurrentEpisodePosition();
    public IPamelloEpisode? GetCurrentEpisode();
}
