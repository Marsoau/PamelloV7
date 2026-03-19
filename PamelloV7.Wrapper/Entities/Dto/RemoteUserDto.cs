using System.Text.Json.Serialization;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Converters;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemoteUserDto : PamelloUserDto
{
    public new SafeStoredEntities<RemoteSong> FavoriteSongsIds { get; set; }
}
