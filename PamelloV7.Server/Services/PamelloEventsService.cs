using PamelloV7.Server.Model;
using System.Reflection;

namespace PamelloV7.Server.Services
{
    public class PamelloEventsService
    {
        public event Func<PamelloUser, Task>? OnUserCreated;
        public event Func<PamelloUser, Task>? OnUserLoaded;
        public event Func<PamelloUser, Task>? OnUserEdited;
    }
}
