using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongAssociationsAdd : PamelloCommand
{
    public bool Execute(IPamelloSong song, string newAssociation) {
        return song.AddAssociation(newAssociation);
    }
}
