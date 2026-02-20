using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserAddedSongsUpdated : PamelloEvent
    {
        public UserAddedSongsUpdated() : base(EEventName.UserAddedSongsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> AddedSongsIds { get; set; }
    }
}

