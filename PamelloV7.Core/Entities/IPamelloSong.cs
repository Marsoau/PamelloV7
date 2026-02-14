using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Core.Entities;

[ValueEntity("songs")]
public interface IPamelloSong : IPamelloDatabaseEntity
{
    public string CoverUrl { get; set; }
    public DateTime AddedAt { get; }
    
    public IPamelloUser? AddedBy { get; }
    
    public int SelectedSourceIndex { get; set; }
    public SongSource? SelectedSource { get; }
    public IReadOnlyList<SongSource> Sources { get; }
    
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    public IReadOnlyList<IPamelloEpisode> Episodes { get; }
    public IReadOnlyList<IPamelloPlaylist> Playlists { get; }
    public IReadOnlyList<string> Associations { get; }
    
    public void AddSource(SongSource source);
    
    public bool AddAssociation(string association);
    public bool RemoveAssociation(string association);
    public void MakeFavorite(IPamelloUser user, bool fromInside = false);
    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false);
    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip);
    public IPamelloEpisode AddEpisode(IEpisodeInfo episodeInfo, bool autoSkip);
    public void RemoveEpisode(IPamelloEpisode episode, IPamelloUser scopeUser);
    public void RemoveEpisodeAt(int position, IPamelloUser scopeUser);
    public void RemoveAllEpisodes();
    public IPamelloPlaylist AddToPlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false);
    public void RemoveFromPlaylist(IPamelloPlaylist playlist, bool fromInside = false);
}
