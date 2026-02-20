using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.InfoUpdate;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class SongRestoreRevertPack : RevertPack<SongRestored>
{
    protected override void RevertInternal(IPamelloUser scopeUser) {
        if (Event.Song is null) throw new PamelloException("Cannot revert song restore, song is null");
        
        var songs = (PamelloSongRepository)Services.GetRequiredService<IPamelloSongRepository>();
        
        songs.Delete(Event.Song, scopeUser);
        
        Debug.WriteLine($"Reverted song restore, {Event.Song} is deleted");
    }
}
