using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.Entities;

[ValueEntity("users")]
public interface IPamelloUser : IPamelloDatabaseEntity
{
    public IPamelloPlayerOld? PreviousPlayer { get; }
    public IPamelloPlayerOld? SelectedPlayer { get; set; }
    public IPamelloPlayerOld RequiredSelectedPlayer { get; }
    
    public Guid Token { get; }
    
    public DateTime JoinedAt { get; }

    public int SelectedAuthorizationIndex { get; set; }
    public UserAuthorization? SelectedAuthorization { get; }
    public IReadOnlyList<UserAuthorization> Authorizations { get; }
    
    public IReadOnlyList<IPamelloSong> AddedSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists { get; }
    public IReadOnlyList<IPamelloSong> FavoriteSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists { get; }

    public void TryLoadLastPlayer();
    public bool TrySelectPlayer(IPamelloPlayerOld? player);
    public void RequireSelectPlayer(IPamelloPlayerOld? player);

    public void AddAuthorization(UserAuthorization authorization);
    
    public IPamelloSong? AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false);
    public IPamelloSong? RemoveFavoriteSong(IPamelloSong song, bool fromInside = false);
    public IPamelloSong? MoveFavoriteSong(int fromPosition, int toPosition);
    public IEnumerable<IPamelloSong> ReplaceFavoriteSongs(List<IPamelloSong> newSongs);
    public IEnumerable<IPamelloSong> ClearFavoriteSongs();
    public IPamelloPlaylist? AddFavoritePlaylist(IPamelloPlaylist song, int? position = null, bool fromInside = false);
    public IPamelloPlaylist? RemoveFavoritePlaylist(IPamelloPlaylist playlist, bool fromInside = false);
    public IEnumerable<IPamelloPlaylist> ReplaceFavoritePlaylists(List<IPamelloPlaylist> newPlaylists);
    public IEnumerable<IPamelloPlaylist> ClearFavoritePlaylists();

    public IPamelloPlaylist CreatePlaylist(string name);
}
