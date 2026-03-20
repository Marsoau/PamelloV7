using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemoteSongDto : PamelloEntityDto
{
    public required string CoverUrl { get; set; }
    public required SafeStoredEntity<RemoteUser> AddedBy { get; set; }
    public required DateTime AddedAt { get; set; }

    public required IEnumerable<string> Associations { get; set; }
    public required SafeStoredEntities<RemoteUser> FavoriteBy { get; set; }
    public required SafeStoredEntities<RemoteEpisode> Episodes { get; set; }
    public required SafeStoredEntities<RemotePlaylist> Playlists { get; set; }

    public required int SelectedSourceIndex { get; set; }
    public required IEnumerable<string> SourcesPlatformKeys { get; set; }
}
