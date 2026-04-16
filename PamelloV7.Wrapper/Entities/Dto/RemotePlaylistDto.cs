using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemotePlaylistDto : PamelloEntityDto
{
    public required Safe<RemoteUser> Owner { get; set; }
    public required bool IsProtected { get; set; }
    
    public required SafeList<RemoteSong> Songs { get; set; }
    public required SafeList<RemoteUser> FavoriteBy { get; set; }
}
