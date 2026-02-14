using PamelloV7.Core.Events.RestorePacks.Base;

namespace PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

public class SongRestoreRevertPack : RevertPack
{
    public int SongId { get; set; }
    
    public override void Revert() {
        Console.WriteLine($"Reverted song restore, song is deleted: {SongId}");
    }
}
