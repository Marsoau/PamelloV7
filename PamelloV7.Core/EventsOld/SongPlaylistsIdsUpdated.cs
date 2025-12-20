using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongPlaylistsIdsUpdated : PamelloEvent
    {
        public SongPlaylistsIdsUpdated() : base(EEventName.SongPlaylistsIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> PlaylistsIds { get; set; }
    }
}

