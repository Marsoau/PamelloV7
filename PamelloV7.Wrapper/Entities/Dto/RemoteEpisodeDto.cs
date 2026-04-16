using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemoteEpisodeDto : PamelloEntityDto
{
    public required int Start { get; set; }
    public required bool AutoSkip { get; set; }
    public required Safe<RemoteSong> Song { get; set; }
}
