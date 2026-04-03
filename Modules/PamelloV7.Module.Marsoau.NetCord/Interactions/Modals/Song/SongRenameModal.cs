using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;


[DiscordModal("Song Rename")]
public partial class SongRenameModal
{
    [AddShortInput("soitsid", "Text input name")]
    [AddShortInput("soitsid2", "Text input name another")]
    [AddShortInput("soitsid3", "Text input name moooore")]

    public partial class Builder;

    public async Task Submit() {
        Output.Write("modal actually submitted");
    }
}
