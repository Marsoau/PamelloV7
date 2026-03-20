using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemotePlaylistDto : PamelloEntityDto
{
    public required SafeStoredEntity<RemoteUser> Owner { get; set; }
    public required bool IsProtected { get; set; }
    
    public required SafeStoredEntities<RemoteSong> Songs { get; set; }
    public required SafeStoredEntities<RemoteUser> FavoriteBy { get; set; }
}
