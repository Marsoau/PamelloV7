using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserIsAdministratorUpdated : PamelloEvent
    {
        public UserIsAdministratorUpdated() : base(EEventName.UserIsAdministratorUpdated) { }

        public int UserId { get; set; }
        public bool IsAdministrator { get; set; }
    }
}

