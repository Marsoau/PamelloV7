using System.Diagnostics;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Server.Entities.Base;
using PamelloV7.Server.Repositories.Database;

namespace PamelloV7.Server.Entities;

public class PamelloSong : PamelloEntity<DatabaseSong>, IPamelloSong
{
    private string _name;
    private string _coverUrl;
    private int _playCount;
    private DateTime _addedAt;
    private IPamelloUser _addedBy;
    
    private List<IPamelloUser> _favoritedBy;
    private List<IPamelloEpisode> _songEpisodes;
    private List<IPamelloPlaylist> _songPlaylists;
    private List<string> _associations;
    private List<string> _sources;

    public override string Name {
        get => _name;
        set => _name = value;
    }

    public string YoutubeId => "noytid";
    public string CoverUrl => _coverUrl;

    public int PlayCount {
        get => _playCount;
        set => _playCount = value;
    }
    public DateTime AddedAt => _addedAt;

    public bool IsDownloaded => false;
    public IPamelloUser? AddedBy => _addedBy;
    public IReadOnlyList<IPamelloUser> FavoritedBy => _favoritedBy;
    public IReadOnlyList<IPamelloEpisode> Episodes => _songEpisodes;
    public IReadOnlyList<IPamelloPlaylist> Playlists => _songPlaylists;
    public IReadOnlyList<string> Associations => _associations;
    public IReadOnlyList<string> Sources => _sources;
    
    public PamelloSong(DatabaseSong databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _coverUrl = databaseEntity.CoverUrl;
        _playCount = databaseEntity.PlayCount;
        _addedAt = databaseEntity.AddedAt;
        _associations = databaseEntity.Associations.ToList();
        _sources = databaseEntity.Sources.ToList();
    }
    
    protected override void InitBase() {
        var databasePlaylists = ((PamelloPlaylistRepository)_playlists).GetCollection().GetAll();
        var databaseEpisodes = ((PamelloEpisodeRepository)_episodes).GetCollection().GetAll();
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection().GetAll();
        
        _addedBy = _users.Get(_databaseEntity.AddedBy)!;
        
        _favoritedBy = databaseUsers
            .Where(databaseUser => databaseUser.FavoriteSongIds.Contains(Id))
            .Select(databaseUser => _users.Get(databaseUser.Id))
            .OfType<IPamelloUser>()
            .ToList();
        _songEpisodes = databaseEpisodes.
            Where(databaseEpisode => databaseEpisode.SongId == Id)
            .Select(databaseEpisode => _episodes.Get(databaseEpisode.Id))
            .OfType<IPamelloEpisode>()
            .ToList();
        _songPlaylists = databasePlaylists.
            Where(databasePlaylist => databasePlaylist.SongIds.Contains(Id))
            .Select(databasePlaylist => _playlists.Get(databasePlaylist.Id))
            .OfType<IPamelloPlaylist>()
            .ToList();
    }

    public override void Save() {
        var songCollection = ((PamelloSongRepository)_songs).GetCollection();
        
        var databaseSong = songCollection.Get(Id);
        Debug.Assert(databaseSong is not null, "Song doesnt exist in the database for some reason, cant save song");
        
        databaseSong.Name = _name;
        databaseSong.CoverUrl = _coverUrl;
        databaseSong.PlayCount = _playCount;

        databaseSong.Associations = _associations;
        
        songCollection.Save(databaseSong);
    }
    
    /*
    *
    *
    *
    *
    *
    * 
    *
    *
    *
    * 
    */
    
    public void AddAssociation(string association) {
        throw new NotImplementedException();
    }

    public void RemoveAssociation(string association) {
        throw new NotImplementedException();
    }

    public void MakeFavorite(IPamelloUser user) {
        throw new NotImplementedException();
    }

    public void UnmakeFavorite(IPamelloUser user) {
        throw new NotImplementedException();
    }

    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip) {
        throw new NotImplementedException();
    }

    public void RemoveEpisode(IPamelloEpisode episode) {
        throw new NotImplementedException();
    }

    public void RemoveEpisodeAt(int position) {
        throw new NotImplementedException();
    }

    public void RemoveAllEpisodes() {
        throw new NotImplementedException();
    }

    public void AddToPlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false) {
        throw new NotImplementedException();
    }

    public void RemoveFromPlaylist(IPamelloPlaylist playlist, bool fromInside = false) {
        throw new NotImplementedException();
    }
}
