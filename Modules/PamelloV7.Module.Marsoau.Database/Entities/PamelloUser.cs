using System.Diagnostics;
using PamelloV7.Core;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Platforms;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloUser : PamelloEntity<DatabaseUser>, IPamelloUser
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
    
    public int SelectedAuthorizationIndex { get; set; }

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
        
        _authorizations = databaseEntity.Authorizations.Select(
            pk => new UserAuthorization(services, this, pk)
        ).ToList();
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

    public override void Save() {
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection();
        
        var databaseUser = databaseUsers.Get(Id);
        Debug.Assert(databaseUser is not null, "User doesnt exist in the database for some reason, cant save user");

        databaseUser.FavoriteSongIds = IPamelloEntity.GetIds(FavoriteSongs).ToList();
        databaseUser.FavoritePlaylistIds = IPamelloEntity.GetIds(FavoritePlaylists).ToList();
        
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

    public void AddAuthorization(IUserAuthorization authorization) {
        throw new NotImplementedException();
    }

    public void AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false) {
        if (_favoriteSongs.Contains(song)) return;
        
        _favoriteSongs.Insert(position ?? _favoriteSongs.Count, song);
        
        if (!fromInside) song.MakeFavorite(this, true);
        
        Save();
    }

    public void RemoveFavoriteSong(IPamelloSong song, bool fromInside = false) {
        if (!_favoriteSongs.Remove(song)) return;
        
        if (!fromInside) song.UnmakeFavorite(this, true);
        
        Save();
    }

    public void AddFavoritePlaylist(IPamelloPlaylist song, bool fromInside = false) {
        throw new NotImplementedException();
    }

    public void RemoveFavoritePlaylist(IPamelloPlaylist song, bool fromInside = false) {
        throw new NotImplementedException();
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
