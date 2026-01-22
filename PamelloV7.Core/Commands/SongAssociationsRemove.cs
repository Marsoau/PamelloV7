using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongAssociationsRemove : PamelloCommand
{
    public bool Execute(IPamelloSong song, string association) {
        return song.RemoveAssociation(association);
    }
}
