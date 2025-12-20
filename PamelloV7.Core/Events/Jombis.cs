using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

public class Jombis : IPamelloEvent
{
    public string MessageFromBombis { get; set; }
    public int BombisNumber { get; set; }
    public string Baibyes { get; set; }
}
