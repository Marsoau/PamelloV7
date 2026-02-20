using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongAssociationsRemove : PamelloCommand
{
    public bool Execute(IPamelloSong song, string association) {
        return song.RemoveAssociation(association, ScopeUser);
    }
}
