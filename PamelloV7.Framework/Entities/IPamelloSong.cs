using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Audio;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Entities;

[PamelloEntity("songs", typeof(PamelloSongDto))]
public interface IPamelloSong : IPamelloDatabaseEntity
{
    public string CoverUrl { get; }
    public DateTime AddedAt { get; }
    
    public IPamelloUser? AddedBy { get; }
    
    public int SelectedSourceIndex { get; }
    public SongSource? SelectedSource { get; }
    public IReadOnlyList<SongSource> Sources { get; }
    
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    public IReadOnlyList<IPamelloEpisode> Episodes { get; }
    public IReadOnlyList<IPamelloPlaylist> Playlists { get; }
    public IReadOnlyList<string> Associations { get; }
    
    public string SetCoverUrl(string url, IPamelloUser? scopeUser);
    
    public void SelectSource(int index, IPamelloUser? scopeUser);
    public void AddSource(SongSource source, IPamelloUser? scopeUser);
    
    public bool AddAssociation(string association, IPamelloUser? scopeUser);
    public bool RemoveAssociation(string association, IPamelloUser? scopeUser);
    public void MakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false);
    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false);
    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip, IPamelloUser scopeUser);
    public IPamelloEpisode AddEpisode(IEpisodeInfo episodeInfo, bool autoSkip, IPamelloUser scopeUser);
    public void RemoveEpisode(IPamelloEpisode episode, IPamelloUser scopeUser);
    public void RemoveEpisodeAt(int position, IPamelloUser scopeUser);
    public void RemoveAllEpisodes(IPamelloUser scopeUser);
    public IPamelloPlaylist AddToPlaylist(IPamelloPlaylist playlist, IPamelloUser scopeUser, int? position = null, bool fromInside = false);
    public void RemoveFromPlaylist(IPamelloPlaylist playlist, IPamelloUser scopeUser, bool fromInside = false);
}
