using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.Data.Other;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseUser : DatabaseEntity
{
    public IEnumerable<int> FavoriteSongIds { get; set; }
    public IEnumerable<int> FavoritePlaylistIds { get; set; }
    
    public DateTime JoinedAt { get; set; }
    public int SongsPlayed { get; set; }
    
    public Guid Token { get; set; }
    public IEnumerable<DatabaseUserAuthorization> Authorizations { get; set; }
}
