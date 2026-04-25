using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Speaker;

[DiscordCommand("speaker internet-connect", "Connect internet speaker to the internet")]
public partial class SpeakerInternetConnect
{
    public async Task Execute(
        string? name = null
    ) {
        var speakerObject = await WithLoadingAsync(
            Command($"SpeakerInternetConnect{(name is null ? "" : $"?name={name}")}")
        );
        if (speakerObject is not IPamelloInternetSpeaker internetSpeaker) throw new PamelloException("Speaker is not internet speaker");

        await RespondAsync("Internet Speaker Connected", () => internetSpeaker);
    }
}
