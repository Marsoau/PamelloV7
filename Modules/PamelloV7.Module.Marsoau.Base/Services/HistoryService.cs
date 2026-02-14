using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class HistoryService : IHistoryService
{
    public void Record(IPamelloEvent e) {
        Console.WriteLine($"Record event: {e.GetType().Name}");
    }

    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent) {
        Console.WriteLine($"Record nested event: {nestedEvent.GetType().Name}; in parent event: {parentEvent.GetType().Name}");
    }
}
