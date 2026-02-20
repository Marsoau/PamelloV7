using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongPlaylistsIdsUpdated : PamelloEvent
    {
        public SongPlaylistsIdsUpdated() : base(EEventName.SongPlaylistsIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> PlaylistsIds { get; set; }
    }
}

