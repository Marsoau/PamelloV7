using Discord.Audio;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Player;

public partial class Player
{
    [SlashCommand("create", "Get info about a song", runMode: RunMode.Async)]
    public async Task Create(
        [Summary("name", "Name of the player")] string name
    ) {
        var player = Command<PlayerCreate>().Execute(name);

        await RespondUpdatableAsync(() =>
            PamelloComponentBuilders.Info("Player Created & Selected", $"{player.ToDiscordString()} ({player.Queue?.Count})").Build()
        , player);
        
        if (player.Pump is not IAudioModuleWithOutput pump) return;
        
        var vc = Context.Guild.GetUser(Context.DiscordUser.Id).VoiceChannel;
        var ac = await vc.ConnectAsync();
        var stream = ac.CreatePCMStream(AudioApplication.Music);

        pump.Output.ProcessAudio = (audio, wait, cts) => {
            stream.WriteAsync(audio, cts).AsTask().GetAwaiter().GetResult();
            return true;
        };
    }
}
