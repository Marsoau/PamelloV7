using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Difference;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Edit song associations")]

[AddParagraphInput("NewAssociations", "New Associations")]

public partial class SongEditAssociationsModal
{
    public partial class Builder
    {
        public void Build(IPamelloSong song) {
            NewAssociations.Value = string.Join("\n", song.Associations);
        }
    }
    
    public void Submit(IPamelloSong song) {
        var newAssociations = NewAssociations.Split(',').SelectMany(a => a.Split('\n'));
        
        var differenceResult = DifferenceResult<string>.From(song.Associations, newAssociations, null, true);
        
        foreach (var (_, association) in differenceResult.Added) {
            Output.Write($"+{association}");
            Command<SongAssociationsAdd>().Execute(song, association);
        }
        foreach (var (_, association) in differenceResult.Deleted) {
            Output.Write($"-{association}");
            Command<SongAssociationsRemove>().Execute(song, association);
        }
    }
}
