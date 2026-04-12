using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;

namespace PamelloV7.Framework.Entities;

[PamelloEntity("users", typeof(PamelloUserDto))]
public interface IPamelloUser : IPamelloDatabaseEntity
{
    public string? AvatarUrl { get; }
    
    public IPamelloPlayer? SelectedPlayer { get; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }
    public IPamelloPlayer GuaranteedSelectedPlayer { get; }
    
    public Guid Token { get; }
    
    public DateTime JoinedAt { get; }

    public int SelectedAuthorizationIndex { get; }
    public UserAuthorization? SelectedAuthorization { get; }
    public IReadOnlyList<UserAuthorization> Authorizations { get; }
    
    public IReadOnlyList<IPamelloSong> AddedSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists { get; }
    public IReadOnlyList<IPamelloSong> FavoriteSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists { get; }

    public IPamelloPlayer? SelectPlayer(IPamelloPlayer? player, bool autoSelected = false);
    
    public void SelectAuthorization(int index, bool autoSelected = false);
    
    public Task AddAuthorization(string platform, string key);
    public void AddAuthorizationForced(string platform, string key);

    public IPamelloSong? AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false, bool automatic = false);
    public IPamelloSong? RemoveFavoriteSong(IPamelloSong song, bool fromInside = false, bool automatic = false);
    public IPamelloSong? MoveFavoriteSong(int fromPosition, int toPosition, bool automatic = false);
    public IEnumerable<IPamelloSong> ReplaceFavoriteSongs(List<IPamelloSong> newSongs);
    public IEnumerable<IPamelloSong> ClearFavoriteSongs(bool automatic = false);
    
    public IPamelloPlaylist? AddFavoritePlaylist(IPamelloPlaylist song, int? position = null, bool fromInside = false, bool automatic = false);
    public IPamelloPlaylist? RemoveFavoritePlaylist(IPamelloPlaylist playlist, bool fromInside = false, bool automatic = false);
    public IPamelloPlaylist? MoveFavoritePlaylist(int fromPosition, int toPosition, bool automatic = false);
    public IEnumerable<IPamelloPlaylist> ReplaceFavoritePlaylists(List<IPamelloPlaylist> newPlaylists, bool automatic = false);
    public IEnumerable<IPamelloPlaylist> ClearFavoritePlaylists(bool automatic = false);

    public string? GetPriorityPlatformKey(string platform);
}
