using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloUser : IPamelloDatabaseEntity
{
    public IPamelloPlayer? PreviousPlayer { get; }
    public IPamelloPlayer? SelectedPlayer { get; set; }
    public IPamelloPlayer RequiredSelectedPlayer { get; }
    
    public IPamelloCommandsModule Commands { get; }
    
    public Guid Token { get; }
    
    public DateTime JoinedAt { get; }
    public int SongsPlayed { get; set; }
    
    public ulong DiscordId => 0;
    
    public IReadOnlyList<IPamelloSong> AddedSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> AddedPlaylists { get; }
    public IReadOnlyList<IPamelloSong> FavoriteSongs { get; }
    public IReadOnlyList<IPamelloPlaylist> FavoritePlaylists { get; }

    public void TryLoadLastPlayer();
    public bool TrySelectPlayer(IPamelloPlayer? player);
    public void RequireSelectPlayer(IPamelloPlayer? player);

    public void AddFavoriteSong(IPamelloSong song);
    public void RemoveFavoriteSong(IPamelloSong song);
    public void AddFavoritePlaylist(IPamelloPlaylist song);
    public void RemoveFavoritePlaylist(IPamelloPlaylist song);

    public IPamelloPlaylist CreatePlaylist(string name);
}
