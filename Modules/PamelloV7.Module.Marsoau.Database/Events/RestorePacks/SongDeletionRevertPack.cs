using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.Destructive;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class SongDeletionRevertPack : RevertPack<SongDeleted>
{
    public required DatabaseSong DatabaseSong { get; set; }

    protected override void RevertInternal(IPamelloUser scopeUser) {
        var songs = (PamelloSongRepository)Services.GetRequiredService<IPamelloSongRepository>();

        Debug.WriteLine($"Reverted song deletion, song is restored: {DatabaseSong.Name}");
        
        songs.Restore(scopeUser, DatabaseSong);
    }
}
