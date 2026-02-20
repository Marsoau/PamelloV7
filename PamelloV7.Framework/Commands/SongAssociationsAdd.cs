using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongAssociationsAdd : PamelloCommand
{
    public bool Execute(IPamelloSong song, string newAssociation) {
        return song.AddAssociation(newAssociation, ScopeUser);
    }
}
