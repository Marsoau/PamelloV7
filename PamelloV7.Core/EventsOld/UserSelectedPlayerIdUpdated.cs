using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class UserSelectedPlayerIdUpdated : PamelloEvent
    {
        public UserSelectedPlayerIdUpdated() : base(EEventName.UserSelectedPlayerIdUpdated) { }

        public int UserId { get; set; }
        public int? SelectedPlayerId { get; set; }
    }
}

