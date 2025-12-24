using System.Diagnostics;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloPlaylist : PamelloEntity<DatabasePlaylist>, IPamelloPlaylist
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
    }

    public override void Save() {
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
    
    public IPamelloSong? AddSong(IPamelloSong song, int? position = null, bool fromInside = false) {
        _playlistSongs.Insert(position ?? _playlistSongs.Count, song);
        
        if (!fromInside) song.AddToPlaylist(this, null, true);
        
        Save();
        
        return song;
    }

    public void AddList(IReadOnlyList<IPamelloSong> list, int? position = null) {
        throw new NotImplementedException();
    }

    public IPamelloSong? MoveSong(int fromPosition, int toPosition) {
        throw new NotImplementedException();
    }

    public int RemoveSong(IPamelloSong song, bool fromInside = false) {
        var count = _playlistSongs.RemoveAll(s => s == song);
        
        if (!fromInside) song.RemoveFromPlaylist(this);
        
        Save();
        
        return count;
    }

    public IPamelloSong? RemoveAt(int position) {
        throw new NotImplementedException();
    }

    public void MakeFavorite(IPamelloUser user) {
        throw new NotImplementedException();
    }

    public void UnmakeFavorite(IPamelloUser user) {
        throw new NotImplementedException();
    }
}
