using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloSong : IPamelloDatabaseEntity
{
    public string YoutubeId { get; }
    public string CoverUrl { get; }
    public int PlayCount { get; set; }
    public DateTime AddedAt { get; }
    public bool IsDownloaded { get; }
    
    public IPamelloUser? AddedBy { get; }
    
    public IReadOnlyList<IPamelloUser> FavoritedBy { get; }
    public IReadOnlyList<IPamelloEpisode> Episodes { get; }
    public IReadOnlyList<IPamelloPlaylist> Playlists { get; }
    public IReadOnlyList<string> Associations { get; }
    public IReadOnlyList<string> Sources { get; }
    
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
