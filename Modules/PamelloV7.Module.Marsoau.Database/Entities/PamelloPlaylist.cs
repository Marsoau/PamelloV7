using System.Diagnostics;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloPlaylist : PamelloDatabaseEntity<DatabasePlaylist>, IPamelloPlaylist
{
    private string _name;
    private bool _isProtected;
    private IPamelloUser _owner = null!;
    
    private List<IPamelloSong> _playlistSongs = null!;
    private List<IPamelloUser> _favoriteBy = null!;

    public override string Name => _name;

    public override bool IsDeleted { get; set; }

    public override string SetName(string name, IPamelloUser scopeUser) {
        if (_name == name) return _name;

        _name = name;
        _sink.Invoke(scopeUser, new PlaylistNameUpdated() {
            Playlist = this,
            Name = _name
        });

        Save();
        
        return _name;
    }

    public bool IsProtected => _isProtected;

    public IPamelloUser Owner => _owner;

    public IReadOnlyList<IPamelloSong> Songs => _playlistSongs;
    public IReadOnlyList<IPamelloUser> FavoriteBy => _favoriteBy;
    
    public PamelloPlaylist(DatabasePlaylist databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _isProtected = databaseEntity.IsProtected;
    }
    
    protected override void InitBase() {
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection().GetAll();
        
        _owner = _users.Get(DatabaseEntity.OwnerId)!;
        
        _playlistSongs = DatabaseEntity.SongIds
            .Select(id => _songs.Get(id))
            .OfType<IPamelloSong>()
            .ToList();
        _favoriteBy = databaseUsers
            .Where(databaseUser => databaseUser.FavoritePlaylistIds.Contains(Id))
            .Select(databaseUser => _users.Get(databaseUser.Id))
            .OfType<IPamelloUser>()
            .ToList();
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

    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> newSongs, IPamelloUser? scopeUser,
        int? position = null, bool fromInside = false) {
        var songs = newSongs as List<IPamelloSong> ?? newSongs.ToList();
        
        _playlistSongs.InsertRange(position ?? _playlistSongs.Count, songs);
        
        if (!fromInside) foreach (var song in songs) {
            song.AddToPlaylist(this, scopeUser, position, true);
        }

        _sink.Invoke(scopeUser, new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = IPamelloEntity.GetIds(songs)
        });

        Save();
        
        return songs;
    }

    public IPamelloSong? MoveSong(int fromPosition, int toPosition, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> ReplaceSongs(IEnumerable<IPamelloSong> newSongs, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public int RemoveSong(IPamelloSong song, IPamelloUser? scopeUser, bool fromInside = false) {
        var count = _playlistSongs.RemoveAll(s => s == song);
        
        if (!fromInside) song.RemoveFromPlaylist(this, scopeUser);

        _sink.Invoke(scopeUser, new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = IPamelloEntity.GetIds(_playlistSongs)
        });
        
        Save();
        
        return count;
    }

    public IPamelloSong? RemoveAt(int position, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public void MakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false) {
        if (_favoriteBy.Contains(user)) return;
        
        _favoriteBy.Add(user);
        
        if (!fromInside) user.AddFavoritePlaylist(this, null, true, automatic);

        _sink.Invoke(automatic ? null : user, new PlaylistFavoriteByUpdated() {
            Playlist = this,
            FavoriteBy = IPamelloEntity.GetIds(_favoriteBy)
        });
        
        Save();
    }

    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false) {
        if (!_favoriteBy.Remove(user)) return;
        
        if (!fromInside) user.RemoveFavoritePlaylist(this, true, automatic);

        _sink.Invoke(automatic ? null : user, new PlaylistFavoriteByUpdated() {
            Playlist = this,
            FavoriteBy = IPamelloEntity.GetIds(_favoriteBy)
        });
        
        Save();
    }

    public override PamelloEntityDto GetDto() {
        return new PamelloPlaylistDto {
            Id = Id,
            Name = Name,
            Owner = Owner.Id,
            IsProtected = IsProtected,
            
            Songs = IPamelloEntity.GetIds(_playlistSongs),
            FavoriteBy = IPamelloEntity.GetIds(_favoriteBy),
        };
    }
}
