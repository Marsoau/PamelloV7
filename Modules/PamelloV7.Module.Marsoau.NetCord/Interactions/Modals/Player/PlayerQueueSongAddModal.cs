using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Queue add songs")]

[AddParagraphInput<List<IPamelloSong>>("Songs", "Songs to add")]

public partial class PlayerQueueSongAddModal
{
    public void Submit() {
        Command<PlayerQueueSongAdd>().Execute(Songs);
    }
}
