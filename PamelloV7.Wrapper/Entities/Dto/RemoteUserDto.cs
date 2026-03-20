using System.Text.Json.Serialization;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Containers;
using PamelloV7.Wrapper.Converters;

namespace PamelloV7.Wrapper.Entities.Dto;

public class RemoteUserDto : PamelloEntityDto
{
    public required string AvatarUrl { get; set; }
    public required int SelectedAuthorizationIndex { get; set; }
    public required SafeStoredEntity<RemotePlayer> SelectedPlayer { get; set; }

    public required DateTime JoinedAt { get; set; }

    public required SafeStoredEntities<RemoteSong> AddedSongs { get; set; }
    public required SafeStoredEntities<RemoteSong> FavoriteSongs { get; set; }
    public required SafeStoredEntities<RemotePlaylist> AddedPlaylists { get; set; }
    public required SafeStoredEntities<RemotePlaylist> FavoritePlaylists { get; set; }
        
    public required IEnumerable<string> AuthorizationsPlatformKeys { get; set; }
        
    public required bool IsAdministrator { get; set; }
}
