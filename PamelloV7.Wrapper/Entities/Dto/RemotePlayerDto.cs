using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Entities.Dto.Other;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemotePlayerDto : PamelloEntityDto
{
    public required Safe<RemoteUser> Owner { get; set; }
    public required bool IsProtected { get; set; }
    public required bool IsPaused { get; set; }
    public required RemoteQueueDto Queue { get; set; }
}
