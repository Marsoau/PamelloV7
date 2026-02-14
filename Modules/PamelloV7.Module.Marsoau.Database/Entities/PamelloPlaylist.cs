using System.Diagnostics;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloPlaylist : PamelloDatabaseEntity<DatabasePlaylist>, IPamelloPlaylist
{
    private string _name;
    private bool _isProtected;
    private IPamelloUser _owner;
    
    private List<IPamelloSong> _playlistSongs;
    private List<IPamelloUser> _favoriteBy;

    public override string Name {
        get => _name;
        set => _name = value;
    }

    public bool IsProtected {
        get => _isProtected;
        set => _isProtected = value;
    }

    public IPamelloUser Owner {
        get => _owner;
        set => _owner = value;
    }

    public IReadOnlyList<IPamelloSong> Songs => _playlistSongs;
    public IReadOnlyList<IPamelloUser> FavoriteBy => _favoriteBy;
    
    public PamelloPlaylist(DatabasePlaylist databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _isProtected = databaseEntity.IsProtected;
    }
    
    protected override void InitBase() {
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection().GetAll();
        
        _owner = _users.Get(_databaseEntity.OwnerId)!;
        
        _playlistSongs = _databaseEntity.SongIds
            .Select(id => _songs.Get(id))
            .OfType<IPamelloSong>()
            .ToList();
        _favoriteBy = databaseUsers
            .Where(databaseUser => databaseUser.FavoritePlaylistIds.Contains(Id))
            .Select(databaseUser => _users.Get(databaseUser.Id))
            .OfType<IPamelloUser>()
            .ToList();
        
        //_events.Subscribe<SongDeleted>(OnSongDeleted);
    }

    public override void SaveInternal() {
        var playlistCollection = ((PamelloPlaylistRepository)_playlists).GetCollection();
        
        var databasePlaylist = playlistCollection.Get(Id);
        Debug.Assert(databasePlaylist is not null, "Playlist doesnt exist in the database for some reason, cant save playlist");
        
        databasePlaylist.Name = _name;
        databasePlaylist.OwnerId = Owner.Id;
        
        databasePlaylist.SongIds = _playlistSongs.Select(song => song.Id).ToList();
        
        databasePlaylist.IsProtected = IsProtected;
        
        playlistCollection.Save(databasePlaylist);
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

    public void AddList(IReadOnlyList<IPamelloSong> list, int? position = null) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> newSongs, int? position = null, bool fromInside = false) {
        var songs = newSongs as List<IPamelloSong> ?? newSongs.ToList();
        
        _playlistSongs.InsertRange(position ?? _playlistSongs.Count, songs);
        
        if (!fromInside) foreach (var song in songs) {
            song.AddToPlaylist(this, null, true);
        }

        _sink.Invoke(new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = Songs
        });

        Save();
        
        return songs;
    }

    public IPamelloSong? MoveSong(int fromPosition, int toPosition) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong>? ReplaceSongs(IEnumerable<IPamelloSong> newSongs) {
        throw new NotImplementedException();
    }

    public int RemoveSong(IPamelloSong song, bool fromInside = false) {
        var count = _playlistSongs.RemoveAll(s => s == song);
        
        if (!fromInside) song.RemoveFromPlaylist(this);

        _sink.Invoke(new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = Songs
        });
        
        Save();
        
        return count;
    }

    public IPamelloSong? RemoveAt(int position) {
        throw new NotImplementedException();
    }

    public void MakeFavorite(IPamelloUser user, bool fromInside = false) {
        if (_favoriteBy.Contains(user)) return;
        
        _favoriteBy.Add(user);
        
        if (!fromInside) user.AddFavoritePlaylist(this, null, true);

        _sink.Invoke(new PlaylistFavoriteByUpdated() {
            Playlist = this,
            FavoriteBy = FavoriteBy
        });
        
        Save();
    }

    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false) {
        if (!_favoriteBy.Remove(user)) return;
        
        if (!fromInside) user.RemoveFavoritePlaylist(this, true);

        _sink.Invoke(new PlaylistFavoriteByUpdated() {
            Playlist = this,
            FavoriteBy = FavoriteBy
        });
        
        Save();
    }

    public override IPamelloDTO GetDto() {
        return new PamelloPlaylistDTO {
            Id = Id,
            Name = Name,
            OwnerId = Owner.Id,
            IsProtected = IsProtected,
            
            SongsIds = IPamelloEntity.GetIds(_playlistSongs),
            FavoriteByIds = IPamelloEntity.GetIds(_favoriteBy),
        };
    }
}
