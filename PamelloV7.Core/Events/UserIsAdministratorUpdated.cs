using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserIsAdministratorUpdated : PamelloEvent
    {
        public UserIsAdministratorUpdated() : base(EEventName.UserIsAdministratorUpdated) { }

        public int UserId { get; set; }
        public bool IsAdministrator { get; set; }
    }
}

