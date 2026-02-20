using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserSelectedPlayerIdUpdated : PamelloEvent
    {
        public UserSelectedPlayerIdUpdated() : base(EEventName.UserSelectedPlayerIdUpdated) { }

        public int UserId { get; set; }
        public int? SelectedPlayerId { get; set; }
    }
}

