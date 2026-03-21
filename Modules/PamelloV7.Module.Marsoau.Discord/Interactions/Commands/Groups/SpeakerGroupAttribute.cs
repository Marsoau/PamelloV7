using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class SpeakerGroupAttribute : DiscordGroupAttribute {
    public SpeakerGroupAttribute() : base("speaker", "Actions with speakers") { }
}
