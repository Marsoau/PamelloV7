using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.InfoUpdate;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class SongDeletionRevertPack : RevertPack<SongDeleted>
{
    public DatabaseSong DatabaseSong { get; set; }

    protected override void RevertInternal(IPamelloUser scopeUser) {
        var songs = (PamelloSongRepository)Services.GetRequiredService<IPamelloSongRepository>();

        Debug.WriteLine($"Reverted song deletion, song is restored: {DatabaseSong.Name}");
        
        songs.Restore(scopeUser, DatabaseSong);
    }
}
