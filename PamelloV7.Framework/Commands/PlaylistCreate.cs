using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Framework.Commands;

public class PlaylistCreate : PamelloCommand
{
    public IPamelloPlaylist Execute(string name) {
        var playlists = Services.GetRequiredService<IPamelloPlaylistRepository>();
        var playlist = playlists.Add(name, ScopeUser);
        
        return playlist;
    }
}
