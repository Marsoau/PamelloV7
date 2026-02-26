using System.Text.Json;
using Discord.Interactions;
using PamelloV7.Framework.Converters;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Debug;

public partial class Debug : DiscordCommand
{
    [SlashCommand("get-token", "Get your user token", runMode: RunMode.Async)]
    public async Task GetToken() {
        await RespondUpdatableAsync(() =>
            Builder<BasicComponentsBuilder>().Info("Your Token", DiscordString.Spoiler(ScopeUser.Token)).Build()
        );
    }
}
