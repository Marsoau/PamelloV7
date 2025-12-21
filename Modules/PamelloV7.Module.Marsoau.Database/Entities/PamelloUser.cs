using System.Diagnostics;
using PamelloV7.Core;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloUser : PamelloEntity<DatabaseUser>, IPamelloUser
{
    private Guid _token;
    
    private DateTime _joinedAt;
    private int _songsPlayed;
    
    internal List<IUserAuthorization> _authorizations;
    
    internal List<IPamelloSong> _addedSongs;
    internal List<IPamelloPlaylist> _addedPlaylists;
    internal List<IPamelloSong> _favoriteSongs;
    internal List<IPamelloPlaylist> _favoritePlaylists;
    
    public override string Name {
        get => $"User{Id}";
        set => throw new NotImplementedException();
    }

    public Guid Token => _token;
    public DateTime JoinedAt => _joinedAt;
    
    public int SelectedAuthorizationIndex { get; set; }

    public IPamelloPlayer? PreviousPlayer { get; }
    public IPamelloPlayer? SelectedPlayer { get; set; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }
    public IPamelloCommandsModule Commands { get; }

    public IUserAuthorization? SelectedAuthorization => _authorizations.ElementAtOrDefault(SelectedAuthorizationIndex);
    
    public IReadOnlyList<IUserAuthorization> Authorizations => _authorizations;
    
    public IReadOnlyList<IPamelloSong> AddedSongs => _addedSongs;
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists => _addedPlaylists;
    public IReadOnlyList<IPamelloSong> FavoriteSongs => _favoriteSongs;
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists => _favoritePlaylists;
    
    public PamelloUser(DatabaseUser databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _token = databaseEntity.Token;
        _joinedAt = databaseEntity.JoinedAt;
        
        //Commands = new PamelloCommandsModule(services, this);
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

    public void AddFavoriteSong(IPamelloSong song) {
        throw new NotImplementedException();
    }

    public void RemoveFavoriteSong(IPamelloSong song) {
        throw new NotImplementedException();
    }

    public void AddFavoritePlaylist(IPamelloPlaylist song) {
        throw new NotImplementedException();
    }

    public void RemoveFavoritePlaylist(IPamelloPlaylist song) {
        throw new NotImplementedException();
    }

    public IPamelloPlaylist CreatePlaylist(string name) {
        throw new NotImplementedException();
    }
}
