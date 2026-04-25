using System.Diagnostics;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Difference;
using PamelloV7.Framework.Dto;
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

    public bool SetIsProtected(bool state, IPamelloUser? scopeUser) {
        if (_isProtected == state) return _isProtected;
        
        _isProtected = state;
        
        _sink.Invoke(scopeUser, new PlaylistIsProtectedUpdated() {
            Playlist = this,
            IsProtected = _isProtected
        });
        
        Save();
        
        return _isProtected;
    }

    public IPamelloUser Owner => _owner;

    public IPamelloUser TransferOwnership(IPamelloUser newOwner, IPamelloUser? scopeUser) {
        if (Owner == newOwner) return Owner;
        
        _owner = newOwner;
        
        _sink.Invoke(scopeUser, new PlaylistOwnerUpdated() {
            Playlist = this,
            Owner = _owner.Id
        });
        
        Save();
        
        return _owner;
    }

    public IReadOnlyList<IPamelloSong> Songs => _playlistSongs;
    public IReadOnlyList<IPamelloUser> FavoriteBy => _favoriteBy;
    
    public DateTime AddedAt { get; }

    public PamelloPlaylist(DatabasePlaylist databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _isProtected = databaseEntity.IsProtected;
        AddedAt = databaseEntity.AddedAt;
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

    public IPamelloSong? MoveSong(string fromPosition, string toPosition, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public IPamelloSong? MoveSong(int fromPosition, int toPosition, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> ReplaceSongs(IEnumerable<IPamelloSong> newSongs, IPamelloUser? scopeUser) {
        var newSongsList = newSongs.ToList();
        
        var difference = DifferenceResult<IPamelloSong>.From(Songs, newSongsList, (oldSong, newSong) => oldSong.Id == newSong.Id, true);
        
        foreach (var (_, song) in difference.Deleted) song.RemoveFromPlaylist(this, scopeUser, true);
        foreach (var (_, song) in difference.Added) song.AddToPlaylist(this, scopeUser, -1, true);
        
        _playlistSongs = newSongsList;
        
        _sink.Invoke(scopeUser, new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = IPamelloEntity.GetIds(_playlistSongs)
        });
        
        Save();
        
        return newSongsList;
    }

    public int RemoveSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser, bool fromInside = false) {
        var songsToRemove = songs.Distinct().ToList();
        var count = _playlistSongs.RemoveAll(song => songsToRemove.Contains(song));

        if (!fromInside) {
            foreach (var song in songsToRemove) {
                song.RemoveFromPlaylist(this, scopeUser, true);
            }
        }

        _sink.Invoke(scopeUser, new PlaylistSongsUpdated() {
            Playlist = this,
            Songs = IPamelloEntity.GetIds(_playlistSongs)
        });
        
        Save();
        
        return count;
    }

    public IPamelloSong? RemoveAt(string position, IPamelloUser? scopeUser) {
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
