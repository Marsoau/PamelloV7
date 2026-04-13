using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongAssociationsAdd
{
    public bool Execute(IPamelloSong song, string newAssociation) {
        return song.AddAssociation(newAssociation, ScopeUser);
    }
}
