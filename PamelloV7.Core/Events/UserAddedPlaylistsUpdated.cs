using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserAddedPlaylistsUpdated : PamelloEvent
    {
        public UserAddedPlaylistsUpdated() : base(EEventName.UserAddedPlaylistsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> AddedPlaylistsIds { get; set; }
    }
}

