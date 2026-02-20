using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserAddedPlaylistsUpdated : PamelloEvent
    {
        public UserAddedPlaylistsUpdated() : base(EEventName.UserAddedPlaylistsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> AddedPlaylistsIds { get; set; }
    }
}

