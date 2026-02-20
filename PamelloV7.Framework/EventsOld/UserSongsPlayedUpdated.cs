using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserSongsPlayedUpdated : PamelloEvent
    {
        public UserSongsPlayedUpdated() : base(EEventName.UserSongsPlayedUpdated) { }

        public int UserId { get; set; }
        public int SongsPlayed { get; set; }
    }
}
