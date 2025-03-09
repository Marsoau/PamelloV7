using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserAddedSongsUpdated : PamelloEvent
    {
        public UserAddedSongsUpdated() : base(EEventName.UserAddedSongsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> AddedSongsIds { get; set; }
    }
}

