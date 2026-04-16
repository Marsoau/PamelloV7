using PamelloV7.Core.Audio;
using PamelloV7.Framework.Commands;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Rewind current song")]

[AddShortInput<AudioTime>("Time*", "Time")]

public partial class PlayerQueueRewindModal
{
    public partial class Builder
    {
        public void Build() {
            Time.Value = Queue?.CurrentSongTimePosition.ToString() ?? AudioTime.Zero.ToString();
        }
    }
    
    public void Submit() {
        Command<PlayerQueueCurrentSongRewind>().Execute(Time);
    }
}
