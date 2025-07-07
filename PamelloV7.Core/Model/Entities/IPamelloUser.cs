using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloUser : IPamelloEntity
{
    public IPamelloPlayer? PreviousPlayer { get; }
    public IPamelloPlayer? SelectedPlayer { get; set; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }
    
    public IPamelloCommandsModule Commands { get; }
    
    public int SongsPlayed { get; set; }
    
    public ulong DiscordId { get; }
    
    public Guid Token { get; }
    public DateTime JoinedAt { get; }
    
    public IReadOnlyList<IPamelloSong> AddedSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists { get; }
    public IReadOnlyList<IPamelloSong> FavoriteSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists { get; }
    public bool IsAdministrator { get; }

    public void TryLoadLastPlayer();
    public bool TrySelectPlayer(IPamelloPlayer? player);
    public void RequireSelectPlayer(IPamelloPlayer? player);

    public void AddFavoriteSong(IPamelloSong song);
    public void RemoveFavoriteSong(IPamelloSong song);
    public void AddFavoritePlaylist(IPamelloPlaylist song);
    public void RemoveFavoritePlaylist(IPamelloPlaylist song);

    public IPamelloPlaylist CreatePlaylist(string name);
}
