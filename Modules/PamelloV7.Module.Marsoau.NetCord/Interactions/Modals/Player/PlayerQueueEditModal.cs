using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Player;

[DiscordModal("Edit queue")]

[AddParagraphInput<List<IPamelloSong>>("Songs", "Songs to edit")]

public partial class PlayerQueueEditModal
{
    public partial class Builder
    {
        public void Build() {
            Songs.Value = string.Join("\n", Queue?.Songs.Select(s => s.Id) ?? []);
        }
    }
    
    public void Submit() {
        Command<PlayerQueueSongsReplace>().Execute(Songs);
    }
}
