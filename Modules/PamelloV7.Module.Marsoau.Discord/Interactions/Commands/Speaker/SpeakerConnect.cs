using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands;
using PamelloV7.Module.Marsoau.Discord.Services;
using PamelloV7.Module.Marsoau.Discord.Speakers;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Speaker;

public partial class Speaker
{
    [SlashCommand("connect", "Connect discord speaker to a voice channel")]
    public async Task Connect() {
        var speaker = await WithLoadingAsync(
            Command<SpeakerDiscordConnect>().Execute()
        );

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.Info("Speaker Connected", speaker.ToDiscordString()).Build()
        , speaker);
    }
}
