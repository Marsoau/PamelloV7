using System.Diagnostics;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Difference;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Events;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloUser : PamelloDatabaseEntity<DatabaseUser>, IPamelloUser
{
    private Guid _token;
    
    private DateTime _joinedAt;
    private int _songsPlayed;
    
    internal List<UserAuthorization> _authorizations;
    
    internal List<IPamelloSong> _addedSongs;
    internal List<IPamelloPlaylist> _addedPlaylists;
    internal List<IPamelloSong> _favoriteSongs;
    internal List<IPamelloPlaylist> _favoritePlaylists;
    
    public override string Name {
        get => SelectedAuthorization?.Info?.Name ?? $"User{Id}";
        set => throw new NotImplementedException();
    }

    public string? AvatarUrl {
        get => SelectedAuthorization?.Info?.AvatarUrl;
        set => throw new NotImplementedException();
    }

    public Guid Token => _token;
    public DateTime JoinedAt => _joinedAt;
    
    private int _selectedAuthorizationIndex;

    public int SelectedAuthorizationIndex {
        get => _selectedAuthorizationIndex;
        set {
            if (value == _selectedAuthorizationIndex) return;
            
            _selectedAuthorizationIndex = value;
            _sink.Invoke(new UserSelectedAuthorizationIndexUpdated() {
                User = this,
                SelectedAuthorizationIndex = value
            });
            
            Save();
        }
    }

    public IPamelloPlayer? PreviousPlayer { get; }
    public IPamelloPlayer? SelectedPlayer { get; set; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }

    public UserAuthorization? SelectedAuthorization => _authorizations.ElementAtOrDefault(SelectedAuthorizationIndex);
    
    public IReadOnlyList<UserAuthorization> Authorizations => _authorizations;
    
    public IReadOnlyList<IPamelloSong> AddedSongs => _addedSongs;
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists => _addedPlaylists;
    public IReadOnlyList<IPamelloSong> FavoriteSongs => _favoriteSongs;
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists => _favoritePlaylists;
    
    public PamelloUser(DatabaseUser databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _token = databaseEntity.Token;
        _joinedAt = databaseEntity.JoinedAt;

        _selectedAuthorizationIndex = databaseEntity.SelectedAuthorization;
        _authorizations = databaseEntity.Authorizations.Select(pk =>
            new UserAuthorization(services, this, pk)
        ).ToList();
        _authorizations.Sort((a, b) => string.Compare(a.PK.Platform, b.PK.Platform, StringComparison.Ordinal));
    }
    
    protected override void InitBase() {
        var databaseSongs = ((PamelloSongRepository)_songs).GetCollection().GetAll();
        var databasePlaylists = ((PamelloPlaylistRepository)_playlists).GetCollection().GetAll();

        _addedSongs = databaseSongs
            .Where(databaseSong => databaseSong.AddedBy == Id)
            .Select(databaseSong => _songs.Get(databaseSong.Id))
            .OfType<IPamelloSong>()
            .ToList();
        _addedPlaylists = databasePlaylists
            .Where(databasePlaylist => databasePlaylist.OwnerId == Id)
            .Select(databasePlaylist => _playlists.Get(databasePlaylist.Id))
            .OfType<IPamelloPlaylist>()
            .ToList();
        _favoriteSongs = _databaseEntity.FavoriteSongIds
            .Select(id => _songs.Get(id))
            .OfType<IPamelloSong>()
            .ToList();
        _favoritePlaylists = _databaseEntity.FavoritePlaylistIds
            .Select(id => _playlists.Get(id))
            .OfType<IPamelloPlaylist>()
            .ToList();
    }

    public override void SaveInternal() {
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection();
        
        var databaseUser = databaseUsers.Get(Id);
        Debug.Assert(databaseUser is not null, "User doesnt exist in the database for some reason, cant save user");

        databaseUser.FavoriteSongIds = IPamelloEntity.GetIds(FavoriteSongs).ToList();
        databaseUser.FavoritePlaylistIds = IPamelloEntity.GetIds(FavoritePlaylists).ToList();
        
        databaseUser.SelectedAuthorization = SelectedAuthorizationIndex;
        databaseUser.Authorizations = Authorizations.Select(authorization => authorization.PK).ToList();
        
        databaseUsers.Save(databaseUser);
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
    
    public void TryLoadLastPlayer() {
        throw new NotImplementedException();
    }

    public bool TrySelectPlayer(IPamelloPlayer? player) {
        throw new NotImplementedException();
    }

    public void RequireSelectPlayer(IPamelloPlayer? player) {
        throw new NotImplementedException();
    }

    public void AddAuthorization(UserAuthorization authorization) {
        throw new NotImplementedException();
    }

    public IPamelloSong? AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false) {
        if (_favoriteSongs.Contains(song)) return null;
        
        _favoriteSongs.Insert(position ?? _favoriteSongs.Count, song);

        if (!fromInside) song.MakeFavorite(this, true);
        
        _sink.Invoke(new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = FavoriteSongs
        });
        
        Save();
        
        return song;
    }

    public IPamelloSong? RemoveFavoriteSong(IPamelloSong song, bool fromInside = false) {
        if (!_favoriteSongs.Remove(song)) return null;
        
        if (!fromInside) song.UnmakeFavorite(this, true);
        
        _sink.Invoke(new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = FavoriteSongs
        });
        
        Save();
        
        return song;
    }

    public IPamelloSong? MoveFavoriteSong(int fromPosition, int toPosition) {
        var song = _favoriteSongs.ElementAtOrDefault(fromPosition);
        if (song is null) return null;
        
        _favoriteSongs.RemoveAt(fromPosition);
        _favoriteSongs.Insert(toPosition < fromPosition ? toPosition : toPosition - 1, song);

        _sink.Invoke(new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = FavoriteSongs
        });
        
        Save();
        
        return song;
    }

    public IEnumerable<IPamelloSong> ReplaceFavoriteSongs(List<IPamelloSong> newSongs) {
        var filteredSongs = new List<IPamelloSong>();
        newSongs.ForEach(song => {
            if (!filteredSongs.Contains(song)) filteredSongs.Add(song);
        });
        
        var difference = DifferenceResult<IPamelloSong>.From(_favoriteSongs, filteredSongs, (oldSong, newSong) => oldSong.Id == newSong.Id, true);
        
        foreach (var (pos, song) in difference.Deleted) song.UnmakeFavorite(this, true);
        foreach (var (pos, song) in difference.Added) song.MakeFavorite(this, true);
        
        _favoriteSongs = filteredSongs;
        
        _sink.Invoke(new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = FavoriteSongs
        });
        
        Save();
        
        return FavoriteSongs;
    }

    public IEnumerable<IPamelloSong> ClearFavoriteSongs() {
        var before = new List<IPamelloSong>(_favoriteSongs);
        foreach (var song in before) {
            song.UnmakeFavorite(this, true);
        }
        
        _favoriteSongs.Clear();
        
        _sink.Invoke(new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = FavoriteSongs
        });
        
        Save();
        
        return FavoriteSongs;
    }

    public IPamelloPlaylist? AddFavoritePlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false) {
        if (_favoritePlaylists.Contains(playlist)) return null;
        
        _favoritePlaylists.Insert(position ?? _favoriteSongs.Count, playlist);

        if (!fromInside) playlist.MakeFavorite(this, true);
        
        _sink.Invoke(new UserFavoritePlaylistsUpdated {
            User = this,
            FavoritePlaylists = FavoritePlaylists
        });
        
        Save();
        
        return playlist;
    }

    public IPamelloPlaylist? RemoveFavoritePlaylist(IPamelloPlaylist playlist, bool fromInside = false) {
        if (!_favoritePlaylists.Remove(playlist)) return null;
        
        if (!fromInside) playlist.UnmakeFavorite(this, true);
        
        _sink.Invoke(new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = FavoritePlaylists
        });
        
        Save();
        
        return playlist;
    }

    public IEnumerable<IPamelloPlaylist> ReplaceFavoritePlaylists(List<IPamelloPlaylist> newPlaylists) {
        var filteredPlaylists = new List<IPamelloPlaylist>();
        newPlaylists.ForEach(playlist => {
            if (!filteredPlaylists.Contains(playlist)) filteredPlaylists.Add(playlist);
        });
        
        var difference = DifferenceResult<IPamelloPlaylist>.From(_favoritePlaylists, filteredPlaylists, (oldSong, newSong) => oldSong.Id == newSong.Id, true);
        
        foreach (var (pos, playlist) in difference.Deleted) playlist.UnmakeFavorite(this, true);
        foreach (var (pos, playlist) in difference.Added) playlist.MakeFavorite(this, true);
        
        _favoritePlaylists = filteredPlaylists;
        
        _sink.Invoke(new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = FavoritePlaylists
        });
        
        Save();
        
        return FavoritePlaylists;
    }

    public IEnumerable<IPamelloPlaylist> ClearFavoritePlaylists() {
        var before = new List<IPamelloPlaylist>(_favoritePlaylists);
        foreach (var playlist in before) {
            playlist.UnmakeFavorite(this, true);
        }
        
        _favoritePlaylists.Clear();
        
        _sink.Invoke(new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = FavoritePlaylists
        });
        
        Save();
        
        return FavoritePlaylists;
    }

    public IPamelloPlaylist CreatePlaylist(string name) {
        return _playlists.Add(name, this);
    }

    public override IPamelloDTO GetDto() {
        return new PamelloUserDTO {
            Id = Id,
            Name = Name,
            AvatarUrl = AvatarUrl,
            SelectedPlayerId = SelectedPlayer?.Id,
            SelectedAuthorizationPos = SelectedAuthorizationIndex,

            AddedSongsIds = IPamelloEntity.GetIds(AddedSongs),
            AddedPlaylistsIds = IPamelloEntity.GetIds(AddedPlaylists),
            FavoriteSongsIds = IPamelloEntity.GetIds(FavoriteSongs),
            FavoritePlaylistsIds = IPamelloEntity.GetIds(FavoritePlaylists),

            AuthorizationsPlatfromKeys = Authorizations.Select(authorization => authorization.PK.ToString()),

            IsAdministrator = false
        };
    }
}
