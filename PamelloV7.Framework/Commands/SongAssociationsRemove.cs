using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongAssociationsRemove
{
    public bool Execute(IPamelloSong song, string association) {
        return song.RemoveAssociation(association, ScopeUser);
    }
}
