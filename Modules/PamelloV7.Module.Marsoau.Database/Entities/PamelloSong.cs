using System.Diagnostics;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Events;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloSong : PamelloEntity<DatabaseSong>, IPamelloSong
{
    private string _name;
    private string _coverUrl;
    private DateTime _addedAt;
    private IPamelloUser _addedBy;
    private bool _isSoftDeleted;
    
    public List<SongSource> _sources;
    
    public List<IPamelloUser> _favoritedBy;
    public List<IPamelloEpisode> _songEpisodes;
    public List<IPamelloPlaylist> _songPlaylists;
    public List<string> _associations;

    public override string Name {
        get => _name;
        set {
            if (_name == value) return;
            
            _name = value;
            _events.Invoke(new SongNameUpdated {
                Song = this,
                NewName = _name
            });
            
            //Save();
        }
    }

    public string CoverUrl {
        get => _coverUrl;
        set => _coverUrl = value;
    }

    public DateTime AddedAt => _addedAt;
    
    public int SelectedSourceIndex { get; set; }

    public bool IsSoftDeleted {
        get => _isSoftDeleted;
        set => _isSoftDeleted = value;
    }

    public bool IsDownloaded => false;
    public IPamelloUser? AddedBy => _addedBy;
    
    public SongSource? SelectedSource => _sources.ElementAtOrDefault(SelectedSourceIndex);
    
    public IReadOnlyList<SongSource> Sources => _sources;
    
    public IReadOnlyList<IPamelloUser> FavoriteBy => _favoritedBy;
    public IReadOnlyList<IPamelloEpisode> Episodes => _songEpisodes;
    public IReadOnlyList<IPamelloPlaylist> Playlists => _songPlaylists;
    public IReadOnlyList<string> Associations => _associations;
    
    public PamelloSong(DatabaseSong databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _coverUrl = databaseEntity.CoverUrl;
        _addedAt = databaseEntity.AddedAt;
        _associations = databaseEntity.Associations.ToList();
        _isSoftDeleted = databaseEntity.IsSoftDeleted;
        
        _sources = databaseEntity.Sources.Select(
            pk => new SongSource(services, this, pk)
        ).ToList();
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
        
        _songEpisodes.Sort((a, b) => {
            if (a.Start.TotalSeconds == b.Start.TotalSeconds) return 0;
            return a.Start.TotalSeconds > b.Start.TotalSeconds ? 1 : -1;
        });
    }

    public override void Save() {
        var songCollection = ((PamelloSongRepository)_songs).GetCollection();
        
        var databaseSong = songCollection.Get(Id);
        Debug.Assert(databaseSong is not null, "Song doesnt exist in the database for some reason, cant save song");
        
        databaseSong.Name = _name;
        databaseSong.CoverUrl = _coverUrl;

        databaseSong.Associations = _associations;
        databaseSong.IsSoftDeleted = _isSoftDeleted;

        databaseSong.Sources = _sources.Select(source => source.PK).ToList();
        
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

    public void AddSource(ISongSource source) {
        throw new NotImplementedException();
    }

    public void AddAssociation(string association) {
        var databaseAssociations = ((PamelloSongRepository)_songs)
            .GetCollection()
            .GetAll()
            .SelectMany(databaseSong => databaseSong.Associations);
        
        if (databaseAssociations.Contains(association)) return;
        
        _associations.Add(association);
        
        Save();
    }

    public void RemoveAssociation(string association) {
        if (!_associations.Contains(association)) return;
        
        _associations.Remove(association);
        
        Save();
    }

    public void MakeFavorite(IPamelloUser user, bool fromInside = false) {
        if (_favoritedBy.Contains(user)) return;
        
        _favoritedBy.Add(user);
        
        if (!fromInside) user.AddFavoriteSong(this, null, true);
        
        Save();
    }

    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false) {
        if (!_favoritedBy.Remove(user)) return;
        
        if (!fromInside) user.RemoveFavoriteSong(this, true);
        
        Save();
    }

    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip) {
        return _episodes.Add(start, name, autoSkip, this);
    }

    public IPamelloEpisode AddEpisode(IEpisodeInfo episodeInfo, bool autoSkip) {
        return AddEpisode(new AudioTime(episodeInfo.Start), episodeInfo.Name, autoSkip);
    }

    public void RemoveEpisode(IPamelloEpisode episode) {
        _episodes.Delete(episode);
    }

    public void RemoveEpisodeAt(int position) {
        var episode = _songEpisodes.ElementAtOrDefault(position);
        if (episode is null) return;
        
        _episodes.Delete(episode);
    }

    public void RemoveAllEpisodes() {
        _episodes.DeleteAllFrom(this);
    }

    public IPamelloPlaylist AddToPlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false) {
        if (!_songPlaylists.Contains(playlist)) _songPlaylists.Add(playlist);
        
        if (!fromInside) playlist.AddSong(this, position, true);

        Save();
        
        return playlist;
    }

    public void RemoveFromPlaylist(IPamelloPlaylist playlist, bool fromInside = false) {
        if (!_songPlaylists.Remove(playlist)) return;

        if (!fromInside) playlist.RemoveSong(this, fromInside);
    }

    public override IPamelloDTO GetDto() {
        return new PamelloSongDTO {
            Id = Id,
            Name = Name,
            CoverUrl = CoverUrl,
            Associations = _associations,
            AddedAt = AddedAt,
            AddedById = AddedBy?.Id ?? 0,
            
            FavoriteByIds = IPamelloEntity.GetIds(FavoriteBy),
            PlaylistsIds = IPamelloEntity.GetIds(Playlists),
            EpisodesIds = IPamelloEntity.GetIds(Episodes),
            
            SourcesPlatfromKeys = Sources.Select(source => source.PK.ToString())
        };
    }
}
