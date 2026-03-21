using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Services;
using PamelloV7.Module.Marsoau.Discord.Speakers;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Speaker;

[SpeakerGroup]
public class Speaker : DiscordCommand
{
    [SlashCommand("connect-internet", "Connect internet speaker to the internet")]
    public async Task ConnectInternet(
        [Summary("name", "Name of the speaker")] string? name = null
    ) {
        var speakerObject = await WithLoadingAsync(
            Command($"SpeakerInternetConnect{(name is null ? "" : $"?name={name}")}")
        );
        if (speakerObject is not IPamelloInternetSpeaker internetSpeaker) throw new PamelloException("Speaker is not internet speaker");

        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Internet Speaker Connected", internetSpeaker.ToDiscordString()).Build()
        , internetSpeaker);
    }
}
