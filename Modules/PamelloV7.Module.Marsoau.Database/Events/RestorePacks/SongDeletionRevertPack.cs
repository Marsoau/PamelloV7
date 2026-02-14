using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class SongDeletionRevertPack : RevertPack
{
    public DatabaseSong DatabaseSong { get; set; }

    public override void Revert() {
        var songs = (PamelloSongRepository)Services.GetRequiredService<IPamelloSongRepository>();

        Console.WriteLine($"Reverted song deletion, song is restored: {DatabaseSong.Name}");
        
        songs.Restore(DatabaseSong);
    }
}
