using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.Entities;

[ValueEntity("users")]
public interface IPamelloUser : IPamelloDatabaseEntity
{
    public IPamelloPlayer? PreviousPlayer { get; }
    public IPamelloPlayer? SelectedPlayer { get; set; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }
    
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
    public bool TrySelectPlayer(IPamelloPlayer? player);
    public void RequireSelectPlayer(IPamelloPlayer? player);

    public void AddAuthorization(UserAuthorization authorization);
    
    public IPamelloSong? AddFavoriteSong(IPamelloSong song, int? position = null, bool fromInside = false);
    public IPamelloSong? RemoveFavoriteSong(IPamelloSong song, bool fromInside = false);
    public IPamelloSong? MoveFavoriteSong(int fromPosition, int toPosition);
    public IEnumerable<IPamelloSong> ReplaceFavoriteSongs(List<IPamelloSong> newSongs);
    public IEnumerable<IPamelloSong> ClearFavoriteSongs();
    public void AddFavoritePlaylist(IPamelloPlaylist song, bool fromInside = false);
    public void RemoveFavoritePlaylist(IPamelloPlaylist song, bool fromInside = false);

    public IPamelloPlaylist CreatePlaylist(string name);
}
