using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Song Rename")]

[AddShortInput("Input11", "First input on modal")]
[AddShortInput("Input22*", "Second input on modal")]

public partial class SongRenameModal
{
    [AddShortInput("Input1*", "First input")]
    [AddShortInput("Input2*", "Second input")]
    [AddShortInput("Input3", "Third input")]

    public partial class Builder
    {
        public void Build() {
            Input11.WithRequired();
        }
    }

    public void Submit() {
        Output.Write($"modal actually submitted, value {Input22}; {Input3}");
    }
}
