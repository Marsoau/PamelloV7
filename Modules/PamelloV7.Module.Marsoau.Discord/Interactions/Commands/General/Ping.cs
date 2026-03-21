using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.General;

public class Ping : DiscordCommand
{
    [SlashCommand("ping", "Ping the bot")]
    public async Task ExecuteAsync() {
        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Pong!", $"Hi {Context.User.ToDiscordString()}!").Build()
        , Context.User);
    }
}
