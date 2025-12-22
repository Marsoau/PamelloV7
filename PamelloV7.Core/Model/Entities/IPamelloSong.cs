using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

[ValueEntity("songs")]
public interface IPamelloSong : IPamelloDatabaseEntity
{
    public string CoverUrl { get; }
    public DateTime AddedAt { get; }
    
    public IPamelloUser? AddedBy { get; }
    
    public bool IsSoftDeleted { get; }
    
    public int SelectedSourceIndex { get; set; }
    public ISongSource? SelectedSource { get; }
    public IReadOnlyList<ISongSource> Sources { get; }
    
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    public IReadOnlyList<IPamelloEpisode> Episodes { get; }
    public IReadOnlyList<IPamelloPlaylist> Playlists { get; }
    public IReadOnlyList<string> Associations { get; }
    
    public void AddSource(ISongSource source);
    
    public void AddAssociation(string association);
    public void RemoveAssociation(string association);
    public void MakeFavorite(IPamelloUser user);
    public void UnmakeFavorite(IPamelloUser user);
    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip);
    public void RemoveEpisode(IPamelloEpisode episode);
    public void RemoveEpisodeAt(int position);
    public void RemoveAllEpisodes();
    public void AddToPlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false);
    public void RemoveFromPlaylist(IPamelloPlaylist playlist, bool fromInside = false);
}
