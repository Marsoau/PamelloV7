using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Difference;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloUser : PamelloDatabaseEntity<DatabaseUser>, IPamelloUser
{
    private readonly IPamelloCommandsService _commands;
    
    private Guid _token;
    
    private DateTime _joinedAt;
    
    internal List<UserAuthorization> _authorizations;
    
    internal List<IPamelloSong> _addedSongs = null!;
    internal List<IPamelloPlaylist> _addedPlaylists = null!;
    internal List<IPamelloSong> _favoriteSongs = null!;
    internal List<IPamelloPlaylist> _favoritePlaylists = null!;
    
    public override string Name => SelectedAuthorization?.Info?.Name ?? $"User{Id}";
    public override string SetName(string name, IPamelloUser scopeUser) {
        throw new PamelloException("Cannot set name of a user, its determined by the authorization");
    }

    public override bool IsDeleted { get; set; }

    public string? AvatarUrl {
        get => SelectedAuthorization?.Info?.AvatarUrl;
    }

    public Guid Token => _token;
    public DateTime JoinedAt => _joinedAt;
    
    public int SelectedAuthorizationIndex { get; set; }
    
    public IPamelloPlayer? SelectedPlayer { get; private set; }

    public IPamelloPlayer RequiredSelectedPlayer
        => SelectedPlayer ?? throw new PamelloException("Selected player required");

    public IPamelloPlayer GuaranteedSelectedPlayer
        => SelectedPlayer ?? _commands.Get<PlayerCreate>(this).Execute("Player");

    public UserAuthorization? SelectedAuthorization => _authorizations.ElementAtOrDefault(SelectedAuthorizationIndex);
    
    public IReadOnlyList<UserAuthorization> Authorizations => _authorizations;
    
    public IReadOnlyList<IPamelloSong> AddedSongs => _addedSongs;
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists => _addedPlaylists;
    public IReadOnlyList<IPamelloSong> FavoriteSongs => _favoriteSongs;
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists => _favoritePlaylists;
    
    public PamelloUser(DatabaseUser databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _commands = services.GetRequiredService<IPamelloCommandsService>();
        
        _token = databaseEntity.Token;
        _joinedAt = databaseEntity.JoinedAt;

        SelectedAuthorizationIndex = databaseEntity.SelectedAuthorization;
        _authorizations = databaseEntity.Authorizations.Select(pk =>
            new UserAuthorization(services, this, pk)
        ).ToList();
        if (Id == 3) {
            //_authorizations.Add(new UserAuthorization(services, this, new PlatformKey("osu", "29347119")));
        }
        
        SortAuthorizations(true);
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
        _favoriteSongs = DatabaseEntity.FavoriteSongIds
            .Select(id => _songs.Get(id))
            .OfType<IPamelloSong>()
            .ToList();
        _favoritePlaylists = DatabaseEntity.FavoritePlaylistIds
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

    public void SortAuthorizations(bool onInit = false) {
        var before = _authorizations.ToList();
        
        _authorizations.Sort((a, b) => string.Compare(a.PK.ToString(), b.PK.ToString(), StringComparison.Ordinal));
        
        if (onInit || _authorizations.SequenceEqual(before)) return;
        
        _sink.Invoke(null, new UserAuthorizationsUpdated() {
            User = this,
            AuthorizationsPlatformKeys = Authorizations.Select(a => a.PK.ToString())
        });
        
        Save();
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

    public IPamelloPlayer? SelectPlayer(IPamelloPlayer? player, bool autoSelected = false) {
        if (SelectedPlayer == player) return SelectedPlayer;
        if (!player?.IsAvailableFor(this) ?? false) throw new PamelloException("That player is not available for this user");
        
        SelectedPlayer = player;

        _sink.Invoke(autoSelected ? null : this, new UserSelectedPlayerUpdated() {
            User = this,
            SelectedPlayer = SelectedPlayer?.Id ?? 0
        });
        
        return SelectedPlayer;
    }

    public bool TrySelectPlayer(IPamelloPlayer? player) {
        throw new NotImplementedException();
    }

    public void RequireSelectPlayer(IPamelloPlayer? player) {
        throw new NotImplementedException();
    }

    public void SelectAuthorization(int value, bool autoSelected = false) {
        if (SelectedAuthorizationIndex == value) return;

        SelectedAuthorizationIndex = value;
        _sink.Invoke(autoSelected ? null : this, new UserSelectedAuthorizationIndexUpdated() {
            User = this,
            SelectedAuthorizationIndex = SelectedAuthorizationIndex
        });

        Save();
    }

    public async Task AddAuthorization(string platform, string key) {
        var platforms = _services.GetRequiredService<IPlatformService>();
        
        var userPlatform = platforms.GetUserPlatform(platform);
        if (userPlatform is null) throw new PamelloException($"Platform \"{userPlatform}\" not found");
        
        var userInfo = await userPlatform.GetUserInfo(key);
        if (userInfo is null) throw new PamelloException($"User with key \"{key}\" not found on \"{platform}\" platform");
        
        AddAuthorizationForced(platform, key);
    }

    public void AddAuthorizationForced(string platform, string key) {
        var authorization = new UserAuthorization(_services, this, new PlatformKey(platform, key));
        
        _authorizations.Add(authorization);
        SortAuthorizations(true);
        
        _sink.Invoke(this, new UserAuthorizationsUpdated() {
            User = this,
            AuthorizationsPlatformKeys = Authorizations.Select(a => a.PK.ToString())
        });
        
        Save();
    }

    public IPamelloSong? AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false, bool automatic = false) {
        if (_favoriteSongs.Contains(song)) return null;
        
        _favoriteSongs.Insert(position ?? _favoriteSongs.Count, song);

        if (!fromInside) song.MakeFavorite(this, true, automatic);
        
        _sink.Invoke(this, new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs)
        });
        
        Save();
        
        return song;
    }

    public IPamelloSong? RemoveFavoriteSong(IPamelloSong song, bool fromInside = false, bool automatic = false) {
        if (!_favoriteSongs.Remove(song)) return null;
        
        if (!fromInside) song.UnmakeFavorite(this, true, automatic);
        
        _sink.Invoke(this, new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs)
        });
        
        Save();
        
        return song;
    }

    public IPamelloSong? MoveFavoriteSong(int fromPosition, int toPosition, bool autoMoved) {
        var song = _favoriteSongs.ElementAtOrDefault(fromPosition);
        if (song is null) return null;
        
        _favoriteSongs.RemoveAt(fromPosition);
        _favoriteSongs.Insert(toPosition < fromPosition ? toPosition : toPosition - 1, song);

        _sink.Invoke(this, new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs)
        });
        
        Save();
        
        return song;
    }

    public IEnumerable<IPamelloSong> ReplaceFavoriteSongs(List<IPamelloSong> newSongs) {
        var filteredSongs = newSongs.Distinct().ToList();
        
        var difference = DifferenceResult<IPamelloSong>.From(_favoriteSongs, filteredSongs, (oldSong, newSong) => oldSong.Id == newSong.Id, true);
        
        foreach (var (_, song) in difference.Deleted) song.UnmakeFavorite(this, true);
        foreach (var (_, song) in difference.Added) song.MakeFavorite(this, true);
        
        _favoriteSongs = filteredSongs;
        
        _sink.Invoke(this, new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs)
        });
        
        Save();
        
        return FavoriteSongs;
    }

    public IEnumerable<IPamelloSong> ClearFavoriteSongs(bool automatic = false) {
        var before = new List<IPamelloSong>(_favoriteSongs);
        foreach (var song in before) {
            song.UnmakeFavorite(this, true);
        }
        
        _favoriteSongs.Clear();
        
        _sink.Invoke(automatic ? null : this, new UserFavoriteSongsUpdated() {
            User = this,
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs)
        });
        
        Save();
        
        return FavoriteSongs;
    }

    public IPamelloPlaylist? AddFavoritePlaylist(IPamelloPlaylist playlist, int? position = null, bool fromInside = false, bool automatic = false) {
        if (_favoritePlaylists.Contains(playlist)) return null;
        
        _favoritePlaylists.Insert(position ?? _favoriteSongs.Count, playlist);

        if (!fromInside) playlist.MakeFavorite(this, true, automatic);
        
        _sink.Invoke(automatic ? null : this, new UserFavoritePlaylistsUpdated {
            User = this,
            FavoritePlaylists = IPamelloEntity.GetIds(FavoritePlaylists)
        });
        
        Save();
        
        return playlist;
    }

    public IPamelloPlaylist? RemoveFavoritePlaylist(IPamelloPlaylist playlist, bool fromInside = false, bool automatic = false) {
        if (!_favoritePlaylists.Remove(playlist)) return null;
        
        if (!fromInside) playlist.UnmakeFavorite(this, true, automatic);
        
        _sink.Invoke(automatic ? null : this, new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = IPamelloEntity.GetIds(FavoritePlaylists)
        });
        
        Save();
        
        return playlist;
    }

    public IPamelloPlaylist? MoveFavoritePlaylist(int fromPosition, int toPosition, bool automatic = false) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloPlaylist> ReplaceFavoritePlaylists(List<IPamelloPlaylist> newPlaylists, bool automatic = false) {
        var filteredPlaylists = new List<IPamelloPlaylist>();
        newPlaylists.ForEach(playlist => {
            if (!filteredPlaylists.Contains(playlist)) filteredPlaylists.Add(playlist);
        });
        
        var difference = DifferenceResult<IPamelloPlaylist>.From(_favoritePlaylists, filteredPlaylists, (oldSong, newSong) => oldSong.Id == newSong.Id, true);
        
        foreach (var (_, playlist) in difference.Deleted) playlist.UnmakeFavorite(this, true, automatic);
        foreach (var (_, playlist) in difference.Added) playlist.MakeFavorite(this, true, automatic);
        
        _favoritePlaylists = filteredPlaylists;
        
        _sink.Invoke(automatic ? null : this, new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = IPamelloEntity.GetIds(FavoritePlaylists)
        });
        
        Save();
        
        return FavoritePlaylists;
    }

    public IEnumerable<IPamelloPlaylist> ClearFavoritePlaylists(bool automatic = false) {
        var before = new List<IPamelloPlaylist>(_favoritePlaylists);
        foreach (var playlist in before) {
            playlist.UnmakeFavorite(this, true);
        }
        
        _favoritePlaylists.Clear();
        
        _sink.Invoke(automatic ? null : this, new UserFavoritePlaylistsUpdated() {
            User = this,
            FavoritePlaylists = IPamelloEntity.GetIds(FavoritePlaylists)
        });
        
        Save();
        
        return FavoritePlaylists;
    }

    public string? GetPriorityPlatformKey(string platform) {
        if (SelectedAuthorization?.PK.Platform == platform) return SelectedAuthorization.PK.Key;
        return Authorizations.FirstOrDefault(auth => auth.PK.Platform == platform)?.PK.Key;
    }

    public override PamelloEntityDto GetDto() {
        return new PamelloUserDto() {
            Id = Id,
            Name = Name,
            AvatarUrl = AvatarUrl ?? "",
            SelectedPlayer = SelectedPlayer?.Id ?? 0,
            SelectedAuthorizationIndex = SelectedAuthorizationIndex,
            JoinedAt = JoinedAt,

            AddedSongs = IPamelloEntity.GetIds(AddedSongs),
            AddedPlaylists = IPamelloEntity.GetIds(AddedPlaylists),
            FavoriteSongs = IPamelloEntity.GetIds(FavoriteSongs),
            FavoritePlaylists = IPamelloEntity.GetIds(FavoritePlaylists),

            AuthorizationsPlatformKeys = Authorizations.Select(authorization => authorization.PK.ToString()),

            IsAdministrator = false
        };
    }
}
