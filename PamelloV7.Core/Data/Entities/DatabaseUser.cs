using PamelloV7.Core.Data.Entities.Base;
using PamelloV7.Core.Data.Other;

namespace PamelloV7.Core.Data.Entities;

public class DatabaseUser : DatabaseEntity
{
    public List<int> FavoriteSongIds { get; set; }
    public List<int> FavoritePlaylistIds { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    public Guid Token { get; set; }
    
    //Authorization index to get profile info from
    public int SelectedAuthorization { get; set; }
    
    //key - Service, value - User
    public Dictionary<string, string> Authorizations { get; set; }
}
